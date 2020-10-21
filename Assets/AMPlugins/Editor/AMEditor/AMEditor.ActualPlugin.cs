#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;

namespace AMEditor
{
	public class ActualPlugin
	{
		public string name = string.Empty;
		public string version = string.Empty;
		public List<string> permitted = null;
		public string displayType = string.Empty;
		public List<string> buildTypes = null;

		public bool selected = false;
		public bool mandatory = false;

		public string urlMasterBranch = string.Empty;

		public bool installed = false;
		public bool oldVersion = false;
		public bool missing = false;

		public List<string> oldNames = null;
		
		private static string defaultName = "PluginName";

		public ActualPlugin (Hashtable source)
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
				version = (string)(source["version"]);
			} 
			catch (Exception) 
			{
				version = "?";
			}
			
			try 
			{
				permitted = new List<string> ();
				var permitted_versions = source["permitted_versions"] as ArrayList;
				foreach (var item in permitted_versions) 
				{
					permitted.Add ((string)item);
				}				
			} 
			catch (Exception) 
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

			try
			{
				displayType = (string)(source["display_type"]);
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
				urlMasterBranch = (string)(source["url"]);
			} 
			catch (Exception) 
			{
				urlMasterBranch = string.Empty;
			}
			
			try 
			{
				mandatory = (bool)(source["mandatory"]);
			} 
			catch (Exception) 
			{
				mandatory = false;
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
		}
	}
}
#endif