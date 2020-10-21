#if UNITY_EDITOR
#pragma warning disable
using UnityEngine;
using System;

namespace AMEditor
{
	public class AMEditorSystem 
	{
		public class Links
		{
			#if AM_EDITOR_PLUGINS_TYPE_DEV
			public static string PublicAboutConfig = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/dev/about_config.json";
			public static string PublicAMEditorConfig = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/dev/ameditor_plugins.json";
			public static string PublicAMEditorExternalPlugins = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/dev/ameditor_external_plugins.json";
			public static string PublicAMEditorWhitelist = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/dev/ameditor_whitelist.json";
			#elif AM_EDITOR_PLUGINS_TYPE_RC
			public static string PublicAboutConfig = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/rc/about_config.json";
			public static string PublicAMEditorConfig = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/rc/ameditor_plugins.json";
			public static string PublicAMEditorExternalPlugins = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/rc/ameditor_external_plugins.json";
			public static string PublicAMEditorWhitelist = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/rc/ameditor_whitelist.json";
			#else
			//AM_EDITOR_PLUGINS_TYPE_RELEASE
			public static string PublicAboutConfig = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/master/about_config.json";
			public static string PublicAMEditorConfig = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/master/ameditor_plugins.json";
			public static string PublicAMEditorExternalPlugins = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/master/ameditor_external_plugins.json";
			public static string PublicAMEditorWhitelist = "http://pgit.digital-ecosystems.ru/unity-plugins/am-editor-plugins/raw/master/ameditor_whitelist.json";
			#endif
		}

		public class FileNames
		{
			public static string _ConfigAboutMenu = "about_config.json";
			public static string _ConfigAMEditor = "Assets/AMPlugins/Editor/AMEditor/ameditor_config.json";
			public static string _Account = "accounts.json";
			public static string _NeedDisplaySucces = "display_dialog_success.json";
			public static string _ConfigPlugin = "plugin_config.json";
			public static string _WhiteList = "ameditor_whitelist.json";
			#if AM_EDITOR_PLUGINS_TYPE_DEV
			public static string _ActualPlugins = "ameditor_plugins_dev.json";
			public static string _ExternalPlugins = "ameditor_external_plugins_dev.json";
			#elif AM_EDITOR_PLUGINS_TYPE_RC
			public static string _ActualPlugins = "ameditor_plugins_rc.json";
			public static string _ExternalPlugins = "ameditor_external_plugins_rc.json";
			#else
			//AM_EDITOR_PLUGINS_TYPE_RELEASE
			public static string _ActualPlugins = "ameditor_plugins.json";
			public static string _ExternalPlugins = "ameditor_external_plugins.json";			
			#endif
		}

		public class FolderNames
		{
			#if AM_EDITOR_PLUGINS_TYPE_DEV
			public static string _LocalRepository = "Downloads_dev";
			#elif AM_EDITOR_PLUGINS_TYPE_RC
			public static string _LocalRepository = "Downloads_rc";
			#else
			//AM_EDITOR_PLUGINS_TYPE_RELEASE
			public static string _LocalRepository = "Downloads";
			#endif

		}

		public class ContentAMEditorInfo
		{
#if AM_EDITOR_LANGUAGE_EN
			public static string _Language = "Language: English";
			public static string _PluginsTypeLabel = "Plugins type: ";
			public static string _ViewTypeLabel = "View: ";
			#if AM_EDITOR_PLUGINS_TYPE_DEV
			public static string _PluginsType = "Develop";
			#elif AM_EDITOR_PLUGINS_TYPE_RC
			public static string _PluginsType = "RC";
			#else
			//AM_EDITOR_PLUGINS_TYPE_RELEASE
			public static string _PluginsType = "Release";
			#endif
			#if AM_EDITOR_VIEW_TYPE_EXTENDED
			public static string _ViewType = "Extended";
			#else
			//AM_EDITOR_VIEW_TYPE_MINIMAL
			public static string _ViewType = "Standard";
			#endif
			#if AM_EDITOR_COMPACT_ON
			public static string _CompactViewType = ". Compact";
			#else
			public static string _CompactViewType = "";
			#endif
#else
			public static string _Language = "Язык: Русский";
			public static string _PluginsTypeLabel = "Тип плагинов: ";
			public static string _ViewTypeLabel = "Вид: ";
			#if AM_EDITOR_PLUGINS_TYPE_DEV
			public static string _PluginsType = "Develop";
			#elif AM_EDITOR_PLUGINS_TYPE_RC
			public static string _PluginsType = "RC";
			#else
			//AM_EDITOR_PLUGINS_TYPE_RELEASE
			public static string _PluginsType = "Release";
			#endif
			#if AM_EDITOR_VIEW_TYPE_EXTENDED
			public static string _ViewType = "Расширенный";
			#else
			//AM_EDITOR_VIEW_TYPE_MINIMAL
			public static string _ViewType = "Стандартный";
			#endif
			#if AM_EDITOR_COMPACT_ON
			public static string _CompactViewType = ". Компактный";
			#else
			public static string _CompactViewType = "";
			#endif
#endif
		}

		public class Git
		{
			public static string _GroupName = "Unity Plugins";
			#if AM_EDITOR_PLUGINS_TYPE_DEV
			public static string _BranchName = "develop";
			#elif AM_EDITOR_PLUGINS_TYPE_RC
			public static string _BranchName = "release";
			#else
			//AM_EDITOR_PLUGINS_TYPE_RELEASE
			public static string _BranchName = "master";
			#endif
		}

		public class Paths
		{
			
		}

		public class ContentMenuViewType
		{
#if AM_EDITOR_LANGUAGE_EN
			public const string _ViewTypeMinimal = "AM Editor/Settings/View/Standard";
			public const string _ViewTypeExtended = "AM Editor/Settings/View/Extended";

			public const string _ViewTypeCompact = "AM Editor/Settings/View/Compact";
			public const string _ViewTypeCompactOn = "AM Editor/Settings/View/Compact/On";
			public const string _ViewTypeCompactOff = "AM Editor/Settings/View/Compact/Off";
#else
			public const string _ViewTypeMinimal = "AM Editor/Настройки/Вид/Стандартный";
			public const string _ViewTypeExtended = "AM Editor/Настройки/Вид/Расширенный";

			public const string _ViewTypeCompact = "AM Editor/Настройки/Вид/Компактный";
			public const string _ViewTypeCompactOn = "AM Editor/Настройки/Вид/Компактный/Вкл.";
			public const string _ViewTypeCompactOff = "AM Editor/Настройки/Вид/Компактный/Выкл.";
#endif
		}
		public class ContentMenuPluginsType
		{
#if AM_EDITOR_LANGUAGE_EN
			public const string _Dev = "AM Editor/Settings/Plugins type/Develop";
			public const string _RC = "AM Editor/Settings/Plugins type/RC";
			public const string _Release = "AM Editor/Settings/Plugins type/Release";
#else
			public const string _Dev = "AM Editor/Настройки/Тип плагинов/Develop";
			public const string _RC = "AM Editor/Настройки/Тип плагинов/RC";
			public const string _Release = "AM Editor/Настройки/Тип плагинов/Release";
#endif
		}

		public class ContentDebugMode
		{
#if AM_EDITOR_LANGUAGE_EN
			public const string _DebugMode = "AM Editor/Settings/Debug-mode";
			public const string _DebugModeOn = "AM Editor/Settings/Debug-mode/On";
			public const string _DebugModeOff = "AM Editor/Settings/Debug-mode/Off";
#else
			public const string _DebugMode = "AM Editor/Настройки/Debug-режим";
			public const string _DebugModeOn = "AM Editor/Настройки/Debug-режим/Вкл.";
			public const string _DebugModeOff = "AM Editor/Настройки/Debug-режим/Выкл.";
#endif
		}

#if AM_EDITOR_LANGUAGE_EN
		public class ContentMenuItem
		{
			public const string _ShowAMEditor =  "AM Editor/Show AM Editor";
			public const string _CheckUpdate =  "AM Editor/Check for updates";
			public const string _ShowAMStore = "AM Editor/Open AM Store";
			public const string _RestoreCRM =  "AM Editor/Restore Custom Resources Manager";
			public const string _FixMetaALibs =  "AM Editor/Fix .a files";
			public const string _FixMetaBundle =  "AM Editor/Fix .bundle files";
			public const string _MakeAssetBundle = "AM Editor/Make an Asset Bundle/For current platform";
			public const string _MakeAssetBundleForAll = "AM Editor/Make an Asset Bundle/For all platforms";			

			public const string _HelpWiki = "AM Editor/Help/Wiki";
			public const string _HelpSupport = "AM Editor/Help/Support";
			public const string _HelpChangeLog = "AM Editor/Help/Change log";
			
			public const string _WindowCreateConfigFile = "AM Editor/For developers/Create plugin_config.json";
			public const string _WindowCreateConfigALL = "AM Editor/For developers/Create ameditor_plugins.json";

            public const string _SetDefaultsForCustomQualitySetup = "AM Editor/For developers/Set defaults for Custom Quality Setup";
			
			public const string _LanguageEN = "AM Editor/Settings/Language/EN";
			public const string _LanguageRU = "AM Editor/Settings/Language/RU";

			public const string _Backups = "AM Editor/Backups";
		}

        public class ContentCustomQualitySetup
        {
            public static string _TitleDialog = "Loading default settings";
            public static string _MessageSuccessDialog = "Default settings for Custom Quality Setup loaded successfully";
        }

		public class ContentMenuAbout
		{
			public static string _GetLinks = "Get about_config.json";
		}
		
		public class ContentPluginsType
		{
			public static string _TitleDialog = "Changing plugins type";
			public static string _MessageSuccessDialog = "Plugins type sucessfully changed";
		}

		public class ContentViewType
		{
			public static string _TitleDialog = "Changing AM Editor's view";
			public static string _MessageSuccessDialog = "AM Editor's view sucessfully changed";
			public static string _ChangeViewTypeQuestion = "Are you sure you want to change current AM Editor's view?";
			public static string _SwitchCompactModeMessage ()
			{
				string actionType;
				#if AM_EDITOR_COMPACT_ON
				actionType = "off";
				#else
				actionType = "on";
				#endif
				return "\n\nTo turn compact mode "+actionType+" AM Editor will be restarted";
			}
		}
		public class ContentVisibleMeta
		{
			public const string _FailedMakeVisibleTitle = "Error accessing meta files";
			public const string _FailedMakeVisible = "Failed to change the version control mode to \"Visible Meta Files\"\nTry changing the mode manually";
			public const string _YouKnowWhatToDo = "I know what to do";
			public const string _ShowMe = "Show me the settings";
		}
		public class ContentFixMetaA
		{
			public const string _SuccessFixTitle = ".a files successfully fixed";			
			public const string _SuccessFix = "All .a files have been fixed";
			public const string _FailedFixTitle = "Error fix .a files";
			public const string _FailedFix = "There were some errors during the fix";
			public const string _RepeatFix = "Repeat fix";
		}
		public class ContentFixMetaBundle
		{
			public const string _SuccessFix = "All .bundle files have been fixed";
			public const string _SuccessFixTitle = ".bundle files successfully fixed";
			public const string _FailedFix = "There were some errors during the fix";
			public const string _FailedFixTitle = "Error fix .bundle files";
			public const string _RepeatFix = "Repeat fix";
		}
		public class ContentAssetBundles
		{
			public const string _DialogTitle = "Asset Bundle Creation";
			public const string _DialogMessage = "You are going to create an Asset Bundle.\n\nAll of your active scenes, but the first will be placed to the Asset Bundle and disabled in the Build Settings for later download from the bundle.";
			public const string _Continue = "Continue";
			public const string _No = "No, I'll do it myself";
			public const string _SuccessCreationMessage = "Asset Bundle successfully created at StreamingAssets folder\n\nDon't forget to check an asset bundles loading settings at the CustomResourcesManager prefab";
			public const string _ScenesNotFoundMessage = "Enabled scenes not found in Build Settings!";
			public const string _NotEnoughScenesMessage = "Number of enabled scenes in Build Settings must be greater than 1 to create an asset bundle";
			public const string _ErrorMessage = "Failed to create an AssetBundle for the following platforms:\n";
		}

		public class ContentMenuLanguage
		{
			public static string _TitleDialog = "Change language";
			public static string _MessageSuccessDialog = "Success change language AM Editor";
			//public static string _MessageFailedDialog = "Failed change language AM Editor";
		}

		public class ContentDeleteExample
		{
			public const string _Title = "Deleting example";
			public const string _Message = "Are you sure you want to delete an example file?";
		}

		public class ContentMenuLocalRepository
		{
			public const string _Open = "AM Editor/Local storage/Open";
			public const string _Clean = "AM Editor/Local storage/Delete the downloaded plugins";
			
			public static string _DeleteTitle = "Deleting files from a local storage";
			public static string _DeleteQuestion = "Are you sure you want to delete the downloaded files of plugins from the local storage?";
			public static string _SuccessMessage = "Removal was successful.";
			public static string _FailureMessage = "Unable to completely clear the local storage.";
			public const string _OpenButton = "Open";
		}
		
