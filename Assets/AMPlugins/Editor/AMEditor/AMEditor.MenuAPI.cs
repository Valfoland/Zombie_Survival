#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMEditor
{
	[InitializeOnLoad]
	public class AMEditorMenuAPI
	{
		static AMEditorMenuAPI ()
		{
			#if UNITY_5 && !UNITY_5_5_OR_NEWER
			EditorApplication.delayCall += () => {SetMarks ();};
			#endif
		}

		public enum ViewType
		{
			EXTENDED,
			MINIMAL
		}
		public enum CompactModeState
		{
			ON,
			OFF
		}
		public enum PluginsType
		{
			DEV,
			RC,
			RELEASE
		}
		public enum SupportLanguage
		{
			EN,
			RU
		}
		public enum DebugModeState
		{
			ON,
			OFF
		}

		public static string viewPrefix = "AM_EDITOR_VIEW_TYPE";
		public static string compactModePrefix = "AM_EDITOR_COMPACT";
		public static string pluginsTypePrefix = "AM_EDITOR_PLUGINS_TYPE";
		public static string languagePrefix = "AM_EDITOR_LANGUAGE";
		public static string debugModePrefix = "AM_EDITOR_DEBUG_MODE";

		public static void SetMarks ()
		{
			#if AM_EDITOR_LANGUAGE_EN
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuItem._LanguageRU, false);
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuItem._LanguageEN, true);
			#else
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuItem._LanguageRU, true);
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuItem._LanguageEN, false);
			#endif

			#if AM_EDITOR_PLUGINS_TYPE_DEV
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuPluginsType._Dev, true);
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuPluginsType._RC, false);
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuPluginsType._Release, false);
			#elif AM_EDITOR_PLUGINS_TYPE_RC
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuPluginsType._Dev, false);
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuPluginsType._RC, true);
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuPluginsType._Release, false);
			#else
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuPluginsType._Dev, false);
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuPluginsType._RC, false);
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuPluginsType._Release, true);
			#endif

			#if AM_EDITOR_COMPACT_ON
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuViewType._ViewTypeCompact, true);
			#else
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuViewType._ViewTypeCompact, false);
			#endif

			#if AM_EDITOR_VIEW_TYPE_EXTENDED
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuViewType._ViewTypeExtended, true);
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuViewType._ViewTypeMinimal, false);
			#else
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuViewType._ViewTypeExtended, false);
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentMenuViewType._ViewTypeMinimal, true);
			#endif

			#if AM_EDITOR_DEBUG_MODE_ON
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentDebugMode._DebugMode, true);
			#else
			UnityEditor.Menu.SetChecked (AMEditorSystem.ContentDebugMode._DebugMode, false);
			#endif
		}

		#region View
		public static void ChangeViewType (ViewType newType)
		{
			if (!EditorUtility.DisplayDialog(AMEditorSystem.ContentViewType._TitleDialog, AMEditorSystem.ContentViewType._ChangeViewTypeQuestion, 
				AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._Cancel))
			{
				return;
			}

			UnityCompileHandler.SetHandlerName ("ViewTypeChanged");

			AMEditorDefineController.UpdateDefine (newType, viewPrefix);

			if (AMEditor.WindowMain.isInit)
			{
				AMEditor.WindowMain.Init ();
			}
		}

		public static void SwitchCompactView ()
		{
			string message = (AMEditor.WindowMain.instance != null) ? AMEditorSystem.ContentViewType._SwitchCompactModeMessage () : string.Empty;

			if (EditorUtility.DisplayDialog(AMEditorSystem.ContentViewType._TitleDialog, AMEditorSystem.ContentViewType._ChangeViewTypeQuestion + message, 
				AMEditorSystem.ContentStandardButton._Yes, AMEditorSystem.ContentStandardButton._Cancel))
			{
				UnityCompileHandler.SetHandlerName ("ViewTypeChanged");
				AMEditor.WindowMain.compilationHandlerName = "ViewTypeChanged";

				if (AMEditor.WindowMain.instance != null)
					AMEditor.WindowMain.instance.Close ();

				#if AM_EDITOR_COMPACT_ON
				CompactModeState compactModeState = CompactModeState.OFF;
				#else
				CompactModeState compactModeState = CompactModeState.ON;
				#endif

				SwitchCompactMode (compactModeState);

				if (AMEditor.WindowMain.isInit)
				{
					AMEditor.WindowMain.Init ();
				}
			}
		}

		static void SwitchCompactMode (CompactModeState newState)
		{
			AMEditorDefineController.UpdateDefine (newState, compactModePrefix);
		}
		#endregion

		#region Plugins Type
		public static void ChangePluginsType (PluginsType newType)
		{
			UnityCompileHandler.SetHandlerName ("PluginsTypeChanged");

			AMEditorDefineController.UpdateDefine (newType, pluginsTypePrefix);

			if (AMEditor.WindowMain.isInit)
			{
				AMEditor.WindowMain.Init ();
			}
		}
		#endregion

		#region Language
		public static void ChangeLanguage (SupportLanguage newLanguage)
		{
			UnityCompileHandler.SetHandlerName ("LanguageChanged");

			AMEditorDefineController.UpdateDefine (newLanguage, languagePrefix);

			if (AMEditor.WindowMain.isInit)
			{
				AMEditor.WindowMain.Init ();
			}
		}
		#endregion

		#region Debug Mode
		public static void SwitchDebugMode ()
		{
			#if AM_EDITOR_DEBUG_MODE_ON
			DebugModeState debugModeState = DebugModeState.OFF;
			#else
			DebugModeState debugModeState = DebugModeState.ON;
			#endif

			SwitchDebugModeDefine (debugModeState);

			if (AMEditor.WindowMain.isInit)
			{
				AMEditor.WindowMain.Init ();
			}
		}

		static void SwitchDebugModeDefine (DebugModeState newState)
		{
			AMEditorDefineController.UpdateDefine (newState, debugModePrefix);
		}
		#endregion

		public static void RestoreAMEditorDefaultSettings ()
		{
			UnityCompileHandler.SetHandlerName ("RestoreDefaults");

			List<string> definesToRemove = new List<string> ();
			definesToRemove.Add (languagePrefix);
			definesToRemove.Add (pluginsTypePrefix);
			definesToRemove.Add (viewPrefix);
			definesToRemove.Add (compactModePrefix);
			definesToRemove.Add (debugModePrefix);
			definesToRemove.Add (CustomResourceManagerAPI.assetBundlesDefine);
			definesToRemove.Add (WindowCreateConfigFile.hideDuplicatesDialogDefine);
			//definesToRemove.Add (AMEditor.DLC.dlcDefinePrefix);

			bool definesToRemoveDetected = false;
			try
			{
				var buildTargetGroupsList = Enum.GetValues (typeof (BuildTargetGroup)).OfType<BuildTargetGroup> ().ToList ();
				foreach (var buildTargetGroup in buildTargetGroupsList) 
				{
					if (!AMEditorDefineController.BuildTargetGroupIsAvailable (buildTargetGroup))
						continue;
					string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (buildTargetGroup);
					string tempDefine = string.Empty;
					foreach (var define in definesToRemove)
					{
						if (defineSymbols.Contains (define))
						{
							definesToRemoveDetected = true;
							tempDefine = AMEditorDefineController.RemoveOldValue (defineSymbols, define);
							defineSymbols = tempDefine;
						}
					}
					PlayerSettings.SetScriptingDefineSymbolsForGroup (buildTargetGroup, defineSymbols);
				}
				if (definesToRemoveDetected)
				{
					AssetDatabase.Refresh ();
					if (AMEditor.WindowMain.instance != null)
						AMEditor.WindowMain.instance.Close ();
				}
				else
				{
					UnityCompileHandler.CompilationCompleteHandler ();
				}
			}
			catch(Exception)
			{
				UnityCompileHandler.CompilationCompleteHandler ();
			}
		}
	}
}
#endif