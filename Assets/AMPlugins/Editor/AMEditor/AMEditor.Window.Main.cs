#if UNITY_EDITOR
#pragma warning disable
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System; 

namespace AMEditor
{
	[InitializeOnLoad]
	class PluginsCheckUpdateOnLoad
	{
		static string[] invalidProductNameSymbols = new string[]{"&", "'", "`", "–", "\u001f—"};

		static bool start = false;
		static PluginsCheckUpdateOnLoad ()
		{
		#if UNITY_WEBPLAYER
			if (EditorUtility.DisplayDialog (AMEditorSystem.ContentOtherWindow._TitleWarning, AMEditorSystem.ContentOtherWindow._WebPlayerMessage, AMEditorSystem.ContentStandardButton._Ok))
			{
				EditorWindow.GetWindow (System.Type.GetType ("UnityEditor.BuildPlayerWindow, UnityEditor"), true, "Build Settings");
			}
		#endif
			if (EditorApplication.timeSinceStartup < 15.0f)
			{
				EditorApplication.update += Update;
			}
			if (AMEditor.WindowMain.isInit)
			{
				AMEditor.WindowMain.Init ();
			}
			#if !(UNITY_2018_2_OR_NEWER || UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER)
				if (EditorApplication.projectWindowChanged.GetInvocationList ().ToList ().FindIndex (d => d.Method.Name.Contains ("CheckAssetBundlesAvailability")) == -1)
					EditorApplication.projectWindowChanged += CustomResourceManagerAPI.CheckAssetBundlesAvailability;
			#endif

			if (invalidProductNameSymbols.Any (PlayerSettings.productName.Contains))
			{
				if (EditorUtility.DisplayDialog (AMEditorSystem.ContentOtherWindow._TitleErrorPopup, AMEditorSystem.ContentOtherWindow._InvalidProductNameSymbolsMessage, 
					    AMEditorSystem.ContentStandardButton._Ok, AMEditorSystem.ContentStandardButton._Cancel))
				{
					PlayerSettings.productName = PlayerSettings.productName.Replace ("&", "and");
					PlayerSettings.productName = PlayerSettings.productName.Replace ("'", "");
					PlayerSettings.productName = PlayerSettings.productName.Replace ("`", "");
					PlayerSettings.productName = PlayerSettings.productName.Replace ("–", "-");
					PlayerSettings.productName = PlayerSettings.productName.Replace ("\u001f—", "-");
				}
			}
				
			if (PlayerSettings.productName.Any (c => System.Convert.ToInt32 (c) > 128))
			{
				EditorUtility.DisplayDialog (AMEditorSystem.ContentOtherWindow._TitleWarning, AMEditorSystem.ContentOtherWindow._NonLatinProductNameSymbolsMessage, 
					AMEditorSystem.ContentStandardButton._Ok);
			}
		}

		static void Update ()
		{
			if ((2.0f < EditorApplication.timeSinceStartup) && (EditorApplication.timeSinceStartup < 15.0f) && !start) 
			{
				start = true;
				EditorApplication.update -= Update;
				var path = LocalRepositoryAPI.pathToRepository;
				if (!string.IsNullOrEmpty (path))
				{
					StartProject ();
				}
				CustomResourceManagerAPI.CheckAssetBundlesAvailability ();
			}
		}

		static void StartProject ()
		{
			if (!AMEditor.WindowMain.SearchConfigFile ()) 
			{
				if (AMEditor.WindowMain.firstStart) 
					AMEditor.WindowMain.Init ();
				return;
			}
			if (AMEditor.WindowMain.CheckPublicRepo ())
			{
				AMEditor.WindowMain.checkFromStartProject = true;
				AMEditor.WindowMain.CheckUpdate ();
				CustomResourceManagerAPI.CheckCorrect ();
			}
			HelpAPI.ForceDownload ();
		}
	}

	public class WindowAMEditor : EditorWindow
	{
		//for update issue
	}
	
	public class WindowMain : EditorWindow
	{
		public enum WindowType
		{
			ListPlugins = 1, 
			FixConfict, 
			Backup, 
			Authorization, 
			Wait, 
			CriticalError
		}

		public enum PluginsStatus
		{
			AllGood, 
			BuildTypeProblems, 
			MissingFiles, 
			HasConflicts, 
			ChangedFiles, 
			NeedDepends, 
			NeedUpdate, 
			NeedMandatory, 
			OutdatedPlugins
		}

		public enum LaunchMode
		{
			UI, 
			Console
		}

		public static AMEditor.WindowMain instance = null;
		public static Rect currentWindowPosition = new Rect (0, 0, 0, 0);
		private const int buffer_size = 100;
		string tempWindowType = string.Empty;
		List<PluginGUI> installedPluginsGUIList = null;
		List<PluginGUI> mandatoryPluginsGUIList = null;
		List<PluginGUI> otherPluginsGUIList = null;
		List<ExternalPluginGUI> externalPluginsGUIList = null;
		public static bool amEditorIsBusy = false;
		public static bool authError = false;
		static bool cancelCurrentWork = false;
		public static bool checkFromStartProject = false;
		bool allInstalledSelected = false;
		bool someInstalledSelected = false;
		#if AM_EDITOR_VIEW_TYPE_EXTENDED
			bool updatabelInstalledSelected = false;
		#endif		
		bool allMandatorySelected = false;
		bool someMandatorySelected = false;
		bool allOtherSelected = false;
		bool someOtherSelected = false;
		int buttonHeight = 18;
		#if AM_EDITOR_COMPACT_ON
			int buttonWidth = 24;
			int wideButtonWidth = 24;
		#else
			int buttonWidth = 80;
			int wideButtonWidth = 160;
		#endif
		static int selectedBuildTypeIndex;
		static bool cancelPressed = false;
		Vector2 scroll = Vector2.zero;
		public static WindowType currentWindow = WindowType.ListPlugins;
		public PluginsStatus pluginsStatus = PluginsStatus.AllGood;
		public static LaunchMode launchMode = LaunchMode.UI;
		#if AM_EDITOR_VIEW_TYPE_EXTENDED
			Plugin currentPlugin;
		#endif
		public static List<Plugin> installedPlugins;
		List<Plugin> tempDownloadedPlugins;
		List<Backup> backups;
		static List<ActualPlugin> actualPlugins;
		static List<ExternalPlugin> externalPlugins;
		static List<string> pluginsWhitelist;
		public static List<string> DownloadQueue;
		static List<string> CheckList;
		static List<string> pluginsBuildtypes;
		public static string selectedBuildType = "standard";
		public static string compilationHandlerName = string.Empty;
		public static List<string> listLocationConfig = new List<string>{"Assets/AMPlugins", "Assets/Plugins"};
		static string groupName = "Unity Plugins";
		static string branchName = "ameditor";
		static string resourceFolder = string.Empty;

		#if AM_EDITOR_VIEW_TYPE_EXTENDED
            #if UNITY_2018_2_OR_NEWER || UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER
                Texture _fixmeTexture;
                public Texture fixmeTexture
                {
                    get
                    {
                        return _fixmeTexture;
                    }

                    set
                    {
                        _fixmeTexture = value;
                    }
                }
            #else
			Texture fixmeTexture = new Texture ();
            #endif
		#endif

		#if UNITY_2018_2_OR_NEWER || UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER
			Texture pluginErrorTexture;
			public Texture PluginErrorTexture
        	{
            	get
            	{
                	return pluginErrorTexture;
            	}

           		set
            	{
                	pluginErrorTexture = value;
            	}
        	}
			Texture pluginWarnTexture;
        	public Texture PluginWarnTexture
        	{
            	get
            	{
                	return pluginWarnTexture;
            	}

            	set
            	{
                	pluginWarnTexture = value;
            	}
        	}
			Texture pluginOkTexture;
        	public Texture PluginOkTexture
        	{
            	get
            	{
                	return pluginOkTexture;
            	}

            	set
            	{
                	pluginOkTexture = value;
            	}
        	}
		Texture infoTexture;
        public Texture InfoTexture
        {
            get
            {
                return infoTexture;
            }

            set
            {
                infoTexture = value;
            }
        }
		Texture pluginSelectorBackground;
        public Texture PluginSelectorBackground
        {
            get
            {
                return pluginSelectorBackground;
            }

            set
            {
                pluginSelectorBackground = value;
            }
        }
			#else
				Texture pluginErrorTexture = new Texture ();
				Texture pluginWarnTexture = new Texture ();
				Texture pluginOkTexture = new Texture ();
				Texture infoTexture = new Texture ();
				Texture pluginSelectorBackground = new Texture ();
			#endif

		#if AM_EDITOR_COMPACT_ON
        #if UNITY_2018_2_OR_NEWER || UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER
        Texture _logoutButtonTexture;
        public Texture logoutButtonTexture{
            get{ return _logoutButtonTexture; }
            set{ _logoutButtonTexture = value; }
        }
        Texture _updateButtonTexture;
        public Texture updateButtonTexture{
            get{ return _updateButtonTexture; }
            set{ _updateButtonTexture = value; }
        }
        Texture _listButtonTexture;
        public Texture listButtonTexture{
            get{ return _listButtonTexture; }
            set{ _listButtonTexture = value; }
        }
        Texture _openButtonTexture;
        public Texture openButtonTexture{
            get{ return _openButtonTexture; }
            set{ _openButtonTexture = value; }
        }
        Texture _gitlabButtonTexture;
        public Texture gitlabButtonTexture{
            get { return _gitlabButtonTexture; }
            set { _gitlabButtonTexture = value; }
        }
        Texture _deleteButtonTexture;
        public Texture deleteButtonTexture{
            get{ return _deleteButtonTexture; }
            set{ _deleteButtonTexture = value; }
        }
        Texture _exampleButtonTexture;
        public Texture exampleButtonTexture{
            get { return _exampleButtonTexture; }
            set { _exampleButtonTexture = value; }
        }
        Texture _expandOnButtonTexture;
        public Texture expandOnButtonTexture{
            get{ return _expandOnButtonTexture; }
            set{ _expandOnButtonTexture = value; }
        }
        Texture _expandOffButtonTexture;
        public Texture expandOffButtonTexture{
            get{ return _expandOffButtonTexture; }
            set{ _expandOffButtonTexture = value; }
        }
        Texture _backButtonTexture;
        public Texture backButtonTexture{
            get{ return _backButtonTexture; }
            set{ _backButtonTexture = value; }
        }
        Texture _closeButtonTexture;
        public Texture closeButtonTexture{
            get{ return _closeButtonTexture; }
            set{ _closeButtonTexture = value; }
        }
        static Texture _downloadButtonTexture;
        public static Texture downloadButtonTexture{
            get{ return _downloadButtonTexture; }
            set{ _downloadButtonTexture = value; }
        }
        static Texture _descriptionButtonTexture;
        public static Texture descriptionButtonTexture{
            get { return _descriptionButtonTexture; }
            set { _descriptionButtonTexture = value; }
        }
        #else
			Texture logoutButtonTexture = new Texture ();
			Texture updateButtonTexture = new Texture ();
			Texture listButtonTexture = new Texture ();
			Texture openButtonTexture = new Texture ();
			Texture gitlabButtonTexture = new Texture ();
			Texture deleteButtonTexture = new Texture ();
			Texture exampleButtonTexture = new Texture ();
			Texture expandOnButtonTexture = new Texture ();
			Texture expandOffButtonTexture = new Texture ();
			Texture backButtonTexture = new Texture ();
			Texture closeButtonTexture = new Texture ();
			public static Texture downloadButtonTexture = new Texture ();
			public static Texture descriptionButtonTexture = new Texture ();
        #endif
		#endif
		public static List<Texture> progressCircle = new List<Texture> ();
		static int progressFrameIndex;
		float progressRepaintInterval = 0.1f;
		float progressNextRepaintTime = 0;
		double restoringCRMStatusTime = 0;
		public static string messageWait = "...";
		public static bool isInit = false;
		public static bool firstStart = false;
		static bool needForceCheck;
		static bool isDownloadPlugin = false;
		static Plugin CurrentDownloadPlugin;
		static string CurrentStatusPlugin = string.Empty; 
		static string CurrentPluginData = string.Empty; 
		static float CurrentProgressStep = 0.0f; 
		static float CurrentProgressDownloads = 0.0f; 
		static ActualPlugin CurrentDownloadActualPlugin;
		static string makeAllGoodButtonLabel = AMEditorSystem.ContentMakeAllGoodButton._AllGood;
		static string makeAllGoodButtonHelp = AMEditorSystem.ContentMakeAllGoodButton._AllGoodHelp;