		public class ContentWindowAuthorization
		{
			public static string _ShowPassword = "Show password";
			public static string _HidePassword = "Hide password";
			public static string _Or = "or";
		}
		public class ContentWindowConflict
		{
			public static string _ContentOutdatedMessage = "file from the older versions of the plugin";
			public static string _ContentPossibleConflist = "possible conflict with the plugin files";
			public static string _ContentQuestionDeleteFiles = "Are you sure you want to delete files?";
			public static string _ContentQuestionDeleteSelectedFilesFor (string pluginName)
			{
				return "Are you sure you want to delete selected files conflicting with " + pluginName + " ?";
			}
			public static string _ContentQuestionDeleteAllSelectedFiles = "Are you sure you want to delete ALL selected files ?";
		}
		public class ContentFixConflicts
		{
			public static string _SuccessFixConflictTitle = "Conflicts autofix success";
			public static string _SuccessFixConflict = "Conflicts have been successfully fixed. Conflict files moved to backups";
			public static string _FailedFixConflictTitle = "Conflicts autofix failed";
			public static string _FailedFixConflict = "Any errors during the autofix conflicts. Would you like to go to the manual fix?";
		}
		public class ContentWindowChangeLog
		{
			public static string _Title = "About AM Editor";
		}
		public class ContentTexure
		{
			public static string _Ok =  "Ok";
			public static string _OkHelp (string pluginName)
			{
				return "Update for " + pluginName + " plugin is up to date";
			}
			
			public static string _NeedCheck =  "Checking required";
			public static string _NeedCheckHelp = "Check update for the plugin is required ";
			
			public static string _Fixme =  "some problems with plugin";
			public static string _FixmeHelp = "fix me ";
			
			public static string _NeedUpdate =  "Requires download the plugin";
			public static string _NeedUpdateHelp (string pluginName)
			{
				return "Update for " + pluginName + " plugin is required. Download the latest version";
			}

			public static string _NotLatestVersion = "Not the latest version of the plugin";
			public static string _NotLatestVersionHelp (string pluginName)
			{
				return "Current version of " + pluginName + " plugin is not the latest. Select the required version from the list and download, if necessary";
			}

			public static string _VersionNotForBuildType = "Version of the plugin is not compatible with the build type";
			public static string _VersionNotForBuildTypeHelp (string pluginName)
			{
				return "Current version of " + pluginName + " plugin is not compatible with selected build type. Download the required version or delete the plugin";
			}

			public static string _SelecdtedAnotherVersion =  "The selected version doesn't match with installed";
			public static string _SelecdtedAnotherVersionHelp (string pluginName)
			{
				return  "The selected version of the " + pluginName + " plugin doesn't match with installed version";
			}
		}
		public class ContentPlugin
		{
			public static string _Logout = "Change account";
			public static string _LogoutHelp = "Change account";

			public static string _LogoutDialogTitle = "Change account";
			public static string _LogoutDialogMessage = "Are you sure you want to change current account?";
			
			public static string _CurrentAccount = "Current account: ";
			public static string _CurrentAccountHelp = "Current account";

			public static string _PluginsBuildTypes = "Plugins build type ";
			public static string _PluginsBuildTypesHelp = "The types of builds that may involve plugins";

			public static string _DownloadForBuildType = "Download";
			public static string _DownloadForBuildTypeHelp = "Download compatible plugins for selected build type";
			
			public static string _ConnectedPluginsTitle = "Connected plugins";
			public static string _ConnectedPluginsTitleHelp = "Plugins connected to the project";

			public static string _PluginName = "Plugin name";
			public static string _PluginNameHelp = "Plugin name in GitLab";
			
			public static string _CurrentVersion = "Current version";
			public static string _CurrentVersionHelp = "Version of the connected plugin";
			
			public static string _ActualVersion = "Actual version";
			public static string _ActualVersionHelp = "The latest available version of the plugin";
			
			public static string _UnconnectedPluginsTitle = "Unconnected plugins";
			public static string _UnconnectedPluginsTitleHelp = "Available to connect plugins";

			public static string _ExternalPluginsTitle = "External plugins";
			public static string _ExternalPluginsTitleHelp = "External plugins";
			public static string _ExternalPluginsUnavailable = "(You do not a member in Unity Plugins group!)";
			public static string _ExternalPluginsUnavailableTitleDialog = "Error access to Unity Plugins group";
			public static string _ExternalPluginsUnavailableMessage = "To download external plugins is necessery to consist in Unity Plugins group. Please refer to your teamlead or to the head of your department";

			public static string _SearchTitle = "Search";
			public static string _SearchPlaceholder = "Plugin name...";

			public static string _MandatoryPluginsTitle = "Necessary to connect plugins";
			public static string _MandatoryPluginsTitleHelp = "Necessary to connect plugins";
			
			public static string _Selected = "Selected";
			public static string _SelectedHelp = "Selected";
						
			public static string _Download = "Download";
			public static string _DownloadHelp = "Download ";

			public static string _Update = "Update";
			
			public static string _Delete = "Delete";
			public static string _DeleteHelp = "Delete the plugin ";

			public static string _ExamplesInfoHelp (string button)
			{
				return "The plugin has example files.\nPress the " + button + " button for the plugin to interaction";
			}

			public static string _IncorrectDownloadUrl = "Icorrect localgit url";
			
			public static string _ManualLink = "GitLab";
			public static string _ManualLinkHelp (string pluginName)
			{
				return "Manually download " + pluginName + " plugin";
			}

			public static string _Description = "Description";
			public static string _DescriptionHelp = "Open plugin description";

			public static string _Check = "Check";
			public static string _CheckHelp = "Check for plugin update ";
			
			public static string _CheckForce = "Check hash";
			public static string _CheckForceHelp = "Check hash"+Environment.NewLine+"Help";
			
			public static string _Version= "";
			public static string _VersionHelp = "";
			
			public static string _CheckSelected = "Check selected ";
			public static string _CheckSelectedHelp = "Check for updates at the selected plugins";
			
			public static string _DownloadSelected = "Download selected ";
			public static string _DownloadSelectedHelp = "Download updates from the selected plugins";
			public static string _DownloadSelectedUnconnectHelp = "Download selected plugins";
			
			public static string _DeleteSelected = "Delete selected ";
			public static string _DeleteSelectedHelp = "Delete selected plugins";
		}
        public class ContentExternalPlugin
        {
			public static string _SuccessDownloadMessage (string pluginName)
			{
				return pluginName + " plugin has been successfully downloaded";
			}
			public static string _SuccessImportMessage (string pluginName)
			{
				return pluginName + " plugin has been successfully imported to the project";
			}
			public static string _FailedImportMessage (string pluginName)
			{
				return "Importing " + pluginName + " to the project failed. Try to import with \"Assets/Import Package\"";
			}
			public static string _FailedDownloadMessage (string pluginName)
			{
				return "Downloading " + pluginName + " plugin failed. Try again";
			}
        }
		public class ContentMakeAllGoodButton
		{
			public static string _AllGood = "Everything is OK";
			public static string _AllGoodHelp = "Everything is fine with plugins";
			public static string _BuildTypeProblems = "Change build type";
			public static string _BuildTypeProblemsHelp = "Download compatible plugins for selected build type";
			public static string _MissingFiles = "Download missing files";
			public static string _MissingFilesHelp = "Download missing files of plugins";
			public static string _HasConflicts = "Fix conflicts";
			public static string _HasConflictsHelp = "Some files has conflicts. Fix conflicts";
			public static string _ChangedFiles = "Update changed files";
			public static string _ChangedFilesHelp = "Download changel files of plugins";
			public static string _NeedDepends = "Download dependent plugins";
			public static string _NeedDependsHelp = "Download dependent plugins for installed plugins";
			public static string _NeedUpdate = "Update plugins";
			public static string _NeedUpdateHelp = "Download actual versions for plugins";
			public static string _NeedMandatory = "Download mandatory plugins";
			public static string _NeedMandatoryHelp = "Download missing mandatory plugins";
			public static string _OutdatedPlugins = "Remove outdated plugins";
			public static string _OutdatedPluginsHelp = "Outdated plugins found, it's recommended to remove";
		}
		public class ContentStandardButton
		{
			public static string _Yes = "Yes";
			public static string _YesHelp = "Yes";
			
			public static string _Ok = " Ok";
			public static string _OkHelp = "Ok";
			
			public static string _More = "More...";
			public static string _MoreHelp = "More";
			
			public static string _No = "No";
			public static string _NoHelp = "No";
			
			public static string _Apply = "Apply";
			public static string _ApplyHelp = "Apply";
			
			public static string _Cancel = "Cancel";
			public static string _CancelHelp = "Cancel";

			public static string _Add = "Add";	
			public static string _AddHelp = "Add";
			
			public static string _Delete = "Delete";
			public static string _DeleteHelp = "Delete";
			
			public static string _None = "None";
			public static string _NoneHelp = "None";
			
			public static string _All = "All";
			public static string _AllHelp = "All";

			public static string _OnlyCurrent = "Current";
			public static string _OnlyCurrentHelp = "Delete only ";

			public static string _WithDepends = "With depends";
			public static string _WithDependsHelp (string pluginName)
			{
				return "Delete " + pluginName + " plugin with all depends";
			}

			public static string _ApplyForAll = "For all";
			public static string _ApplyForCurrent = "Only current";

			public static string _DontShowAgain = "Don't show again";
			public static string _DontShowAgainHelp = "This message will not appear in the project"+Environment.NewLine+"(it may need to restart the window)";

			public static string _ShowAMEditor = "Show AM Editor";
		}
		public class ContentBackup
		{
			public static string _ButtonBackups = "Backups";
			public static string _ButtonBackupsHelp = "Backup copies of deleted files in the fix of conflicts";
			
			public static string _TitleWindowBackup = "Backups";
			public static string _TitleWindowBackupHelp = "Backups ";
		}
		public class ContentProblems
		{
			public static string _Fix = "Fix conflicts ";
			public static string _FixHelp = "Fix conflicts ";
			
			public static string _ErrorMissingFiles = " - Lacking plugin files. Download the plugin";
			public static string _ErrorMissingFilesHelp = " - Lacking plugin files. Download the plugin";
			
			public static string _ErrorMissingFilesHash = " - Plugin files was changed. Download the plugin";
			public static string _ErrorMissingFilesHashHelp = " - Plugin files was changed. Download the plugin";
			
			public static string _ErrorConflictFiles = " - There is a conflict with this plugin";
			public static string _ErrorConflictFilesHelp = " - There is a conflict with this plugin";

			public static string _ErrorDependVersion = "Version error";
			public static string _ErrorDependVersionNotSupported = " is not supported by AM Editor."+Environment.NewLine+"Contact support";
			public static string _ErrorDependVersionIsOld = " is not supported by AM Editor, because it's not actual."+Environment.NewLine+"You can download the actual version";
			
			public static string _Depends = "Dependencies: ";
			public static string _DependsHelp = "To use the plugin you have to load ";

			public static string _CustomCodeDependConflict = "- There is a conflict with ";
			public static string _CustomCodeDependConflictHelp = "There is a conflict with the plugin included in the Сustom Code";
			public static string _CustomCodeDependMissingFiles = "- Lacking files of ";
			public static string _CustomCodeDependMissingFilesHelp = "Lacking files of the plugin included in the Custom Code";
			public static string _CustomCodeDependMissingHash (string pluginName)
			{
				return "- Files of the " + pluginName + " plugin was changed";
			}
			public static string _CustomCodeDependMissingHashHelp = "Files of the plugin included in the Custom Code was changed";
		}
		public class ContentProgressBar
		{
			public static string _NeedUpdate = "Update requires ";

			public static string _UpdateCustomCode = ". Please update the Custom Code plugin";

			public static string _NeedDownload = "Update requires ";
			public static string _NeedDownloadFor = " for the following plugins: ";
			public static string _NeedUpdateHelp = "Update requires ";

			public static string _NotLatestVersion = "Not the latest version of ";

			public static string _NotForBuildTypeCC = "The current version of the Custom Code plugin is incompatible with build type ";
			public static string _NotForBuildType = " is incompatible with build type ";
			
			public static string _Update = "Update";
			public static string _NoUpdate = "Plugins are already up to date";
			public static string _UpdateHelp = "Update ";

			public static string _ConfigPrepare = "Preparing ameditor_plugins.json...";
			public static string _ConfigPrepareHelp = "Preparing ameditor_plugins.json";

			public static string _PushingConfig = "Pushing ameditor_plugins.json to the Git repo...";

            public static string _DefaultsForCustomQualitySetup = "Default settings for Custom Quality Setup";

			public static string _LaunchingAMEditorTitle = "Launching AM Editor";
			public static string _LaunchingAMEditorMessage = "Launching AM Editor. Please wait...";
			
			public static string _Checking = "Checking";
			public static string _CheckingPublicRepo = "Checking public repository";
			public static string _CheckingUpdatePlugins = "Checking plugin updates";
			public static string _UpdateList = "Updating the list";
			public static string _CheckingUpdate = "Checking for updates";

			public static string _GettingAssetsList = "Getting assets list";
			
			public static string _Plugin = " ";
			
			public static string _DownloadTo = "Downloading ";
			
			public static string _SuccessUpdateMessage = "Successfully downloaded ";
			
			public static string _FailedUpdateTitle = "Error downloading ";
			public static string _FailedUpdateMessage = "An error has occurred. Try again";
			
			public static string _CancelUpdateTitle = "Cancel download ";
			public static string _CancelUpdateMessage = "Downloading canceled by user";
			
			public static string _ForceCheck = "Updating";
			public static string _ForceCheckHelp = "Updating ";
			
			public static string _AutoFix = " Automatic conflict fix ";
			public static string _AutoFixHelp = " Automatic conflict fix";

