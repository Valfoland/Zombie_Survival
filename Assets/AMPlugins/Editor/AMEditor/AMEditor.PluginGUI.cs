#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AMEditor
{
	class PluginGUI
	{
		public class Error
		{
			public bool conflict;
			public bool missingFiles;
			public bool missingFilesHash;
			public bool noActualVersion;
			public bool versionNotForBuildType;
			public bool dependPlugins;
			public string messageDependPlugins;
			public string message;
			public bool needUpdate;

			public Error ()
			{
				conflict = false;
				missingFiles = false;
				noActualVersion = false;
				versionNotForBuildType = false;
				dependPlugins = false;
				messageDependPlugins = string.Empty;
				message = string.Empty;
				missingFilesHash = false;
			}

			public Error (Hashtable source)
			{
				try 
				{
					conflict = (bool)source["conflict"];
				} 
				catch (Exception) 
				{
					conflict = false;
				}

				try 
				{
					missingFiles = (bool)source["missingFiles"];
				} 
				catch (Exception) 
				{
					missingFiles = false;
				}

				try 
				{
					noActualVersion = (bool)source["noActualVersion"];
				} 
				catch (Exception) 
				{
					noActualVersion = false;
				}

				try
				{
					versionNotForBuildType = (bool)source["versionNotForBuildType"];
				}
				catch (Exception)
				{
					versionNotForBuildType = false;
				}

				try 
				{
					dependPlugins = (bool)source["dependPlugins"];
				} 
				catch (Exception) 
				{
					dependPlugins = false;
				}

				try 
				{
					messageDependPlugins = (string)source["messageDependPlugins"];
				} 
				catch (Exception) 
				{
					messageDependPlugins = string.Empty;
				}

				try
				{
					message = (string)source["message"];
				}
				catch (Exception)
				{
					message = string.Empty;
				}

				try 
				{
					missingFilesHash = (bool)source["missingFilesHash"];
				} 
				catch (Exception) 
				{
					missingFilesHash = false;
				}

				try 
				{
					needUpdate = (bool)source["needUpdate"];
				} 
				catch (Exception) 
				{
					needUpdate = false;
				}
			}

			public Error CopyTo ()
			{
				var error = new Error ();

				error.conflict = conflict;
				error.missingFiles = missingFiles;
				error.noActualVersion = noActualVersion;
				error.versionNotForBuildType = versionNotForBuildType;
				error.dependPlugins = dependPlugins;
				error.needUpdate = needUpdate;
				error.missingFilesHash = missingFilesHash;
				error.messageDependPlugins = messageDependPlugins;
				error.message = message;

				return error;
			}

			public Hashtable ToHashtable ()
			{
				var result = new Hashtable ();

				result.Add ("conflict", conflict);
				result.Add ("missingFiles", missingFiles);
				result.Add ("noActualVersion", noActualVersion);
				result.Add ("versionNotForBuildType", versionNotForBuildType);
				result.Add ("dependPlugins", dependPlugins);
				result.Add ("needUpdate", needUpdate);
				result.Add ("missingFilesHash", missingFilesHash);
				result.Add ("messageDependPlugins", messageDependPlugins);
				result.Add ("message", message);

				return result;
			}

			public void Update (Error source)
			{
				conflict = source.conflict;
				missingFiles = source.missingFiles;
				noActualVersion = source.noActualVersion;
				versionNotForBuildType = source.versionNotForBuildType;
				dependPlugins = source.dependPlugins;
				messageDependPlugins = source.messageDependPlugins;
				message = source.message;
				missingFilesHash = source.missingFilesHash;
			}
		}

		public class File
		{
			public enum StatusFile
			{
				missing = -1, 
				changed, 
				good
			}
			public string path;
			public StatusFile status;

			public File (string path, StatusFile status = StatusFile.good)
			{
				this.path = path;
				this.status = status;
			}

			public File (Hashtable source)
			{
				try 
				{
					path = (string)source["path"];
				} 
				catch (Exception) 
				{
					path = string.Empty;
				}

				try 
				{
					switch (int.Parse (source["status"].ToString ())) 
					{
					case -1:
						status = StatusFile.missing;
						break;
					case 0:
						status = StatusFile.changed;
						break;
					case 1:
						status = StatusFile.good;
						break;
					default:
					break;
					}
				} 
				catch (Exception) 
				{
					status = StatusFile.good;
				}
			}

			public Hashtable ToHashtable ()
			{
				var result = new Hashtable ();
				
				result.Add ("path", path);
				result.Add ("status", status);
			
				return result;
			}

			public void Update (File source)
			{
				path = source.path;
				status = source.status;
			}
		}

		public string name;

		public bool installed;
		public bool selected;
		public bool isExpandContent;
		public bool showExamples;

		public Error error;

		public string actualVersion;
		public string currentVersion;
		public string selectedVersion;
		public string displayType;
		public List<string> permittedVersions;
		public List<string> buildTypes;
		public List<File> files;
		public List<File> examples;

		public string url;

		private static string defaultName = "PluginName";
		private static string defaultUrl = "http://pgit.digital-ecosystems.ru";

		public PluginGUI ()
		{
			name = defaultName;
			url = defaultUrl;
			installed = false;
			selected = false;
			isExpandContent = false;
			showExamples = false;

			error = new Error ();

			actualVersion = string.Empty;
			currentVersion = string.Empty;
			selectedVersion = string.Empty;
			displayType = string.Empty;
			permittedVersions = new List<string> ();
			buildTypes = new List<string> ();
			files = new List<File> ();
			examples = new List<File> ();
		}

		public PluginGUI (Hashtable source)
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
				url = (string)source["url"];
			} 
			catch (Exception) 
			{
				url = defaultUrl;
			}

			try 
			{
				installed = (bool)source["install"];
			} 
			catch (Exception) 
			{
				installed = false;
			}

			try 
			{
				selected = (bool)source["selected"];
			} 
			catch (Exception) 
			{
				selected = false;
			}
			
			try 
			{
				isExpandContent = (bool)source["isExpandContent"];
			} 
			catch (Exception) 
			{
				isExpandContent = false;
			}

			try
			{ 
				showExamples = (bool)source["showExamples"];
			}
			catch (Exception)
			{
				showExamples = false;
			}

			try 
			{
				actualVersion = (string)source["actualVersion"];
			} 
			catch (Exception) 
			{
				actualVersion = string.Empty;
			}
			
			try
			{
				displayType = (string)source["displayType"];
				if (string.IsNullOrEmpty (displayType))
				{
					displayType = "none";
				}
			}
			catch (Exception)
			{
				displayType = "none";
			}

			try
			{
				selectedVersion = (string)source["selectedVersion"];
				if (selectedVersion == string.Empty)
					selectedVersion = actualVersion;
			} 
			catch (Exception) 
			{
				selectedVersion = actualVersion;
			}

			try
			{
				permittedVersions = new List<string> ();
				var permittedList = source["permittedVersions"] as ArrayList;
				foreach (var item in permittedList)
				{
					permittedVersions.Add ((string)item);
				}
			}
			catch (Exception)
			{
				permittedVersions.Add (actualVersion);
			}

			try
			{
				buildTypes = new List<string> ();
				var buildTypesList = source["buildTypes"] as ArrayList;
				foreach (var item in buildTypesList)
				{
					buildTypes.Add ((string)item);
				}
			}
			catch (Exception)
			{
				buildTypes = new List<string> ();
			}

			try 
			{
				currentVersion = (string)source["currentVersion"];
			} 
			catch (Exception) 
			{
				currentVersion = string.Empty;
			}

			try 
			{
				error = new Error (source["error"] as Hashtable);
			} 
			catch (Exception) 
			{
				error = new Error ();
			}
			try 
			{
				files = new List<File> ();
				var filesList = source["files"] as ArrayList;
				foreach (var item in filesList) 
				{
					files.Add (new File (item as Hashtable));
				}
			} 
			catch (Exception) 
			{
				files = new List<File> ();
			}
			try 
			{
				examples = new List<File> ();
				var examplesList = source["examples"] as ArrayList;
				foreach (var item in examplesList) 
				{
					examples.Add (new File (item as Hashtable));
				}
			} 
			catch (Exception) 
			{
				examples = new List<File> ();
			}
		}

		public Hashtable ToHashtable ()
		{
			var result = new Hashtable ();

			result.Add ("name", name);
			result.Add ("url", url);
			result.Add ("install", installed);
			result.Add ("selected", selected);
			result.Add ("isExpandContent", isExpandContent);
			result.Add ("showExamples", showExamples);
			result.Add ("actualVersion", actualVersion);
			result.Add ("displayType", displayType);
			result.Add ("selectedVersion", selectedVersion);
			result.Add ("error", error.ToHashtable ());
			result.Add ("currentVersion", currentVersion);

			var permittedList = new ArrayList ();
			foreach (var item in permittedVersions) 
			{
				permittedList.Add (item);
			}			
			result.Add ("permittedVersions", permittedList);

			var buildTypesList = new ArrayList ();
			foreach (var item in buildTypes) 
			{
				buildTypesList.Add (item);
			}			
			result.Add ("buildTypes", buildTypesList);

			var filesList = new ArrayList ();
			foreach (var item in files) 
			{
				filesList.Add (item.ToHashtable ());
			}
			result.Add ("files", filesList);

			var examplesList = new ArrayList ();
			foreach (var item in examples) 
			{
				examplesList.Add (item.ToHashtable ());
			}
			result.Add ("examples", examplesList);
			return result;
		}

		public void Update (PluginGUI source)
		{
			error.Update (source.error);

			actualVersion = source.actualVersion;
			currentVersion = source.currentVersion;
			displayType = source.displayType;
			permittedVersions = source.permittedVersions;
			buildTypes = source.buildTypes;
			files = new List<File> ();
			foreach (var item in source.files) 
			{
				files.Add (item);
			}
			examples = new List<File> ();
			foreach (var item in source.examples) 
			{
				examples.Add (item);
			}
			url = source.url;
		}
	}
}
#endif