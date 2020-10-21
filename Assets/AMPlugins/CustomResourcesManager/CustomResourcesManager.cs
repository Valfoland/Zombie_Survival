#pragma warning disable
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using AMConfigsParser;
using AMEvents;
using AMLogging;
using CustomQualitySetup;

namespace CustomCode
{
	/// <summary>
	/// Инструмент для загрузки кастомных скриптов из ресурсов
	/// </summary>
	public class CustomResourcesManager : MonoBehaviour
	{
		/// <summary>
		/// Имя папки для кастомных скриптов
		/// </summary>
		const string CUSTOM_ITEM_PATH = "CustomCode";

		public string customCodeSceneName = "SceneCustomCode";

		string pluginsNames = string.Empty;

		#region Inspector Settings
		/// <summary>
		/// The checkbox to enable/disable automatic loading next scene after Custom Code initialized.
		/// </summary>
		public bool shouldLoadNextScene = true;

		/// <summary>
		/// The checkbox to enable/disable debug menu for all plugins.
		/// </summary>
		public bool debugMenu = false;

		#if UNITY_TVOS
		/// <summary>
		/// The checkbox to enable/disable controller handling debug menu for all plugins.
		/// </summary>
		public bool handleController = false;
		#endif

		/// <summary>
		/// The name of the next scene to load (first scene from asset bundle).
		/// </summary>
		public string assetBundlesNextSceneName = string.Empty;

		/// <summary>
		/// The checkbox to enable/disable debug button for AnalytiAll plugin.
		/// </summary>
		public bool analytiAllDebugMode = false;

		/// <summary>
		/// The checkbox to enable/disable auto interstitial mode for AM Events plugin.
		/// </summary>
		public bool amEventsAutoInterstitial = true;
		/// <summary>
		/// The list of scenes for AMEvents AutoInterstitial.
		/// </summary>
		public List<AMEvents.AMEvents.AutoInterstitialScene> amEventsAutoInterstitialScenes;
		/// <summary>
		/// The checkbox to enable/disable cross ad flag.
		/// </summary>
		public bool amEventsCrossAd = false;
		/// <summary>
		/// The am events start screen.
		/// </summary>
		public bool amEventsAdAtStart = true;
		/// <summary>
		/// The am events start screen opacity.
		/// </summary>
		public float amEventsStartScreenOpacity = 0.95f;
		/// <summary>
		/// The checkbox to enable/disable debug mode for AM Events plugin.
		/// </summary>
		public bool amEventsDebugMode = false;
		/// <summary>
		/// The checkbox to enable/disable an automatic pause and resume app for fuulscreen ads and inner inapp.
		/// </summary>
		public bool amEventsAutoPause = false;
		/// <summary>
		/// Minimum time delay between showing ads in seconds.
		/// </summary>
		public int amEventsInterstitialDelay = 60;
		
		/// <summary>
		/// The checkbox to enable/disable debug mode with the console window.
		/// </summary>
		public bool enableLogConsole = false;
		/// <summary>
		/// The size of the console text font.
		/// </summary>
		public int consoleFontSize = 25;
		/// <summary>
		/// The use timestamp.
		/// </summary>
		public bool useTimestamp = false;
		/// <summary>
		/// The console repaint.
		/// </summary>
		public bool consoleRepaint = false;
		/// <summary>
		/// The color of the console text.
		/// </summary>
		public Color consoleTextColor = Color.white;
		/// <summary>
		/// The hotkey to show and hide the console window.
		/// </summary>
		public KeyCode consoleToggleKey = KeyCode.BackQuote;
		/// <summary>
		/// The (squared) acceleration above which the window should open.
		/// </summary>
		public float consoleShakeAcceleration = 5f;

		/// <summary>
		/// Custom Mode param for Custom Quality Setup.
		/// </summary>
		public bool cqsCustomMode = false;
		#endregion
		public static CustomResourcesManager Instance;
		static bool isLoadScript = false;
		public static bool isInit = false;
		List<string> classes;
		public bool enableAssetBundles = false;
		public bool enableAnalytiAll = true;
		public bool enableAMEvents = true;
		public bool enableAMLogger = true;
		public bool enableCustomQualitySetup = true;

		public static event Action EndInit;

		public static AMLogger amLogger = AMLogger.GetInstance ("CustomResourceManager: ");

