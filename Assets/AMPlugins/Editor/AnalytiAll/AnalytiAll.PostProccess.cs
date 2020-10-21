#if UNITY_EDITOR
using System;	
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_2017_4_OR_NEWER || UNITY_2018_2_OR_NEWER || UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER && (UNITY_IPHONE || UNITY_IOS || UNITY_TVOS)
using UnityEditor.iOS.Xcode;
#endif

namespace AnalytiAll
{
	public static class AnalytiAllPostProccess
	{
		public static void PostProccess()
		{

		}

		[UnityEditor.Callbacks.PostProcessBuild]
		public static void ChangeXcodePlist (BuildTarget buildTarget, string pathToBuiltProject) 
		{
			Debug.Log ("Starting AnalytiAll PostProcess");
			string innerId = GetinnerID ();
			string keyInnerId = "AnalytiAllInnerId";
			string keyInnerId2 = "AnalytiAllAppId";
			if (buildTarget == BuildTarget.StandaloneOSX)
			{
				string plistPath = pathToBuiltProject + "/Contents/Info.plist";
			#if UNITY_2017_4_OR_NEWER || UNITY_2018_2_OR_NEWER || UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER && (UNITY_IPHONE || UNITY_IOS || UNITY_TVOS)
				PlistDocument trueplist = new PlistDocument ();
				trueplist.ReadFromString (File.ReadAllText (plistPath));
				PlistElementDict rootDict = trueplist.root;
				rootDict.SetString (keyInnerId,innerId);
				rootDict.SetString (keyInnerId2,innerId);
				File.WriteAllText (plistPath, trueplist.WriteToString ());
			#else
				string key = "\t<key>"+keyInnerId+"</key>\n" +
					"\t<string>"+innerId+"</string>\n";
				string key2 = "\t<key>"+keyInnerId2+"</key>\n" +
					"\t<string>"+innerId+"</string>";
				string separator = "</dict>";

				var plistLines = File.ReadAllLines (plistPath).ToList ();
				int index = plistLines.LastIndexOf (separator);
				if (index != -1)
				{
					plistLines.Insert (index, key + key2);
				}
				File.WriteAllLines (plistPath, plistLines.ToArray ());
			#endif		
				var outFolder = pathToBuiltProject + "/Contents/Resources/AnalytiAllModel.momd";
				var inFolder = "Assets/AMPlugins/Editor/AnalytiAll/AnalytiAllModel.momd";
				var filesBase = new string[]{
					"/AnalytiAllModel.mom",
					"/VersionInfo.plist"
				};
				Directory.CreateDirectory (outFolder);

				foreach (var item in filesBase) 
				{
					File.Copy (inFolder + item, outFolder + item, true);
				}
			}
		}

		public static string GetinnerID()
		{
			try 
			{
				string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "am_builds.txt");
				var am_build = File.ReadAllText (filePath);

				var innerId = (string)((((((AnalytiAllJSON.JsonDecode (am_build) as Hashtable)["build_params"]) as ArrayList)[0]) as Hashtable)["innerID"]);
				return innerId;

			} 
			catch (Exception) 
			{
				Debug.LogError("не смог получить inner id");
				return "";
			}
		}
	}
}
#endif