			public static string _CheckingLocalRepo = "Checking local repo";
			public static string _CheckingLocalRepoHelp = "Checking local repo";
			
			public static string _CopyFiles = " Copying files to the project ";
			public static string _CopyFilesHelp = " Copying files to the project ";
			
			public static string _IsCompiling = "Compiling";
		}
		public class ContentOtherWindow
		{
			public static string _TitleWait = "Waiting";
			public static string _TitleCriticalError = "Critical Error";
			public static string _TitleErrorPopup = "Error ";
			public static string _TitleWarning = "Warning!";
			public static string _WebPlayerMessage = "The current platform is Web Player\nPlease switch the platform for correct plugins work";
			public static string _InvalidProductNameSymbolsMessage = "Invalid symbols at Product Name detected!\nFix it automatically?";
			public static string _NonLatinProductNameSymbolsMessage = "Product Name filed must include latin symbols only!";
			public static string _CompileWaitMessage = "Unity is compiling scripts. Please wait...";
			public static string _RestoringCRMWaitMessage = "Restoring Custom Resources Manager components...";
			public static string _CloseOnBisyMessage = "Loading plugins is in progress\nTo close the AM editor, first cancel the download!";
		}
		public class ContentAMStoreWindow
		{
			public static string _Title = "AM Store";
			public static string _Search = "Search";
			public static string _SearchButtonHelp = "Go Search!";
			public static string _SearchPlaceholder = "Search...";
			public static string _AssetName = "Name";
			public static string _AssetNameHelp = "Asset name";
			public static string _AssetVersion = "Version";
			public static string _AssetVersionHelp = "Asset version";
		}
		public class ContentAuth
		{
			public static string _Title = "Authorization";
			
			public static string _CurrentAccount = "Current account";
			public static string _CurrentAccountHelp = "Current account";
			
			public static string _Login = "Login";
			public static string _LoginHelp = "Your LDAP login";
			
			public static string _Password = "Password";
			public static string _PasswordHelp = "Your LDAP password";
			
			public static string _PrivateToken = "Private Token";
			public static string _PrivateTokenHelp = "Located in the account settings of localgit";
			
			public static string _Auth = "Log In";
			public static string _AuthHelp = "Authorization";		
		}
		public class WebError
		{
			public static string _100 = "GitLab unavailabe";
			public static string _101 = "Authorization error";
			public static string _102 (string plugin)
			{ 
				return plugin + " repository error ";
			}
			public static string _103 (string plugin, string branch)
			{ 
				return plugin + " repository error ";
			}
			public static string _104 (string plugin)
			{ 
				return plugin + " repository error ";
			}
			public static string _105 (string plugin)
			{
				return plugin + " repository error ";
			}
			public static string _106 (string url)
			{
				return "Error repository project " + url;
			}
			public static string _107 (string url)
			{ 
				return "No access to " + url;
			}
			//public static string _108 = "Ошибка не поддерживаемого запроса";
			//public static string _109 = "Ошибка параметров запроса";
			public static string _110 = "GitLab unavailable";
			public static string _111 = "NotSupportedException. Request format error";
			public static string _112 = "ArgumentNullException. Request parameters error";
			public static string _113 = "SecurityException. User permission error";
			public static string _114 = "UriFormatException. URI format error";
			public static string _115 = "NotImplementedException. Request method error";
			public static string _116 = "OutOfMemoryException. Memory error";
			public static string _117 = "IOException. I/O error";
			public static string _118 = "Private token error";
			public static string _119 = "Update checking error";
			public static string _120 = "Update checking error";
		}
		public class FileSystemError
		{
			public static string _200 (string plugin)
			{ 
				return "Unable to extract an archive of " + plugin + " plugin ";
			}
			public static string _201 (string plugin)
			{ 
				return "Unable to delete an archive of " + plugin + " plugin ";
			}
			public static string _202 (string path)
			{ 
				return "Unable to create " + path + " plugin directory ";
			}
			public static string _203 (string path)
			{ 
				return "Unable to delete " + path + " plugin directory ";
			}
			public static string _204 (string plugin)
			{
				return plugin + " import error ";
			}
			public static string _205 (string file)
			{
				return "Config file reading error at \"" + file + "\"";
			}
		}
		public class ContentError
		{
			public static string _301 = "Incorrectly copied meta files. " + Environment.NewLine + 
				"Turn on 'Visible Meta files' in 'Edit->Project Settings->Editor'" + Environment.NewLine + 
					"Restart the plugin.";
			public static string _302 = "Missing 'СustomResourcesManager' prefab at start scene ";
			public static string _303 = "Missing 'СustomResourcesManager' script at 'СustomResourcesManager' prefab ";
			public static string _304 (string plugin)
			{ 
				return plugin + " has conflicts";
			}
			public static string _305 (string plugin)
			{ 
				return plugin + " outdated";
			}
			public static string _306 (string plugin)
			{ 
				return plugin + " is missing files";
			}
			public static string _307 (string plugin)
			{ 
				return plugin + " files was changed";
			}
			public static string _308 (string path)
			{ 
				return "Access to '" + path + "' is denied. " + Environment.NewLine + 
					"Would you like to move the storage of the temporary folder in the project? ";
			}
			public static string _309 (string path)
			{
				return  "Deleting of "+ path + " plugin from local" +Environment.NewLine +
					"repository is failed ";
			}
			public static string _310 (string link)
			{
				return  "No link was found for \"" + link +"\"";
			}
			public static string _311 = "Failed to get the latest version of the plugins." +  Environment.NewLine +
				"Try clicking: AM Editor -> Check for plugins update";
			public static string _312 = "Restoring the 'SceneCustomCode' scene failed" +  Environment.NewLine +
				"Recreate it manually and add the 'CustomResourcesManager' prefab on it if necessary";
			public static string _313 = "Missing 'am_project.txt' file at 'Assets/StreamingAssets/'";
			public static string _314 = "Missing файл 'am_builds.txt' file at 'Assets/StreamingAssets/'";
		}
		public class HTTPClientError
		{
			public static string _400 (string url)
			{ 
				return url + Environment.NewLine + "400 Bad Request";
			}
			public static string _401 (string url)
			{ 
				return url + Environment.NewLine + "401 Unauthorized";
			}
			public static string _402 (string url)
			{ 
				return url + Environment.NewLine + "402 Payment Required";
			}
			public static string _403 (string url)
			{ 
				return url + Environment.NewLine + "403 Forbidden";
			}
			public static string _404 (string url)
			{ 
				return url + Environment.NewLine + "404 Not Found";
			}
			public static string _405 (string url)
			{ 
				return url + Environment.NewLine + "405 Method Not Allowed";
			}
			public static string _406 (string url)
			{ 
				return url + Environment.NewLine + "406 Not Acceptable";
			}
			public static string _407 (string url)
			{ 
				return url + Environment.NewLine + "407 Proxy Authentication Required";
			}
			public static string _408 (string url)
			{ 
				return url + Environment.NewLine + "408 Request Timeout";
			}
			public static string _409 (string url)
			{ 
				return url + Environment.NewLine + "409 Conflict";
			}
			public static string _410 (string url)
			{ 
				return url + Environment.NewLine + "410 Gone";
			}
			public static string _411 (string url)
			{ 
				return url + Environment.NewLine + "411 Length Required";
			}
			public static string _412 (string url)
			{ 
				return url + Environment.NewLine + "412 Precondition Failed";
			}
			public static string _413 (string url)
			{ 
				return url + Environment.NewLine + "413 Request Entity Too Large";
			}
			public static string _414 (string url)
			{ 
				return url + Environment.NewLine + "414 Request-URI Too Large";
			}
			public static string _415 (string url)
			{ 
				return url + Environment.NewLine + "415 Unsupported Media Type";
			}
			public static string _416 (string url)
			{ 	
				return url + Environment.NewLine + "416 Requested Range Not Satisfiable";
			}
			public static string _417 (string url)
			{ 
				return url + Environment.NewLine + "417 Expectation Failed";
			}
			public static string _418 (string url)
			{ 
				return url + Environment.NewLine + "418 I'm a teapot";
			}
			public static string _422 (string url)
			{ 
				return url + Environment.NewLine + "422 Unprocessable Entity";
			}
			public static string _423 (string url)
			{ 
				return url + Environment.NewLine + "423 Locked";
			}
			public static string _424 (string url)
			{ 
				return url + Environment.NewLine + "424 Failed Dependency";
			}
			public static string _425 (string url)
			{ 
				return url + Environment.NewLine + "425 Unordered Collection";
			}
			public static string _426 (string url)
			{ 
				return url + Environment.NewLine + "426 Upgrade Required";
			}
			public static string _428 (string url)
			{ 
				return url + Environment.NewLine + "428 Precondition Required";
			}
			public static string _429 (string url)
			{ 
				return url + Environment.NewLine + "429 Too Many Requests";
			}
			public static string _431 (string url)
			{ 
				return url + Environment.NewLine + "431 Request Header Fields Too Large";
			}
			public static string _434 (string url)
			{ 
				return url + Environment.NewLine + "434 Requested host unavailable";
			}
			public static string _449 (string url)
			{ 
				return url + Environment.NewLine + "449 Retry With";
			}
			public static string _451 (string url)
			{ 
				return url + Environment.NewLine + "451 Unavailable For Legal Reasons";
			}
			public static string _456 (string url)
			{ 	
				return url + Environment.NewLine + "456 Unrecoverable Error";
			}
			public static string _499 (string url)
			{ 
				return url + Environment.NewLine + "499 Use Nginx, when a client closes the connection before receiving a reply";
			}
		}
		public class HTTPServerError
		{
			public static string _500 (string url)
			{ 
				return url + Environment.NewLine + "500 Internal Server Error";
			}
			public static string _501 (string url)
			{ 
				return url + Environment.NewLine + "501 Not Implemented";
			}
			public static string _502 (string url)
			{ 
				return url + Environment.NewLine + "502 Bad Gateway";
			}
			public static string _503 (string url)
			{ 
				return url + Environment.NewLine + "503 Service Unavailable";
			}
			public static string _504 (string url)
			{ 
				return url + Environment.NewLine + "504 Gateway Timeout";
			}
			public static string _505 (string url)
			{ 
				return url + Environment.NewLine + "505 HTTP Version Not Supported";
			}
			public static string _506 (string url)
			{ 
				return url + Environment.NewLine + "506 Variant Also Negotiates";
			}
			public static string _507 (string url)
			{ 
				return url + Environment.NewLine + "507 Insufficient Storage";
			}
			public static string _508 (string url)
			{ 
				return url + Environment.NewLine + "508 Loop Detected";
			}
			public static string _509 (string url)
			{ 
				return url + Environment.NewLine + "509 Bandwidth Limit Exceeded";
			}
			public static string _510 (string url)
			{ 
				return url + Environment.NewLine + "510 Not Extended";
			}
			public static string _511 (string url)
			{ 
				return url + Environment.NewLine + "511 Network Authentication Required";
			}
		}
		public class ContentCreateConfig
		{
			public static string _Title = "Creating plugin_config.json";
			public static string _ConfigExistQuestion = "Is there any previous version plugin_config.json?";
			public static string _SelectConfig = "Select a plugin_config.json file";

			public static string _MissingFilesTitle = "Missing plugin files detected";
			public static string _MissingFilesQuestion = "Add to outdated list?";

			public static string _DuplicatedFilesTitle = "Files with the same name detected";
			public static string _DuplicatedFilesMessage = "The \"Unique\" flags will be turned off for this files";

			public static string _OutFolder = "Select a folder for plugin_config.json file";
			
			public static string _MainFolder = "Plugin main folder";
			public static string _MainFolderHelp = "Plugin main folder";
			
			public static string _ToExportFolder = "Export folder";
			public static string _ToExportFolderHelp = "Export folder";

			public static string _BuildNumber = "";
			public static string _BuildNumberHelp = "";
			
			public static string _PackageBuildType = "Current build type";
			public static string _PackageBuildTypeHelp = "A type of current build type of the plugin";

			public static string _PackageBuildNumber = "Current build number";
			public static string _PackageBuildNumberHelp = "Current build number (not necessary)";

			public static string _PackageExtraBuildOptions = "Additional build options";
			public static string _PackageExtraBuildOptionsHelp = "Additional build options";
			
			public static string _CreateConfigFile = "Create plugin_config.json";
			public static string _CreateConfigFileHelp = "Create plugin_config.json";
			
			public static string _CreateUnityPackage = "Create .unitypackage";
			public static string _CreateUnityPackageHelp = "Create .unitypackage";
			
			public static string _NamePlugin = "Plugin name : ";
			public static string _NamePluginHelp = "Plugin name in GitLab";
			
			public static string _OldNames = "Outdated plugin name ";
			public static string _OldNamesHelp = "Outdated plugin name";
			
			public static string _CopyFiles = "Copy the plugin files";
			public static string _CopyFilesHelp = "Copy the plugin files";

			public static string _EditFilesList = "Edit files";
			public static string _Back = "Back";
			public static string _SelectFolder = "Select folder";
		
			public static string _ViewType = "Display type : ";
			public static string _ViewTypeHelp = "Display type in wich will be seen plugin";
			
			public static string _PluginBuildType = "Build type(s) : ";
			public static string _PluginBuildTypeHelp = "A list of supported build types for the plugin. Separated by spaces. Empty filed for all build types support";

			public static string _IsMandatory = "Mandatory plugin : ";
			public static string _IsMandatoryHelp = "Wether the plugin mandatory";
			
