#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMEditor
{
	public class Plugin 
	{
		public enum FileParameter
		{
			unique, 
			example, 
			modifiable, 
			outdated	
		}
		public class Error 
		{
			public bool conflict;
			public bool missingFiles;
			public bool oldVersion;
			public bool dependPlugins;
			public bool needUpdate;
			public bool missingFilesHash;
			
			public Error ()
			{
				conflict = false;
				missingFiles = false;
				oldVersion = false;
				dependPlugins = false;
				missingFilesHash = false;
			}
			public void CopyTo (Error error)
			{
				error = new Error ();
				error.conflict = conflict;
				error.missingFiles = missingFiles;
				error.oldVersion = oldVersion;
				error.dependPlugins = dependPlugins;
				error.needUpdate = needUpdate;
				error.missingFilesHash = missingFilesHash;
			}
		}
		
		public class FilePlugin
		{
			public string path;
			public string hash;
			public List<FileParameter> parameters;
			
			public FilePlugin ()
			{
				path = string.Empty;
				hash = string.Empty;
				parameters = new List<FileParameter> ();
			}
			
			public FilePlugin (Hashtable source)
			{
				path = (string)source["name"];
				hash = (string)source["hash"];
				parameters = new List<FileParameter> ();
				var param = source["parameters"] as ArrayList;
				foreach (var item in param) 
				{
					switch ((string)item) 
					{
					case "unique":
						parameters.Add (FileParameter.unique);
						break;
					case "example":
						parameters.Add (FileParameter.example);
						break;
					case "modifiable":
						parameters.Add (FileParameter.modifiable);
						break;
					case "outdated":
						parameters.Add (FileParameter.outdated);
						break;
					default:
						break;
					}
				}
			}
			
			public void CopyTo (FilePlugin filePlugin)
			{
				filePlugin = new FilePlugin ();
				filePlugin.path = path;
				filePlugin.hash = hash;
				foreach (var item in this.parameters) 
				{
					filePlugin.parameters.Add (item);
				}
			}
			
			public Hashtable ToHashtable ()
			{
				Hashtable result = new Hashtable ();
				ArrayList param = new ArrayList ();
				foreach (var item in parameters) 
				{
					param.Add (item.ToString ());
				}
				result.Add ("parameters", param);
				result.Add ("name", path);
				result.Add ("hash", hash);
				return result;
			}
		}
		
		public string name;
		public string url;
		public string version;
		public List<string> permitted;
		public string displayType;
		public List<string> buildTypes;
		public string currentMessage;	
		public string dependErrors;
		
		public bool selected;
		public bool mandatory;
		
		public string urlMasterBranch;
		
		public Error errors;
		public List<FilePlugin> files;
		public List<Depend> depends;
		
		public bool showFiles;
		
		public List<fileToggle> conflictFiles;
		public List<fileToggle> conflictFilesOldFiles;
		
		public List<string> oldNames = null;
		
		public Plugin (Hashtable source)
		{
			if (source == null) 
			{
				permitted = new List<string> ();
				buildTypes = new List<string> ();
				errors = new Error ();
				conflictFiles = new List<fileToggle> ();
				conflictFilesOldFiles = new List<fileToggle> ();
				depends = new List<Depend> ();
				files = new List<FilePlugin> ();
				oldNames = new List<string> ();
			}
			else
			{
				name = (string)source ["name"];
				urlMasterBranch = (string)source ["url"];
				try
				{
					displayType = (string)source ["display_type"];
					if (string.IsNullOrEmpty (displayType))
					{
						displayType = "none";
					}
				}
				catch (Exception)
				{
					displayType = "none";
				}

				errors = new Error ();
				
				var temp = (source["files"]) as ArrayList;
				files = new List<FilePlugin> ();
				if (temp==null)
					temp = new ArrayList ();
				foreach (var item in temp) 
				{
					files.Add (new FilePlugin (item as Hashtable));
				}
				
				currentMessage = string.Empty;
				showFiles = false;
				version = (string)source ["version"];
				
				permitted = new List<string> ();
				var permitted_versions = source["permitted_versions"] as ArrayList;
				if (permitted_versions != null) 
				{
					foreach (var item in permitted_versions)
					{
						permitted.Add ((string)item);
					}
				} 
				else
				{
					permitted.Add (version);
				}

				try 
				{
					buildTypes = new List<string> ();
					var build_types = source["build_types"] as ArrayList;
					foreach (var item in build_types) 
					{
						buildTypes.Add ((string)item);
					}				
				} 
				catch (Exception) 
				{
					buildTypes = new List<string> ();
				}
				
				var tempDepend = (source["depends"]) as ArrayList;
				depends = new List<Depend> ();
				if (tempDepend == null)
					tempDepend= new ArrayList ();
				foreach (var item in tempDepend) 
				{
					depends.Add (new Depend (item as Hashtable));
				}
				try 
				{
					oldNames = new List<string> ();
					var jsonOldNames = source["old_names"] as ArrayList;
					foreach (var item in jsonOldNames) 
					{
						oldNames.Add ((string)item);
					}
					
				} 
				catch (Exception) 
				{
					oldNames = new List<string> ();
				}
				try 
				{
					mandatory = (bool)(source["mandatory"]);
				} 
				catch (Exception) 
				{
					mandatory = false;
				}
			}
		}
		
		public Plugin (Hashtable source, string groupName)
		{	
			if (source == null) 
			{
				permitted = new List<string> ();
				buildTypes = new List<string> ();
				errors = new Error ();
				conflictFiles = new List<fileToggle> ();
				conflictFilesOldFiles = new List<fileToggle> ();
				depends = new List<Depend> ();
				files = new List<FilePlugin> ();
				oldNames = new List<string> ();
			}
			else
			{
				name = (string)source ["name"];
				urlMasterBranch = (string)source ["url"];
				try
				{
					displayType = (string)source["display_type"];
					if (string.IsNullOrEmpty (displayType))
					{
						displayType = "none";
					}
				}
				catch (Exception)
				{
					displayType = "none";
				}

				//urlMasterBranch = serverName + "/" + groupName.ToLower ().Replace (" ", "-") + "/" + name.ToLower ().Replace (" ", "-") + "/tree/master";

				errors = new Error ();
				
				var temp = (source["files"]) as ArrayList;
				files = new List<FilePlugin> ();
				if (temp==null)
					temp = new ArrayList ();
				foreach (var item in temp) 
				{
					files.Add (new FilePlugin (item as Hashtable));
				}
				
				currentMessage = string.Empty;
				showFiles = false;
				version = (string)source ["version"];
				
				permitted = new List<string> ();
				var permitted_versions = source["permitted_versions"] as ArrayList;
				if (permitted_versions != null) 
				{
					foreach (var item in permitted_versions)
					{
						permitted.Add ((string)item);
					}
				} 
				else
				{
					permitted.Add (version);
				}

				try 
				{
					buildTypes = new List<string> ();
					var build_types = source["build_types"] as ArrayList;
					foreach (var item in build_types) 
					{
						buildTypes.Add ((string)item);
					}				
				} 
				catch (Exception) 
				{
					buildTypes = new List<string> ();
				}
				
				var tempDepend = (source["depends"]) as ArrayList;
				depends = new List<Depend> ();
				if (tempDepend == null)
					tempDepend= new ArrayList ();
				foreach (var item in tempDepend) 
				{
					depends.Add (new Depend (item as Hashtable));
				}
				
				try 
				{
					oldNames = new List<string> ();
					var jsonOldNames = source["old_names"] as ArrayList;
					foreach (var item in jsonOldNames) 
					{
						oldNames.Add ((string)item);
					}
					
				} 
				catch (Exception) 
				{
					oldNames = new List<string> ();
				}

				try 
				{
					mandatory = (bool)(source["mandatory"]);
				} 
				catch (Exception) 
				{
					mandatory = false;
				}
			}
		}
		
		public List<Depend> GetDepends ()
		{
			var result = new List<Depend> ();
			foreach (var item in depends) 
			{
				result.Add (item);
			}
			return result;
		}
		
		public void Update (Plugin source)
		{
			files = new List<FilePlugin> ();
			foreach (var item in source.files) 
			{
				files.Add (item);
			}
			
			oldNames = new List<string> ();
			
			foreach (var item in source.oldNames) 
			{
				oldNames.Add (item);
			}
			this.urlMasterBranch = source.urlMasterBranch;
			displayType = source.displayType;
			version = source.version;
			permitted = source.permitted;
			buildTypes = source.buildTypes;
			depends = new List<Depend> ();
			foreach (var item in source.depends) 
			{
				depends.Add (item);
			}
		}
		
		public bool EqualsVersion (string source)
		{
			try 
			{
				return source.Equals (this.version);
			}
			catch (Exception) 
			{
				return false;
			}
		}
		
		public Hashtable ToHashtable ()
		{
			Hashtable result = new Hashtable ();
			result.Add ("version", version);
			result.Add ("name", name);
			result.Add ("url", urlMasterBranch);
			result.Add ("mandatory", mandatory);
			result.Add ("display_type", displayType);
			
			ArrayList dep = new ArrayList ();
			
			foreach (var item in depends) 
			{
				dep.Add (item.ToHashtable ());
			}
			result.Add ("depends", dep);
			
			ArrayList file = new ArrayList ();
			
			foreach (var item in files) 
			{
				file.Add (item.ToHashtable ());
			}
			result.Add ("files", file);
			
			ArrayList old_names = new ArrayList ();
			
			foreach (var item in oldNames) 
			{
				old_names.Add (item);
			}
			result.Add ("old_names", old_names);
			
			ArrayList permitted_versions = new ArrayList ();
			
			foreach (var item in permitted) 
			{
				permitted_versions.Add (item);
			}			
			result.Add ("permitted_versions", permitted_versions);

			ArrayList build_types = new ArrayList ();
			
			foreach (var item in buildTypes) 
			{
				build_types.Add (item);
			}			
			result.Add ("build_types", build_types);
			
			return result;
		}
	}
}
#endif