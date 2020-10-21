#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMEditor
{
	public class AMEditorDefineController
	{
		public static void AddDefine (string newDefine)
		{
			if (string.IsNullOrEmpty (newDefine))
				return;
			try
			{
				var buildTargetGroupsList = Enum.GetValues (typeof (BuildTargetGroup)).OfType<BuildTargetGroup> ().ToList ();
				foreach (var buildTargetGroup in buildTargetGroupsList) 
				{
					if (!BuildTargetGroupIsAvailable (buildTargetGroup))
						continue;
					string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup (buildTargetGroup);
					if (currentDefines != null && !currentDefines.Contains (newDefine))
						PlayerSettings.SetScriptingDefineSymbolsForGroup (buildTargetGroup, currentDefines + ";" + newDefine);
				}
				AssetDatabase.Refresh ();
			}
			catch (Exception)
			{}
		}

		public static void RemoveDefine (string targetDefine)
		{
			if (string.IsNullOrEmpty (targetDefine))
				return;
			try
			{
				var buildTargetGroupsList = Enum.GetValues (typeof (BuildTargetGroup)).OfType<BuildTargetGroup> ().ToList ();
				foreach (var buildTargetGroup in buildTargetGroupsList) 
				{
					if (!BuildTargetGroupIsAvailable (buildTargetGroup))
						continue;
					string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup (buildTargetGroup);
					if (currentDefines != null && currentDefines.Contains (targetDefine))
						PlayerSettings.SetScriptingDefineSymbolsForGroup (buildTargetGroup, RemoveOldValue (currentDefines, targetDefine));
				}
				AssetDatabase.Refresh ();
			}
			catch (Exception)
			{}
		}

		public static void UpdateDefine (object newValue, string prefix)
		{
			if (string.IsNullOrEmpty (prefix) || newValue == null)
				return;
			try
			{
				var buildTargetGroupsList = Enum.GetValues (typeof (BuildTargetGroup)).OfType<BuildTargetGroup> ().ToList ();
				foreach (var buildTargetGroup in buildTargetGroupsList) 
				{
					if (!BuildTargetGroupIsAvailable (buildTargetGroup))
						continue;
					var cleanDefine = RemoveOldValue (PlayerSettings.GetScriptingDefineSymbolsForGroup (buildTargetGroup), prefix);                 
					PlayerSettings.SetScriptingDefineSymbolsForGroup (buildTargetGroup, cleanDefine + ";" + prefix + "_" + newValue.ToString ());
				}
				AssetDatabase.Refresh ();
			}
			catch (Exception)
			{}
		}

		public static bool BuildTargetGroupIsAvailable (BuildTargetGroup buildTargetGroup)
		{
            switch(buildTargetGroup){
                case BuildTargetGroup.Android:
                case BuildTargetGroup.iOS:
                case BuildTargetGroup.Facebook:
                case BuildTargetGroup.PS4:
                case BuildTargetGroup.Standalone:
                case BuildTargetGroup.Switch:
                case BuildTargetGroup.tvOS:
                    return true;
            }
            return false;
		}

		public static string RemoveOldValue (string defineSymbols, string definePrefix)
		{
			string result = string.Empty;
			var define = defineSymbols.Split (new Char[]{';'}, StringSplitOptions.RemoveEmptyEntries);

			foreach (var item in define) 
			{
				if (!item.Contains (definePrefix))
				{
					result += item + ";";
				}
			}
			return result;
		}
	}
}
#endif