			public static string _Link = "Link : ";
			public static string _LinkHelp = "Link to plugin project in GitLab, for example: http://pgit.digital-ecosystems.ru/unity-plugins/plugin-name";
			
			public static string _PluginVersion = "Plugin version : ";
			public static string _PluginVersionHelp = "Plugin version";
			
			public static string _DependsPlugin = "Dependencies ";
			public static string _DependsPluginHelp = "The names of plugins which depend on the current plugin";

			public static string _CoreFiles = "Plugin files ";//"Main files ";
			public static string _CoreFilesHelp = "Main files of the plugin";

			public static string _Example = "Example";
			public static string _ExampleHelp = "Whether the current file is an example of using the plugin";
			public static string _ExampleFiles = "Examples ";
			public static string _ExampleFilesHelp = "Files with plugin examples";

			public static string _Modifiable = "Modifiable";
			public static string _ModifiableHelp = "Whether the current file is modifiable"+System.Environment.NewLine+"The file can be changed";
			public static string _ModifiableFiles = "Modifiable files ";
			public static string _ModifiableFilesHelp = "Files of the plugin that you can change";
			
			public static string _DefaultNameDepend = "Plugin Name";
			public static string _DefaultOldName = "Old Name";
			
			public static string _CreateConfig = "Create";
			
			public static string _AddDepend = "Add dependance";
			public static string _AddDependHelp = "Add dependance";
			
			public static string _AddOutdated = "Add file";
			
			public static string _AddOldName = "Add outdated name";
			public static string _AddOldNameHelp = "Add outdated name";

			public static string _AddCore = "Add plugin files";//main files";
			public static string _AddCoreHelp = "Add main files";

			public static string _AddExample = "Add examples";
			public static string _AddExampleHelp = "Add examples";
			
			public static string _AddModifiable = "Add modified files";
			public static string _AddModifiableHelp = "Add modified files";

			public static string _DragAndDropAreaCoreText = "Place here main files(folders) of the plugin ";
			public static string _DragAndDropAreaExampleText = "Place here example files(folders) of the plugin ";
			public static string _DragAndDropAreaModifiableText = "Place here modified files(folders) of the plugin ";
			public static string _DragAndDropAreaOutdatedText = "Place here outdated files(folders) of the plugin ";
			
			public static string _CurrentFilesPlugins = "Current plugin files";
			public static string _OutdatedFilesPlugins = "Outdated plugin files";

			public static string _SelectAsset = "Select";
			public static string _SelectAssetHelp = "Select the files and folders that are part of the plugin";
			
			public static string _Unique = "Unique";
			public static string _UniqueHelp = "Whether the current file is unique to the project"+System.Environment.NewLine+"Disable, or if there are files with the same name but a different path";

			public static string _Path = "File full path";

			public static string _EmptyMainFolderTitle = "Empty main folder";
			public static string _EmptyMainFolderMessage = "Select main plugin folder in the project";

			public static string _BadExportFolderTitle = "Invalid export folder";
			public static string _BadExportFolderMessage = "The export folder path cannot be empty or direct through the \"Assets\" folder";

			public static string _IncorrectParametersDetected = "Incorrect parameters detected";
		}
		public class ContentCreatePublicFile
		{
			public static string _Title = "Creating ameditor_plugins.json";
			public static string _OutFolder = "Select folder for plugin_config.json";

			public static string _IsMandatory = "Mandatory"+Environment.NewLine+"plugin";
			public static string _IsMandatoryHelp = "Wether the plugin mandatory";
			public static string _IsMandatoryTrue = "Mandatory";
			public static string _IsMandatoryFalse = "Optional";
			
			public static string _PluginName = "Plugin name";
			public static string _PluginNameHelp = "Plugin name in GitLab";

			public static string _SortByNameHelp = "Sort plugins by name";
			
			public static string _PluginVersion = "Plugin version";
			public static string _PluginVersionHelp = "Actual version of the plugin";

			public static string _SortByVersionHelp = "Sort plugins by version";

			public static string _MinimumVersion = "Minimum version";
			public static string _MinimumVersionHelp = "Download from selected to the latest version";
			
			public static string _OldNames = "Old names";
			public static string _OldNamesHelp = "Old names of the plugin";

			public static string _BuildTypes = "Build types";
			public static string _BuildTypesHelp = "Build types supported by this version of the plugin";
			public static string _BuildTypesNoneHelp = "Build type isn't specified. The plugin supports all build types";
		
			public static string _DisplayType = "Display type";
			public static string _DisplayTypeHelp = "Display type in wich will be seen plugin";

			public static string _Depends = "Dependent plugins";
			public static string _DependsHelp = "Dependent plugins list";

			public static string _Url = "GitLab";
			public static string _UrlHelp = "GitLab URL";

			public static string _VersionFilter = "Version filter";
			public static string _VersionFilterHelp = "The nubmer of plugins versions displayed for each build type";
			public static string _DefaultVersionFilter = "All";
			public static string _ActualVersionFilter = "Actual only";
			public static string _OtherVersionFilter = "Actual +";

			public static string _ReleaseOnly = "Release only";
			public static string _ReleaseOnlyHelp = "Hyde Dev and RC plugins";
			
			public static string _AddPlugin = "Add plugin";
			public static string _AddPluginHelp = "Add plugin for public file";
			
			public static string _Create = "Create";
			public static string _CreateAndPush = "Create and push to Git";
			public static string _CreateHelp = "Create public file";

			public static string _SelectHelp = "Selected plugin will be add to ameditor_plugins.json";
			
			public static string _SelectAll = "All";
			public static string _SelectAllHelp = "Select all plugins";
			
			public static string _DeselectAll = "None";
			public static string _DeselectAllHelp = "Deselect all plugins";
			
			public static string _UpdateFromGit = "Update";
			public static string _UpdateFromGitHelp = "Update plugins info from GitLab";

			public static string _ForceUpdateFromGit = "Force search";
			public static string _ForceUpdateFromGitHelp_True = "Search will be performed on all versions of plugins";
			public static string _ForceUpdateFromGitHelp_False = "Search will be made only by the latest versions of plugins";

			public static string _SuccessDialogTitle = "Preparation completed";
			public static string _SuccessDialogMessage = "Preparation of plugins for ameditor_plugins.json completed";
			
			public static string _FailedDialogTitle = "Preparation failed";
			public static string _FailedDialogMessage = "Failed to create ameditor_plugins.json. It will use a local file";

			public static string _CanceledDialogTitle = "Preparation cancelled";
			public static string _CanceledDialogMessage = "Preparation of plugins for ameditor_plugins.json was cancelled by user. It will use a local file";

			public static string _PluginsCount = "All plugins: ";
			public static string _SelectedPlugins = "Selected plugins: ";

			public static string _EmptyCommitTitle = "File upload error";
			public static string _EmptyCommitMessage = "The commit message can not be empty!";

			public static string _PushingConfigToggle = " Push the config file to the Git after the creation";
			public static string _PushingConfigToggleHelp = "branch: ";

			public static string _PushingConfigTitle = "Pushing the config";
			public static string _PushingConfigSuccessDialog (string fileName)
			{
				return "Pushing " + fileName + " file successfully completed";
			}
			public static string _PushingConfigFailedDialog (string fileName)
			{
				return "Pushing " + fileName + " file failed";
			}

			public static string _RetryConfigSearchButton = "Try again";
		}
		public class ContentStatuses
		{
			public static string _CheckingBranch = "Checking project branches";
			public static string _GettingLastCommit = "Getting the latest commit";
			public static string _GettingTags = "Getting a list of the plugin versions";
			public static string _DownloadingArchive = "Downloading project archive";
			public static string _ExtractingArchive = "Extracting archive";
			public static string _DeletingArchive = "Deleting archive";
			public static string _ImportingPlugin = "Importing plugin to the project";

            public static string _DownloadingUnitypackage = "Downloading unitypackage";
            public static string _ImportingUnitypackage = "Importing unitypackage to the project";

            public static string _LoadingDefaultsForCQS = "Loading settings";
            public static string _ReadingDefaultsForCQS = "Processing settings";
            public static string _SettingDefaultsForCQS = "Accepting settings";

			public static string _CheckingLocalConfig = "Checking local config";
			public static string _GettingGroupsList = "Getting groups list";
			public static string _SearchingNeededGroup = "Searching for the group: ";
			public static string _GettingGroupProjects = "Getting a list of projects for group ";
			public static string _SearchingCompatibleProjects = "Searching compatible projects";
			public static string _WorkingWithCompatible = "Getting compatible projects";
			public static string _SearchingPluginConfig = "Searching plugin_config.json";
			public static string _GettingPluginConfig = "Getting plugin_config.json";

			public static string _RestoringCRM = "Restoring Custom Resources Manager";

			public static string _DontCloseAMEditor = "Please do not close AM Editor window";
		}
		public class ContentRestoreDefaults
		{
			public const string _RestoreDefaultSettingsMenu = "AM Editor/Settings/Restore default settings";
			public static string _TitleDialog = "Restoring default settings";
			public static string _RestoreDefaultSettingsQuestion = "AM Editor default settings will be restored."
				+System.Environment.NewLine+"Continue?";
			public static string _MessageSuccessDialog = "AM Editor default setting successfully restored";
		}
		
		public static string _ContentClose = "Close";
		public static string _ContentCloseHelp = "Close current window";
		
		public static string _ContentBackToPlugins = "Back to the plugins";
		public static string _ContentBackToPluginsHelp = "Go to plugins list window";
		
		public static string _ContentListOfFiles = "Files";//"Files list";
		public static string _ContentListOfFilesHelp = "Show a list of files of ";

		public static string _ContentIncludedPlugins = "Included plugins";
		public static string _ContentIncludedPluginsHelp = "Show a list of included plugins of ";

		public static string _ContentExamples = "Examples";
		public static string _ContentExamplesHelp = "Show a list of examples for ";
		
		public static string _ContentButtonUpdateList = "Update the list";
		public static string _ContentButtonUpdateListHelp = "Update the list ";
		
		public static string _ContentTitleWindowListPlugins = "Plugins list";
		public static string _ContentTitleWindowListPluginsHelp = "Plugins list ";
		
		public static string _ContentTitleWindowFixConflict = "Fix conflicts";
		public static string _ContentTitleWindowFixConflictHelp = "Fix conflicts ";
		
		public static string _ContentTurn = "Close";
		public static string _ContentTurnHelp = "Close";
		
		public static string _ContentExpand = "Expand";
		public static string _ContentExpandHelp = "Expand";
		
		public static string _ContentDeleteFiles = "Delete files";
		public static string _ContentDeleteFilesHelp = "Delete files";
		
		public static string _ContentDeleteAllFiles = "Delete all files";
		public static string _ContentDeleteAllFilesHelp = "Delete all files";
		
		public static string _ContentOpenFile = "Open";
		public static string _ContentOpenFileHelp = "Open file ";
		
		public static string _ContentRestoreFiles = "Restore files";
		public static string _ContentRestoreFilesHelp = "Restore files";
		
		public static string _ContentNonePlugin = "Link to the plugin";
		public static string _ContentNonePluginHelp = "Link to the plugin";
		
		public static string _ContentDeletePlugin = "Delete plugin";
		public static string _ContentDeletePluginHelp = "Delete plugin";
		
		public static string _ContentTitleDeletePlugin = "Deliting plugin files";
		public static string _ContentTitleDeletePluginHelp = "Deliting plugin files";
		
		public static string _ContentQuestionDeletePlugin = "Are you sure you want to delete ALL files of ";
		public static string _ContentQuestionDeleteSelectedPlugin = "Are you sure you want to delete ALL files of selected plugins?";
		public static string _ContentQuestionDeletePluginHelp = "Are you sure you want to delete ALL plugin files?";

		public static string _ContentTitleDeleteDepends = "Deliting plugin depends";
		public static string _ContentTitleDeleteDependsHelp = "Deliting plugin depends";

		public static string _ContentQuestionDeleteDepends (string pluginName)
		{
			return pluginName+" plugin depends founded.\nWould you like to delete only current plugin or the depends to?";
		}

		public static string _ContentQuestionSeveralPlugins = "There are several deleted plugins with the dependent plugins.\nWould you like to apply the selected option to all such plugins?";
		
		public static string _ContentTitleDeletingFiles = "Deleting files";
		public static string _ContentTitleDeletingFilesHelp = "Deleting files";
		
		public static string _ContentQuestionDeleteFiles = "Are you sure you want to delete files?";
		public static string _ContentQuestionDeleteFilesHelp = "Are you sure you want to delete files?";
		
		public static string _ContentBackups = "Backups";
		public static string _ContentBackupsHelp = "Backups";
		
		public static string _ContentDateTimeBackup = "The time of the backup : ";
		public static string _ContentDateTimeBackupHelp = "The time of the backup :";
		
		public static string _ContentDeleteBackup = "Delete backup";
		public static string _ContentDeleteBackupHelp = "Delete backup";
		
		public static string _ContentTitleDeletingBackup = "Deleting backup";
		public static string _ContentTitleDeletingBackupHelp = "Deleting backup";
		
		public static string _ContentQuestionDeleteBackup = "Are you sure you want to delete the backup?";
		public static string _ContentQuestionDeleteBackupHelp = "Are you sure you want to delete the backup?";
		
		public static string _ContentDeleteBackups = "Delete all backups";
		public static string _ContentDeleteBackupsHelp = "Delete all backups";
		public static string _ContentDeleteBackupsCompact = " All backups";
		
