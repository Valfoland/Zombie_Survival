#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace AMEditor
{
	public class AMEditorConflictAPI 
	{
		/// <summary>
		/// Gets the conflict.
		/// </summary>
		/// <param name="plugin">Plugin.</param>
		public static void GetConflict (Plugin plugin)
		{
			plugin.currentMessage = string.Empty;
			List<fileToggle> conflictFiles = new List<fileToggle> ();
			List<fileToggle> oldFiles = new List<fileToggle> ();

			foreach (var item in plugin.files) 
			{
				int conflictIndex = item.parameters.IndexOf (Plugin.FileParameter.outdated);
				if (conflictIndex != -1)
				{
					if (AMEditorFileStorage.FileExist (item.path))
						oldFiles.Add (new fileToggle (item.path, true, AMEditorSystem.ContentWindowConflict._ContentOutdatedMessage));	
				}
				else
				{
					int uniqueIndex = item.parameters.IndexOf (Plugin.FileParameter.unique);
					if (uniqueIndex != -1)
					{
						var list = AMEditorFileStorage.SearchFile ("Assets", GetName (item.path.ToLower ()));
						if ((list != null) && (list.Count > 0))
						{
							foreach (var f in list) 
							{
								if (item.path.ToLower () != f.ToLower ())
								{
									conflictFiles.Add (new fileToggle (f, true, AMEditorSystem.ContentWindowConflict._ContentPossibleConflist));
								}
							}
						}
					}
				}
			}

			plugin.conflictFiles = conflictFiles;
			plugin.conflictFilesOldFiles = oldFiles;
			if ((conflictFiles.Count > 0) || (oldFiles.Count>0))
			{
				plugin.errors.conflict = true;
			}
		}

		public static void GetConflict (List<Plugin> plugins)
		{
			List<string> uniqueFiles = new List<string> ();

			for (int i = 0; i < plugins.Count; i++) 
			{
				if (plugins [i].conflictFiles == null)
					plugins [i].conflictFiles = new List<fileToggle> ();
				if (plugins [i].conflictFilesOldFiles == null)
					plugins [i].conflictFilesOldFiles = new List<fileToggle> ();
				foreach (var file in plugins[i].files) 
				{
					int uniqueIndex = file.parameters.IndexOf (Plugin.FileParameter.unique);
					if (uniqueIndex != -1)
					{
						if (!uniqueFiles.Contains (GetName (file.path)))
						{
							uniqueFiles.Add (GetName (file.path));
						}
					}
				}
			}

			Dictionary<string, List<string>> uniqFilesMap = AMEditorFileStorage.SearchFile ("Assets", uniqueFiles);

			for (int i = 0; i < plugins.Count; i++) 
			{
				plugins[i].currentMessage = string.Empty;

				List<fileToggle> filesConflict = new List<fileToggle> ();
				List<fileToggle> oldFilesConflict = new List<fileToggle> ();
				foreach (var file in plugins[i].files) 
				{
					int conflictIndex = file.parameters.IndexOf (Plugin.FileParameter.outdated);
					if (conflictIndex != -1)
					{
						if (AMEditorFileStorage.FileExist (file.path))
							oldFilesConflict.Add (new fileToggle (file.path, true, AMEditorSystem.ContentWindowConflict._ContentOutdatedMessage));	
					}
					else
					{
						int uniqueIndex = file.parameters.IndexOf (Plugin.FileParameter.unique);
						if ((uniqueIndex != -1) && uniqueFiles.Contains (GetName (file.path)))
						{
							var list = uniqFilesMap[GetName (file.path)];
							if ((list != null) && (list.Count > 0))
							{
								foreach (var f in list) 
								{
									if (file.path.ToLower () != f.ToLower ())
									{
										filesConflict.Add (new fileToggle (f, true, AMEditorSystem.ContentWindowConflict._ContentPossibleConflist));
									}
								}
							}
						}
					}

					plugins[i].conflictFiles = filesConflict;
					plugins[i].conflictFilesOldFiles = oldFilesConflict;
					if ((filesConflict.Count > 0) || (oldFilesConflict.Count>0))
					{
						plugins[i].errors.conflict = true;
						//plugin.currentMessage = "Conflict!!";
					}
				}
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <returns>The name.</returns>
		/// <param name="path">Path.</param>
		public static string GetName (string path)
		{
			return path.Substring (path.LastIndexOf ("/") + 1);
		}
	}
}
#endif