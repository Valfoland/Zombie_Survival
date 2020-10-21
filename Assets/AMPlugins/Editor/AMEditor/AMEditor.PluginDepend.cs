#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMEditor
{
	public class Depend
	{
		public string name;
		public string version;
		public string mod;

		public Depend ()
		{
			name = string.Empty;
			version = string.Empty;
			mod = ">=";
		}

		public Depend (Hashtable source)
		{
			name = (string)source["name"];
			version = (string)source["version"];
			if (version == null)
				version = string.Empty;
			mod = (string)source["mod"];
			if (string.IsNullOrEmpty (mod))
				mod = ">=";
		}
		public void CopyTo (Depend depend)
		{
			depend = new Depend ();
			depend.name = name;
			depend.version = version;
			depend.mod = mod;
		}
		public Hashtable ToHashtable ()
		{
			Hashtable result = new Hashtable ();
			result.Add ("name", name);
			result.Add ("version", version);
			result.Add ("mod", mod);
			return result;
		}
	}
}
#endif