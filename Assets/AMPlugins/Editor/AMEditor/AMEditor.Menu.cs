#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AMEditor
{
	public class AMEditorMenu
	{
		class MainWindow
		{
			[MenuItem (AMEditorSystem.ContentMenuItem._ShowAMEditor, false, 1)]
			static void ShowAMEditorWindow ()
			{
				AMEditor.WindowMain.Show ();
			}

			[MenuItem (AMEditorSystem.ContentMenuItem._CheckUpdate, false, 2)]
			static void CheckUpdate () 
			{
				AMEditor.WindowMain.CheckUpdate ();
			}

			[MenuItem (AMEditorSystem.ContentMenuItem._Backups, false, 103)]
			static void ShowBackups ()
			{
				AMEditor.WindowMain.ShowBackups ();
			}
		}

		#if CUSTOM_CODE_ASSET_BUNDLES
		class AssetBundles
		{
			[MenuItem (AMEditorSystem.ContentMenuItem._MakeAssetBundle, false, 21)]
			static void MakeAssetBundleForCurrentTarget ()
			{
				MakeAssetBundles ();
			}

			[MenuItem (AMEditorSystem.ContentMenuItem._MakeAssetBundleForAll, false, 22)]
			static void MakeAssetBundleForAllTargets ()
			{
				MakeAssetBundles (true);
			}

			static void MakeAssetBundles (bool forAll = false)
			{
				if (File.Exists (CustomResourceManagerAPI.assetBundlesScriptPath))
				{
                    int sz = EditorBuildSettings.scenes.Count();
                    sz = sz / 10;
                    char dop = (forAll) ?'1':'0';
                    if (sz == 0) { EditorWindow.GetWindowWithRect(typeof(MyWindow), new Rect((Screen.width - 300) / 2, (Screen.height - 200) / 2, 300, 200), false, dop+"Выбор сцен"); }
                    {EditorWindow.GetWindowWithRect(typeof(MyWindow), new Rect((Screen.width - 300) / 2, (Screen.height - (200 + sz * 70)) / 2, 300, 200 + sz * 70), false, dop+"Выбор сцен"); }
                }
            }
		}
		#endif
			
		class Fixes
		{
			[MenuItem (AMEditorSystem.ContentMenuItem._RestoreCRM, false, 50)]
			static void RecreateCRMComponents ()
			{
				CustomResourceManagerAPI.RestoreCRMComponents (true);
			}

			[MenuItem (AMEditorSystem.ContentMenuItem._FixMetaALibs, false, 51)]
			static void FixAppleNativeLibs ()
			{
				AMEditor.WindowMain.FixMetaA ();
			}

			[MenuItem (AMEditorSystem.ContentMenuItem._FixMetaBundle, false, 52)]
			static void FixAppleBundleFiles ()
			{
				AMEditor.WindowMain.FixMetaBundle ();
			}
		}

		class ForDevelopers
		{
			[MenuItem (AMEditorSystem.ContentMenuItem._WindowCreateConfigFile, false, 53)]
			static void CreateConfigFile ()
			{
				WindowCreateConfigFile.Init ();
			}

			[MenuItem (AMEditorSystem.ContentMenuItem._WindowCreateConfigALL, false, 54)]
			static void CreatePublicConfig ()
			{
				WindowCreatePublicConfig.Init ();
			}

			[MenuItem (AMEditorSystem.ContentMenuItem._SetDefaultsForCustomQualitySetup, false, 55)]
			static void SetDefaultsForCQS ()
			{
				AMEditorCustomQualitySetupAPI.SetDefaults ();
			}
		}

		class LocalRepository
		{
			[MenuItem (AMEditorSystem.ContentMenuLocalRepository._Open, false, 101)]
			static void MenuOpen ()
			{
				UnityEditor.EditorUtility.OpenWithDefaultApp (LocalRepositoryAPI.pathToRepository);
			}

			[MenuItem (AMEditorSystem.ContentMenuLocalRepository._Clean, false, 102)]
			static void MenuClean ()
			{
				new UI.AMDisplayDialog (AMEditorSystem.ContentMenuLocalRepository._DeleteTitle, AMEditorSystem.ContentMenuLocalRepository._DeleteQuestion, 
					AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._No, LocalRepositoryAPI.DeleteLocalRepositories, () => {}, true).Show ();
			}
		}

		class Settings
		{
			class View
			{
				[MenuItem (AMEditorSystem.ContentMenuViewType._ViewTypeMinimal, false, 104)]
				static void Minimal ()
				{
					#if AM_EDITOR_VIEW_TYPE_EXTENDED
					AMEditorMenuAPI.ChangeViewType (AMEditorMenuAPI.ViewType.MINIMAL);
					#endif
				}
				[MenuItem (AMEditorSystem.ContentMenuViewType._ViewTypeMinimal, true, 104)]
				static bool MinimalAvailability ()
				{
					#if AM_EDITOR_VIEW_TYPE_EXTENDED
					return true;
					#else
					return false;
					#endif
				}

				[MenuItem (AMEditorSystem.ContentMenuViewType._ViewTypeExtended, false, 105)]
				static void Extended ()
				{
					#if !AM_EDITOR_VIEW_TYPE_EXTENDED
					AMEditorMenuAPI.ChangeViewType (AMEditorMenuAPI.ViewType.EXTENDED);
					#endif
				}
				[MenuItem (AMEditorSystem.ContentMenuViewType._ViewTypeExtended, true, 105)]
				static bool ExtendedAvailability ()
				{
					#if AM_EDITOR_VIEW_TYPE_EXTENDED
					return false;
					#else
					return true;
					#endif
				}

				[MenuItem (AMEditorSystem.ContentMenuViewType._ViewTypeCompactOn, false, 126)]
				static void TurnCompactViewOn ()
				{
					AMEditorMenuAPI.SwitchCompactView ();
				}
				[MenuItem (AMEditorSystem.ContentMenuViewType._ViewTypeCompactOn, true, 126)]
				static bool CompactViewOnAvailability ()
				{
					#if AM_EDITOR_COMPACT_ON
					return false;
					#else
					return true;
					#endif
				}
				[MenuItem (AMEditorSystem.ContentMenuViewType._ViewTypeCompactOff, false, 127)]
				static void TurnCompactViewOff ()
				{
					AMEditorMenuAPI.SwitchCompactView ();
				}
				[MenuItem (AMEditorSystem.ContentMenuViewType._ViewTypeCompactOff, true, 127)]
				static bool CompactViewOffAvailability ()
				{
					#if AM_EDITOR_COMPACT_ON
					return true;
					#else
					return false;
					#endif
				}
			}

			class PluginsType
			{
				[MenuItem (AMEditorSystem.ContentMenuPluginsType._Dev, false, 108)]
				static void DevPluginsType ()
				{
					#if !AM_EDITOR_PLUGINS_TYPE_DEV
					AMEditorMenuAPI.ChangePluginsType (AMEditorMenuAPI.PluginsType.DEV);
					#endif
				}
				[MenuItem (AMEditorSystem.ContentMenuPluginsType._Dev, true, 108)]
				static bool DevPluginsTypeAvailability ()
				{
					#if AM_EDITOR_PLUGINS_TYPE_DEV
					return false;
					#elif AM_EDITOR_PLUGINS_TYPE_RC
					return true;
					#else 
					return true;
					#endif
				}

				[MenuItem (AMEditorSystem.ContentMenuPluginsType._RC, false, 109)]
				static void RCPluginsType ()
				{
					#if !AM_EDITOR_PLUGINS_TYPE_RC
					AMEditorMenuAPI.ChangePluginsType (AMEditorMenuAPI.PluginsType.RC);
					#endif
				}
				[MenuItem (AMEditorSystem.ContentMenuPluginsType._RC, true, 109)]
				static bool RCPluginsTypeAvailability ()
				{
					#if AM_EDITOR_PLUGINS_TYPE_DEV
					return true;
					#elif AM_EDITOR_PLUGINS_TYPE_RC
					return false;
					#else 
					return true;
					#endif
				}
		
				[MenuItem (AMEditorSystem.ContentMenuPluginsType._Release, false, 110)]
				static void ReleasePluginsType ()
				{
					#if AM_EDITOR_PLUGINS_TYPE_DEV || AM_EDITOR_PLUGINS_TYPE_RC
					AMEditorMenuAPI.ChangePluginsType (AMEditorMenuAPI.PluginsType.RELEASE);
					#endif
				}
				[MenuItem (AMEditorSystem.ContentMenuPluginsType._Release, true, 110)]
				static bool ReleasePluginsTypeAvailability ()
				{
					#if AM_EDITOR_PLUGINS_TYPE_DEV
					return true;
					#elif AM_EDITOR_PLUGINS_TYPE_RC
					return true;
					#else 
					return false;
					#endif
				}
			}

			class Language
			{
				[MenuItem (AMEditorSystem.ContentMenuItem._LanguageRU, false, 111)]
				static void SetLanguageRU ()
				{
					#if AM_EDITOR_LANGUAGE_EN
					AMEditorMenuAPI.ChangeLanguage (AMEditorMenuAPI.SupportLanguage.RU);
					#endif
				}
				[MenuItem (AMEditorSystem.ContentMenuItem._LanguageRU, true, 111)]
				static bool SetLanguageRUAvalability ()
				{
					#if AM_EDITOR_LANGUAGE_EN
					return true;
					#else
					return false;
					#endif
				}

				[MenuItem (AMEditorSystem.ContentMenuItem._LanguageEN, false, 112)]
				static void SetLanguageEN ()
				{
					#if !AM_EDITOR_LANGUAGE_EN
					AMEditorMenuAPI.ChangeLanguage (AMEditorMenuAPI.SupportLanguage.EN);
					#endif
				}
				[MenuItem (AMEditorSystem.ContentMenuItem._LanguageEN, true, 112)]
				static bool SetLanguageENAvailability ()
				{
					#if AM_EDITOR_LANGUAGE_EN
					return false;
					#else
					return true;
					#endif
				}
			}

			class DebugMode
			{
				[MenuItem (AMEditorSystem.ContentDebugMode._DebugModeOn, false, 136)]
				static void TurnDebugModeOn ()
				{
					AMEditorMenuAPI.SwitchDebugMode ();
				}
				[MenuItem (AMEditorSystem.ContentDebugMode._DebugModeOn, true, 136)]
				static bool TurnDebugModeOnAvailability ()
				{
					#if AM_EDITOR_DEBUG_MODE_ON
					return false;
					#else
					return true;
					#endif
				}
				[MenuItem (AMEditorSystem.ContentDebugMode._DebugModeOff, false, 137)]
				static void TurnDebugModeOff ()
				{
					AMEditorMenuAPI.SwitchDebugMode ();
				}
				[MenuItem (AMEditorSystem.ContentDebugMode._DebugModeOff, true, 137)]
				static bool TurnDebugModeOffAvailability ()
				{
					#if AM_EDITOR_DEBUG_MODE_ON
					return true;
					#else
					return false;
					#endif
				}
			}

			[MenuItem (AMEditorSystem.ContentRestoreDefaults._RestoreDefaultSettingsMenu, false, 148)]
			static void RestoreDefaults ()
			{
				new AMEditor.UI.AMDisplayDialog(AMEditorSystem.ContentRestoreDefaults._TitleDialog, 
					AMEditorSystem.ContentRestoreDefaults._RestoreDefaultSettingsQuestion, 
					AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._No, 
					() => { AMEditorMenuAPI.RestoreAMEditorDefaultSettings (); }, () => {}, true).Show ();
			}
		}

		class Help
		{
			[MenuItem (AMEditorSystem.ContentMenuItem._HelpWiki, false, 151)]
			static void AboutWiki ()
			{
				HelpAPI.SearchLink (HelpAPI.TypeAbout.Tutorial);
			}

			[MenuItem (AMEditorSystem.ContentMenuItem._HelpSupport, false, 152)]
			static void AboutSupport ()
			{
				HelpAPI.SearchLink (HelpAPI.TypeAbout.SkypeSupport);
			}

			[MenuItem (AMEditorSystem.ContentMenuItem._HelpChangeLog, false, 153)]
			static void AboutChangeLog ()
			{
				HelpAPI.SearchLink (HelpAPI.TypeAbout.ChangeLog);
			}
		}
	}
}
#endif