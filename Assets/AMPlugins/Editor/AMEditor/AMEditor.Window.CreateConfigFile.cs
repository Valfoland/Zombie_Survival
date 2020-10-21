#if UNITY_EDITOR
#pragma warning disable
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace AMEditor
{
	public class WindowCreateConfigFile :  EditorWindow
	{
		enum TypeWindow
		{
			PluginSettings = 1, 
			ExportSettings, 
			CriticalError
		}
		enum DragAndDropFilesGroup
		{
			Core, 
			Example, 
			modifiable, 
			Outdated
		}
		struct PluginDepend
		{
			public string name;
			public string version;
			public string mod;
		}
		public static string hideDuplicatesDialogDefine = "AM_EDITOR_HIDE_DUPLICATE_FILES_DIALOG";

		//Plugins Settings
		static string pluginName = string.Empty;
		static string pluginUrl = string.Empty;
		static string pluginVersion = string.Empty;
		static string pluginViewTypeString = string.Empty;
		static List<string> pluginViewTypesList;
		static string pluginBuildTypesString = string.Empty;
		static bool isMandatory = false;
		static List<PluginDepend> pluginDepends;
		static List<string> dependModsList;
		static List<string> pluginOldNamesList;
		static string pluginOldName = string.Empty;
		public static List<ProjectFile> coreFilesList;
		public static List<ProjectFile> examplesFilesList;
		public static List<ProjectFile> modifiableFilesList;
		static List<ProjectFile> dragAndDropList;

		public static ProjectFolder assetsFolder;
		public static List<string> allAssetsPaths;
		static List<string> ignoreFiles;
		static List<string> ignoreFolders;
		static List<ProjectFile> outdatedFiles;
		static List<string> exportFilesList;

		//Export Settings
		static string pathToMainFolder = string.Empty;
		static string pathToExport = string.Empty;
		static bool isCreateConfig = true;
		static bool isUnityPackage = true;
		static bool isCopyFiles = true;
		static string[] projectBuildTypes = new string[] { "none", "SNAPSHOT", "RC", "RELEASE", "HOTFIX" };
		static int projectBuildTypeIndex = 0;
		static string projectBuildNumber = "";
		static string extraBuildOptions = "";

		private const string CUSTOM_CODE_PLUGIN_NAME = "Custom Code";

		static string configText = string.Empty;		

		string[] filesType = new string[]{AMEditorSystem.ContentCreateConfig._CurrentFilesPlugins, AMEditorSystem.ContentCreateConfig._OutdatedFilesPlugins};
		int filesTypeIndex = 0; 

		bool isInit = false;
		bool firstShow = true;
		static bool dontShowDuplicateDialog = false;
		#if AM_EDITOR_COMPACT_ON
		static string resourceFolder = string.Empty;
        #if UNITY_2018_1_OR_NEWER || UNITY_2018_2_OR_NEWER
        Texture _addButtonTexture;
        public Texture addButtonTexture{
            get{ return _addButtonTexture; }
            set{ _addButtonTexture = value; }
        }
        Texture _nextButtonTexture;
        public Texture nextButtonTexture{
            get{ return _nextButtonTexture; }
            set{ _nextButtonTexture = value; }
        }
        Texture _backButtonTexture;
        public Texture backButtonTexture{
            get{ return _backButtonTexture; }
            set{ _backButtonTexture = value; }
        }
        Texture _cancelButtonTexture;
        public Texture cancelButtonTexture{
            get{ return _cancelButtonTexture; }
            set{ _cancelButtonTexture = value; }
        }
        Texture _saveButtonTexture;
        public Texture saveButtonTexture{
            get{ return _saveButtonTexture; }
            set{ _saveButtonTexture = value; }
        }
        Texture _editButtonTexture;
        public Texture editButtonTexture{
            get{ return _editButtonTexture; }
            set{ _editButtonTexture = value; }
        }
        Texture _deleteButtonTexture;
        public Texture deleteButtonTexture{
            get{ return _deleteButtonTexture; }
            set{ _deleteButtonTexture = value; }
        }
        #else
		static Texture addButtonTexture = new Texture ();
		static Texture nextButtonTexture = new Texture ();
		static Texture backButtonTexture = new Texture ();
		static Texture cancelButtonTexture = new Texture ();
		static Texture saveButtonTexture = new Texture ();
		static Texture editButtonTexture = new Texture ();
		static Texture deleteButtonTexture = new Texture ();
        #endif
		#endif
		
		#if UNITY_2018_1_OR_NEWER || UNITY_2018_2_OR_NEWER
		Texture folderTexture;
		public Texture FolderTexture
        {
            get
            {
                return folderTexture;
            }

            set
            {
                folderTexture = value;
            }
        }
		Texture csFileTexture;
        public Texture CsFileTexture
        {
            get
            {
                return csFileTexture;
            }

            set
            {
                csFileTexture = value;
            }
        }
		Texture jsFileTexture;
        public Texture JsFileTexture
        {
            get
            {
                return jsFileTexture;
            }

            set
            {
                jsFileTexture = value;
            }
        }
		Texture plistFileTexture;
        public Texture PlistFileTexture
        {
            get
            {
                return plistFileTexture;
            }

            set
            {
                plistFileTexture = value;
            }
        }
		Texture textFileTexture;
        public Texture TextFileTexture
        {
            get
            {
                return textFileTexture;
            }

            set
            {
                textFileTexture = value;
            }
        }
		Texture prefabFileTexture;
        public Texture PrefabFileTexture
        {
            get
            {
                return prefabFileTexture;
            }

            set
            {
                prefabFileTexture = value;
            }
        }
		Texture sceneFileTexture;
        public Texture SceneFileTexture
        {
            get
            {
                return sceneFileTexture;
            }

            set
            {
                sceneFileTexture = value;
            }
        }
		Texture fileTexture;
        public Texture FileTexture
        {
            get
            {
                return fileTexture;
            }

            set
            {
                fileTexture = value;
            }
        }
		#else
		Texture folderTexture = new Texture ();
		Texture csFileTexture = new Texture ();
		Texture jsFileTexture = new Texture ();
		Texture plistFileTexture = new Texture ();
		Texture textFileTexture = new Texture ();
		Texture prefabFileTexture = new Texture ();
		Texture sceneFileTexture = new Texture ();
		Texture fileTexture = new Texture ();
		#endif

		Vector2 scrollPosition = Vector2.zero;

		float stepIndent = 16.0f;

		static TypeWindow currentWindow = TypeWindow.PluginSettings;
		static DragAndDropFilesGroup dragAndDropFilesGroup = DragAndDropFilesGroup.Core;

		static string pluginNamePlaceholder = AMEditorSystem.ContentCreateConfig._DefaultNameDepend;
		string pluginUrlPlaceholder = @"http://pgit.digital-ecosystems.ru/groups/unity-plugins/plugin-name";
		string pluginVersionPlaceholder = "1.0.0  ";
		string pluginOldNamePlaceholder = AMEditorSystem.ContentCreateConfig._DefaultOldName;

		public static Dictionary<string, List<string>> uniqFilesMap;
		public static List<string> uniqFilesList;
		public static List<string> duplicateFilesList;

		GUIStyle foldoutStyle = null;
		GUIStyle popupStyle = null;
		GUIStyle pluginNameFieldStyle = null;
		GUIStyle pluginVersionFieldStyle = null;
		GUIStyle pluginUrlFieldStyle = null;
		GUIStyle dependNameFieldStyle = null;
		GUIStyle dependVersionFieldStyle = null;
		GUIStyle oldNameFieldStyle = null;
		GUIStyle fileStyle = null;
		GUIStyle folderTextureStyle = null;
		GUIStyle folderLabelStyle = null;
		GUIStyle toggleAllStyle = null;
		GUIStyle uniqueLabelStyle = null;
		GUIStyle dragAndDropAreaStyle = null;

		int buttonHeight = 18;
		#if AM_EDITOR_COMPACT_ON
			int buttonWidth = 24;
			int widebuttonWidth = 24;
		#else
			int buttonWidth = 100;
			int widebuttonWidth = 200;
		#endif
		static GUIContent addDependButtonContent;
		static GUIContent deleteButtonContent;
		static GUIContent addOldNameButtonContent;
		static GUIContent addCoreButtonContent;
		static GUIContent addModifiableButtonContent;
		static GUIContent addExampleButtonContent;
		static GUIContent addOutdatedFileButtonContent;
		static GUIContent editFilesButtonContent;
		static GUIContent backButtonContent;
		static GUIContent selectFolderButtonContent;
		static GUIContent cancelButtonContent;
		static GUIContent createConfigButtonContent;

		void OnEnable ()
		{
			#if AM_EDITOR_COMPACT_ON
			resourceFolder = EditorGUIUtility.isProSkin ? "pro" : "free";
			#endif
			#if AM_EDITOR_COMPACT_ON
				addButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/"+resourceFolder+"/Compact/am_editor_add.png");
				nextButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/"+resourceFolder+"/Compact/am_editor_next.png");
				backButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/"+resourceFolder+"/Compact/am_editor_back.png");
				cancelButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/"+resourceFolder+"/Compact/am_editor_close.png");
				saveButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/"+resourceFolder+"/Compact/am_editor_save.png");
				editButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/"+resourceFolder+"/Compact/am_editor_edit.png");
				deleteButtonTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/"+resourceFolder+"/Compact/am_editor_delete.png");
			#endif
				folderTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_folder.png");
				csFileTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_cs_file.png");
				jsFileTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_js_file.png");
				plistFileTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_plist_file.png");
				textFileTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_text_file.png");
				prefabFileTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_prefab.png");
				sceneFileTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_scene.png");
				fileTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_other_file.png");
			#if AM_EDITOR_COMPACT_ON
				addDependButtonContent = new GUIContent (addButtonTexture, AMEditorSystem.ContentCreateConfig._AddDependHelp);
				deleteButtonContent = new GUIContent (deleteButtonTexture, AMEditorSystem.ContentStandardButton._DeleteHelp);
				addOldNameButtonContent = new GUIContent (addButtonTexture, AMEditorSystem.ContentCreateConfig._AddOldNameHelp);
				addCoreButtonContent = new GUIContent (addButtonTexture, AMEditorSystem.ContentCreateConfig._AddCoreHelp);
				addModifiableButtonContent = new GUIContent (addButtonTexture, AMEditorSystem.ContentCreateConfig._AddModifiableHelp);
				addExampleButtonContent = new GUIContent (addButtonTexture, AMEditorSystem.ContentCreateConfig._AddExampleHelp);
				addOutdatedFileButtonContent = new GUIContent (addButtonTexture, AMEditorSystem.ContentCreateConfig._AddOutdated);
				editFilesButtonContent = new GUIContent (editButtonTexture, AMEditorSystem.ContentCreateConfig._EditFilesList);
				backButtonContent = new GUIContent (backButtonTexture, AMEditorSystem.ContentCreateConfig._Back);
				selectFolderButtonContent = new GUIContent (nextButtonTexture, AMEditorSystem.ContentCreateConfig._SelectFolder);
				cancelButtonContent = new GUIContent (cancelButtonTexture, AMEditorSystem.ContentStandardButton._Cancel);
				createConfigButtonContent = new GUIContent (saveButtonTexture, AMEditorSystem.ContentCreateConfig._CreateConfig);
			#else
				addDependButtonContent = new GUIContent (AMEditorSystem.ContentCreateConfig._AddDepend, AMEditorSystem.ContentCreateConfig._AddDependHelp);
				deleteButtonContent = new GUIContent (AMEditorSystem.ContentStandardButton._Delete, AMEditorSystem.ContentStandardButton._DeleteHelp);
				addOldNameButtonContent = new GUIContent (AMEditorSystem.ContentCreateConfig._AddOldName, AMEditorSystem.ContentCreateConfig._AddOldNameHelp);
				addCoreButtonContent = new GUIContent (AMEditorSystem.ContentCreateConfig._AddCore, AMEditorSystem.ContentCreateConfig._AddCoreHelp);
				addModifiableButtonContent = new GUIContent (AMEditorSystem.ContentCreateConfig._AddModifiable, AMEditorSystem.ContentCreateConfig._AddModifiableHelp);
				addExampleButtonContent = new GUIContent (AMEditorSystem.ContentCreateConfig._AddExample, AMEditorSystem.ContentCreateConfig._AddExampleHelp);
				addOutdatedFileButtonContent = new GUIContent (AMEditorSystem.ContentCreateConfig._AddOutdated, "");
				editFilesButtonContent = new GUIContent (AMEditorSystem.ContentCreateConfig._EditFilesList, "");
				backButtonContent = new GUIContent (AMEditorSystem.ContentCreateConfig._Back, "");
				selectFolderButtonContent = new GUIContent (AMEditorSystem.ContentCreateConfig._SelectFolder, "");
				cancelButtonContent = new GUIContent (AMEditorSystem.ContentStandardButton._Cancel, "");
				createConfigButtonContent = new GUIContent (AMEditorSystem.ContentCreateConfig._CreateConfig, "");
			#endif
		}

		public static void Init ()
		{
			WindowCreateConfigFile window = EditorWindow.GetWindow<WindowCreateConfigFile> (true, AMEditorSystem.ContentCreateConfig._Title);

			window.minSize = new Vector2 (600, 700);
			window.maxSize = new Vector2 (600, Screen.currentResolution.height);

			if (pluginOldNamesList == null)
				pluginOldNamesList = new List<string> ();

			if (pluginDepends == null)
				pluginDepends = new List<PluginDepend> ();

			if (coreFilesList == null)
				coreFilesList = new List<ProjectFile> ();

			if (examplesFilesList == null)
				examplesFilesList = new List<ProjectFile> ();

			if (modifiableFilesList == null)
				modifiableFilesList = new List<ProjectFile> ();

			if (outdatedFiles == null)
				outdatedFiles = new List<ProjectFile> ();
			
			window.Repaint ();
        }

		#region Initialization
		void ParseConfig (string path)
		{
			List<ProjectFile> missingFiles = new List<ProjectFile> ();

			Hashtable config = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (path)) as Hashtable;
			pluginName = (string)(config ["name"]);
			pluginVersion = (string)(config ["version"]);
			try 
			{
				isMandatory = (bool)(config ["mandatory"]);
			} 
			catch (Exception)
			{
				isMandatory = false;
			}
			try
			{
				pluginViewTypeString = (string)(config["display_type"]);
				if (pluginViewTypeString == string.Empty)
				{
					pluginViewTypeString = "none";
				}
			}
			catch (Exception)
			{
				pluginViewTypeString = "none";
			}
			pluginUrl = (string)(config ["url"]);
			ArrayList pluginBuildTypes = config ["build_types"] as ArrayList;
			if (pluginBuildTypes != null && pluginBuildTypes.Count > 0)
			{
				for (int i = 0; i < pluginBuildTypes.Count; i++)
				{
					pluginBuildTypesString += (string)pluginBuildTypes[i]+" ";
				}
			}
			else
			{
				pluginBuildTypesString = "";
			}

			ArrayList files = config["files"] as ArrayList;

			if (files != null && files.Count > 0)
			{
				for (int i = 0; i < files.Count; i++)
				{
					ProjectFile temp = new ProjectFile (files[i] as Hashtable);

					if (temp.outdated) 
					{
						temp.unique = false;
						temp.modifiable = false;
						temp.example = false;
						outdatedFiles.Add (temp);
					} 
					else
					{
						if (!temp.fullPath.Contains (".prefab") && !temp.fullPath.Contains (".unity") && !temp.fullPath.Contains ("plugin_config.json") &&
							!AMEditorFileStorage.FileExist (temp.fullPath)) 
						{
							missingFiles.Add (temp);
						} 
						else 
						{
							if (temp.modifiable)
							{
								modifiableFilesList.Add (temp);
							}
							else if (temp.example)
							{
								examplesFilesList.Add (temp);
							}
							else
							{
								coreFilesList.Add (temp);
							}
						}
					}
				}
			}

			var depen = config["depends"] as ArrayList;
			if (depen != null && depen.Count > 0)
			{
				string tempName = string.Empty;
				string tempVersion = string.Empty;
				string tempMod = string.Empty;

				for (int i = 0; i < depen.Count; i++)
				{
					tempName = (string)((depen[i] as Hashtable)["name"]);
					tempVersion = (string)((depen[i] as Hashtable)["version"]);
					if (tempVersion == null)
					{
						tempVersion = string.Empty;
					}
					tempMod = (string)((depen[i] as Hashtable)["mod"]);
					if (tempMod == null)
					{
						tempMod = ">=";
					}
					pluginDepends.Add (new PluginDepend {
						name = tempName, 
						version = tempVersion, 
						mod = tempMod, 
					});
				}
			}
			try 
			{
				var old_names = config["old_names"] as ArrayList;	
				if (old_names != null && old_names.Count > 0)
				{
					for (int i = 0; i < old_names.Count; i++)
					{
						pluginOldNamesList.Add ((string)old_names[i]);
					}
				}
			} 
			catch (Exception) 
			{}

			if (missingFiles != null && missingFiles.Count > 0)
			{
				new AMEditor.UI.AMDisplayDialog (AMEditorSystem.ContentCreateConfig._MissingFilesTitle, AMEditorSystem.ContentCreateConfig._MissingFilesQuestion, 
					AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._No, 
					() => { 
						for (int i = 0; i < missingFiles.Count; i++)
						{ 
							missingFiles[i].unique = false;
							missingFiles[i].modifiable = false;
							missingFiles[i].example = false;
							missingFiles[i].outdated = true;
							outdatedFiles.Add (missingFiles[i]);
						}
					}, () => { }, true).Show ();
			}
		}

		static void InitFiles ()
		{
			List<string> files = AMEditorFileStorage.GetAllFiles ("Assets");
			assetsFolder = new ProjectFolder ("Assets", "Assets");
			assetsFolder.expand = true;

			if (files == null) return;

			for (int i = 0; i < files.Count; i++)
			{
				string item = files [i];

				if ((!isIgnoreFile (item))&& (!isIgnoreFolder (item)))
				{
					assetsFolder.AddFile (item);
				}
			}

			uniqFilesList = new List<string> ();

			List<Hashtable> allFiles = assetsFolder.GetAllFiles ();

			if (allFiles == null) return;

			for (int i = 0; i < allFiles.Count; i++)
			{
				Hashtable item = allFiles [i];

				ProjectFile file = new ProjectFile (item);
				if (!file.outdated && file.unique)
					uniqFilesList.Add (file.name);
			}
		}
		static void InitIgnore ()
		{
			if ((pluginName == "AMEditor") || (pluginName == "AM Editor") || (pluginName == "AM Editor DLC"))
			{
				if (pluginOldName != pluginName)
				{
					pluginOldName = pluginName;
					ignoreFiles = new List<string> ();
					ignoreFiles.Add ("*.meta");
					ignoreFiles.Add ("*.DS_Store");
					ignoreFiles.Add ("plugin_config.json");
					ignoreFiles.Add ("ameditor_config.json");
					ignoreFolders = new List<string> ();
				}
			}
			else
			{
				if (pluginOldName != pluginName || pluginName == string.Empty || pluginName == pluginNamePlaceholder)
				{
					pluginOldName = pluginName;
					ignoreFiles = new List<string> ();
					ignoreFiles.Add ("*.meta");
					ignoreFiles.Add ("*.DS_Store");
					ignoreFiles.Add ("plugin_config.json");
					ignoreFiles.Add ("ameditor_config.json");

					if ((pluginName != "AMBuilder") && (pluginName != "AM Builder"))
						ignoreFiles.Add ("Ionic.Zip.Unity.dll");
					ignoreFiles.Add ("AMEditorMail.dll");

					ignoreFolders = new List<string> ();
					ignoreFolders.Add ("AMEditor");
					ignoreFolders.Add ("AMGitIntegration");
					ignoreFolders.Add ("AMEditorUtils");
				}
			}
		}
		static bool isIgnoreFile (string path)
		{
			string file = path.Substring (path.LastIndexOf (Path.DirectorySeparatorChar) + 1);
			string ext = file.Substring (file.LastIndexOf (".") + 1);
			if (ignoreFiles != null)
			{
				for (int i = 0; i < ignoreFiles.Count; i++)
				{
					string item = ignoreFiles [i];

					if (item[0] == '*')
					{
						if (item.Substring (item.LastIndexOf (".") + 1).ToLower () == ext.ToLower ())
						{
							return true;
						}
					}
					if (item.ToLower () == file.ToLower ())
					{
						return true;
					}
				}
			}
			return false;
		}
		static bool isIgnoreFolder (string path)
		{
			string[] folders = path.Split (new char[]{Path.DirectorySeparatorChar}, System.StringSplitOptions.RemoveEmptyEntries);
			if (ignoreFolders != null)
			{
				for (int i = 0; i < ignoreFolders.Count; i++)
				{
					string item = ignoreFolders [i];

					for (int j = 0; j < folders.Length; j++)
					{
						if (item.ToLower () == folders[j].ToLower ())
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		#endregion

		void OnFocus ()
		{
			if (firstShow)
			{
				isInit = false;
			}
		}

		void Update ()
		{
			if (EditorApplication.isCompiling)
			{
				Close ();
			}
		}

		Color uiTextColor = Color.magenta;
		void OnGUI ()
		{
			if (uiTextColor == Color.magenta)
				uiTextColor = new GUIStyle (GUI.skin.label).normal.textColor;

			if (foldoutStyle == null)
			{
				foldoutStyle = new GUIStyle (EditorStyles.foldout);
				foldoutStyle.focused.background = foldoutStyle.normal.background;
				foldoutStyle.onFocused.background = foldoutStyle.onNormal.background;
				foldoutStyle.focused.textColor = foldoutStyle.normal.textColor;
				foldoutStyle.active.textColor = foldoutStyle.normal.textColor;
				foldoutStyle.onFocused.textColor = foldoutStyle.normal.textColor;
				foldoutStyle.onActive.textColor = foldoutStyle.normal.textColor;
			}
			if (popupStyle == null)
			{
				popupStyle = new GUIStyle (EditorStyles.popup);
				popupStyle.fontSize = new GUIStyle (GUI.skin.button).fontSize;
				popupStyle.fixedHeight = buttonHeight;
			}
			if (dependModsList == null || dependModsList.Count == 0)
			{
				dependModsList = new List<string> ();
				dependModsList.Add (">");
				dependModsList.Add (">=");
				dependModsList.Add ("=");
			}
			if (pluginViewTypesList == null || pluginViewTypesList.Count == 0)
			{
				pluginViewTypesList = new List<string> ();
				pluginViewTypesList.Add ("minimal");
				pluginViewTypesList.Add ("extended");
				pluginViewTypesList.Add ("none");
			}

			if (!isInit)
			{
				isInit = true;
				if (firstShow)
				{
					firstShow = false;
					//проблема в El Capitan (на Unity 4.7.0 и 5.3.1) с отображением здесь стандартного EditorUtility.DisplayDIalog
					//и OkHandler'ом при использовании AMEditor.UI.AMDisplayDialog 
					//поэтому assetFolder формируется в двух местах
					new AMEditor.UI.AMDisplayDialog (AMEditorSystem.ContentCreateConfig._ConfigExistQuestion, "", 
						AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._No, 
						() => { 
							string path = EditorUtility.OpenFilePanel (AMEditorSystem.ContentCreateConfig._SelectConfig, "Assets", "json");
							if (path != string.Empty)
							{
								ParseConfig (path);
								InitIgnore ();
								InitFiles ();
								uniqFilesMap = new Dictionary<string, List<string>> ();
								uniqFilesMap = AMEditorFileStorage.SearchFile ("Assets", uniqFilesList);
								assetsFolder.UpdateFlags ();
							} 
						}, 
						() => { }, true).Show ();
				}
				InitIgnore ();
				InitFiles ();
				uniqFilesMap = new Dictionary<string, List<string>> ();
				uniqFilesMap = AMEditorFileStorage.SearchFile ("Assets", uniqFilesList);
				assetsFolder.UpdateFlags ();

				if (!PlayerSettings.GetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup).Contains (hideDuplicatesDialogDefine))
				{
					new AMEditor.UI.AMDisplayDialog (AMEditorSystem.ContentCreateConfig._DuplicatedFilesTitle, AMEditorSystem.ContentCreateConfig._DuplicatedFilesMessage, 
						AMEditorSystem.ContentStandardButton._Ok, 
						AMEditorSystem.ContentStandardButton._DontShowAgain, () => { }, () => {
							dontShowDuplicateDialog = true; }, true).Show ();
				}
			}

			switch (currentWindow) 
			{
			case TypeWindow.PluginSettings:
				DrawPluginSetting ();
				break;
			case TypeWindow.ExportSettings:
				DrawExportSettings ();
				break;
			default:
				break;
			}
		}

		#region Plugin Settings
		bool editPluginFiles = false;
		bool dragAndDrop = false;
		string dragAndDropAreaText;
		Rect dragAndDropAreaRect = new Rect (0, 0, 0, 0);
		Event currentEvent;
		void DrawPluginSetting ()
		{
			if (pluginNameFieldStyle == null)
				pluginNameFieldStyle = new GUIStyle (GUI.skin.textField);
			pluginNameFieldStyle.fontStyle = (pluginName == pluginNamePlaceholder) ? FontStyle.Italic : FontStyle.Normal;
			pluginNameFieldStyle.normal.textColor = (pluginName == pluginNamePlaceholder) ? Color.gray : uiTextColor;
			pluginNameFieldStyle.focused.textColor = (pluginName == pluginNamePlaceholder) ? Color.gray : uiTextColor;

			if (pluginUrlFieldStyle == null)
				pluginUrlFieldStyle = new GUIStyle (GUI.skin.textField);
			pluginUrlFieldStyle.fontStyle = (pluginUrl == pluginUrlPlaceholder) ? FontStyle.Italic : FontStyle.Normal;
			pluginUrlFieldStyle.normal.textColor = (pluginUrl == pluginUrlPlaceholder) ? Color.gray : uiTextColor;
			pluginUrlFieldStyle.focused.textColor = (pluginUrl == pluginUrlPlaceholder) ? Color.gray : uiTextColor;

			if (pluginVersionFieldStyle == null)
				pluginVersionFieldStyle = new GUIStyle (GUI.skin.textField);
			pluginVersionFieldStyle.fontStyle = (pluginVersion == pluginVersionPlaceholder) ? FontStyle.Italic : FontStyle.Normal;
			pluginVersionFieldStyle.normal.textColor = (pluginVersion == pluginVersionPlaceholder) ? Color.gray : uiTextColor;
			pluginVersionFieldStyle.focused.textColor = (pluginVersion == pluginVersionPlaceholder) ? Color.gray : uiTextColor;

			GUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._NamePlugin, AMEditorSystem.ContentCreateConfig._NamePluginHelp), GUILayout.Width (140));
			GUI.SetNextControlName ("plugin_name_field");
			pluginName = GUILayout.TextField (pluginName, pluginNameFieldStyle, GUILayout.Width (Screen.width - 152));
			if (Event.current.type == EventType.Repaint)
			{
				if (GUI.GetNameOfFocusedControl () == "plugin_name_field")
				{
					if (pluginName == pluginNamePlaceholder) pluginName = "";
				}
				else
				{
					if (pluginName == "") pluginName = pluginNamePlaceholder;
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._Link, AMEditorSystem.ContentCreateConfig._LinkHelp), GUILayout.Width (140));
			GUI.SetNextControlName ("plugin_url_field");
			pluginUrl = GUILayout.TextField (pluginUrl, pluginUrlFieldStyle, GUILayout.Width (Screen.width - 152));
			if (Event.current.type == EventType.Repaint)
			{
				if (GUI.GetNameOfFocusedControl () == "plugin_url_field")
				{
					if (pluginUrl == pluginUrlPlaceholder) 
						pluginUrl = "";
				}
				else
				{
					if (pluginUrl == "") 
						pluginUrl = pluginUrlPlaceholder;
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._PluginVersion, AMEditorSystem.ContentCreateConfig._PluginVersionHelp), GUILayout.Width (140));
			GUI.SetNextControlName ("plugin_version_field");
			pluginVersion = GUILayout.TextField (pluginVersion, pluginVersionFieldStyle, GUILayout.Width (Screen.width - 152));
			if (Event.current.type == EventType.Repaint)
			{
				if (GUI.GetNameOfFocusedControl () == "plugin_version_field")
				{
					if (pluginVersion == pluginVersionPlaceholder) 
						pluginVersion = "";
				}
				else
				{
					if (pluginVersion == "") 
						pluginVersion = pluginVersionPlaceholder;
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._PluginBuildType, AMEditorSystem.ContentCreateConfig._PluginBuildTypeHelp), GUILayout.Width (140));
			pluginBuildTypesString = GUILayout.TextField (pluginBuildTypesString, GUILayout.Width (Screen.width - 152));

			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._ViewType, AMEditorSystem.ContentCreateConfig._ViewTypeHelp), GUILayout.Width (140));
			int pluginTypeId = pluginViewTypesList.IndexOf (pluginViewTypeString);
			pluginTypeId = EditorGUILayout.Popup (pluginTypeId, pluginViewTypesList.ToArray (), popupStyle, new GUILayoutOption[] {GUILayout.Width (120)});
			try
			{
				pluginViewTypeString = pluginViewTypesList[pluginTypeId];
			}
			catch (Exception)
			{
				pluginViewTypeString = "none";
			}

			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._IsMandatory, AMEditorSystem.ContentCreateConfig._IsMandatoryHelp), GUILayout.Width (140));
			isMandatory = GUILayout.Toggle (isMandatory, "");

			GUILayout.EndHorizontal ();

			DrawGUIDepens ();

			DrawGUIOldNames ();

			GUI.enabled = false;
			GUILayout.Label ("", GUILayout.Height (1));
			GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect ().y-5, Screen.width + 2, 1), "");
			GUI.enabled = true;

			filesTypeIndex = GUILayout.SelectionGrid (filesTypeIndex, filesType, 2);

			GUI.enabled = false;
			GUILayout.Label ("", GUILayout.Height (1));
			GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect ().y+20, Screen.width + 2, 1), "");
			GUI.enabled = true;

			switch (filesTypeIndex) 
			{
			case 0:
				dragAndDropAreaRect = new Rect (4, 214, Screen.width - 8, Screen.height - 252);
				if (dragAndDrop && dragAndDropFilesGroup != DragAndDropFilesGroup.Outdated) 
				{
					DragNDrop ();
				} 
				else 
				{
					if (editPluginFiles)
					{
						GUILayout.Label ("", GUILayout.Height (16));

						scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.MaxHeight (Screen.height - 200));
						DrawGUICoreFiles ();
						DrawGUImodifiable ();
						DrawGUIExamples ();
						GUILayout.EndScrollView ();
					}
					else
					{
						DrawGUIProjectFiles ();
					}
					#if AM_EDITOR_COMPACT_ON
						Rect buttomLeftButtonRect = new Rect (10, Screen.height - (buttonWidth + 7), buttonWidth + 2, buttonWidth);
					#else
						Rect buttomLeftButtonRect = new Rect (10, Screen.height - 33, 150, 30);
					#endif
					GUILayout.Label ("", GUILayout.Height (1));
					GUI.Box (new Rect (-1, Screen.height - 38, Screen.width + 2, 40), "");

					GUIContent content = editPluginFiles ? backButtonContent : editFilesButtonContent;
					if (GUI.Button (buttomLeftButtonRect, content))
					{
						if (editPluginFiles)
						{
							InitFiles ();
							assetsFolder.UpdateFlags ();
						}
						else
						{
							List<Hashtable> selectedAssets = assetsFolder.GetSelectedFiles ();

							for (int i = 0; i < selectedAssets.Count; i++)
							{
								Hashtable item = selectedAssets [i];

								ProjectFile file = new ProjectFile (item);

								if (file.modifiable && modifiableFilesList.FindIndex ((p) => {return p.fullPath == file.fullPath;}) == -1)
								{
									modifiableFilesList.Add (file);
								}
								else if (file.example && examplesFilesList.FindIndex ((p) => {return p.fullPath == file.fullPath;}) == -1)
								{
									examplesFilesList.Add (file);
								}
								else if (!file.example && !file.modifiable && coreFilesList.FindIndex ((p) => {return p.fullPath == file.fullPath;}) == -1)
								{
									coreFilesList.Add (file);
								}
							}
						}
						editPluginFiles = !editPluginFiles;
					}
				}
				GUILayout.BeginHorizontal ();
				GUILayout.Space (10);
				GUILayout.Label ("", GUILayout.Height (30));
				GUILayout.EndHorizontal ();
				break;
			case 1:
				dragAndDropAreaRect = new Rect (4, 214, Screen.width - 8, Screen.height - 252);
				if (dragAndDrop && dragAndDropFilesGroup == DragAndDropFilesGroup.Outdated) 
				{
					DragNDrop ();
				}
				else
				{
					DrawGUIOutdatedFiles ();
				}
				break;
			}

			#if AM_EDITOR_COMPACT_ON
				Rect buttomRightButtonRect = new Rect (Screen.width -  (buttonWidth + 12), Screen.height - (buttonWidth + 7), buttonWidth + 2, buttonWidth);
			#else
				Rect buttomRightButtonRect = new Rect (Screen.width - 160, Screen.height - 33, 150, 30);
			#endif
			GUI.enabled = !dragAndDrop;
			if (GUI.Button (buttomRightButtonRect, selectFolderButtonContent))
			{
				if (ValidParameters ())
				{
					ShowSelectedWindow ();
				}
				else
				{
					EditorUtility.DisplayDialog (AMEditorSystem.ContentCreateConfig._IncorrectParametersDetected, "", AMEditorSystem.ContentStandardButton._Ok);
				}
			}
			GUI.enabled = true;
			if (Event.current.Equals (Event.KeyboardEvent ("return")))
			{
				if (ValidParameters ())
				{
					ShowSelectedWindow ();
				}
				else
				{
					EditorUtility.DisplayDialog (AMEditorSystem.ContentCreateConfig._IncorrectParametersDetected, "", AMEditorSystem.ContentStandardButton._Ok);
				}
			}
		}

		bool ValidParameters ()
		{
			if (pluginName != string.Empty && pluginName != pluginNamePlaceholder &&
				pluginVersion != string.Empty && pluginVersion != pluginVersionPlaceholder &&
				pluginUrl != string.Empty && pluginUrl != pluginUrlPlaceholder)
			{
				if (validDepends () && validOldNames ())
					return true;
				else
					return false;
			}
			else
				return false;
		}
		bool validDepends ()
		{
			if (pluginDepends == null || pluginDepends.Count == 0)
				return true;
			else
			{
				var depList = 
					(from p in pluginDepends
						where (p.name == pluginNamePlaceholder || p.version == pluginVersionPlaceholder || p.name == string.Empty || p.version == string.Empty)
						select p).ToList ();
				if (depList == null || depList.Count == 0)
					return true;
				else
					return false;
			}
		}
		bool validOldNames ()
		{
			if (pluginOldNamesList == null || pluginOldNamesList.Count == 0)
				return true;
			else
			{
				var oldNames = 
					(from n in pluginOldNamesList
						where (n == pluginOldNamePlaceholder || n == string.Empty)
						select n).ToList ();
				if (oldNames == null || oldNames.Count == 0)
					return true;
				else
					return false;
			}
		}

		bool expandDepends = (pluginDepends != null && pluginDepends.Count > 0) ? true : false;
		void DrawGUIDepens ()
		{
			string count = string.Empty;
			if (pluginDepends != null && pluginDepends.Count > 0 && !expandDepends)
				count = " ("+pluginDepends.Count+"):";
			else count = ":";

			if (pluginDepends == null || pluginDepends.Count == 0) 
				expandDepends = false;

			GUILayout.BeginHorizontal ();
			expandDepends = EditorGUILayout.Foldout (expandDepends, new GUIContent (AMEditorSystem.ContentCreateConfig._DependsPlugin+count, AMEditorSystem.ContentCreateConfig._DependsPluginHelp), foldoutStyle);
			GUILayout.Label ("");
			if (!expandDepends)
			{
				if (GUILayout.Button (addDependButtonContent, new GUILayoutOption []{GUILayout.Width (widebuttonWidth), GUILayout.Height (buttonHeight)}))
				{
					pluginDepends.Add (new PluginDepend {
						name = pluginNamePlaceholder, 
						version = pluginVersionPlaceholder, 
						mod = ">=", 
					});
					expandDepends = true;
				}
			}
			GUILayout.EndHorizontal ();

			if (pluginDepends != null && expandDepends)
			{
				EditorGUILayout.Space ();

				for (int i = 0; i < pluginDepends.Count; i++) 
				{
					if (dependNameFieldStyle == null)
						dependNameFieldStyle = new GUIStyle (GUI.skin.textField);
					dependNameFieldStyle.fontStyle = (pluginDepends[i].name == pluginNamePlaceholder)?FontStyle.Italic:FontStyle.Normal;
					dependNameFieldStyle.normal.textColor = (pluginDepends[i].name == pluginNamePlaceholder)?Color.gray:uiTextColor;
					dependNameFieldStyle.focused.textColor = (pluginDepends[i].name == pluginNamePlaceholder)?Color.gray:uiTextColor;

					if (dependVersionFieldStyle == null)
						dependVersionFieldStyle = new GUIStyle (GUI.skin.textField);
					dependVersionFieldStyle.fontStyle = (pluginDepends[i].version == pluginVersionPlaceholder)?FontStyle.Italic:FontStyle.Normal;
					dependVersionFieldStyle.normal.textColor = (pluginDepends[i].version == pluginVersionPlaceholder)?Color.gray:uiTextColor;
					dependVersionFieldStyle.focused.textColor = (pluginDepends[i].version == pluginVersionPlaceholder)?Color.gray:uiTextColor;

					string name = pluginDepends[i].name;
					string version = pluginDepends[i].version;
					string mod = pluginDepends[i].mod;

					GUILayout.BeginHorizontal ();

					GUI.SetNextControlName ("depend_name_field"+i);
					name = GUILayout.TextField (name, dependNameFieldStyle, GUILayout.Width (Screen.width -  (200+buttonWidth)));
					if (Event.current.type == EventType.Repaint)
					{
						if (GUI.GetNameOfFocusedControl () == "depend_name_field"+i)
						{
							if (name == pluginNamePlaceholder) 
								name = "";
						}
						else
						{
							if (name == "") 
								name = pluginNamePlaceholder;
						}
					}
					GUI.SetNextControlName ("depend_version_field"+i);
					version = GUILayout.TextField (version, dependVersionFieldStyle, GUILayout.Width (140));
					Rect versionRect = GUILayoutUtility.GetLastRect ();
					if (Event.current.type == EventType.Repaint)
					{
						if (GUI.GetNameOfFocusedControl () == "depend_version_field"+i)
						{
							if (version == pluginVersionPlaceholder) 
								version = "";
						}
						else
						{
							if (version == "") 
								version = pluginVersionPlaceholder;
						}
					}
					int dependModId = dependModsList.IndexOf (mod);
					dependModId = EditorGUI.Popup (new Rect (versionRect.x + versionRect.width + 4, versionRect.y - 1, 40, buttonHeight), dependModId, dependModsList.ToArray (), popupStyle);
					mod = dependModsList[dependModId];

					pluginDepends.RemoveAt (i);
					pluginDepends.Insert (i, new PluginDepend {
						name = name, 
						version = version, 
						mod = mod, 
					});

					if (GUI.Button (new Rect (versionRect.x + versionRect.width + 48, versionRect.y - 1, buttonWidth, buttonHeight), deleteButtonContent))
					{
						pluginDepends.RemoveAt (i);
					}
					GUILayout.EndHorizontal ();
				}

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("");

				if (GUILayout.Button (addDependButtonContent, new GUILayoutOption []{GUILayout.Width (widebuttonWidth), GUILayout.Height (buttonHeight)}))
				{
					pluginDepends.Add (new PluginDepend {
						name = pluginNamePlaceholder, 
						version = pluginVersionPlaceholder, 
						mod = ">=", 
					});
				}
				GUILayout.EndHorizontal ();
			}
			GUI.enabled = false;
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("", GUILayout.Height (5));
			GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect ().y + 2.5f, Screen.width + 2, 1), "");
			GUILayout.EndHorizontal ();
			GUI.enabled = true;
		}

		bool expandOldNames = (pluginOldNamesList != null && pluginOldNamesList.Count > 0) ? true : false;
		void DrawGUIOldNames ()
		{
			string count = string.Empty;
			if (pluginOldNamesList != null && pluginOldNamesList.Count > 0 && !expandOldNames)
				count = " ("+pluginOldNamesList.Count+"):";
			else count = ":";

			if (pluginOldNamesList == null || pluginOldNamesList.Count == 0) 
				expandOldNames = false;

			GUILayout.BeginHorizontal ();
			expandOldNames = EditorGUILayout.Foldout (expandOldNames, new GUIContent (AMEditorSystem.ContentCreateConfig._OldNames+count, AMEditorSystem.ContentCreateConfig._OldNamesHelp), foldoutStyle);
			GUILayout.Label ("");
			if (!expandOldNames)
			{
				if (GUILayout.Button (addOldNameButtonContent, new GUILayoutOption []{GUILayout.Width (widebuttonWidth), GUILayout.Height (buttonHeight)}))
				{
					pluginOldNamesList.Add (AMEditorSystem.ContentCreateConfig._DefaultOldName);
					expandOldNames = true;
				}
			}
			GUILayout.EndHorizontal ();

			if (pluginOldNamesList != null && expandOldNames)
			{
				EditorGUILayout.Space ();

				for (int i = 0; i < pluginOldNamesList.Count; i++) 
				{
					if (oldNameFieldStyle == null)
						oldNameFieldStyle = new GUIStyle (GUI.skin.textField);
					oldNameFieldStyle.fontStyle = (pluginOldNamesList[i] == pluginOldNamePlaceholder) ? FontStyle.Italic : FontStyle.Normal;
					oldNameFieldStyle.normal.textColor = (pluginOldNamesList[i] == pluginOldNamePlaceholder) ? Color.gray : uiTextColor;
					oldNameFieldStyle.focused.textColor = (pluginOldNamesList[i] == pluginOldNamePlaceholder) ? Color.gray : uiTextColor;

					GUILayout.BeginHorizontal ();

					GUI.SetNextControlName ("old_name_field"+i);
					pluginOldNamesList[i] = GUILayout.TextField (pluginOldNamesList[i], oldNameFieldStyle, GUILayout.Width (Screen.width -  (buttonWidth + 12)));

					if (Event.current.type == EventType.Repaint)
					{
						if (GUI.GetNameOfFocusedControl () == "old_name_field"+i)
						{
							if (pluginOldNamesList[i] == pluginOldNamePlaceholder) 
								pluginOldNamesList[i] = "";
						}
						else
						{
							if (pluginOldNamesList[i] == "") 
								pluginOldNamesList[i] = pluginOldNamePlaceholder;
						}
					}
					if (GUI.Button (new Rect (GUILayoutUtility.GetLastRect ().x + GUILayoutUtility.GetLastRect ().width + 4, GUILayoutUtility.GetLastRect ().y - 1, buttonWidth, buttonHeight), deleteButtonContent))
					{
						pluginOldNamesList.RemoveAt (i);
					}
					GUILayout.EndHorizontal ();
				}

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("");

				if (GUILayout.Button (addOldNameButtonContent, new GUILayoutOption []{GUILayout.Width (widebuttonWidth), GUILayout.Height (buttonHeight)}))
				{
					pluginOldNamesList.Add (AMEditorSystem.ContentCreateConfig._DefaultOldName);
				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.Label ("", GUILayout.Height (5));
		}

		float guiElementHeight = 16.0f;
		void DrawGUIFiles (ProjectFolder folder, int indent)
		{
			if (fileStyle == null)
				fileStyle = new GUIStyle (GUI.skin.label);
			if (folderTextureStyle == null)
				folderTextureStyle = new GUIStyle (EditorStyles.foldout);
			folderTextureStyle.fontStyle = folder.selected ? FontStyle.Bold : FontStyle.Normal;
			if (folderLabelStyle == null)
				folderLabelStyle = new GUIStyle (GUI.skin.label);
			folderLabelStyle.fontStyle = folder.selected ? FontStyle.Bold : FontStyle.Normal;

			GUILayout.BeginHorizontal ();
			bool oldValue = folder.selected;
			folder.selected = GUILayout.Toggle (folder.selected, "" , GUILayout.Width (10), GUILayout.MaxHeight (guiElementHeight));
			Rect folderSelectorToggleRect = GUILayoutUtility.GetLastRect ();

			if (oldValue != folder.selected)
			{
				SwitchBranch (folder);
			}
			folder.expand = EditorGUI.Foldout (new Rect (folderSelectorToggleRect.x + 14 + (stepIndent * indent), folderSelectorToggleRect.y + 1, guiElementHeight, guiElementHeight), 
				folder.expand, new GUIContent (" ", folderTexture, folder.fullPath), folderTextureStyle);
			GUI.Label (new Rect (folderSelectorToggleRect.x + 38 + (stepIndent * indent), folderSelectorToggleRect.y + 1, Screen.width / 3, guiElementHeight), 
				new GUIContent (" " + folder.name, folder.fullPath), folderLabelStyle);
			GUILayout.EndHorizontal ();

			if (folder.expand)
			{
				if (folder.folders == null) return;

				for (int i = 0; i < folder.folders.Count; i++)
				{
					DrawGUIFiles (folder.folders[i], indent + 1);
				}
				if (folder.files == null) return;

				for (int i = 0; i < folder.files.Count; i++)
				{
					ProjectFile item = folder.files [i];

					fileStyle.fontStyle = item.selected ? FontStyle.Bold : FontStyle.Normal;

					bool ismodifiable = item.modifiable;
					bool isExample = item.example;

					GUILayout.BeginHorizontal ();

					//30+ (stepIndent * (indent + 0.5f))
					item.selected = GUILayout.Toggle (item.selected, "", GUILayout.Width (10), GUILayout.MaxHeight (guiElementHeight));
					Rect fileSelectorToggleRect = GUILayoutUtility.GetLastRect ();

					Texture itemIconTexture = fileTexture;
					string itemResolution = string.Empty;
					itemResolution = item.name.Substring (item.name.LastIndexOf (".") == -1 ? 0 : item.name.LastIndexOf ("."));

					switch (itemResolution)
					{
					case ".cs":
						itemIconTexture = csFileTexture;
						break;
					case ".js":
						itemIconTexture = jsFileTexture;
						break;
					case ".prefab":
						itemIconTexture = prefabFileTexture;
						break;
					case ".unity":
						itemIconTexture = sceneFileTexture;
						break;
					case ".plist":
						itemIconTexture = plistFileTexture;
						break;
					case ".txt":
						itemIconTexture = textFileTexture;
						break;
					case ".json":
						itemIconTexture = textFileTexture;
						break;
					default:
						break;
					}

					GUI.Label (new Rect (fileSelectorToggleRect.x + 40 + (stepIndent * indent), fileSelectorToggleRect.y + 1, guiElementHeight - 1, guiElementHeight), 
						itemIconTexture);
					GUI.Label (new Rect (fileSelectorToggleRect.x + 52 + (stepIndent * indent), fileSelectorToggleRect.y + 1, Screen.width / 3, guiElementHeight), 
						new GUIContent (" " + item.name, item.fullPath), fileStyle);

					item.unique = GUI.Toggle (new Rect (fileSelectorToggleRect.x + 370, fileSelectorToggleRect.y, 10, guiElementHeight), item.unique, new GUIContent ("", AMEditorSystem.ContentCreateConfig._UniqueHelp));

					if (GUI.Toggle (new Rect (fileSelectorToggleRect.x + 450, fileSelectorToggleRect.y, 10, guiElementHeight), ismodifiable, new GUIContent (" ", AMEditorSystem.ContentCreateConfig._ModifiableHelp)))
					{
						ismodifiable = true;
						isExample = !ismodifiable == true;
					}
					else
					{
						ismodifiable = false;
					}

					if (GUI.Toggle (new Rect (fileSelectorToggleRect.x + 525, fileSelectorToggleRect.y, 10, guiElementHeight), isExample, new GUIContent ("", AMEditorSystem.ContentCreateConfig._ExampleHelp)))
					{
						isExample = true;
						ismodifiable = !isExample == true;
					}
					else
					{
						isExample = false;
					}
					GUILayout.EndHorizontal ();

					item.modifiable = ismodifiable;
					item.example = isExample;
				}
			}
		}

		void SwitchBranch (ProjectFolder folder)
		{
			List<ProjectFolder> subFolders = folder.folders;
			for (int i = 0; i < subFolders.Count; i++)
			{
				ProjectFolder item = subFolders [i];
				item.selected = folder.selected;
				SwitchBranch (item);
			}
			List<ProjectFile> files = folder.files;
			for (int i = 0; i < files.Count; i++)
			{
				ProjectFile item = files [i];
				item.selected = folder.selected;
			}
		}

		bool expandCore = (coreFilesList != null && coreFilesList.Count > 0) ? true : false;
		void DrawGUICoreFiles ()
		{
			string count = string.Empty;
			if (coreFilesList != null && coreFilesList.Count > 0 && !expandCore)
				count = " ("+coreFilesList.Count+"):";
			else count = ":";

			if (coreFilesList == null || coreFilesList.Count == 0)
				expandCore = false;

			GUILayout.BeginHorizontal ();
			expandCore = EditorGUILayout.Foldout (expandCore, new GUIContent (AMEditorSystem.ContentCreateConfig._CoreFiles+count, AMEditorSystem.ContentCreateConfig._CoreFilesHelp), foldoutStyle);
			GUILayout.Label ("");
			if (GUILayout.Button (addCoreButtonContent, new GUILayoutOption []{GUILayout.Width (widebuttonWidth), GUILayout.Height (buttonHeight)}))
			{
				expandDepends = false;
				expandOldNames = false;

				dragAndDropFilesGroup = DragAndDropFilesGroup.Core;
				dragAndDropAreaText = AMEditorSystem.ContentCreateConfig._DragAndDropAreaCoreText;
				dragAndDropList = coreFilesList;
				dragAndDrop = true;
				expandCore = true;
			}
			GUILayout.EndHorizontal ();

			if (expandCore) 
			{
				for (int i = 0; i < coreFilesList.Count; i++) 
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Label (coreFilesList[i].fullPath);

					if (GUILayout.Button (deleteButtonContent, new GUILayoutOption []{GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
					{
						coreFilesList.RemoveAt (i);
					}
					GUILayout.EndHorizontal ();
				}
			}
			GUI.enabled = false;
			GUILayout.Label ("", GUILayout.Height (5));
			GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect ().y+2.5f, Screen.width + 2, 1), "");
			GUI.enabled = true;
		}

		bool expandModifiable = (modifiableFilesList != null && modifiableFilesList.Count > 0) ? true : false;
		void DrawGUImodifiable ()
		{
			string count = string.Empty;
			if (modifiableFilesList != null && modifiableFilesList.Count > 0 && !expandModifiable)
				count = " ("+modifiableFilesList.Count+"):";
			else count = ":";

			if (modifiableFilesList == null || modifiableFilesList.Count == 0)
				expandModifiable = false;

			GUILayout.BeginHorizontal ();
			expandModifiable = EditorGUILayout.Foldout (expandModifiable, new GUIContent (AMEditorSystem.ContentCreateConfig._ModifiableFiles+count, AMEditorSystem.ContentCreateConfig._ModifiableFilesHelp), foldoutStyle);
			GUILayout.Label ("");
			if (GUILayout.Button (addModifiableButtonContent, new GUILayoutOption []{GUILayout.Width (widebuttonWidth), GUILayout.Height (buttonHeight)}))
			{
				expandDepends = false;
				expandOldNames = false;

				dragAndDropFilesGroup = DragAndDropFilesGroup.modifiable;
				dragAndDropAreaText = AMEditorSystem.ContentCreateConfig._DragAndDropAreaModifiableText;
				dragAndDropList = modifiableFilesList;
				dragAndDrop = true;
				expandModifiable = true;
			}
			GUILayout.EndHorizontal ();

			if (modifiableFilesList != null && expandModifiable)
			{
				for (int i = 0; i < modifiableFilesList.Count; i++) 
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Label (modifiableFilesList[i].fullPath);

					if (GUILayout.Button (deleteButtonContent, new GUILayoutOption []{GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
					{
						modifiableFilesList.RemoveAt (i);
					}
					GUILayout.EndHorizontal ();
				}
			}
			GUI.enabled = false;
			GUILayout.Label ("", GUILayout.Height (5));
			GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect ().y+2.5f, Screen.width + 2, 1), "");
			GUI.enabled = true;
		}

		bool expandExamples = (examplesFilesList != null && examplesFilesList.Count > 0) ? true : false;
		void DrawGUIExamples ()
		{
			string count = string.Empty;
			if (examplesFilesList != null && examplesFilesList.Count > 0 && !expandExamples)
				count = " ("+examplesFilesList.Count+"):";
			else count = ":";

			if (examplesFilesList == null || examplesFilesList.Count == 0)
				expandExamples = false;

			GUILayout.BeginHorizontal ();
			expandExamples = EditorGUILayout.Foldout (expandExamples, new GUIContent (AMEditorSystem.ContentCreateConfig._ExampleFiles+count, AMEditorSystem.ContentCreateConfig._ExampleFilesHelp), foldoutStyle);
			GUILayout.Label ("");
			if (GUILayout.Button (addExampleButtonContent, new GUILayoutOption []{GUILayout.Width (widebuttonWidth), GUILayout.Height (buttonHeight)}))
			{
				expandDepends = false;
				expandOldNames = false;

				dragAndDropFilesGroup = DragAndDropFilesGroup.Example;
				dragAndDropAreaText = AMEditorSystem.ContentCreateConfig._DragAndDropAreaExampleText;
				dragAndDropList = examplesFilesList;
				dragAndDrop = true;
				expandExamples = true;
			}
			GUILayout.EndHorizontal ();

			if (examplesFilesList != null && expandExamples)
			{
				for (int i = 0; i < examplesFilesList.Count; i++) 
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Label (examplesFilesList[i].fullPath);

					if (GUILayout.Button (deleteButtonContent, new GUILayoutOption []{GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
					{
						examplesFilesList.RemoveAt (i);
					}
					GUILayout.EndHorizontal ();
				}
			}
			GUI.enabled = false;
			GUILayout.Label ("", GUILayout.Height (5));
			GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect ().y+2.5f, Screen.width + 2, 1), "");
			GUI.enabled = true;
		}

		bool selectAllAssets = false;
		void DrawGUIProjectFiles ()
		{
			if (toggleAllStyle == null) 
				toggleAllStyle = new GUIStyle (GUI.skin.toggle);
			toggleAllStyle.normal.background = selectAllAssets ? new GUIStyle (GUI.skin.toggle).onNormal.background : new GUIStyle (GUI.skin.toggle).normal.background;

			if (uniqueLabelStyle == null)
			{
				uniqueLabelStyle = new GUIStyle (GUI.skin.label);
				uniqueLabelStyle.alignment = TextAnchor.MiddleCenter;
			}

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (new GUIContent ("", AMEditorSystem.ContentCreateConfig._SelectAssetHelp), toggleAllStyle, GUILayout.Width (12)))
			{
				selectAllAssets = !selectAllAssets;
				assetsFolder.selected = selectAllAssets;
				SwitchBranch (assetsFolder);
			}

			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._SelectAsset, AMEditorSystem.ContentCreateConfig._SelectAssetHelp), 
				GUILayout.Width (315), GUILayout.MaxHeight (guiElementHeight));

			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._Unique, AMEditorSystem.ContentCreateConfig._UniqueHelp), 
				uniqueLabelStyle, new GUILayoutOption[] {GUILayout.Width (80), GUILayout.MaxHeight (guiElementHeight)});

			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._Modifiable, AMEditorSystem.ContentCreateConfig._ModifiableHelp), 
				uniqueLabelStyle, new GUILayoutOption[] {GUILayout.Width (80), GUILayout.MaxHeight (guiElementHeight)});

			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._Example, AMEditorSystem.ContentCreateConfig._ExampleHelp), 
				uniqueLabelStyle, new GUILayoutOption[] {GUILayout.Width (60), GUILayout.MaxHeight (guiElementHeight)});

			GUILayout.EndHorizontal ();

			scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.MaxHeight (Screen.height - 200));
			DrawGUIFiles (assetsFolder, 0);
			GUILayout.EndScrollView ();
		}

		void DrawGUIOutdatedFiles ()
		{
			GUILayout.Label (AMEditorSystem.ContentCreateConfig._Path, GUILayout.Width (200));

			scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.MaxHeight (Screen.height - 200));
			if (outdatedFiles != null)
			{
				for (int i = 0; i < outdatedFiles.Count; i++) 
				{
					GUILayout.BeginHorizontal ();

					GUILayout.Label (outdatedFiles[i].fullPath);

					if (GUILayout.Button (deleteButtonContent, new GUILayoutOption []{GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight)}))
					{
						outdatedFiles.RemoveAt (i);
					}
					GUILayout.EndHorizontal ();
				}
			}
			GUILayout.EndScrollView ();

			GUILayout.Label ("", GUILayout.Height (1));
			GUI.Box (new Rect (-1, Screen.height - 38, Screen.width + 2, 40), "");

			GUILayout.BeginHorizontal ();
			GUILayout.Space (10);
			#if AM_EDITOR_COMPACT_ON
				float addOutdatedFileWidth = buttonWidth + 2;
				float addOutdatedFileHeight = buttonWidth;
			#else
				float addOutdatedFileWidth = 150;
				float addOutdatedFileHeight = 30;
			#endif
			if (GUILayout.Button (addOutdatedFileButtonContent, GUILayout.Width (addOutdatedFileWidth), GUILayout.Height (addOutdatedFileHeight)))
			{
				expandDepends = false;
				expandOldNames = false;

				dragAndDropFilesGroup = DragAndDropFilesGroup.Outdated;
				dragAndDropAreaText = AMEditorSystem.ContentCreateConfig._DragAndDropAreaOutdatedText;
				dragAndDropList = outdatedFiles;
				dragAndDrop = true;
			}
			GUILayout.EndHorizontal ();
			#if AM_EDITOR_COMPACT_ON
				GUILayout.Label ("", GUILayout.Height (2));
			#endif
		}
		#endregion

		#region Drag And Drop
		List<string> GetDragAndDropFiles (string path)
		{
			List<string> result = new List<string> ();

			string[] filesPathesArray = Directory.GetFiles (path);
			string[] foldersPathesArray = Directory.GetDirectories (path);

			for (int i = 0; i < filesPathesArray.Length; i++)
			{
				string filePath = filesPathesArray [i];
				if (!result.Contains (filePath)) 
				{
					result.Add (filePath);
				}
			}
			for (int i = 0; i < foldersPathesArray.Length; i++)
			{
				string folderPath = foldersPathesArray [i];
				List<string> folderFiles = GetDragAndDropFiles (folderPath);
				for (int j = 0; j < folderFiles.Count; j++)
				{
					string file = folderFiles [j];
					if (!result.Contains (file)) 
					{
						result.Add (file);
					}
				}
			}
			return result;
		}

		void DragNDrop ()
		{
			if (dragAndDropAreaStyle == null)
			{
				dragAndDropAreaStyle = new GUIStyle (GUI.skin.box);
				dragAndDropAreaStyle.normal.textColor = uiTextColor;
				dragAndDropAreaStyle.alignment = TextAnchor.MiddleCenter;
			}

			if (expandDepends || expandOldNames)
				dragAndDrop = false;

			GUI.Box (dragAndDropAreaRect, dragAndDropAreaText, dragAndDropAreaStyle);
			if (GUI.Button (new Rect (Screen.width - (buttonWidth + 4), dragAndDropAreaRect.y, buttonWidth, 20), cancelButtonContent)) 
			{
				dragAndDrop = false;
				expandCore = false;
				expandModifiable = false;
				expandExamples = false;
			}

			currentEvent = Event.current;

			if (currentEvent.isKey && currentEvent.keyCode == KeyCode.Escape) 
			{
				dragAndDrop = false;
			}

			switch (currentEvent.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!dragAndDropAreaRect.Contains (currentEvent.mousePosition) || dragAndDropAreaRect == new Rect (0, 0, 0, 0))
				{
					return;
				}

				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (currentEvent.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag ();
					for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
					{
						UnityEngine.Object dragged_object = DragAndDrop.objectReferences [i];
						if (dragged_object != null)
						{
							string dragged_object_path = AssetDatabase.GetAssetPath (dragged_object);

							if (AMEditorFileStorage.FileExist (dragged_object_path))
							{
								string fileName = dragged_object_path.Substring (dragged_object_path.LastIndexOf ('/')+1);
								ProjectFile file = new ProjectFile (fileName, dragged_object_path);

								if (dragAndDropList.FindIndex ((f) => {return f.fullPath == file.fullPath;}) == -1 && 
									!isIgnoreFile (dragged_object_path) && !isIgnoreFolder (dragged_object_path))
									dragAndDropList.Add (file);
							}
							else if (AMEditorFileStorage.ExistFolder (dragged_object_path))
							{
								List<string> filesPathes = GetDragAndDropFiles (dragged_object_path);

								for (int j = 0; j < filesPathes.Count; j++)
								{
									string filePath = filesPathes [j];
									string fileName = filePath.Substring (filePath.LastIndexOf ('/')+1);
									ProjectFile file = new ProjectFile (fileName, filePath);

									if (dragAndDropList.FindIndex ((f) => {return f.fullPath == file.fullPath;}) == -1 && 
										!isIgnoreFile (filePath) && !isIgnoreFolder (filePath))
										dragAndDropList.Add (file);
								}
							}
						}
					}
				}
				break;
			case EventType.DragExited:
				dragAndDrop = false;
				dragAndDropAreaRect = new Rect (0, 0, 0, 0);

				switch (dragAndDropFilesGroup) 
				{
				case DragAndDropFilesGroup.Core:
					coreFilesList = dragAndDropList;
					break;
				case DragAndDropFilesGroup.Example:
					examplesFilesList = dragAndDropList;
					break;
				case DragAndDropFilesGroup.modifiable:
					modifiableFilesList = dragAndDropList;
					break;
				case DragAndDropFilesGroup.Outdated:
					outdatedFiles = dragAndDropList;
					break;
				default:
					break;
				}
				dragAndDropList = null;
				DragAndDrop.PrepareStartDrag ();
				break;
			}
		}
		#endregion

		#region Export Settings
		void DrawExportSettings ()
		{
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._MainFolder, AMEditorSystem.ContentCreateConfig._PluginVersionHelp), GUILayout.Width (300));
			GUILayout.BeginHorizontal ();
			pathToMainFolder = GUILayout.TextField (pathToMainFolder, GUILayout.Width (Screen.width - 42));
			if (GUI.Button (new Rect (GUILayoutUtility.GetLastRect ().x + GUILayoutUtility.GetLastRect ().width + 4, GUILayoutUtility.GetLastRect ().y, 
				30, GUILayoutUtility.GetLastRect ().height), "..."))
			{
				SelectPathToMainFolder ();
			}
			GUILayout.EndHorizontal ();
			EditorGUILayout.Space ();

			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._ToExportFolder, AMEditorSystem.ContentCreateConfig._ToExportFolderHelp), GUILayout.Width (300));
			GUILayout.BeginHorizontal ();
			pathToExport = GUILayout.TextField (pathToExport, GUILayout.Width (Screen.width - 42));
			if (GUI.Button (new Rect (GUILayoutUtility.GetLastRect ().x + GUILayoutUtility.GetLastRect ().width + 4, GUILayoutUtility.GetLastRect ().y, 
				30, GUILayoutUtility.GetLastRect ().height), "..."))
			{
				SelectPathToExport ();
			}
			GUILayout.EndHorizontal ();
			EditorGUILayout.Space ();

			isCreateConfig = GUILayout.Toggle (isCreateConfig, new GUIContent (AMEditorSystem.ContentCreateConfig._CreateConfigFile, AMEditorSystem.ContentCreateConfig._CreateConfigFileHelp));
			EditorGUILayout.Space ();

			GUI.enabled = (!pluginName.Equals (CUSTOM_CODE_PLUGIN_NAME));
			isCopyFiles = GUILayout.Toggle (isCopyFiles, new GUIContent (AMEditorSystem.ContentCreateConfig._CopyFiles, AMEditorSystem.ContentCreateConfig._CopyFilesHelp));
			EditorGUILayout.Space ();
			GUI.enabled = true;

			isUnityPackage = GUILayout.Toggle (isUnityPackage, new GUIContent (AMEditorSystem.ContentCreateConfig._CreateUnityPackage, AMEditorSystem.ContentCreateConfig._CreateUnityPackageHelp));
			EditorGUILayout.Space ();

			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._PackageBuildType, AMEditorSystem.ContentCreateConfig._PackageBuildTypeHelp), GUILayout.Width (300));
			GUILayout.BeginHorizontal (GUILayout.Width (250));
			GUILayout.Label (string.Empty, GUILayout.MinWidth (25));
			projectBuildTypeIndex = GUILayout.SelectionGrid (projectBuildTypeIndex, projectBuildTypes, 1, EditorStyles.radioButton);
			GUILayout.EndHorizontal ();
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._PackageBuildNumber, AMEditorSystem.ContentCreateConfig._PackageBuildNumberHelp), GUILayout.Width (300));
			projectBuildNumber = GUILayout.TextField (projectBuildNumber, GUILayout.Width (Screen.width/2-12));
			GUILayout.EndHorizontal ();
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent (AMEditorSystem.ContentCreateConfig._PackageExtraBuildOptions, AMEditorSystem.ContentCreateConfig._PackageExtraBuildOptionsHelp), GUILayout.Width (300));
			extraBuildOptions = GUILayout.TextField (extraBuildOptions, GUILayout.Width (Screen.width/2-12));
			GUILayout.EndHorizontal ();
			EditorGUILayout.Space ();

			GUILayout.Label ("", GUILayout.Height (1));
			GUI.Box (new Rect (-1, Screen.height - 38, Screen.width + 2, 40), "");

			#if AM_EDITOR_COMPACT_ON
				Rect buttomLeftButtonRect = new Rect (10, Screen.height - (buttonWidth + 7), buttonWidth + 2, buttonWidth);
				Rect buttomRightButtonRect = new Rect (Screen.width -  (buttonWidth + 12), Screen.height - (buttonWidth + 7), buttonWidth + 2, buttonWidth);
			#else
				Rect buttomLeftButtonRect = new Rect (10, Screen.height - 33, 150, 30);
				Rect buttomRightButtonRect = new Rect (Screen.width - 160, Screen.height - 33, 150, 30);
			#endif
			if (GUI.Button (buttomLeftButtonRect, backButtonContent))
			{
				ShowPluginSettings ();
			}

			if (GUI.Button (buttomRightButtonRect, createConfigButtonContent) || Event.current.Equals (Event.KeyboardEvent ("return")))
			{
				bool success = SaveConfig ();
				if (success) 
				{
					this.Close ();
					if (AMEditor.WindowMain.instance != null)
						AMEditor.WindowMain.instance.UpdateAllPluginsLists ();
				}
			}
		}

		static void ShowPluginSettings ()
		{
			currentWindow = TypeWindow.PluginSettings;
		}
		#endregion

		#region Create And Save
		static bool SaveConfig ()
		{
			try
			{
				string tempConfigText = AMEditorJSON.FormatJson (configText);
				configText = tempConfigText;
			}
			catch (Exception)
			{}
			if (pathToMainFolder == string.Empty)
			{
				if (EditorUtility.DisplayDialog (AMEditorSystem.ContentCreateConfig._EmptyMainFolderTitle, AMEditorSystem.ContentCreateConfig._EmptyMainFolderMessage, AMEditorSystem.ContentStandardButton._Ok))
				{
					SelectPathToMainFolder ();
				}
				return false;
			}
			try 
			{
				File.WriteAllText (pathToMainFolder + "/"+"plugin_config.json", configText);						
				exportFilesList.Add (pathToMainFolder + "/"+"plugin_config.json");
				exportFilesList.Sort ();
			}
			catch (Exception) 
			{
				return false;
			}
			if (pathToExport == string.Empty || pathToExport.Contains ("Assets"))
			{
				if (EditorUtility.DisplayDialog (AMEditorSystem.ContentCreateConfig._BadExportFolderTitle, AMEditorSystem.ContentCreateConfig._BadExportFolderMessage, AMEditorSystem.ContentStandardButton._Ok))
				{
					SelectPathToExport ();
				}
				return false;
			}
			if (isCreateConfig)
			{
				try 
				{
					AMEditorFileStorage.CreateFolder (pathToExport, false);
					File.WriteAllText (pathToExport + "/"+"plugin_config.json", configText);
				}
				catch (Exception) 
				{}
			}
			if (isCopyFiles)
			{
				for (int i = 0; i < exportFilesList.Count; i++)
				{
					string item = exportFilesList [i];
					if (AMEditorFileStorage.FileExist (item))
					{
						string pathToFile = item.Substring (item.IndexOf ("Assets"));
						AMEditorFileStorage.CopyFile (pathToFile, pathToExport +"/"+pathToFile, true);

						MetaFilesAPI.CopyMetafile (pathToFile + ".meta", pathToExport + "/" + pathToFile + ".meta");

						if (item.Contains (".bundle"))
						{
							string[] folders = item.Split (new char[]{'/'});
							string pathBundle = string.Empty;
							for (int j = 0; j < folders.Length; j++)
							{
								string f = folders [j];
								pathBundle += f;
								if (!f.Contains (".bundle"))
									pathBundle += "/";
								else
								{
									if (AMEditorFileStorage.FileExist (pathBundle + ".meta"))
									{
										MetaFilesAPI.CopyMetafile (pathBundle + ".meta", pathToExport + "/" + pathBundle + ".meta");
									}
									break;
								}
							}
						}
					}
				}
			}
			if (isUnityPackage)
			{
				if (pluginName.Equals (CUSTOM_CODE_PLUGIN_NAME))
				{
					exportFilesList = CollectCustomCodeFiles ();
				}

				List<string> exportFiles = new List<string> ();
				for (int i = 0; i < exportFilesList.Count; i++)
				{
					string item = exportFilesList [i];
					exportFiles.Add (item);
					exportFiles.Add (item + ".meta");
				}
				exportFilesList.Sort ();
				CreateUnityPackage (exportFiles);
			}
			if (!isUnityPackage)
				EditorUtility.OpenWithDefaultApp (pathToExport);
			return true;
		}

		public static List<string> CollectCustomCodeFiles ()
		{
			bool result = false;

			string pathToLocalRepo = AMEditor.LocalRepositoryAPI.pathToRepository + AMEditorSystem.FolderNames._LocalRepository;

			var cc = new Plugin (AMEditorJSON.JsonDecode (configText) as Hashtable);
			foreach (var item in cc.depends)
			{

			}

			////////

			List<Plugin> newPlugins = new List<Plugin> ();
			List<Plugin> otherPlugins = new List<Plugin> ();

			AMEditor.WindowMain.SilentCheckUpdate ();

			ArrayList currentPluginsList = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AMEditor.WindowMain.AMEDITOR_CONFIG_FILENAME)) as ArrayList;

			foreach (var item in currentPluginsList) 
			{
				Plugin pluginItem = new Plugin (item as Hashtable);
				if (pluginItem.errors.oldVersion || pluginItem.errors.needUpdate)
				{
					newPlugins.Add (pluginItem);
				}
				else if (!pluginItem.errors.conflict && !pluginItem.errors.dependPlugins && !pluginItem.errors.missingFiles &&
					!pluginItem.errors.missingFilesHash && !pluginItem.errors.needUpdate && !pluginItem.errors.oldVersion)
				{
					otherPlugins.Add (pluginItem);
				}
				else
				{
					EditorUtility.DisplayDialog ("Error!", "Plugins have some errors!", AMEditorSystem.ContentStandardButton._Ok);
				}
			}

			//- для не требующих апдейта поиск конфигов в локальном репозитории и сравнение их версий с актуальными

			//- подходящие конфиги копируем в проект

			//- при нехватке актуальных конфигов - дергаем их с гита, без файлов, сразу в проект

			//- собираем по этим конфигам файлы в пак

			return new List<string> ();
		}

		static void SelectPathToMainFolder ()
		{
			pathToMainFolder = EditorUtility.OpenFolderPanel (AMEditorSystem.ContentCreateConfig._OutFolder, pathToMainFolder, "");
			if (pathToMainFolder.Contains ("Assets"))
				pathToMainFolder = pathToMainFolder.Substring (pathToMainFolder.IndexOf ("Assets"));
		}
		static void SelectPathToExport ()
		{
			pathToExport = EditorUtility.OpenFolderPanel (AMEditorSystem.ContentCreateConfig._OutFolder, pathToExport, "");
		}

		static void CreateUnityPackage (List<string> files)
		{
			string name = pluginName.Replace (" ", "_") + "_" + pluginVersion.Replace (" ", "_");
			if (projectBuildTypeIndex != 0) 
			{
				name += "_" + projectBuildTypes[projectBuildTypeIndex];
			}
			if (projectBuildNumber != string.Empty) 
			{
				name += projectBuildNumber;
			}
			if (extraBuildOptions != string.Empty) 
			{
				name += "+" + extraBuildOptions;
			}
			name += ".unitypackage";

			string[] filesPathes = files.ToArray ();
			string unitypackageFullPath = pathToExport + "/" + name;

			AssetDatabase.ExportPackage (filesPathes, unitypackageFullPath, ExportPackageOptions.Interactive);
		}

		static void ShowSelectedWindow ()
		{
			configText = CreateConfigFile ();

			pathToExport = "Assets";

			string candidatepath = "Assets/AMPlugins/" + pluginName.Replace (" ", "");
			if (AMEditorFileStorage.ExistFolder (candidatepath)) 
			{
				pathToMainFolder = candidatepath;
			}
			else
			{
				candidatepath = "Assets/AMPlugins/Editor/" + pluginName.Replace (" ", "");
				if (AMEditorFileStorage.ExistFolder (candidatepath)) 
				{
					pathToMainFolder = candidatepath;
				}
				else
				{
					pathToMainFolder = "Assets";
				}
			}
			isUnityPackage = assetsFolder.GetSelectedFiles ().Count > 0;
			isCopyFiles = assetsFolder.GetSelectedFiles ().Count > 0;

			currentWindow = TypeWindow.ExportSettings;
		}

		static string CreateConfigFile ()
		{
			Hashtable result = new Hashtable ();
			ArrayList depend = new ArrayList ();
			exportFilesList = new List<string> ();

			for (int i = 0; i < pluginDepends.Count; i++)
			{
				PluginDepend item = pluginDepends [i];
				Hashtable tmp = new Hashtable ();
				tmp.Add ("name", item.name);
				tmp.Add ("version", item.version);
				tmp.Add ("mod", item.mod);
				depend.Add (tmp);
			}
			result.Add ("depends", depend);

			ArrayList files = new ArrayList ();
			List<Hashtable> currentSelected = assetsFolder.GetSelectedFiles ();
			for (int i = 0; i < currentSelected.Count; i++)
			{
				Hashtable item = currentSelected [i];
				files.Add (item);
				exportFilesList.Add (new ProjectFile (item).fullPath);
			}
			for (int i = 0; i < outdatedFiles.Count; i++)
			{
				ProjectFile item = outdatedFiles [i];
				files.Add (item.ToHashTable ());
			}
			result.Add ("files", files);

			ArrayList old_names = new ArrayList ();
			for (int i = 0; i < pluginOldNamesList.Count; i++)
			{
				string item = pluginOldNamesList [i];
				old_names.Add (item);
			}
			result.Add ("old_names", old_names);

			result.Add ("name", pluginName);
			result.Add ("version", pluginVersion);
			result.Add ("mandatory", isMandatory);
			result.Add ("display_type", pluginViewTypeString);

			ArrayList pluginBuildTypes = new ArrayList ();
			string[] pluginBuildTypesArray = pluginBuildTypesString.Split (new string[]{" "}, System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < pluginBuildTypesArray.Length; i++)
			{
				pluginBuildTypes.Add (pluginBuildTypesArray[i]);
			}
			result.Add ("build_types", pluginBuildTypes);
			result.Add ("url", pluginUrl);

			return AMEditorJSON.JsonEncode (result);
		}
		#endregion

		void OnDestroy ()
		{
			if (dontShowDuplicateDialog)
				AMEditorDefineController.AddDefine (hideDuplicatesDialogDefine);

			firstShow = false;
			pluginOldNamesList = null;
			coreFilesList = null;
			examplesFilesList = null;
			modifiableFilesList = null;
			dragAndDropList = null;
			duplicateFilesList = null;
			outdatedFiles = null;
			ignoreFiles = null;
			ignoreFolders = null;
			pluginDepends = null;
			dependModsList = null;
			pluginViewTypesList = null;
			allAssetsPaths = null;
			pluginUrl = string.Empty;
			pluginName = string.Empty;
			pluginOldName = string.Empty;
			pluginVersion = string.Empty;
			pluginBuildTypesString = string.Empty;
			isMandatory = false;
			currentWindow = TypeWindow.PluginSettings;
		}
	}
}
#endif