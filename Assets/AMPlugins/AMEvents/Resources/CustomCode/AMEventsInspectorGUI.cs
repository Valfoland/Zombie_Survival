#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using UnityEditor;

namespace AMEvents
{
	/// <summary>
	/// Класс для передачи событий перехода на новую сцену, паузы и возобновления сцены, а также собственных вызовов в моменты, когда приложение неактивно в нативный код.
	/// </summary>
	[CustomEditor (typeof (AMEvents))]
	public class AMEventsInspectorGUI : Editor
	{
		bool isInit = false;

		int labelsWidth = 170;
		int labelsSpace = 15;

		static bool expandScenesList = true;

		AMEvents amEvents;

		/// <summary>
		/// Инициализация.
		/// </summary>
		void Init ()
		{
			if (!isInit)
			{
				isInit = true;
				amEvents = (AMEvents)target;
				amEvents.autoInterstitialScenes = AMEvents.AutoInterstitialScene.UpdateList (amEvents.autoInterstitialScenes, true);
				if (amEvents.autoInterstitialScenes == null)
					expandScenesList = false;
				if (!EditorApplication.isPlaying)
					GetAMProjectFlags ();
			}
		}

		/// <summary>
		/// Перерисовка GUI инспектора.
		/// </summary>
		public override void OnInspectorGUI ()
		{
			Init ();

			GUIStyle delayFieldStyle = new GUIStyle (GUI.skin.textField);
			delayFieldStyle.alignment = TextAnchor.MiddleRight;
			GUIStyle delayLabelStyle = new GUIStyle (GUI.skin.label);
			delayLabelStyle.alignment = TextAnchor.MiddleLeft;

			GUILayout.BeginHorizontal ();
			GUILayout.Space (labelsSpace);
			GUILayout.Label ("Interstitial Delay", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
			TimeSpan delayHMS = new TimeSpan (0, 0, amEvents.InterstitialDelay);

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
			if (amEvents.InterstitialDelay != Convert.ToInt32 (delayHMS.TotalSeconds))
			{
				amEvents.InterstitialDelay = Convert.ToInt32 (delayHMS.TotalSeconds);
				SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.INTERSTITIAL_DELAY_KEY);
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Space (labelsSpace);
			GUILayout.Label ("Auto Interstitial", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
			bool tempAmEventsAutoInterstitial = amEvents.AutoInterstitial;
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

				if (amEvents.autoInterstitialScenes == null)
				{
					amEvents.autoInterstitialScenes = AMEvents.AutoInterstitialScene.UpdateList (amEvents.autoInterstitialScenes, true);
				}
				if (expandScenesList)
				{
					for (int i = 0; i < amEvents.autoInterstitialScenes.Count; i++)
					{
						var tempScene = amEvents.autoInterstitialScenes [i];

						sceneNameToggleStyle.normal.textColor = tempScene.skip ? Color.gray : new GUIStyle (GUI.skin.label).normal.textColor;
						sceneNameToggleStyle.active.textColor = sceneNameToggleStyle.normal.textColor;

						GUILayout.BeginHorizontal ();
						GUILayout.Space (labelsSpace * 2);
						bool tempToggle = !tempScene.skip;
						tempToggle = GUILayout.Toggle (tempToggle, tempScene.name, sceneNameToggleStyle, 
							new GUILayoutOption[] {
								GUILayout.ExpandWidth (false),
								GUILayout.MinWidth (labelsWidth),
								GUILayout.MaxWidth (labelsWidth)
							});
						if (tempScene.skip == tempToggle)
						{
							tempScene.skip = !tempToggle;
							amEvents.autoInterstitialScenes [i] = tempScene;
							SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.AUTO_INTERSTITIAL_SCENES_KEY);
						}
						GUILayout.EndHorizontal ();
					}
				}
			}

			if (tempAmEventsAutoInterstitial != amEvents.AutoInterstitial)
			{
				if (!tempAmEventsAutoInterstitial)
				{
					amEvents.autoInterstitialScenes = null;
				}
				SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.AUTO_INTERSTITIAL_SCENES_KEY);

				amEvents.AutoInterstitial = tempAmEventsAutoInterstitial;
				SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.DISABLE_AUTO_INTERSTITIAL_KEY);
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Space (labelsSpace);
			GUILayout.Label ("Cross Ad", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
			bool tempAmEventsCrossAd = amEvents.CrossAd;
			tempAmEventsCrossAd = GUILayout.Toggle (tempAmEventsCrossAd, "");
			GUILayout.EndHorizontal ();
			if (tempAmEventsCrossAd != amEvents.CrossAd)
			{
				amEvents.CrossAd = tempAmEventsCrossAd;
				SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.CROSS_AD_KEY);
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Space (labelsSpace);
			GUILayout.Label ("Default Start Screen", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
			amEvents.AdAtStart = GUILayout.Toggle (amEvents.AdAtStart, "");
			GUILayout.EndHorizontal ();

			if (amEvents.AdAtStart)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Start Screen Opacity", new GUILayoutOption[] {
					GUILayout.ExpandWidth (false),
					GUILayout.MinWidth (labelsWidth),
					GUILayout.MaxWidth (labelsWidth)
				});
				float tempStartScreenOpacity = amEvents.StartScreenOpacity;
				tempStartScreenOpacity = GUILayout.HorizontalSlider (tempStartScreenOpacity, 0, 1);
				GUILayout.Label ((Math.Round (tempStartScreenOpacity, 2) * 100).ToString () + "%", new GUILayoutOption[]{ GUILayout.Width (40) });
				GUILayout.EndHorizontal ();
				if (tempStartScreenOpacity != amEvents.StartScreenOpacity)
				{
					amEvents.StartScreenOpacity = tempStartScreenOpacity;
					SetOptionToAMProject (AMConfigsParser.AMProjectInfoInside.START_SCREEN_OPACITY_KEY);
				}
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Space (labelsSpace);
			GUILayout.Label ("Debug Mode", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
			amEvents.DebugMode = GUILayout.Toggle (amEvents.DebugMode, "");
			GUILayout.EndHorizontal ();

			if (amEvents.DebugMode)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Debug Menu", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				amEvents.DebugMenu = GUILayout.Toggle (amEvents.DebugMenu, "");
				GUILayout.EndHorizontal ();
				#if UNITY_TVOS
				if (amEvents.DebugMenu)
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Space (labelsSpace);
					GUILayout.Label ("Handle Controller", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
					amEvents.HandleController = GUILayout.Toggle (amEvents.HandleController, "");
					GUILayout.EndHorizontal ();
				}
				#endif
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Space (labelsSpace);
			GUILayout.Label ("Auto Pause", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
			bool tempAmEventsAutoPause = amEvents.AutoPause;
			tempAmEventsAutoPause = GUILayout.Toggle (tempAmEventsAutoPause, "");
			GUILayout.EndHorizontal ();
			if (tempAmEventsAutoPause != amEvents.AutoPause)
			{
				amEvents.AutoPause = tempAmEventsAutoPause;
			}
		}

		void GetAMProjectFlags ()
		{
			string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, "am_project.txt");

			Hashtable am_project = AMUtils.AMJSON.JsonDecode (System.IO.File.ReadAllText (filePath)) as Hashtable;

			if (am_project == null) return;

			try
			{
				amEvents.AutoInterstitial = !(bool)am_project[AMConfigsParser.AMProjectInfoInside.DISABLE_AUTO_INTERSTITIAL_KEY];
			}
			catch (Exception)
			{
				amEvents.AutoInterstitial = true;
			}
			try
			{
				amEvents.CrossAd = (bool)am_project[AMConfigsParser.AMProjectInfoInside.CROSS_AD_KEY];
			}
			catch (Exception)
			{
				amEvents.CrossAd = false;
			}
			try
			{
				amEvents.AdAtStart = (bool)am_project[AMConfigsParser.AMProjectInfoInside.AD_AT_THE_START_KEY];
			}
			catch (Exception)
			{
				amEvents.AdAtStart = true;
			}
			try
			{
				amEvents.StartScreenOpacity = float.Parse (am_project[AMConfigsParser.AMProjectInfoInside.START_SCREEN_OPACITY_KEY].ToString ());
			}
			catch (Exception)
			{
				amEvents.StartScreenOpacity = 0.95f;
			}
			try
			{
				amEvents.InterstitialDelay = int.Parse (am_project[AMConfigsParser.AMProjectInfoInside.INTERSTITIAL_DELAY_KEY].ToString ());
			}
			catch (Exception)
			{
				amEvents.InterstitialDelay = 60;
			}
			try
			{
				var scenesTempList = (am_project[AMConfigsParser.AMProjectInfoInside.AUTO_INTERSTITIAL_SCENES_KEY] as ArrayList).ToArray ().ToList ();

				for (int i = 0; i < amEvents.autoInterstitialScenes.Count; i++)
				{
					var item = amEvents.autoInterstitialScenes [i];
					item.skip = (scenesTempList.FindIndex (s => {return s.ToString () == item.name;})) == -1;
					amEvents.autoInterstitialScenes [i] = item;
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
				value = value == null ? !amEvents.AutoInterstitial : value;
				am_project.Add (key, (bool)value);
				break;
			case AMConfigsParser.AMProjectInfoInside.CROSS_AD_KEY:
				value = value == null ? amEvents.CrossAd : value;
				am_project.Add (key, (bool)value);
				break;
			case AMConfigsParser.AMProjectInfoInside.AD_AT_THE_START_KEY:
				value = value == null ? amEvents.AdAtStart : value;
				am_project.Add (key, (bool)value);
				break;
			case AMConfigsParser.AMProjectInfoInside.START_SCREEN_OPACITY_KEY:
				var floatValue = value == null ? amEvents.StartScreenOpacity : float.Parse (value.ToString ());
				am_project.Add (key, Math.Round (floatValue, 2));
				break;
			case AMConfigsParser.AMProjectInfoInside.INTERSTITIAL_DELAY_KEY:
				var intValue = value == null ? amEvents.InterstitialDelay : int.Parse (value.ToString ());
				am_project.Add (key, intValue);
				break;
			case AMConfigsParser.AMProjectInfoInside.AUTO_INTERSTITIAL_SCENES_KEY:
				ArrayList listValue = new ArrayList ();
				foreach (var item in amEvents.autoInterstitialScenes)
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
}
#endif