		public static string _ContentTitleDeletingBackups = "Deleting all backups";
		public static string _ContentTitleDeletingBackupsHelp = "Deleting all backups";
		
		public static string _ContentTitleRestoredBackups = "Restoring files";
		public static string _ContentAlertRestoredBackups = "Files was restored";

		public static string _ContentBackupsCount = "Number of backups: ";
		
		public static string _ContentQuestionDeleteBackups = "Are you sure you want to delete ALL backups?";
		public static string _ContentQuestionDeleteBackupsHelp = "Are you sure you want to delete ALL backups?";

		public static string _ContentSupport = "Write to support";
		public static string _ContentDownloadActual = "Download actual";
#else
		//if AM_EDITOR_LANGUAGE_RU
		
		public class ContentMenuItem
		{
			public const string _ShowAMEditor =  "AM Editor/Показать AM Editor";
			public const string _CheckUpdate =  "AM Editor/Проверка обновлений";
			public const string _ShowAMStore = "AM Editor/Открыть AM Store";
			public const string _RestoreCRM =  "AM Editor/Восстановить Custom Resources Manager";
			public const string _FixMetaALibs =  "AM Editor/Исправить .a файлы";
			public const string _FixMetaBundle =  "AM Editor/Исправить .bundle файлы";
			public const string _MakeAssetBundle = "AM Editor/Создать Asset Bundle/Для текущей платформы";
			public const string _MakeAssetBundleForAll = "AM Editor/Создать Asset Bundle/Для всех платформ";

			public const string _HelpWiki = "AM Editor/Помощь/Wiki";
			public const string _HelpSupport = "AM Editor/Помощь/Поддержка";
			public const string _HelpChangeLog = "AM Editor/Помощь/История изменений версий";
			
			public const string _WindowCreateConfigFile = "AM Editor/Разработчикам/Создать plugin_config.json";
			public const string _WindowCreateConfigALL = "AM Editor/Разработчикам/Создать ameditor_plugins.json";

            public const string _SetDefaultsForCustomQualitySetup = "AM Editor/Разработчикам/Значения по-умолчанию для Custom Quality Setup";

            public const string _LanguageEN = "AM Editor/Настройки/Язык/EN";
			public const string _LanguageRU = "AM Editor/Настройки/Язык/RU";

			public const string _Backups = "AM Editor/Резервные копии";
		}

        public class ContentCustomQualitySetup
        {
            public static string _TitleDialog = "Загрузка параметров по-умолчанию";
            public static string _MessageSuccessDialog = "Успешно загружены параметры по-умолчанию для Custom Quality Setup";
        }

		public class ContentMenuAbout
		{
			public static string _GetLinks = "Получение about_config.json";
		}

		public class ContentPluginsType
		{
			public static string _TitleDialog = "Изменение типа плагинов";
			public static string _MessageSuccessDialog = "Успешно изменен тип плагинов";
		}

		public class ContentViewType
		{
			public static string _TitleDialog = "Изменение вида AM Editor'a";
			public static string _MessageSuccessDialog = "Успешно изменен вид AM Editor'a";
			public static string _ChangeViewTypeQuestion = "Вы действительно хотите сменить вид AM Editor'a?";
			public static string _SwitchCompactModeMessage ()
			{
				string actionType;
				#if AM_EDITOR_COMPACT_ON
				actionType = "отключения";
				#else
				actionType = "включения";
				#endif
				return "\n\nДля "+actionType+" компактного режима AM Editor будет перезапущен";
			}
		}

		public class ContentMenuLanguage
		{
			public static string _TitleDialog = "Изменение языка";
			public static string _MessageSuccessDialog = "Успешно изменен язык AM Editor'a";
		}

		public class ContentDeleteExample
		{
			public const string _Title = "Удаление примера";//Deleting example
			public const string _Message = "Вы действительно хотите удалить файл-пример?";//Are you sure you want to delete an example file?
		}
		
		public class ContentMenuLocalRepository
		{
			public const string _Open = "AM Editor/Локальное хранилище/Открыть";
			public const string _Clean = "AM Editor/Локальное хранилище/Удалить загруженные плагины";
			
			public static string _DeleteTitle = "Удаление файлов из локального хранилища";
			public static string _DeleteQuestion = "Вы действительно хотите удалить загруженные файлы плагинов из локального хранилища?";
			public static string _SuccessMessage = "Удаление прошло успешно.";
			public static string _FailureMessage = "Не удалось полностью очистить локальное хранилище \nот плагинов текущего типа.";
			public const string _OpenButton = "Открыть";
		}
		public class ContentVisibleMeta
		{
			public const string _FailedMakeVisibleTitle = "Ошибка доступа к meta файлам";
			public const string _FailedMakeVisible = "Не удалось сменить режим контроля версий на \"Visible Meta Files\"\nПопробуйте изменить параметр вручную";
			public const string _YouKnowWhatToDo = "Я знаю что делать";
			public const string _ShowMe = "Показать параметр";
		}
		public class ContentFixMetaA
		{
			public const string _SuccessFixTitle = "Успешно исправлены .a файлы";
			public const string _SuccessFix = "Все .a файлы были исправлены";
			public const string _FailedFixTitle = "Ошибка исправления .a файлов";
			public const string _FailedFix = "Во время исправления возникли ошибки";
			public const string _RepeatFix = "Запустить еще раз";
		}
		public class ContentFixMetaBundle
		{
			public const string _SuccessFix = "Все .bundle файлы были исправлены";
			public const string _SuccessFixTitle = "Успешно исправлены .bundle файлы";
			public const string _FailedFix = "Во время исправления возникли ошибки";
			public const string _FailedFixTitle = "Ошибка исправления .bundle файлов";
			public const string _RepeatFix = "Запустить еще раз";
		}
		public class ContentAssetBundles
		{
			public const string _DialogTitle = "Создание Asset Bundle";
			public const string _DialogMessage = "Вы собираетесь создать Asset Bundle.\n\nВсе выбранные сцены, будут помещены в Asset Bundle и отключены в Build Settings для последующей загрузки из бандла.";
			public const string _Continue = "Продолжить";
			public const string _SuccessCreationMessage = "Asset Bundle успешно создан в StreamingAssets\n\nНе забудьте проверить параметры загрузки ассет бандлов на префабе CustomResourcesManager'a";
			public const string _ScenesNotFoundMessage = "В Build Settings не отмечено ни одной сцены!";
			public const string _NotEnoughScenesMessage = "Количество выбранных сцен должно быть больше 0 для создания ассет бандла";
			public const string _ErrorMessage = "Не удалось создать AssetBundle для следующих платформ:\n";
		}

		public class ContentWindowAuthorization
		{
			public static string _ShowPassword = "Показать пароль";
			public static string _HidePassword = "Скрыть пароль";
			public static string _Or = "или";
		}
		public class ContentWindowConflict
		{
			public static string _ContentOutdatedMessage = "файл из старых версий плагина";
			public static string _ContentPossibleConflist = "возможен конфликт с файлами из плагина";
			public static string _ContentQuestionDeleteFiles = "Вы действительно хотите удалить файлы?";
			public static string _ContentQuestionDeleteSelectedFilesFor (string pluginName)
			{
				return "Вы действительно хотите удалить выделенные файлы, конфликтующие с " + pluginName + " ?";
			}
			public static string _ContentQuestionDeleteAllSelectedFiles = "Вы действительно хотите удалить BCE выделенные файлы ?";
		}
		public class ContentFixConflicts
		{
			public static string _SuccessFixConflictTitle = "Конфликты успешно исправлены";
			public static string _SuccessFixConflict = "Конфликты с файлами плагинов успешно исправлены. Конфликтующие файлы перемещены в резервные копии";
			public static string _FailedFixConflictTitle = "Ошибка исправления конфликтов";
			public static string _FailedFixConflict = "Во время автоматического исправления конфликтов возникли ошибки. Перейти к ручному устранению конфликтов?";
		}
		public class ContentWindowChangeLog
		{
			public static string _Title = "Об AM Editor";
		}
		public class ContentTexure
		{
			public static string _Ok =  " Ок";
			public static string _OkHelp (string pluginName)
			{
				return "Обновление плагина " + pluginName + " не требуется";
			}
			
			public static string _NeedCheck =  "Нужна проверка";
			public static string _NeedCheckHelp = "Требуется проверка обновлений плагина ";

			public static string _Fixme =  "проблемы с плагином";
			public static string _FixmeHelp = "fix me ";
			
			public static string _NeedUpdate =  "Нужно загрузить плагин";
			public static string _NeedUpdateHelp (string pluginName)
			{
				return "Требуется обновление плагина " + pluginName + ". Загрузите последнюю версию";
			}

			public static string _NotLatestVersion = "Версия плагина не является последней";
			public static string _NotLatestVersionHelp (string pluginName)
			{
				return "Текущая версия плагина " + pluginName + " не является последней. Выберите нужную версию из списка и загрузите, если необходимо";
			}

			public static string _VersionNotForBuildType = "Версия плагина не совместима с типом сборки";
			public static string _VersionNotForBuildTypeHelp (string pluginName)
			{
				return "Текущая версия плагина " + pluginName + " не подходит для выбранного типа сборки. Загрузите подходящую версию или удалите плагин";
			}

			public static string _SelecdtedAnotherVersion =  "Выбранная версия отлична от установленной";
			public static string _SelecdtedAnotherVersionHelp (string pluginName)
			{
				return  "Выбранная версия плагина " + pluginName + " отличается от установленной";
			}
		}
		public class ContentPlugin
		{
			public static string _Logout = "Сменить аккаунт";
			public static string _LogoutHelp = "Сменить аккаунт";

			public static string _LogoutDialogTitle = "Сменить аккаунт";
			public static string _LogoutDialogMessage = "Вы действительно хотите сменить аккаунт?";

			public static string _CurrentAccount = "Текущий аккаунт: ";
			public static string _CurrentAccountHelp = "Текущий аккаунт";

			public static string _PluginsBuildTypes = "Тип сборки плагинов ";
			public static string _PluginsBuildTypesHelp = "Типы сборок в которых могут участвовать плагины";

			public static string _DownloadForBuildType = "Загрузить";
			public static string _DownloadForBuildTypeHelp = "Будут загружены плагины, соответсвующие выбранному типу сборки";

			public static string _ConnectedPluginsTitle = "Подключенные плагины";
			public static string _ConnectedPluginsTitleHelp = "Подключенные в проект плагины";

			public static string _PluginName = "Название плагина";
			public static string _PluginNameHelp = "Название плагина в GitLab";
			
			public static string _CurrentVersion = "Текущая версия";
			public static string _CurrentVersionHelp = "Версия подключенного в проект плагина";
			
			public static string _ActualVersion = "Актуальная версия";
			public static string _ActualVersionHelp = "Последняя доступная версия плагина";
			
			public static string _UnconnectedPluginsTitle = "Неподключенные плагины";
			public static string _UnconnectedPluginsTitleHelp = "Доступные для подключения плагины";

			public static string _ExternalPluginsTitle = "Сторонние плагины";
			public static string _ExternalPluginsTitleHelp = "Сторонние плагины";
			public static string _ExternalPluginsUnavailable = "(Вы не состоите в группе Unity Plugins!)";
			public static string _ExternalPluginsUnavailableTitleDialog = "Ошибка доступа к Unity Plugins";
			public static string _ExternalPluginsUnavailableMessage = "Для загрузки сторонних плагинов необходим доступ к группе Unity Plugins. Обратитесь к тимлиду или начальнику отдела";

			public static string _SearchTitle = "Поиск";
			public static string _SearchPlaceholder = "Имя плагина...";

			public static string _MandatoryPluginsTitle = "Необходимые для подключения плагины";
			public static string _MandatoryPluginsTitleHelp = "Необходимые для подключения плагины";
			
			public static string _Selected = "Выбранные";
			public static string _SelectedHelp = "Выбранные";
			
			public static string _Download = "Загрузить";
			public static string _DownloadHelp = "Загрузить плагин ";

			public static string _Update = "Обновить";

			public static string _ExamplesInfoHelp (string button)
			{
				return "У плагина имеются файлы-примеры.\nДля взаимодействия нажмите кнопку " + button + " для этого плагина";
			}

			public static string _IncorrectDownloadUrl = "Некорректная ссылка на репозиторий в localgit";
			
			public static string _Delete = "Удалить";
			public static string _DeleteHelp = "Удалить плагин ";
			
			public static string _ManualLink = "GitLab";
			public static string _ManualLinkHelp (string pluginName)
			{
				return "Скачать плагин " + pluginName + " вручную";
			}

			public static string _Description = "Описание";
			public static string _DescriptionHelp = "Открыть описание плагина";

			public static string _Check = "Проверить";
			public static string _CheckHelp = "Проверить обновление плагина ";
			
			public static string _CheckForce = "Проверить hash";
			public static string _CheckForceHelp = "Проверить hash"+Environment.NewLine+"Help";
			
			public static string _Version= "";
			public static string _VersionHelp = "";

			public static string _CheckSelected = "Проверить выбранные ";
			public static string _CheckSelectedHelp = "Проверить наличие обновлений у отмеченных плагинов";
			
			public static string _DownloadSelected = "Загрузить выбранные ";
			public static string _DownloadSelectedHelp = "Загрузить обновления у отмеченных плагинов";
			public static string _DownloadSelectedUnconnectHelp = "Загрузить отмеченные плагины";
			
