#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace AMEditor
{
	public class UnityCompileHandler
	{
		static string handlerKey = "compilationHandlerName";

		public static void SetHandlerName (string eventName)
		{
			Hashtable am_editor_state = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AMEditor.WindowMain.FILE_NAME_STATE_EDITOR)) as Hashtable;
			am_editor_state.Remove (handlerKey);
			am_editor_state.Add (handlerKey, eventName);
			string resultString = AMEditorJSON.JsonEncode (am_editor_state);
			File.WriteAllText (AMEditor.WindowMain.FILE_NAME_STATE_EDITOR,AMEditorJSON.FormatJson (resultString));
		}

		public static string GetHandlerName ()
		{
			string result = "";

			if (!File.Exists (AMEditor.WindowMain.FILE_NAME_STATE_EDITOR))
				File.WriteAllText (AMEditor.WindowMain.FILE_NAME_STATE_EDITOR, "{}");

			Hashtable am_editor_state = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AMEditor.WindowMain.FILE_NAME_STATE_EDITOR)) as Hashtable;
			if (am_editor_state != null)
			{
				result = (string)am_editor_state[handlerKey];
			}
			return result;
		}

		public static bool HandlerNameEquals (string eventName)
		{
			bool result = false;

			if (!File.Exists (AMEditor.WindowMain.FILE_NAME_STATE_EDITOR))
				return false;

			Hashtable am_editor_state = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AMEditor.WindowMain.FILE_NAME_STATE_EDITOR)) as Hashtable;
			if (am_editor_state == null)
				return false;
			
			try
			{
				result = eventName.Equals ((string)am_editor_state[handlerKey]);
			}
			catch (Exception)
			{
				return false;
			}

			return result;
		}

		[DidReloadScripts]
		public static void CompilationCompleteHandler ()
		{
			try
			{
				if (AMEditor.WindowMain.instance == null && AMEditor.WindowMain.launchMode == WindowMain.LaunchMode.UI)
				{
					System.Reflection.Assembly ameditorAssembly = System.Reflection.Assembly.GetAssembly (typeof (UnityCompileHandler));
					var oldEditorClass = UnityEditor.EditorWindow.GetWindow (System.Type.GetType ("AMEditor.WindowAMEditor, "+ameditorAssembly.ToString (), false), true, AMEditorSystem.ContentOtherWindow._TitleWait);
					oldEditorClass.Close ();
				}
			}
			catch (Exception)
			{}

			string eventName = GetHandlerName ();
			SetHandlerName ("");
			AMEditor.WindowMain.compilationHandlerName = string.Empty;

			switch (eventName)
			{
			case "LanguageChanged":
				if (AMEditor.WindowMain.instance != null)
				{
					new UI.AMDisplayDialog (AMEditorSystem.ContentMenuLanguage._TitleDialog, 
						AMEditorSystem.ContentMenuLanguage._MessageSuccessDialog, 
						AMEditorSystem.ContentStandardButton._Ok, "", 
						() => {}, () => {}, true).Show ();
				}
				else
				{
					new UI.AMDisplayDialog (AMEditorSystem.ContentMenuLanguage._TitleDialog, 
						AMEditorSystem.ContentMenuLanguage._MessageSuccessDialog, 
						AMEditorSystem.ContentStandardButton._Ok, AMEditorSystem.ContentStandardButton._More, 
						() => {}, () => {AMEditor.WindowMain.Show ();}, true).Show ();
				}
				break;
			case "ViewTypeChanged":
				if (AMEditor.WindowMain.instance != null)
				{
					new UI.AMDisplayDialog (AMEditorSystem.ContentViewType._TitleDialog, 
						AMEditorSystem.ContentViewType._MessageSuccessDialog, 
						AMEditorSystem.ContentStandardButton._Ok, "", 
						() => {}, () => {}, true).Show ();
				}
				else
				{
					new UI.AMDisplayDialog (AMEditorSystem.ContentViewType._TitleDialog, 
						AMEditorSystem.ContentViewType._MessageSuccessDialog, 
						AMEditorSystem.ContentStandardButton._Ok, AMEditorSystem.ContentStandardButton._More, 
						() => {}, () => {AMEditor.WindowMain.Show ();}, true).Show ();
				}
				break;
			case "PluginsTypeChanged":
				if (AMEditor.WindowMain.instance != null)
				{
					new UI.AMDisplayDialog (AMEditorSystem.ContentPluginsType._TitleDialog, 
						AMEditorSystem.ContentPluginsType._MessageSuccessDialog, 
						AMEditorSystem.ContentStandardButton._Ok, "", 
						() => {}, () => {}, true).Show ();
				}
				else
				{
					new UI.AMDisplayDialog (AMEditorSystem.ContentPluginsType._TitleDialog, 
						AMEditorSystem.ContentPluginsType._MessageSuccessDialog, 
						AMEditorSystem.ContentStandardButton._Ok, AMEditorSystem.ContentStandardButton._More, 
						() => {}, () => {AMEditor.WindowMain.Show ();}, true).Show ();
				}
				break;
			case "RestoreDefaults":
				if (AMEditor.WindowMain.instance != null)
				{
					new UI.AMDisplayDialog (AMEditorSystem.ContentRestoreDefaults._TitleDialog, 
						AMEditorSystem.ContentRestoreDefaults._MessageSuccessDialog, 
						AMEditorSystem.ContentStandardButton._Ok, "", 
						() => {}, () => {}, true).Show ();
				}
				else
				{
					new AMEditor.UI.AMDisplayDialog (AMEditorSystem.ContentRestoreDefaults._TitleDialog, 
						AMEditorSystem.ContentRestoreDefaults._MessageSuccessDialog, 
						AMEditorSystem.ContentStandardButton._Ok, AMEditorSystem.ContentStandardButton._More, 
						() => {}, () => {AMEditor.WindowMain.Show ();}, true).Show ();
				}
				break;
			case "RestoreCRMComponents":
				AMEditor.WindowMain.messageWait = AMEditorSystem.ContentOtherWindow._RestoringCRMWaitMessage;
				CustomResourceManagerAPI.RestoreCRMComponents ();
				CustomResourceManagerAPI.CheckCorrect ();
				break;
			default:
				break;
			}
			CustomResourceManagerAPI.CheckAssetBundlesAvailability ();
			#if UNITY_5 && !UNITY_5_5_OR_NEWER
			AMEditorMenuAPI.SetMarks ();
			#endif
		}
	}
}
#endif