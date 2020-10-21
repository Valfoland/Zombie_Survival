#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

namespace AMEditor
{
	class ExternalPluginGUI
	{
		//public bool selected;
		public string name;
		public string version;
		public string uriDescription;
		public string uriDownload;

		private static string defaultName = "PluginName";
		private static string defaultUrl = "http://pgit.digital-ecosystems.ru";

		public ExternalPluginGUI ()
		{
			name = defaultName;
			uriDescription = defaultUrl;
			uriDownload = defaultUrl;
			version = string.Empty;
		}

		public ExternalPluginGUI (Hashtable source)
		{
			try 
			{
				name = (string)source["name"];
			} 
			catch (Exception) 
			{
				name = defaultName;
			}

			try 
			{
				uriDescription = (string)source["url_description"];
			} 
			catch (Exception) 
			{
				uriDescription = defaultUrl;
			}

			try 
			{
				uriDownload = (string)source["url_download"];
			} 
			catch (Exception) 
			{
				uriDownload = defaultUrl;
			}

			try 
			{
				version = (string)source["version"];
			} 
			catch (Exception) 
			{
				version = string.Empty;
			}
		}
		public Hashtable ToHashtable ()
		{
			var result = new Hashtable ();

			result.Add ("name", name);
			result.Add ("url_download", uriDownload);
			result.Add ("url_description", uriDescription);
			result.Add ("version", version);

			return result;
		}
		static string CurrentStatus = string.Empty;

		static AMEditorGit gitDownload;
		static string LOCAL_REPOSITORY = AMEditorSystem.FolderNames._LocalRepository;
		static string downloadPath = string.Empty;

		public void DrawGUI ()
		{
			GUILayout.BeginHorizontal ();
			// name
			GUILayout.Label (new GUIContent (name, name), GUILayout.Width (Screen.width/3.19f+42));

			// acrual version
			GUILayout.Label (new GUIContent (version, version), GUILayout.Width (Screen.width/8.85f));

			GUILayout.Label ("");

			// buttons
			/// dowload button
			GUI.enabled = uriDownload.Contains ("unitypackage");
			string downloadHelp = uriDownload.Contains ("unitypackage") ? AMEditorSystem.ContentPlugin._DownloadHelp : AMEditorSystem.ContentPlugin._IncorrectDownloadUrl;
			#if AM_EDITOR_COMPACT_ON
			GUIContent downloadButtonContent = new GUIContent (AMEditor.WindowMain.downloadButtonTexture, downloadHelp);
			int buttonWidth = 24;
			#else
			GUIContent downloadButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._Download, downloadHelp);
			int buttonWidth = 80;
			#endif
			if (GUILayout.Button (downloadButtonContent, new GUILayoutOption[] {GUILayout.Width (buttonWidth), GUILayout.Height (18)}))
			{
				CurrentStatus = AMEditorSystem.ContentProgressBar._Plugin + name +" "+ version +". "+ AMEditorSystem.ContentStatuses._DownloadingUnitypackage;
				if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._Update, CurrentStatus, 0.3f))
				{
					CancelProgressBar ();
				}

				string url = uriDownload.Replace ("blob", "raw");
				downloadPath = LocalRepositoryAPI.pathToRepository + LOCAL_REPOSITORY + Path.DirectorySeparatorChar + url.Substring (url.LastIndexOf ('/') + 1);

				if (AMEditorFileStorage.FileExist (downloadPath))
				{
					DownloadCompleteHandler (true);
				}
				else
				{ 
					gitDownload = new AMEditorGit ();
					gitDownload.UnitypackageDownloadComplete += DownloadCompleteHandler;
#if AM_EDITOR_DEBUG_MODE_ON
					gitDownload.printDebug = true;
#else
					gitDownload.printDebug = false;
#endif
					gitDownload.AuthByPT (GitAccount.current.server, GitAccount.current.privateToken);

					gitDownload.UnitypackageDownload (url, downloadPath);
				}
			}
			GUI.enabled = true;
			/// description button
#if AM_EDITOR_COMPACT_ON
			GUIContent descriptionButtonContent = new GUIContent (AMEditor.WindowMain.descriptionButtonTexture, AMEditorSystem.ContentPlugin._DescriptionHelp);
#else
			GUIContent descriptionButtonContent = new GUIContent (AMEditorSystem.ContentPlugin._Description, AMEditorSystem.ContentPlugin._DescriptionHelp);
#endif
			if (GUILayout.Button (descriptionButtonContent, new GUILayoutOption[] {GUILayout.Width (buttonWidth), GUILayout.Height (18)}))
			{
				Application.OpenURL (uriDescription);
			}
#if AM_EDITOR_COMPACT_ON
			GUILayout.Label ("", GUILayout.Width (52));
#else
			GUILayout.Label ("", GUILayout.Width (248));
#endif
			GUILayout.EndHorizontal ();
		}

		void DownloadCompleteHandler (bool downloadComplete)
		{
			if (downloadComplete)
			{
				try
				{
					//#if AM_EDITOR_VIEW_TYPE_EXTENDED
					new UI.AMDisplayDialog (AMEditorSystem.ContentProgressBar._Update, 
						AMEditorSystem.ContentExternalPlugin._SuccessDownloadMessage (name+" "+version), 
						AMEditorSystem.ContentStandardButton._Ok, "", () => {AssetDatabase.ImportPackage (downloadPath, true);}, () => { }, true).Show ();
					/*
					#elif AM_EDITOR_VIEW_TYPE_MINIMAL
                    AssetDatabase.ImportPackage (downloadPath, false);
                    new UI.AMDisplayDialog (AMEditorSystem.ContentProgressBar._Update, 
                        AMEditorSystem.ContentExternalPlugin._SuccessImportMessage + name+" "+version + AMEditorSystem.ContentExternalPlugin._SuccessImportMessage_end, 
                        AMEditorSystem.ContentStandardButton._Ok, "", () => { }, () => { }, true).Show ();
                    #endif
                    */
				}
				catch (Exception ex)
				{
					Debug.LogWarning (ex.ToString ());
					CancelProgressBar ();
					new UI.AMDisplayDialog (AMEditorSystem.ContentOtherWindow._TitleErrorPopup, 
						AMEditorSystem.ContentExternalPlugin._FailedImportMessage (name+" "+version), 
						AMEditorSystem.ContentStandardButton._Ok, "", () => { }, () => { }, true).Show ();
				}
				CancelProgressBar ();
			}
			else
			{
				new UI.AMDisplayDialog (AMEditorSystem.ContentOtherWindow._TitleErrorPopup, 
					AMEditorSystem.ContentExternalPlugin._FailedDownloadMessage (name+" "+version), 
					AMEditorSystem.ContentStandardButton._Ok, "", () => { }, () => { }, true).Show ();
				CancelProgressBar ();
			}
		}

		static void CancelProgressBar ()
		{
			try
			{
				EditorUtility.ClearProgressBar ();
			}
			catch (Exception)
			{

			}

			if (gitDownload != null)
				gitDownload.StopDownload ();
		}
	}
}

#endif