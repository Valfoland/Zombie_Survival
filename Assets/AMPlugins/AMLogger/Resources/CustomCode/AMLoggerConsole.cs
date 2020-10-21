using System;
using System.Collections.Generic;
using UnityEngine;

namespace AMLogging
{
	/// <summary>
	/// Консоль для отображения Unity логов в игре.
	/// </summary>
	public class AMLoggerConsole : MonoBehaviour
	{
		public static AMLoggerConsole Instance = null;

		enum WindowSize
		{
			FullScreen,
			HalfSize
		}

		public struct Log
		{
			public string timestamp;
			public string message;
			public string stackTrace;
			public LogType type;
			public int logCount;
		}

		#region Inspector Settings
		/// <summary>
		/// Чекбокс для включения/отключения консоли.
		/// </summary>
		public bool enableConsole = false;

		/// <summary>
		/// Размер шрифта для отображаемых логов.
		/// </summary>
		public int fontSize = 25;

		/// <summary>
		/// Чекбокс для включения/отключения отображения времени в логах.
		/// </summary>
		public bool useTimestamp = false;

		/// <summary>
		/// Облегченный вариант отображения консоли.
		/// Выводит только сообщения, без окна.
		/// </summary>
		public bool lightweight = false;

		/// <summary>
		/// Горячая клавиша для отображения/скрытия консоли.
		/// </summary>
		public KeyCode toggleKey = KeyCode.BackQuote;

		/// <summary>
		/// Сила, с которой следует встряхнуть девайс для отображения/скрытия консоли.
		/// </summary>
		public float shakeAcceleration = 5f;
		#endregion

		public static bool customCodeEnable = false;
		public static bool customCodeIsInit = false;
		public static bool ccEnableConsole;
		public static int ccFontSize;
		public static bool ccUseTimestamp = false;
		public static bool ccLightweightConsole;
		public static KeyCode ccToggleKey;
		public static float ccShakeAcceleration;

		public static List<Log> logs = null;
		public static List<Log> collapsedLogs = null;

		TextAsset checkboxTrueAsset;
		TextAsset checkboxFalseAsset;

		static Texture2D checkboxTrueImage;
		static Texture2D checkboxFalseImage;
		
		Vector2 scrollPosition;
		bool visible;
		bool isCollapsed;
		bool showStackTrace;
		string isCollapsedLebel;

		WindowSize windowSize = WindowSize.FullScreen;

		const string windowTitle = "AMLoggerConsole";
		const int margin = 20;
		static readonly GUIContent clearButtonGUIContent = new GUIContent ("Clear", "Clear the contents of the console.");
		static readonly GUIContent stackTraceButtonGUIContent = new GUIContent ("StackTrace", "Show log stack traces.");
		static readonly GUIContent collapseButtonGUIContent = new GUIContent ("Collapse", "Hide repeated messages.");
		
		readonly Rect titleBarRect = new Rect (0, 0, 10000, Screen.width / 20);
		Rect windowRect;
		float bannerSpace = 0f;

		void Init ()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad (this);

				if (customCodeEnable) 
				{
					enableConsole = ccEnableConsole;
					fontSize = ccFontSize;
					useTimestamp = ccUseTimestamp;
					lightweight = ccLightweightConsole;
					toggleKey = ccToggleKey;
					shakeAcceleration = ccShakeAcceleration;
				}