			public static string _DeleteSelected = "Удалить выбранные ";
			public static string _DeleteSelectedHelp = "Удалить отмеченные плагины";
		}
        public class ContentExternalPlugin
        {
			public static string _SuccessDownloadMessage (string pluginName)
			{
				return "Плагин " + pluginName + " успешно загружен";
			}
			public static string _SuccessImportMessage (string pluginName)
			{
				return "Плагин " + pluginName + " успешно импортирован в проект";
			}
			public static string _FailedImportMessage (string pluginName)
			{
				return "Не удалось импортировать " + pluginName + " в проект. Попробуйте импортировать с помощью \"Assets/Import Package\"";
			}
			public static string _FailedDownloadMessage (string pluginName)
			{
				return "Не удалось загрузить плагин " + pluginName + ". Попробуйте еще раз";
			}
        }
		public class ContentMakeAllGoodButton
		{
			public static string _AllGood = "Всё хорошо";
			public static string _AllGoodHelp = "С плагинами всё в порядке";
			public static string _BuildTypeProblems = "Применить тип сборки";
			public static string _BuildTypeProblemsHelp = "Будут загружены плагины, соответсвующие выбранному типу сборки";
			public static string _MissingFiles = "Загрузить отсутствующие файлы";
			public static string _MissingFilesHelp = "Некоторые файлы плагинов отсутствуют и будут загружены";
			public static string _HasConflicts = "Исправить конфликты";
			public static string _HasConflictsHelp = "Имеются конфликты с файлами некоторых плагинов, необходимо исправить";
			public static string _ChangedFiles = "Обновить изменённые файлы";
			public static string _ChangedFilesHelp = "Некоторые файлы плагинов были изменены и будут загружены заново";
			public static string _NeedDepends = "Загрузить зависимости";
			public static string _NeedDependsHelp = "Будут загружены плагины, необходимые для уже установленных";
			public static string _NeedUpdate = "Обновить плагины";
			public static string _NeedUpdateHelp = "Будут загружены актуальные версии плагинов";
			public static string _NeedMandatory = "Загрузить обязательные плагины";
			public static string _NeedMandatoryHelp = "Будут загружены обязательные плагины";
			public static string _OutdatedPlugins = "Удалить устаревшие плагины";
			public static string _OutdatedPluginsHelp = "Найдены устаревшие плагины, рекомендуется удалить";
		}
		public class ContentStandardButton
		{
			public static string _Yes = "Да";
			public static string _YesHelp = "Да";
			
			public static string _Ok = " Ок";
			public static string _OkHelp = " Ок";
			
			public static string _More = "Подробнее...";
			public static string _MoreHelp = "Подробнее";
			
			public static string _No = " Нет";
			public static string _NoHelp = "Нет";
			
			public static string _Apply = "Принять";
			public static string _ApplyHelp = "Принять";
			
			public static string _Cancel = " Отмена";
			public static string _CancelHelp = "Отмена";

			public static string _Add = "Добавить";	
			public static string _AddHelp = "Добавить";

			public static string _Delete = "Удалить";
			public static string _DeleteHelp = "Удалить";
			
			public static string _None = "Ни одного";
			public static string _NoneHelp = "Ни одного";
			
			public static string _All = "Все";	
			public static string _AllHelp = "Все";

			public static string _OnlyCurrent = " Текущий";
			public static string _OnlyCurrentHelp = "Удалить только ";

			public static string _WithDepends = "C зависимостями";
			public static string _WithDependsHelp (string pluginName)
			{
				return "Удалить плагин " + pluginName + " вместе со всеми зависимостями";
			}

			public static string _ApplyForAll = "Для всех";
			public static string _ApplyForCurrent = "Только текущий";

			public static string _DontShowAgain = "Больше не показывать";
			public static string _DontShowAgainHelp = "Данное сообщение больше не появится в этом проекте"+Environment.NewLine+"(может потребоваться перезапуск окна)";

			public static string _ShowAMEditor = "Показать AM Editor";
		}
		public class ContentBackup
		{
			public static string _ButtonBackups = "Резервные копии";
			public static string _ButtonBackupsHelp = "Резервные копии удаленных при решении конфликтов файлов";
			
			public static string _TitleWindowBackup = "Резервные копии";
			public static string _TitleWindowBackupHelp = "Резервные копии ";
		}
		public class ContentProblems
		{
			public static string _Fix = "Исправление конфликтов ";
			public static string _FixHelp = "Исправление конфликтов ";
			
			public static string _ErrorMissingFiles = " - Нехватает файлов плагина. Загрузите плагин";
			public static string _ErrorMissingFilesHelp = " - Нехватает файлов плагина. Загрузите плагин";
			
			public static string _ErrorMissingFilesHash = " - Файлы плагина были изменены. Загрузите плагин заново";
			public static string _ErrorMissingFilesHashHelp = " - Файлы плагина были изменены. Загрузите плагин заново";
			
			public static string _ErrorConflictFiles = " - Существуют конфликты с этим плагином";
			public static string _ErrorConflictFilesHelp = " - Существуют конфликты с этим плагином";
			
			public static string _ErrorDependVersion = "Ошибка версии";
			public static string _ErrorDependVersionNotSupported = " не поддерживается AM Editor'ом."+Environment.NewLine+"Обратитесь в поддержку";
			public static string _ErrorDependVersionIsOld = " не поддерживается AM Editor'ом, так как требуемая версия не является актуальной."+Environment.NewLine+"Вы можете загрузить аткуальную версию";

			public static string _Depends = "Зависимости: ";
			public static string _DependsHelp = "Для работы плагина нужно загрузить ";

			public static string _CustomCodeDependConflict = "- Существуют конфликты с плагином ";
			public static string _CustomCodeDependConflictHelp = "Существуют конфликты с плагином, входящим в состав Сustom Code";
			public static string _CustomCodeDependMissingFiles = "- Нехватает файлов плагина ";
			public static string _CustomCodeDependMissingFilesHelp = "Нехватает файлов плагина, входящего в состав Custom Code";
			public static string _CustomCodeDependMissingHash (string pluginName)
			{
				return "- Файлы плагина " + pluginName + " были изменены";
			}
			public static string _CustomCodeDependMissingHashHelp = "Файлы плагина, входящего в состав Custom Code, были изменены";
		}
		public class ContentProgressBar
		{
			public static string _NeedUpdate = "Требуется обновить ";

			public static string _UpdateCustomCode = ". Обновите Custom Code";

			public static string _NeedDownload = "Требуется загрузить ";
			public static string _NeedDownloadFor = " для следующих плагинов: ";
			public static string _NeedUpdateHelp = "Нужно обновление ";

			public static string _NotLatestVersion = "Не последняя версия ";

			public static string _NotForBuildTypeCC = "Текущая версия Custom Code не совместима с типом сборки ";
			public static string _NotForBuildType = " не совместим с типом сборки ";
			
			public static string _Update = "Обновление";
			public static string _NoUpdate = "Обновлений плагинов не требуется";
			public static string _UpdateHelp = "Обновление ";

			public static string _ConfigPrepare = "Подготовка ameditor_plugins.json...";
			public static string _ConfigPrepareHelp = "Подготовка ameditor_plugins.json";

			public static string _PushingConfig = "Отправка ameditor_plugins.json в репозиторий Git...";

            public static string _DefaultsForCustomQualitySetup = "Параметры по-умолчанию для Custom Quality Setup";

			public static string _LaunchingAMEditorTitle = "Запуск AM Editor'a";
			public static string _LaunchingAMEditorMessage = "Запуск AM Editor'a. Пожалуйста, подождите...";

            public static string _Checking = "Проверка";
			public static string _CheckingPublicRepo = "Проверка публичного репозитория";
			public static string _CheckingUpdatePlugins = "Проверка обновлений плагинов";
			public static string _UpdateList = "Обновление списка";
			public static string _CheckingUpdate = "Проверка обновления";

			public static string _GettingAssetsList = "Получение списка ассетов";
			
			public static string _Plugin = "Плагин ";
			
			public static string _DownloadTo = "Загружается ";
			
			public static string _SuccessUpdateMessage = "Успешно загружено ";
			
			public static string _FailedUpdateTitle = "Ошибка загрузки ";
			public static string _FailedUpdateMessage = "Произошла ошибка. Попробуйте еще раз";
			
			public static string _CancelUpdateTitle = "Отмена загрузки ";
			public static string _CancelUpdateMessage = "Произошла отмена загрузки пользователем";
			
			public static string _ForceCheck = "Обновление";
			public static string _ForceCheckHelp = "Обновление ";
			
			public static string _AutoFix = " Автоматичесое решение конфликтов ";
			public static string _AutoFixHelp = " Автоматичесое решение конфликтов";

			public static string _CheckingLocalRepo = "Проверка локального репозитория";
			public static string _CheckingLocalRepoHelp = "Проверка локального репозитория";
			
			public static string _CopyFiles = " Копирование файлов в проект ";
			public static string _CopyFilesHelp = " Копирование файлов в проект ";

			public static string _IsCompiling = "Компиляция";
		}
		public class ContentOtherWindow
		{
			public static string _TitleWait = "Ожидание";
			public static string _TitleCriticalError = "Критическая ошибка";
			public static string _TitleErrorPopup = "Ошибка ";
			public static string _TitleWarning = "Внимание!";
			public static string _WebPlayerMessage = "В качестве текущей платформы выбран Web Player\nДля корректной работы плагинов следует сменить платформу";
			public static string _InvalidProductNameSymbolsMessage = "Обнаружены невалидные символы в Product Name!\nИсправить их автоматически?";
			public static string _NonLatinProductNameSymbolsMessage = "Поле Product Name должно содержать только латинские символы!";
			public static string _CompileWaitMessage = "Unity выполняет компиляцию скриптов. Пожалуйста, подождите...";
			public static string _RestoringCRMWaitMessage = "Восстановление компонентов Custom Resources Manager...";
			public static string _CloseOnBisyMessage = "Выполняется загрузка плагинов\nДля закрытия AM Editor'a сперва отмените загрузку!";
		}
		public class ContentAMStoreWindow
		{
			public static string _Title = "AM Store";
			public static string _Search = "Поиск";
			public static string _SearchButtonHelp = "Начать Поиск!";
			public static string _SearchPlaceholder = "Поиск...";
			public static string _AssetName = "Название";
			public static string _AssetNameHelp = "Название ассета";
			public static string _AssetVersion = "Версия";
			public static string _AssetVersionHelp = "Версия ассета";
		}
		public class ContentAuth
		{
			public static string _Title = "Авторизация";
			
			public static string _CurrentAccount = "Текущий аккаунт";
			public static string _CurrentAccountHelp = "Текущий аккаунт";
			
			public static string _Login = "Логин";
			public static string _LoginHelp = "Ваш логин в системе LDAP";
			
			public static string _Password = "Пароль";
			public static string _PasswordHelp = "Пароль от Вашего LDAP аккаунта";
			
			public static string _PrivateToken = "Private Token";
			public static string _PrivateTokenHelp = "Находится в настройках аккаунта в localgit";
			