		#if UNITY_2018_2_OR_NEWER || UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER
			Texture makeAllGoodStatusImage;
			public Texture MakeAllGoodStatusImage
        	{
            	get
            	{
             		return makeAllGoodStatusImage;
            	}

            	set
            	{
                	makeAllGoodStatusImage = value;
         		}
        	}
		#else
			static Texture makeAllGoodStatusImage = new Texture ();
		#endif
		public static string AMEDITOR_CONFIG_FILENAME = "";//"Assets/AMPlugins/Editor/AMEditor/ameditor_config.json";
		static string NEED_DISPLAY_SUCCESS_DIALOG = "";//"display_dialog_success.json";
		public static string PLUGIN_CONFIG_FILENAME = "";//"plugin_config.json";
		static string ACTUAL_VERSION_PLUGINS_FILENAME = "";//"ameditor_plugins.json";
		static string EXTERNAL_PLUGINS_FILENAME = "";
		static string AMEDITOR_WHITELIST_FILENAME = "";//"ameditor_whitelist.json"
		static string LOCAL_REPOSITORY = "";//"Downloads";
		static string LINK_PUBLIC_REPO = "";//"https://gitlab.digital-ecosystems.ru/unity - plugins/ameditor-plugins/raw/master/ameditor_plugins.json";
		static string LINK_PUBLIC_REPO_EXTERNAL_PLUGIN = "";
		static string LINK_PUBLIC_REPO_WHITELIST = "";//"https://gitlab.digital-ecosystems.ru/unity - plugins/ameditor-plugins/raw/master/ameditor_whitelist.json";
		static string FILE_NAME_RC_MODE = "ameditor_rc_mode.json";
		public static string FILE_NAME_STATE_EDITOR = "ameditor_state.json";
		void OnEnable ()
		{
			resourceFolder = EditorGUIUtility.isProSkin ? "pro" : "free";

			#if AM_EDITOR_VIEW_TYPE_EXTENDED
				fixmeTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_fixme.png");
			#endif
			pluginErrorTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_error.png");
			pluginWarnTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_warn.png");
			pluginOkTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_ok.png");
			infoTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_info.png");
			pluginSelectorBackground = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/am_editor_selector.png");
			#if AM_EDITOR_COMPACT_ON
				logoutButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/Compact/am_editor_logout.png");
				updateButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/Compact/am_editor_update.png");
				listButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_list.png");
				openButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/Compact/am_editor_open.png");
				gitlabButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_gitlab.png");
				deleteButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/Compact/am_editor_delete.png");
				exampleButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/Compact/am_editor_example.png");
				expandOnButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/Compact/am_editor_expand_on.png");
				expandOffButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/Compact/am_editor_expand_off.png");
				backButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/Compact/am_editor_back.png");
				closeButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/Compact/am_editor_close.png");
				downloadButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/Compact/am_editor_download.png");
				descriptionButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/Compact/am_editor_about.png");
			#endif
			progressCircle = new List<Texture> {AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_0.png"), 
				AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_1.png"), 
				AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_2.png"), 
				AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_3.png"), 
				AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_4.png"), 
				AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_5.png"), 
				AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_6.png"), 
				AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/" + resourceFolder + "/progress/am_editor_progress_7.png")};
		}
		public static void ClearProgressBar ()
		{
			EditorUtility.ClearProgressBar ();
		}
		public static void Show ()
		{
			Init ();
		}
		public static int CheckUpdate () 
		{
//			AMEditor.DLC.CheckUpdate ();
			return CheckPluginsUpdate ();
		}
		public static int SilentCheckUpdate ()
		{
			return CheckPluginsUpdate (true);
		}
		public static void FixMetaA ()
		{
			bool success = true;

			success = FixMetaALibs ();

			if (success) 
			{
				EditorUtility.DisplayDialog (AMEditorSystem.ContentFixMetaA._SuccessFixTitle, AMEditorSystem.ContentFixMetaA._SuccessFix, AMEditorSystem.ContentStandardButton._Ok);
			}
			else
			{
				AssetDatabase.Refresh ();

				if (EditorUtility.DisplayDialog (AMEditorSystem.ContentFixMetaA._FailedFixTitle, AMEditorSystem.ContentFixMetaA._FailedFix, 
					AMEditorSystem.ContentFixMetaA._RepeatFix, AMEditorSystem.ContentStandardButton._Ok))
				{
					FixMetaA ();
				}
			}
			AssetDatabase.Refresh ();
		}
		static bool FixMetaALibs ()
		{
			var importers = PluginImporter.GetImporters (BuildTarget.iOS).ToList ();
			importers.AddRange (PluginImporter.GetImporters (BuildTarget.tvOS).ToList ());
			
			foreach (var item in importers)
			{
				try
				{
					if (!item.assetPath.EndsWith (".a"))
						continue;

					#if AM_EDITOR_DEBUG_MODE_ON
						string path = LocalRepositoryAPI.pathToRepository + LOCAL_REPOSITORY + "platforms.txt";
						string content = string.Empty;					

						if (File.Exists (path))
							File.Delete (path);
						File.Create (path).Dispose ();
					#endif

					item.SetCompatibleWithAnyPlatform (false);
					item.SetCompatibleWithEditor (false);

					var targets = Enum.GetValues (typeof(BuildTarget)).OfType<BuildTarget> ().ToList ();
					foreach (var bt in targets)
					{
						if (bt.ToString ().Contains ("NoTarget") || 
							bt.ToString ().Contains ("XBOX360") || 
							bt.ToString ().Contains ("MetroPlayer") || 
							bt.ToString ().Contains ("BB10") || 
							bt.ToString ().Contains ("BlackBerry") || 
							bt.ToString ().Contains ("iPhone") || 
							bt.ToString ().Contains ("iOS") || 
							bt.ToString ().Contains ("tvOS"))
							continue;

						#if AM_EDITOR_DEBUG_MODE_ON
							content += bt.ToString () + "\n";
							File.WriteAllText (path, content);
						#endif

						item.SetCompatibleWithPlatform (bt, false);
					}

					if (item.assetPath.Contains ("tvOS"))
					{
						item.SetCompatibleWithPlatform (BuildTarget.iOS, false);
						item.SetCompatibleWithPlatform (BuildTarget.tvOS, true);
						item.SetPlatformData (BuildTarget.tvOS, "FrameworkDependencies", "CoreData;");
					}
					if (item.assetPath.Contains ("iOS"))
					{
						item.SetCompatibleWithPlatform (BuildTarget.tvOS, false);
						item.SetCompatibleWithPlatform (BuildTarget.iOS, true);
						item.SetPlatformData (BuildTarget.iOS, "FrameworkDependencies", "CoreData;CoreTelephony;");
					}
					item.SaveAndReimport ();
				}
				catch (Exception)
				{
					return false;
				}
			}
			return true;
		}

		public static void FixMetaBundle ()
		{
			var bun = CustomPostProcessBuild.SearchBundle ("Assets");
			bool success = true;
			foreach (var item in bun) 
			{
				if (!AddCorrectMeta (item + ".meta", "standalone"))
				{
					success = false;
					break;
				}
			}
			if (success) 
			{
				EditorUtility.DisplayDialog (AMEditorSystem.ContentFixMetaBundle._SuccessFixTitle, AMEditorSystem.ContentFixMetaBundle._SuccessFix, AMEditorSystem.ContentStandardButton._Ok);
			}
			else
			{
				AssetDatabase.Refresh ();

				if (EditorUtility.DisplayDialog (AMEditorSystem.ContentFixMetaBundle._FailedFixTitle, AMEditorSystem.ContentFixMetaBundle._FailedFix, 
					AMEditorSystem.ContentFixMetaBundle._RepeatFix, AMEditorSystem.ContentStandardButton._Ok))
				{
					FixMetaBundle ();
				}
			}
			AssetDatabase.Refresh ();
		}

		public static bool MakeMetaVisible ()
		{
			if (!UnityEditor.EditorSettings.externalVersionControl.Equals ("Visible Meta Files"))
			{
				UnityEditor.EditorSettings.externalVersionControl = "Visible Meta Files";
				if (!UnityEditor.EditorSettings.externalVersionControl.Equals ("Visible Meta Files"))
				{
					if (EditorUtility.DisplayDialog (AMEditorSystem.ContentVisibleMeta._FailedMakeVisibleTitle, AMEditorSystem.ContentVisibleMeta._FailedMakeVisible, 
						AMEditorSystem.ContentVisibleMeta._ShowMe, AMEditorSystem.ContentVisibleMeta._YouKnowWhatToDo))
					{
						EditorApplication.ExecuteMenuItem ("Edit/Project Settings/Editor");
					}
					return false;
				}
			}
			return true;
		}
		public static void ShowBackups ()
		{
			var path = LocalRepositoryAPI.pathToRepository;
			if (!string.IsNullOrEmpty (path))
			{
				Init ();
				ShowBackupsWindow ();
			}
		}

		public static void Init () 
		{
			cancelCurrentWork = false;

			if (launchMode == LaunchMode.UI)
			{
				#if UNITY_5 && !UNITY_5_5_OR_NEWER
					AMEditorMenuAPI.SetMarks ();
				#endif

				try 
				{
					Hashtable stateConfig = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (FILE_NAME_STATE_EDITOR)) as Hashtable;
					selectedBuildType = (string)stateConfig["selectedBuildType"];
					compilationHandlerName = (string)stateConfig["compilationHandlerName"];
				} 
				catch (Exception) 
				{
					selectedBuildType = "standard";
				}

				if (!isDownloadPlugin && instance == null)
				{
					if (!ShowProgressBarLoop (AMEditorSystem.ContentProgressBar._LaunchingAMEditorTitle, AMEditorSystem.ContentProgressBar._LaunchingAMEditorMessage, 0.1f, 1.1f))
						return;
				}
			}

			var success = AMEditor.WindowMain.SearchConfigFile ();
			authError = !success;

			var window = EditorWindow.GetWindow<AMEditor.WindowMain> (true, "AM Editor");
			#if UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER
				#if AM_EDITOR_COMPACT_ON
					window.minSize = new Vector2 (780, 500); 
				#else
					//window.minSize = new Vector2 (Math.Min (924, Screen.currentResolution.width), 500);
					window.minSize = new Vector2 (980, 500);
				#endif
			#else
				#if AM_EDITOR_COMPACT_ON
					window.minSize = new Vector2 (780, 500); 
				#else
					window.minSize = new Vector2 (Math.Min (924, Screen.currentResolution.width), 500);
				#endif
			#endif
			Rect newWindowPosition = new Rect (0, 0, 0, 0);
			if (AMEditorFileStorage.FileExist (FILE_NAME_STATE_EDITOR))
			{
				Hashtable am_editor_state = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (FILE_NAME_STATE_EDITOR)) as Hashtable;
				var windowPosition = am_editor_state["windowPosition"] as Hashtable;
				if (windowPosition != null)
				{
					newWindowPosition = new Rect (int.Parse (windowPosition["x"].ToString ()), int.Parse (windowPosition["y"].ToString ()), 
						int.Parse (windowPosition["w"].ToString ()), int.Parse (windowPosition["h"].ToString ()));
				}
			}
			if (newWindowPosition.height >= 500)
				window.position = newWindowPosition;
			else
				window.position = new Rect (Screen.currentResolution.width / 2 - window.minSize.x / 2,
					Screen.currentResolution.height / 2 - window.minSize.y / 2,
					window.minSize.x, window.minSize.y);
			window.maxSize = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);
			if (launchMode == LaunchMode.Console)
			{
				window.position = new Rect (Screen.width, Screen.height, 0, 0);
			}
			currentWindowPosition = window.position;
			window.Repaint ();
		}
		
		static void ShowBackupsWindow ()
		{
			var inst = AMEditor.WindowMain.instance;
			
			inst.backups = AMEditorBackupAPI.GetFilesBackup ();
			currentWindow = WindowType.Backup;
		}
		static bool AddCorrectMeta (string path, string platform)
		{
			if (AMEditorFileStorage.FileExist (path))
			{
				var currentMeta = AMEditorFileStorage.ReadTextFile (path);
				var indexIgnore = currentMeta.IndexOf (ignoreMeta);
				if (indexIgnore != -1)
				{
					return true;
				}

				var keyword = "PluginImporter:";
				if (currentMeta.IndexOf ("DefaultImporter:") != -1) 
				{
					currentMeta = currentMeta.Replace ("DefaultImporter:", keyword);
				}
				var index = currentMeta.IndexOf (keyword);
				if (index != -1) 
				{
					string newMeta = currentMeta.Substring (0, index + keyword.Length);
					switch (platform) 
					{
					case "standalone":
						newMeta += rightStandaloneMeta;
						File.WriteAllText (path, newMeta);
						return true;
					default:
						return false;
					}
				}
				else
				{
					AMEditorFileStorage.RemoveFile (path);
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public static int CheckPluginsUpdate (bool silent = false)
		{
			int result = 0;
			float progress = checkFromStartProject ? 0.5f : 0f;
			float step = checkFromStartProject ? 0.05f : 0.1f;
			
			if (launchMode == LaunchMode.UI)
			{
				progress += step;
				if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, progress))
				{
					CancelProgressBar ();
				}

				try 
				{
					selectedBuildType = (string)(AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (FILE_NAME_STATE_EDITOR)) as Hashtable) ["selectedBuildType"];
				} 
				catch (Exception) 
				{
					selectedBuildType = "standard";
				}
			}

			progress += step;
			if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, progress))
			{
				CancelProgressBar ();
			}

			var checkRcResult = CheckRCMode ();
			if (checkRcResult != 0)
				result = checkRcResult;
			else
			{
				bool needUpdate = false;
				bool needPlugins = false;
				string messageNeedPlugins = string.Empty;
				ArrayList newVersionPlugins = new ArrayList ();
			
				try
				{
					progress += step;
					if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, progress))
					{
						CancelProgressBar ();
					}

					string tempStr = AMEditorGit.RequestGet (LINK_PUBLIC_REPO);
					newVersionPlugins = AMEditorJSON.JsonDecode (tempStr) as ArrayList;
					File.WriteAllText (LocalRepositoryAPI.pathToRepository + ACTUAL_VERSION_PLUGINS_FILENAME, tempStr);
				}
				catch (Exception)
				{}
				try
				{
					progress += step;
					if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, progress))
					{
						CancelProgressBar ();
					}

					string tempStr = AMEditorGit.RequestGet (LINK_PUBLIC_REPO_EXTERNAL_PLUGIN);
					var newExternalPlugins = AMEditorJSON.JsonDecode (tempStr) as ArrayList;
					if (newExternalPlugins != null)
					{
						File.WriteAllText (LocalRepositoryAPI.pathToRepository + EXTERNAL_PLUGINS_FILENAME, tempStr);
					}
				}
				catch (Exception)
				{}
			
				actualPlugins = new List<ActualPlugin> ();
				if ((newVersionPlugins == null) || (newVersionPlugins.Count == 0))
				{
					newVersionPlugins = new ArrayList ();
					EditorUtility.ClearProgressBar ();
					AMEditorPopupErrorWindow.ShowErrorPopup ("", "500");
					return 500;
				}
			
				progress += step;
				if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, progress))
				{
					CancelProgressBar ();
				}

				UpdateActualPluginsList (newVersionPlugins);
			
				progress += step;
				if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, progress))
				{
					CancelProgressBar ();
				}

				SearchConfigFile ();
				var tmpInstalledPlugins = new List<Plugin> ();
			
				if (!AMEditorFileStorage.FileExist (AMEDITOR_CONFIG_FILENAME))
				{
					File.WriteAllText (AMEDITOR_CONFIG_FILENAME, "[]");
				}
			
				ArrayList list = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AMEDITOR_CONFIG_FILENAME)) as ArrayList;
				foreach (var item in list)
				{
					tmpInstalledPlugins.Add (new Plugin (item as Hashtable, groupName));
				}

				progress += step;
				if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, progress))
				{
					CancelProgressBar ();
				}

				RemoveIncorrectPlugins ();
			
				string messageUpPlugin = string.Empty;
			
				AMEditorConflictAPI.GetConflict (tmpInstalledPlugins);
				#if !AM_EDITOR_VIEW_TYPE_EXTENDED
				var ccIndex = tmpInstalledPlugins.FindIndex ((p) => {return p.name == "Custom Code";});

				List<Depend> ccDepends = new List <Depend> ();
				if (ccIndex != -1)
				{
					ccDepends = tmpInstalledPlugins [ccIndex].depends;
				}
				#endif

				progress += step;
				if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, progress))
				{
					CancelProgressBar ();
				}

				foreach (var item in tmpInstalledPlugins)
				{
					#if !AM_EDITOR_VIEW_TYPE_EXTENDED
					var depIndex = ccDepends.FindIndex ((p) => {return p.name == item.name && p.version == item.version;});
					#endif
					CheckingMissFilesAndHash (item);
					var actualName = GetActualNamePlugin (item.name);
					if (item.errors.conflict  || 
					  item.errors.missingFiles  || 
					  item.errors.missingFilesHash  || 
					  item.errors.dependPlugins  || 
					  (actualName != item.name))
					{
						needUpdate = true;
						if (launchMode == LaunchMode.Console)
						{
							AddPluginInQueue (item.name, item.version);
						}
						#if !AM_EDITOR_VIEW_TYPE_EXTENDED
					if (depIndex != -1)
					{
						if (!messageUpPlugin.Contains ( "Custom Code"))
							messageUpPlugin += AMEditorSystem.ContentProgressBar._NeedUpdate + "Custom Code" + Environment.NewLine;
					}
					else
					{						
						#endif
						if (!messageUpPlugin.Contains (item.name))
							messageUpPlugin += AMEditorSystem.ContentProgressBar._NeedUpdate + item.name + Environment.NewLine;
						#if !AM_EDITOR_VIEW_TYPE_EXTENDED
					}
						#endif
					}
					else if (item.errors.oldVersion)
					{
						needUpdate = true;
						#if !AM_EDITOR_VIEW_TYPE_EXTENDED
					if (depIndex != -1)
					{
						if (!messageUpPlugin.Contains ( "Custom Code"))
							messageUpPlugin += AMEditorSystem.ContentProgressBar._NotLatestVersion + "Custom Code" + Environment.NewLine;
					}
					else
					{						
						#endif
						if (!messageUpPlugin.Contains (item.name))
							messageUpPlugin += AMEditorSystem.ContentProgressBar._NotLatestVersion + item.name + Environment.NewLine;
						#if !AM_EDITOR_VIEW_TYPE_EXTENDED
					}
						#endif
					}
				}

				progress += step;
				if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, progress))
				{
					CancelProgressBar ();
				}

				foreach (var item in actualPlugins)
				{
					int index = -1;
					if (item.buildTypes != null && item.buildTypes.Count > 0 && !item.buildTypes [0].Equals (selectedBuildType))
						continue;
					else
					{
						index = tmpInstalledPlugins.FindIndex ((p) => {
							return p.name == item.name;
						});
					}
				
					if (index == -1)
					{
						if (item.mandatory)
						{
							var actualName = GetActualNamePlugin (item.name);
							if (actualName == item.name)
							{
								needPlugins = true;
								if (launchMode == LaunchMode.Console)
								{
									AddPluginInQueue (item.name, item.version);
								}
							
								if (!messageNeedPlugins.Contains (item.name))
									messageNeedPlugins += AMEditorSystem.ContentProgressBar._NeedDownload + item.name + Environment.NewLine;
							}
						}
					}
					else
					{
						#if !AM_EDITOR_VIEW_TYPE_EXTENDED
						var depIndex = ccDepends.FindIndex ((p) => {return p.name == tmpInstalledPlugins[index].name && p.version == tmpInstalledPlugins[index].version;});
						#endif
						if (tmpInstalledPlugins [index].buildTypes != null && tmpInstalledPlugins [index].buildTypes.Count > 0 && !tmpInstalledPlugins [index].buildTypes [0].Equals (selectedBuildType))
						{
							needUpdate = true;
							if (launchMode == LaunchMode.Console)
							{
								AddPluginInQueue (item.name, item.version);
							}
							#if !AM_EDITOR_VIEW_TYPE_EXTENDED
							if (depIndex != -1)
							{
								if (!messageUpPlugin.Contains ( "Custom Code"))
									messageUpPlugin += AMEditorSystem.ContentProgressBar._NotForBuildTypeCC + Environment.NewLine;
							}
							else
							{
							#endif
								if (!messageUpPlugin.Contains (tmpInstalledPlugins [index].name))
									messageUpPlugin += tmpInstalledPlugins [index].name + " " + tmpInstalledPlugins [index].version + AMEditorSystem.ContentProgressBar._NotForBuildType + "\"" + selectedBuildType + "\"" + Environment.NewLine;
							#if !AM_EDITOR_VIEW_TYPE_EXTENDED
							}
							#endif
						}
						if (!tmpInstalledPlugins [index].EqualsVersion (item.version))
						{
							needUpdate = true;
							if (launchMode == LaunchMode.Console)
							{
								AddPluginInQueue (item.name, item.version);
							}
							#if !AM_EDITOR_VIEW_TYPE_EXTENDED
							if (depIndex != -1)
							{
								if (!messageUpPlugin.Contains ( "Custom Code"))
									messageUpPlugin += AMEditorSystem.ContentProgressBar._NotLatestVersion + "Custom Code" + Environment.NewLine;
							}
							else
							{						
							#endif
								if (!messageUpPlugin.Contains (item.name))
									messageUpPlugin += AMEditorSystem.ContentProgressBar._NotLatestVersion + item.name + Environment.NewLine;
							#if !AM_EDITOR_VIEW_TYPE_EXTENDED
							}
							#endif
						}
					}
				}
			
				progress += step;
				if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, progress))
				{
					CancelProgressBar ();
				}

				string message = string.Empty;
				if (needUpdate)
				{
					message += messageUpPlugin;
				}
				if (needPlugins)
				{
					message += messageNeedPlugins;
				}
				if (needUpdate || needPlugins)
				{
					needForceCheck = true;
					result = 1;
					if (launchMode == LaunchMode.UI && !silent)
					{
						if (AMEditor.WindowMain.instance != null)
						{
							if (EditorUtility.DisplayDialog (AMEditorSystem.ContentProgressBar._ForceCheck, message, AMEditorSystem.ContentStandardButton._Ok))
							{
								#if !UNITY_2018_2_OR_NEWER || !UNITY_2018_3_OR_NEWER || !UNITY_2019_1_OR_NEWER
									Init ();
								#endif
								instance.UpdateAllPluginsLists ();
							}
						}
						else
						{
							if (EditorUtility.DisplayDialog (AMEditorSystem.ContentProgressBar._ForceCheck, message, AMEditorSystem.ContentStandardButton._More, AMEditorSystem.ContentStandardButton._Ok))
							{
								Init ();
							}
						}
					}
				}
				else
				{
					result = 0;
					if (launchMode == LaunchMode.UI && !silent)
					{
						if (AMEditor.WindowMain.instance != null)
						{
							if (EditorUtility.DisplayDialog (AMEditorSystem.ContentProgressBar._NoUpdate, message, AMEditorSystem.ContentStandardButton._Ok))
								instance.UpdateAllPluginsLists ();
						}
						else
						{
							if (EditorUtility.DisplayDialog (AMEditorSystem.ContentProgressBar._NoUpdate, message, AMEditorSystem.ContentStandardButton._Ok, AMEditorSystem.ContentStandardButton._ShowAMEditor))
							{
							}
							else
							{
								Show ();
							}
						}
					}
				}
			}
			EditorUtility.ClearProgressBar ();
			return result;
		}

		static string RemoveIncorrectPlugins ()
		{
			string result = "";

			if (installedPlugins != null && installedPlugins.Count > 0)
			{
				var incorrectPlugins = (from p in installedPlugins
					where string.IsNullOrEmpty (p.name) || string.IsNullOrEmpty (p.version)
				  select p).ToList ();
				result = incorrectPlugins.Count.ToString ();

				foreach (var item in incorrectPlugins) 
				{
					var index = installedPlugins.FindIndex ((p) => { return p == item; });
					if (index != -1)
						installedPlugins.RemoveAt (index);
				}
			}
			return result;
		}

		void Update ()
		{
			try
			{
				if (EditorApplication.isCompiling)
				{
					if (string.IsNullOrEmpty (tempWindowType))
						tempWindowType = currentWindow.ToString ();
					if (messageWait == AMEditorSystem.ContentOtherWindow._RestoringCRMWaitMessage)
					{
						if (EditorApplication.timeSinceStartup > restoringCRMStatusTime && restoringCRMStatusTime == 0) 
						{
							restoringCRMStatusTime = EditorApplication.timeSinceStartup + 2;
						}
						else 
						{
							if (EditorApplication.timeSinceStartup <= restoringCRMStatusTime)
							{
								messageWait = AMEditorSystem.ContentOtherWindow._RestoringCRMWaitMessage;
							}
							else
							{
								messageWait = AMEditorSystem.ContentOtherWindow._CompileWaitMessage;
								restoringCRMStatusTime = 0;
							}
						}
					}
					else
					{
						messageWait = AMEditorSystem.ContentOtherWindow._CompileWaitMessage;
						restoringCRMStatusTime = 0;
					}
					currentWindow = WindowType.Wait;
					allInstalledSelected = false;
					allMandatorySelected = false;
					allOtherSelected = false;
				}
				else
				{
					if (!string.IsNullOrEmpty (tempWindowType))
					{
						currentWindow = (WindowType)Enum.Parse (typeof (WindowType), tempWindowType);
						if (currentWindow == WindowType.Backup)
							backups = AMEditorBackupAPI.GetFilesBackup ();
						if (currentWindow == WindowType.ListPlugins)
							UpdateAllPluginsLists ();
						tempWindowType = string.Empty;
					}
				}
			}
			catch (Exception) 
			{
				currentWindow = WindowType.ListPlugins;
			}

			if (currentWindow == WindowType.Wait)
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
			{
				progressNextRepaintTime = 0;
				Repaint ();

				//#region DLC Update
				//if (AMEditor.DLC.repaintRequired)
				//{
				//	AMEditor.DLC.RepaintContent ();
				//}
				//#endregion
			}
		}
		
		public static void ChangeCurrentWindowToListPlugins ()
		{
			currentWindow = WindowType.ListPlugins;
			if (firstStart) 
			{
				firstStart = false;
				SearchConfigFile ();
				if (CheckPublicRepo ())
				{
					checkFromStartProject = true;
					CheckUpdate ();
					CustomResourceManagerAPI.CheckCorrect ();
				}
				HelpAPI.ForceDownload ();
			}
		}
		
		static int CheckRCMode ()
		{
			if (AMEditorFileStorage.FileExist (FILE_NAME_RC_MODE)) 
			{
				AMEditorFileStorage.RemoveFile (FILE_NAME_RC_MODE);
				AMEditorMenuAPI.ChangePluginsType (AMEditorMenuAPI.PluginsType.RC);
			}
			
			branchName = AMEditorSystem.Git._BranchName;
			groupName = AMEditorSystem.Git._GroupName;
			
			LINK_PUBLIC_REPO = AMEditorSystem.Links.PublicAMEditorConfig;
			
			LOCAL_REPOSITORY = AMEditorSystem.FolderNames._LocalRepository;
			
			AMEDITOR_CONFIG_FILENAME = AMEditorSystem.FileNames._ConfigAMEditor;
			NEED_DISPLAY_SUCCESS_DIALOG = AMEditorSystem.FileNames._NeedDisplaySucces;
			PLUGIN_CONFIG_FILENAME = AMEditorSystem.FileNames._ConfigPlugin;
			ACTUAL_VERSION_PLUGINS_FILENAME = AMEditorSystem.FileNames._ActualPlugins;

			EXTERNAL_PLUGINS_FILENAME = AMEditorSystem.FileNames._ExternalPlugins;
			LINK_PUBLIC_REPO_EXTERNAL_PLUGIN = AMEditorSystem.Links.PublicAMEditorExternalPlugins;

			AMEDITOR_WHITELIST_FILENAME = AMEditorSystem.FileNames._WhiteList;
			LINK_PUBLIC_REPO_WHITELIST = AMEditorSystem.Links.PublicAMEditorWhitelist;

			LINK_PUBLIC_REPO = GetLinkPublicRepo ();
			LINK_PUBLIC_REPO_EXTERNAL_PLUGIN = GetLinkPublicRepoExternalPlugin ();
			LINK_PUBLIC_REPO_WHITELIST = GetLinkPublicRepoWhitelist ();
			if (LINK_PUBLIC_REPO == "" || LINK_PUBLIC_REPO_EXTERNAL_PLUGIN == "")
			{
				if (authError)
					AMEditorPopupErrorWindow.ShowErrorPopup ("101", AMEditorSystem.WebError._101);
				return 101;
			}
			return 0;
		}
		
		public static string GetLinkPublicRepo ()
		{
			var currentToken = GitAccount.current == null ? "" : GitAccount.current.privateToken;
			if (currentToken == "") 
			{
				firstStart = true;
				return "";
			}
			
			return LINK_PUBLIC_REPO + "?private_token=" + currentToken;
		}
		
		public static string GetLinkPublicRepoExternalPlugin ()
		{
			var currentToken = GitAccount.current == null ? "":GitAccount.current.privateToken;
			
			if (currentToken == "") 
			{
				return "";
			}
			
			return LINK_PUBLIC_REPO_EXTERNAL_PLUGIN + "?private_token=" + currentToken;
		}

		public static string GetLinkPublicRepoWhitelist ()
		{
			var currentToken = GitAccount.current == null ? "":GitAccount.current.privateToken;
			if (currentToken == "") 
			{
				return "";
			}

			return LINK_PUBLIC_REPO_WHITELIST + "?private_token=" + currentToken;
		}

		GUIStyle pluginsHeaderBackStyle = null;
		GUIStyle pluginSelectorBackStyle = null;
		GUIStyle versionSelectorStyle = null;
		Color uiTextColor;
		Color proBackColor = new Color32 (56, 56, 56, 255);
		Color freeBackColor = new Color32 (194, 194, 194, 255);
		Color pluginVersionOkColor = new Color32 (17, 170, 9, 255);
		void OnGUI ()
		{
			pluginsHeaderBackStyle = EditorGUIUtility.isProSkin ? new GUIStyle (GUI.skin.box) : new GUIStyle (GUI.skin.textField);
			if (pluginSelectorBackStyle == null)
			{
				pluginSelectorBackStyle = new GUIStyle (GUI.skin.box);
				pluginSelectorBackStyle.normal.background = pluginSelectorBackground as Texture2D;
			}
			uiTextColor = new GUIStyle (GUI.skin.label).normal.textColor;
			if (versionSelectorStyle == null)
			{
				versionSelectorStyle = new GUIStyle (EditorStyles.foldout);
				versionSelectorStyle.focused.background = versionSelectorStyle.normal.background;
				versionSelectorStyle.normal.textColor = EditorGUIUtility.isProSkin ? proBackColor : freeBackColor;
				versionSelectorStyle.focused.textColor = EditorGUIUtility.isProSkin ? proBackColor : freeBackColor;
			}
			#if AM_EDITOR_COMPACT_ON
				GUIContent backButtonContent = new GUIContent (backButtonTexture, AMEditorSystem._ContentBackToPluginsHelp);
				GUIContent closeButtonContent = new GUIContent (closeButtonTexture, AMEditorSystem._ContentCloseHelp);
				int width = buttonWidth;
			#else
				GUIContent backButtonContent = new GUIContent (AMEditorSystem._ContentBackToPlugins, AMEditorSystem._ContentBackToPluginsHelp);
				GUIContent closeButtonContent = new GUIContent (AMEditorSystem._ContentClose, AMEditorSystem._ContentCloseHelp);
				int width = Screen.width / 2 - 4;
			#endif
			switch (currentWindow) 
			{
			case WindowType.ListPlugins:
				this.titleContent = new GUIContent (AMEditorSystem._ContentTitleWindowListPlugins);
				try
				{
					DrawWindowListPlugins ();
				}
				catch (Exception)
				{}
				break;
				
			case WindowType.FixConfict:
				this.titleContent = new GUIContent (AMEditorSystem._ContentTitleWindowFixConflict);

				if (GUILayout.Button (backButtonContent, GUILayout.Width (wideButtonWidth), GUILayout.Height (buttonHeight)))
				{
					currentWindow = WindowType.ListPlugins;
					UpdateAllPluginsLists ();
				}
				
				scroll = GUILayout.BeginScrollView (scroll);
				ViewFixConflict ();
				GUILayout.EndScrollView ();	
				
				break;
				
			case WindowType.Backup:
				this.titleContent = new GUIContent (AMEditorSystem.ContentBackup._TitleWindowBackup);
				
				GUILayout.BeginHorizontal ();

				if (GUILayout.Button (backButtonContent, GUILayout.Width (width), GUILayout.Height (buttonHeight)))
				{
					currentWindow = WindowType.ListPlugins;
				}
			#if AM_EDITOR_COMPACT_ON
				GUILayout.Label ("", GUILayout.ExpandWidth (true));
			#endif
				if (GUILayout.Button (closeButtonContent, GUILayout.Width (width), GUILayout.Height (buttonHeight)))
				{
					this.Close ();
				}
				GUILayout.EndHorizontal ();

				ViewBackup ();

				break;
				
			case WindowType.Authorization:
				this.titleContent = new GUIContent (AMEditorSystem.ContentAuth._Title);
				AuthWindow.DrawGUI ();
				
				break;
				
			case WindowType.Wait:
				GUIStyle labelStyle = new GUIStyle (GUI.skin.label);
				labelStyle.alignment = TextAnchor.MiddleCenter;

				this.titleContent = new GUIContent (AMEditorSystem.ContentOtherWindow._TitleWait);
				GUI.Label (new Rect (4, Screen.height / 2 - 36, Screen.width - 4, 20), messageWait, labelStyle);
				if (progressCircle.Count > 0)
				{
					GUI.Label (new Rect (4, Screen.height / 2 - 16, Screen.width - 4, 32), progressCircle [progressFrameIndex], labelStyle);
				}
				if (messageWait.Contains ("Загружается") || messageWait.Contains ("Downloading"))
				{
					GUI.Label (new Rect (4, Screen.height / 2 + 16, Screen.width - 4, 20), AMEditorSystem.ContentStatuses._DontCloseAMEditor, labelStyle);
				}

				#region Wait Window Status Bar
				GUIStyle statusBarStyle = new GUIStyle (GUI.skin.box);
				statusBarStyle.normal.textColor = uiTextColor;
				statusBarStyle.alignment = TextAnchor.MiddleLeft;

				string statusBarMessage = AMEditorSystem.ContentProgressBar._IsCompiling; 
				string statusBarProgress = string.Empty;

				if (CurrentPluginData != string.Empty) 
				{
					statusBarMessage = AMEditorSystem.ContentProgressBar._Plugin + CurrentPluginData + ". " + CurrentStatusPlugin;
					try
					{
						statusBarProgress = " " + ((Int32)(CurrentProgressDownloads * 100)).ToString () + "%";
					}
					catch (Exception)
					{}
				}
				if (EditorApplication.isCompiling)
				{
					statusBarMessage = AMEditorSystem.ContentProgressBar._IsCompiling;
					statusBarProgress = string.Empty;
				}
				if (messageWait == AMEditorSystem.ContentOtherWindow._RestoringCRMWaitMessage)
				{
					statusBarMessage = AMEditorSystem.ContentStatuses._RestoringCRM;
					statusBarProgress = string.Empty;
				}

				GUI.Box (new Rect (-1, Screen.height - 20, Screen.width + 2, 21), statusBarMessage + "...", statusBarStyle);
				if (CurrentPluginData != string.Empty && !EditorApplication.isCompiling) 
				{
					EditorGUI.ProgressBar (new Rect (Screen.width - 290, Screen.height - 17, 200, 15), CurrentProgressDownloads, statusBarProgress);
					if (GUI.Button (new Rect (Screen.width - 80, Screen.height - 20, 80, 20), AMEditorSystem.ContentStandardButton._Cancel))
					{
						try
						{
							CancelProgressBar ();
						}
						catch (Exception)
						{}
					}
				}
				#endregion
				break;
				
			case WindowType.CriticalError:
				this.titleContent = new GUIContent (AMEditorSystem.ContentOtherWindow._TitleCriticalError);
				GUILayout.Label ("WAT??? " + Environment.NewLine + "Если вы видете это сообщение, значит что-то пошло не так. Срочно переставить плагин.");
				break;
				
			default:
				this.titleContent = new GUIContent (AMEditorSystem.ContentOtherWindow._TitleCriticalError);
				GUILayout.Label ("WAT??? " + Environment.NewLine + "Если вы видете это сообщение, значит что-то пошло не так. Срочно переставить плагин.");
				break;
			}
			
			if (needForceCheck)
			{
				needForceCheck = false;
				CheckingUpdateAllPluginForce ();
			}
		}
		
		public static bool SearchConfigFile ()
		{
			var checkRcResult = CheckRCMode ();
			if (checkRcResult != 0)
				return false;
			
			List<string> foundedConfigs = new List<string> (); 
			
			foreach (var item in listLocationConfig) 
			{
				var tmpFoundedConfigs = AMEditorFileStorage.SearchFile (item, PLUGIN_CONFIG_FILENAME);
				foreach (var config in tmpFoundedConfigs) 
				{
					foundedConfigs.Add (config);
				}
			}
			
			if ((foundedConfigs != null) && (foundedConfigs.Count > 0))
			{
				installedPlugins = new List<Plugin> ();
				
				if (!AMEditorFileStorage.FileExist (AMEDITOR_CONFIG_FILENAME)) 
				{
					File.WriteAllText (AMEDITOR_CONFIG_FILENAME, "[]");
				}
				
				ArrayList currentPluginsFromConfig = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AMEDITOR_CONFIG_FILENAME)) as ArrayList;
				
				foreach (var item in currentPluginsFromConfig) 
				{
					installedPlugins.Add (new Plugin (item as Hashtable, groupName));
				}
				
				foreach (var config in foundedConfigs) 
				{
					var item = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (config)) as Hashtable;
					AMEditorFileStorage.RemoveFile (config);
					
					Plugin newPlugin = new Plugin (item, groupName);

					int indexpl = installedPlugins.FindIndex ((plugin) => {return (plugin.name == newPlugin.name);});
					if (indexpl != -1)
					{
						installedPlugins[indexpl].Update (newPlugin);
					}
					//else
					//{
					//	if (newPlugin.name != "AM Editor DLC")
					//		installedPlugins.Add (newPlugin);
					//}
				}

				RemoveIncorrectPlugins ();
				
				ArrayList newPluginsForConfigList = new ArrayList ();
				foreach (var item in installedPlugins) 
				{
					newPluginsForConfigList.Add (item.ToHashtable ());
				}
				string newPluginsForConfigString = AMEditorJSON.JsonEncode (newPluginsForConfigList);
				File.WriteAllText (AMEDITOR_CONFIG_FILENAME, AMEditorJSON.FormatJson (newPluginsForConfigString));
			}
			return true;
		}

		static bool ShowProgressBarLoop (string title, string message, float progressStep, float progressLimit = 1f, float progressStartValue = 0f)
		{
			for (float progress = progressStartValue; progress <= progressLimit; progress += progressStep)
			{
				if (EditorUtility.DisplayCancelableProgressBar (title, message, progress))
				{
					CancelProgressBar ();
					return false;
				}
			}
			return true;
		}

		public static void CancelProgressBar ()
		{
			cancelPressed = true;

			try 
			{
				EditorUtility.ClearProgressBar ();
			}
			catch (Exception) 
			{}

			if (isDownloadPlugin && (currentDownloadGit != null))
			{
				currentDownloadGit.StopDownload ();
			}

			DownloadQueue = new List<string> ();
			CheckList = new List<string> ();
			localRepositoryPluginsPathes = new List<string> ();
			isDownloadPlugin = false;
			isInit = false;
			if (instance != null)
			{
				if (currentWindow == WindowType.Wait)
				{
					currentWindow = WindowType.ListPlugins;
				}
				instance.UpdateGUI ();
			}
			else
			{
				if (currentWindow == WindowType.Wait)
				{
					currentWindow = WindowType.ListPlugins;
				}
			}
		}
		
		public static bool CheckPublicRepo ()
		{
			float progress = 0f;
			float step = 0.1f;

			progress += step;
			if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingPublicRepo, progress))
			{
				CancelProgressBar ();
			}
			
			actualPlugins = new List<ActualPlugin> ();
			ArrayList newVersionPlugins = new ArrayList ();
			try 
			{
				var tempStr = AMEditorGit.RequestGet (LINK_PUBLIC_REPO);
				if (tempStr == string.Empty || tempStr == "{}" )
				{
					EditorUtility.ClearProgressBar ();
					AMEditorPopupErrorWindow.ShowErrorPopup ("", "500");
					return false;
				}
				File.WriteAllText (LocalRepositoryAPI.pathToRepository + ACTUAL_VERSION_PLUGINS_FILENAME, tempStr);
				newVersionPlugins = AMEditorJSON.JsonDecode (tempStr) as ArrayList;
			} 
			catch (Exception) 
			{
				EditorUtility.ClearProgressBar ();
				if (!firstStart)
					AMEditorPopupErrorWindow.ShowErrorPopup ("311", AMEditorSystem.ContentError._311);
				return false;
			}
			
			if ((newVersionPlugins == null) || (newVersionPlugins.Count == 0))
			{
				newVersionPlugins = new ArrayList ();
				EditorUtility.ClearProgressBar ();
				if (!firstStart)
					AMEditorPopupErrorWindow.ShowErrorPopup ("311", AMEditorSystem.ContentError._311);
				return false;
			}

			progress += step;
			if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingPublicRepo, progress))
			{
				CancelProgressBar ();
			}
			
			UpdateActualPluginsList (newVersionPlugins);

			progress += step;
			if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingPublicRepo, progress))
			{
				CancelProgressBar ();
			}
			
			externalPlugins = new List<ExternalPlugin> ();
			ArrayList newExternalPlugins = new ArrayList ();
			try 
			{
				var tempStr = AMEditorGit.RequestGet (LINK_PUBLIC_REPO_EXTERNAL_PLUGIN);
				if (tempStr == string.Empty)
				{
					EditorUtility.ClearProgressBar ();
					AMEditorPopupErrorWindow.ShowErrorPopup ("", "500");
					return false;
				}
				File.WriteAllText (LocalRepositoryAPI.pathToRepository + EXTERNAL_PLUGINS_FILENAME, tempStr);
				newVersionPlugins = AMEditorJSON.JsonDecode (tempStr) as ArrayList;
			} 
			catch (Exception) 
			{
				EditorUtility.ClearProgressBar ();
				if (!firstStart)
					AMEditorPopupErrorWindow.ShowErrorPopup ("311", AMEditorSystem.ContentError._311);
				return false;
			}
			if (newExternalPlugins == null)
			{
				newVersionPlugins = new ArrayList ();
				EditorUtility.ClearProgressBar ();
			}
			foreach (var item in newExternalPlugins) 
			{
				externalPlugins.Add (new ExternalPlugin (item as Hashtable));
			}

			installedPlugins = new List<Plugin> ();
			
			if (!AMEditorFileStorage.FileExist (AMEDITOR_CONFIG_FILENAME)) 
			{
				File.WriteAllText (AMEDITOR_CONFIG_FILENAME, "[]");
			}
			
			ArrayList list = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AMEDITOR_CONFIG_FILENAME)) as ArrayList;
			
			foreach (var item in list) 
			{
				installedPlugins.Add (new Plugin (item as Hashtable, groupName));
			}

			progress += step;
			if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingPublicRepo, progress))
			{
				CancelProgressBar ();
			}

			pluginsWhitelist = new List<string> ();
			ArrayList whiteList = new ArrayList ();
			try 
			{
				var tempStr = AMEditorGit.RequestGet (LINK_PUBLIC_REPO_WHITELIST);
				if ((tempStr == string.Empty) || tempStr == "{}" )
				{
					EditorUtility.ClearProgressBar ();
					AMEditorPopupErrorWindow.ShowErrorPopup ("", "500");
					return false;
				}
				File.WriteAllText (LocalRepositoryAPI.pathToRepository + AMEDITOR_WHITELIST_FILENAME, tempStr);
				whiteList = AMEditorJSON.JsonDecode (tempStr) as ArrayList;
			} 
			catch (Exception) 
			{
				EditorUtility.ClearProgressBar ();
				if (!firstStart)
					AMEditorPopupErrorWindow.ShowErrorPopup ("311", AMEditorSystem.ContentError._311);
				return false;
			}
			foreach (var item in whiteList)
			{
				pluginsWhitelist.Add (item.ToString ());
			}

			progress += step;
			if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._CheckingUpdatePlugins, AMEditorSystem.ContentProgressBar._CheckingPublicRepo, progress))
			{
				CancelProgressBar ();
			}

			RemoveIncorrectPlugins ();

			foreach (var item in actualPlugins) 
			{
				var index = installedPlugins.FindIndex ((p) => {return p.name == item.name;});
				if (index == -1)
				{
					item.missing = true;
				}
				else
					if (!installedPlugins[index].EqualsVersion (item.version))
				{
					item.oldVersion = true;
				}
			}
			
			EditorUtility.ClearProgressBar ();
			return true;
		}

		public void CheckingUpdateAllPluginForce ()
		{
			foreach (var item in installedPlugins) 
			{
				var index = actualPlugins.FindIndex (ap=> {return (ap.name == item.name) && (!item.EqualsVersion (ap.version));});
				if (index != -1)
				{
					item.errors.oldVersion = true;
				}
				CheckingMissFilesAndHash (item);
			}
		}
		
		void SavePlugins ()
		{
			RemoveIncorrectPlugins ();

			ArrayList plugins = new ArrayList ();
			foreach (var item in installedPlugins) 
			{
				plugins.Add (item.ToHashtable ());
			}
			string list = AMEditorJSON.JsonEncode (plugins);
			File.WriteAllText (AMEDITOR_CONFIG_FILENAME, AMEditorJSON.FormatJson (list));
			currentWindow = WindowType.ListPlugins;
		}

		void ViewFixConflict ()
		{
			int countConflict = 0;
			if (installedPlugins != null) 
			{
				foreach (var plugin in installedPlugins)
				{
					if ((plugin.conflictFiles.Count > 0) || (plugin.conflictFilesOldFiles.Count > 0))
					{
						GUIStyle guistyle = new GUIStyle ();
						#if AM_EDITOR_COMPACT_ON
							bool selectAll = plugin.conflictFiles.TrueForAll ((f) => {return f.delete;}) && plugin.conflictFilesOldFiles.TrueForAll ((f) => {return f.delete;});

							GUIContent openFileButtonContent = new GUIContent (openButtonTexture, AMEditorSystem._ContentOpenFileHelp);
							GUIContent deleteButtonContent = new GUIContent (deleteButtonTexture, AMEditorSystem._ContentDeleteFiles);
						#else
							GUIContent openFileButtonContent = new GUIContent (AMEditorSystem._ContentOpenFile, "");
							GUIContent deleteButtonContent = new GUIContent (AMEditorSystem._ContentDeleteFiles, "");
						#endif						
						guistyle.normal.textColor = Color.magenta;

						countConflict++;
						#if AM_EDITOR_COMPACT_ON
							GUIStyle toggleAllStyle = new GUIStyle (GUI.skin.toggle);
							toggleAllStyle.normal.background = selectAll ? new GUIStyle (GUI.skin.toggle).onNormal.background : new GUIStyle (GUI.skin.toggle).normal.background;
							string toggleAllHelp = selectAll ? AMEditorSystem.ContentStandardButton._NoneHelp : AMEditorSystem.ContentStandardButton._AllHelp;

							if (GUILayout.Button (new GUIContent (plugin.name, toggleAllHelp), toggleAllStyle, GUILayout.Width (300)))
							{
								selectAll = !selectAll;
								if (plugin.conflictFiles.Count > 0)
								{
									foreach (var file in plugin.conflictFiles) 
									{
										file.delete = selectAll;
									}
								}
								if (plugin.conflictFilesOldFiles.Count > 0)
								{
									foreach (var file in plugin.conflictFilesOldFiles) 
									{
										file.delete = selectAll;
									}
								}
							}
						#else
							GUILayout.Label (plugin.name, GUILayout.Width (300));
						#endif
						foreach (var file in plugin.conflictFiles) 
						{
							GUILayout.BeginHorizontal ();
							
							file.delete = GUILayout.Toggle (file.delete, file.name);

							GUILayout.Label (file.cause, guistyle, GUILayout.Width (300));

							if (GUILayout.Button (openFileButtonContent, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
							{
								EditorUtility.OpenWithDefaultApp (file.name);
							}
							
							GUILayout.EndHorizontal ();
						}
						foreach (var file in plugin.conflictFilesOldFiles) 
						{	
							GUILayout.BeginHorizontal ();
							file.delete = GUILayout.Toggle (file.delete, file.name);

							GUILayout.Label (file.cause, guistyle, GUILayout.Width (300));

							if (GUILayout.Button (openFileButtonContent, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
							{
								EditorUtility.OpenWithDefaultApp (file.name);
							}
							
							GUILayout.EndHorizontal ();
						}
						
						EditorGUILayout.Space ();
						#if !AM_EDITOR_COMPACT_ON
							GUILayout.BeginHorizontal ();
							if (GUILayout.Button (AMEditorSystem.ContentStandardButton._All, GUILayout.Width (70)))
							{
								if (plugin.conflictFiles.Count > 0)
								{
									foreach (var file in plugin.conflictFiles) 
									{
										file.delete = true;
									}
								}
								if (plugin.conflictFilesOldFiles.Count > 0)
								{
									foreach (var file in plugin.conflictFiles) 
									{
										file.delete = true;
									}
								}
							}
							
							if (GUILayout.Button (AMEditorSystem.ContentStandardButton._None, GUILayout.Width (70)))
							{
								if (plugin.conflictFiles.Count > 0)
								{
									foreach (var file in plugin.conflictFiles) 
									{
										file.delete = false;
									}
								}
								if (plugin.conflictFilesOldFiles.Count > 0)
								{
									foreach (var file in plugin.conflictFiles) 
									{
										file.delete = false;
									}
								}
							}
							GUILayout.EndHorizontal ();
						#endif
						EditorGUILayout.Space ();

						bool isEnableDeleteButton = plugin.conflictFiles.FindIndex ((file) => {return file.delete;}) != -1 || 
													plugin.conflictFilesOldFiles.FindIndex ((file) => {return file.delete;}) != -1;
						GUI.enabled = isEnableDeleteButton;
						if (GUILayout.Button (deleteButtonContent, GUILayout.Width (wideButtonWidth), GUILayout.Height (buttonHeight)))
						{
							if (EditorUtility.DisplayDialog (AMEditorSystem._ContentTitleDeletingFiles, 
								AMEditorSystem.ContentWindowConflict._ContentQuestionDeleteSelectedFilesFor (plugin.name), 
								AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._Cancel))
							{
								foreach (var file in plugin.conflictFiles) 
								{
									if (file.delete)
									{
										AMEditorBackupAPI.BackupingFile (file.name);
										var listFolder = file.name.Split (new char[]{System.IO.Path.DirectorySeparatorChar});
										var path = file.name;
										for (int i = 0; i < listFolder.Length - 1; i++) 
										{
											path = path.Substring (0, path.LastIndexOf (System.IO.Path.DirectorySeparatorChar.ToString ()));
											RemoveEmptyFolder (path);
										}
									}
								}
								foreach (var file in plugin.conflictFilesOldFiles) 
								{
									if (file.delete)
									{
										AMEditorBackupAPI.BackupingFile (file.name);
										
										var listFolder = file.name.Split (new char[]{System.IO.Path.DirectorySeparatorChar});
										var path = file.name;
										for (int i = 0; i < listFolder.Length - 1; i++) 
										{
											path = path.Substring (0, path.LastIndexOf (System.IO.Path.DirectorySeparatorChar.ToString ()));
											RemoveEmptyFolder (path);
										}
									}
								}
								
								AMEditorBackupAPI.pathBackup = string.Empty;
								#if AM_EDITOR_VIEW_TYPE_EXTENDED
								AMEditorConflictAPI.GetConflict (currentPlugin);
								#endif
								AssetDatabase.Refresh ();
								installedPlugins = new List<Plugin> ();
								isInit = false;
							}
						}
						GUI.enabled = true;
						GUILayout.Box ("", new GUILayoutOption[]{GUILayout.ExpandWidth (true), GUILayout.Height (1)});
						EditorGUILayout.Space ();
					}
				}
			}
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			if (countConflict > 1)
			{
				GUILayout.BeginHorizontal ();
				
				if (GUILayout.Button (AMEditorSystem.ContentStandardButton._All, GUILayout.Width (70)))
				{
					foreach (var plugin in installedPlugins)
					{
						
						if (plugin.conflictFiles.Count > 0)
						{
							foreach (var file in plugin.conflictFiles) 
							{
								file.delete = true;
							}
						}
						if (plugin.conflictFilesOldFiles.Count > 0)
						{
							foreach (var file in plugin.conflictFiles) 
							{
								file.delete = true;
							}
						}
					}
				}
				
				if (GUILayout.Button (AMEditorSystem.ContentStandardButton._None, GUILayout.Width (70)))
				{
					foreach (var plugin in installedPlugins)
					{						
						if (plugin.conflictFiles.Count > 0)
						{
							foreach (var file in plugin.conflictFiles) 
							{
								file.delete = false;
							}
						}
						if (plugin.conflictFilesOldFiles.Count > 0)
						{
							foreach (var file in plugin.conflictFiles) 
							{
								file.delete = false;
							}
						}
					}
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (AMEditorSystem._ContentDeleteAllFiles))
				{
					if (EditorUtility.DisplayDialog (AMEditorSystem._ContentTitleDeletingFiles, 
						AMEditorSystem.ContentWindowConflict._ContentQuestionDeleteAllSelectedFiles, 
						AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._Cancel))
					{
						foreach (var plugin in installedPlugins)
						{
							foreach (var file in plugin.conflictFiles) 
							{
								if (file.delete)
								{
									AMEditorBackupAPI.BackupingFile (file.name);
									var listFolder = file.name.Split (new char[]{System.IO.Path.DirectorySeparatorChar});
									var path = file.name;
									for (int i = 0; i < listFolder.Length - 1; i++) 
									{
										path = path.Substring (0, path.LastIndexOf (System.IO.Path.DirectorySeparatorChar.ToString ()));
										RemoveEmptyFolder (path);
									}
								}
							}
							foreach (var file in plugin.conflictFilesOldFiles) 
							{
								if (file.delete)
								{
									AMEditorBackupAPI.BackupingFile (file.name);
									
									var listFolder = file.name.Split (new char[]{System.IO.Path.DirectorySeparatorChar});
									var path = file.name;
									for (int i = 0; i < listFolder.Length - 1; i++) 
									{
										path = path.Substring (0, path.LastIndexOf (System.IO.Path.DirectorySeparatorChar.ToString ()));
										RemoveEmptyFolder (path);
									}
								}
							}
						}
						AMEditorBackupAPI.pathBackup = string.Empty;
						#if AM_EDITOR_VIEW_TYPE_EXTENDED
						AMEditorConflictAPI.GetConflict (currentPlugin);
						#endif
						AssetDatabase.Refresh ();
						installedPlugins = null;
						isInit = false;
					}
				}
				GUILayout.EndHorizontal ();
			}
			EditorGUILayout.Space ();
		}
		
		void ViewBackup ()
		{
		#if AM_EDITOR_COMPACT_ON
			GUIContent expandOnContent = new GUIContent (expandOnButtonTexture, AMEditorSystem._ContentTurn);
			GUIContent expandOffContent = new GUIContent (expandOffButtonTexture, AMEditorSystem._ContentExpand);
			GUIContent deleteButtonContent = new GUIContent (deleteButtonTexture, AMEditorSystem._ContentDeleteBackup);
		#else
			GUIContent expandOnContent = new GUIContent (AMEditorSystem._ContentTurn, AMEditorSystem._ContentTurn);
			GUIContent expandOffContent = new GUIContent (AMEditorSystem._ContentExpand, AMEditorSystem._ContentExpand);
			GUIContent deleteButtonContent = new GUIContent (AMEditorSystem.ContentStandardButton._Delete, AMEditorSystem._ContentDeleteBackup);
		#endif
			scroll = GUILayout.BeginScrollView (scroll);
			foreach (var b in backups) 
			{
				GUILayout.BeginHorizontal ();
				
				GUILayout.Label (AMEditorSystem._ContentDateTimeBackup + b.dateTime);
				if (GUILayout.Button (b.extended ? expandOnContent : expandOffContent, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
				{
					b.extended = !b.extended;
				}
				if (GUILayout.Button (deleteButtonContent, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
				{
					if (EditorUtility.DisplayDialog (AMEditorSystem._ContentTitleDeletingBackup, AMEditorSystem._ContentQuestionDeleteBackup, AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._Cancel))
					{
						AMEditorBackupAPI.DeleteBackup (b.name);
						backups = AMEditorBackupAPI.GetFilesBackup ();	
					}
				}	
				
				GUILayout.EndHorizontal ();
				if (b.extended)
				{
					foreach (var p in b.files) 
					{
						GUILayout.BeginHorizontal ();
						GUILayout.Space (20);
						
						p.delete = GUILayout.Toggle (p.delete, AMEditorBackupAPI.ViewModeBackupName (p.name));
						#if AM_EDITOR_COMPACT_ON
							GUIContent openFileButtonContent = new GUIContent (openButtonTexture, AMEditorSystem._ContentOpenFileHelp);
						#else
							GUIContent openFileButtonContent = new GUIContent (AMEditorSystem._ContentOpenFile, "");
						#endif
						if (GUILayout.Button (openFileButtonContent, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
						{
							EditorUtility.OpenWithDefaultApp (p.name);
						}
						
						GUILayout.EndHorizontal ();
					}
					
					GUILayout.BeginHorizontal ();
					GUILayout.Space (20);
					if (GUILayout.Button (AMEditorSystem.ContentStandardButton._All, GUILayout.Width (73)))
					{
						foreach (var f in b.files) 
						{
							f.delete = true;
						}
					}
					if (GUILayout.Button (AMEditorSystem.ContentStandardButton._None, GUILayout.Width (73)))
					{
						foreach (var f in b.files) 
						{
							f.delete = false;
						}
					}
					
					GUILayout.EndHorizontal ();
					GUILayout.BeginHorizontal ();
					GUILayout.Space (20);
					GUI.enabled = (b.files.FindIndex ((p) => { return p.delete == true; }) != -1);
					if (GUILayout.Button (AMEditorSystem._ContentRestoreFiles, GUILayout.Width (150)))
					{
						foreach (var p in b.files) 
						{
							if (p.delete)
								AMEditorBackupAPI.RestoreFile (p.name);
						}
						if (EditorUtility.DisplayDialog (AMEditorSystem._ContentTitleRestoredBackups, AMEditorSystem._ContentAlertRestoredBackups, AMEditorSystem.ContentStandardButton._Ok)){};
					}
					GUI.enabled = true;
					GUILayout.EndHorizontal ();
					EditorGUILayout.Space ();
				}
			}
			GUILayout.EndScrollView ();

			#if AM_EDITOR_COMPACT_ON
				if (GUILayout.Button (new GUIContent (AMEditorSystem._ContentDeleteBackupsCompact, deleteButtonTexture), GUILayout.Height (buttonHeight)))
				{
			#else
				if (GUILayout.Button (new GUIContent (AMEditorSystem._ContentDeleteBackups)))
				{
			#endif
				if (EditorUtility.DisplayDialog (AMEditorSystem._ContentTitleDeletingBackups, AMEditorSystem._ContentQuestionDeleteBackups, 
				                      AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._Cancel))
				{
					AMEditorBackupAPI.DeleteAllBackups ();
					backups = AMEditorBackupAPI.GetFilesBackup ();
				}
			}
			GUILayout.Label ("", GUILayout.Height (20));

			#region Backups Status Bar
			GUIStyle statusBarStyle = new GUIStyle (GUI.skin.box);
			statusBarStyle.alignment = TextAnchor.MiddleLeft;
			statusBarStyle.normal.textColor = uiTextColor;

			try 
			{
				string statusMessage = backups != null ? AMEditorSystem._ContentBackupsCount + backups.Count : AMEditorSystem._ContentBackupsCount + 0.ToString ();

				GUI.Box (new Rect (-1, Screen.height - 20, Screen.width + 2, 21), statusMessage, statusBarStyle);
			}
			catch (Exception) 
			{}
			#endregion
		}
		
		public bool UpdateAllPluginsLists ()
		{
			if (installedPlugins == null || installedPlugins.Count == 0)
			{
				installedPlugins = new List<Plugin> ();
				
				if (!AMEditorFileStorage.FileExist (AMEDITOR_CONFIG_FILENAME)) 
				{
					File.WriteAllText (AMEDITOR_CONFIG_FILENAME, "[]");
				}
				
				ArrayList list = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AMEDITOR_CONFIG_FILENAME)) as ArrayList;
				foreach (var item in list) 
				{
					installedPlugins.Add (new Plugin (item as Hashtable, groupName));
				}
			}
			RemoveIncorrectPlugins ();

			actualPlugins = new List<ActualPlugin> ();
			
			if (!AMEditorFileStorage.FileExist (LocalRepositoryAPI.pathToRepository + ACTUAL_VERSION_PLUGINS_FILENAME)) 
			{
				var tempStr = AMEditorGit.RequestGet (LINK_PUBLIC_REPO);
				File.WriteAllText (LocalRepositoryAPI.pathToRepository + ACTUAL_VERSION_PLUGINS_FILENAME, tempStr);
			}
			
			externalPlugins = new List<ExternalPlugin> ();
			
			if (!AMEditorFileStorage.FileExist (LocalRepositoryAPI.pathToRepository + EXTERNAL_PLUGINS_FILENAME)) 
			{
				try
				{
					var tempStr = AMEditorGit.RequestGet (LINK_PUBLIC_REPO_EXTERNAL_PLUGIN);
					File.WriteAllText (LocalRepositoryAPI.pathToRepository + EXTERNAL_PLUGINS_FILENAME, tempStr);
				}
				catch (Exception)
				{
					if (!firstStart)
						AMEditorPopupErrorWindow.ShowErrorPopup ("311", AMEditorSystem.ContentError._311);
				}
			}
			ArrayList listExternalPlugin = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (LocalRepositoryAPI.pathToRepository + EXTERNAL_PLUGINS_FILENAME)) as ArrayList;
			if (listExternalPlugin != null)
			{
				foreach (var item in listExternalPlugin) 
				{
					externalPlugins.Add (new ExternalPlugin (item as Hashtable));
				}
			}
			ArrayList listActual = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (LocalRepositoryAPI.pathToRepository + ACTUAL_VERSION_PLUGINS_FILENAME)) as ArrayList;
			if ((listActual == null) || (listActual.Count == 0))
			{
				EditorUtility.ClearProgressBar ();
				if (!firstStart)
					AMEditorPopupErrorWindow.ShowErrorPopup ("311", AMEditorSystem.ContentError._311);
				return false;
			}
			else
			{
				UpdateActualPluginsList (listActual);

				if (!authError)
					SearchConfigFile ();
				CheckingMissFiles ();
				CheckingDependedFiles ();
				CheckingActualVersion ();
				if (installedPlugins != null)
					AMEditorConflictAPI.GetConflict (installedPlugins);
				UpdatePluginsGUI ();
			}
			return true;
		}
		
		static void DeleteLocalRepositories ()
		{
			var path = LocalRepositoryAPI.pathToRepository + LOCAL_REPOSITORY;
			var dirs = Directory.GetDirectories (path);
			string dir = "";
			
			try
			{
				for (int s = 0; s < dirs.Length; s++)
				{
					dir = dirs[s];
					dir = dir.Substring (dir.LastIndexOf (Path.DirectorySeparatorChar) + 1);
					AMEditorFileStorage.RemoveFolder (path + Path.DirectorySeparatorChar.ToString () + dir);
				}
				Debug.Log ("Local repository was cleared successfully.");
			}
			catch (Exception)
			{
				AMEditorPopupErrorWindow.ShowErrorPopup ("", AMEditorSystem.ContentError._309 (dir));
			}
		}
		
		static string GetActualNamePlugin (string currentName)
		{
			var result = currentName;
			if (actualPlugins != null)
			{
				foreach (var item in actualPlugins) 
				{
					if ((item.buildTypes != null && item.buildTypes.Count > 0 && item.buildTypes [0] == selectedBuildType)  || 
					  item.buildTypes == null || item.buildTypes.Count == 0) 
					{
						var index = item.oldNames.FindIndex (name => { return name == currentName; }); 
						if (index != -1) 
						{
							result = item.name;
							break;
						}
					}
				}
			}
			return result;
		}
		
		public static void CheckVersionActuality (Plugin source)
		{
			if (actualPlugins == null)
			{
				ArrayList actual = new ArrayList ();
				try 
				{
					if (!AMEditorFileStorage.FileExist (LocalRepositoryAPI.pathToRepository + ACTUAL_VERSION_PLUGINS_FILENAME)) 
					{
						var tempStr = AMEditorGit.RequestGet (LINK_PUBLIC_REPO);
						File.WriteAllText (LocalRepositoryAPI.pathToRepository + ACTUAL_VERSION_PLUGINS_FILENAME, tempStr);
					}
					actual = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (LocalRepositoryAPI.pathToRepository + ACTUAL_VERSION_PLUGINS_FILENAME)) as ArrayList;
				} 
				catch (Exception) 
				{}
				if (actual == null || actual.Count == 0)
				{
					actual = new ArrayList ();
				}
				AMEditor.WindowMain.UpdateActualPluginsList (actual);
			}
			
			var index = actualPlugins.FindIndex (ap => {return ap.name == source.name  && 
				 (ap.buildTypes == null && source.buildTypes == null  || 
				 ap.buildTypes.Count == 0 && source.buildTypes.Count == 0  || 
				 (ap.buildTypes.Count > 0 && source.buildTypes.Count > 0 && ap.buildTypes[0] == source.buildTypes[0])); });
			
			if (index != -1)
			{
				if (!source.EqualsVersion (actualPlugins[index].version))
				{
					source.errors.oldVersion = true;
				}
			}
		}
		
		static void AddPluginInQueue (string namePlugin, string pluginVersion)
		{
			namePlugin = GetActualNamePlugin (namePlugin);
			string pluginData = namePlugin + " " + pluginVersion;
			
			if (DownloadQueue == null)
			{
				DownloadQueue = new List<string> ();
			}
			
			var index = DownloadQueue.FindIndex ((item) => {return item.Contains (namePlugin);});
			if (index == -1)
			{
				DownloadQueue.Add (pluginData);
			}
		}
		public void DownloadPluginInQueue ()
		{
			cancelPressed = false;
			if ((DownloadQueue != null) && (DownloadQueue.Count > 0))
			{
				if (launchMode == LaunchMode.UI)
				{
					amEditorIsBusy = true;
				}

				isDownloadPlugin = true;
				var index = actualPlugins.FindIndex ((p) => {
					return (p.name + " " + p.version) == DownloadQueue [0];
				});
				
				if (index != -1)
				{
					DownloadFilesThroughPublicConfig (actualPlugins [index]);
				}
				else
				{
					DownloadQueue.Remove (DownloadQueue [0]);
				}
			}
			else
			{
				if (launchMode == LaunchMode.Console)
				{
					if (!IsAllGood ())
						MakeAllGood ();
					else
					{
						UnityCompileHandler.CompilationCompleteHandler ();
						if (MakeAllGoodComplete != null)
							MakeAllGoodComplete ("0");
					}
				}
			}
		}

		static AMEditorGit currentDownloadGit;
		void DownloadFilesThroughPublicConfig (ActualPlugin plugin)
		{
			if (!IsGoodInLocalRepo (plugin))
			{
				CurrentProgressDownloads = 0.0f;
			}
			CurrentPluginData = plugin.name + " " + plugin.version;
			if (instance == null && !cancelCurrentWork)
			{ 
				try
				{
					if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._Update, AMEditorSystem.ContentProgressBar._Plugin + CurrentPluginData, CurrentProgressDownloads))
					{
						CancelProgressBar ();
						cancelCurrentWork = true;
					}
				}
				catch (Exception)
				{}
			}
			CurrentDownloadActualPlugin = plugin;

			if (IsGoodInLocalRepo (plugin))
			{
				currentWindow = WindowType.Wait;
				messageWait = AMEditorSystem.ContentProgressBar._DownloadTo + plugin.name;
				try
				{
					float step = 1f / (DownloadQueue.Count + 1);
					ChangeStatusHandler (AMEditorSystem.ContentProgressBar._CheckingLocalRepo, step);
				}
				catch (Exception)
				{}
				DownloadHandler (false);
			}
			else
			{
				try
				{
					currentDownloadGit = new AMEditorGit ();
					currentDownloadGit.ChangeStatus += ChangeStatusHandler;
					currentDownloadGit.ArchiveDownloadComplete += DownloadCompleteHandler;
					currentDownloadGit.ErrorHappened += ErrorHappenedHandler;
					#if AM_EDITOR_DEBUG_MODE_ON
						currentDownloadGit.printDebug = true;
					#else
						currentDownloadGit.printDebug = false;
					#endif
					currentDownloadGit.AuthByPT (GitAccount.current.server, GitAccount.current.privateToken);
					switch (launchMode)
					{
					case LaunchMode.Console:
						currentDownloadGit.GetProjectForCI (new string[] {plugin.name}, plugin.version, groupName, branchName, LocalRepositoryAPI.pathToRepository + LOCAL_REPOSITORY);
						break;
					case LaunchMode.UI:
						currentDownloadGit.GetProjectFromArchive (new string[] {plugin.name}, plugin.version, groupName, branchName, LocalRepositoryAPI.pathToRepository + LOCAL_REPOSITORY);
						break;
					default:
						break;
					}
					
					currentWindow = WindowType.Wait;
					messageWait = AMEditorSystem.ContentProgressBar._DownloadTo + plugin.name;
					
					if (instance == null && !cancelCurrentWork)
					{
						if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._Update, AMEditorSystem.ContentProgressBar._Plugin + plugin.name, 0.0f))
						{
							CancelProgressBar ();
							cancelCurrentWork = true;
						}
					}
				}
				catch (Exception)
				{}
			}
		}

		static void ChangeStatusHandler (string status, float step)
		{
			CurrentStatusPlugin = status;
			CurrentProgressStep = step;

			try
			{
				EditorApplication.update += UpdateStatus;
			}
			catch (Exception)
			{}
			
			Debug.Log ("Status: " + status);
		}
		static void UpdateStatus ()
		{
			try
			{
				EditorApplication.update -= UpdateStatus;
				if (CurrentProgressDownloads > 1)
					CurrentProgressDownloads -= 1;
				if (instance == null && !cancelCurrentWork)
				{
					if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._Update, AMEditorSystem.ContentProgressBar._Plugin + CurrentPluginData
				    	                    + ". " + CurrentStatusPlugin , CurrentProgressDownloads))
					{
						CancelProgressBar ();
						cancelCurrentWork = true;
					}
				}
				CurrentProgressDownloads += CurrentProgressStep;
			}
			catch (Exception)
			{}
		}
		void ErrorHappenedHandler (string error)
		{
			currentDownloadGit.ErrorHappened -= ErrorHappenedHandler;
			if (error != string.Empty)
			{
				Debug.LogError (error);
			}
			DownloadCompleteHandler (false);
			if (error != string.Empty && launchMode == LaunchMode.Console && MakeAllGoodComplete != null)
				MakeAllGoodComplete (error);
		}
		
		void DownloadCompleteHandler (bool downloadComplete)
		{
			Debug.Log ("download complete: " + downloadComplete);
			currentDownloadGit.ArchiveDownloadComplete -= DownloadCompleteHandler;
			if (downloadComplete || cancelPressed)
			{
				if (launchMode == LaunchMode.Console)
				{
					DownloadHandler (false);
				}
				else
				{
					DownloadHandler ();
				}
			}
			else
			{
				EditorApplication.update += DownloadFailed;
				try
				{
					//EditorApplication.Step ();
				}
				catch (Exception)
				{
					DownloadFailed ();
				}
			}
		}
		void DownloadFailed ()
		{
			amEditorIsBusy = false;
			try
			{
				EditorApplication.update -= DownloadFailed;
				currentDownloadGit.ErrorHappened -= ErrorHappenedHandler;
				EditorUtility.ClearProgressBar ();
				currentDownloadGit.StopDownload ();
				currentDownloadGit = new AMEditorGit ();

				DownloadQueue = new List<string> ();
				CheckList = new List<string> ();
				localRepositoryPluginsPathes = new List<string> ();
				isDownloadPlugin = false;
				isInit = false;

				if (EditorUtility.DisplayDialog (AMEditorSystem.ContentProgressBar._FailedUpdateTitle, AMEditorSystem.ContentProgressBar._FailedUpdateMessage, AMEditorSystem.ContentStandardButton._Ok))
				{
					currentWindow = WindowType.ListPlugins;
					UpdateAllPluginsLists ();
				}
			}
			catch (Exception)
			{
				currentWindow = WindowType.ListPlugins;
				UpdateAllPluginsLists ();
			}
		}
		bool IsGoodInLocalRepo (ActualPlugin actualPlugin)
		{
			bool result = true;
			string pathToPlugin = string.Empty;
			string localRepoPath = LocalRepositoryAPI.pathToRepository + LOCAL_REPOSITORY + System.IO.Path.DirectorySeparatorChar.ToString () + actualPlugin.name + " " + actualPlugin.version; 
			if (!Directory.Exists (localRepoPath))
				return false;
			var subFolders = Directory.GetDirectories (localRepoPath);
			if (subFolders != null && subFolders.Length > 0)
			{
				pathToPlugin = localRepoPath + System.IO.Path.DirectorySeparatorChar.ToString () + subFolders[0].Substring (subFolders[0].LastIndexOf (System.IO.Path.DirectorySeparatorChar.ToString ()));
			}
			if (!AMEditorFileStorage.FileExist (pathToPlugin + System.IO.Path.DirectorySeparatorChar.ToString () + PLUGIN_CONFIG_FILENAME)) 
			{
				return false;
			}
			var newConfig = new Plugin (AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (pathToPlugin + System.IO.Path.DirectorySeparatorChar.ToString () + PLUGIN_CONFIG_FILENAME)) as Hashtable, groupName);
			foreach (var item in newConfig.files) 
			{
				var ind = item.parameters.FindIndex ((type) => {return Plugin.FileParameter.outdated == type;});
				if (ind == -1)
				{
					if (!AMEditorFileStorage.FileExist (pathToPlugin + System.IO.Path.DirectorySeparatorChar.ToString () + item.path))
					{
						result = false;
					}
					else
					{
						if (AMEditorFileStorage.HashFile (pathToPlugin + System.IO.Path.DirectorySeparatorChar.ToString () + item.path) != item.hash) // &&  item.parameters.FindIndex ((type) => {return Plugin.FileParameter.modifiable == type;}) == -1)
						{
							result = false;
						}
					}
				}
			}
			if (!newConfig.EqualsVersion (actualPlugin.version))
				return false;
			
			return result;
		}

		string exampleFilePath = string.Empty;
		static List<string> localRepositoryPluginsPathes;
		void DownloadHandler (bool threadSleepRequired = true)
		{
			if (localRepositoryPluginsPathes == null || localRepositoryPluginsPathes.Count == 0)
			{
				localRepositoryPluginsPathes = new List<string> ();
			}
			if (tempDownloadedPlugins == null || tempDownloadedPlugins.Count == 0)
			{
				tempDownloadedPlugins = new List<Plugin> ();
			}

			FirstUpdateGUI = true;

			string localRepoPath = LocalRepositoryAPI.pathToRepository + LOCAL_REPOSITORY + System.IO.Path.DirectorySeparatorChar.ToString () + CurrentPluginData;
			string downloadedPluginPath = string.Empty;
			var subFolders = Directory.GetDirectories (localRepoPath);
			if (subFolders != null && subFolders.Length > 0)
			{
				downloadedPluginPath = localRepoPath + subFolders[0].Substring (subFolders[0].LastIndexOf (System.IO.Path.DirectorySeparatorChar.ToString ()));
			}
			Plugin downloadedPluginConfig = new Plugin (AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (downloadedPluginPath + System.IO.Path.DirectorySeparatorChar.ToString () + PLUGIN_CONFIG_FILENAME)) as Hashtable);

			if (!localRepositoryPluginsPathes.Contains (downloadedPluginPath))
			{
				localRepositoryPluginsPathes.Add (downloadedPluginPath);
			}

			if (!tempDownloadedPlugins.Contains (downloadedPluginConfig))
			{
				tempDownloadedPlugins.Add (downloadedPluginConfig);
			}
			
			var dep = downloadedPluginConfig.GetDepends ();
			foreach (var item in dep) 
			{
				var indexInListPlugin = installedPlugins.FindIndex ((p) => {return (p.name == item.name);});
				if (indexInListPlugin == -1)
				{
					var indexInTempDownloaded = tempDownloadedPlugins.FindIndex ((p) => {return (p.name == item.name);});
					if (indexInTempDownloaded == -1)
					{
						AddPluginInQueue (item.name, item.version);
					}
				}
			}
			
			var downloaded = (from p in DownloadQueue
			         where p == downloadedPluginConfig.name + " " + downloadedPluginConfig.version
			         select p).ToList ();
			foreach (var item in downloaded)
			{
				DownloadQueue.Remove (item);
			}
			
			if (DownloadQueue.Count == 0)
			{
				float copyStep = 1f / (localRepositoryPluginsPathes.Count + 1);
				foreach (var pluginPath in localRepositoryPluginsPathes)
				{
					try
					{
						var pluginPathFolders = pluginPath.Split (System.IO.Path.DirectorySeparatorChar).ToList ();
						if (File.Exists (pluginPath + System.IO.Path.DirectorySeparatorChar.ToString () + PLUGIN_CONFIG_FILENAME))
						{
							CurrentPluginData = pluginPathFolders [pluginPathFolders.Count - 2];
						}
						ChangeStatusHandler (AMEditorSystem.ContentProgressBar._CopyFiles + ". " + CurrentPluginData, copyStep);
					}
					catch (Exception)
 					{}

					var config = new Plugin (AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (pluginPath + System.IO.Path.DirectorySeparatorChar.ToString () + PLUGIN_CONFIG_FILENAME)) as Hashtable);
					List<string> filesPathesList;

					if (exampleFilePath != string.Empty && exampleFilePath != null)
					{
						filesPathesList = new List<string> ();
						filesPathesList.Add (exampleFilePath);
						exampleFilePath = string.Empty;
					}
					else
					{
						filesPathesList = new List<string> ();

						if (config.name == "Custom Resources Manager")// && launchMode == LaunchMode.UI)
						{
							compilationHandlerName = "RestoreCRMComponents";
							UnityCompileHandler.SetHandlerName (compilationHandlerName);
						}

						foreach (var item in config.files) 
						{
							var ind = item.parameters.FindIndex ((type) => {return (Plugin.FileParameter.outdated == type || Plugin.FileParameter.example == type); });

							if (ind == -1)
							{
								filesPathesList.Add (item.path);
							}
						}
					}

					foreach (var item in filesPathesList) 
					{
						AMEditorFileStorage.CopyFile (pluginPath + System.IO.Path.DirectorySeparatorChar.ToString () + item, item, true);
						
						if (!MetaFilesAPI.CopyMetafile (pluginPath + System.IO.Path.DirectorySeparatorChar.ToString () + item + ".meta", item + ".meta"))
						{
							AMEditorPopupErrorWindow.ShowErrorPopup ("301", AMEditorSystem.ContentError._301);
						}

						if (item.Contains (".bundle"))
						{
							var folders = item.Split (new char[]{ System.IO.Path.DirectorySeparatorChar });
							string pathBundle = string.Empty;
							foreach (var f in folders) 
							{
								pathBundle += f;
								if (!f.Contains (".bundle"))
									pathBundle += System.IO.Path.DirectorySeparatorChar.ToString ();
								else
								{
									if (AMEditorFileStorage.FileExist (pluginPath + System.IO.Path.DirectorySeparatorChar.ToString () + pathBundle + ".meta"))
									{
										if (!MetaFilesAPI.CopyMetafile (pluginPath + System.IO.Path.DirectorySeparatorChar.ToString () + pathBundle + ".meta", pathBundle + ".meta"))
										{
											AMEditorPopupErrorWindow.ShowErrorPopup ("301", AMEditorSystem.ContentError._301);
										}
									}
									break;
								}
							}
						}
					}
					
					int index = installedPlugins.FindIndex ((p) => {
						return p.name == config.name;});
					if (index != -1)
					{
						installedPlugins [index].Update (config);
					}
					else
					{
						installedPlugins.Add (config);
					}
				}
				tempDownloadedPlugins = null;
				localRepositoryPluginsPathes = null;
				
				currentWindow = WindowType.ListPlugins;
				SavePlugins ();
				UpdateAllPluginsLists ();

				SaveEditorState ();

				FixMetaALibs ();
				FixMetaBundle ();
			}
			EditorApplication.update += UpdateGUI;
			try
			{
				switch (launchMode)
				{
				case LaunchMode.Console:
					if (DownloadQueue == null || DownloadQueue.Count == 0)
					{
						isDownloadPlugin = false;
						AssetDatabase.Refresh ();

						if (!IsAllGood ())
						{
							MakeAllGood ();
						}
						else
						{
							UnityCompileHandler.CompilationCompleteHandler ();
							if (MakeAllGoodComplete != null)
								MakeAllGoodComplete ("0");
						}
					}
					DownloadPluginInQueue ();
					break;
				case LaunchMode.UI:
					if (threadSleepRequired)
					{
						System.Threading.Thread.Sleep (new TimeSpan (0, 0, 0, 0, 600));
					}
					if (instance != null)
					{
						instance.Focus ();
					}
					else 
					{
						Focus ();
					}
					break;
				default:
					break;
				}
			}
			catch (Exception)
			{
				UpdateGUI ();
			}
		}
		
		public static void AutoFixConflict (Plugin plugin, bool refreshAssetsRequired = false)
		{
			AMEditorConflictAPI.GetConflict (plugin);

			foreach (var file in plugin.conflictFiles) 
			{
				if (file.delete)
				{
					AMEditorBackupAPI.BackupingFile (file.name);
					var listFolder = file.name.Split (new char[]{System.IO.Path.DirectorySeparatorChar});
					var path = file.name;
					for (int i = 0; i < listFolder.Length - 1; i++) 
					{
						path = path.Substring (0, path.LastIndexOf (System.IO.Path.DirectorySeparatorChar.ToString ()));
						RemoveEmptyFolder (path);
					}
				}
			}
			foreach (var file in plugin.conflictFilesOldFiles) 
			{
				if (file.delete)
				{
					AMEditorBackupAPI.BackupingFile (file.name);

					var listFolder = file.name.Split (new char[]{System.IO.Path.DirectorySeparatorChar});
					var path = file.name;
					for (int i = 0; i < listFolder.Length - 1; i++) 
					{
						path = path.Substring (0, path.LastIndexOf (System.IO.Path.DirectorySeparatorChar.ToString ()));
						RemoveEmptyFolder (path);
					}
				}
			}
			AMEditorBackupAPI.pathBackup = string.Empty;
			if (launchMode == LaunchMode.UI && refreshAssetsRequired)
			{
				AssetDatabase.Refresh ();
			}
			installedPlugins = new List<Plugin> ();
			isInit = false;
		}
		
		static bool RemoveEmptyFolder (string path)
		{	
			try
			{
				if (Directory.Exists (path) && Directory.GetDirectories (path).Length == 0 && Directory.GetFiles (path).Length == 0)
				{
					if (AMEditorFileStorage.FileExist (path + ".meta"))
					{
						AMEditorFileStorage.RemoveFile (path + ".meta");
					}
					return AMEditorFileStorage.RemoveFolder (path);
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		static int candidatesWithDepends = 0;
		void DeleteSelectedPlugins ()
		{
			var candidatesList = installedPluginsGUIList.FindAll ((p) => {
				return p.selected;
			});

			foreach (var item in candidatesList)
			{
				int index = installedPlugins.FindIndex ((p) => {
					return p.name == item.name && p.depends != null && p.depends.Count > 0 && item.name != "Custom Code";
				});
				if (index != -1)
				{
					candidatesWithDepends++;
				}
			}

			bool successDeleted = false;
			foreach (var item in installedPluginsGUIList) 
			{
				if (item.selected)
				{
					successDeleted = DeletePlugin (item.name, false);
					if (!successDeleted)
						break;
				}
			}

			if (successDeleted)
			{
				applyForAll = false;
				dialogDecision = -1;
				try
				{
					installedPluginsGUIList.RemoveAll ((p) => {
						return p.selected;
					});
				}
				catch (Exception)
				{}
				AssetDatabase.Refresh ();
				SavePlugins ();
				UpdateAllPluginsLists ();
			}
		}
		static int dialogDecision = -1;
		static bool applyForAll = false;
		bool DeletePlugin (string pluginName, bool singleDeleting = true)
		{
			bool forceDeleting = false;

			var index = installedPlugins.FindIndex ((p) => {
				return p.name == pluginName;
			});
			
			if (index != -1)
			{
				if (installedPlugins[index].name.Equals ("Custom Code"))
				{
					forceDeleting = true;
				}
				else if (installedPlugins[index].depends.Count > 0)
				{
					if (launchMode == LaunchMode.UI)
					{
						if (!applyForAll)
							dialogDecision = EditorUtility.DisplayDialogComplex (AMEditorSystem._ContentTitleDeleteDepends, 
								AMEditorSystem._ContentQuestionDeleteDepends (installedPlugins [index].name), 
								AMEditorSystem.ContentStandardButton._WithDepends, 
								AMEditorSystem.ContentStandardButton._OnlyCurrent, 
								AMEditorSystem.ContentStandardButton._Cancel);

						if (candidatesWithDepends > 1 && dialogDecision != 2 && !applyForAll)
						{
							applyForAll = EditorUtility.DisplayDialog (AMEditorSystem._ContentTitleDeleteDepends, 
								AMEditorSystem._ContentQuestionSeveralPlugins, 
								AMEditorSystem.ContentStandardButton._ApplyForAll, 
								AMEditorSystem.ContentStandardButton._ApplyForCurrent);
						}

						switch (dialogDecision)
						{
						case 0:
							forceDeleting = true;
							break;
						case 1:
							forceDeleting = false;
							break;
						case 2:
							return false;
						}
					}
					else
					{
						forceDeleting = true;
					}
					candidatesWithDepends--;
				}
				DeletePlugin (installedPlugins[index], forceDeleting, singleDeleting);
			}
			CustomResourceManagerAPI.CheckAssetBundlesAvailability ();
			return true;
		}

		void DeletePlugin (Plugin source, bool forceDeleting, bool singleDeleting)
		{
			List<Plugin> pluginsToDelete = new List<Plugin> ();
			pluginsToDelete.Add (source);
			
			if (forceDeleting && source.depends.Count > 0)
			{
				foreach (var depend in source.depends)
				{
					var list = (from p in installedPlugins
					      where p.name == depend.name
					      select p).ToList ();
					
					if ((list != null) && (list.Count > 0))
					{
						pluginsToDelete.Add (list[0]);
					}
				}
			}
			
			foreach (var item in pluginsToDelete)
			{
				foreach (var f in item.files)
				{
					if (AMEditorFileStorage.FileExist (f.path))
					{
						AMEditorFileStorage.RemoveFile (f.path);
					}
					if (AMEditorFileStorage.FileExist (f.path + ".meta"))
					{
						AMEditorFileStorage.RemoveFile (f.path + ".meta");
					}
					var listFolder = f.path.Split (new char[] { System.IO.Path.DirectorySeparatorChar });
					var path = f.path;
					for (int i = 0; i < listFolder.Length - 1; i++)
					{
						path = path.Substring (0, path.LastIndexOf (System.IO.Path.DirectorySeparatorChar.ToString ()));
						RemoveEmptyFolder (path);
					}
				}
				installedPlugins.Remove (item);
			}
			if (singleDeleting)
			{
				AssetDatabase.Refresh ();
				SavePlugins ();
				UpdateAllPluginsLists ();
			}
			if (source.name == "AM Editor")
				this.Close ();
		}
		
		static bool FirstUpdateGUI = true;
		void UpdateGUI ()
		{
			if (!FirstUpdateGUI)
			{
				return;
			}
			FirstUpdateGUI = false;
			try 
			{
				EditorApplication.update -= UpdateGUI;
				EditorUtility.ClearProgressBar ();
			} 
			catch (Exception) 
			{}
			try 
			{
				if (cancelPressed)
				{
					currentDownloadGit.StopDownload ();
					if (EditorUtility.DisplayDialog (AMEditorSystem.ContentProgressBar._CancelUpdateTitle, AMEditorSystem.ContentProgressBar._CancelUpdateMessage, AMEditorSystem.ContentStandardButton._Ok))
					{
						cancelPressed = false;
						currentWindow = WindowType.ListPlugins;
						UpdateAllPluginsLists ();
						amEditorIsBusy = false;
					}
				}
				else
				{
					if (DownloadQueue == null || DownloadQueue.Count == 0)
					{
						isDownloadPlugin = false;
						if (launchMode == LaunchMode.Console)
						{
							if (!IsAllGood ())
								MakeAllGood ();
							else
							{
								if (MakeAllGoodComplete != null)
									MakeAllGoodComplete ("0");
							}
						}
						else
						{
							var conflictList = installedPlugins.FindAll ((p) => {return p.errors.conflict;});
							if (conflictList != null && conflictList.Count > 0)
							{
								float progress = 0;
								float step = (float)(1 / conflictList.Count);
								foreach (var item in conflictList)
								{
									currentWindow = WindowType.Wait;
									messageWait = AMEditorSystem.ContentProgressBar._AutoFix + item;
									CurrentPluginData = item.name + " " + item.version;
									CurrentStatusPlugin = AMEditorSystem.ContentProgressBar._AutoFix;

									if (instance == null && !cancelCurrentWork)
									{
										if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._AutoFix, item.name + " " + AMEditorSystem.ContentProgressBar._AutoFix, progress))
										{
											CancelProgressBar ();
											cancelCurrentWork = true;
										}
									}
									try
									{
										AutoFixConflict (item, true);
									}
									catch (Exception)
									{}
									float subStep = step / 10;
									for (float i = 0; i < 10; i++)
									{
										progress += subStep;
										if (instance == null && !cancelCurrentWork)
										{
											if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._AutoFix, item.name + " " + AMEditorSystem.ContentProgressBar._AutoFix, progress))
											{
												CancelProgressBar ();
												cancelCurrentWork = true;
											}
										}
									}
								}
							}
							amEditorIsBusy = false;
							CancelProgressBar ();
							ShowDisplayDialog ();
						}
					}
				}
			} 
			catch (Exception) 
			{}
			DownloadPluginInQueue ();
		}
		static void ShowDisplayDialog ()
		{
			EditorApplication.update -= ShowDisplayDialog;

			try 
			{
				new AMEditor.UI.AMDisplayDialog (AMEditorSystem.ContentProgressBar._Update, AMEditorSystem.ContentProgressBar._SuccessUpdateMessage, 
					AMEditorSystem.ContentStandardButton._Ok, "", () => 
					{
						if (!UnityCompileHandler.HandlerNameEquals ("RestoreCRMComponents"))
							CustomResourceManagerAPI.CheckCorrect (); 

						AssetDatabase.Refresh ();
					}, () => { }, true).Show ();
			} 
			catch (Exception) 
			{
				File.WriteAllText (NEED_DISPLAY_SUCCESS_DIALOG, "");
			}
		}
		
		static void DeleteOutdatedPlugin (Plugin plugin)
		{
			for (int i = 0; i < installedPlugins.Count; i++) 
			{
				int index = plugin.oldNames.FindIndex ((p) => {return p == installedPlugins[i].name;});
				if (index != -1)
				{
					installedPlugins.RemoveAt (i);
					i--;
				}
			}
			if (instance != null)
			{
				instance.SavePlugins ();
			}
		}
		static bool GetPrivateToken ()
		{
			if ((GitAccount.current != null) && (!string.IsNullOrEmpty (GitAccount.current.privateToken)))
			{
				return true;
			}
			return false;
		}
		
		string GetFileName (string path)
		{
			return path.Substring (path.LastIndexOf (System.IO.Path.DirectorySeparatorChar.ToString ()) + 1); 
		}
		
		void CheckingActualVersion ()
		{
			foreach (var plugin in installedPlugins) 
			{
				var index = actualPlugins.FindIndex ((p) => {return p.name == plugin.name;});
				if (index != -1)
				{
					if (!plugin.EqualsVersion (actualPlugins[index].version))
					{
						plugin.errors.oldVersion = true;
					}
				}
			}
		}
		
		void CheckingDependedFiles ()
		{
			foreach (var plugin in installedPlugins) 
			{
				plugin.dependErrors = string.Empty;
				var result = false;
				
				foreach (var d in plugin.depends) 
				{
					var dependedPlugin = installedPlugins.FindIndex ((p) => {return (p.name == d.name);});
					if (dependedPlugin == -1 || !installedPlugins[dependedPlugin].EqualsVersion (d.version))
					{
						result = true;
						if (plugin.dependErrors == string.Empty)
						{
							plugin.dependErrors = AMEditorSystem.ContentProblems._Depends + Environment.NewLine;
						}
						plugin.dependErrors += d.name + Environment.NewLine;
					}	
				}
				plugin.errors.dependPlugins = result;
			}
		}
		void CheckingMissFiles ()
		{
			foreach (var plugin in installedPlugins) 
			{
				CheckingMissFilesAndHash (plugin);
			}
		}
		public static void CheckingMissFilesAndHash (Plugin source)
		{
			var result = false;
			foreach (var f in source.files) 
			{
				var index = f.parameters.FindIndex ((p) => {return p == Plugin.FileParameter.outdated;});
				if (index == -1)
				{
					if (!f.path.Contains (".prefab") && !f.path.Contains (".unity") && !f.path.Contains ("plugin_config.json"))
					{
						if (!AMEditorFileStorage.FileExist (f.path))
						{
							result = true;
						}
						else
						{
							if (AMEditorFileStorage.HashFile (f.path) != f.hash && f.parameters.FindIndex ((type) => {return Plugin.FileParameter.modifiable == type;}) == -1)
							{
								source.errors.missingFilesHash = true;
							}
						}
					}
				}
			}
			source.errors.missingFiles = result;
		}
		
		void MergeAllOutdatedPlugin ()
		{
			int CountPlugins = installedPlugins.Count;
			for (int i = 0; i < installedPlugins.Count; i++) 
			{
				DeleteOutdatedPlugin (installedPlugins[i]);
				if (CountPlugins != installedPlugins.Count)
				{
					CountPlugins = installedPlugins.Count;
					i--;
				}
			}
		}
		public bool IsAllGood ()
		{
			if (CheckList == null)
			{
				CheckList = new List<string> ();
			}
			
			List<string> byildTypesCheckList = new List<string> ();
			List<string> missingFilesCheckList = new List<string> ();
			List<string> conflictsCheckList = new List<string> ();
			List<string> changedFilesCheckList = new List<string> ();
			List<string> needDependsCheckList = new List<string> ();
			List<string> needUpdateCheckList = new List<string> ();
			List<string> needMandatoryCheckList = new List<string> ();
			List<string> outdatedPluginsCheckList = new List<string> ();
			
			foreach (var item in installedPluginsGUIList)
			{
				if (item.buildTypes.Count > 0 && !item.buildTypes[0].Equals (selectedBuildType))
				{
					if (!byildTypesCheckList.Contains (item.name + " " + item.actualVersion))
						byildTypesCheckList.Add (item.name + " " + item.actualVersion);
					if (launchMode == LaunchMode.Console)
					{
						var plugin = (from p in installedPlugins
						       where p.name == item.name
						       select p).ToList ();
						if (plugin != null && plugin.Count > 0)
						{
							if (plugin[0].depends.Count > 0)
							{
								foreach (var depend in plugin[0].depends)
								{
									var list = (from p in installedPluginsGUIList
									      where p.name == depend.name
									      select p).ToList ();
									
									if ((list != null) && (list.Count > 0))
									{
										if (!byildTypesCheckList.Contains (list[0].name + " " + list[0].actualVersion))
											byildTypesCheckList.Add (list[0].name + " " + list[0].actualVersion);
									}
								}
							}
						}
					}
				}
				if (item.error.missingFiles && item.permittedVersions.Contains (item.currentVersion))
				{
					missingFilesCheckList.Add (item.name + " " + item.currentVersion);
				}
				if (item.error.conflict)
				{
					conflictsCheckList.Add (item.name + " " + item.currentVersion);
				}
				if (item.error.missingFilesHash && item.permittedVersions.Contains (item.currentVersion))
				{
					changedFilesCheckList.Add (item.name + " " + item.currentVersion);
				}
				if (item.error.dependPlugins && item.permittedVersions.Contains (item.currentVersion))
				{
					needDependsCheckList.Add (item.name + " " + item.currentVersion);
				}
				if (item.error.noActualVersion)
				{
					needUpdateCheckList.Add (item.name + " " + item.actualVersion);
				}
				if (mandatoryPluginsGUIList != null && mandatoryPluginsGUIList.Count > 0)
				{
					foreach (var plugin in mandatoryPluginsGUIList)
					{
						needMandatoryCheckList.Add (plugin.name + " " + plugin.actualVersion);
					}
				}
				if (item.actualVersion == "?") 
				{
					outdatedPluginsCheckList.Add (item.name + " " + item.actualVersion);
				}
			}
			if (byildTypesCheckList.Count > 0)
			{
				pluginsStatus = PluginsStatus.BuildTypeProblems;
				CheckList = byildTypesCheckList;
				return false;
			}
			if (missingFilesCheckList.Count > 0)
			{
				pluginsStatus = PluginsStatus.MissingFiles;
				CheckList = missingFilesCheckList;
				return false;
			}
			if (conflictsCheckList.Count > 0)
			{
				pluginsStatus = PluginsStatus.HasConflicts;
				CheckList = conflictsCheckList;
				return false;
			}
			if (changedFilesCheckList.Count > 0)
			{
				pluginsStatus = PluginsStatus.ChangedFiles;
				CheckList = changedFilesCheckList;
				return false;
			}
			if (needDependsCheckList.Count > 0)
			{
				pluginsStatus = PluginsStatus.NeedDepends;
				CheckList = needDependsCheckList;
				return false;
			}
			if (needUpdateCheckList.Count > 0)
			{
				pluginsStatus = PluginsStatus.NeedUpdate;
				CheckList = needUpdateCheckList;
				return false;
			}
			if (needMandatoryCheckList.Count > 0) 
			{
				pluginsStatus = PluginsStatus.NeedMandatory;
				CheckList = needMandatoryCheckList;
				return false;
			}
			if (outdatedPluginsCheckList.Count > 0) 
			{
				pluginsStatus = PluginsStatus.OutdatedPlugins;
				CheckList = outdatedPluginsCheckList;
				return false;
			}
			pluginsStatus = PluginsStatus.AllGood;
			CheckList = new List<string> ();
			return true;
		}
		public delegate void MakeAllGoodDelegate (string exitCode);
		public event MakeAllGoodDelegate MakeAllGoodComplete;
		public void MakeAllGood ()
		{
			var ccIdx = CheckList.FindIndex (p => p.Contains ("Custom Code"));
			if (ccIdx != -1)
			{
				var ccItem = CheckList [ccIdx];
				CheckList.RemoveAt (ccIdx);
				CheckList.Insert (0, ccItem);
			}

			if (pluginsStatus == PluginsStatus.HasConflicts)
			{
				foreach (var item in CheckList)
				{
					currentWindow = WindowType.Wait;
					messageWait = AMEditorSystem.ContentProgressBar._AutoFix + item;
					CurrentPluginData = item;
					CurrentStatusPlugin = AMEditorSystem.ContentProgressBar._AutoFix;

					var index = installedPlugins.FindIndex ((p) => {return (p.name + " " + p.version == item);});
					if (index != -1)
					{
						if (instance == null && !cancelCurrentWork)
						{
							if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._AutoFix, item + " " + AMEditorSystem.ContentProgressBar._AutoFix, 0.33f))
							{
								CancelProgressBar ();
								cancelCurrentWork = true;
								return;
							}
						}
						try
						{
							AutoFixConflict (installedPlugins[index], true);
						}
						catch (Exception)
						{}
						for (int p = 330; p < 1000; p++)
						{
							float progress = (float)p / 1000;
							if (instance == null && !cancelCurrentWork)
							{
								if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._AutoFix, item + " " + AMEditorSystem.ContentProgressBar._AutoFix, progress))
								{
									CancelProgressBar ();
									cancelCurrentWork = true;
									return;
								}
							}
						}
					}
					else
					{
						if (launchMode == LaunchMode.UI)
						{
							if (EditorUtility.DisplayDialog (AMEditorSystem.ContentFixConflicts._FailedFixConflictTitle, AMEditorSystem.ContentFixConflicts._FailedFixConflict, AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._No))
							{
								currentWindow = WindowType.FixConfict;
								break;
							}
						}
						else
						{
							Debug.Log ("ERROR FIXING CONFLICTS");
							break;
						}
					}
				}
				if (launchMode == LaunchMode.UI)
				{
					if (EditorUtility.DisplayDialog (AMEditorSystem.ContentFixConflicts._SuccessFixConflictTitle, AMEditorSystem.ContentFixConflicts._SuccessFixConflict, AMEditorSystem.ContentStandardButton._Ok))
					{
						EditorUtility.ClearProgressBar ();
					}
					this.Focus ();
				}
			}
			else
			{
				if (pluginsStatus == PluginsStatus.BuildTypeProblems)
				{
					foreach (var item in CheckList)
					{
						int index = installedPluginsGUIList.FindIndex ((p) => {
							return (p.name + " " + p.actualVersion == item);
						});

						if (index != -1)
						{
							installedPluginsGUIList [index].selected = true;
						}
					}
					DeleteSelectedPlugins ();
				}
				else if (pluginsStatus == PluginsStatus.NeedUpdate)
				{
					if (launchMode == LaunchMode.Console)
					{
						if (pluginsWhitelist == null)
						{
							if (!AMEditorFileStorage.FileExist (LocalRepositoryAPI.pathToRepository + AMEDITOR_WHITELIST_FILENAME))
							{
								var tempStr = AMEditorGit.RequestGet (LINK_PUBLIC_REPO_WHITELIST);
								File.WriteAllText (LocalRepositoryAPI.pathToRepository + AMEDITOR_WHITELIST_FILENAME, tempStr);
							}
							var tempWhiteList = AMEditorJSON.JsonDecode (File.ReadAllText (LocalRepositoryAPI.pathToRepository + AMEDITOR_WHITELIST_FILENAME)) as ArrayList;
							pluginsWhitelist = new List<string> ();
							foreach (var item in tempWhiteList)
							{
								pluginsWhitelist.Add (item.ToString ());
							}
						}
						foreach (var item in CheckList)
						{
							int index = installedPlugins.FindIndex ((p) => {
								return (item.Contains (p.name) && !pluginsWhitelist.Contains (p.name));
							});

							if (index != -1)
							{
								DeletePlugin (installedPlugins [index], true, true);
							}
						}
						DeleteOutdatedPlugins ();
					}
				}
				else 
				{
					if (pluginsStatus == PluginsStatus.NeedDepends)
					{
						var tempDependsList = new List<string> ();
						foreach (var item in CheckList)
						{
							var curPlugin = 
								 (from p in installedPlugins
								where (p.name + " " + p.version == item)
								select p).ToArray ();
							if ((curPlugin != null) && (curPlugin.Length > 0))
							{
								var checkDependsResult = CheckDepends (curPlugin [0]);
								if (checkDependsResult.error.dependPlugins)
								{
									var dependsForCheckList = checkDependsResult.error.messageDependPlugins.Split (Environment.NewLine.ToCharArray ());
									foreach (var depend in dependsForCheckList)
									{
										if (depend != string.Empty && !tempDependsList.Contains (depend))
										{
											string currentDependName = depend.Substring (0, depend.LastIndexOf (" "));
											Version currentDependVersion = new Version (depend.Substring (depend.LastIndexOf (" ")));

											if (tempDependsList.FindIndex ((dep) => {
												return dep.Contains (currentDependName);
											}) != -1)
											{
												int index = tempDependsList.FindIndex ((dep) => {
													return currentDependVersion > (new Version (dep.Substring (dep.LastIndexOf (" "))));
												});
												if (index != -1)
												{
													tempDependsList [index] = depend;
												}
											}
											else
											{
												tempDependsList.Add (depend);
											}
										}
									}
								}
							}
						}
						CheckList = tempDependsList;
					}
				}
				DownloadQueue = new List<string> ();

				foreach (var item in CheckList)
				{
					if (!item.Contains ("?"))
					{
						DownloadQueue.Add (item);
					}
				}
				DeleteOutdatedPlugins ();
				DownloadPluginInQueue ();
			}
		}

		void DeleteOutdatedPlugins ()
		{
			foreach (var plugin in installedPluginsGUIList)
			{
				if (plugin.actualVersion == "?")
				{
					var index = installedPlugins.FindIndex ((p) => {
						return p.name == plugin.name;
					});

					if (index != -1)
					{
						DeletePlugin (installedPlugins[index], true, false);
					}
				}
			}
			AssetDatabase.Refresh ();
			SavePlugins ();
			UpdateAllPluginsLists ();
		}

		public static bool buildTypeProblems = false;
		void DrawWindowListPlugins ()
		{
			#region Plugins List Header
			GUIStyle currentAccountLabelStyle = new GUIStyle (GUI.skin.label);
			currentAccountLabelStyle.alignment = TextAnchor.MiddleRight;
			GUIStyle windowHeaderBackStyle = EditorGUIUtility.isProSkin ? new GUIStyle (GUI.skin.textField) : new GUIStyle (GUI.skin.box);
			GUI.Box (new Rect (-1, -1, Screen.width + 2, 29), "", windowHeaderBackStyle);

			//#region DLC Header
			//if (AMEditor.DLC.drawHeader)
			//{
			//	AMEditor.DLC.DrawHeader ();
			//}
			//#endregion

			GUILayout.BeginHorizontal ();
			#if AM_EDITOR_COMPACT_ON
			GUIContent updateButtonContent = new GUIContent (updateButtonTexture, AMEditorSystem._ContentButtonUpdateListHelp);
			GUIContent logoutButtonContent = new GUIContent (logoutButtonTexture, AMEditorSystem.ContentPlugin._LogoutHelp);
			#else
			GUIContent updateButtonContent = new GUIContent (AMEditorSystem._ContentButtonUpdateList, "");
			GUIContent logoutButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._Logout, "");
			#endif
			if (GUILayout.Button (updateButtonContent, new GUILayoutOption[] {GUILayout.Height (buttonHeight), GUILayout.Width (wideButtonWidth)}))
			{
				ShowProgressBarLoop (AMEditorSystem.ContentProgressBar._UpdateList, AMEditorSystem.ContentProgressBar._Update, 0.1f, 0.5f);
				MergeAllOutdatedPlugin ();

				ShowProgressBarLoop (AMEditorSystem.ContentProgressBar._UpdateList, AMEditorSystem.ContentProgressBar._Update, 0.1f, 1.1f, 0.6f);
				UpdateAllPluginsLists ();

				#if !UNITY_2018_1_OR_NEWER || !UNITY_2018_2_OR_NEWER || !UNITY_2018_3_OR_NEWER || !UNITY_2019_1_OR_NEWER
					EditorUtility.ClearProgressBar ();
				#endif

				CheckPluginsUpdate ();
			}

			GUILayout.Label ("");

			GUILayout.Label (AMEditorSystem.ContentPlugin._CurrentAccount + GitAccount.current.name, currentAccountLabelStyle, 
				new GUILayoutOption[] {GUILayout.ExpandWidth (false), GUILayout.Height (buttonHeight)});

			if (GUILayout.Button (logoutButtonContent, new GUILayoutOption[] {GUILayout.Height (buttonHeight), GUILayout.Width (wideButtonWidth)}))
			{
				if (EditorUtility.DisplayDialog (AMEditorSystem.ContentPlugin._LogoutDialogTitle, AMEditorSystem.ContentPlugin._LogoutDialogMessage, 
				                      AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._Cancel))
				{
					AuthWindow.indexTypeAuth = 1;
					AuthWindow.privateTokenEditBox = GitAccount.current.privateToken;
					currentWindow = WindowType.Authorization;
				}
			}

			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
			#endregion

			scroll = GUILayout.BeginScrollView (scroll);
			DrawGUIInstalledPlugins ();
			DrawGUIMandatoryPlugins ();
			DrawGUIOtherPlugins ();
			DrawExternalPluginGUIs ();
			GUILayout.EndScrollView ();
			GUILayout.Label ("");

			//#region DLC Main
			//if (AMEditor.DLC.drawMain)
			//{
			//	AMEditor.DLC.DrawMain ();
			//}
			//#endregion

			//#region DLC Buttom
			//if (AMEditor.DLC.drawButtom)
			//{
			//	AMEditor.DLC.DrawButtom ();
			//}
			//#endregion
			
			#region Plugins List Status Bar
			GUIStyle statusBarItemStyle = new GUIStyle (GUI.skin.box);
			statusBarItemStyle.normal.textColor = uiTextColor;

			GUI.Box (new Rect (-1, Screen.height - 20, 150, 21), AMEditorSystem.ContentAMEditorInfo._Language, statusBarItemStyle);
			GUI.Box (new Rect (148, Screen.height - 20, 150, 21), 
				AMEditorSystem.ContentAMEditorInfo._PluginsTypeLabel + AMEditorSystem.ContentAMEditorInfo._PluginsType, statusBarItemStyle);
			GUI.Box (new Rect (297, Screen.height - 20, 210, 21), 
				AMEditorSystem.ContentAMEditorInfo._ViewTypeLabel + AMEditorSystem.ContentAMEditorInfo._ViewType + AMEditorSystem.ContentAMEditorInfo._CompactViewType, statusBarItemStyle);
			GUI.Box (new Rect (506, Screen.height - 20, Screen.width - 532, 21), "");
			GUI.Box (new Rect (Screen.width - 27, Screen.height - 20, 28, 21), new GUIContent (makeAllGoodStatusImage, makeAllGoodButtonLabel));
			#endregion
		}

		public void UpdatePluginsGUI ()
		{
			UpdateInstalledPluginsGUI ();
			UpdateMandatoryPluginsGUI ();
			UpdateOtherPluginsGUI ();
			UpdateExternalPluginsGUI ();
			
			SaveEditorState ();	
		}
		
		void GetCurrentListPlugins ()
		{
			installedPlugins = new List<Plugin> ();
			
			if (!AMEditorFileStorage.FileExist (AMEDITOR_CONFIG_FILENAME)) 
			{
				File.WriteAllText (AMEDITOR_CONFIG_FILENAME, "[]");
			}
			
			ArrayList list = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AMEDITOR_CONFIG_FILENAME)) as ArrayList;
			
			foreach (var item in list) 
			{
				installedPlugins.Add (new Plugin (item as Hashtable));
			}
			RemoveIncorrectPlugins ();
		}
		
		void GetCurrentActualPlugins ()
		{
			actualPlugins = new List<ActualPlugin> ();
			//download local repo
			
			if (!AMEditorFileStorage.FileExist (LocalRepositoryAPI.pathToRepository + ACTUAL_VERSION_PLUGINS_FILENAME)) 
			{
				var tempStr = AMEditorGit.RequestGet (LINK_PUBLIC_REPO);
				File.WriteAllText (LocalRepositoryAPI.pathToRepository + ACTUAL_VERSION_PLUGINS_FILENAME, tempStr);
			}
			
			ArrayList listActual = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (LocalRepositoryAPI.pathToRepository + ACTUAL_VERSION_PLUGINS_FILENAME)) as ArrayList;
			if (listActual != null)
			{
				UpdateActualPluginsList (listActual);
			}
			else
			{
				if (!firstStart)
					AMEditorPopupErrorWindow.ShowErrorPopup ("311", AMEditorSystem.ContentError._311);
			}
			
			externalPlugins = new List<ExternalPlugin> ();
			//download local repo
			
			if (!AMEditorFileStorage.FileExist (LocalRepositoryAPI.pathToRepository + EXTERNAL_PLUGINS_FILENAME)) 
			{
				try 
				{
					var tempStr = AMEditorGit.RequestGet (LINK_PUBLIC_REPO_EXTERNAL_PLUGIN);
					File.WriteAllText (LocalRepositoryAPI.pathToRepository + EXTERNAL_PLUGINS_FILENAME, tempStr);
				}
				catch (Exception) 
				{	
					AMEditorPopupErrorWindow.ShowErrorPopup ("311", AMEditorSystem.ContentError._311);
				}
			}
			
			ArrayList listExtern = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (LocalRepositoryAPI.pathToRepository + EXTERNAL_PLUGINS_FILENAME)) as ArrayList;
			if (listExtern != null)
			{
				foreach (var item in listExtern) 
				{
					externalPlugins.Add (new ExternalPlugin (item as Hashtable));
				}
			}
		}
		
		static void UpdateActualPluginsList (ArrayList pluginsList)
		{
			var tempActualPlugins = new List<ActualPlugin> ();
			if (pluginsBuildtypes == null)
				pluginsBuildtypes = new List<string> ();
			
			foreach (var item in pluginsList) 
			{
				var newVersionPlugin = new ActualPlugin (item as Hashtable);
				try
				{
					if (newVersionPlugin.buildTypes != null && newVersionPlugin.buildTypes.Count > 0 && 
						!pluginsBuildtypes.Contains (newVersionPlugin.buildTypes[0]))
					{
						pluginsBuildtypes.Add (newVersionPlugin.buildTypes[0]);
					}
					else
					{
						if (!pluginsBuildtypes.Contains ("standard"))
							pluginsBuildtypes.Add ("standard");
					}
				}
				catch (Exception)
				{
					if (!pluginsBuildtypes.Contains ("standard"))
						pluginsBuildtypes.Add ("standard");
				}
				tempActualPlugins.Add (newVersionPlugin);
			}
			
			foreach (var item in pluginsList)
			{
				var newVersionPlugin = new ActualPlugin (item as Hashtable);
				
				for (int i = 0; i < tempActualPlugins.Count; i++)
				{
					if (tempActualPlugins[i].name.Equals (newVersionPlugin.name) 
					  && !tempActualPlugins[i].version.Equals (newVersionPlugin.version) 
					  && !tempActualPlugins[i].permitted.Contains (newVersionPlugin.version)
					  && tempActualPlugins[i].buildTypes[0].Equals (newVersionPlugin.buildTypes[0]))
					{
						tempActualPlugins[i].permitted.Add (newVersionPlugin.version);
						tempActualPlugins[i].permitted.Sort ();
					}
				}
			}
			actualPlugins = tempActualPlugins;
			
			for (int i = 0; i < tempActualPlugins.Count; i++)
			{
				var item = tempActualPlugins[i];
				var duplicatePlugins = 
					 (from p in tempActualPlugins
					 where (p.name == item.name && p.buildTypes.Count > 0 && item.buildTypes.Count > 0 && p.buildTypes[0] == item.buildTypes[0])
					 select p).ToArray ();
				
				if (duplicatePlugins.Length > 1)
				{
					actualPlugins.RemoveRange (actualPlugins.IndexOf (duplicatePlugins[0]) + 1, duplicatePlugins.Length-1);
				}
			}
		}
		
		void UpdateInstalledPluginsGUI ()
		{
			if (installedPluginsGUIList == null)
				installedPluginsGUIList = new List<PluginGUI> ();
			
			AMEditorConflictAPI.GetConflict (installedPlugins);
			#if !AM_EDITOR_VIEW_TYPE_EXTENDED
			var ccIndex = installedPlugins.FindIndex ((p) => {return p.name == "Custom Code";});
			var guiCCIndex = installedPluginsGUIList.FindIndex ((p) => {return p.name == "Custom Code";});

			List<Depend> ccDepends = new List <Depend> ();
			if (ccIndex != -1)
				ccDepends = installedPlugins [ccIndex].depends;
			#endif
			foreach (var item in installedPlugins) 
			{
				var newitem = new PluginGUI ();
				var actual = 
					 (from v in actualPlugins
					 where v.name == item.name
					 select v).ToArray ();
				
				var installPluginPermitted = new List<string> ();
				for (int i = 0; i < actual.Length; i++)
				{
					for (int j = 0; j < actual[i].permitted.Count; j++)
					{
						if (!installPluginPermitted.Contains (actual[i].permitted[j]))
							installPluginPermitted.Add (actual[i].permitted[j]);
					}
				}
				installPluginPermitted.Sort ();
				#if !AM_EDITOR_VIEW_TYPE_EXTENDED
				var depIndex = ccDepends.FindIndex ((p) => {return p.name == item.name && p.version == item.version;});
				#endif
				newitem.error = new PluginGUI.Error ();
				if ((actual == null) || (actual.Length == 0))
				{
					newitem.actualVersion = "?";
					if (item.name != GetActualNamePlugin (item.name))
					{
						newitem.error.needUpdate = true;
					}
				}
				else
				{
					var actualForBuildType = 
						 (from v in actual
						 where (v.buildTypes != null && v.buildTypes.Count > 0 && v.buildTypes[0] == selectedBuildType || 
						    v.buildTypes == null || v.buildTypes.Count == 0)
						 select v).ToArray ();
					if (actualForBuildType.Length > 0)
					{
						newitem.actualVersion = actualForBuildType[0].version;
					}
					else
					{
						newitem.actualVersion = "?";
					}
					newitem.buildTypes = item.buildTypes;
					
					if (item.buildTypes.Count > 0 && !item.buildTypes[0].Equals (selectedBuildType))
					{
						newitem.error.versionNotForBuildType = true;
					}
					else if (!item.EqualsVersion (newitem.actualVersion))
					{
						newitem.error.noActualVersion = true;
					}
					//TODO: install mark for custom code
					for (int i = 0; i < actual.Length; i++)
					{
						actual[i].installed = true;
					}
				}
				newitem.currentVersion = item.version;
				newitem.permittedVersions = installPluginPermitted;
				newitem.error.conflict = item.errors.conflict;

				#if !AM_EDITOR_VIEW_TYPE_EXTENDED
				if (newitem.error.conflict)
				{
					if (depIndex != -1 && guiCCIndex != -1)
					{
						try
						{
							if (!installedPluginsGUIList[guiCCIndex].error.message.Contains (AMEditorSystem.ContentProblems._CustomCodeDependConflict + item.name))
							{
								installedPluginsGUIList[guiCCIndex].error.message += AMEditorSystem.ContentProblems._CustomCodeDependConflict + item.name + Environment.NewLine;
							}
						}
						catch (Exception)
						{}
					}
				}
				else
				{
					try
					{
						if (installedPluginsGUIList[guiCCIndex].error.message.Contains (AMEditorSystem.ContentProblems._CustomCodeDependConflict + item.name))
						{
							installedPluginsGUIList[guiCCIndex].error.message = installedPluginsGUIList[guiCCIndex].error.message.Replace (AMEditorSystem.ContentProblems._CustomCodeDependConflict + item.name + Environment.NewLine, string.Empty);
						}
					}
					catch (Exception)
					{}
				}
				#endif
				newitem.selectedVersion = newitem.currentVersion;
				
				newitem.name = item.name;
				newitem.url = item.urlMasterBranch;
				newitem.displayType = item.displayType;
				newitem.files = new List<PluginGUI.File> ();
				
				foreach (var f in item.files) 
				{
					var file = new PluginGUI.File (f.path);
					
					var index = f.parameters.FindIndex ((p) => {return p == Plugin.FileParameter.outdated;});

					if (index == -1)
					{
						if (!f.path.Contains (".prefab") && !f.path.Contains (".unity") && !f.path.Contains ("plugin_config.json"))
						{
							if (!AMEditorFileStorage.FileExist (f.path))
							{
								file.status = PluginGUI.File.StatusFile.missing;
								if (f.parameters.FindIndex ((type) => {return Plugin.FileParameter.example == type;}) == -1)
								{
									newitem.error.missingFiles = true;
									#if !AM_EDITOR_VIEW_TYPE_EXTENDED
									if (depIndex != -1 && guiCCIndex != -1)
									{
										try
										{
											if (!installedPluginsGUIList [guiCCIndex].error.message.Contains (AMEditorSystem.ContentProblems._CustomCodeDependMissingFiles + item.name))
											{
												installedPluginsGUIList [guiCCIndex].error.message += AMEditorSystem.ContentProblems._CustomCodeDependMissingFiles + item.name + Environment.NewLine;
											}
										}
										catch (Exception)
										{}
									}
									#endif
								}
							}
							else
							{
								if (!AMEditorFileStorage.HashFile (f.path).Equals (f.hash) && f.parameters.FindIndex ((type) => {return Plugin.FileParameter.modifiable == type;}) == -1)
								{
									file.status = PluginGUI.File.StatusFile.changed;
									if (f.parameters.FindIndex ((type) => {return Plugin.FileParameter.example == type;}) == -1)
									{
										newitem.error.missingFilesHash = true;
										#if !AM_EDITOR_VIEW_TYPE_EXTENDED
										if (depIndex != -1 && guiCCIndex != -1)
										{
											try
											{
												if (!installedPluginsGUIList[guiCCIndex].error.message.Contains (AMEditorSystem.ContentProblems._CustomCodeDependMissingHash (item.name)))
												{
													installedPluginsGUIList[guiCCIndex].error.message += AMEditorSystem.ContentProblems._CustomCodeDependMissingHash (item.name) + Environment.NewLine;
												}
											}
											catch (Exception)
											{}
										}
										#endif
									}
								}
							}
						}
						if (f.parameters.FindIndex ((type) => {return Plugin.FileParameter.example == type;}) == -1)
							newitem.files.Add (file);
						else
							newitem.examples.Add (file);
					}	
				}
				//check depends
				newitem.error.messageDependPlugins = AMEditorSystem.ContentProblems._Depends + Environment.NewLine;
				
				var checkDependsResult = CheckDepends (item);
				newitem.error.dependPlugins = checkDependsResult.error.dependPlugins;
				newitem.error.messageDependPlugins = checkDependsResult.error.messageDependPlugins;

				var current = 
					 (from p in installedPluginsGUIList
					 where p.name == item.name
					 select p).ToArray ();
				if ((current == null) || (current.Length == 0))
				{
					installedPluginsGUIList.Add (newitem);
				}
				else
				{
					for (int i = 0; i < current.Length; i++) 
					{
						installedPluginsGUIList.Find (p => {
							return p.name == current [i].name;
						}).Update (newitem);
					}
				}
			}

			//remove excess
			for (int i = 0; i < installedPluginsGUIList.Count; i++)
			{
				var current = 
					 (from p in installedPlugins
					 where p.name == installedPluginsGUIList[i].name
					 select p).ToArray ();
				if ((current == null) || (current.Count () == 0))
				{
					installedPluginsGUIList.RemoveAt (i);
					i--;
				}
			}

			var sortList = installedPluginsGUIList.FindAll ((p) => {return p.name != "AM Editor";});
			sortList.Sort ((p1, p2) => {return p1.name.CompareTo (p2.name);});
			var editor = installedPluginsGUIList.Find ((p) => {return p.name == "AM Editor";});
			if (editor != null)	
				sortList.Insert (0, editor);
			
			installedPluginsGUIList = sortList;
		}
		
		void UpdateMandatoryPluginsGUI ()
		{
			mandatoryPluginsGUIList = new List<PluginGUI> ();
			foreach (var item in actualPlugins) 
			{
				if (!item.installed && item.mandatory)
				{
					bool ignore = false;
					foreach (var plugin in installedPlugins) 
					{
						var index = plugin.oldNames.FindIndex (name => {return name == item.name;});
						if (index != -1)
						{
							ignore = true;
							break;
						}
					}
					
					foreach (var plugin in actualPlugins) 
					{
						var index = plugin.oldNames.FindIndex (name => {return name == item.name;});
						if (index != -1)
						{
							ignore = true;
							break;
						}
					}
					
					if (!ignore)
					{
						var newitem = new PluginGUI ();
						
						newitem.actualVersion = item.version;
						newitem.permittedVersions = item.permitted;
						newitem.selectedVersion = newitem.actualVersion;
						newitem.name = item.name;
						newitem.url = item.urlMasterBranch;
						newitem.buildTypes = item.buildTypes;
						newitem.displayType = item.displayType;
						
						if (newitem.buildTypes == null || 
						  newitem.buildTypes.Count < 1 || 
						  (newitem.buildTypes != null && newitem.buildTypes.Count > 0 && newitem.buildTypes[0].Equals (selectedBuildType)))
						{
							#if AM_EDITOR_VIEW_TYPE_EXTENDED
							if (newitem.displayType.Equals ("extended") || newitem.displayType.Equals ("none") || string.IsNullOrEmpty (newitem.displayType))
							{
								mandatoryPluginsGUIList.Add (newitem);
							}
							#else
							if (newitem.displayType.Equals ("minimal") || newitem.displayType.Equals ("none") || string.IsNullOrEmpty (newitem.displayType))
							{
								mandatoryPluginsGUIList.Add (newitem);
							}
							#endif 
						}
					}
				}
			}
			mandatoryPluginsGUIList.Sort ((p1, p2) => {return p1.name.CompareTo (p2.name);});
		}
		
		void UpdateOtherPluginsGUI ()
		{
			otherPluginsGUIList = new List<PluginGUI> ();
			for (int i = 0; i < actualPlugins.Count; i++)
			{
				var item = actualPlugins[i];
				
				if (!item.installed && !item.mandatory)
				{
					bool ignore = false;
					
					foreach (var plugin in installedPlugins) 
					{
						var index = plugin.oldNames.FindIndex (name => {return name == item.name;});
						if (index != -1)
						{
							ignore = true;
							break;
						}
					}
					
					if (!ignore)
					{
						var newitem = new PluginGUI ();
						
						newitem.actualVersion = item.version;
						newitem.permittedVersions = item.permitted;
						newitem.selectedVersion = newitem.actualVersion;
						newitem.name = item.name;
						newitem.url = item.urlMasterBranch;
						newitem.buildTypes = item.buildTypes;
						newitem.displayType = item.displayType;
						
						if (newitem.buildTypes == null || 
						  newitem.buildTypes.Count < 1 || 
						  (newitem.buildTypes != null && newitem.buildTypes.Count > 0 && newitem.buildTypes[0].Equals (selectedBuildType)))
						{
							#if AM_EDITOR_VIEW_TYPE_EXTENDED
							if (newitem.displayType.Equals ("extended") || newitem.displayType.Equals ("none") || string.IsNullOrEmpty (newitem.displayType))
							{
								otherPluginsGUIList.Add (newitem);
							}
							#else
							if (newitem.displayType.Equals ("minimal") || newitem.displayType.Equals ("none") || string.IsNullOrEmpty (newitem.displayType))
							{
								otherPluginsGUIList.Add (newitem);
							}
							#endif
						}
					}
				}
			}
			var sortList = otherPluginsGUIList.FindAll ((p) => {return p.name != "AM Editor";});
			sortList.Sort ((p1, p2) => {return p1.name.CompareTo (p2.name);});
			var editor = otherPluginsGUIList.Find ((p) => {return p.name == "AM Editor";});
			if (editor != null)	
				sortList.Insert (0, editor);

			otherPluginsGUIList = sortList;
		}
		
		void UpdateExternalPluginsGUI ()
		{
			externalPluginsGUIList = new List<ExternalPluginGUI> ();
			foreach (var item in externalPlugins) 
			{
				var newitem = new ExternalPluginGUI ();
				
				newitem.version = item.version;
				newitem.name = item.name;
				newitem.uriDescription = item.uriDescription;
				newitem.uriDownload = item.uriDownload;
				
				externalPluginsGUIList.Add (newitem);
			}
			externalPluginsGUIList.Sort ((p1, p2) => {return p1.name.CompareTo (p2.name);});
		}
		
		void OnDisable ()
		{
			if (amEditorIsBusy)
			{
				if (currentDownloadGit != null)
				{
					currentDownloadGit.StopDownload ();
					cancelPressed = true;
					if (EditorUtility.DisplayDialog (AMEditorSystem.ContentProgressBar._CancelUpdateTitle, AMEditorSystem.ContentProgressBar._CancelUpdateMessage, AMEditorSystem.ContentStandardButton._Ok))
					{
						cancelPressed = false;
						currentWindow = WindowType.ListPlugins;
						UpdateAllPluginsLists ();
					}
				}
				amEditorIsBusy = false;
			}
			SaveEditorState ();
			isInit = false;
		}
		
		void OnFocus ()
		{
			if (launchMode == LaunchMode.UI)
			{
				try 
				{
					//AMEditor.DLC.Init ();

					Hashtable stateConfig = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (FILE_NAME_STATE_EDITOR)) as Hashtable;
					selectedBuildType = (string)stateConfig["selectedBuildType"];
					compilationHandlerName = (string)stateConfig["compilationHandlerName"];

					if (string.IsNullOrEmpty (selectedBuildType))
						selectedBuildType = "standard";
				} 
				catch (Exception) 
				{
					selectedBuildType = "standard";
				}
			}
			
			if (AMEditorFileStorage.FileExist (NEED_DISPLAY_SUCCESS_DIALOG))
			{
				AMEditorFileStorage.RemoveFile (NEED_DISPLAY_SUCCESS_DIALOG);
				ShowDisplayDialog ();
			}
			
			if (!isDownloadPlugin)
			{
				try 
				{
					if (currentWindow == WindowType.Wait)
						currentWindow = WindowType.ListPlugins;
					EditorUtility.ClearProgressBar ();
				}
				catch (Exception) 
				{}
			}
			if (!isInit && !isDownloadPlugin)
			{
				isInit = true;
				instance = this;
				if (!authError)
					CheckRCMode ();
				InitAccount ();
				UpdateAllPluginsLists ();
				
				UpdatePluginsGUI ();
				
				if (!isDownloadPlugin)
				{
					try 
					{
						if (currentWindow == WindowType.Wait)
							currentWindow = WindowType.ListPlugins;
						EditorUtility.ClearProgressBar ();
					}
					catch (Exception) 
					{}
				}
			}
			
			LoadEditorState ();
		}
		
		void OnLostFocus ()
		{
			SaveEditorState ();
		}

		void LoadEditorState ()
		{
			if (!AMEditorFileStorage.FileExist (FILE_NAME_STATE_EDITOR)) 
			{
				File.WriteAllText (FILE_NAME_STATE_EDITOR, "{}");
			}

			Hashtable am_editor_state = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (FILE_NAME_STATE_EDITOR)) as Hashtable;

			var listInstallPluginGUIs = am_editor_state["listInstallPluginGUIs"] as ArrayList;

			if (listInstallPluginGUIs != null)
			{
				installedPluginsGUIList = new List<PluginGUI> ();
				foreach (var item in listInstallPluginGUIs) 
				{
					installedPluginsGUIList.Add (new PluginGUI (item as Hashtable));
				}
			}

			var listMandatoryPluginGUIs = am_editor_state["listMandatoryPluginGUIs"] as ArrayList;

			if (listMandatoryPluginGUIs != null)
			{
				mandatoryPluginsGUIList = new List<PluginGUI> ();
				foreach (var item in listMandatoryPluginGUIs) 
				{
					mandatoryPluginsGUIList.Add (new PluginGUI (item as Hashtable));
				}
			}

			var listOtherPluginGUIs = am_editor_state["listOtherPluginGUIs"] as ArrayList;

			if (listOtherPluginGUIs != null)
			{
				otherPluginsGUIList = new List<PluginGUI> ();
				foreach (var item in listOtherPluginGUIs) 
				{
					otherPluginsGUIList.Add (new PluginGUI (item as Hashtable));
				}
			}

			var buildType = (string)am_editor_state["selectedBuildType"];

			if (buildType != null && buildType != string.Empty)
			{
				selectedBuildType = buildType;
			}
			else 
			{
				selectedBuildType = "standard";
			}

			var recompileHandler = (string)am_editor_state["compilationHandlerName"];

			if (recompileHandler != null && recompileHandler != string.Empty)
			{
				compilationHandlerName = recompileHandler;
			}
			else 
			{
				compilationHandlerName = string.Empty;
			}
		}
		
		void SaveEditorState ()
		{
			if (!AMEditorFileStorage.FileExist (FILE_NAME_STATE_EDITOR))
			{
				File.WriteAllText (FILE_NAME_STATE_EDITOR, "{}");
			}

			Hashtable am_editor_state = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (FILE_NAME_STATE_EDITOR)) as Hashtable;

			var listInstallPluginGUIs = new ArrayList ();
			try
			{
				foreach (var item in installedPluginsGUIList) 
				{
					listInstallPluginGUIs.Add (item.ToHashtable ());
				}
			}
			catch (Exception)
			{}
			
			var listMandatoryPluginGUIs = new ArrayList ();
			try
			{
				foreach (var item in mandatoryPluginsGUIList) 
				{
					listMandatoryPluginGUIs.Add (item.ToHashtable ());
				}
			}
			catch (Exception)
			{}
			
			var listOtherPluginGUIs = new ArrayList ();
			try
			{
				foreach (var item in otherPluginsGUIList) 
				{
					listOtherPluginGUIs.Add (item.ToHashtable ());
				}
			}
			catch (Exception)
			{}
			
			var result = new Hashtable ();
			
			result.Add ("listInstallPluginGUIs", listInstallPluginGUIs);
			result.Add ("listMandatoryPluginGUIs", listMandatoryPluginGUIs);
			result.Add ("listOtherPluginGUIs", listOtherPluginGUIs);
			result.Add ("selectedBuildType", selectedBuildType);
			result.Add ("compilationHandlerName", compilationHandlerName);

			var windowPosition = new Hashtable ();
			try
			{
				if (this.position.height >= 500 || !am_editor_state.ContainsKey ("windowPosition"))
				{
					windowPosition.Add ("x", this.position.x);
					windowPosition.Add ("y", this.position.y);
					windowPosition.Add ("w", this.position.width);
					windowPosition.Add ("h", this.position.height);
				}
				else
					windowPosition = am_editor_state["windowPosition"] as Hashtable;
			}
			catch (Exception)
			{}
			result.Add ("windowPosition", windowPosition);

			string resultString = AMEditorJSON.JsonEncode (result);
			File.WriteAllText (FILE_NAME_STATE_EDITOR, AMEditorJSON.FormatJson (resultString));
		}
		
		struct PluginVersion
		{
			public Version version;
			public string modifier;
			public int modifierNumber;
		}
		
		PluginGUI CheckDepends (Plugin source)
		{
			PluginGUI result = new PluginGUI ();
			foreach (var dep in source.depends) 
			{
				var istalledDepend = 
					 (from p in installedPluginsGUIList
					 where p.name == dep.name
					 select p).ToArray ();
				if (istalledDepend == null || istalledDepend.Length == 0)
				{
					result.error.dependPlugins = true;
					result.error.messageDependPlugins += dep.name + " " + dep.version + Environment.NewLine;
				}
				else if (!istalledDepend[0].currentVersion.Equals (dep.version))
				{
					if (dep.mod.Equals ("="))
					{
						result.error.dependPlugins = true;
						result.error.messageDependPlugins += dep.name + " " + dep.version + Environment.NewLine;
						continue;
					}
					
					PluginVersion installedVersion = new PluginVersion ();
					PluginVersion neededVersion = new PluginVersion ();
					
					if (istalledDepend[0].currentVersion.Contains ("-"))
					{
						installedVersion.version = new Version (istalledDepend[0].currentVersion.Substring (0, istalledDepend[0].currentVersion.IndexOf ("-")));
						installedVersion.modifier = istalledDepend[0].currentVersion.Substring (istalledDepend[0].currentVersion.IndexOf ("-") + 1);
						
						string modNumberString = System.Text.RegularExpressions.Regex.Match (installedVersion.modifier, @"\d + ").Value;
						if (modNumberString != string.Empty)
						{
							installedVersion.modifier = installedVersion.modifier.Substring (0, installedVersion.modifier.IndexOf (modNumberString));
							installedVersion.modifierNumber = int.Parse (modNumberString);
						}
					}
					else
					{
						installedVersion.version = new Version (istalledDepend[0].currentVersion);
					}
					if (dep.version.Contains ("-"))
					{
						neededVersion.version = new Version (dep.version.Substring (0, dep.version.IndexOf ("-")));
						neededVersion.modifier = dep.version.Substring (dep.version.IndexOf ("-") + 1);
						
						string modNumberString = System.Text.RegularExpressions.Regex.Match (neededVersion.modifier, @"\d + ").Value;
						if (modNumberString != string.Empty)
						{
							neededVersion.modifier = neededVersion.modifier.Substring (0, neededVersion.modifier.IndexOf (modNumberString));
							neededVersion.modifierNumber = int.Parse (modNumberString);
						}
					}
					else if (!string.IsNullOrEmpty (dep.version))
					{
						neededVersion.version = new Version (dep.version);
					}
					else
					{
						neededVersion.version = new Version ();
					}
					
					if (neededVersion.version.Equals (installedVersion.version))
					{
						if (!string.IsNullOrEmpty (installedVersion.modifier))
						{
							if (!string.IsNullOrEmpty (neededVersion.modifier))
							{
								if (installedVersion.modifier.Equals (neededVersion.modifier))
								{
									switch (dep.mod)
									{
									case ">=":
										if (installedVersion.modifierNumber < neededVersion.modifierNumber)
										{
											result.error.dependPlugins = true;
											result.error.messageDependPlugins += dep.name + " " + dep.version + Environment.NewLine;
										}
										break;
									case ">":
										if (! (installedVersion.modifierNumber > neededVersion.modifierNumber))
										{
											result.error.dependPlugins = true;
											result.error.messageDependPlugins += dep.name + " " + dep.version + Environment.NewLine;
										}
										break;
									default:
										break;
									}
								}
								else
								{
									if (installedVersion.modifier != neededVersion.modifier)
									{
										result.error.dependPlugins = true;
										result.error.messageDependPlugins += dep.name + " " + dep.version + Environment.NewLine;
									}
								}
							}
						}
						else
						{
							result.error.dependPlugins = true;
							result.error.messageDependPlugins += dep.name + " " + dep.version + Environment.NewLine;
						}
					}
					else if (installedVersion.version < neededVersion.version)
					{
						result.error.dependPlugins = true;
						result.error.messageDependPlugins += dep.name + " " + dep.version + Environment.NewLine;
					}
				}
			}
			return result;
		}

		void DrawGUIInstalledPlugins ()
		{
			if (installedPluginsGUIList != null && installedPluginsGUIList.Count > 0) 
			{
				#region Installed Plugins Header
				GUILayout.Label ("", GUILayout.Height (1));
				GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect ().y - 4, Screen.width + 2, 25), "", pluginsHeaderBackStyle);

				GUILayout.BeginHorizontal ();
				GUILayout.Label (new GUIContent (AMEditorSystem.ContentPlugin._ConnectedPluginsTitle, AMEditorSystem.ContentPlugin._ConnectedPluginsTitleHelp), GUILayout.Width (190));
				GUILayout.Label ("");
				GUILayout.Label (new GUIContent (AMEditorSystem.ContentPlugin._PluginsBuildTypes, AMEditorSystem.ContentPlugin._PluginsBuildTypesHelp), new GUILayoutOption[] {GUILayout.ExpandWidth (false), GUILayout.Height (20)});				
				try
				{
					int selectedIndex = pluginsBuildtypes.IndexOf (selectedBuildType);
					selectedBuildTypeIndex = selectedIndex;

					GUILayout.Label ("", GUILayout.Width (120));
					GUI.enabled = (pluginsBuildtypes.Count > 1);
					selectedIndex = EditorGUI.Popup (new Rect ( GUILayoutUtility.GetLastRect ().x, GUILayoutUtility.GetLastRect ().y - 2, GUILayoutUtility.GetLastRect ().width, buttonHeight), 
						selectedBuildTypeIndex, pluginsBuildtypes.ToArray ());
					GUI.enabled = true;
					selectedBuildType = pluginsBuildtypes[selectedIndex];

					if (selectedBuildTypeIndex != selectedIndex)
					{
						UpdateInstalledPluginsGUI ();
						UpdateMandatoryPluginsGUI ();
						UpdateOtherPluginsGUI ();
					}
				}
				catch (Exception)
				{
					this.Focus ();
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();

				#if AM_EDITOR_VIEW_TYPE_EXTENDED
				allInstalledSelected = installedPluginsGUIList.TrueForAll ((p) => {return p.selected;});
				someInstalledSelected = installedPluginsGUIList.Any ((p) => {return p.name != "Custom Code" && p.selected;});
				updatabelInstalledSelected = installedPluginsGUIList.Any ((p) => {return p.selected && (p.error.missingFiles || p.error.missingFilesHash || p.error.dependPlugins);});
				#else
				List<PluginGUI> installedWithoutAMEditor = installedPluginsGUIList.FindAll ((p) => {return p.name != "AM Editor";});
				allInstalledSelected = installedWithoutAMEditor.Count > 0 && installedWithoutAMEditor.TrueForAll ((item) => {return item.selected;});
				someInstalledSelected = installedPluginsGUIList.Any ((p) => {return p.selected;});
				#endif

				GUIStyle toggleAllStyle = new GUIStyle (GUI.skin.toggle);
				toggleAllStyle.normal.background = allInstalledSelected ? new GUIStyle (GUI.skin.toggle).onNormal.background : new GUIStyle (GUI.skin.toggle).normal.background;
				string toggleAllHelp = allInstalledSelected ? AMEditorSystem.ContentStandardButton._NoneHelp : AMEditorSystem.ContentStandardButton._AllHelp;

				if (GUILayout.Button (new GUIContent (AMEditorSystem.ContentPlugin._PluginName, toggleAllHelp), toggleAllStyle, GUILayout.Width (Screen.width / 5 + 8)))
				{
					allInstalledSelected = !allInstalledSelected;
					foreach (var plugin in installedPluginsGUIList) 
					{
#if AM_EDITOR_VIEW_TYPE_EXTENDED
						plugin.selected = allInstalledSelected;
#else
						if (!plugin.name.Equals ("AM Editor"))
							plugin.selected = allInstalledSelected;
#endif
					}
				}

				GUILayout.Label (new GUIContent (AMEditorSystem.ContentPlugin._CurrentVersion, AMEditorSystem.ContentPlugin._CurrentVersionHelp), GUILayout.Width (Screen.width / 8.85f + 30));
				GUILayout.Label (new GUIContent (AMEditorSystem.ContentPlugin._ActualVersion, AMEditorSystem.ContentPlugin._ActualVersionHelp), GUILayout.Width (110));
				GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect ().y + 18, Screen.width + 2, 1), "");
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
				#endregion
				#if !AM_EDITOR_VIEW_TYPE_EXTENDED
				var ccIndex = installedPlugins.FindIndex ((p) => {return p.name == "Custom Code";});
				List<Depend> ccDepends = new List <Depend> ();
				if (ccIndex != -1)
				{
					ccDepends = installedPlugins [ccIndex].depends;
				}
				#endif
				foreach (var item in installedPluginsGUIList)
				{
					string displayType = "";
					GUILayout.BeginHorizontal ();
					#if !AM_EDITOR_VIEW_TYPE_EXTENDED
					displayType = "minimal";

					var depIndex = ccDepends.FindIndex ((p) => {return p.name == item.name && p.version == item.currentVersion;});
					#else
					displayType = "extended";
					#endif
					
					if (item.displayType.Equals (displayType) || item.displayType.Equals ("none") || string.IsNullOrEmpty (item.displayType))
					{
						GUILayout.Space (4);
						Rect selectorRect = GUILayoutUtility.GetLastRect ();
						GUIStyle selectedPluginToggleStyle = new GUIStyle (GUI.skin.toggle);
						selectedPluginToggleStyle.onActive.textColor = item.selected ? Color.white : new GUIStyle (GUI.skin.toggle).normal.textColor;
						selectedPluginToggleStyle.onNormal.textColor = item.selected ? Color.white : new GUIStyle (GUI.skin.toggle).normal.textColor;
						GUIStyle selectedPluginLabelStyle = new GUIStyle (GUI.skin.label);
						selectedPluginLabelStyle.normal.textColor = item.selected ? Color.white : new GUIStyle (GUI.skin.label).normal.textColor;

						// name
						#if !AM_EDITOR_VIEW_TYPE_EXTENDED
						if (item.selected && depIndex == -1)
						{
							GUI.Box (new Rect (-1, selectorRect.y + 1, Screen.width + 2, selectorRect.height + 22), "", pluginSelectorBackStyle);
						}

						if (item.name.Equals ("AM Editor"))
						{
							GUILayout.Space (14);
							GUILayout.Label (item.name, GUILayout.Width (Screen.width / 5-14));
						}
						else
						{
							item.selected = GUILayout.Toggle (item.selected, item.name, selectedPluginToggleStyle, GUILayout.Width (Screen.width / 5f));
						}
						#else
						if (item.selected)
						{
							GUI.Box (new Rect (-1, selectorRect.y + 2, Screen.width + 2, selectorRect.height + 20), "", pluginSelectorBackStyle);
						}
						item.selected = GUILayout.Toggle (item.selected, item.name, selectedPluginToggleStyle, GUILayout.Width (Screen.width / 5f));
						#endif

						// current version
						GUIStyle versionLabelStyle = new GUIStyle (GUI.skin.label);
						versionLabelStyle.normal.textColor = pluginVersionOkColor;

						string versionHelper = string.Empty;
						Texture versionIcon;

						if (item.error.missingFiles  || 
						  item.error.missingFilesHash  || 
						  item.error.needUpdate  || 
						  item.error.dependPlugins  || 
						  item.error.message != string.Empty) 
						{
							versionLabelStyle.normal.textColor = Color.red;
							versionHelper = AMEditorSystem.ContentTexure._NeedUpdateHelp (item.name);
							versionIcon = pluginWarnTexture;
						} 
						else if (item.error.versionNotForBuildType) 
						{
							versionLabelStyle.normal.textColor = Color.red;
							versionHelper = AMEditorSystem.ContentTexure._VersionNotForBuildTypeHelp (item.name);
							versionIcon = pluginErrorTexture;
						} 
						else if (item.error.conflict) 
						{
							versionLabelStyle.normal.textColor = Color.yellow;
							versionHelper = AMEditorSystem.ContentTexure._NeedCheckHelp + item.name;
							versionIcon = pluginErrorTexture;
						} 
						else if (item.error.noActualVersion) 
						{
							versionLabelStyle.normal.textColor = pluginVersionOkColor;
							versionHelper = AMEditorSystem.ContentTexure._NotLatestVersionHelp (item.name);
							versionIcon = pluginWarnTexture;
						} 
						else if (item.actualVersion == "?") 
						{
						#if UNITY_2018_2_OR_NEWER || UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER
                            versionIcon = PluginErrorTexture;
						#else
							versionIcon = new Texture ();
						#endif
						}
						else
						{
							versionLabelStyle.normal.textColor = pluginVersionOkColor;
							versionHelper = AMEditorSystem.ContentTexure._OkHelp (item.name);
							versionIcon = pluginOkTexture;
						}

						if (!item.selectedVersion.Equals (item.currentVersion) || item.actualVersion == "?")
						{
							versionLabelStyle.normal.textColor = new GUIStyle (GUI.skin.label).normal.textColor;
							versionHelper = AMEditorSystem.ContentTexure._SelecdtedAnotherVersionHelp (item.name);
						}
						try
						{
							#if AM_EDITOR_VIEW_TYPE_EXTENDED
							if (item.permittedVersions.Count > 1)
							{
								int selectedVersionIndex = item.permittedVersions.IndexOf (item.selectedVersion);
								if (selectedVersionIndex < 0)
									selectedVersionIndex = item.permittedVersions.IndexOf (item.currentVersion);
								selectedVersionIndex = EditorGUILayout.Popup (selectedVersionIndex, item.permittedVersions.ToArray (), versionSelectorStyle, new GUILayoutOption[] {GUILayout.Width (10)});
								item.selectedVersion = item.permittedVersions[selectedVersionIndex];
								GUILayout.Label (new GUIContent (versionIcon, versionHelper), GUILayout.Width (20));
								GUILayout.Label (new GUIContent (item.selectedVersion, item.selectedVersion), versionLabelStyle, GUILayout.Width (Screen.width / 8.85f));
							}
							else
							{
							#endif
								GUILayout.Space (14);
								GUILayout.Label (new GUIContent (versionIcon, versionHelper), GUILayout.Width (20));
								GUILayout.Label (new GUIContent (item.currentVersion, item.currentVersion), versionLabelStyle, GUILayout.Width (Screen.width / 8.85f));
							#if AM_EDITOR_VIEW_TYPE_EXTENDED
							}
							#endif
						}
						catch (Exception)
						{
							this.Focus ();
						}
						// actual version
						GUILayout.Label (new GUIContent (item.actualVersion, item.actualVersion), selectedPluginLabelStyle, GUILayout.Width (Screen.width / 8.85f));

						if (item.examples != null && item.examples.Count > 0)
						{
#if AM_EDITOR_COMPACT_ON
							string infoText = AMEditorSystem.ContentPlugin._ExamplesInfoHelp ("\"E\"");
#else
							string infoText = AMEditorSystem.ContentPlugin._ExamplesInfoHelp ("\"" + AMEditorSystem._ContentExamples + "\"");
#endif
							if (GUILayout.Button (new GUIContent (infoTexture, infoText), new GUIStyle (GUI.skin.label), new GUILayoutOption[] {GUILayout.Height (20), GUILayout.Width (30)}))
							{
								GUI.FocusControl (item.name + "_examples_button");
							}
							GUILayout.Label ("");
						}
						else
							GUILayout.Label ("");
						
						// buttons:
						#if AM_EDITOR_COMPACT_ON
							#if AM_EDITOR_VIEW_TYPE_EXTENDED
								GUIContent downloadButtonContent = new GUIContent (downloadButtonTexture, AMEditorSystem.ContentPlugin._DownloadHelp + " " + item.name);
								GUIContent listButtonContent = new GUIContent (listButtonTexture, AMEditorSystem._ContentListOfFilesHelp + " " + item.name);
							#else
								GUIContent includedPluginsButtonContent = new GUIContent (listButtonTexture, 
									AMEditorSystem._ContentIncludedPluginsHelp + " " + item.name + " " + item.currentVersion);
							#endif
								GUIContent exampleButtonContent = new GUIContent (exampleButtonTexture, AMEditorSystem._ContentExamplesHelp + " " + item.name);
								GUIContent gitlabButtonContent = new GUIContent (gitlabButtonTexture, 
									AMEditorSystem.ContentPlugin._ManualLinkHelp (item.name));
								GUIContent deleteButtonContent = new GUIContent (deleteButtonTexture, AMEditorSystem.ContentStandardButton._DeleteHelp + " " + item.name);
						#else
							#if AM_EDITOR_VIEW_TYPE_EXTENDED
								GUIContent downloadButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._Download, AMEditorSystem.ContentPlugin._DownloadHelp + " " + item.name);
								GUIContent listButtonContent = new GUIContent (AMEditorSystem._ContentListOfFiles, AMEditorSystem._ContentListOfFilesHelp + " " + item.name);
							#else
								GUIContent includedPluginsButtonContent = new GUIContent (AMEditorSystem._ContentIncludedPlugins, 
									AMEditorSystem._ContentIncludedPluginsHelp + " " + item.name + " " + item.currentVersion);
							#endif
								GUIContent exampleButtonContent = new GUIContent (AMEditorSystem._ContentExamples, AMEditorSystem._ContentExamplesHelp + " " + item.name);
								GUIContent gitlabButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._ManualLink, 
									AMEditorSystem.ContentPlugin._ManualLinkHelp (item.name));
								GUIContent deleteButtonContent = new GUIContent (AMEditorSystem.ContentStandardButton._Delete, AMEditorSystem.ContentStandardButton._DeleteHelp + " " + item.name);
						#endif
						GUIStyle pressedListButtonStyle = new GUIStyle (GUI.skin.button);
						GUIStyle pressedExamplesButtonStyle = new GUIStyle (GUI.skin.button);
						pressedListButtonStyle.normal.background = item.isExpandContent ? GUI.skin.button.active.background : GUI.skin.button.normal.background;
						pressedExamplesButtonStyle.normal.background = item.showExamples ? GUI.skin.button.active.background : GUI.skin.button.normal.background;
						
						#if AM_EDITOR_VIEW_TYPE_EXTENDED
						/// dowload button
						if ((!item.error.missingFiles && !item.error.missingFilesHash && !item.error.needUpdate && !item.error.dependPlugins && item.selectedVersion.Equals (item.currentVersion)))
						{
							GUI.enabled = false;
						}

						if (GUILayout.Button (downloadButtonContent, new GUILayoutOption[] {GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
						{
							if (item.selectedVersion != item.currentVersion)
							{
								AddPluginInQueue (item.name, item.selectedVersion);
							}
							else if ((!item.error.missingFiles && !item.error.missingFilesHash && !item.error.needUpdate && item.error.dependPlugins))
							{
								var curPlugin =
									 (from p in installedPlugins
									 where p.name == item.name
									 select p).ToArray ();
								if ((curPlugin != null) && (curPlugin.Length > 0))
								{
									foreach (var dep in curPlugin[0].depends)
									{
										var plugin =
											 (from p in installedPluginsGUIList
											 where p.name == dep.name
											 select p).ToArray ();
										if ((plugin == null) || (plugin.Length == 0) || !plugin[0].currentVersion.Equals (dep.version))//!curPlugin[0].EqualsVersion (dep.version))
										{
											int index = actualPlugins.FindIndex ((p) => {return p.name == dep.name && (p.buildTypes == null || p.buildTypes.Count == 0 || 
																							 (p.buildTypes != null && p.buildTypes.Count != 0 && p.buildTypes[0] == selectedBuildType));});
											if (index != -1)
											{
												if (!actualPlugins[index].version.Equals (dep.version))
												{
													switch (dep.mod)
													{
													case "=":
														if (EditorUtility.DisplayDialog (AMEditorSystem.ContentProblems._ErrorDependVersion, 
															dep.name + " " + dep.version + AMEditorSystem.ContentProblems._ErrorDependVersionNotSupported, 
															AMEditorSystem._ContentSupport, AMEditorSystem._ContentClose))
														{
															HelpAPI.TypeAbout supportLink = HelpAPI.TypeAbout.SkypeSupport;
															HelpAPI.SearchLink (supportLink);
														}
														break;
													case ">=":
													case ">":
														if (EditorUtility.DisplayDialog (AMEditorSystem.ContentProblems._ErrorDependVersion, 
															dep.name + " " + dep.version + AMEditorSystem.ContentProblems._ErrorDependVersionIsOld, 
															AMEditorSystem._ContentDownloadActual, AMEditorSystem._ContentClose))
														{
															AddPluginInQueue (dep.name, actualPlugins[index].version);
														}
														break;
													default:
														break;
													}
												}
												else
												{
													AddPluginInQueue (dep.name, dep.version);
												}
											}
										}
									}
								}
							}
							else if (item.error.noActualVersion)
							{
								AddPluginInQueue (item.name, item.actualVersion);
							}
							else
							{
								AddPluginInQueue (item.name, item.selectedVersion);
							}
							DownloadPluginInQueue ();
						}
						GUI.enabled = true;
						#else
						if (item.name.Equals ("Custom Code"))
						{
							/// included plugins button
							if (GUILayout.Button (includedPluginsButtonContent, pressedListButtonStyle, new GUILayoutOption[] {GUILayout.Width (wideButtonWidth), GUILayout.Height (buttonHeight)}))
							{
								item.isExpandContent = !item.isExpandContent;
								item.showExamples = false;
							}
						}
						///examples list button
						GUI.enabled = (item.examples != null && item.examples.Count > 0);
						GUI.SetNextControlName (item.name + "_examples_button");
						if (GUILayout.Button (exampleButtonContent, pressedExamplesButtonStyle, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
						{
							GUI.FocusControl ("");
							item.showExamples = !item.showExamples;
							item.isExpandContent = false;
						}
						GUI.enabled = true;
						#endif
						
						/// gitlab button
						if (GUILayout.Button (gitlabButtonContent, new GUILayoutOption[] {GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
						{
							Application.OpenURL (item.url);
						}
						#if AM_EDITOR_VIEW_TYPE_EXTENDED
							/// files list button
							if (GUILayout.Button (listButtonContent, pressedListButtonStyle, new GUILayoutOption[] {GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
							{
								item.isExpandContent = !item.isExpandContent;
								item.showExamples = false;
							}
							///examples list button
							GUI.enabled = (item.examples != null && item.examples.Count > 0);
							GUI.SetNextControlName (item.name + "_examples_button");
							if (GUILayout.Button (exampleButtonContent, 	pressedExamplesButtonStyle, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
							{
								GUI.FocusControl ("");
								item.showExamples = !item.showExamples;
								item.isExpandContent = false;
							}
							GUI.enabled = true;
						#else
							GUI.enabled = !item.name.Equals ("AM Editor");
						#endif
						/// delete button
						if (GUILayout.Button (deleteButtonContent, new GUILayoutOption[] {GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
						{
							if (EditorUtility.DisplayDialog (AMEditorSystem._ContentTitleDeletePlugin, AMEditorSystem._ContentQuestionDeletePlugin + item.name + " ?", 
							                      AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._Cancel))
							{
								DeletePlugin (item.name);
							}
						}
						GUI.enabled = true;
					}
					GUILayout.EndHorizontal ();

					if (item.showExamples)
					{
						foreach (var e in item.examples)
						{
							GUIStyle textStyle = new GUIStyle ();
						#if AM_EDITOR_COMPACT_ON
							GUIContent downloadButtonContent = new GUIContent (downloadButtonTexture, AMEditorSystem.ContentPlugin._Download);
							GUIContent updateButtonContent = new GUIContent (updateButtonTexture, AMEditorSystem.ContentPlugin._Update);
							GUIContent openFileButtonContent = new GUIContent (openButtonTexture, AMEditorSystem._ContentOpenFileHelp);
							GUIContent deleteFileButtonContent = new GUIContent (deleteButtonTexture, AMEditorSystem.ContentPlugin._Delete);
						#else
							GUIContent downloadButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._Download, "");
							GUIContent updateButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._Update, "");
							GUIContent openFileButtonContent = new GUIContent (AMEditorSystem._ContentOpenFile, "");
							GUIContent deleteFileButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._Delete, "");
						#endif
							GUILayout.BeginHorizontal ();

							GUILayout.Label ("\t\t\t", GUILayout.Width (30));

							switch (e.status)
							{
							case PluginGUI.File.StatusFile.good:
								GUILayout.Label (e.path);
								break;
							case PluginGUI.File.StatusFile.changed:
								textStyle.normal.textColor = Color.yellow;
								GUILayout.Label (e.path, textStyle);
								break;
							case PluginGUI.File.StatusFile.missing:
								GUI.enabled = false;
								GUI.enabled = true;
								textStyle.normal.textColor = Color.red;
								GUILayout.Label (e.path, textStyle);
								break;
							default:
								break;
							}

							GUIContent downloadExampleContent = (e.status == PluginGUI.File.StatusFile.changed) ? updateButtonContent : downloadButtonContent;

							GUI.enabled = (e.status == PluginGUI.File.StatusFile.missing || e.status == PluginGUI.File.StatusFile.changed);
							if (GUILayout.Button (downloadExampleContent, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
							{
								string localRepoExamplePath = LocalRepositoryAPI.pathToRepository + LOCAL_REPOSITORY + System.IO.Path.DirectorySeparatorChar.ToString () + item.name + " " + item.currentVersion + System.IO.Path.DirectorySeparatorChar.ToString () + item.name.ToLower ().Replace (" ", "-") + ".git/" + e.path;
								if (AMEditorFileStorage.FileExist (localRepoExamplePath))
								{
									AMEditorFileStorage.CopyFile (localRepoExamplePath, e.path, true);
									AssetDatabase.Refresh ();
								}
								else
								{
									var actuals = 
										 (from p in actualPlugins
										where p.name == item.name
										select p).ToList ();
									if (actuals != null && actuals.Count > 0)
									{
										var actualsForBuildType = 
											 (from p in actuals
											where (p.buildTypes != null && p.buildTypes.Count > 0 && p.buildTypes [0] == selectedBuildType  || 
											p.buildTypes == null || p.buildTypes.Count == 0)
											select p).ToArray ();
										exampleFilePath = e.path;
										DownloadFilesThroughPublicConfig (actualsForBuildType [0]);
									}
								}
							}
							GUI.enabled = true;
							GUI.enabled = (e.status != PluginGUI.File.StatusFile.missing);
							if (GUILayout.Button (openFileButtonContent, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
							{
								EditorUtility.OpenWithDefaultApp (e.path);
							}
							GUI.enabled = true;
							GUI.enabled = (e.status != PluginGUI.File.StatusFile.missing);
							if (GUILayout.Button (deleteFileButtonContent, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
							{
								if (EditorUtility.DisplayDialog (AMEditorSystem.ContentDeleteExample._Title, AMEditorSystem.ContentDeleteExample._Message, AMEditorSystem.ContentStandardButton._Ok, AMEditorSystem.ContentStandardButton._Cancel))
								{
									File.Delete (e.path);
									AssetDatabase.Refresh ();
								}
							}
							GUI.enabled = true;
							#if AM_EDITOR_COMPACT_ON
								GUILayout.Label ("", GUILayout.Width (40));
							#else
								GUILayout.Label ("", GUILayout.Width (128));
							#endif
								GUILayout.EndHorizontal ();
						}
					}
					#if AM_EDITOR_VIEW_TYPE_EXTENDED
					if (item.isExpandContent)
					{
						foreach (var f in item.files)
						{
							GUIStyle textStyle = new GUIStyle ();
							#if AM_EDITOR_COMPACT_ON
								GUIContent openFileButtonContent = new GUIContent (openButtonTexture, AMEditorSystem._ContentOpenFileHelp);
							#else
								GUIContent openFileButtonContent = new GUIContent (AMEditorSystem._ContentOpenFile, "");
							#endif
							switch (f.status)
							{
							case PluginGUI.File.StatusFile.good:
								GUILayout.BeginHorizontal ();
								if (GUILayout.Button (openFileButtonContent, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
								{
									EditorUtility.OpenWithDefaultApp (f.path);
								}
								GUILayout.Label ("\t\t\t", GUILayout.Width (30));
								GUILayout.Label (f.path);
								GUILayout.EndHorizontal ();
								break;
							case PluginGUI.File.StatusFile.changed:
								GUILayout.BeginHorizontal ();
								if (GUILayout.Button (openFileButtonContent, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)))
								{
									EditorUtility.OpenWithDefaultApp (f.path);
								}
								textStyle.normal.textColor = Color.yellow;
								GUILayout.Label ("\t\t\t", GUILayout.Width (30));
								GUILayout.Label (f.path, textStyle);
								GUILayout.EndHorizontal ();
								break;
							case PluginGUI.File.StatusFile.missing:
								GUILayout.BeginHorizontal ();
								GUI.enabled = false;
								GUILayout.Button (openFileButtonContent, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight));
								GUI.enabled = true;
								textStyle.normal.textColor = Color.red;
								GUILayout.Label ("\t\t\t", GUILayout.Width (30));
								GUILayout.Label (f.path, textStyle);
								GUILayout.EndHorizontal ();
								break;
							default:
								break;
							}
						}
					}
					#else
					if (item.isExpandContent)
					{
						foreach (var ccDepend in installedPlugins[ccIndex].depends)
						{
							GUILayout.BeginHorizontal ();
							GUILayout.Space (18);
							GUILayout.Label (new GUIContent (ccDepend.name, ccDepend.name), GUILayout.Width (Screen.width / 5f + 25));
							GUILayout.Label (new GUIContent (ccDepend.version, ccDepend.version), GUILayout.Width (Screen.width / 8.85f));
							GUILayout.EndHorizontal ();
						}
					}
					if (depIndex == -1)
					{
						#endif
						if (item.error.conflict)
						{
							GUIStyle textStyle = new GUIStyle ();
							textStyle.normal.textColor = Color.red;
							GUILayout.BeginHorizontal ();
							GUILayout.Label (new GUIContent (AMEditorSystem.ContentProblems._ErrorConflictFiles, AMEditorSystem.ContentProblems._ErrorConflictFilesHelp), textStyle, GUILayout.Width (240));
							textStyle.normal.textColor = Color.blue;
							#if AM_EDITOR_VIEW_TYPE_EXTENDED
							if (GUILayout.Button (new GUIContent (fixmeTexture, AMEditorSystem.ContentProblems._FixHelp), textStyle, GUILayout.Width (75)))
							{
								currentWindow = WindowType.FixConfict;

								var list =
									 (from p in installedPlugins
									 where p.name == item.name
									 select p).ToList ();
							
								if ((list != null) && (list.Count > 0))
								{
									AMEditorConflictAPI.GetConflict (list[0]);
									currentPlugin = list[0];
								}
							}
							#endif
							GUILayout.Label ("");
							GUILayout.EndHorizontal ();
						}
						if (item.error.missingFilesHash)
						{
							GUIStyle textStyle = new GUIStyle ();
							textStyle.normal.textColor = Color.red;
							GUILayout.Label (new GUIContent (AMEditorSystem.ContentProblems._ErrorMissingFilesHash, AMEditorSystem.ContentProblems._ErrorMissingFilesHashHelp), textStyle);
						}
						if (item.error.missingFiles)
						{
							GUIStyle textStyle = new GUIStyle ();
							textStyle.normal.textColor = Color.red;
							GUILayout.Label (new GUIContent (AMEditorSystem.ContentProblems._ErrorMissingFiles, AMEditorSystem.ContentProblems._ErrorMissingFilesHelp), textStyle);
						}
						if (item.error.dependPlugins)
						{
							GUIStyle textStyle = new GUIStyle ();
							textStyle.normal.textColor = Color.red;
							GUILayout.Label (item.error.messageDependPlugins, textStyle);
						}
						#if !AM_EDITOR_VIEW_TYPE_EXTENDED
					}
					if (item.name.Equals ("Custom Code") && item.error.message != string.Empty)
					{
						GUIStyle textStyle = new GUIStyle ();
						textStyle.normal.textColor = Color.red;
						GUILayout.Label (item.error.message, textStyle);
					}
					#endif
				}
				
				#region Installed Plugins Buttom
				GUILayout.BeginHorizontal ();
				#if AM_EDITOR_VIEW_TYPE_EXTENDED
				GUI.enabled = updatabelInstalledSelected;
				// download selected button
				#if AM_EDITOR_COMPACT_ON
					if (GUILayout.Button (new GUIContent (AMEditorSystem.ContentPlugin._Selected, downloadButtonTexture, AMEditorSystem.ContentPlugin._DownloadSelectedHelp), 
						new GUILayoutOption[] {GUILayout.Height (buttonHeight), GUILayout.Width (100)}))
					{
				#else
					if (GUILayout.Button (new GUIContent (AMEditorSystem.ContentPlugin._DownloadSelected, AMEditorSystem.ContentPlugin._DownloadSelectedHelp), GUILayout.Width (200)))
					{
				#endif
					foreach (var item in installedPluginsGUIList) 
					{
						if (item.selected && 
						  (item.error.missingFiles || item.error.missingFilesHash || item.error.versionNotForBuildType || 
						 	item.error.needUpdate || item.error.dependPlugins)) // ||  item.error.noActualVersion))
						{
							AddPluginInQueue (item.name, item.selectedVersion);
						}
						item.selected = false;
					}	
					DownloadPluginInQueue ();
				}
				GUI.enabled = true;
				#endif
				GUI.enabled = someInstalledSelected;
				// delete selected button
				#if AM_EDITOR_COMPACT_ON
					if (GUILayout.Button (new GUIContent (AMEditorSystem.ContentPlugin._Selected, deleteButtonTexture, AMEditorSystem.ContentPlugin._DeleteSelectedHelp), 
						new GUILayoutOption[] {GUILayout.Height (buttonHeight), GUILayout.Width (100)}))
					{
				#else
					if (GUILayout.Button (new GUIContent (AMEditorSystem.ContentPlugin._DeleteSelected, AMEditorSystem.ContentPlugin._DeleteSelectedHelp), GUILayout.Width (200)))
					{
				#endif
					if (EditorUtility.DisplayDialog (AMEditorSystem._ContentTitleDeletePlugin, AMEditorSystem._ContentQuestionDeleteSelectedPlugin, 
					                      AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._Cancel))	
					{
						DeleteSelectedPlugins ();
					}
				}
				GUILayout.Label ("", GUILayout.ExpandWidth (true));
				
				GUI.enabled = !IsAllGood ();
				// make all good button
				switch (pluginsStatus)
				{
				case PluginsStatus.AllGood:
					makeAllGoodButtonLabel = AMEditorSystem.ContentMakeAllGoodButton._AllGood;
					makeAllGoodButtonHelp = AMEditorSystem.ContentMakeAllGoodButton._AllGoodHelp;
					makeAllGoodStatusImage = pluginOkTexture;
					break;
				case PluginsStatus.BuildTypeProblems:
					makeAllGoodButtonLabel = AMEditorSystem.ContentMakeAllGoodButton._BuildTypeProblems;
					makeAllGoodButtonHelp = AMEditorSystem.ContentMakeAllGoodButton._BuildTypeProblemsHelp;
					makeAllGoodStatusImage = pluginErrorTexture;
					break;
				case PluginsStatus.MissingFiles:
					makeAllGoodButtonLabel = AMEditorSystem.ContentMakeAllGoodButton._MissingFiles;
					makeAllGoodButtonHelp = AMEditorSystem.ContentMakeAllGoodButton._MissingFilesHelp;
					makeAllGoodStatusImage = pluginErrorTexture;
					break;
				case PluginsStatus.HasConflicts:
					makeAllGoodButtonLabel = AMEditorSystem.ContentMakeAllGoodButton._HasConflicts;
					makeAllGoodButtonHelp = AMEditorSystem.ContentMakeAllGoodButton._HasConflictsHelp;
					makeAllGoodStatusImage = pluginErrorTexture;
					break;
				case PluginsStatus.ChangedFiles:
					makeAllGoodButtonLabel = AMEditorSystem.ContentMakeAllGoodButton._ChangedFiles;
					makeAllGoodButtonHelp = AMEditorSystem.ContentMakeAllGoodButton._ChangedFilesHelp;
					makeAllGoodStatusImage = pluginErrorTexture;
					break;
				case PluginsStatus.NeedDepends:
					makeAllGoodButtonLabel = AMEditorSystem.ContentMakeAllGoodButton._NeedDepends;
					makeAllGoodButtonHelp = AMEditorSystem.ContentMakeAllGoodButton._NeedDependsHelp;
					makeAllGoodStatusImage = pluginErrorTexture;
					break;
				case PluginsStatus.NeedUpdate:
					makeAllGoodButtonLabel = AMEditorSystem.ContentMakeAllGoodButton._NeedUpdate;
					makeAllGoodButtonHelp = AMEditorSystem.ContentMakeAllGoodButton._NeedUpdateHelp;
					makeAllGoodStatusImage = pluginWarnTexture;
					break;
				case PluginsStatus.NeedMandatory:
					makeAllGoodButtonLabel = AMEditorSystem.ContentMakeAllGoodButton._NeedMandatory;
					makeAllGoodButtonHelp = AMEditorSystem.ContentMakeAllGoodButton._NeedMandatoryHelp;
					makeAllGoodStatusImage = pluginWarnTexture;
					break;
				case PluginsStatus.OutdatedPlugins:
					makeAllGoodButtonLabel = AMEditorSystem.ContentMakeAllGoodButton._OutdatedPlugins;
					makeAllGoodButtonHelp = AMEditorSystem.ContentMakeAllGoodButton._OutdatedPluginsHelp;
					makeAllGoodStatusImage = pluginWarnTexture;
					break;
				default:
					break;
				}
				if (GUILayout.Button (new GUIContent (makeAllGoodButtonLabel, makeAllGoodButtonHelp), GUILayout.Width (210)))
				{
					MakeAllGood ();
				}
				GUI.enabled = true;
				GUILayout.EndHorizontal ();
				#endregion
				EditorGUILayout.Space ();
			}
		}

		void DrawGUIMandatoryPlugins ()
		{
			if (mandatoryPluginsGUIList != null && mandatoryPluginsGUIList.Count > 0) 
			{
				#region Mandatory Plugins Header
				GUILayout.Label ("", GUILayout.Height (1));
				float headerY = GUILayoutUtility.GetLastRect ().y;
				GUI.Box (new Rect (-1, headerY, Screen.width + 2, 21), "", pluginsHeaderBackStyle);
				GUI.Box (new Rect (-1, headerY, Screen.width + 2, 1), "");

				GUILayout.BeginHorizontal ();
				GUILayout.Label (new GUIContent (AMEditorSystem.ContentPlugin._MandatoryPluginsTitle, AMEditorSystem.ContentPlugin._MandatoryPluginsTitleHelp), GUILayout.Width (300));
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();

				allMandatorySelected = mandatoryPluginsGUIList.TrueForAll ((p) => {return p.selected;});
				someMandatorySelected = mandatoryPluginsGUIList.Any ((p) => {return p.selected;});

				GUIStyle toggleAllStyle = new GUIStyle (GUI.skin.toggle);
				toggleAllStyle.normal.background = allMandatorySelected ? new GUIStyle (GUI.skin.toggle).onNormal.background : new GUIStyle (GUI.skin.toggle).normal.background;
				string toggleAllHelp = allMandatorySelected ? AMEditorSystem.ContentStandardButton._NoneHelp : AMEditorSystem.ContentStandardButton._AllHelp;

				if (GUILayout.Button (new GUIContent (AMEditorSystem.ContentPlugin._PluginName, toggleAllHelp), toggleAllStyle, GUILayout.Width (Screen.width / 3.19f + 42)))
				{
					allMandatorySelected = !allMandatorySelected;
					foreach (var plugin in mandatoryPluginsGUIList) 
					{
						plugin.selected = allMandatorySelected;
					}
				}

				GUILayout.Label (new GUIContent (AMEditorSystem.ContentPlugin._ActualVersion, AMEditorSystem.ContentPlugin._ActualVersionHelp));
				GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect ().y + 18, Screen.width + 2, 1), "");
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
				#endregion
				
				foreach (var item in mandatoryPluginsGUIList) 
				{
					#if AM_EDITOR_VIEW_TYPE_EXTENDED
					if (item.displayType.Equals ("extended") || item.displayType.Equals ("none") || string.IsNullOrEmpty (item.displayType))
					{
					#else
					if (item.displayType.Equals ("minimal") || item.displayType.Equals ("none") || string.IsNullOrEmpty (item.displayType))
					{
					#endif
						GUILayout.BeginHorizontal ();
							
						GUILayout.Space (4);
						Rect selectorRect = GUILayoutUtility.GetLastRect ();
						GUIStyle selectedPluginToggleStyle = new GUIStyle (GUI.skin.toggle);
						selectedPluginToggleStyle.onActive.textColor = item.selected ? Color.white : new GUIStyle (GUI.skin.toggle).normal.textColor;
						selectedPluginToggleStyle.onNormal.textColor = item.selected ? Color.white : new GUIStyle (GUI.skin.toggle).normal.textColor;
						GUIStyle selectedPluginLabelStyle = new GUIStyle (GUI.skin.label);
						selectedPluginLabelStyle.normal.textColor = item.selected ? Color.white : new GUIStyle (GUI.skin.label).normal.textColor;

						if (item.selected)
						{
							GUI.Box (new Rect (-1, selectorRect.y + 2, Screen.width + 2, selectorRect.height + 20), "", pluginSelectorBackStyle);
						}

						// name
						item.selected = GUILayout.Toggle (item.selected, item.name, selectedPluginToggleStyle, GUILayout.Width (Screen.width / 3.19f + 28));
	
						// actual version
						try
						{
							if (item.permittedVersions.Count > 1)
							{
								int selectedVersionIndex = item.permittedVersions.IndexOf (item.selectedVersion);
								if (selectedVersionIndex < 0)						
									selectedVersionIndex = item.permittedVersions.IndexOf (item.actualVersion);
								selectedVersionIndex = EditorGUILayout.Popup (selectedVersionIndex, item.permittedVersions.ToArray (), versionSelectorStyle, new GUILayoutOption[] {GUILayout.Width (10)});
								item.selectedVersion = item.permittedVersions[selectedVersionIndex];
								GUILayout.Label ("", GUILayout.Width (20));
								GUILayout.Label (new GUIContent (item.selectedVersion, item.selectedVersion), selectedPluginLabelStyle, GUILayout.Width (Screen.width / 8.85f));
							}
							else
							{
								GUILayout.Space (14);
								GUILayout.Label (pluginErrorTexture, GUILayout.Width (20));
								GUILayout.Label (new GUIContent (item.permittedVersions[0], item.permittedVersions[0]), selectedPluginLabelStyle, GUILayout.Width (Screen.width / 8.85f));
							}
						}
						catch (Exception)
						{
							this.Focus ();
						}
						GUILayout.Label ("");

						// buttons
						/// download button
						#if AM_EDITOR_COMPACT_ON
							GUIContent downloadButtonContent = new GUIContent (downloadButtonTexture, AMEditorSystem.ContentPlugin._DownloadHelp + " " + item.name);
						#else
							GUIContent downloadButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._Download, AMEditorSystem.ContentPlugin._DownloadHelp + " " + item.name);
						#endif
						if (GUILayout.Button (downloadButtonContent, new GUILayoutOption[] {GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
						{
							AddPluginInQueue (item.name, item.selectedVersion);
							DownloadPluginInQueue ();
						}
						/// gitlab button
						#if AM_EDITOR_COMPACT_ON
							GUIContent gitlabButtonContent = new GUIContent (gitlabButtonTexture, 
								AMEditorSystem.ContentPlugin._ManualLinkHelp (item.name));
						#else
							GUIContent gitlabButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._ManualLink, 
								AMEditorSystem.ContentPlugin._ManualLinkHelp (item.name));
						#endif
						if (GUILayout.Button (gitlabButtonContent, new GUILayoutOption[] {GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
						{
							Application.OpenURL (item.url);
						}
						#if AM_EDITOR_COMPACT_ON
							GUILayout.Label ("", GUILayout.Width (52));
						#else
							GUILayout.Label ("", GUILayout.Width (248));
						#endif
							GUILayout.EndHorizontal ();
					}
				}
				#region Mandatory Plugins Buttom
				GUILayout.BeginHorizontal ();

				GUI.enabled = someMandatorySelected;
				// download selected button
				#if AM_EDITOR_COMPACT_ON
					if (GUILayout.Button (new GUIContent (AMEditorSystem.ContentPlugin._Selected, downloadButtonTexture, AMEditorSystem.ContentPlugin._DownloadSelectedHelp), 
						new GUILayoutOption[] {GUILayout.Height (buttonHeight), GUILayout.Width (100)}))
					{
				#else
					if (GUILayout.Button (new GUIContent (AMEditorSystem.ContentPlugin._DownloadSelected, AMEditorSystem.ContentPlugin._DownloadSelectedHelp), GUILayout.Width (200)))
					{
				#endif
					foreach (var item in mandatoryPluginsGUIList) 
					{
						if (item.selected)
						{
							AddPluginInQueue (item.name, item.selectedVersion);
						}
						item.selected = false;
					}	
					DownloadPluginInQueue ();
				}
				GUI.enabled = true;

				GUILayout.EndHorizontal ();
				EditorGUILayout.Space ();
				#endregion
				}
			}

			void DrawGUIOtherPlugins ()
			{
				if (otherPluginsGUIList != null && otherPluginsGUIList.Count > 0) 
				{
					#region Other Plugins Header
					GUIStyle pluginsHeaderBackStyle = EditorGUIUtility.isProSkin ? new GUIStyle (GUI.skin.box) : new GUIStyle (GUI.skin.textField);
					GUILayout.Label ("", GUILayout.Height (1));
					float headerY = GUILayoutUtility.GetLastRect ().y;
					GUI.Box (new Rect (-1, headerY, Screen.width + 2, 21), "", pluginsHeaderBackStyle);
					GUI.Box (new Rect (-1, headerY, Screen.width + 2, 1), "");

					GUILayout.BeginHorizontal ();
					GUILayout.Label (new GUIContent (AMEditorSystem.ContentPlugin._UnconnectedPluginsTitle, AMEditorSystem.ContentPlugin._UnconnectedPluginsTitleHelp), GUILayout.Width (300));
					GUILayout.EndHorizontal ();

					GUILayout.BeginHorizontal ();

					allOtherSelected = otherPluginsGUIList.TrueForAll ((p) => {return p.selected;});
					someOtherSelected = otherPluginsGUIList.Any ((p) => {return p.selected;});

					GUIStyle toggleAllStyle = new GUIStyle (GUI.skin.toggle);
					toggleAllStyle.normal.background = allOtherSelected ? new GUIStyle (GUI.skin.toggle).onNormal.background : new GUIStyle (GUI.skin.toggle).normal.background;
					string toggleAllHelp = allOtherSelected ? AMEditorSystem.ContentStandardButton._NoneHelp : AMEditorSystem.ContentStandardButton._AllHelp;

					if (GUILayout.Button (new GUIContent (AMEditorSystem.ContentPlugin._PluginName, toggleAllHelp), toggleAllStyle, GUILayout.Width (Screen.width / 3.19f + 42)))
					{
						allOtherSelected = !allOtherSelected;
						foreach (var plugin in otherPluginsGUIList) 
						{
							plugin.selected = allOtherSelected;
						}
					}

					GUILayout.Label (new GUIContent (AMEditorSystem.ContentPlugin._ActualVersion, AMEditorSystem.ContentPlugin._ActualVersionHelp));
					GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect ().y + 18, Screen.width + 2, 1), "");
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();
					#endregion
					
					foreach (var item in otherPluginsGUIList) 
					{
						#if AM_EDITOR_VIEW_TYPE_EXTENDED
						if (item.displayType.Equals ("extended") || item.displayType.Equals ("none") || string.IsNullOrEmpty (item.displayType))
						{
						#else
						if (item.displayType.Equals ("minimal") || item.displayType.Equals ("none") || string.IsNullOrEmpty (item.displayType))
						{
						#endif
							GUILayout.BeginHorizontal ();

							GUILayout.Space (4);
							Rect selectorRect = GUILayoutUtility.GetLastRect ();
							GUIStyle selectedPluginToggleStyle = new GUIStyle (GUI.skin.toggle);
							selectedPluginToggleStyle.onActive.textColor = item.selected ? Color.white : new GUIStyle (GUI.skin.toggle).normal.textColor;
							selectedPluginToggleStyle.onNormal.textColor = item.selected ? Color.white : new GUIStyle (GUI.skin.toggle).normal.textColor;
							GUIStyle selectedPluginLabelStyle = new GUIStyle (GUI.skin.label);
							selectedPluginLabelStyle.normal.textColor = item.selected ? Color.white : new GUIStyle (GUI.skin.label).normal.textColor;

							if (item.selected)
							{
								GUI.Box (new Rect (-1, selectorRect.y + 2, Screen.width + 2, selectorRect.height + 20), "", pluginSelectorBackStyle);
							}

							// name
							item.selected = GUILayout.Toggle (item.selected, item.name, selectedPluginToggleStyle, GUILayout.Width (Screen.width / 3.19f + 28), GUILayout.Height (19));

							// version
							try
							{
								if (item.permittedVersions.Count > 1)
								{
									int selectedVersionIndex = item.permittedVersions.IndexOf (item.selectedVersion);
									if (selectedVersionIndex < 0)						
										selectedVersionIndex = item.permittedVersions.IndexOf (item.actualVersion);
								selectedVersionIndex = EditorGUILayout.Popup (selectedVersionIndex, item.permittedVersions.ToArray (), versionSelectorStyle, new GUILayoutOption[] {GUILayout.Width (10)});
									item.selectedVersion = item.permittedVersions[selectedVersionIndex];
									GUILayout.Label (new GUIContent (item.selectedVersion, item.selectedVersion), selectedPluginLabelStyle, GUILayout.Width (Screen.width / 8.85f));
								}
								else
								{
									GUILayout.Space (14);
									GUILayout.Label (new GUIContent (item.permittedVersions[0], item.permittedVersions[0]), selectedPluginLabelStyle, GUILayout.Width (Screen.width / 8.85f));
								}
							}
							catch (Exception)
							{
								this.Focus ();
							}
							GUILayout.Label ("");

							// buttons
							/// download button
							#if AM_EDITOR_COMPACT_ON
								GUIContent downloadButtonContent = new GUIContent (downloadButtonTexture, AMEditorSystem.ContentPlugin._DownloadHelp + " " + item.name);
							#else
								GUIContent downloadButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._Download, AMEditorSystem.ContentPlugin._DownloadHelp + " " + item.name);
							#endif
							if (GUILayout.Button (downloadButtonContent, new GUILayoutOption[] {GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
							{
								AddPluginInQueue (item.name, item.selectedVersion);
								DownloadPluginInQueue ();
							}
							/// gitlab button
							#if AM_EDITOR_COMPACT_ON
								GUIContent gitlabButtonContent = new GUIContent (gitlabButtonTexture, 
									AMEditorSystem.ContentPlugin._ManualLinkHelp (item.name));
							#else
								GUIContent gitlabButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._ManualLink, 
									AMEditorSystem.ContentPlugin._ManualLinkHelp (item.name));
							#endif
							if (GUILayout.Button (gitlabButtonContent, new GUILayoutOption[] {GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
							{
								Application.OpenURL (item.url);
							}
							#if AM_EDITOR_COMPACT_ON
								GUILayout.Label ("", GUILayout.Width (52));
							#else
								GUILayout.Label ("", GUILayout.Width (248));
							#endif
							GUILayout.EndHorizontal ();
						}
					}

					GUILayout.BeginHorizontal ();

					GUI.enabled = someOtherSelected;
					// download selected button
					#if AM_EDITOR_COMPACT_ON
						if (GUILayout.Button (new GUIContent (AMEditorSystem.ContentPlugin._Selected, downloadButtonTexture, AMEditorSystem.ContentPlugin._DownloadSelectedHelp), 
							new GUILayoutOption[] {GUILayout.Height (buttonHeight), GUILayout.Width (100)}))
						{
					#else
						if (GUILayout.Button (new GUIContent (AMEditorSystem.ContentPlugin._DownloadSelected, AMEditorSystem.ContentPlugin._DownloadSelectedHelp), GUILayout.Width (200)))
						{
					#endif
						foreach (var item in otherPluginsGUIList) 
						{
							if (item.selected)
							{
								AddPluginInQueue (item.name, item.selectedVersion);
							}
							item.selected = false;
						}
						DownloadPluginInQueue ();
					}
					GUI.enabled = true;

					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();
				}
			}

			string externalSearch = string.Empty;
			string searchPlaceholder = AMEditorSystem.ContentPlugin._SearchPlaceholder;
			void DrawExternalPluginGUIs ()
			{
				GUIStyle searchTextStyle = new GUIStyle (GUI.skin.textField);
				searchTextStyle.fontStyle = (externalSearch == searchPlaceholder) ? FontStyle.Italic : FontStyle.Normal;
				searchTextStyle.normal.textColor = (externalSearch == searchPlaceholder) ? Color.gray : uiTextColor;
				searchTextStyle.focused.textColor = (externalSearch == searchPlaceholder) ? Color.gray : uiTextColor;
					
				if (externalPluginsGUIList != null && externalPluginsGUIList.Count > 0) 
				{
					#region External Plugins Header
					GUILayout.Label ("", GUILayout.Height (1));
					float headerY = GUILayoutUtility.GetLastRect ().y;
					GUI.Box (new Rect (-1, headerY, Screen.width + 2, 21), "", pluginsHeaderBackStyle);
					GUI.Box (new Rect (-1, headerY, Screen.width + 2, 1), "");
						
					GUILayout.BeginHorizontal ();
					GUILayout.Label (new GUIContent (AMEditorSystem.ContentPlugin._ExternalPluginsTitle, AMEditorSystem.ContentPlugin._ExternalPluginsTitleHelp), GUILayout.Width (300));
					GUILayout.Label ("", GUILayout.ExpandWidth (true));
					GUILayout.Label (AMEditorSystem.ContentPlugin._SearchTitle, new GUILayoutOption[] {GUILayout.ExpandWidth (false)});

					GUI.SetNextControlName ("search_field");
					externalSearch = GUILayout.TextField (externalSearch, searchTextStyle, GUILayout.Width (210));
					if (UnityEngine.Event.current.type == EventType.Repaint)
					{
						if (GUI.GetNameOfFocusedControl () == "search_field")
						{
							if (externalSearch == searchPlaceholder) 
								externalSearch = "";
						}
						else
						{
							if (externalSearch == "") 
								externalSearch = searchPlaceholder;
						}
					}
					GUILayout.EndHorizontal ();
						
					GUILayout.BeginHorizontal ();
					GUILayout.Label (new GUIContent (AMEditorSystem.ContentPlugin._PluginName, AMEditorSystem.ContentPlugin._PluginNameHelp), GUILayout.Width (Screen.width / 3.19f + 42));
					GUILayout.Label (new GUIContent (AMEditorSystem.ContentPlugin._ActualVersion, AMEditorSystem.ContentPlugin._ActualVersionHelp));
					GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect ().y + 18, Screen.width + 2, 1), "");
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();
					#endregion

					try 
					{
						foreach (var item in externalPluginsGUIList) 
						{
							if (externalSearch == string.Empty || externalSearch == searchPlaceholder)
								item.DrawGUI ();
							else
							{
								if (item.name.ToLower ().Contains (externalSearch.ToLower ()))
									item.DrawGUI ();
							}
						}	
					} 
					catch (Exception) 
					{}	
				}
				EditorGUILayout.Space ();
			}
				
		public void InitAccount ()
		{
			if (!GetPrivateToken ())
			{
				currentWindow = WindowType.Authorization;
			}
		}

		void OnDestroy ()
		{
			isInit = false;
			authError = false;
			isDownloadPlugin = false;
			instance = null;
			installedPlugins = null;
			tempDownloadedPlugins = null;
			actualPlugins = null;
			pluginsWhitelist = null;
			DownloadQueue = null;
			CheckList = null;
			pluginsBuildtypes = null;
			exampleFilePath = string.Empty;
		}
				
		static string ignoreMeta=@"
  Editor:
   enabled: 1";
		static string rightStandaloneMeta=@"
 serializedVersion: 1
 iconMap: {}
 executionOrder: {}
 isPreloaded: 0
 platformData:
  Android:
   enabled: 0
   settings:
    CPU: AnyCPU
  Any:
   enabled: 0
   settings: {}
  BlackBerry:
   enabled: 0
   settings: {}
  Editor:
   enabled: 0
   settings:
    CPU: AnyCPU
    DefaultValueInitialized: true
    OS: AnyOS
  Linux:
   enabled: 1
   settings:
    CPU: x86
  Linux64:
   enabled: 1
   settings:
    CPU: x86_64
  LinuxUniversal:
   enabled: 1
   settings: {}
  OSXIntel:
   enabled: 1
   settings:
    CPU: AnyCPU
  OSXIntel64:
   enabled: 1
   settings:
    CPU: AnyCPU
  OSXUniversal:
   enabled: 1
   settings: {}
  Win:
   enabled: 1
   settings:
    CPU: AnyCPU
  Win64:
   enabled: 1
   settings:
    CPU: AnyCPU
  iOS:
   enabled: 0
   settings:
    CompileFlags: 
    FrameworkDependencies:
 userData: 
 assetBundleName: 
 assetBundleVariant: 
";			
}
	public class MergePluginsAPI
	{
		public static void DeleteOutdatedPlugins (Plugin plugin)
		{
		}
	}
			
	public class MetaFilesAPI
	{
		public static bool CopyMetafile (string oldpath, string path)
		{
			try 
			{
				if (AMEditorFileStorage.FileExist (oldpath))
				{
					AMEditorFileStorage.CopyFile (oldpath, path, true);
				}
				return true;
			} 
			catch (Exception) 
			{
				return false;
			}
		}
	}
		
	public class Backup
	{
		public string name;
		public string dateTime;
		public bool extended;
		public List<fileToggle> files;
	}
}
#endif