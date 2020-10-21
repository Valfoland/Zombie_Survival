#if UNITY_EDITOR
#pragma warning disable
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AMEditor
{
	public class WindowCreatePublicConfig : EditorWindow
	{
		public enum TypeWindow
		{
			Main = 1, 
			Wait
		}

		public static WindowCreatePublicConfig instance = null;

		public static List<Plugin> pluginsList;
		static List<Plugin> guiPluginsList;
		static List<Plugin> localPluginsList;
		static List<string> pluginsVersionFilters;
		static int selectedFilterIndex;
		static int tempFilterIndex;
		static bool forceSearch = false;
		static bool releaseOnly = true;
		static bool releaseOnlyAvailability = true;
		static bool orderByNameDescending = false;
		static bool orderByVersionDescending = true;
		static bool pushConfig = false;
		static bool cancelCurrentWork = false;

		public static bool isInit = false;

		static string LOCAL_PLUGINS_FILE = "";
		static string PLUGIN_CONFIG = "plugin_config.json";
		static string PUBLIC_CONFIG_FILE = "ameditor_plugins.json";
		static string PUBLIC_CONFIG_URL_TAIL = "/unity-plugins/am-editor-plugins";
		static string GROUP = "Unity Plugins";
		static string BRANCH = "master";
		public static string CommitMessage = string.Empty;
		static string waitMessage = string.Empty;
		static string CurrentStatus = string.Empty;
		static float CurrentProgress = 0.0f;
		static float ProgressStep = 0.0f;

		public static List<Texture> progressCircle = new List<Texture> ();

		static int progressFrameIndex;
		float progressRepaintInterval = 0.1f;
		float progressNextRepaintTime = 0;

		static string resourceFolder = string.Empty;

		#if UNITY_2018_2_OR_NEWER || UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER
			Texture orderFrontTexture;
			public Texture OrderFrontTexture1
        {
            get
            {
                return orderFrontTexture;
            }

            set
            {
                orderFrontTexture = value;
            }
        }
		Texture orderDescendingTexture;
        public Texture OrderDescendingTexture
        {
            get
            {
                return orderDescendingTexture;
            }

            set
            {
                orderDescendingTexture = value;
            }
        }
		#else
			Texture orderFrontTexture = new Texture ();
			Texture orderDescendingTexture = new Texture ();
		#endif

		#if AM_EDITOR_COMPACT_ON
        #if UNITY_2018_2_OR_NEWER || UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER
        Texture _updateButtonTexture;
        public Texture updateButtonTexture{
            get{ return _updateButtonTexture; }
            set{ _updateButtonTexture = value; }
        }
        Texture _saveButtonTexture;
        public Texture saveButtonTexture{
            get{ return _saveButtonTexture; }
            set{ _saveButtonTexture = value; }
        }
        #else
			Texture updateButtonTexture = new Texture ();
			Texture saveButtonTexture = new Texture ();
        #endif
		#endif
		static TypeWindow windowType = TypeWindow.Main;

		Vector2 scroll = Vector2.zero;

		void OnEnable ()
		{
			resourceFolder = EditorGUIUtility.isProSkin ? "pro" : "free";
				orderFrontTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/"+resourceFolder+"/am_editor_up_arrow.png");
				orderDescendingTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/"+resourceFolder+"/am_editor_down_arrow.png");
				#if AM_EDITOR_COMPACT_ON
					updateButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/"+resourceFolder+"/Compact/am_editor_update.png");
					saveButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/"+resourceFolder+"/Compact/am_editor_save.png");
				#endif
			if (AMEditor.WindowMain.progressCircle.Count > 0)
				progressCircle = AMEditor.WindowMain.progressCircle;
			else
			{
				progressCircle = new List<Texture> 
				{
					AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_0.png"), 
					AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_1.png"), 
					AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_2.png"), 
					AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_3.png"), 
					AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_4.png"), 
					AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_5.png"), 
					AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_6.png"), 
					AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_7.png")
				};
			}
		}

		public static void Init ()
		{
			cancelCurrentWork = false;

			pluginsVersionFilters = new List<string> ();
			pluginsVersionFilters.Add (AMEditorSystem.ContentCreatePublicFile._DefaultVersionFilter);
			selectedFilterIndex = 0;

			CheckLocal ();
			CalculatePluginsFilters ();
			UpdateGUIpluginsList ();

			instance = EditorWindow.GetWindow<WindowCreatePublicConfig> (true, AMEditorSystem.ContentCreatePublicFile._Title);
			instance.minSize = new Vector2 (Math.Min (1000, Screen.currentResolution.width), 500);

			instance.Repaint ();
		}

		static AMEditorGit gitSearch;
		public static void SearchConfig ()
		{
			windowType = WindowCreatePublicConfig.TypeWindow.Wait;
			waitMessage = AMEditorSystem.ContentProgressBar._ConfigPrepare;

			localPluginsList = pluginsList;

			try
			{
				gitSearch = new AMEditorGit ();
				gitSearch.ConfigWorkComplete += instance.ConfigWorkCompleteHandler;
				gitSearch.ChangeStatus += instance.ChangeStatusHandler;
			#if AM_EDITOR_DEBUG_MODE_ON
				gitSearch.printDebug = true;
			#else
				gitSearch.printDebug = false;
			#endif
				gitSearch.AuthByPT (GitAccount.current.server, GitAccount.current.privateToken);
				gitSearch.ConfigSearch (PLUGIN_CONFIG, GROUP, BRANCH, forceSearch);
			}
			catch (Exception)
			{}
		}

		static AMEditorGit gitPush;
		public static void PushConfig (string configContent, string commitMessage)
		{
			windowType = WindowCreatePublicConfig.TypeWindow.Wait;
			waitMessage = AMEditorSystem.ContentProgressBar._PushingConfig;

			if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentCreatePublicFile._PushingConfigTitle, AMEditorSystem.ContentProgressBar._PushingConfig, 0.25f))
			{
				CancelProgressBar ();
			}

			try
			{
				gitPush = new AMEditorGit ();
				gitPush.PushWorkComplete += instance.PushConfgiCompleteHandler;
				gitPush.ChangeStatus += instance.ChangeStatusHandler;
				gitPush.ErrorHappened += instance.ErrorHappenedHandler;
			#if AM_EDITOR_DEBUG_MODE_ON
				gitPush.printDebug = true;
			#else
				gitPush.printDebug = false;
			#endif
				gitPush.AuthByPT (GitAccount.current.server, GitAccount.current.privateToken);

				if (gitPush.printDebug)
					Debug.Log ("Trying to push file\n\t" +
						"name: "+PUBLIC_CONFIG_FILE+"\n\t" +
						"to: origin/"+AMEditorSystem.Git._BranchName+"\n\t" +
						"commit message: "+commitMessage+"\n\t" +
						"file content: \n" +configContent);

				gitPush.PushConfig (configContent, commitMessage, AMEditorSystem.Git._BranchName, GitAccount.current.privateToken);
			}
			catch (Exception)
			{}
		}

		static void CheckLocal ()
		{
			//BRANCH = AMEditorSystem.Git._BranchName;

			windowType = WindowCreatePublicConfig.TypeWindow.Wait;

			float progress = 0.0f;
			float step = 0.0f;

			LOCAL_PLUGINS_FILE = LocalRepositoryAPI.pathToRepository + AMEditorSystem.FileNames._ActualPlugins;

			if (!AMEditorFileStorage.FileExist (LOCAL_PLUGINS_FILE)) 
			{
				File.WriteAllText (LOCAL_PLUGINS_FILE, "[]");
			}

			if (instance == null && !cancelCurrentWork)
			{
				if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._ConfigPrepare, AMEditorSystem.ContentStatuses._CheckingLocalConfig, progress))
				{
					CancelProgressBar ();
					cancelCurrentWork = true;
				}
			}

			ArrayList pluginsFromLocalFile = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (LOCAL_PLUGINS_FILE)) as ArrayList;

			if (pluginsFromLocalFile != null && pluginsFromLocalFile.Count > 0)
			{
				if (pluginsList == null)
				{
					pluginsList = new List<Plugin> ();
				}
				step = 1 / pluginsFromLocalFile.Count;

				foreach (var item in pluginsFromLocalFile)
				{
					Plugin plugin = new Plugin (item as Hashtable);
					if (!pluginsList.Contains (plugin)) 
					{
						plugin.selected = true;
						pluginsList.Add (plugin);
					}

					progress += step;
					if (instance == null && !cancelCurrentWork)
					{
						if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._ConfigPrepare, AMEditorSystem.ContentStatuses._CheckingLocalConfig, progress))
						{
							CancelProgressBar ();
							cancelCurrentWork = true;
						}
					}
				}
				CancelProgressBar ();
				windowType = WindowCreatePublicConfig.TypeWindow.Main;
			}
			else
			{
				AMEditorPopupErrorWindow.ShowErrorPopup ("205", AMEditorSystem.FileSystemError._205 (AMEditorSystem.FileNames._ActualPlugins));//LOCAL_PLUGINS_FILE
				CancelProgressBar ();
				instance.Close ();
			}
		}

		static void CancelProgressBar ()
		{
			try 
			{
				EditorUtility.ClearProgressBar ();
			}
			catch (Exception) 
			{}

			if (gitSearch != null)
				gitSearch.StopDownload ();
			if (gitPush != null)
				gitPush.StopDownload ();

			isInit = false;
		}

		static bool retryConfigSearch = false;
		void ConfigWorkCompleteHandler (List<string> configList)
		{
			firstUpdateGUI = true;

			if (pluginsList == null)
			{
				pluginsList = new List<Plugin> ();
			}
			if (pluginsList.Count > 0)
			{
				pluginsList = new List<Plugin> ();
			}

			if (configList != null && configList.Count > 0) 
			{
				var tempConfigList = configList;
				foreach (var item in tempConfigList) 
				{
					Hashtable hashtable = AMEditorJSON.JsonDecode (item) as Hashtable; 
					Plugin plugin = new Plugin (hashtable);
					if (!pluginsList.Contains (plugin)) 
					{
						plugin.selected = true;
						pluginsList.Add (plugin);
					}
				}
				new UI.AMDisplayDialog (AMEditorSystem.ContentCreatePublicFile._SuccessDialogTitle, AMEditorSystem.ContentCreatePublicFile._SuccessDialogMessage, 
					AMEditorSystem.ContentStandardButton._Ok, "", 
					() => {if (instance == null) Init (); this.Focus (); retryConfigSearch = false;}, 
					() => {}, true).Show ();
			}
			else
			{
				pluginsList = localPluginsList;

				if (configList == null)
					new UI.AMDisplayDialog (AMEditorSystem.ContentCreatePublicFile._FailedDialogTitle, AMEditorSystem.ContentCreatePublicFile._FailedDialogMessage, 
						AMEditorSystem.ContentStandardButton._Ok, AMEditorSystem.ContentCreatePublicFile._RetryConfigSearchButton, 
						() => {this.Focus (); retryConfigSearch = false;}, 
						() => {if (instance == null) Init (); else retryConfigSearch = true;}, true).Show ();
				else if (configList.Count == 0)
					new UI.AMDisplayDialog (AMEditorSystem.ContentCreatePublicFile._CanceledDialogTitle, AMEditorSystem.ContentCreatePublicFile._CanceledDialogMessage, 
						AMEditorSystem.ContentStandardButton._Ok, AMEditorSystem.ContentCreatePublicFile._RetryConfigSearchButton, 
						() => {this.Focus (); retryConfigSearch = false;}, 
						() => {if (instance == null) Init (); else retryConfigSearch = true;}, true).Show ();
			}
			CalculatePluginsFilters ();
			UpdateGUIpluginsList ();
			windowType = WindowCreatePublicConfig.TypeWindow.Main;

			EditorApplication.update += UpdateGUI;
			try
			{
				System.Threading.Thread.Sleep (new TimeSpan (0, 0, 0, 0, 600));
			}
			catch (Exception)
			{
				UpdateGUI ();
			}
		}

		void PushConfgiCompleteHandler (bool success)
		{
			firstUpdateGUI = true;

			if (success)
			{
				AMEditorNetwork.SendMessage (AMEditorSystem.Git._BranchName, CommitMessage, GitAccount.current.server + PUBLIC_CONFIG_URL_TAIL);

				new AMEditor.UI.AMDisplayDialog (AMEditorSystem.ContentCreatePublicFile._PushingConfigTitle, 
					AMEditorSystem.ContentCreatePublicFile._PushingConfigSuccessDialog (PUBLIC_CONFIG_FILE), 
					AMEditorSystem.ContentStandardButton._Ok, "", () => { UpdateGUI (); }, () => { }, true).Show ();
			}
			else
			{
				new AMEditor.UI.AMDisplayDialog (AMEditorSystem.ContentCreatePublicFile._PushingConfigTitle, 
					AMEditorSystem.ContentCreatePublicFile._PushingConfigFailedDialog (PUBLIC_CONFIG_FILE), 
					AMEditorSystem.ContentStandardButton._Ok, "", () => { UpdateGUI (); }, () => { }, true).Show ();
			}

			windowType = WindowCreatePublicConfig.TypeWindow.Main;

			EditorApplication.update += UpdateGUI;
			try
			{
				System.Threading.Thread.Sleep (new TimeSpan (0, 0, 0, 0, 600));
			}
			catch (Exception)
			{
				UpdateGUI ();
			}
		}

		static bool firstUpdateGUI = true;
		void UpdateGUI ()
		{
			if (!firstUpdateGUI)
			{
				return;
			}
			firstUpdateGUI = false;

			try 
			{
				EditorApplication.update -= UpdateGUI;
				EditorUtility.ClearProgressBar ();
				windowType = TypeWindow.Main;
			} 
			catch (Exception) 
			{}
			if (retryConfigSearch)
				SearchConfig ();
		}

		void ChangeStatusHandler (string status, float step)
		{
			CurrentStatus = status;
			ProgressStep = step;

			if (status.Equals (AMEditorSystem.ContentStatuses._WorkingWithCompatible) ||
				status.Equals (AMEditorSystem.ContentStatuses._SearchingCompatibleProjects))
			{
				CurrentProgress = 0.0f;
			}

			try
			{
				EditorApplication.update += UpdateStatus;
			}
			catch (Exception)
			{}
			//Debug.Log ("Status: " + status);
		}
		void ErrorHappenedHandler (string error)
		{
			EditorApplication.update += UpdateGUI;
			if (error != string.Empty)
			{
				Debug.LogError (error);
			}
			try
			{
				CancelProgressBar ();
			}
			catch (Exception)
			{
				UpdateGUI ();
			}
		}
		static void UpdateStatus ()
		{
			try
			{
				EditorApplication.update -= UpdateStatus;

				if (instance == null && !cancelCurrentWork)
				{
					if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._ConfigPrepare, CurrentStatus, CurrentProgress))
					{
						CancelProgressBar ();
						cancelCurrentWork = true;
					}
				}
				CurrentProgress += ProgressStep;
			}
			catch (Exception)
			{}
		}

		static void CalculatePluginsFilters ()
		{
			foreach (var currentPlugin in pluginsList)
			{
				var sameBuildTypePlugins = 
					(from p in pluginsList
						where p.name == currentPlugin.name && 
						(p.buildTypes == null && currentPlugin.buildTypes == null ||
							p.buildTypes.Count == 0 && currentPlugin.buildTypes.Count == 0 ||
							(p.buildTypes.Count > 0 && currentPlugin.buildTypes.Count > 0 && p.buildTypes[0] == currentPlugin.buildTypes[0]))
						select p).ToList ();

				if (sameBuildTypePlugins.Count > 1)
				{
					var index = pluginsVersionFilters.FindIndex ((p) => { return p.Contains ((sameBuildTypePlugins.Count-1).ToString ()); });

					if (index == -1)
					{
						for (int i = 1; i < sameBuildTypePlugins.Count-1; i++)
						{
							if (i == 1 && !pluginsVersionFilters.Contains (AMEditorSystem.ContentCreatePublicFile._ActualVersionFilter))
							{
								pluginsVersionFilters.Add (AMEditorSystem.ContentCreatePublicFile._ActualVersionFilter);
							}
							else
							{
								if (!pluginsVersionFilters.Contains (AMEditorSystem.ContentCreatePublicFile._OtherVersionFilter + i.ToString ()))
									pluginsVersionFilters.Add (AMEditorSystem.ContentCreatePublicFile._OtherVersionFilter + i.ToString ());
							}
						}
					}
				}
			}
		}

		static void UpdateGUIpluginsList ()
		{
			guiPluginsList = new List<Plugin> ();

			releaseOnlyAvailability = (pluginsList.FindIndex ((p) => {return p.version.ToLower ().Contains ("dev") || p.version.ToLower ().Contains ("rc");}) != -1);

			foreach (var currentPlugin in pluginsList)
			{
				if (releaseOnly &&
					(currentPlugin.version.ToLower ().Contains ("dev") || 
						currentPlugin.version.ToLower ().Contains ("rc")))
					continue;

				if (selectedFilterIndex == 0)
				{
					if (!guiPluginsList.Contains (currentPlugin))
					{
						currentPlugin.selected = true;
						guiPluginsList.Add (currentPlugin);
					}
				}
				else
				{
					var index = guiPluginsList.FindIndex ((p) => {return p.name == currentPlugin.name && 
						(p.buildTypes == null && currentPlugin.buildTypes == null ||
							p.buildTypes.Count == 0 && currentPlugin.buildTypes.Count == 0 ||
							(p.buildTypes.Count > 0 && currentPlugin.buildTypes.Count > 0 && p.buildTypes[0] == currentPlugin.buildTypes[0]));});

					if (index == -1)
					{
						int filterValue = selectedFilterIndex;

						List<Plugin> sameBuildTypePlugins = new List<Plugin> ();

						sameBuildTypePlugins = releaseOnly 
							? 
							(from p in pluginsList
								where p.name == currentPlugin.name && 
								(p.buildTypes == null && currentPlugin.buildTypes == null ||
									p.buildTypes.Count == 0 && currentPlugin.buildTypes.Count == 0 ||
									(p.buildTypes.Count > 0 && currentPlugin.buildTypes.Count > 0 && p.buildTypes[0] == currentPlugin.buildTypes[0])) && 
								(!p.version.ToLower ().Contains ("dev") && !p.version.ToLower ().Contains ("rc"))
								select p).ToList ()
							:
							(from p in pluginsList
								where p.name == currentPlugin.name && 
								(p.buildTypes == null && currentPlugin.buildTypes == null ||
									p.buildTypes.Count == 0 && currentPlugin.buildTypes.Count == 0 ||
									(p.buildTypes.Count > 0 && currentPlugin.buildTypes.Count > 0 && p.buildTypes[0] == currentPlugin.buildTypes[0]))
								select p).ToList ();

						sameBuildTypePlugins = sameBuildTypePlugins.OrderByDescending ((p) => {
							return p.version.Contains ("-") ? new Version (p.version.Substring (0, p.version.IndexOf ('-'))) : new Version (p.version);
						}).ToList ();
						var pluginsCount = (sameBuildTypePlugins.Count < filterValue) ? sameBuildTypePlugins.Count : filterValue;
						for (int i = 0; i < pluginsCount; i++)
						{
							if (!guiPluginsList.Contains (sameBuildTypePlugins[i])) 
							{
								sameBuildTypePlugins[i].selected = true;
								guiPluginsList.Add (sameBuildTypePlugins[i]);
							}
						}
					}
				}
			}
			guiPluginsList = SortGUIPlugins (guiPluginsList);
		}

		static List<Plugin> SortGUIPlugins (List<Plugin> source)
		{
			if (orderByNameDescending)
			{
				if (orderByVersionDescending)
				{
					source = source.OrderByDescending (p => p.name).ThenByDescending ((p) => {
						return p.version.Contains ("-") ? new Version (p.version.Substring (0, p.version.IndexOf ('-'))) : new Version (p.version);
					}).ToList ();
				}
				else
				{
					source = source.OrderByDescending (p => p.name).ThenBy ((p) => {
						return p.version.Contains ("-") ? new Version (p.version.Substring (0, p.version.IndexOf ('-'))) : new Version (p.version);
					}).ToList ();
				}
			}
			else
			{
				if (orderByVersionDescending)
				{
					source = source.OrderBy (p => p.name).ThenByDescending ((p) => {
						return p.version.Contains ("-") ? new Version (p.version.Substring (0, p.version.IndexOf ('-'))) : new Version (p.version);
					}).ToList ();
				}
				else
				{
					source = source.OrderBy (p => p.name).ThenBy ((p) => {
						return p.version.Contains ("-") ? new Version (p.version.Substring (0, p.version.IndexOf ('-'))) : new Version (p.version);
					}).ToList ();
				}
			}

			return source;
		}

		void Update ()
		{
			if (EditorApplication.isCompiling)
			{
				Close ();
			}

			if (windowType == TypeWindow.Wait)
			{
				if (EditorApplication.timeSinceStartup > progressNextRepaintTime) 
				{
					progressFrameIndex++;
					if (progressFrameIndex == 8)
						progressFrameIndex = 0;

					Repaint ();

					progressNextRepaintTime = (float)(EditorApplication.timeSinceStartup + progressRepaintInterval);
				}
			}
			else
				progressNextRepaintTime = 0;
		}

		GUIStyle popupStyle;

		Color uiTextColor;
		Color proBackColor = new Color32 (56, 56, 56, 255);
		Color freeBackColor = new Color32 (194, 194, 194, 255);
		int buttonHeight = 18;
		#if AM_EDITOR_COMPACT_ON
			int buttonWidth = 24;
		#else
			int buttonWidth = 80;
		#endif
		void OnGUI ()
		{
			uiTextColor = new GUIStyle (GUI.skin.label).normal.textColor;

			popupStyle = new GUIStyle (EditorStyles.popup);
			popupStyle.fontSize = new GUIStyle (GUI.skin.button).fontSize;
			popupStyle.fixedHeight = buttonHeight;

			switch (windowType)
			{
			case TypeWindow.Main:
				try 
				{
					EditorUtility.ClearProgressBar ();
				}
				catch (Exception) 
				{}

				DrawHeader ();

				DrawPlugins ();

				DrawButtom ();

				break;
			case TypeWindow.Wait:
				GUIStyle labelStyle = new GUIStyle (GUI.skin.label);
				labelStyle.alignment = TextAnchor.MiddleCenter;

				GUI.Label (new Rect (4, Screen.height / 2 - 20, Screen.width - 4, 20), waitMessage, labelStyle);
				GUI.Label (new Rect (4, Screen.height / 2, Screen.width, 32), progressCircle [progressFrameIndex], labelStyle);

				//status bar
				GUIStyle statusBarStyle = new GUIStyle (GUI.skin.box);
				statusBarStyle.alignment = TextAnchor.MiddleLeft;
				statusBarStyle.normal.textColor = uiTextColor;

				string statusBarMessage = AMEditorSystem.ContentProgressBar._IsCompiling; 
				string statusBarProgress = string.Empty;

				if (CurrentStatus != string.Empty)
				{
					statusBarMessage = CurrentStatus;
					try
					{
						statusBarProgress = " " + ((Int32)(CurrentProgress * 100)).ToString () + "%";
					}
					catch (System.Exception)
					{}
				}
				if (EditorApplication.isCompiling)
				{
					statusBarMessage = AMEditorSystem.ContentProgressBar._IsCompiling;
					statusBarProgress = string.Empty;
				}

				GUI.Box (new Rect (-1, Screen.height - 20, Screen.width + 2, 21), statusBarMessage+"...", statusBarStyle);
				if (CurrentStatus != string.Empty && !EditorApplication.isCompiling) 
				{
					EditorGUI.ProgressBar (new Rect (Screen.width-290, Screen.height - 17, 200, 15), CurrentProgress, statusBarProgress);
					if (GUI.Button (new Rect (Screen.width-80, Screen.height - 20, 80, 20), AMEditorSystem.ContentStandardButton._Cancel))
					{
						try
						{
							CancelProgressBar ();
						}
						catch (System.Exception)
						{}
					}
				}
				break;
			default:
				break;
			}
		}

		bool selectAll = false;

        void DrawHeader ()
		{
			GUIStyle headerStyle = new GUIStyle (GUI.skin.label);
			headerStyle.wordWrap = true;

			GUIStyle headerSortButtonStyle = new GUIStyle (GUI.skin.textField);
			headerSortButtonStyle.wordWrap = true;
			headerSortButtonStyle.normal.background = new GUIStyle (GUI.skin.label).normal.background;//toggle off
			headerSortButtonStyle.onNormal.background = new GUIStyle (GUI.skin.label).normal.background;//toggle on
			headerSortButtonStyle.active.background = new GUIStyle (GUI.skin.textField).normal.background;//off to on
			headerSortButtonStyle.active.textColor = uiTextColor;
			headerSortButtonStyle.onActive.background = new GUIStyle (GUI.skin.textField).normal.background;//on to off
			headerSortButtonStyle.onActive.textColor = uiTextColor;

			GUI.Box (new Rect (-1, -1, Screen.width + 2, 28), "");

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("", GUILayout.ExpandWidth (true));

			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreatePublicFile._VersionFilter, AMEditorSystem.ContentCreatePublicFile._VersionFilterHelp), GUILayout.ExpandWidth (false));
			tempFilterIndex = selectedFilterIndex;
			GUI.enabled = pluginsVersionFilters.Count > 1;
			selectedFilterIndex = EditorGUILayout.Popup (selectedFilterIndex, pluginsVersionFilters.ToArray (), popupStyle, GUILayout.Width (110));
			GUI.enabled = true;

			GUI.enabled = releaseOnlyAvailability;
			bool tempReleaseOnly = releaseOnly;
			releaseOnly = GUILayout.Toggle (releaseOnly, new GUIContent (AMEditorSystem.ContentCreatePublicFile._ReleaseOnly, AMEditorSystem.ContentCreatePublicFile._ReleaseOnlyHelp), new GUIStyle (GUI.skin.label), GUILayout.ExpandWidth (false));
			releaseOnly = GUILayout.Toggle (releaseOnly, new GUIContent ("", AMEditorSystem.ContentCreatePublicFile._ReleaseOnlyHelp), GUILayout.Width (20));
			GUI.enabled = true;
			if (tempFilterIndex != selectedFilterIndex || tempReleaseOnly != releaseOnly)
			{
				UpdateGUIpluginsList ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();

			selectAll = guiPluginsList.TrueForAll ((p) => {return p.selected;});

			GUIStyle toggleAllStyle = new GUIStyle (GUI.skin.toggle);
			toggleAllStyle.normal.background = selectAll ? new GUIStyle (GUI.skin.toggle).onNormal.background : new GUIStyle (GUI.skin.toggle).normal.background;
			string toggleAllHelp = selectAll ? AMEditorSystem.ContentStandardButton._NoneHelp : AMEditorSystem.ContentStandardButton._AllHelp;

			if (GUILayout.Button (new GUIContent ("", toggleAllHelp), toggleAllStyle, GUILayout.Width (16)))
			{
				selectAll = !selectAll;
				foreach (var plugin in guiPluginsList) 
				{
					plugin.selected = selectAll;
				}
			}
			bool tempOrderByNameDescending = orderByNameDescending;
			orderByNameDescending = GUILayout.Toggle (orderByNameDescending, 
				new GUIContent (AMEditorSystem.ContentCreatePublicFile._PluginName, AMEditorSystem.ContentCreatePublicFile._PluginNameHelp), 
				headerSortButtonStyle, new GUILayoutOption[] {GUILayout.Width (Screen.width/6-16), GUILayout.Height (30)});
			
			GUI.Label (new Rect (GUILayoutUtility.GetLastRect ().x + GUILayoutUtility.GetLastRect ().width - 18, GUILayoutUtility.GetLastRect ().y, 18, 18), 
				new GUIContent (orderByNameDescending ? orderDescendingTexture : orderFrontTexture, AMEditorSystem.ContentCreatePublicFile._SortByNameHelp));

			bool tempOrderByVersionDescending = orderByVersionDescending;
			orderByVersionDescending = GUILayout.Toggle (orderByVersionDescending, 
				new GUIContent (AMEditorSystem.ContentCreatePublicFile._PluginVersion, AMEditorSystem.ContentCreatePublicFile._PluginVersionHelp), 
				headerSortButtonStyle, new GUILayoutOption[] {GUILayout.Width (Screen.width/9), GUILayout.Height (30)});
			
			GUI.Label (new Rect (GUILayoutUtility.GetLastRect ().x + GUILayoutUtility.GetLastRect ().width - 18, GUILayoutUtility.GetLastRect ().y, 18, 18), 
				new GUIContent (orderByVersionDescending ? orderDescendingTexture : orderFrontTexture, AMEditorSystem.ContentCreatePublicFile._SortByVersionHelp));
			if (tempOrderByNameDescending != orderByNameDescending || tempOrderByVersionDescending != orderByVersionDescending)
			{
				UpdateGUIpluginsList ();
			}

			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreatePublicFile._IsMandatory, AMEditorSystem.ContentCreatePublicFile._IsMandatoryHelp), headerStyle, GUILayout.Width (116));
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreatePublicFile._BuildTypes, AMEditorSystem.ContentCreatePublicFile._BuildTypesHelp), headerStyle, GUILayout.Width (80));
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreatePublicFile._DisplayType, AMEditorSystem.ContentCreatePublicFile._DisplayTypeHelp), headerStyle, GUILayout.Width (96));
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreatePublicFile._OldNames, AMEditorSystem.ContentCreatePublicFile._OldNamesHelp), headerStyle, GUILayout.Width (Screen.width/10f));
			GUILayout.Space (14);
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreatePublicFile._Depends, AMEditorSystem.ContentCreatePublicFile._DependsHelp), headerStyle, GUILayout.Width (Screen.width/8.33f));
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreatePublicFile._Url, AMEditorSystem.ContentCreatePublicFile._UrlHelp), GUILayout.Width (70));
			GUILayout.EndHorizontal ();

			GUI.Box (new Rect (-1, 58, Screen.width + 2, 1), "");
		}

		void DrawPlugins ()
		{
			scroll = GUILayout.BeginScrollView (scroll);

			if (guiPluginsList != null && guiPluginsList.Count > 0)
			{
				for (int i = 0; i < guiPluginsList.Count; i++)
				{
					var source = guiPluginsList[i];

					List<string> dependsList = new List<string> ();
					foreach (var item in source.depends)
					{
						dependsList.Add (item.name+" "+item.version);
					}

					string isMandatoryLabel =string.Empty;

					GUIStyle isMandatoryStyle = new GUIStyle (GUI.skin.toggle);
					isMandatoryLabel = source.mandatory ? AMEditorSystem.ContentCreatePublicFile._IsMandatoryTrue : AMEditorSystem.ContentCreatePublicFile._IsMandatoryFalse;
					isMandatoryStyle.normal.textColor = source.mandatory ? Color.black : Color.gray;
					isMandatoryStyle.onActive.textColor = source.mandatory ? Color.gray : Color.black;

					GUIStyle selectorStyle = new GUIStyle (EditorStyles.foldout);
					selectorStyle.focused.background = selectorStyle.normal.background;
					selectorStyle.normal.textColor = EditorGUIUtility.isProSkin ? proBackColor : freeBackColor;
					selectorStyle.focused.textColor = EditorGUIUtility.isProSkin ? proBackColor : freeBackColor;

					GUILayout.BeginHorizontal ();
					source.selected = GUILayout.Toggle (source.selected, new GUIContent ("", AMEditorSystem.ContentCreatePublicFile._SelectHelp), GUILayout.Width (10));

					GUI.enabled = source.selected;

					GUILayout.Label (new GUIContent (source.name, AMEditorSystem.ContentCreatePublicFile._PluginNameHelp), GUILayout.Width (Screen.width/6));

					GUILayout.Label (new GUIContent (source.version, AMEditorSystem.ContentCreatePublicFile._PluginVersionHelp), GUILayout.Width (Screen.width/9));

					source.mandatory = GUILayout.Toggle (source.mandatory, new GUIContent (isMandatoryLabel, AMEditorSystem.ContentCreatePublicFile._IsMandatoryHelp), isMandatoryStyle, GUILayout.Width (116));

					if (source.buildTypes.Count > 1)
					{
						int buildTypeIndex = 0;
						buildTypeIndex = EditorGUILayout.Popup (buildTypeIndex, source.buildTypes.ToArray (), selectorStyle, GUILayout.Width (10));
						GUILayout.Label (source.buildTypes[buildTypeIndex], GUILayout.Width (70));
					}
					else if (source.buildTypes.Count == 1)
					{
						GUILayout.Label (source.buildTypes[0], GUILayout.Width (80));
					}
					else
					{
						GUILayout.Label (new GUIContent ("*universal", AMEditorSystem.ContentCreatePublicFile._BuildTypesNoneHelp), GUILayout.Width (80));
					}

					GUILayout.Label (new GUIContent (source.displayType, AMEditorSystem.ContentCreatePublicFile._DisplayTypeHelp), GUILayout.Width (80));

					if (source.oldNames.Count > 1)
					{
						int oldNameIndex = 0;
						oldNameIndex = EditorGUILayout.Popup (oldNameIndex, source.oldNames.ToArray (), selectorStyle, GUILayout.Width (10));
						GUILayout.Label (new GUIContent (source.oldNames[oldNameIndex], source.oldNames[oldNameIndex]), GUILayout.Width (Screen.width/10f));
					}
					else if (source.oldNames.Count == 1)
					{
						GUILayout.Space (14);
						GUILayout.Label (new GUIContent (source.oldNames[0], source.oldNames[0]), GUILayout.Width (Screen.width/10f));
					}
					else
					{
						GUILayout.Space (14);
						GUILayout.Label ("", GUILayout.Width (Screen.width/10f));
					}

					if (dependsList.Count > 1)
					{
						int dependNameIndex = 0;
						dependNameIndex = EditorGUILayout.Popup (dependNameIndex, dependsList.ToArray (), selectorStyle, GUILayout.Width (10));
						GUILayout.Label (new GUIContent (dependsList[dependNameIndex], dependsList[dependNameIndex]), GUILayout.Width (Screen.width/8.33f));
					}
					else if (dependsList.Count == 1)
					{
						GUILayout.Space (14);
						GUILayout.Label (new GUIContent (dependsList[0], dependsList[0]), GUILayout.Width (Screen.width/8.33f));
					}
					else
					{
						GUILayout.Space (14);
						GUILayout.Label ("", GUILayout.Width (Screen.width/8.33f));
					}
					GUILayout.Label (new GUIContent (source.urlMasterBranch, source.urlMasterBranch), GUILayout.MinWidth (120));
					GUI.enabled = true;
					GUILayout.EndHorizontal ();
				}
			}
			GUILayout.EndScrollView ();
		}

		void DrawButtom ()
		{
			GUIStyle pushConfigHelpStyle = new GUIStyle (GUI.skin.label);
			pushConfigHelpStyle.fontStyle = FontStyle.Italic;
			pushConfigHelpStyle.normal.textColor = Color.gray;

			Rect buttomSeparatorRect = pushConfig ? new Rect (-1, Screen.height - 162, Screen.width + 2, 1) : new Rect (-1, Screen.height - 62, Screen.width + 2, 1);
			float labelHeight = pushConfig ? 160 : 60;
			GUI.Box (buttomSeparatorRect, "");
			GUILayout.Label ("", new GUILayoutOption[]{GUILayout.Height (labelHeight)});

			// buttons
			#if AM_EDITOR_COMPACT_ON
				GUIContent updateButtonContent = new GUIContent (updateButtonTexture, AMEditorSystem.ContentCreatePublicFile._UpdateFromGitHelp);
				GUIContent createButtonContent = new GUIContent (saveButtonTexture, AMEditorSystem.ContentCreatePublicFile._CreateHelp);
				Rect createButtonRect = new Rect (Screen.width - (buttonWidth + 12), Screen.height - 52, buttonWidth + 2, buttonWidth);
			#else
				GUIContent updateButtonContent = new GUIContent (AMEditorSystem.ContentCreatePublicFile._UpdateFromGit, AMEditorSystem.ContentCreatePublicFile._UpdateFromGitHelp);
				GUIContent createButtonContent = new GUIContent (pushConfig ? AMEditorSystem.ContentCreatePublicFile._CreateAndPush : AMEditorSystem.ContentCreatePublicFile._Create, AMEditorSystem.ContentCreatePublicFile._CreateHelp);
				Rect createButtonRect = new Rect (Screen.width - (buttonWidth * 2 + 10), Screen.height - 50, buttonWidth * 2, buttonHeight);
			#endif
			Rect pushConfigToggleRect = pushConfig ? new Rect (10, Screen.height - 160, 270, buttonHeight) : new Rect (10, Screen.height - 60, Screen.width/2, buttonHeight);
			Rect commitMessageBoxRect = pushConfig ? new Rect (10, Screen.height - 142, Screen.width/2, 118) : new Rect (10, Screen.height - 42, Screen.width/2, buttonHeight);

			if (GUI.Button (new Rect (4, 4, buttonWidth, buttonHeight), updateButtonContent))
			{
				CurrentProgress = 0;
				SearchConfig ();
			}

			string forceSearchToggleHelp = forceSearch ? AMEditorSystem.ContentCreatePublicFile._ForceUpdateFromGitHelp_True : AMEditorSystem.ContentCreatePublicFile._ForceUpdateFromGitHelp_False;
			GUIContent forceSearchToggleContent = new GUIContent (AMEditorSystem.ContentCreatePublicFile._ForceUpdateFromGit, forceSearchToggleHelp);
			forceSearch = GUI.Toggle (new Rect (8 + buttonWidth, 4, buttonWidth * 2, buttonHeight), forceSearch, forceSearchToggleContent);

			int index = -1;
			if (guiPluginsList != null && guiPluginsList.Count > 0)
			{
				index = guiPluginsList.FindLastIndex ((p)=> {return (p.selected == true);});
			}
			GUI.enabled = (index != -1);

			pushConfig = GUI.Toggle (pushConfigToggleRect, pushConfig, AMEditorSystem.ContentCreatePublicFile._PushingConfigToggle);
			if (pushConfig)
				GUI.Label (new Rect (pushConfigToggleRect.width + 10, pushConfigToggleRect.y, pushConfigToggleRect.width, pushConfigToggleRect.height), 
					AMEditorSystem.ContentCreatePublicFile._PushingConfigToggleHelp+AMEditorSystem.Git._BranchName, pushConfigHelpStyle);
			if (!pushConfig)
			{
				CommitMessage = string.Empty;
				GUI.FocusControl ("");
			}
			GUI.enabled = pushConfig;
			CommitMessage = GUI.TextArea (commitMessageBoxRect, CommitMessage);
			GUI.enabled = true;

			if (GUI.Button (createButtonRect, createButtonContent))
			{
				var fullPath = EditorUtility.SaveFilePanel (AMEditorSystem.ContentCreatePublicFile._OutFolder, "Assets", "ameditor_plugins", "json");

				if (fullPath != string.Empty)
				{
					string directory = fullPath.Substring (0, fullPath.LastIndexOf ("/"));
					string contentString = string.Empty;
					try 
					{
						ArrayList ameditor_plugins = new ArrayList ();
						foreach (var item in guiPluginsList)
						{
							if (item.selected)
							{
								Hashtable preparePlugin = item.ToHashtable ();
								preparePlugin.Remove ("files");
								preparePlugin.Remove ("permitted_versions");
								ameditor_plugins.Add (preparePlugin);
							}
						}
						contentString = AMEditorJSON.JsonEncode (ameditor_plugins);

						AMEditorFileStorage.CreateFolder (directory, false);
						string formattedContent = AMEditorJSON.FormatJson (contentString);
						File.WriteAllText (fullPath, formattedContent);

						if (pushConfig)
						{
							if (!string.IsNullOrEmpty (CommitMessage))
								PushConfig (formattedContent, CommitMessage);
							else
							{
								new UI.AMDisplayDialog (AMEditorSystem.ContentCreatePublicFile._EmptyCommitTitle, AMEditorSystem.ContentCreatePublicFile._EmptyCommitMessage, 
									AMEditorSystem.ContentStandardButton._Ok, "", () => {}, () => {}, true).Show ();
								return;
							}
						}
					}
					catch (System.Exception) 
					{}
					this.Close ();
					if (!pushConfig)
						UnityEditor.EditorUtility.OpenWithDefaultApp (directory);
				}
			}
			GUI.enabled = true;

			// status bar
			GUIStyle statusBarStyle = new GUIStyle (GUI.skin.box);
			statusBarStyle.alignment = TextAnchor.MiddleLeft;
			statusBarStyle.normal.textColor = uiTextColor;

			try 
			{
				int selectedPlugins = (from p in guiPluginsList where p.selected == true select p).ToArray ().Length;
				string statusMessage = selectedPlugins > 0 ? 
					AMEditorSystem.ContentCreatePublicFile._SelectedPlugins + selectedPlugins + "/"+guiPluginsList.Count : 
					AMEditorSystem.ContentCreatePublicFile._PluginsCount + guiPluginsList.Count;

				GUI.Box (new Rect (-1, Screen.height - 20, Screen.width + 2, 21), statusMessage, statusBarStyle);
			}
			catch (System.Exception) 
			{}
		}

		void OnFocus ()
		{
			if (!isInit)
			{
				isInit = true;
				instance = this;
			}
		}

		void OnDisable ()
		{
			isInit = false;
		}

		void OnDestroy ()
		{
			instance = null;
			pluginsList = null;
			guiPluginsList = null;
			localPluginsList = null;
			isInit = false;
		}
	}
}
#endif