				if (enableConsole) 
				{
					Application.logMessageReceived += AMLogger.Logger.HandleLog;
					visible = true;
				} 
				else 
				{
					Application.logMessageReceived -= AMLogger.Logger.HandleLog;
				}
			}
			else
			{
				Destroy (gameObject);
			}	
		}

		void Start ()
		{
			Init ();

			checkboxFalseImage = new Texture2D (200, 200);
			checkboxTrueImage = new Texture2D (200, 200);

			string imagePath = "Image/";

			checkboxFalseAsset = Resources.Load (imagePath+"checkbox_false") as TextAsset;
			checkboxTrueAsset = Resources.Load (imagePath+"checkbox_true") as TextAsset;

			checkboxFalseImage.LoadImage (checkboxFalseAsset.bytes);
			GetComponent<Renderer> ().material.mainTexture = checkboxFalseImage;

			checkboxTrueImage.LoadImage (checkboxTrueAsset.bytes);
			GetComponent<Renderer> ().material.mainTexture = checkboxTrueImage;
		}

		void Update ()
		{
			if (customCodeEnable) 
			{
				if (enableConsole != ccEnableConsole)
					enableConsole = ccEnableConsole;
				if (fontSize != ccFontSize)
					fontSize = ccFontSize;
				if (useTimestamp != ccUseTimestamp)
					useTimestamp = ccUseTimestamp;
				if (lightweight != ccLightweightConsole)
					lightweight = ccLightweightConsole;
				if (toggleKey != ccToggleKey)
					toggleKey = ccToggleKey;
				if (shakeAcceleration != ccShakeAcceleration)
					shakeAcceleration = ccShakeAcceleration;
			}
			if (enableConsole) 
			{
				if (Input.GetKeyDown (toggleKey) || Input.acceleration.sqrMagnitude > shakeAcceleration)
				{
					visible = !visible;
					if (visible)
					{
						Application.logMessageReceived += AMLogger.Logger.HandleLog;
					}
					else
					{
						Application.logMessageReceived -= AMLogger.Logger.HandleLog;
					}
				}
			} 
			else 
			{
				if (visible)
					visible = false;
				if (logs != null)
					logs = null;
				if (collapsedLogs != null)
					collapsedLogs = null;
			}
		}

		void OnGUI ()
		{
			if (!visible) 
			{
				return;
			}

			if (lightweight)
			{
				DrawLogs ();
			}
			else
			{
				if (windowRect.height == 0)
					windowRect = new Rect (margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));

				if (Screen.height > Screen.width)
				{
					bannerSpace = windowRect.height / 15;
					windowRect.height = windowSize == WindowSize.FullScreen ? Screen.height - (margin * 2) - bannerSpace : (Screen.height / 2) - (margin * 2);
				}
				else
				{
					bannerSpace = windowRect.width / 15;
					windowRect.width = windowSize == WindowSize.FullScreen ? Screen.width - (margin * 2) : (Screen.width / 2) - (margin * 2);
					windowRect.height = Screen.height - (margin * 2) - bannerSpace;
				}

				windowRect = GUILayout.Window (123456, windowRect, ConsoleWindow, windowTitle);
			}
		}

		GUIStyle labelStyle;
		void DrawLogs ()
		{
			if (logs == null)
				logs = new List<AMLoggerConsole.Log> ();
			if (collapsedLogs == null)
				collapsedLogs = new List<AMLoggerConsole.Log> ();

			List<Log> displayedLogs = isCollapsed ? collapsedLogs : logs;

			scrollPosition = GUILayout.BeginScrollView (scrollPosition);

			for (int i = 0; i < displayedLogs.Count; i++)
			{
				Log currentLog = displayedLogs [i];

				GUILayout.BeginHorizontal ();

				Color logColor = Color.white;
				if (currentLog.type == LogType.Error || currentLog.type == LogType.Exception)
				{
					logColor = Color.red;
				}
				else if (currentLog.type == LogType.Warning)
				{
					logColor = Color.yellow;
				}
				else
				{
					logColor = Color.white;
				}

				if (labelStyle == null)
				{
					labelStyle = GUI.skin.GetStyle ("label");
					labelStyle.fontSize = fontSize;
				}
				labelStyle.normal.textColor = logColor;

				if (showStackTrace)
				{
					if (lightweight)
						GUILayout.Label (currentLog.stackTrace, labelStyle, GUILayout.Width (Screen.width / 2));
					else
						GUILayout.Label (currentLog.stackTrace, labelStyle);
				}
				else
				{
					string message = !isCollapsed && useTimestamp ? currentLog.timestamp + currentLog.message : currentLog.message;

					if (lightweight)
						GUILayout.Label (message, labelStyle, GUILayout.Width (Screen.width / 2));
					else
						GUILayout.Label (message, labelStyle);
				}
				if (isCollapsed)
				{
					float countBoxSize = windowRect.width / 20;
					GUILayout.Box (currentLog.logCount.ToString (), labelStyle, new GUILayoutOption[] {
						GUILayout.ExpandWidth (false),
						GUILayout.MinHeight (countBoxSize),
						GUILayout.MinWidth (countBoxSize)
					});
				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndScrollView ();
		}

		GUIStyle smallerButtonStyle;
		GUIStyle biggerButtonStyle;
		void ConsoleWindow (int windowID)
		{
			if (visible)
			{
				DrawLogs ();
			
				GUI.contentColor = Color.white;
			
				GUILayout.BeginHorizontal ();

				float buttonHeight = windowRect.width / 15;

				if (smallerButtonStyle == null)
				{
					smallerButtonStyle = new GUIStyle (GUI.skin.button);
				}
				smallerButtonStyle.fontSize = (int)(buttonHeight / 2);
				if (biggerButtonStyle == null)
				{
					biggerButtonStyle = new GUIStyle (GUI.skin.button);
				}
				biggerButtonStyle.fontSize = (int)(buttonHeight / 2);

				bool isFullscreen = windowSize == WindowSize.FullScreen;
				string resizeButtonLabel = isFullscreen ? "FS" : "1/2";
				isFullscreen = GUILayout.Toggle (isFullscreen, resizeButtonLabel, smallerButtonStyle, 
					new GUILayoutOption[] {
						GUILayout.ExpandWidth (false), 
						GUILayout.MinHeight (buttonHeight), 
						GUILayout.MinWidth (buttonHeight),
						GUILayout.MaxHeight (buttonHeight),
						GUILayout.MaxWidth (buttonHeight)
					});
				windowSize = isFullscreen ? WindowSize.FullScreen : WindowSize.HalfSize;

				Texture collapseCheckBoxImage = isCollapsed ? checkboxTrueImage : checkboxFalseImage;
				GUIContent collapseGuiContent = new GUIContent (" " + collapseButtonGUIContent.text, collapseCheckBoxImage, collapseButtonGUIContent.tooltip);
				isCollapsed = GUILayout.Toggle (isCollapsed, collapseGuiContent, biggerButtonStyle, 
					new GUILayoutOption[] { 
						GUILayout.ExpandWidth (false), 
						GUILayout.MinHeight (buttonHeight), 
						GUILayout.MinWidth (buttonHeight), 
						GUILayout.MaxHeight (buttonHeight)
					});
			
				showStackTrace = GUILayout.Toggle (showStackTrace, stackTraceButtonGUIContent, biggerButtonStyle, new GUILayoutOption[] {
					GUILayout.ExpandWidth (false),
					GUILayout.MinHeight (buttonHeight),
					GUILayout.MaxHeight (buttonHeight)
				});

				if (GUILayout.Button (clearButtonGUIContent, biggerButtonStyle, new GUILayoutOption[] {
					GUILayout.ExpandWidth (true),
					GUILayout.MinHeight (buttonHeight),
					GUILayout.MaxHeight (buttonHeight)
				}))
				{
					ClearLogs ();
				}

				GUILayout.EndHorizontal ();
			
				// Окно можно переместить за область заголовка.
				GUI.DragWindow (titleBarRect);
			}
		}

		public static void ClearLogs ()
		{
			if (logs != null)
				logs.Clear ();
			if (collapsedLogs != null)
				collapsedLogs.Clear ();
		}

		void OnDestroy ()
		{
//			logs = null;
//			collapsedLogs = null;
		}
	}
}