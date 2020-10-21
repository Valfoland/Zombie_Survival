#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace CustomCode
{
	/// <summary>
	/// Инструмент для загрузки кастомных скриптов из ресурсов
	/// </summary>
	[CustomEditor (typeof (CustomResourcesManager))]
	public class CustomResourcesManagerInspectorGUI : Editor
	{
		bool isInit = false;

		int labelsWidth = 180;
		int labelsSpace = 15;
		int buttonHeight = 15;
		int spaceBetweenPlugins = 10;

		public static bool showBindWindow = false;
		static bool expandScenesList = true;
		static bool customCodeScene = false;

		#if AM_EDITOR_LANGUAGE_EN
		string _BindButtonLabel = "Bind";
		string _RefreshCQSListButtonLabel = "Refresh list Custom Quality Settings";
		#else
		string _BindButtonLabel = "Назначить";
		string _RefreshCQSListButtonLabel = "Обновить параметры Custom Quality Settings";
		#endif
		
		CustomResourcesManager customResourcesManager;

		/// <summary>
		/// Инициализация.
		/// </summary>
		void Init ()
		{
			if (!isInit)
			{
				isInit = true;
				customResourcesManager = (CustomResourcesManager)target;
				customResourcesManager.amEventsAutoInterstitialScenes = AMEvents.AMEvents.AutoInterstitialScene.UpdateList (customResourcesManager.amEventsAutoInterstitialScenes, true);
				if (customResourcesManager.amEventsAutoInterstitialScenes == null)
					expandScenesList = false;
				if (!EditorApplication.isPlaying)
					GetAMProjectFlags ();
				if (!System.IO.File.Exists (System.IO.Path.Combine (CustomCode.CustomResourcesManagerAssetBundles.GetAssetBundlePath (true), CustomCode.CustomResourcesManagerAssetBundles.INFO_FILE_NAME)))
					customResourcesManager.enableAssetBundles = false;
				
				customCodeScene = EditorSceneManager.GetActiveScene ().path.Contains (AMEvents.AMEvents.CUSTOM_CODE_SCENE_NAME);
			}
		}

		/// <summary>
		/// Ожидание события назначения кнопки консоли.
		/// </summary>
		void BindButtonHandler ()
		{
			BindPopupWindow.bindButtonCallback -= BindButtonHandler;
			showBindWindow = false;
			customResourcesManager.consoleToggleKey = BindPopupWindow.bindButtonKeyCode;
		}

		/// <summary>
		/// Перерисовка GUI инспектора.
		/// </summary>
		public override void OnInspectorGUI ()
		{
			Init ();

			GUIStyle pluginNameToggleStyle = new GUIStyle (GUI.skin.toggle);
			pluginNameToggleStyle.fontStyle = FontStyle.Bold;

			GUILayout.BeginHorizontal ();
			GUILayout.Space (labelsSpace);
			GUILayout.Label ("Debug Menu", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
			customResourcesManager.debugMenu = GUILayout.Toggle (customResourcesManager.debugMenu, "");
			GUILayout.EndHorizontal ();
			#if UNITY_TVOS
			if (customResourcesManager.debugMenu)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Handle Controller", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				customResourcesManager.handleController = GUILayout.Toggle (customResourcesManager.handleController, "");
				GUILayout.EndHorizontal ();
			}
			#endif
			
			#region Scenes Settings
			if (customCodeScene)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUI.enabled = !customResourcesManager.enableAssetBundles;
				GUILayout.Label ("Auto Load Next Scene", new GUILayoutOption[] {GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				customResourcesManager.shouldLoadNextScene = GUILayout.Toggle (customResourcesManager.shouldLoadNextScene, "");
				GUI.enabled = true;
				GUILayout.EndHorizontal ();
			}
			pluginNameToggleStyle.normal.textColor = !customResourcesManager.enableAssetBundles?Color.gray:new GUIStyle (GUI.skin.label).normal.textColor;
			pluginNameToggleStyle.active.textColor = pluginNameToggleStyle.normal.textColor;

			GUILayout.BeginHorizontal ();
			GUILayout.Space (labelsSpace);
			GUILayout.Label ("Use Asset Bundles", new GUILayoutOption[] {GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
			bool tempEnableAssetBundles = customResourcesManager.enableAssetBundles;
			tempEnableAssetBundles = GUILayout.Toggle (tempEnableAssetBundles, "");
			if (tempEnableAssetBundles != customResourcesManager.enableAssetBundles)
			{
				customResourcesManager.enableAssetBundles = tempEnableAssetBundles;
				if (customResourcesManager.enableAssetBundles)
					customResourcesManager.shouldLoadNextScene = true;
			}
			GUILayout.EndHorizontal ();

			if (customResourcesManager.enableAssetBundles) 
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Next Scene Name", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				customResourcesManager.assetBundlesNextSceneName = EditorGUILayout.TextField (customResourcesManager.assetBundlesNextSceneName);
				GUILayout.EndHorizontal ();
			}
			#endregion

			#region AnalytiAll Settings
			pluginNameToggleStyle.normal.textColor = !customResourcesManager.enableAnalytiAll?Color.gray:new GUIStyle (GUI.skin.label).normal.textColor;
			pluginNameToggleStyle.active.textColor = pluginNameToggleStyle.normal.textColor;

			GUILayout.BeginHorizontal ();
			customResourcesManager.enableAnalytiAll = GUILayout.Toggle (customResourcesManager.enableAnalytiAll, "AnalytiAll", pluginNameToggleStyle);
			GUILayout.EndHorizontal ();

			if (customResourcesManager.enableAnalytiAll) 
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Debug Mode", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				customResourcesManager.analytiAllDebugMode = GUILayout.Toggle (customResourcesManager.analytiAllDebugMode, "");
				GUILayout.EndHorizontal ();

				GUILayout.Space (spaceBetweenPlugins);
			}
			#endregion

			#region AM Events Settings
			pluginNameToggleStyle.normal.textColor = !customResourcesManager.enableAMEvents?Color.gray:new GUIStyle (GUI.skin.label).normal.textColor;
			pluginNameToggleStyle.active.textColor = pluginNameToggleStyle.normal.textColor;

			GUILayout.BeginHorizontal ();
			customResourcesManager.enableAMEvents = GUILayout.Toggle (customResourcesManager.enableAMEvents, "AM Events", pluginNameToggleStyle);
			GUILayout.EndHorizontal ();

			if (customResourcesManager.enableAMEvents) 
			{
				GUIStyle delayFieldStyle = new GUIStyle (GUI.skin.textField);
				delayFieldStyle.alignment = TextAnchor.MiddleRight;
				GUIStyle delayLabelStyle = new GUIStyle (GUI.skin.label);
				delayLabelStyle.alignment = TextAnchor.MiddleLeft;

				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Interstitial Delay", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				TimeSpan delayHMS = new TimeSpan (0, 0, customResourcesManager.amEventsInterstitialDelay);

				var tempM = delayHMS.Minutes;
				tempM = EditorGUILayout.IntField (tempM, delayFieldStyle, new GUILayoutOption[]{GUILayout.Width (22)});
				tempM = tempM < 0 ? tempM * -1 : tempM;
				GUILayout.Label ("M", delayLabelStyle, new GUILayoutOption[]{GUILayout.Width (13)});

				var tempS = delayHMS.Seconds;
				tempS = EditorGUILayout.IntField (tempS, delayFieldStyle, new GUILayoutOption[]{GUILayout.Width (22)});
				tempS = tempS < 0 ? tempS * -1 : tempS;
				GUILayout.Label ("S", delayLabelStyle, new GUILayoutOption[]{GUILayout.Width (13)});
				GUILayout.EndHorizontal ();

				delayHMS = new TimeSpan (0, tempM, tempS);
				if (customResourcesManager.amEventsInterstitialDelay != Convert.ToInt32 (delayHMS.TotalSeconds))
				{
					customResourcesManager.amEventsInterstitialDelay = Convert.ToInt32 (delayHMS.TotalSeconds);
					SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.INTERSTITIAL_DELAY_KEY);
				}

				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Auto Interstitial", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				bool tempAmEventsAutoInterstitial = customResourcesManager.amEventsAutoInterstitial;
				tempAmEventsAutoInterstitial = GUILayout.Toggle (tempAmEventsAutoInterstitial, "");
				GUILayout.EndHorizontal ();

				if (tempAmEventsAutoInterstitial)
				{
					GUIStyle sceneNameToggleStyle = new GUIStyle (GUI.skin.toggle);
					GUIStyle foldoutStyle = new GUIStyle (EditorStyles.foldout);
					foldoutStyle.focused.background = foldoutStyle.normal.background;
					foldoutStyle.onFocused.background = foldoutStyle.onNormal.background;
					foldoutStyle.focused.textColor = foldoutStyle.normal.textColor;
					foldoutStyle.active.textColor = foldoutStyle.normal.textColor;
					foldoutStyle.onFocused.textColor = foldoutStyle.normal.textColor;
					foldoutStyle.onActive.textColor = foldoutStyle.normal.textColor;

					GUILayout.BeginHorizontal ();
					GUILayout.Space (labelsSpace * 1.8f);
					expandScenesList = EditorGUILayout.Foldout (expandScenesList, "Scenes for New Scene Interstitial:", foldoutStyle);
					GUILayout.EndHorizontal ();

					if (customResourcesManager.amEventsAutoInterstitialScenes == null)
					{
						customResourcesManager.amEventsAutoInterstitialScenes = AMEvents.AMEvents.AutoInterstitialScene.UpdateList (customResourcesManager.amEventsAutoInterstitialScenes, true);
					}
					if (expandScenesList)
					{
						for (int i = 0; i < customResourcesManager.amEventsAutoInterstitialScenes.Count; i++)
						{
							var tempScene = customResourcesManager.amEventsAutoInterstitialScenes [i];

							sceneNameToggleStyle.normal.textColor = tempScene.skip?Color.gray:new GUIStyle (GUI.skin.label).normal.textColor;
							sceneNameToggleStyle.active.textColor = sceneNameToggleStyle.normal.textColor;

							GUILayout.BeginHorizontal ();
							GUILayout.Space (labelsSpace * 2);
							bool tempToggle = !tempScene.skip;
							tempToggle = GUILayout.Toggle (tempToggle, tempScene.name, sceneNameToggleStyle, 
								new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
							if (tempScene.skip == tempToggle)
							{
								tempScene.skip = !tempToggle;
								customResourcesManager.amEventsAutoInterstitialScenes [i] = tempScene;
								SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.AUTO_INTERSTITIAL_SCENES_KEY);
							}
							GUILayout.EndHorizontal ();
						}
					}
				}

				if (tempAmEventsAutoInterstitial != customResourcesManager.amEventsAutoInterstitial)
				{
					if (!tempAmEventsAutoInterstitial)
					{
						customResourcesManager.amEventsAutoInterstitialScenes = null;
					}
					SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.AUTO_INTERSTITIAL_SCENES_KEY);

					customResourcesManager.amEventsAutoInterstitial = tempAmEventsAutoInterstitial;
					SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.DISABLE_AUTO_INTERSTITIAL_KEY);
				}

				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Cross Ad", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				bool tempAmEventsCrossAd = customResourcesManager.amEventsCrossAd;
				tempAmEventsCrossAd = GUILayout.Toggle (tempAmEventsCrossAd, "");
				GUILayout.EndHorizontal ();
				if (tempAmEventsCrossAd != customResourcesManager.amEventsCrossAd)
				{
					customResourcesManager.amEventsCrossAd = tempAmEventsCrossAd;
					SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.CROSS_AD_KEY);
				}

				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Default Start Screen", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				bool tempAdAtStart = customResourcesManager.amEventsAdAtStart;
				tempAdAtStart = GUILayout.Toggle (tempAdAtStart, "");
				GUILayout.EndHorizontal ();
				if (tempAdAtStart != customResourcesManager.amEventsAdAtStart)
				{
					customResourcesManager.amEventsAdAtStart = tempAdAtStart;
					SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.AD_AT_THE_START_KEY);
				}

				if (customResourcesManager.amEventsAdAtStart)
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Space (labelsSpace);
					GUILayout.Label ("Start Screen Opacity", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
					float tempStartScreenOpacity = customResourcesManager.amEventsStartScreenOpacity;
					tempStartScreenOpacity = GUILayout.HorizontalSlider (tempStartScreenOpacity, 0, 1);
					GUILayout.Label ((Math.Round (tempStartScreenOpacity, 2) * 100).ToString ()+"%", new GUILayoutOption[]{GUILayout.Width (40)});
					GUILayout.EndHorizontal ();
					if (tempStartScreenOpacity != customResourcesManager.amEventsStartScreenOpacity)
					{
						customResourcesManager.amEventsStartScreenOpacity = tempStartScreenOpacity;
						SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.START_SCREEN_OPACITY_KEY);
					}
				}

				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Debug Mode", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				customResourcesManager.amEventsDebugMode = GUILayout.Toggle (customResourcesManager.amEventsDebugMode, "");
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Auto Pause", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				bool tempAmEventsAutoPause = customResourcesManager.amEventsAutoPause;
				tempAmEventsAutoPause = GUILayout.Toggle (tempAmEventsAutoPause, "");
				GUILayout.EndHorizontal ();
				if (tempAmEventsAutoPause != customResourcesManager.amEventsAutoPause)
				{
					customResourcesManager.amEventsAutoPause = tempAmEventsAutoPause;
				}

				GUILayout.Space (spaceBetweenPlugins);
			}
			#endregion

			#region AM Logger Settings
			pluginNameToggleStyle.normal.textColor = !customResourcesManager.enableAMLogger?Color.gray:new GUIStyle (GUI.skin.label).normal.textColor;
			pluginNameToggleStyle.active.textColor = pluginNameToggleStyle.normal.textColor;

			GUILayout.BeginHorizontal ();
			customResourcesManager.enableAMLogger = GUILayout.Toggle (customResourcesManager.enableAMLogger, "AM Logger", pluginNameToggleStyle);
			GUILayout.EndHorizontal ();

			if (customResourcesManager.enableAMLogger) 
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Enable Log Console", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				customResourcesManager.enableLogConsole = GUILayout.Toggle (customResourcesManager.enableLogConsole, "");
				GUILayout.EndHorizontal ();

				if (customResourcesManager.enableLogConsole)
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Space (labelsSpace);
					GUILayout.Label ("Font Size", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
					try 
					{
						customResourcesManager.consoleFontSize = int.Parse (EditorGUILayout.TextField (customResourcesManager.consoleFontSize.ToString ()));
						if (customResourcesManager.consoleFontSize < 0)
							customResourcesManager.consoleFontSize = customResourcesManager.consoleFontSize * (-1);
					} 
					catch (Exception) {}
					GUILayout.EndHorizontal ();

//					GUILayout.BeginHorizontal ();
//					GUILayout.Space (labelsSpace);
//					GUILayout.Label ("Timestamp", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
//					customResourcesManager.useTimestamp = GUILayout.Toggle (customResourcesManager.useTimestamp, "");
//					GUILayout.EndHorizontal ();

//					GUILayout.BeginHorizontal ();
//					GUILayout.Space (labelsSpace);
//					GUILayout.Label ("Repaint Logs", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
//					customResourcesManager.consoleRepaint = GUILayout.Toggle (customResourcesManager.consoleRepaint, "");
//					GUILayout.EndHorizontal ();

//					if (customResourcesManager.consoleRepaint)
//					{
//						GUILayout.BeginHorizontal ();
//						GUILayout.Space (labelsSpace);
//						GUILayout.Label ("Text Color", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
//						customResourcesManager.consoleTextColor = EditorGUILayout.ColorField (customResourcesManager.consoleTextColor);
//						GUILayout.EndHorizontal ();
//					}

					GUILayout.BeginHorizontal ();
					GUILayout.Space (labelsSpace);
					GUILayout.Label ("Console Toggle Key", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
					var selectedKeyCode = EditorGUILayout.EnumPopup (customResourcesManager.consoleToggleKey).ToString ();
					customResourcesManager.consoleToggleKey = (KeyCode) System.Enum.Parse (typeof (KeyCode), selectedKeyCode);
					GUILayout.Space (2);
					GUIStyle pressedButton = new GUIStyle (GUI.skin.button);
					if (showBindWindow)
						pressedButton.normal = GUI.skin.button.active;
					else
						pressedButton.normal = GUI.skin.button.normal;
					if (GUILayout.Button (_BindButtonLabel, pressedButton, new GUILayoutOption[]{GUILayout.Width (80), GUILayout.MaxHeight (buttonHeight)}))
					{
						showBindWindow = true;
						BindPopupWindow.bindButtonCallback += BindButtonHandler;
						BindPopupWindow.Init ();
					}
					GUILayout.EndHorizontal ();

					GUILayout.BeginHorizontal ();
					GUILayout.Space (labelsSpace);
					GUILayout.Label ("Console Shake Acceleration", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
					try 
					{
						customResourcesManager.consoleShakeAcceleration = float.Parse (EditorGUILayout.TextField (customResourcesManager.consoleShakeAcceleration.ToString ()));
						if (customResourcesManager.consoleShakeAcceleration < 0)
							customResourcesManager.consoleShakeAcceleration = customResourcesManager.consoleShakeAcceleration * (-1);
					} 
					catch (Exception) {}
					GUILayout.EndHorizontal ();
				}

				GUILayout.Space (spaceBetweenPlugins);
			}
			#endregion

			#region Custom Quality Setup Settings
			pluginNameToggleStyle.normal.textColor = !customResourcesManager.enableCustomQualitySetup?Color.gray:new GUIStyle (GUI.skin.label).normal.textColor;
			pluginNameToggleStyle.active.textColor = pluginNameToggleStyle.normal.textColor;

			GUILayout.BeginHorizontal ();
			customResourcesManager.enableCustomQualitySetup = GUILayout.Toggle (customResourcesManager.enableCustomQualitySetup, "Custom Quality Setup", pluginNameToggleStyle);
			GUILayout.EndHorizontal ();

			if (customResourcesManager.enableCustomQualitySetup) 
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Custom Mode", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				customResourcesManager.cqsCustomMode = GUILayout.Toggle (customResourcesManager.cqsCustomMode, "");
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				CustomQualitySetup.CustomQualitySetupInspectorGUI.DrawInspector ();
				GUILayout.EndHorizontal ();

				if (GUILayout.Button (_RefreshCQSListButtonLabel)) 
				{
					CustomQualitySetup.CustomQualitySetupInspectorGUI.UpdateData ();
				}

				GUILayout.Space (spaceBetweenPlugins);
			}
			#endregion
			EditorUtility.SetDirty (customResourcesManager);
		}

		void GetAMProjectFlags ()
		{
			string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, "am_project.txt");

			Hashtable am_project = AMUtils.AMJSON.JsonDecode (System.IO.File.ReadAllText (filePath)) as Hashtable;

			if (am_project == null) return;

			try
			{
				customResourcesManager.amEventsAutoInterstitial = !(bool)am_project[AMConfigsParser.AMProjectInfoInside.DISABLE_AUTO_INTERSTITIAL_KEY];
			}
			catch (Exception)
			{
				customResourcesManager.amEventsAutoInterstitial = true;
			}
			try
			{
				customResourcesManager.amEventsCrossAd = (bool)am_project[AMConfigsParser.AMProjectInfoInside.CROSS_AD_KEY];
			}
			catch (Exception)
			{
				customResourcesManager.amEventsCrossAd = false;
			}
			try
			{
				customResourcesManager.amEventsAdAtStart = (bool)am_project[AMConfigsParser.AMProjectInfoInside.AD_AT_THE_START_KEY];
			}
			catch (Exception)
			{
				customResourcesManager.amEventsAdAtStart = true;
			}
			try
			{
				customResourcesManager.amEventsStartScreenOpacity = float.Parse (am_project[AMConfigsParser.AMProjectInfoInside.START_SCREEN_OPACITY_KEY].ToString ());
			}
			catch (Exception)
			{
				customResourcesManager.amEventsStartScreenOpacity = 0.95f;
			}
			try
			{
				customResourcesManager.amEventsInterstitialDelay = int.Parse (am_project[AMConfigsParser.AMProjectInfoInside.INTERSTITIAL_DELAY_KEY].ToString ());
			}
			catch (Exception)
			{
				customResourcesManager.amEventsInterstitialDelay = 60;
			}
			try
			{
				var scenesTempList = (am_project[AMConfigsParser.AMProjectInfoInside.AUTO_INTERSTITIAL_SCENES_KEY] as ArrayList).ToArray ().ToList ();

				for (int i = 0; i < customResourcesManager.amEventsAutoInterstitialScenes.Count; i++)
				{
					var item = customResourcesManager.amEventsAutoInterstitialScenes [i];
					item.skip = (scenesTempList.FindIndex (s => {return s.ToString () == item.name;})) == -1;
					customResourcesManager.amEventsAutoInterstitialScenes [i] = item;
				}
			}
			catch (Exception)
			{}
		}

		void SetOptionToAMProject (string key, object value = null)
		{
			string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, "am_project.txt");

			Hashtable am_project = AMUtils.AMJSON.JsonDecode (System.IO.File.ReadAllText (filePath)) as Hashtable;

			if (am_project == null) return;

			am_project.Remove (key);

			switch (key)
			{
			case AMConfigsParser.AMProjectInfoInside.DISABLE_AUTO_INTERSTITIAL_KEY:
				value = value == null ? !customResourcesManager.amEventsAutoInterstitial : value;
				am_project.Add (key, (bool)value);
				break;
			case AMConfigsParser.AMProjectInfoInside.CROSS_AD_KEY:
				value = value == null ? customResourcesManager.amEventsCrossAd : value;
				am_project.Add (key, (bool)value);
				break;
			case AMConfigsParser.AMProjectInfoInside.AD_AT_THE_START_KEY:
				value = value == null ? customResourcesManager.amEventsAdAtStart : value;
				am_project.Add (key, (bool)value);
				break;
			case AMConfigsParser.AMProjectInfoInside.START_SCREEN_OPACITY_KEY:
				var floatValue = value == null ? customResourcesManager.amEventsStartScreenOpacity : float.Parse (value.ToString ());
				am_project.Add (key, Math.Round (floatValue, 2));
				break;
			case AMConfigsParser.AMProjectInfoInside.INTERSTITIAL_DELAY_KEY:
				var intValue = value == null ? customResourcesManager.amEventsInterstitialDelay : int.Parse (value.ToString ());
				am_project.Add (key, intValue);
				break;
			case AMConfigsParser.AMProjectInfoInside.AUTO_INTERSTITIAL_SCENES_KEY:
				if (customResourcesManager.amEventsAutoInterstitialScenes == null)
					break;
				ArrayList listValue = new ArrayList ();
				foreach (var item in customResourcesManager.amEventsAutoInterstitialScenes)
				{
					if (!item.skip)
					{
						listValue.Add (item.name);
					}
				}
				am_project.Add (key, listValue);
				break;
			default:
				break;
			}

			string result = AMUtils.AMJSON.FormatJson (AMUtils.AMJSON.JsonEncode (am_project));

			System.IO.File.WriteAllText (filePath, result);
		}
	}

	/// <summary>
	/// Popup для назначения кнопки консоли.
	/// </summary>
	class BindPopupWindow : EditorWindow
	{		
		public static System.Action bindButtonCallback;
		public static KeyCode bindButtonKeyCode = KeyCode.None;

		#if AM_EDITOR_LANGUAGE_EN
		static string _BindWindowTitle = "Bind button";
		static string _BindWindowText = "Press the button you want to bind \nto show/hide logger console.";
		static string _BindWindowCancel = "Cancel";
		#else
		static string _BindWindowTitle = "Назначить кнопку";
		static string _BindWindowText = "Нажмите кнопку, которую хотите назначить \nдля показа консоли.";
		static string _BindWindowCancel = "Отмена";
		#endif
		static EditorWindow bindPopupWindow;

		/// <summary>
		/// Инициализация.
		/// </summary>
		public static void Init ()
		{
			bindPopupWindow = EditorWindow.GetWindow<BindPopupWindow> (true, _BindWindowTitle, true);
			bindPopupWindow.Show ();
		}

		/// <summary>
		/// Отрисовка GUI окна.
		/// </summary>
		void OnGUI ()
		{
			bindPopupWindow.minSize = new Vector2 (280, 110);
			bindPopupWindow.maxSize = bindPopupWindow.minSize;
			#if !UNITY_EDITOR_OSX
			bindPopupWindow.position = new Rect (Screen.currentResolution.width / 2 - minSize.x / 2, Screen.currentResolution.height / 2 - minSize.y / 2, 0, 0);
			#endif
			GUILayout.Space (20);
			GUILayout.Label (_BindWindowText);
			GUILayout.Space (30);

			Event e = Event.current;

			if (e.isKey) 
			{
				bindButtonKeyCode = e.keyCode;
				if (bindButtonCallback != null)
					bindButtonCallback ();
				bindPopupWindow.Close ();
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Space ((int)minSize.x / 3);
			if (GUILayout.Button (_BindWindowCancel, new GUILayoutOption[] {GUILayout.ExpandWidth (false), GUILayout.MaxWidth ((int)minSize.x / 3)})) 
			{
				bindPopupWindow.Close ();
			}
			GUILayout.EndHorizontal ();
		}

		/// <summary>
		/// Событие при закрытии окна.
		/// </summary>
		void OnDestroy ()
		{
			CustomResourcesManagerInspectorGUI.showBindWindow = false;
		}

		void OnLostFocus ()
		{
			CustomResourcesManagerInspectorGUI.showBindWindow = false;
			bindPopupWindow.Close ();
		}
	}
}
#endif