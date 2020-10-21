#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

namespace AMEditor
{
	public class LocalRepositoryAPI
	{
		static string _pathToRepository;
		static string KEY_REPO_EDITOR_PREFS = "IsRepoPathInProject";


		public static string pathToRepository
		{
			get
			{
				if (string.IsNullOrEmpty (_pathToRepository)) 
				{
					if (getValueKey ())
					{
						if (AMEditorFileStorage.CreateFolder ("AMEditor", false))
						{
							_pathToRepository = "AMEditor/";
						}
					}
					else
					{
						string result = string.Empty;
						#if UNITY_EDITOR_OSX
						result = "/Users/" + System.Environment.UserName + "/Library/Unity";
						#else
						result = "C:\\ProgramData/Unity";
						#endif
						if (AMEditorFileStorage.CreateFolder (result + "/AMEditor", false))
						{
							_pathToRepository = result + "/AMEditor/";
						}
						else
						{
							AMEditorPopupErrorWindow.CopyToProject += () =>
							{
								if (AMEditorFileStorage.CreateFolder ("AMEditor", false))
								{
									_pathToRepository = "AMEditor/";
									EditorPrefs.SetBool (KEY_REPO_EDITOR_PREFS, true);
								}
							};
							AMEditorPopupErrorWindow.ShowAccessErrorPopup (AMEditorSystem.ContentError._308 (result));
						}
					}
				}
				return _pathToRepository;
			}
		}

		static bool getValueKey ()
		{
			bool result = false;

			if (EditorPrefs.HasKey (KEY_REPO_EDITOR_PREFS) && EditorPrefs.GetBool (KEY_REPO_EDITOR_PREFS)) 
			{
				result = true;
			}

			return result;
		}

		public static void DeleteLocalRepositories ()
		{
			string FOLDER_NAME_LOCAL_REPOSITORY = string.Empty;

			AssetDatabase.Refresh ();
			FOLDER_NAME_LOCAL_REPOSITORY = AMEditorSystem.FolderNames._LocalRepository;
			var path = pathToRepository + FOLDER_NAME_LOCAL_REPOSITORY;
			if (!Directory.Exists (path))
				Directory.CreateDirectory (path);
			List<string> dirsList = new List<string> ();
			dirsList.AddRange (Directory.GetDirectories (path));
			List<string> filesList = new List<string> ();
			filesList.AddRange (Directory.GetFiles (path));
			string dir = "";
			string file = "";

			try
			{
				foreach (var item in dirsList)
				{
					dir = item;
					dir = dir.Substring (dir.LastIndexOf (System.IO.Path.DirectorySeparatorChar) + 1);
					AMEditorFileStorage.RemoveFolder (path + System.IO.Path.DirectorySeparatorChar.ToString () + dir);
				}
			}
			catch (Exception)
			{
				AMEditorPopupErrorWindow.ShowErrorPopup ("", AMEditorSystem.ContentError._309 (dir));
			}
			try
			{
				foreach (var item in filesList)
				{
					file = item;
					if (file.Contains (".unitypackage") || file.Contains (".zip"))
					{
						file = file.Substring (file.LastIndexOf (System.IO.Path.DirectorySeparatorChar) + 1);
						AMEditorFileStorage.RemoveFile (path + System.IO.Path.DirectorySeparatorChar.ToString () + file);
					}
				}
			}
			catch (Exception)
			{
				AMEditorPopupErrorWindow.ShowErrorPopup ("", AMEditorSystem.ContentError._309 (file));
			}

			if (dirsList.FindIndex ((d) => {return AMEditorFileStorage.ExistFolder (d);}) != -1 ||
				filesList.FindIndex ((f) => {return AMEditorFileStorage.FileExist (f) && !f.Contains (".DS_Store");}) != -1)
			{
				new UI.AMDisplayDialog (AMEditorSystem.ContentMenuLocalRepository._DeleteTitle, AMEditorSystem.ContentMenuLocalRepository._FailureMessage, 
					AMEditorSystem.ContentMenuLocalRepository._OpenButton, AMEditorSystem.ContentStandardButton._Ok, () => {
						UnityEditor.EditorUtility.OpenWithDefaultApp (pathToRepository + FOLDER_NAME_LOCAL_REPOSITORY);}, () => {
					}, true).Show ();
			}
			else
			{
				new UI.AMDisplayDialog (AMEditorSystem.ContentMenuLocalRepository._DeleteTitle, AMEditorSystem.ContentMenuLocalRepository._SuccessMessage, 
					AMEditorSystem.ContentStandardButton._Ok, "", () => {
					}, () => {
					}, true).Show ();
			}
		}
	}

}
#endif