			public static string _Auth = "Авторизация";
			public static string _AuthHelp = "Авторизация";		
		}
		public class WebError
		{
			public static string _100 = "GitLab недоступен";
			public static string _101 = "Ошибка авторизации";
			public static string _102 (string plugin)
			{ 
				return "Ошибка репозитория проекта " + plugin;
			}
			public static string _103 (string plugin, string branch)
			{ 
				return "Ошибка репозитория проекта " + plugin;
			}
			public static string _104 (string plugin)
			{ 
				return "Ошибка репозитория проекта " + plugin;
			}
			public static string _105 (string plugin)
			{
				return "Ошибка репозитория проекта " + plugin;
			}
			public static string _106 (string url)
			{
				return "Ошибка репозитория проекта " + url;
			}
			public static string _107 (string url)
			{ 
				return "Нет доступа к " + url;
			}
			//public static string _108 = "Ошибка не поддерживаемого запроса";
			//public static string _109 = "Ошибка параметров запроса";
			public static string _110 = "GitLab недоступен";
			public static string _111 = "NotSupportedException. Ошибка формата запроса";
			public static string _112 = "ArgumentNullException. Ошибка параметров запроса";
			public static string _113 = "SecurityException. Ошибка прав пользователя";
			public static string _114 = "UriFormatException. Ошибка формата URI";
			public static string _115 = "NotImplementedException. Ошибка определения метода запроса";
			public static string _116 = "OutOfMemoryException. Ошибка памяти";
			public static string _117 = "IOException. Ошибка ввода-вывода";
			public static string _118 = "Ошибка токена";
			public static string _119 = "Ошибка проверки обновления";
			public static string _120 = "Ошибка проверки обновления";
		}
		public class FileSystemError
		{
			public static string _200 (string plugin)
			{ 
				return "Не удалось распаковать архив плагина " + plugin;
			}
			public static string _201 (string plugin)
			{ 
				return "Не удалось удалить архив плагина " + plugin;
			}
			public static string _202 (string path)
			{ 
				return "Не удалось создать директорию " + path;
			}
			public static string _203 (string path)
			{ 
				return "Не удалось удалить директорию " + path;
			}
			public static string _204 (string plugin)
			{
				return "Ошибка импорта плагина " + plugin;
			}
			public static string _205 (string file)
			{
				return "Ошибка чтения конфиг файла \"" + file +"\"";
			}
		}
		public class ContentError
		{
			public static string _301 = "Некорректно скопированы мета файлы. " + Environment.NewLine + 
				"Включите 'Visible Meta files' в меню 'Edit->Project Settings->Editor'" + Environment.NewLine + 
					"Перезагрузите плагин.";
			public static string _302 = "Нет префаба 'СustomResourcesManager' на стартовой сцене. ";
			public static string _303 = "Нет скрипта 'СustomResourcesManager' на префабе 'СustomResourcesManager'";
			public static string _304 (string plugin)
			{ 
				return "Есть конфликты у плагина " + plugin;
			}
			public static string _305 (string plugin)
			{ 
				return "У плагина " + plugin + " устаревшая версия";
			}
			public static string _306 (string plugin)
			{ 
				return "У плагина " + plugin + " не хватает файлов";
			}
			public static string _307 (string plugin)
			{ 
				return "Файлы плагина " + plugin + " были изменены";
			}
			public static string _308 (string path)
			{ 
				return "Нет доступа к папке '" + path + "'. " + Environment.NewLine + 
					"Перенести хранение временных папок в проект? ";
			}
			public static string _309 (string path)
			{
				return  "Не удалось удалить плагин "+ path + Environment.NewLine +
					"из локального хранилища ";
			}
			public static string _310 (string link)
			{
				return  "Не была найдена ссылка для \"" + link +"\"";
			}
			public static string _311 = "Не удалось получить последнюю версию плагинов" +  Environment.NewLine +
				"Попробуйте нажать: AM Editor -> Проверить обновление плагинов";
			public static string _312 = "Не удалось восстановить сцену 'SceneCustomCode'" +  Environment.NewLine +
				"Пересоздайте сцену вручную и добавьте префаб 'CustomResourcesManager' если необходимо";
			public static string _313 = "Отсутствует файл 'am_project.txt' в папке 'Assets/StreamingAssets/'";
			public static string _314 = "Отсутствует файл 'am_builds.txt' в папке 'Assets/StreamingAssets/'";
		}
		public class HTTPClientError
		{
			public static string _400 (string url)
			{ 
				return url + Environment.NewLine + "400 Bad Request (\"плохой, негодный запрос\")";
			}
			public static string _401 (string url)
			{ 
				return url + Environment.NewLine + "401 Unauthorized (\"неавторизован\")";
			}
			public static string _402 (string url)
			{ 
				return url + Environment.NewLine + "402 Payment Required (\"необходима оплата\")";
			}
			public static string _403 (string url)
			{ 
				return url + Environment.NewLine + "403 Forbidden (\"запрещено\")";
			}
			public static string _404 (string url)
			{ 
				return url + Environment.NewLine + "404 Not Found (\"не найдено\")";
			}
			public static string _405 (string url)
			{ 
				return url + Environment.NewLine + "405 Method Not Allowed (\"метод не поддерживается\")";
			}
			public static string _406 (string url)
			{ 
				return url + Environment.NewLine + "406 Not Acceptable (\"неприемлемо\")";
			}
			public static string _407 (string url)
			{ 
				return url + Environment.NewLine + "407 Proxy Authentication Required (\"необходима аутентификация прокси\")";
			}
			public static string _408 (string url)
			{ 
				return url + Environment.NewLine + "408 Request Timeout (\"истекло время ожидания\")";
			}
			public static string _409 (string url)
			{ 
				return url + Environment.NewLine + "409 Conflict (\"конфликт\")";
			}
			public static string _410 (string url)
			{ 
				return url + Environment.NewLine + "410 Gone (\"удалён\")";
			}
			public static string _411 (string url)
			{ 
				return url + Environment.NewLine + "411 Length Required (\"необходима длина\")";
			}
			public static string _412 (string url)
			{ 
				return url + Environment.NewLine + "412 Precondition Failed (\"условие ложно\")";
			}
			public static string _413 (string url)
			{ 
				return url + Environment.NewLine + "413 Request Entity Too Large (\"размер запроса слишком велик\")";
			}
			public static string _414 (string url)
			{ 
				return url + Environment.NewLine + "414 Request-URI Too Large (\"запрашиваемый URI слишком длинный\")";
			}
			public static string _415 (string url)
			{ 
				return url + Environment.NewLine + "415 Unsupported Media Type (\"неподдерживаемый тип данных\")";
			}
			public static string _416 (string url)
			{ 
				return url + Environment.NewLine + "416 Requested Range Not Satisfiable (\"запрашиваемый диапазон не достижим\")";
			}
			public static string _417 (string url)
			{ 
				return url + Environment.NewLine + "417 Expectation Failed (\"ожидаемое неприемлемо\")";
			}
			public static string _418 (string url)
			{ 
				return url + Environment.NewLine + "418 I'm a teapot (\"я - чайник\")";
			}
			public static string _422 (string url)
			{ 
				return url + Environment.NewLine + "422 Unprocessable Entity (\"необрабатываемый экземпляр\")";
			}
			public static string _423 (string url)
			{ 
				return url + Environment.NewLine + "423 Locked (\"заблокировано\")";
			}
			public static string _424 (string url)
			{ 
				return url + Environment.NewLine + "424 Failed Dependency (\"невыполненная зависимость\")";
			}
			public static string _425 (string url)
			{ 
				return url + Environment.NewLine + "425 Unordered Collection (\"неупорядоченный набор\")";
			}
			public static string _426 (string url)
			{ 
				return url + Environment.NewLine + "426 Upgrade Required (\"необходимо обновление\")";
			}
			public static string _428 (string url)
			{ 
				return url + Environment.NewLine + "428 Precondition Required (\"необходимо предусловие\")";
			}
			public static string _429 (string url)
			{ 
				return url + Environment.NewLine + "429 Too Many Requests (\"слишком много запросов\")";
			}
			public static string _431 (string url)
			{ 
				return url + Environment.NewLine + "431 Request Header Fields Too Large (\"поля заголовка запроса слишком большие\")";
			}
			public static string _434 (string url)
			{ 
				return url + Environment.NewLine + "434 Requested host unavailable. (\"Запрашиваемый адрес недоступен\")";
			}
			public static string _449 (string url)
			{ 
				return url + Environment.NewLine + "449 Retry With (\"повторить с\")";
			}
			public static string _451 (string url)
			{ 
				return url + Environment.NewLine + "451 Unavailable For Legal Reasons (\"недоступно по юридическим причинам\")";
			}
			public static string _456 (string url)
			{ 
				return url + Environment.NewLine + "456 Unrecoverable Error (\"некорректируемая ошибка\").";
			}
			public static string _499 (string url)
			{ 
				return url + Environment.NewLine + "499 Используется Nginx, когда клиент закрывает соединение до получения ответа";
			}
		}
		public class HTTPServerError
		{
			public static string _500 (string url)
			{ 
				return url + Environment.NewLine + "500 Internal Server Error (\"внутренняя ошибка сервера\")";
			}
			public static string _501 (string url)
			{ 
				return url + Environment.NewLine + "501 Not Implemented (\"не реализовано\")";
			}
			public static string _502 (string url)
			{ 
				return url + Environment.NewLine + "502 Bad Gateway (\"плохой, ошибочный шлюз\")";
			}
			public static string _503 (string url)
			{ 
				return url + Environment.NewLine + "503 Service Unavailable (\"сервис недоступен\")";
			}
			public static string _504 (string url)
			{ 
				return url + Environment.NewLine + "504 Gateway Timeout (\"шлюз не отвечает\")";
			}
			public static string _505 (string url)
			{ 
				return url + Environment.NewLine + "505 HTTP Version Not Supported (\"версия HTTP не поддерживается\")";
			}
			public static string _506 (string url)
			{ 
				return url + Environment.NewLine + "506 Variant Also Negotiates (\"вариант тоже проводит согласование\")";
			}
			public static string _507 (string url)
			{ 
				return url + Environment.NewLine + "507 Insufficient Storage (\"переполнение хранилища\")";
			}
			public static string _508 (string url)
			{ 
				return url + Environment.NewLine + "508 Loop Detected (\"обнаружена петля\")";
			}
			public static string _509 (string url)
			{ 
				return url + Environment.NewLine + "509 Bandwidth Limit Exceeded (\"исчерпана пропускная ширина канала\")";
			}
			public static string _510 (string url)
			{ 
				return url + Environment.NewLine + "510 Not Extended (\"не расширено\")";
			}
			public static string _511 (string url)
			{ 
				return url + Environment.NewLine + "511 Network Authentication Required (\"требуется сетевая аутентификация\")";
			}
		}
		public class ContentCreateConfig
		{
			public static string _Title = "Создание plugin_config.json";
			public static string _ConfigExistQuestion = "Есть ли прошлая версия plugin_config.json?";
			public static string _SelectConfig = "Выберите файл plugin_config.json";

			public static string _MissingFilesTitle = "Обнаружены отсутствующие файлы плагина";
			public static string _MissingFilesQuestion = "Добавить в список устаревших?";

			public static string _DuplicatedFilesTitle = "Обнаружены файлы с одинаковыми именами";
			public static string _DuplicatedFilesMessage = "Отметки параметра \"Уникальный\" будут отключены для этих файлов";

			public static string _OutFolder = "Выберите папку для хранения plugin_config.json";
			
			public static string _MainFolder = "Основная папка плагина";
			public static string _MainFolderHelp = "Основная папка плагина";
			
			public static string _ToExportFolder = "Папка для Export'a";
			public static string _ToExportFolderHelp = "Папка для Export'a";

			public static string _PackageBuildType = "Тип текущей сборки";
			public static string _PackageBuildTypeHelp = "Тип текущей сборки плагина";

			public static string _PackageBuildNumber = "Номер текущей сборки";
			public static string _PackageBuildNumberHelp = "Номер текущей сборки (необязательно)";

			public static string _PackageExtraBuildOptions = "Дополнительные параметры сборки";
			public static string _PackageExtraBuildOptionsHelp = "Дополнительные параметры сборки";

			public static string _CreateConfigFile = "Создать plugin_config.json";
			public static string _CreateConfigFileHelp = "Создать plugin_config.json";
			
			public static string _CreateUnityPackage = "Создать .unitypackage";
			public static string _CreateUnityPackageHelp = "Создать .unitypackage";
			
			public static string _NamePlugin = "Название плагина : ";
			public static string _NamePluginHelp = "Название плагина в GitLab";
			
			public static string _OldNames = "Устаревшие названия плагина ";
			public static string _OldNamesHelp = "Устаревшие названия плагина : ";
			
			public static string _CopyFiles = "Скопировать файлы плагина";
			public static string _CopyFilesHelp = "Скопировать файлы плагина";

			public static string _EditFilesList = "Редактировать файлы";
			public static string _Back = "Назад";
			public static string _SelectFolder = "Выбрать папку";
			
			public static string _ViewType = "Тип отображения : ";
			public static string _ViewTypeHelp = "Тип отображения, при котором будет виден плагин";

			public static string _PluginBuildType = "Тип(ы) сборки : ";
			public static string _PluginBuildTypeHelp = "Список поддерживаемых плагином типов сборки. Через пробел. Оставить пустым для поддержки всех типов сборки";

			public static string _IsMandatory = "Обязательный плагин : ";
			public static string _IsMandatoryHelp = "Является ли плагин обязательным для подключения";
			
			public static string _Link = "Ссылка : ";
			public static string _LinkHelp = "Ссылка на проект плагина в GitLab, например, http://pgit.digital-ecosystems.ru/unity-plugins/plugin-name";
			
			public static string _PluginVersion = "Версия плагина : ";
			public static string _PluginVersionHelp = "Версия плагина";
			
			public static string _DependsPlugin = "Зависимости ";
			public static string _DependsPluginHelp = "Название плагинов, от которых зависит текущий плагин";

			public static string _CoreFiles = "Файлы плагина ";//"Основные файлы ";
			public static string _CoreFilesHelp = "Основные файлы плагина";

			public static string _Example = "Пример";
			public static string _ExampleHelp = "Является ли файл примером использования плагина";
			public static string _ExampleFiles = "Примеры ";
			public static string _ExampleFilesHelp = "Файлы с примерами использования плагина";

			public static string _Modifiable = "Изменяемый";
			public static string _ModifiableHelp = "Является ли файл изменяемым"+System.Environment.NewLine+"Файл можно изменить";
			public static string _ModifiableFiles = "Изменяемые файлы ";
			public static string _ModifiableFilesHelp = "Файлы плагина, содержимое которых можно изменять";
			
			public static string _DefaultNameDepend = "Plugin Name";
			public static string _DefaultOldName = "Old Name";
			
			public static string _CreateConfig = "Создать";
			
			public static string _AddDepend = "Добавить зависимость";
			public static string _AddDependHelp = "Добавить зависимость";
			
			public static string _AddOutdated = "Добавить файл";
			
			public static string _AddOldName = "Добавить устаревшее имя";
			public static string _AddOldNameHelp = "Добавить устаревшее имя";

			public static string _AddCore = "Добавить файлы плагина";//основные файлы";
			public static string _AddCoreHelp = "Добавить основные файлы";

			public static string _AddExample = "Добавить примеры";
			public static string _AddExampleHelp = "Добавить примеры";

			public static string _AddModifiable = "Добавить изменяемые файлы";
			public static string _AddModifiableHelp = "Добавить изменяемые файлы";

			public static string _DragAndDropAreaCoreText = "Переместите сюда основные файлы(папки) плагина ";
			public static string _DragAndDropAreaExampleText = "Переместите сюда файлы(папки)-примеры для плагина ";
			public static string _DragAndDropAreaModifiableText = "Переместите сюда изменяемые файлы(папки) плагина ";
			public static string _DragAndDropAreaOutdatedText = "Переместите сюда устаревшие файлы(папки) плагина ";
			
