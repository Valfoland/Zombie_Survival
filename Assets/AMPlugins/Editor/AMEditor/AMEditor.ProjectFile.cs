#if UNITY_EDITOR
using System.Collections;
using System.IO;

namespace AMEditor
{
	public class ProjectFile
	{
		public string name = "";
		public string fullPath = "";
		public string cause = "";
		public bool selected = false;
		public bool unique = false;
		public bool modifiable = false;
		public bool example = false;
		public bool outdated = false;

		public ProjectFile (string name, string fullPath, bool unique = true, bool modifiable = false, bool example = false, bool outdated = false, bool selected = false, string cause = "")
		{
            #if UNITY_EDITOR_OSX
            string targetFilePath = fullPath;
            #else
            string targetFilePath = fullPath.Replace ('/', '\\');
            #endif

            if (WindowCreateConfigFile.coreFilesList != null) 
			{
				#if UNITY_EDITOR_OSX
				int index = WindowCreateConfigFile.coreFilesList.FindIndex ((f) => {return targetFilePath.Contains (f.fullPath);});
				#else
				int index = WindowCreateConfigFile.coreFilesList.FindIndex ((f) => {return targetFilePath.Contains (f.fullPath.Replace ('/', '\\'));});
				#endif
                if (index != -1)
				{
					selected = true;
					unique = WindowCreateConfigFile.coreFilesList [index].unique;
				}
			}

			if (WindowCreateConfigFile.modifiableFilesList != null)
			{
				#if UNITY_EDITOR_OSX
				int index = WindowCreateConfigFile.modifiableFilesList.FindIndex ((f) => {return targetFilePath.Contains (f.fullPath);});
				#else
				int index = WindowCreateConfigFile.modifiableFilesList.FindIndex ((f) => {return targetFilePath.Contains (f.fullPath.Replace ('/', '\\'));});
				#endif
				if (index != -1)
				{
					modifiable = true;
					selected = true;
				}
			}

			if (WindowCreateConfigFile.examplesFilesList != null)
			{
				#if UNITY_EDITOR_OSX
				int index = WindowCreateConfigFile.examplesFilesList.FindIndex ((f) => {return targetFilePath.Contains (f.fullPath);});
				#else
				int index = WindowCreateConfigFile.examplesFilesList.FindIndex ((f) => {return targetFilePath.Contains (f.fullPath.Replace ('/', '\\'));});
				#endif
				if (index != -1)
				{
					example = true;
					selected = true;
				}
			}

			if (fullPath.Contains (".bundle"))
			{
				unique = false;
			}

			this.name = name;
			this.fullPath = fullPath;
			this.unique = unique;
			this.modifiable = modifiable;
			this.example = example;
			this.outdated = outdated;
			this.selected = selected;
			this.cause = cause;
		}
		public ProjectFile (Hashtable source)
		{
			this.fullPath = (string)(source["name"]);
			this.name = this.fullPath.Substring (this.fullPath.LastIndexOf ("/") + 1);

			var parameters = source ["parameters"] as ArrayList;
			if (parameters !=null)
			{
				foreach (var item in parameters) 
				{
					switch ((string)item)
					{
					case "uniq":
						this.unique = true;
						break;
					case "unique":
						this.unique = true;
						break;
					case "modifiable":
						this.modifiable = true;
						break;
					case "example":
						this.example = true;
						break;
					case "outdated":
						this.outdated = true;
						break;
					default:
						this.unique = false;
						break;
					}
				}
			}
		}
		public Hashtable ToHashTable ()
		{
			Hashtable result = new Hashtable ();
			ArrayList parameters = new ArrayList ();

			result.Add ("name", this.fullPath.Replace ("\\", "/"));

			if (this.unique)
			{
				parameters.Add ("unique");
			}
			if (this.modifiable)
			{
				parameters.Add ("modifiable");
			}
			if (this.example)
			{
				parameters.Add ("example");
			}
			if (this.outdated)
			{
				parameters.Add ("outdated");
				result.Add ("hash", "");
			}
			else
			{
				result.Add ("hash", AMEditorFileStorage.HashFile (this.fullPath));
			}
			result.Add ("parameters", parameters);

			return result;
		}
	}
}
#endif