		GameObject amEventsGO = null;

		/// <summary>
		/// Raises the application inactive event.
		/// </summary>
		public void OnApplicationInactive ()
		{
			AMEvents.AMEvents.OnApplicationInactive ();
		}
		/// <summary>
		/// Awake this instance.
		/// </summary>
		void Awake ()
		{
			Init ();
		}
		/// <summary>
		/// Init this instance.
		/// </summary>
		void Init ()
		{
			if (Instance == null) 
			{
				Instance = this;			
				DontDestroyOnLoad (this);

				new GameObject ("AMConfigsParser").AddComponent (typeof (AMParseJsonConfig));
				AMParseJsonConfig.isParseEnd (LoadPlugin);

				pluginsNames += "    AM Configs Parser\n";

                AMLoggerConsole.customCodeEnable = true;
				AMEvents.AMEvents.customCodeEnable = true;
				AnalytiAll.EventManager.customCodeEnable = true;

				CustomQualitySetup.CustomQualitySetup.customCodeEnable = true;

				ThrowParameters ();
			}
			else
			{
				Destroy (gameObject);
			}
		}

		static bool isLoading = false;
		/// <summary>
		/// Loads the plugin.
		/// </summary>
		void LoadPlugin ()
		{
			if (isLoading)
				return;

			isLoading = true;

			amEventsAutoInterstitial = !AMProjectInfoInside.disableAutoInterstitial;
			amEventsCrossAd = AMProjectInfoInside.crossAd;
			amEventsAdAtStart = AMProjectInfoInside.advertisingAtTheStart;
			amEventsStartScreenOpacity = AMProjectInfoInside.startScreenOpacity;

			if (AMProjectInfoInside.enableAMLoggerConsole != null)
				enableLogConsole = true;

			debugMenu = AMProjectInfoInside.customCodeDebug == string.Empty ? debugMenu : Convert.ToBoolean (AMProjectInfoInside.customCodeDebug);

			if (AMProjectInfoInside.debugModeForPlugins != null)
			{
				#if UNITY_EDITOR_OSX || !(UNITY_WSA_10_0 && NETFX_CORE)
				var classVariables = this.GetType ().GetFields ().ToList ();

				foreach (var plugin in AMProjectInfoInside.debugModeForPlugins)
				{
					try
					{
						classVariables.Find ((p) => {return p.Name.ToLower ().Contains (plugin.ToString ().ToLower () + "debugmode");}).SetValue (Instance, true);
					}
					catch (Exception)
					{}
				}
				#else
				if (AMProjectInfoInside.debugModeForPlugins.Contains ("amevents"))
				{
					amEventsDebugMode = true;
				}
				if (AMProjectInfoInside.debugModeForPlugins.Contains ("analytiall"))
				{
					analytiAllDebugMode = true;
				}
				#endif
			}

			if (amEventsAutoInterstitial && AMProjectInfoInside.autoInterstitialScenes != null)
			{
				amEventsAutoInterstitialScenes = AMEvents.AMEvents.AutoInterstitialScene.UpdateList (amEventsAutoInterstitialScenes);
			}

			LoadClassesByResources ();

			MakeItems ();

			if (isLoadScript)
				return;
			isLoadScript = true;
			
			if ((AMProjectInfoInside.eventConsole == true) & (AMBuildParamsInside.build_type == "test")) {
				StartCoroutine(CoroutineStartApp());
			}
			else {
				StartApp();
			}
			isLoading = false;
		}
		IEnumerator CoroutineStartApp()
		{    
			yield return new WaitForSeconds(0.2f);
				StartApp ();
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update ()
		{
			ThrowParameters ();
		}

		/// <summary>
		/// Проброс параметров с префаба CustomResourcesManager на подключенные плагины.
		/// </summary>
		void ThrowParameters ()
		{
			if (CustomResourcesManagerAssetBundles.nextSceneName != assetBundlesNextSceneName)
				CustomResourcesManagerAssetBundles.nextSceneName = assetBundlesNextSceneName;
			//AnalytiAll Debug
			if (AnalytiAll.EventManager.ccDebugMode != analytiAllDebugMode)
				AnalytiAll.EventManager.ccDebugMode = analytiAllDebugMode;
			//AM Events Debug
			if (AMEvents.AMEvents.ccDebugMode != amEventsDebugMode)
				AMEvents.AMEvents.ccDebugMode = amEventsDebugMode;
			if (AMEvents.AMEvents.ccAutoInterstitial != amEventsAutoInterstitial)
				AMEvents.AMEvents.ccAutoInterstitial = amEventsAutoInterstitial;
			if (AMEvents.AMEvents.ccAutoInterstitialScenes != amEventsAutoInterstitialScenes)
				AMEvents.AMEvents.ccAutoInterstitialScenes = amEventsAutoInterstitialScenes;
			if (AMEvents.AMEvents.ccCrossAd != amEventsCrossAd)
				AMEvents.AMEvents.ccCrossAd = amEventsCrossAd;
			if (AMEvents.AMEvents.ccAdAtStart != amEventsAdAtStart)
				AMEvents.AMEvents.ccAdAtStart = amEventsAdAtStart;
			if (AMEvents.AMEvents.ccStartScreenOpacity != amEventsStartScreenOpacity)
				AMEvents.AMEvents.ccStartScreenOpacity = amEventsStartScreenOpacity;
			if (AMEvents.AMEvents.ccAutoPause != amEventsAutoPause)
				AMEvents.AMEvents.ccAutoPause = amEventsAutoPause;
			//AM Logger Console
			if (AMLoggerConsole.ccEnableConsole != enableLogConsole)
				AMLoggerConsole.ccEnableConsole = enableLogConsole;
			if (AMLoggerConsole.ccFontSize != consoleFontSize)
				AMLoggerConsole.ccFontSize = consoleFontSize;
			if (AMLoggerConsole.ccToggleKey != consoleToggleKey)
				AMLoggerConsole.ccToggleKey = consoleToggleKey;
			if (AMLoggerConsole.ccShakeAcceleration != consoleShakeAcceleration)
				AMLoggerConsole.ccShakeAcceleration = consoleShakeAcceleration;
			//Custom Quality Setup
			if (CustomQualitySetup.CustomQualitySetup.ccCustomMode != cqsCustomMode)
				CustomQualitySetup.CustomQualitySetup.ccCustomMode = cqsCustomMode;
		}
		/// <summary>
		/// Ises the init plugin.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public static void isInitPlugin (Action callback)
		{
			if (isInit)
				callback ();
			else
				EndInit += callback;
		}

		/// <summary>
		/// Starts the app.
		/// </summary>
		void StartApp ()
		{
			try
			{
				if (shouldLoadNextScene && (!enableAssetBundles || string.IsNullOrEmpty (assetBundlesNextSceneName)))
					LoadNextScene ();
			}
			catch (Exception)
			{}
			if (EndInit != null)
				EndInit ();

			isInit = true;
			AMLoggerConsole.customCodeIsInit = true;
		}
		void LoadNextScene ()
		{
			if (SceneManager.GetActiveScene ().name == customCodeSceneName)
				SceneManager.LoadScene (1);
		}
		/// <summary>
		/// Загрузка кастомных классов из ресурсов
		/// </summary>
		void LoadClassesByResources ()
		{
			classes = new List<string> ();
			UnityEngine.Object[] objects = Resources.LoadAll (CUSTOM_ITEM_PATH, typeof (TextAsset));
			for (int i = 0; i < objects.Length; i++)
			{
				var obj = objects [i];
				TextAsset ta = (TextAsset)obj;
				classes.Add (ta.name);
			}

			int loggerIndex = classes.FindIndex (cl => {
				return cl.Equals ("AMLoggerConsole");
			});
			if (loggerIndex != -1)
			{
				var loggerClass = classes [loggerIndex];
				classes.RemoveAt (loggerIndex);
				classes.Add (loggerClass);
			}
		}
		
		/// <summary>
		/// Добавление всех кастомных скриптов на текущий объект
		/// </summary>
		void MakeItems ()
		{
			if (enableAssetBundles)
			{
				gameObject.AddComponent<CustomCode.CustomResourcesManagerAssetBundles> ();
			}

			foreach (string type in classes)
			{
				try
				{
					switch (type)
					{
					case "AMEvents":
						if (enableAMEvents) 
						{
							var AMEvents = new GameObject ("AMEvents");
							AMEvents.AddComponent<AMEvents.AMEvents> ();
							amEventsGO = Instantiate (AMEvents);
							pluginsNames += "    AM Events\n";
						}
						break;
					case "CustomQualitySetup":
						if (enableCustomQualitySetup) 
						{
							var CustomQualitySetupGO = new GameObject ("CustomQualitySetup");
							CustomQualitySetupGO.AddComponent<CustomQualitySetup.CustomQualitySetup> ();
							Instantiate (CustomQualitySetupGO);
							pluginsNames += "    Custom Quality Setup\n";
						}
						break;
					case "EventManager":
						if (enableAnalytiAll) 
						{
							var AnalytiAllGO = new GameObject ("AnalytiAll");
							AnalytiAllGO.AddComponent<AnalytiAll.EventManager> ();
							Instantiate (AnalytiAllGO);
							pluginsNames += "    AnalytiAll\n";
						}
						break;
					case "AMLoggerConsole":
						if (enableAMLogger) 
						{
							var AMLoggerConsole = new GameObject ("AMLogger");
							AMLoggerConsole.AddComponent<AMLoggerConsole> ();
							AMLoggerConsole.AddComponent<SpriteRenderer> ();
							Instantiate (AMLoggerConsole);
							pluginsNames += "    AM Logger\n";
						}
						break;
					default:
						break;
					}
				}
				catch (Exception e)
				{
					#if UNITY_STANDALONE_OSX
					Debug.LogException (e);
					#else
					if (amLogger != null)
						amLogger.LogException (e);
					#endif
				}
			}
			pluginsNames = pluginsNames.Remove (pluginsNames.Length - 1);
			#if UNITY_STANDALONE_OSX
			Debug.Log ("CustomResourceManager: Starts with plugins:\n" + pluginsNames);
			#else
			if (amLogger != null)
				amLogger.Log ("Starts with plugins:\n" + pluginsNames);
			#endif
		}

		public enum DebugMenuState
		{
			Main = 1,
			AmEvents,
			AnalytiAll
		}

		DebugMenuState debugMenuState = DebugMenuState.Main;
		GUIStyle selectedButtonStyle = null;
		Rect MenuRect;
		int gridX = 3;
		int gridY = 6;
		int dx;
		int dy;
		int btnIdx = -1;

		void OnGUI ()
		{
			if (debugMenu)
			{
				if (selectedButtonStyle == null)
				{
					selectedButtonStyle = new GUIStyle (GUI.skin.button);
					selectedButtonStyle.normal.background = selectedButtonStyle.hover.background;
				}

				#if UNITY_TVOS
				RemoteControllerHandler ();
				#endif

				switch (debugMenuState)
				{
				case DebugMenuState.Main:
					if (!analytiAllDebugMode)
						analytiAllDebugMode = true;
					if (!amEventsDebugMode)
						amEventsDebugMode = true;

					dx = Screen.width / (2 * (gridX));
					dy = Screen.height / (gridY + 2);

					MenuRect = new Rect (Screen.width / 2, 0, gridX * dx, dy * 5);

					GUIStyle menuTextStyle = new GUIStyle (GUI.skin.box);
					menuTextStyle.fontSize = (MenuRect.height < MenuRect.width) ? (int)(MenuRect.height / 18) : (int)(MenuRect.width / 18);
					menuTextStyle.alignment = TextAnchor.UpperLeft;

					GUI.Box (MenuRect, "Custom Code Debug Menu\nPlugin: Custom Code\nCurrent Position: Plugins", menuTextStyle);

					if (enableAnalytiAll)
					{
						GUIStyle selectedAnalytiAllButtonStyle = btnIdx == 0 ? selectedButtonStyle : new GUIStyle (GUI.skin.button);
						if (GUI.Button (new Rect (MenuRect.x, MenuRect.y + dy, dx, dy), "AnalytiAll", selectedAnalytiAllButtonStyle))
						{
							debugMenuState = DebugMenuState.AnalytiAll;
							AnalytiAll.EventManager.ccDebugMenu = true;
						}
					}
					if (enableAMEvents)
					{
						GUIStyle selectedAMEventsButtonStyle = btnIdx == 1 ? selectedButtonStyle : new GUIStyle (GUI.skin.button);
						if (GUI.Button (new Rect (MenuRect.x + dx, MenuRect.y + dy, dx, dy), "AMEvents", selectedAMEventsButtonStyle))
						{
							debugMenuState = DebugMenuState.AmEvents;
							AMEvents.AMEvents.ccDebugMenu = true;
						}
					}

					if (!enableAnalytiAll && !enableAMEvents)
					{
						GUIStyle messageStyle = new GUIStyle (GUI.skin.label);
						messageStyle.fontSize = menuTextStyle.fontSize;
						messageStyle.alignment = TextAnchor.UpperCenter;

						GUI.Label (new Rect (MenuRect.x, dy * 2, MenuRect.width, dy * 4), "No compatible plugins enabled", messageStyle);
					}
					break;
				case DebugMenuState.AnalytiAll:
					AnalytiAll.EventManager.Instance.DrawDebugMenu ();

					DrawExitButton ();
					break;
				case DebugMenuState.AmEvents:
					AMEvents.AMEvents.Instance.DrawDebugMenu ();

					DrawExitButton ();
					break;
				default:
					break;
				}
			}
		}

		#if UNITY_TVOS
		void RemoteControllerHandler ()
		{
			if (!handleController)
				return;

			UnityEngine.Apple.TV.Remote.allowExitToHome = debugMenuState == DebugMenuState.Main ? true : false;

			btnIdx = btnIdx == -1 ? 0 : btnIdx;

			if (debugMenuState == DebugMenuState.Main)
			{
				float tempAxisH = Input.GetAxis ("Horizontal");
				if (tempAxisH != 0)
					btnIdx = tempAxisH > 0 ? 1 : 0;
			}

			float tempAxisV = Input.GetAxis ("Vertical");
			if (tempAxisV != 0)
			{
				if (tempAxisV > 0)
				{
					if (debugMenuState == DebugMenuState.Main)
						btnIdx = btnIdx == 100 ? 0 : btnIdx;
					else
						btnIdx = 0;
				}
				if (tempAxisV < 0)
				{
					if (debugMenuState != DebugMenuState.Main)
						btnIdx = (enableAMEvents && AMEvents.AMEvents.Instance.btnIdxOut == "down") || (enableAnalytiAll && AnalytiAll.EventManager.Instance.btnIdxOut == "down") ? 100 : 0;
					else
						btnIdx = 0;
				}
			}

			if (Input.GetKeyUp (KeyCode.JoystickButton14) || Input.GetKeyUp (KeyCode.Return))
			{
				if (debugMenuState == DebugMenuState.Main)
				{
					if (btnIdx == 0 && enableAnalytiAll)
					{
						debugMenuState = DebugMenuState.AnalytiAll;
						AnalytiAll.EventManager.ccDebugMenu = true;
						AnalytiAll.EventManager.ccHandleController = true;
						AnalytiAll.EventManager.Instance.exitFromMenu += ExitToMainMenu;
					}
					if (btnIdx == 1 && enableAMEvents)
					{
						debugMenuState = DebugMenuState.AmEvents;
						AMEvents.AMEvents.ccDebugMenu = true;
						AMEvents.AMEvents.ccHandleController = true;
						AMEvents.AMEvents.Instance.exitFromMenu += ExitToMainMenu;
					}
				}
				if (btnIdx == 100)
					ExitToMainMenu ();
			}
		}
		#endif

		void DrawExitButton ()
		{
			GUIStyle selectedExitButtonStyle = btnIdx == 100 ? selectedButtonStyle : new GUIStyle (GUI.skin.button);
			if (GUI.Button (new Rect (MenuRect.x + MenuRect.width - dx, MenuRect.height - dy, dx, dy), "Exit", selectedExitButtonStyle))
			{
				ExitToMainMenu ();
			}
		}

		void ExitToMainMenu ()
		{
			switch (debugMenuState)
			{
			case DebugMenuState.AnalytiAll:
				AnalytiAll.EventManager.Instance.CloseDebugMenu ();

				break;
			case DebugMenuState.AmEvents:
				AMEvents.AMEvents.Instance.CloseDebugMenu ();
				break;
			default:
				break;
			}
			debugMenuState = DebugMenuState.Main;
		}
		//for correct system methods order
		void OnEnable ()
		{

		}
		void OnDestroy ()
		{
			
		}
	}
}