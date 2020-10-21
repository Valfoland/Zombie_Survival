#if UNITY_EDITOR
using System.Collections;
using UnityEngine;

namespace AMEditor
{
	public class GitAccount
	{
		public string name;
		public string privateToken;
		public string server;

		static GitAccount _current;

		public static GitAccount current 
		{
			get
			{
				if (_current == null)
				{
					if (!LoadGitAccountsFromFile ())
					{
						_current = null;
					}
				}
				return _current;
			}
			set
			{
				if (value != null)
				{
					_current = value;
				}
				if (_current != null)
				{
					SaveGitAccountsInFile ();
				}
			}
		}

		static void SaveGitAccountsInFile ()
		{
			ArrayList accs = new ArrayList ();
			accs.Add (_current.ToHashtable ());
			string list = AMEditorJSON.JsonEncode (accs);
			System.IO.File.WriteAllText (LocalRepositoryAPI.pathToRepository + AMEditorSystem.FileNames._Account, AMEditorJSON.FormatJson (list));
		}

		static bool LoadGitAccountsFromFile ()
		{
			ArrayList list = null;
			if (AMEditorFileStorage.FileExist (LocalRepositoryAPI.pathToRepository + AMEditorSystem.FileNames._Account))
			{
				list = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (LocalRepositoryAPI.pathToRepository + AMEditorSystem.FileNames._Account)) as ArrayList;	 
			}
			if ((list != null) && (list.Count > 0))
			{
				_current = new GitAccount (list[0] as Hashtable); 
				return true;
			}
			return false;
		}

		public GitAccount ()
		{
			name = string.Empty;
			privateToken = string.Empty;
			server = string.Empty;
		}

		public GitAccount (Hashtable source)
		{
			name = (string)source ["user"];
			privateToken = (string)source ["private_token"];
			server = (string)source ["server"];
		}
		public Hashtable ToHashtable ()
		{
			Hashtable result = new Hashtable ();
			result.Add ("user", name);
			result.Add ("private_token", privateToken);
			result.Add ("server", server);
			return result;
		}
	}
}
#endif