			public static string _CurrentFilesPlugins = "Текущие файлы плагина";
			public static string _OutdatedFilesPlugins = "Устаревшие файлы плагина";

			public static string _SelectAsset = "Выбрать";
			public static string _SelectAssetHelp = "Выберите файлы и папки, входящие в состав плагина";
			
			public static string _Unique = "Уникальный";
			public static string _UniqueHelp = "Является ли файл уникальным для проекта"+System.Environment.NewLine+"Убрать, если есть или будут файлы с таким же именем, но по другому пути";

			public static string _Path = "Полный путь к файлу";

			public static string _EmptyMainFolderTitle = "Не указана основная папка плагина";
			public static string _EmptyMainFolderMessage = "Укажите путь до основной папки плагина в проекте";

			public static string _BadExportFolderTitle = "Неверный путь для экспорта";
			public static string _BadExportFolderMessage = "Путь для экспорта не может быть пустым или вести внутрь папки \"Assets\"";

			public static string _IncorrectParametersDetected = "Обнаружены не правильные параметры";
		}
		public class ContentCreatePublicFile
		{
			public static string _Title = "Создание ameditor_plugins.json";
			public static string _OutFolder = "Выберите папку для хранения ameditor_plugins.json";

			public static string _IsMandatory = "Обязательный"+Environment.NewLine+"плагин";
			public static string _IsMandatoryHelp = "Является ли плагин обязательным для подключения";
			public static string _IsMandatoryTrue = "Обязательный";
			public static string _IsMandatoryFalse = "Необязательный";

			public static string _PluginName = "Название плагина";
			public static string _PluginNameHelp = "Название плагина в GitLab";

			public static string _SortByNameHelp = "Сортировка плагинов по имени";

			public static string _PluginVersion = "Версия плагина";
			public static string _PluginVersionHelp = "Актуальная версия плагина";

			public static string _SortByVersionHelp = "Сортировка плагинов по версии";

			public static string _MinimumVersion = "Минимальная версия";
			public static string _MinimumVersionHelp = "Будут загружены версии с выбранной по новейшую";
			
			public static string _OldNames = "Устаревшие названия";
			public static string _OldNamesHelp = "Устаревшие названия плагина";

			public static string _BuildTypes = "Типы сборки";
			public static string _BuildTypesHelp = "Типы сборок, поддерживаемые данной версией плагина";
			public static string _BuildTypesNoneHelp = "Тип сборки не указан. Плагин поддерживает любой тип сборки";

			public static string _DisplayType = "Тип отображения";
			public static string _DisplayTypeHelp = "Тип отображения, при котором будет виден плагин";
			
			public static string _Depends = "Зависимые плагины";
			public static string _DependsHelp = "Список зависимых плагинов";

			public static string _Url = "GitLab";
			public static string _UrlHelp = "Ссылка на плагин";

			public static string _VersionFilter = "Фильтр версий";
			public static string _VersionFilterHelp = "Число отображаемых версий плагинов для каждого типа сборки";
			public static string _DefaultVersionFilter = "Все";
			public static string _ActualVersionFilter = "Только актуальные";
			public static string _OtherVersionFilter = "Актуальные +";

			public static string _ReleaseOnly = "Только релизные";
			public static string _ReleaseOnlyHelp = "Не отобпажать плагины Dev и RC";

			public static string _AddPlugin = "Добавить плагин";
			public static string _AddPluginHelp = "Добавить плагин для публичного файла";

			public static string _Create = "Создать";
			public static string _CreateAndPush = "Создать и отправить в Git";
			public static string _CreateHelp = "Создать публичный файл";

			public static string _SelectHelp = "Выбранный плагин будет добавлен в ameditor_plugins.json";

			public static string _SelectAll = "Все";
			public static string _SelectAllHelp = "Выбрать все плагины";

			public static string _DeselectAll = "Ни одного";
			public static string _DeselectAllHelp = "Убрать отметки у всех плагинов";

			public static string _UpdateFromGit = "Обновить";
			public static string _UpdateFromGitHelp = "Обновить информацию о плагинах с GitLab";

			public static string _ForceUpdateFromGit = "Полная проверка";
			public static string _ForceUpdateFromGitHelp_True = "Будет произведен поиск по всем версиям плагинов";
			public static string _ForceUpdateFromGitHelp_False = "Будет произведен поиск только по последним версиям плагинов";

			public static string _SuccessDialogTitle = "Подготовка завершена";
			public static string _SuccessDialogMessage = "Подготовка плагинов для ameditor_plugins.json завершена";

			public static string _FailedDialogTitle = "Подготовка не удалась";
			public static string _FailedDialogMessage = "Не удалось составить ameditor_plugins.json. Будет отображен локальный файл";

			public static string _CanceledDialogTitle = "Подготовка отменена";
			public static string _CanceledDialogMessage = "Подготовка плагинов для ameditor_plugins.json отменена пользователем. Будет отображен локальный файл";

			public static string _PluginsCount = "Всего плагинов: ";
			public static string _SelectedPlugins = "Выбрано плагинов: ";

			public static string _EmptyCommitTitle = "Ошибка отправки файла";
			public static string _EmptyCommitMessage = "Сообщение к коммиту не может быть пустым!";

			public static string _PushingConfigToggle = " Отправить конфиг файл в Git после создания";
			public static string _PushingConfigToggleHelp = "ветка: ";

			public static string _PushingConfigTitle = "Отправка конфига";
			public static string _PushingConfigSuccessDialog (string fileName)
			{
				return "Конфиг файл " + fileName + " успешно отправлен";
			}
			public static string _PushingConfigFailedDialog (string fileName)
			{
				return "Отправка конфиг файла " + fileName + " не удалась";
			}

			public static string _RetryConfigSearchButton = "Попробовать снова";
		}
		public class ContentStatuses
		{
			public static string _CheckingBranch = "Проверка веток проекта";
			public static string _GettingLastCommit = "Получение последнего коммита";
			public static string _GettingTags = "Получение списка версий проекта";
			public static string _DownloadingArchive = "Загрузка архива";
			public static string _ExtractingArchive = "Распаковка архива";
			public static string _DeletingArchive = "Удаление архива";
			public static string _ImportingPlugin = "Импорт файлов плагина в проект";

            public static string _DownloadingUnitypackage = "Загрузка unitypackage";
            public static string _ImportingUnitypackage = "Импорт unitypackage в проект";

            public static string _LoadingDefaultsForCQS = "Загрузка параметров";
            public static string _ReadingDefaultsForCQS = "Обработка параметров";
            public static string _SettingDefaultsForCQS = "Установка параметров";

            public static string _CheckingLocalConfig = "Проверка локального файла";
			public static string _GettingGroupsList = "Получение списка групп";
			public static string _SearchingNeededGroup = "Поиск группы: ";
			public static string _GettingGroupProjects = "Получение списка проектов в группе ";
			public static string _SearchingCompatibleProjects = "Поиск совместимых проектов";
			public static string _WorkingWithCompatible = "Получение совместимых плагинов";
			public static string _SearchingPluginConfig = "Поиск plugin_config.json";
			public static string _GettingPluginConfig = "Получение plugin_config.json";

			public static string _RestoringCRM = "Восстановление Custom Resources Manager'a";

			public static string _DontCloseAMEditor = "Пожалуйста, не закрывайте окно AM Editor'a";
		}
		public class ContentRestoreDefaults
		{
			public const string _RestoreDefaultSettingsMenu = "AM Editor/Настройки/Настройки по-умолчанию";
			public static string _TitleDialog = "Восстановление настроек по-умолчанию";
			public static string _RestoreDefaultSettingsQuestion = "Будут восстановлены настройки по-умолчанию для AM Editor'a."
				+System.Environment.NewLine+"Продолжить?";
			public static string _MessageSuccessDialog = "Настройки по-умолчанию для AM Editor'a успешно восстановлены";
		}

		public static string _ContentClose = "Закрыть";
		public static string _ContentCloseHelp = "Закрыть текущее окно";

		public static string _ContentBackToPlugins = "Назад к плагинам";
		public static string _ContentBackToPluginsHelp = "Перейти к окну списка плагинов";
		
		public static string _ContentListOfFiles = "Файлы";//"Cписок файлов";
		public static string _ContentListOfFilesHelp = "Показать список файлов плагина ";
		
		public static string _ContentIncludedPlugins = "Входящие плагины";
		public static string _ContentIncludedPluginsHelp = "Показать список плагинов, входящих в ";

		public static string _ContentExamples = "Примеры";
		public static string _ContentExamplesHelp = "Показать список примеров использования плагина ";

		public static string _ContentButtonUpdateList = "Обновить список";
		public static string _ContentButtonUpdateListHelp = "Обновить список ";
		
		public static string _ContentTitleWindowListPlugins = "Список плагинов";
		public static string _ContentTitleWindowListPluginsHelp = "Список плагинов ";
		
		public static string _ContentTitleWindowFixConflict = "Исправление конфликтов";
		public static string _ContentTitleWindowFixConflictHelp = "Исправление конфликтов ";
		
		public static string _ContentTurn = "Свернуть";
		public static string _ContentTurnHelp = "Свернуть";
		
		public static string _ContentExpand = "Развенуть";
		public static string _ContentExpandHelp = "Развенуть";
		
		public static string _ContentDeleteFiles = "Удалить файлы";
		public static string _ContentDeleteFilesHelp = "Удалить файлы";
		
		public static string _ContentDeleteAllFiles = "Удалить все файлы";
		public static string _ContentDeleteAllFilesHelp = "Удалить все файлы";
		
		public static string _ContentOpenFile = "Открыть";
		public static string _ContentOpenFileHelp = "Открыть файл ";
		
		public static string _ContentRestoreFiles = "Восстановить файлы";
		public static string _ContentRestoreFilesHelp = "Восстановить файлы";
		
		public static string _ContentNonePlugin = "Ссылка на плагин";
		public static string _ContentNonePluginHelp = "Ссылка на плагин";
		
		public static string _ContentDeletePlugin = "Удалить плагин";
		public static string _ContentDeletePluginHelp = "Удалить плагин";
		
		public static string _ContentTitleDeletePlugin = "Удаление файлов плагина";
		public static string _ContentTitleDeletePluginHelp = "Удаление файлов плагина";
		
		public static string _ContentQuestionDeletePlugin = "Вы действительно хотите удалить ВСЕ файлы плагина ";
		public static string _ContentQuestionDeleteSelectedPlugin = "Вы действительно хотите удалить ВСЕ файлы выбранных плагинов?";
		public static string _ContentQuestionDeletePluginHelp = "Вы действительно хотите удалить ВСЕ файлы плагина";

		public static string _ContentTitleDeleteDepends = "Удаление зависимостей плагина";
		public static string _ContentTitleDeleteDependsHelp = "Удаление зависимостей плагина";
		public static string _ContentQuestionDeleteDepends (string pluginName)
		{
			return "У плагина "+pluginName+" обнаружены зависимые плагины.\nХотите удалить только текущий плагин или вместе с зависимостями?";
		}

		public static string _ContentQuestionSeveralPlugins = "Обнаружено несколько удаляемых плагинов с зависимостями.\nПрименить выбранное действие ко всем таким плагинам?";

		public static string _ContentTitleDeletingFiles = "Удаление файлов";
		public static string _ContentTitleDeletingFilesHelp = "Удаление файлов";
		
		public static string _ContentQuestionDeleteFiles = "Вы действительно хотите удалить файлы?";
		public static string _ContentQuestionDeleteFilesHelp = "Вы действительно хотите удалить файлы?";
		
		public static string _ContentBackups = "Резервные копии";
		public static string _ContentBackupsHelp = "Резервные копии";
		
		public static string _ContentDateTimeBackup = "Время создания резевной копии : ";
		public static string _ContentDateTimeBackupHelp = "Время создания резевной копии :";
		
		public static string _ContentDeleteBackup = "Удалить резервную копию";
		public static string _ContentDeleteBackupHelp = "Удалить резервную копию";
		
		public static string _ContentTitleDeletingBackup = "Удаление резервной копии";
		public static string _ContentTitleDeletingBackupHelp = "Удаление резервной копии";
		
		public static string _ContentQuestionDeleteBackup = "Вы дейстительно хотите удалить резервную копию?";
		public static string _ContentQuestionDeleteBackupHelp = "Вы дейстительно хотите удалить резервную копию?";
		
		public static string _ContentDeleteBackups = "Удалить все резерные копии";
		public static string _ContentDeleteBackupsHelp = "Удалить все резерные копии";
		public static string _ContentDeleteBackupsCompact = " Все резерные копии";
		
		public static string _ContentTitleDeletingBackups = "Удаление всех резерных копий";
		public static string _ContentTitleDeletingBackupsHelp = "Удаление всех резерных копий";
		
		public static string _ContentTitleRestoredBackups = "Восстановление файлов";
		public static string _ContentAlertRestoredBackups = "Файлы были успешно восстановленны";

		public static string _ContentBackupsCount = "Число резервных копий: ";
		
		public static string _ContentQuestionDeleteBackups = "Вы действительно хотите удалить ВСЕ резервные копии?";
		public static string _ContentQuestionDeleteBackupsHelp = "Вы действительно хотите удалить ВСЕ резервные копии?";

		public static string _ContentSupport = "Написать в поддержку";
		public static string _ContentDownloadActual = "Загрузить актуальную версию";
#endif
    }
}
#endif