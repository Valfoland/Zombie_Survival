#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

namespace AMEditor
{
	class CustomPostProcessBuild
	{
		[UnityEditor.Callbacks.PostProcessBuild]
		public static void OnPostProcessBuild (BuildTarget target, string path) 
		{
			#region add app_info.json
			string streamingAssetsPath = null;

			switch (target) 
			{
			case BuildTarget.Android:
				var apkName = path.Substring (path.LastIndexOf (Path.DirectorySeparatorChar) + 1);
				if (apkName.Contains (".apk"))
				{
					path = path.Substring (0, path.Length - apkName.Length);
				}
				else
				{
					streamingAssetsPath = Path.Combine (path, PlayerSettings.productName);
					streamingAssetsPath = Path.Combine (streamingAssetsPath, "src");
					streamingAssetsPath = Path.Combine (streamingAssetsPath, "main");
					streamingAssetsPath = Path.Combine (streamingAssetsPath, "assets");
				}
				break;
			case BuildTarget.iOS:
			case BuildTarget.tvOS:
				streamingAssetsPath = Path.Combine (path, "Data");
				streamingAssetsPath = Path.Combine (streamingAssetsPath, "Raw");
				break;
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				string root = Path.Combine (Path.GetDirectoryName (path), Path.GetFileNameWithoutExtension (path) + "_Data");
				streamingAssetsPath = Path.Combine (root, "StreamingAssets");
				break;
			case BuildTarget.WSAPlayer:
				streamingAssetsPath = Path.Combine (path, PlayerSettings.productName);
				streamingAssetsPath = Path.Combine (streamingAssetsPath, "Data");
				streamingAssetsPath = Path.Combine (streamingAssetsPath, "StreamingAssets");
				break;
			case BuildTarget.StandaloneOSX:
				streamingAssetsPath = Path.Combine (path, "Contents");
				streamingAssetsPath = Path.Combine (streamingAssetsPath, "Resources");
				streamingAssetsPath = Path.Combine (streamingAssetsPath, "Data");
				streamingAssetsPath = Path.Combine (streamingAssetsPath, "StreamingAssets");
				break;
			}

			if (streamingAssetsPath != null && Directory.Exists (streamingAssetsPath))
			{
				AppInfoAPI.CreateFile (streamingAssetsPath);
			}
			#endregion

			AMLocalizationAPI.ZipAndMoveLocalization (path);

			#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
			var bun = SearchBundle (path);
			var message = "Найдены .bundle в Xcode проекте: " + Environment.NewLine;
			foreach (var item in bun) 
			{
				message += item.Substring (item.LastIndexOf (Path.DirectorySeparatorChar) + 1) + Environment.NewLine;
			}
			if (bun.Count > 0)
			{
				EditorUtility.DisplayDialog ("Warning", message, "ok");
				throw new Exception (message);
			}
			#endif
		}
		public static List<string> SearchBundle (string path)
		{
			var result = new List<string> ();
			var listFolder = Directory.GetDirectories (path);
			foreach (var item in listFolder) 
			{
				if (item.Contains (".bundle"))
				{
					result.Add (item);
				}
				else
				{
					var bundles = SearchBundle (item);
					foreach (var b in bundles) 
					{
						result.Add (b);
					}
				}
			}
			return result;
		}
	}
}
#endif