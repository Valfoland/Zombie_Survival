#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using Ionic.Zip;
using AMEditor;

namespace AMEditor
{
	public class AMLocalizationAPI 
	{
		static string AM_PROJECT = "Assets/StreamingAssets/am_project.txt";
		static string AM_LOCALIZATION_NAME = "localization.zip";

		public static void ZipAndMoveLocalization (string pathToLocalization)
		{
			if (!AMEditorFileStorage.FileExist (AM_PROJECT)) 
			{
				AMEditorPopupErrorWindow.ShowErrorPopup ("205", AMEditorSystem.FileSystemError._205 ("am_project.txt"));
				EditorUtility.ClearProgressBar ();
				return;
			}

			Hashtable am_project = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AM_PROJECT)) as Hashtable;
			Hashtable project_info = am_project ["project_info"] as Hashtable;
			ArrayList localization_path = project_info ["localization_path"] as ArrayList;

			if (localization_path == null || localization_path.Count == 0) 
			{
				Debug.LogWarning ("localization_path param in am_project.txt is empty or not exist!");
				return;
			}

			ZipFile localization = new ZipFile ();
			foreach (var item in localization_path) 
			{
				DirectoryInfo mainDirectory = new DirectoryInfo ((string)item);

				if (mainDirectory.Exists)
				{
					foreach (var file in mainDirectory.GetFiles ()) 
					{
						if (file.Extension != ".meta")
						{
							localization.AddFile (file.FullName, mainDirectory.Name);
						}
					}
					foreach (var directory in mainDirectory.GetDirectories ()) 
					{
						localization.AddDirectory(directory.FullName, mainDirectory.Name + "/" + directory.Name);
					}
				}
				else
				{
					FileInfo file = new FileInfo ((string)item);
					if (file.Exists)
					{
						if (file.Extension != ".meta")
						{
							localization.AddFile (file.FullName, mainDirectory.Name);
						}
					}
				}
			}

			if (localization.Entries.Count > 0)
			{
				localization.Save (pathToLocalization + "/" + AM_LOCALIZATION_NAME);
				Debug.Log ("Create localization.zip success");
			}
			else
			{
				Debug.LogWarning ("Localization file(s) not found at specified path(es)!");
			}
		}
	}
}
#endif