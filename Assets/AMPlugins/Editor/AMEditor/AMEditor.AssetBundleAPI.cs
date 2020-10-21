#if UNITY_EDITOR && CUSTOM_CODE_ASSET_BUNDLES
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace AMEditor
{
    public class MyWindow : EditorWindow
    {
        bool[] tmp = new bool[40];

        void OnGUI()
        {
            bool forAll = (this.titleContent.text[0] == '1') ? true : false;
            this.titleContent.text = "Выбор сцен";
            var vc = new Vector2(450, 0);
            vc = EditorGUILayout.BeginScrollView(vc);
            GUILayout.Label("Выберите сцены:", EditorStyles.boldLabel);

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            int scncnt = scenes.Count();
            tmp[0] = false;
            for (int i = 1; i < scncnt; ++i)
            {
                tmp[i] = EditorGUILayout.Toggle(scenes[i].path.Substring(scenes[i].path.LastIndexOf('/') + 1), tmp[i]);
            }

            if (GUILayout.Button("Выбрать"))
            {
                List<EditorBuildSettingsScene> res = new List<EditorBuildSettingsScene>();
                for (int i = 1; i < scncnt; ++i)
                {
                    if (tmp[i])
                    {
                        res.Add(scenes[i]);
                    }
                }
                EditorBuildSettingsScene[] newScenesList = EditorBuildSettings.scenes;
                for (int i = 1; i < newScenesList.Length; i++)
                {
                    newScenesList[i].enabled = true;
                }
                EditorBuildSettings.scenes = newScenesList;
                if (res.Count == 0)
                {
                    string titl = res.Count == 0 ? AMEditorSystem.ContentOtherWindow._TitleWarning : AMEditorSystem.ContentOtherWindow._TitleErrorPopup;
                    string message = res.Count == 0 ? AMEditorSystem.ContentAssetBundles._NotEnoughScenesMessage : AMEditorSystem.ContentAssetBundles._ScenesNotFoundMessage;

                    if (EditorUtility.DisplayDialog(titl, message, AMEditorSystem.ContentStandardButton._Ok, ""))
                    {
                        res.Clear();
                    }
                }
                else
                {
                    AMEditorAssetBundleAPI.rightScenes = res.ToArray();
                    AMEditorAssetBundleAPI.chosedScenes = tmp;
                    if (!AMEditorAssetBundleAPI.AddActiveScenes())
                        return;

                    if (EditorUtility.DisplayDialog(AMEditorSystem.ContentAssetBundles._DialogTitle, AMEditorSystem.ContentAssetBundles._DialogMessage,
                           AMEditorSystem.ContentAssetBundles._Continue, AMEditorSystem.ContentStandardButton._Cancel))
                    {
                        AMEditorAssetBundleAPI.MakeAssetBundle(forAll);
                    }
                    AMEditorAssetBundleAPI.RemoveAllScenes();
                    this.Close();
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }

    class AMEditorAssetBundleAPI 
	{
		static BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        static BuildTargetGroup currentBuildTargetGroup = GetGroupForBuildTarget(currentBuildTarget);
        public static EditorBuildSettingsScene[] rightScenes;
        public static bool[] chosedScenes;
        static AMEditor.WindowMain.WindowType currentAMEditorWindow = AMEditor.WindowMain.WindowType.Wait;

		public static MethodInfo getAssetBundlePathMethodInfo;
		static MethodInfo getPlatformMethodInfo;
		static string packageNameFieldValue = string.Empty;
		public static string infoFileNameFieldValue = string.Empty;
		static string assetbundlesFolderNameFieldValue = string.Empty;

		public static bool InitReflection ()
		{
			var assetBundlesClassType = System.Type.GetType ("CustomCode.CustomResourcesManagerAssetBundles, Assembly-CSharp");
			if (assetBundlesClassType == null)
			{
				return false;
			}

			FieldInfo packageNameFieldInfo = assetBundlesClassType.GetField ("PACKAGE_NAME");
			if (packageNameFieldInfo == null)
			{
				return false;
			}
			packageNameFieldValue = packageNameFieldInfo.GetValue (null).ToString ();
			if (string.IsNullOrEmpty (packageNameFieldValue))
			{
				return false;
			}

			FieldInfo infoFileNameNameFieldInfo = assetBundlesClassType.GetField ("INFO_FILE_NAME");
			if (infoFileNameNameFieldInfo == null)
			{
				return false;
			}
			infoFileNameFieldValue = infoFileNameNameFieldInfo.GetValue (null).ToString ();
			if (string.IsNullOrEmpty (infoFileNameFieldValue))
			{
				return false;
			}
			
			FieldInfo assetbudnlesFolderNameNameFieldInfo = assetBundlesClassType.GetField ("ASSETBUNDLES_FOLDER_NAME");
			if (assetbudnlesFolderNameNameFieldInfo == null)
			{
				return false;
			}
			assetbundlesFolderNameFieldValue = assetbudnlesFolderNameNameFieldInfo.GetValue (null).ToString ();
			if (string.IsNullOrEmpty (assetbundlesFolderNameFieldValue))
			{
				return false;
			}

			getAssetBundlePathMethodInfo = assetBundlesClassType.GetMethod ("GetAssetBundlePath");
			if (getAssetBundlePathMethodInfo == null)
			{
				return false;
			}

			getPlatformMethodInfo = assetBundlesClassType.GetMethod ("GetPlatform");
			if (getPlatformMethodInfo == null)
			{
				return false;
			}

			return true;
		}

		public static bool AddActiveScenes ()
		{
			if (!InitReflection ())
				return false;

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            for (int i = 0; i < scenes.Length; i++)
			{
                if (!chosedScenes[i]) continue; //если сцена не выбрана пропускаем
                EditorBuildSettingsScene scene = scenes [i];
				string name = scene.path.Substring (scene.path.LastIndexOf ('/') + 1);
				name = name.Substring (0, name.Length - 6);

				AssetImporter assetImporter = AssetImporter.GetAtPath (scene.path);

				if (scene.enabled) 
				{
					assetImporter.SetAssetBundleNameAndVariant (packageNameFieldValue, string.Empty);
				}
				else 
					assetImporter.SetAssetBundleNameAndVariant (string.Empty, string.Empty);
			}
			return true;
		}

		public static void RemoveAllScenes ()
		{
			string[] allAssetsPathes = AssetDatabase.GetAllAssetPaths ();
			for (int i = 0; i < allAssetsPathes.Length; i++) 
			{
				AssetImporter assetImporter = AssetImporter.GetAtPath (allAssetsPathes[i]);
                if (assetImporter != null && assetImporter.assetBundleName != string.Empty) 
					assetImporter.SetAssetBundleNameAndVariant (string.Empty, string.Empty);
			}
			AssetDatabase.RemoveUnusedAssetBundleNames ();
		}

		public static void MakeAssetBundle (bool forAllPLatforms = false)
		{
			currentAMEditorWindow = AMEditor.WindowMain.currentWindow;
			AMEditor.WindowMain.currentWindow = AMEditor.WindowMain.WindowType.Wait;
			AMEditor.WindowMain.messageWait = "Prepare to Asset Bundles build...";

			PlayerSettings.stripEngineCode = false;
			
			List<BuildTarget> requiredBuildTargets = new List<BuildTarget> ();
			if (forAllPLatforms)
			{
				currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
                currentBuildTargetGroup = GetGroupForBuildTarget (currentBuildTarget);

				requiredBuildTargets.Add (BuildTarget.Android);
				requiredBuildTargets.Add (BuildTarget.iOS);
				requiredBuildTargets.Add (BuildTarget.tvOS);
				requiredBuildTargets.Add (BuildTarget.StandaloneOSX);
				#if !UNITY_EDITOR_OSX
				requiredBuildTargets.Add (BuildTarget.StandaloneWindows);
				requiredBuildTargets.Add (BuildTarget.StandaloneWindows64);
				requiredBuildTargets.Add (BuildTarget.WSAPlayer);
				#endif
			}
			else
			{
				requiredBuildTargets.Add (EditorUserBuildSettings.activeBuildTarget);
			}

			string error = string.Empty;
			foreach (var item in requiredBuildTargets)
			{
				AMEditor.WindowMain.messageWait = "Building asset bundle for " + item.ToString ();

				if (item != EditorUserBuildSettings.activeBuildTarget)
				{
					EditorUserBuildSettings.SwitchActiveBuildTarget (GetGroupForBuildTarget (item), item);
				}

				string outputPath = getAssetBundlePathMethodInfo.Invoke (null, new System.Object[]{true}).ToString ();

				if (!Directory.Exists (outputPath))
					Directory.CreateDirectory (outputPath);
				
				BuildPipeline.BuildAssetBundles (outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

				bool successCreation = SaveInfo ();

				if (!successCreation)
				{
					error = string.IsNullOrEmpty (error) ? 
						AMEditorSystem.ContentAssetBundles._ErrorMessage + getPlatformMethodInfo.Invoke (null, null).ToString () + "\n" :
						error + getPlatformMethodInfo.Invoke (null, null).ToString () + "\n";
				}
			}

			EditorUserBuildSettings.SwitchActiveBuildTarget (currentBuildTargetGroup, currentBuildTarget);

			AMEditor.WindowMain.messageWait = string.Empty;
			AMEditor.WindowMain.currentWindow = currentAMEditorWindow;

			string dialogTitle = string.IsNullOrEmpty (error) ? 
				AMEditorSystem.ContentAssetBundles._DialogTitle :
				AMEditorSystem.ContentOtherWindow._TitleErrorPopup;
			string dialogMessage = string.IsNullOrEmpty (error) ? 
				AMEditorSystem.ContentAssetBundles._SuccessCreationMessage :
				error;
			if (EditorUtility.DisplayDialog (dialogTitle, dialogMessage, AMEditorSystem.ContentStandardButton._Ok, ""))
				AssetDatabase.Refresh ();
			
			try
			{
				string nextScenePath = EditorBuildSettings.scenes[1].path;
				string nextSceneName = nextScenePath.Substring (nextScenePath.LastIndexOf ('/') + 1);
				nextSceneName = nextSceneName.Substring (0, nextSceneName.LastIndexOf ('.'));

				var crmScenePath = CustomResourceManagerAPI.FindSceneWithCRM ();
				if (string.IsNullOrEmpty (crmScenePath))
					crmScenePath = EditorBuildSettings.scenes[0].path;

				var crmScene = EditorSceneManager.OpenScene (crmScenePath);

				var crmPrefabs = crmScene.GetRootGameObjects ().ToList ().FindAll ((go)=>{return go.name == "CustomResourcesManager";});

				if (crmPrefabs != null && crmPrefabs.Count > 0)
				{
					var targetPrefab = crmPrefabs[0];
					var targetComponent = targetPrefab.GetComponent ("CustomResourcesManager");

					Type targetType = System.Type.GetType ("CustomCode.CustomResourcesManager, Assembly-CSharp");

					FieldInfo enableAssetBundlesFI = targetType.GetField ("enableAssetBundles");
					enableAssetBundlesFI.SetValue (targetComponent, true);

					FieldInfo nextSceneNameFI = targetType.GetField ("assetBundlesNextSceneName");
					nextSceneNameFI.SetValue (targetComponent, nextSceneName);

					EditorSceneManager.SaveScene (crmScene);

					Selection.activeGameObject = targetPrefab;
				}
				else
				{
					AMEditorPopupErrorWindow.ShowErrorPopup ("302", AMEditorSystem.ContentError._302);
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError (ex.ToString ());
			}

			EditorBuildSettingsScene[] newScenesList = EditorBuildSettings.scenes;
			for (int i = 1; i < newScenesList.Length; i++)
			{
				newScenesList [i].enabled = false;
			}
			EditorBuildSettings.scenes = newScenesList;
		}

		static BuildTargetGroup GetGroupForBuildTarget (BuildTarget target)
		{
			switch (target)
			{
			case BuildTarget.Android:
				return BuildTargetGroup.Android;
			case BuildTarget.iOS:
				return BuildTargetGroup.iOS;
			case BuildTarget.tvOS:
				return BuildTargetGroup.tvOS;
			case BuildTarget.StandaloneOSX:
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return BuildTargetGroup.Standalone;
			case BuildTarget.WSAPlayer:
				return BuildTargetGroup.WSA;
			default:
				return BuildTargetGroup.Unknown;
			}
		}

		static bool SaveInfo ()
		{
			string packagePath = Path.Combine (getAssetBundlePathMethodInfo.Invoke (null, new System.Object[]{true}).ToString (), packageNameFieldValue);

			if (!File.Exists (packagePath))
			{
				Debug.LogError (string.Format ("Файл {0} не найден!", packagePath));
				return false;
			}
			FileInfo packageFile = new FileInfo (packagePath);

			string resultPath = Path.Combine (getAssetBundlePathMethodInfo.Invoke (null, new System.Object[]{true}).ToString (), infoFileNameFieldValue);

			int version = 0;
			if (File.Exists (resultPath))
			{
				string existingInfoString = File.ReadAllText (resultPath);
				var existingInfo = AMEditorJSON.JsonDecode (existingInfoString) as Hashtable;
				if (existingInfo != null)
				{
					try
					{
						version = Convert.ToInt32 (existingInfo["version"].ToString ());
					}
					catch (Exception)
					{}
				}
			}
			version += 1;

			string manifestPath = packagePath + ".manifest";
			uint crc = 0;
			List<string> manifest = File.ReadAllLines (manifestPath).ToList ();
			var crcLine = manifest.Find (line => {return line.Contains ("CRC");});
			if (string.IsNullOrEmpty (crcLine))
			{
				Debug.LogError ("Не удалось получить контрольную сумму!");
				return false;
			}
			try
			{
				crc = uint.Parse (crcLine.Substring (crcLine.LastIndexOf (" ")));
			}
			catch (Exception)
			{
				Debug.LogError ("Не удалось получить контрольную сумму!");
				return false;
			}

			Hashtable newInfo = new Hashtable ();
			newInfo.Add ("size", (int)packageFile.Length);
			newInfo.Add ("version", version);
			newInfo.Add ("checksum", crc);
			string result = AMEditorJSON.JsonEncode (newInfo);
			result = AMEditorJSON.FormatJson (result);

			StreamWriter streamWriter = new StreamWriter (resultPath);
			streamWriter.WriteLine (result);
			streamWriter.Close ();

			return true;
		}

		[UnityEditor.Callbacks.PostProcessBuild (100)]
		public static void OnPostprocessBuild (BuildTarget target, string pathToBuiltProject) 
		{
			string assetBundlesPath = null;

			switch (target) 
			{
			case BuildTarget.Android:
				assetBundlesPath = Path.Combine (pathToBuiltProject, PlayerSettings.productName);
				assetBundlesPath = Path.Combine (assetBundlesPath, "assets");
				break;
			case BuildTarget.iOS:
			case BuildTarget.tvOS:
				assetBundlesPath = Path.Combine (pathToBuiltProject, "Data");
				assetBundlesPath = Path.Combine (assetBundlesPath, "Raw");
				break;
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				string root = Path.Combine (Path.GetDirectoryName (pathToBuiltProject), Path.GetFileNameWithoutExtension (pathToBuiltProject) + "_Data");
				assetBundlesPath = Path.Combine (root, "StreamingAssets");
				break;
			case BuildTarget.WSAPlayer:
				assetBundlesPath = Path.Combine (pathToBuiltProject, PlayerSettings.productName);
				assetBundlesPath = Path.Combine (assetBundlesPath, "Data");
				assetBundlesPath = Path.Combine (assetBundlesPath, "StreamingAssets");
				break;
			case BuildTarget.StandaloneOSX:
				assetBundlesPath = Path.Combine (pathToBuiltProject, "Contents");
				assetBundlesPath = Path.Combine (assetBundlesPath, "Resources");
				assetBundlesPath = Path.Combine (assetBundlesPath, "Data");
				assetBundlesPath = Path.Combine (assetBundlesPath, "StreamingAssets");
				break;
			}
			assetBundlesPath = Path.Combine (assetBundlesPath, assetbundlesFolderNameFieldValue);

			if (assetBundlesPath == null || !Directory.Exists (assetBundlesPath))
				return;

			var assetBundlesArray = Directory.GetDirectories (assetBundlesPath);
			for (int i = 0; i < assetBundlesArray.Length; i++)
			{
				string platformAssetBundle = assetBundlesArray[i];

				if (platformAssetBundle.Substring (platformAssetBundle.LastIndexOf (Path.DirectorySeparatorChar) + 1) == getPlatformMethodInfo.Invoke (null, null).ToString ())
					continue;
				
				Directory.Delete (platformAssetBundle, true);
			}
		}
	}
}
#endif