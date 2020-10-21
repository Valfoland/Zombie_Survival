#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace CustomCode
{
	public class CustomResourcesManagerAssetBundles : MonoBehaviour
	{
		enum SizeUnit
		{
			B,
			KB,
			MB,
			GB
		}

		public static CustomResourcesManagerAssetBundles Instance;

		public const string ASSETBUNDLES_FOLDER_NAME = "AssetBundles";
		public const string PACKAGE_NAME = "package";
		public const string PACKAGE_MANIFEST = "package.manifest";
		public const string INFO_FILE_NAME = "info.json";

		public static string outputPath = string.Empty;

		public static string nextSceneName = string.Empty;

		public delegate void LoadingProgress (float progress);
		public static event LoadingProgress loadingProgress;

		public delegate void LoadingSpeed (string speed);
		public static event LoadingSpeed loadingSpeed;

		public delegate void LoadingTime (string time);
		public static event LoadingTime loadingTime;

		private WWW www = null;
		private int packagesCount = 1;
		private int version = 1;
		private uint checksum = 0;

		double size = 0;
		SizeUnit sizeUnit = SizeUnit.B;
		float currentLoadingTime = 0.01f;

		bool isCached = false;

		void Start ()
		{
			if (Instance == null)
			{
				Instance = this;

				if (Application.isEditor) {
					Caching.ClearCache ();
				}
					

				outputPath = GetAssetBundlePath ();

				StartCoroutine (LoadingController ());
			}
			else
			{
				Destroy (gameObject);
			}
		}

		IEnumerator LoadingController ()
		{
			yield return StartCoroutine (GetAssetBundleInfo ());
			try
			{
				string packagePath = Path.Combine (outputPath, PACKAGE_NAME);
				isCached = (Caching.GetCacheByPath(packagePath) != new Cache()) ? true : false;
			}
			catch (Exception ex)
			{
				Debug.LogError (ex.ToString ());
				#if UNITY_STANDALONE_OSX
				Debug.LogError ("AssetBundle loading error!\n" + ex.ToString ());
				#else
				if (CustomResourcesManager.amLogger != null)
					CustomResourcesManager.amLogger.LogError ("AssetBundle loading error!\n" + ex.ToString ());
				#endif
			}
			yield return StartCoroutine (DownloadAndCache ());

			SceneManager.LoadScene (nextSceneName, LoadSceneMode.Single);

			Destroy (this);
		}

		IEnumerator GetAssetBundleInfo ()
		{
			while (true) 
			{
				string infoFilePath = Path.Combine (outputPath, INFO_FILE_NAME);
				www = new WWW (infoFilePath);
				yield return www;

				if (string.IsNullOrEmpty (www.error)) 
				{
					var info = AMUtils.AMJSON.JsonDecode (www.text) as Hashtable;
					try
					{
						size = Convert.ToDouble (info["size"].ToString ());
					}
					catch (Exception ex)
					{
						#if UNITY_STANDALONE_OSX
						Debug.LogError ("Key 'size' not found at info.json!\n" + ex.ToString ());
						#else
						if (CustomResourcesManager.amLogger != null)
							CustomResourcesManager.amLogger.LogError ("Key 'size' not found at info.json!\n" + ex.ToString ());
						#endif
					}

					CalculateSize ();

					try
					{
						version = Convert.ToInt32 (info["version"].ToString ());
					}
					catch (Exception ex)
					{
						#if UNITY_STANDALONE_OSX
						Debug.LogError ("Key 'version' not found at info.json!\n" + ex.ToString ());
						#else
						if (CustomResourcesManager.amLogger != null)
							CustomResourcesManager.amLogger.LogError ("Key 'version' not found at info.json!\n" + ex.ToString ());
						#endif
					}

					try
					{
						checksum = Convert.ToUInt32 (info["checksum"].ToString ());
					}
					catch (Exception ex)
					{
						#if UNITY_STANDALONE_OSX
						Debug.LogError ("Key 'checksum' not found at info.json!\n" + ex.ToString ());
						#else
						if (CustomResourcesManager.amLogger != null)
							CustomResourcesManager.amLogger.LogError ("Key 'checksum' not found at info.json!\n" + ex.ToString ());
						#endif
					}
					break;
				} 
				else 
				{
					#if UNITY_STANDALONE_OSX
					Debug.LogError (www.error);
					#else
					if (CustomResourcesManager.amLogger != null)
						CustomResourcesManager.amLogger.LogError (www.error);
					#endif
					break;
				}
			}
			#if UNITY_STANDALONE_OSX
			Debug.Log (string.Format ("CustomResourceManager: AssetBundle size: {0} {1}, version: {2}", size, sizeUnit.ToString (), version));
			#else
			if (CustomResourcesManager.amLogger != null)
				CustomResourcesManager.amLogger.Log (string.Format ("AssetBundle size: {0} {1}, version: {2}", size, sizeUnit.ToString (), version));
			#endif
		}

		void CalculateSize ()
		{
			double tempSize = size / 1024;
			if (tempSize >= 1)
			{
				switch (sizeUnit)
				{
				case SizeUnit.B:
					sizeUnit = SizeUnit.KB;
					break;
				case SizeUnit.KB:
					sizeUnit = SizeUnit.MB;
					break;
				case SizeUnit.MB:
					sizeUnit = SizeUnit.GB;
					break;
				default:
					break;
				}
				size = Math.Round (tempSize, 2);
				if (tempSize / 1024 >= 1 && sizeUnit != SizeUnit.GB)
				{
					CalculateSize ();
				}
			}
		}

		IEnumerator DownloadAndCache ()
		{
			if (!isCached){
					Caching.ClearCache ();
			}

			while (true) 
			{
				string loadingPath = Path.Combine (outputPath, PACKAGE_NAME);
				www = WWW.LoadFromCacheOrDownload (loadingPath, version, checksum);

				if (!isCached)
				{
					StartCoroutine (UpdateLoadingProgress ());
					StartCoroutine (UpdateLoadingTimer ());
				}

				yield return www;

				if (string.IsNullOrEmpty (www.error) && www.assetBundle != null)
				{
					var includedAssets = www.assetBundle.GetAllScenePaths ();
					if (includedAssets != null && includedAssets.Length > 0)
					{
						string includedAssetsString = string.Empty;

						for (int i = 0; i < includedAssets.Length; i++)
							includedAssetsString += includedAssets [i].Substring (includedAssets [i].LastIndexOf ('/')) + "\n";

						#if UNITY_STANDALONE_OSX
						Debug.Log ("AssetBundle successfully cached: \n" + includedAssets);
						#else
						if (CustomResourcesManager.amLogger != null)
							CustomResourcesManager.amLogger.Log ("AssetBundle successfully cached: \n" + includedAssetsString);
						#endif
					}
					else
					{
						#if UNITY_STANDALONE_OSX
						Debug.LogError ("AssetBundle is empty!");
						#else
						if (CustomResourcesManager.amLogger != null) 
							CustomResourcesManager.amLogger.LogError ("AssetBundle is empty!");
						#endif
					}
					break;
				}
				else
				{
					#if UNITY_STANDALONE_OSX
					Debug.LogError (www.error);
					#else
					if (CustomResourcesManager.amLogger != null) 
						CustomResourcesManager.amLogger.LogError (www.error);
					#endif
					break;
				}
			}
		}

		IEnumerator UpdateLoadingProgress ()
		{
			while (true)
			{
				if (www != null) 
				{
					if (loadingProgress != null) 
						loadingProgress (www.progress);

					CalcLoadingSpeed (www.progress);
					CalcLoadingTime (www.progress);
				}
				yield return null;
			}
		}

		IEnumerator UpdateLoadingTimer ()
		{
			while (true) 
			{
				currentLoadingTime += 1;
				yield return new WaitForSeconds (1f);
			}
		}

		void CalcLoadingSpeed (float progress)
		{
			if (Math.Round (size * progress / (packagesCount * currentLoadingTime), 1) > 1) 
			{ 
				if (loadingSpeed != null) 
					loadingSpeed ((Math.Round (size * progress / (packagesCount * currentLoadingTime), 1)) + " MB/s");
			} 
			else 
			{
				if (loadingSpeed != null) 
					loadingSpeed ((Math.Round (1000 * size * progress / (packagesCount * currentLoadingTime), 1)) + " KB/s");
			}
		}

		void CalcLoadingTime (float progress)
		{
			double remainingSize = (1 - progress / packagesCount) * size;
			int minute = (int)Math.Round ((remainingSize / (size * progress / (packagesCount * currentLoadingTime))), 0) / 60;
			int second = (int)Math.Round ((remainingSize / (size * progress / (packagesCount * currentLoadingTime))), 0) % 60;

			if (minute < 0) minute = 0;
			
			if (second < 0) second = 0;

			if (loadingTime != null) 
				loadingTime (string.Format ("{0:D2}:{1:D2}", minute, second));
		}

		public static string GetAssetBundlePath (bool isForEditor = false)
		{
			string result = Path.Combine (Application.streamingAssetsPath, CustomCode.CustomResourcesManagerAssetBundles.ASSETBUNDLES_FOLDER_NAME);
			result = Path.Combine (result, CustomCode.CustomResourcesManagerAssetBundles.GetPlatform ());
			if (!isForEditor)
				result = StreamingAssetsPathPrefix () + result;

			return result;
		}

		static string StreamingAssetsPathPrefix ()
		{
			if (Application.isEditor)
				return "file://";
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				return string.Empty;
			case RuntimePlatform.tvOS:
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.WSAPlayerARM:
      case RuntimePlatform.WSAPlayerX64:
      case RuntimePlatform.WSAPlayerX86:
        return "file://";
			default:
				return string.Empty;
			}
		}

		public static string GetPlatform ()
		{
		#if UNITY_EDITOR
			BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
			switch (target)
			{
			case BuildTarget.Android:
				return "Android";
			case BuildTarget.tvOS:
				return "tvOS";
			case BuildTarget.iOS:
				return "iOS";
			case BuildTarget.StandaloneOSX:
				return "OSX";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "Windows";
			case BuildTarget.WSAPlayer:
				return "WSA";
			default:
				return null;
			}
		#else
			RuntimePlatform target = Application.platform;

			switch (target)
			{
			case RuntimePlatform.Android:
				return "Android";
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			case RuntimePlatform.tvOS:
				return "tvOS";
			case RuntimePlatform.OSXPlayer:
				return "OSX";
			case RuntimePlatform.WindowsPlayer:
				return "Windows";
			case RuntimePlatform.WSAPlayerARM:
			case RuntimePlatform.WSAPlayerX64:
			case RuntimePlatform.WSAPlayerX86:
				return "WSA";
			default:
				return null;
			}
		#endif
		}

		void OnDisable ()
		{
			www = null;
		}
	}
}