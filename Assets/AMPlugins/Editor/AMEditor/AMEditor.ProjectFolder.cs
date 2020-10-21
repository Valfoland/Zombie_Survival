#if UNITY_EDITOR
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AMEditor
{
	public class ProjectFolder
	{
		public List<ProjectFile> files = new List<ProjectFile> ();
		public List<ProjectFolder> folders = new List<ProjectFolder> ();
		public string name = "";
		public string fullPath = "";
		public bool selected = false;
		public bool expand = true;
		public ProjectFolder (string name, string fullPath = "", List<ProjectFile> files = null, List<ProjectFolder> folders = null, bool selected = false, bool expand = false)
		{
			fullPath = fullPath + Path.DirectorySeparatorChar.ToString ();
			this.name = name;
			this.fullPath = fullPath;
			this.selected = selected;
			this.expand = expand;

			if (files != null) 
			{
				this.files = files;
			}
			else
			{
				this.files = new List<ProjectFile> ();
			}

			if (folders != null)
			{
				this.folders = folders;
			}
			else
			{
				this.folders = new List<ProjectFolder> ();
			}
		}
		public ProjectFolder (Hashtable source)
		{
			this.name = (string)source["name"];
			this.fullPath = (string)source["full_path"] + Path.DirectorySeparatorChar.ToString ();
			this.selected = (bool)source["selected"];
			this.expand = (bool)source["expand"];

			ArrayList files_list = source ["files"] as ArrayList;
			for (int i = 0; i < files_list.Count; i++)
			{
				object file = files_list [i];
				this.files.Add (new ProjectFile (file as Hashtable));
			}

			ArrayList folders_list = source ["folders"] as ArrayList;
			for (int i = 0; i < folders_list.Count; i++)
			{
				object folder = folders_list [i];
				this.folders.Add (new ProjectFolder (folder as Hashtable));
			}
		}
		public void AddFile (string filePath)
		{
			string[] listFolders = filePath.Split (new char[]{Path.DirectorySeparatorChar}, System.StringSplitOptions.RemoveEmptyEntries);

			if ((listFolders == null) || (listFolders.Length < 3))
			{
				return;
			}
			string nextFolder = string.Empty;

			for (int i = 0; i < listFolders.Length-2; i++) 
			{
				if (listFolders[i].ToLower () == this.name.ToLower ())
				{
					nextFolder = listFolders[i+1];
					break;
				}
			}
			if (nextFolder == string.Empty) 
			{
				if (listFolders[listFolders.Length - 2].ToLower () == this.name.ToLower ())
				{
					this.files.Add (new ProjectFile (listFolders[listFolders.Length - 1], filePath));
				}
			} 
			else 
			{
				int index = folders.FindIndex ((folder) => { return folder.name.ToLower () == nextFolder.ToLower ();});
				if (index == -1)
				{
					folders.Add (new ProjectFolder (nextFolder, Path.GetDirectoryName (filePath)));
				}
				for (int i = 0; i < this.folders.Count; i++)
				{
					ProjectFolder item = this.folders [i];
					if (item.name.ToLower () == nextFolder.ToLower ())
					{
						item.AddFile (filePath);
						item.fullPath = filePath.Substring (0, filePath.LastIndexOf (item.name) + item.name.Length);
						nextFolder = string.Empty;
						break;
					}
				}
			}
		}
		public List<Hashtable> GetSelectedFiles ()
		{
			List<Hashtable> result = new List<Hashtable> ();
			for (int i = 0; i < this.files.Count; i++)
			{
				ProjectFile item = this.files [i];
				if (item.selected && !item.name.Contains ("ameditor_config.json"))
				{
					result.Add (item.ToHashTable ());
				}
			}
			for (int i = 0; i < this.folders.Count; i++)
			{
				ProjectFolder item = this.folders [i];
				List<Hashtable> selectedCurrentFiles = item.GetSelectedFiles ();
				for (int j = 0; j < selectedCurrentFiles.Count; j++)
				{
					Hashtable f = selectedCurrentFiles [j];
					result.Add (f);
				}
			}
			return result;
		}
		public List<Hashtable> GetAllFiles ()
		{
			List<Hashtable> result = new List<Hashtable> ();
			for (int i = 0; i < this.files.Count; i++)
			{
				ProjectFile item = this.files [i];
				if (!item.name.Contains ("ameditor_config.json"))
				{
					result.Add (item.ToHashTable ());
				}
			}
			for (int i = 0; i < this.folders.Count; i++)
			{
				ProjectFolder item = this.folders [i];
				List<Hashtable> allFolderFiles = item.GetAllFiles ();
				for (int j = 0; j < allFolderFiles.Count; j++)
				{
					Hashtable f = allFolderFiles [j];
					result.Add (f);
				}
			}
			return result;
		}
		public void UpdateFlags (bool checkUniq = true)
		{
            #if UNITY_EDITOR_OSX
            string targetFolderPath = fullPath;
            #else
            string targetFolderPath = fullPath.Replace ('/', '\\');
            #endif

            if (WindowCreateConfigFile.coreFilesList != null)
            {
                for (int i = 0; i < WindowCreateConfigFile.coreFilesList.Count; i++)
                {
                    string coreFilePath = WindowCreateConfigFile.coreFilesList[i].fullPath;
                    #if !UNITY_EDITOR_OSX
                    coreFilePath = coreFilePath.Replace ('/', '\\');
                    #endif

                    if (coreFilePath.Contains (targetFolderPath))
                    {
                        expand = true;
                    }
                }
            }
            if (WindowCreateConfigFile.modifiableFilesList != null)
            {
                for (int i = 0; i < WindowCreateConfigFile.modifiableFilesList.Count; i++)
                {
                    string modifiableFilePath = WindowCreateConfigFile.modifiableFilesList[i].fullPath;
                    #if !UNITY_EDITOR_OSX
                    modifiableFilePath = modifiableFilePath.Replace ('/', '\\');
                    #endif

                    if (modifiableFilePath.Contains (targetFolderPath))
                    {
                        expand = true;
                    }
                }
            }
            if (WindowCreateConfigFile.examplesFilesList != null)
            {
                for (int i = 0; i < WindowCreateConfigFile.examplesFilesList.Count; i++)
                {
                    string exampleFilePath = WindowCreateConfigFile.examplesFilesList[i].fullPath;
                    #if !UNITY_EDITOR_OSX
                    exampleFilePath = exampleFilePath.Replace ('/', '\\');
                    #endif

                    if (exampleFilePath.Contains (targetFolderPath))
                    {
                        expand = true;
                    }
                }
            }
            if (this.files.Count > 0 && this.folders.Count > 0) 
			{
				this.selected = this.files.TrueForAll ((f) => {return f.selected;}) && this.folders.TrueForAll ((f) => {return f.selected;});
			}
			else if (this.files.Count > 0 && this.folders.Count == 0) 
			{
				this.selected = this.files.TrueForAll ((f) => {return f.selected;});
			}
			else if (this.files.Count == 0 && this.folders.Count > 0) 
			{
				this.selected = this.folders.TrueForAll ((f) => {return f.selected;});
			}

			if (checkUniq) 
			{
				for (int i = 0; i < this.files.Count; i++)
				{
					ProjectFile file = this.files [i];

					if (WindowCreateConfigFile.duplicateFilesList == null)
						WindowCreateConfigFile.duplicateFilesList = new List<string> ();
					if (!WindowCreateConfigFile.duplicateFilesList.Contains (file.name))
					{
						if ((file.unique) && WindowCreateConfigFile.uniqFilesList.Contains (file.name))
						{
							List<string> list = WindowCreateConfigFile.uniqFilesMap[file.name];
							if ((list != null) && (list.Count > 0))
							{
								for (int j = 0; j < list.Count; j++)
								{
                                    #if UNITY_EDITOR_OSX
                                    string tempFilePath = file.fullPath;
                                    string tempUniqueFilePath = list[j];
                                    #else
                                    string tempFilePath = file.fullPath.Replace ('/', '\\');
                                    string tempUniqueFilePath = list[j].Replace ('/', '\\');
                                    #endif
                                    if (tempFilePath != tempUniqueFilePath)
									{
										WindowCreateConfigFile.duplicateFilesList.Add (file.name);
										file.unique = false;
                                    }
								}
							}
						}
					}
					else
					{
						file.unique = false;
                    }
				}
			}
			for (int i = 0; i < (this.folders.Count-this.folders.FindAll ((f) => {return f.selected;}).Count); i++)
			{
				for (int j = 0; j < this.folders.Count; j++)
				{
					this.folders[j].UpdateFlags (checkUniq);
				}
			}
		}
		public Hashtable ToHashtable ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("selected", this.selected);
			result.Add ("name", this.fullPath.Replace ("\\", "/"));
			result.Add ("expand", this.expand);
			result.Add ("full_path", this.fullPath);

			ArrayList files_list = new ArrayList ();
			for (int i = 0; i < this.files.Count; i++)
			{
				ProjectFile item = this.files [i];
				Hashtable fileItem = item.ToHashTable ();
				files_list.Add (fileItem);
			}
			result.Add ("files", files_list);

			ArrayList folders_list = new ArrayList ();
			for (int i = 0; i < this.folders.Count; i++)
			{
				ProjectFolder item = this.folders [i];
				Hashtable folderItem = item.ToHashtable ();
				folders_list.Add (folderItem);
			}
			result.Add ("folders", folders_list);

			return result;
		}
	}
}
#endif