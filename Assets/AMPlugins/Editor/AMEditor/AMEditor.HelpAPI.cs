#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

namespace AMEditor
{
	class HelpAPI
	{
		public static string NAME_FILE_MENU_ABOUT = "about_config.json";
		static string LINK_TO_PUBLIC_REPO = "";
		static string PATH_TO_FOLDER = "";

		public enum TypeAbout
		{
			SkypeSupport,
			Tutorial,
			ChangeLog
		}
		public static void SearchLink (TypeAbout typeAbout)
		{
			var linkPublicRepo = GetLinkPublicRepo ();
			if (linkPublicRepo == "") 
			{
				AMEditorPopupErrorWindow.ShowErrorPopup ("101", AMEditorSystem.WebError._101);
				return;
			}
			LINK_TO_PUBLIC_REPO = linkPublicRepo;
			PATH_TO_FOLDER = LocalRepositoryAPI.pathToRepository;
			var link = GetLink (typeAbout);
			if (link != string.Empty)
			{
				Application.OpenURL (link);
			}
			else
			{
				switch (typeAbout) 
				{
				case TypeAbout.SkypeSupport:
					AMEditorPopupErrorWindow.ShowErrorPopup ("310", AMEditorSystem.ContentError._310 ("Wiki (Tutorial)"));
					break;
				case TypeAbout.Tutorial:
					AMEditorPopupErrorWindow.ShowErrorPopup ("310", AMEditorSystem.ContentError._310 ("Support"));
					break;
				case TypeAbout.ChangeLog:
					AMEditorPopupErrorWindow.ShowErrorPopup ("310", AMEditorSystem.ContentError._310 ("Change Log"));
					break;
				default:
					break;
				}	
			}	
		}
		static string GetLink (TypeAbout typeAbout)
		{
			new AMEditor.UI.AMProgressBar ("Get link", typeAbout.ToString (), 0.5f, () => {}, true);

			var result = string.Empty;
			try 
			{
				var hash = AMEditorJSON.JsonDecode (GetAboutFile ()) as Hashtable;

				switch (typeAbout) 
				{
				case TypeAbout.SkypeSupport:
					result = (string)hash["skype_support"];
					break;
				case TypeAbout.Tutorial:
					result = (string)hash["tutorial"];
					break;
				case TypeAbout.ChangeLog:
					result = (string)hash["change_log"];
					break;
				default:
					break;
				}	
			}
			catch (Exception) 
			{}

			AMEditor.UI.AMProgressBar.Clean ();

			return result;
		}

		static string GetAboutFile ()
		{
			var result = string.Empty;

			if (AMEditorFileStorage.FileExist (PATH_TO_FOLDER + "/" + NAME_FILE_MENU_ABOUT)) 
			{
				var fileContent = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (PATH_TO_FOLDER + "/" + NAME_FILE_MENU_ABOUT)) as Hashtable;
				if (fileContent == null)
				{
					result = AMEditorGit.RequestGet (LINK_TO_PUBLIC_REPO);
					File.WriteAllText (PATH_TO_FOLDER + "/" + NAME_FILE_MENU_ABOUT, result);
				}
				else
				{
					result = AMEditorFileStorage.ReadTextFile (PATH_TO_FOLDER + "/" + NAME_FILE_MENU_ABOUT);
				}
			}
			else
			{
				result = AMEditorGit.RequestGet (LINK_TO_PUBLIC_REPO);
				File.WriteAllText (PATH_TO_FOLDER + "/" +NAME_FILE_MENU_ABOUT, result);
			}

			return result;
		}

		public static string GetLinkPublicRepo ()
		{
			if ((GitAccount.current == null) || (string.IsNullOrEmpty (GitAccount.current.privateToken)))
			{
				return "";
			}
			return  AMEditorSystem.Links.PublicAboutConfig + "?private_token=" + GitAccount.current.privateToken;
		}

		public static void ForceDownload ()
		{

			var linkPublicRepo = GetLinkPublicRepo ();
			if (linkPublicRepo == "") 
			{
				AMEditorPopupErrorWindow.ShowErrorPopup ("101", AMEditorSystem.WebError._101);
				return;
			}
			LINK_TO_PUBLIC_REPO = linkPublicRepo;
			//LINK_TO_PUBLIC_REPO = AMEditorSystem.Links.PublicAboutConfig;
			PATH_TO_FOLDER = LocalRepositoryAPI.pathToRepository;
			NAME_FILE_MENU_ABOUT = AMEditorSystem.FileNames._ConfigAboutMenu;
			var result = AMEditorGit.RequestGet (LINK_TO_PUBLIC_REPO);
			//Debug.Log (result);

			if (ValidLink (result))
			{
				File.WriteAllText (PATH_TO_FOLDER + "/" +NAME_FILE_MENU_ABOUT, result);
			}
		}

		static bool ValidLink (string source)
		{
			bool result = true;
			try 
			{
				var hash = AMEditorJSON.JsonDecode (GetAboutFile ()) as Hashtable;

				var skype_support = (string)hash["skype_support"];
				if (skype_support == string.Empty)
				{
					result = false;
				}

				var tutorial = (string)hash["tutorial"];
				if (tutorial == string.Empty)
				{
					result = false;
				}

				var change_log = (string)hash["change_log"];
				if (change_log == string.Empty)
				{
					result = false;
				}
			
			}
			catch (Exception) 
			{
				result = false;
			}
			return result;
		}
	}
}
#endif