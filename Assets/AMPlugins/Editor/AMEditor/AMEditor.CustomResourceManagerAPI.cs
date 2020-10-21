#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace AMEditor
{
	public class CustomResourceManagerAPI
	{
		static List<GameObject> crmPrefabsList = new List<GameObject> ();
		static string crmName = "CustomResourcesManager";
		static string scriptPath = "Assets/AMPlugins/CustomResourcesManager/CustomResourcesManager.cs";
		static string prefabPath = "Assets/AMPlugins/CustomResourcesManager/CustomResourcesManager.prefab";
		static string amProjectPath = "Assets/StreamingAssets/am_project.txt";
		static string amBuildsPath = "Assets/StreamingAssets/am_builds.txt";
		public static string assetBundlesScriptPath = "Assets/AMPlugins/CustomResourcesManager/CustomResourcesManagerAssetBundles.cs";
		public static string assetBundlesDefine = "CUSTOM_CODE_ASSET_BUNDLES";

		static Scene startScene;
		static string sceneCustomCodePath = "Assets/AMPlugins/CustomCode/SceneCustomCode.unity";

		public static string FindSceneWithCRM ()
		{
			if (EditorBuildSettings.scenes.Length == 0)
				return string.Empty;

			foreach (var scene in EditorBuildSettings.scenes)
			{
				if (scene.path == string.Empty || !scene.enabled)
					continue;
				var crmGOs = (from p in GameObject.FindObjectsOfType (typeof (GameObject))
					where p.name.Contains (crmName)
					select p as GameObject).ToList ();
				if (crmGOs != null && crmGOs.Count > 0)
				{
					return scene.path;
				}
			}
			return string.Empty;
		}

		public static void RestoreCRMComponents (bool force = false)
		{
			Debug.Log (AMEditorSystem.ContentOtherWindow._RestoringCRMWaitMessage);

			string currentScenePath = string.Empty;

			currentScenePath = EditorSceneManager.GetActiveScene ().path;

			if (force && File.Exists (sceneCustomCodePath))
			{
				File.Delete (sceneCustomCodePath);
				AssetDatabase.Refresh ();
			}
			Scene sceneCustomCode = new Scene ();
			if (force)
			{
				sceneCustomCode = EditorSceneManager.NewScene (NewSceneSetup.EmptyScene, NewSceneMode.Single);
				GameObject cameraGO = new GameObject ("Main Camera");
				cameraGO.tag = "MainCamera";
				ConfigureCameraObject (cameraGO);
			}
			else
			{
				try
				{
					sceneCustomCode = EditorSceneManager.OpenScene (sceneCustomCodePath);
				}
				catch (Exception)
				{
					return;
				}
				var missingGOs = (from p in GameObject.FindObjectsOfType (typeof (GameObject))
					where p.name.Contains ("Missing Prefab") || p.name.Equals ("")
					select p as GameObject).ToList ();
				if (missingGOs != null && missingGOs.Count > 0)
				{
					foreach (var mgo in missingGOs)
					{
						UnityEngine.Object.DestroyImmediate (mgo);
					}
				}
				var oldCrmGOs = (from p in GameObject.FindObjectsOfType (typeof (GameObject))
					where p.name.Contains (crmName)
					select p as GameObject).ToList ();
				if (oldCrmGOs != null && oldCrmGOs.Count > 0)
				{
					foreach (var go in oldCrmGOs)
					{
						UnityEngine.Object.DestroyImmediate (go);
					}
				}
			}
			if (!File.Exists (scriptPath))
				return;
			
			System.Type scriptType = null;
			try
			{
				scriptType = System.Type.GetType ("CustomCode.CustomResourcesManager, Assembly-CSharp");
			}
			catch (Exception)
			{}

			GameObject crmGO = new GameObject (crmName);
			if (scriptType == null && AMEditor.WindowMain.launchMode == AMEditor.WindowMain.LaunchMode.UI)
				return;
			crmGO.AddComponent (scriptType);

			if (File.Exists (prefabPath)) 
			{
				File.Delete (prefabPath);
				AssetDatabase.Refresh ();
			}
			PrefabUtility.CreatePrefab (prefabPath, crmGO);
			AssetDatabase.Refresh ();
			UnityEngine.Object.DestroyImmediate (crmGO);

			var crmPrefabAsset = AssetDatabase.LoadAssetAtPath (prefabPath, typeof (GameObject));

			if (crmPrefabAsset != null) 
			{
				UnityEngine.Object crmPrefab = PrefabUtility.InstantiateAttachedAsset (crmPrefabAsset);
				crmPrefab.name = crmPrefab.name.Replace (" (Clone)", "");
				crmPrefab.name = crmPrefab.name.Replace ("(Clone)", "");

				crmPrefabsList.Add ((GameObject)crmPrefab);
			}

			List<GameObject> foundedCameras = new List<GameObject> ();
			foundedCameras = sceneCustomCode.GetRootGameObjects ().ToList ().FindAll ((go)=>{return go.GetType () == (typeof (GameObject)) && go.name == "Main Camera";});
			if (foundedCameras != null && foundedCameras.Count > 0)
			{
				int index = foundedCameras.FindIndex ((p) => {return p.GetComponent ("Camera") != null;});
				if (index != -1)
				{
					ConfigureCameraObject (foundedCameras[index]);
				}
			}
			else
			{
				GameObject cameraGO = new GameObject ("Main Camera");
				cameraGO.tag = "MainCamera";
				ConfigureCameraObject (cameraGO);
			}

			EditorSceneManager.SaveScene (sceneCustomCode, sceneCustomCodePath);

			if (currentScenePath != string.Empty)
			{
				EditorSceneManager.OpenScene (currentScenePath);
			}
			CheckAssetBundlesAvailability ();
		}

		static void ConfigureCameraObject (GameObject cameraGameObject)
		{
			if (cameraGameObject.GetComponent <Camera> () == null)
				cameraGameObject.AddComponent <Camera> ();
			cameraGameObject.GetComponent <Camera> ().backgroundColor = Color.black;
			cameraGameObject.GetComponent <Camera> ().orthographic = true;
			cameraGameObject.GetComponent <Camera> ().orthographicSize = 5;
			cameraGameObject.GetComponent <Camera> ().backgroundColor = Color.black;
			if (cameraGameObject.GetComponent <FlareLayer> () == null)
				cameraGameObject.AddComponent <FlareLayer> ();
			if (cameraGameObject.GetComponent <AudioListener> () == null)
				cameraGameObject.AddComponent <AudioListener> ();
		}

		public static bool LoadStartScene ()
		{
			try 
			{
				var startScenePath = string.Empty;
				foreach (var item in EditorBuildSettings.scenes) 
				{
					if (item.enabled)
					{
						startScenePath = item.path;
						break;
					}
				}
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ())
				{
					startScene = EditorSceneManager.OpenScene (startScenePath);
				}
				else
				{
					return false;
				}
				return true;
			}
			catch (Exception) 
			{
				return false;
			}
		}

		public static int CheckCorrect (bool force = false)
		{
			int result = 0;
			string currentScenePath = string.Empty;

			if (force)
				RestoreCRMComponents ();

			currentScenePath = EditorSceneManager.GetActiveScene ().path;
			LoadStartScene ();

			result = (CheckPrefab ());

			if (result == 0)
			{
				result = CheckScript ();
			}
			try
			{
				EditorSceneManager.SaveScene (startScene);
				if (currentScenePath != string.Empty)
				EditorSceneManager.OpenScene (currentScenePath);
			}
			catch (System.Exception)
			{}
			if (!File.Exists (amProjectPath))
			{
				AMEditorPopupErrorWindow.ShowErrorPopup ("313", AMEditorSystem.ContentError._313);
				return 313;
			}

			if (!File.Exists (amBuildsPath))
			{
				AMEditorPopupErrorWindow.ShowErrorPopup ("314", AMEditorSystem.ContentError._314);
				return 314;
			}
			return result;
		}

		static int CheckPrefab ()
		{
			try
			{
				List<GameObject> foundedPrefabs = new List<GameObject> ();
				foundedPrefabs = 
					 (from p in GameObject.FindObjectsOfType (typeof (GameObject))
					where p.name.Contains (crmName)
					select p as GameObject).ToList ();
				if (foundedPrefabs != null && foundedPrefabs.Count > 0)
				{
					crmPrefabsList = foundedPrefabs;
				}
				else
				{
					AMEditorPopupErrorWindow.ShowErrorPopup ("302", AMEditorSystem.ContentError._302);
					return 302;
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogException (ex);
			}
			return 0;
		}

		static int CheckScript ()
		{
			try
			{
				try
				{
					AssetDatabase.ImportAsset (scriptPath);
				}
				catch (Exception)
				{}

				int index = crmPrefabsList.FindIndex ((p) => { return p.GetComponent (crmName) != null; });

				if (index == -1)
				{
					AMEditorPopupErrorWindow.ShowErrorPopup ("303", AMEditorSystem.ContentError._303);
					return 303;
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogException (ex);
			}
			return 0;
		}

		public static void CheckAssetBundlesAvailability ()
		{
				if (File.Exists (assetBundlesScriptPath))
				{
					#if !CUSTOM_CODE_ASSET_BUNDLES
					AMEditorDefineController.AddDefine (assetBundlesDefine);
					#endif
				}
				else
				{
					#if CUSTOM_CODE_ASSET_BUNDLES
					AMEditorDefineController.RemoveDefine (assetBundlesDefine);
					#endif
				}
		}
	}
}
#endif