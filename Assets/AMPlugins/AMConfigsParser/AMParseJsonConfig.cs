#pragma warning disable
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using AMUtils;
using AMLogging;

namespace AMConfigsParser
{
	/// <summary>
	/// Класс для парсинга конфиг файлов.
	/// </summary>
	public class AMParseJsonConfig : MonoBehaviour
	{
		#if UNITY_EDITOR
		public static bool printDebug = false;
		#else
		public static bool printDebug = false;
		#endif
		static bool projectInfoError;
		static bool buildParamsError;

		static AMLogger amLogger = AMLogger.GetInstance ("AMConfigParser: ");

		static bool parsingBuildParams = false;
		static bool parsingProjectInfo = false;

		public static AMParseJsonConfig instance = null;	
		static bool isInit = false;

		static List<string> validValuesBannerPositionX = new List<string> {"left", "center", "right"};
		static List<string> validValuesBannerPositionY = new List<string> {"top", "center", "bottom"};
		static List<string> validValuesBuildType = new List<string> {"release", "test", "screenshot"};
		static List<string> validValuesBannerMethod = new List<string> {"none", "file", "check", "mapp", "xml", "del", "script"};
		static List<string> validValuesOrientation = new List<string> {"ll", "lr", "p", "pud", "ar"};

		static event System.Action Init;
		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start ()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad (gameObject);

				StartCoroutine (CoroutineOpenBuildParams ());
				StartCoroutine (CoroutineOpenProjectInfo ());
			}
			else
			{
				Destroy (gameObject);
			}
		}
		#if UNITY_EDITOR
		public static bool DevelopParse ()
		{
			OpenBuildParams ();
			OpenProjectInfo ();
			return true;
		}
		#endif
		#if UNITY_EDITOR
		/// <summary>
		/// Метод для открытия файла am_builds.
		/// </summary>
		static void OpenBuildParams ()
		{
			string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, AMBuildParamsInside.AM_BUILDS_FILE_NAME);
			string text = "";
			using (System.IO.StreamReader reader = System.IO.File.OpenText (filePath))
			{
				text = reader.ReadToEnd ();
			}
			parseBuildParams (text);
		}
		#endif
		#if UNITY_EDITOR
		/// <summary>
		/// Метод для открытия файла am_project.
		/// </summary>
		static void OpenProjectInfo ()
		{
			string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, AMProjectInfoInside.AM_PROJECT_FILE_NAME);
			string text = "";

			using (System.IO.StreamReader reader = System.IO.File.OpenText (filePath))
			{
				text = reader.ReadToEnd ();
			}
			parseProjectInfo (text);
		}
		#endif

		/// <summary>
		/// Метод корутина для открытия файла am_project.
		/// </summary>
		private IEnumerator CoroutineOpenProjectInfo ()
		{
			parsingProjectInfo = true;
			#if UNITY_WEBPLAYER && !UNITY_EDITOR
			string filePath = System.IO.Path.Combine (Application.dataPath + "/StreamingAssets", AMProjectInfoInside.AM_PROJECT_FILE_NAME);
			string text = "";
			WWW www = new WWW (filePath);
			yield return www;
			text = www.text;
			parseProjectInfo (text);
			#else
			string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, AMProjectInfoInside.AM_PROJECT_FILE_NAME);
			string text = "";
			if (filePath.Contains ("://"))
			{
				WWW www = new WWW (filePath);
				yield return www;
				text = www.text;
				parseProjectInfo (text);
			}
			else
			{
				try 
				{
			#if !UNITY_METRO || UNITY_EDITOR
					using (System.IO.StreamReader reader = System.IO.File.OpenText (filePath))
					{
						text = reader.ReadToEnd ();
					}
					parseProjectInfo (text);
			#else
					byte[] byteArray = UnityEngine.Windows.File.ReadAllBytes (filePath);
					char[] charArray = System.Text.UTF8Encoding.UTF8.GetChars (byteArray);
					text = new string (charArray);
					parseProjectInfo (text);
			#endif

				} 
				catch (Exception ex) 
				{
					amLogger.LogError ("File reading error! Message: " + ex.Message);
					projectInfoError = true;
					Application.Quit ()	;
				}
			}
			#endif
			EndInit ();
		}
		/// <summary>
		/// Метод проверки окончания парсинга.
		/// </summary>
		/// <param name="callback">Колбэк окончания инициализации.</param>
		public static void isParseEnd (System.Action callback)
		{
			if (isInit)
				callback ();
			else
				Init += callback;
		}
		/// <summary>
		/// Метод проверки окончания инициализации плагина.
		/// </summary>
		static void EndInit ()
		{
			if ((AMBuildParamsInside._loaded) && (AMProjectInfoInside._loaded))
			{
				isInit = true;
				if (Init != null)
					Init ();
			}
		}
		/// <summary>
		/// Метод для получения данных файла am_builds.
		/// </summary>
		public static AMBuildParams GetAMBuildParams ()
		{
			if (!AMBuildParamsInside._loaded)
				return null;
			else
				return new AMBuildParams ();
		}
		/// <summary>
		/// Метод для получения данных файла am_project.
		/// </summary>
		public static AMProjectInfo GetAMProjectInfo ()
		{
			if (!AMProjectInfoInside._loaded)
				return null;
			else
				return new AMProjectInfo ();
		}
		/// <summary>
		/// Метод корутина для открытия файла am_builds.
		/// </summary>
		private IEnumerator CoroutineOpenBuildParams ()
		{
			parsingBuildParams = true;
			#if UNITY_WEBPLAYER && !UNITY_EDITOR
			string filePath = System.IO.Path.Combine (Application.dataPath + "/StreamingAssets", AMBuildParamsInside.AM_BUILDS_FILE_NAME);
			string text = "";
			WWW www = new WWW (filePath);
			yield return www;
			text = www.text;
			parseBuildParams (text);
			#else
			string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, AMBuildParamsInside.AM_BUILDS_FILE_NAME);
			string text = "";
			if (filePath.Contains ("://"))
			{
				WWW www = new WWW (filePath);
				yield return www;
				text = www.text;
				parseBuildParams (text);
			}
			else
			{
				try 
				{
			#if !UNITY_METRO || UNITY_EDITOR
					using (System.IO.StreamReader reader = System.IO.File.OpenText (filePath))
					{
						text = reader.ReadToEnd ();
					}
					parseBuildParams (text);
			#else
			byte[] byteArray = UnityEngine.Windows.File.ReadAllBytes (filePath);
			char[] charArray = System.Text.UTF8Encoding.UTF8.GetChars (byteArray);
			text = new string (charArray);
			parseBuildParams (text);
			#endif

				} 
				catch (Exception ex) 
				{
					amLogger.LogError ("File reading error! Message: " + ex.Message);
					projectInfoError = true;
					Application.Quit ();
				}
			}
			#endif
			EndInit ();
		}
		/// <summary>
		/// Метод проверки валидности значений параметров конфиг файла. Сравнивает параметр со списком валидности.
		/// </summary>
		/// <param name="element">Значение параметра конфиг файла.</param>
		/// <param name="list">Список валидности для параметра конфиг файла.</param>
		static bool IsValid (string element, List<string> list)
		{
			try 
			{
				var index = list.FindIndex ((el)=>{return el == element;});
				if (index != -1)
					return true;

			} 
			catch (Exception ex) 
			{
				amLogger.Log ("Error IsValid. Message: " + ex.Message);
			}
			return false;
		}

		/// <summary>
		/// Метод для парсинга файла am_project.
		/// </summary>
		/// <param name="text">Содержимое файла am_project.</param>
		#if UNITY_EDITOR
		static void parseProjectInfo (string text) 
		{
		#else
		void parseProjectInfo (string text) 
		{
		#endif
			projectInfoError = false;

			if (printDebug) 
				amLogger.Log (AMProjectInfoInside.AM_PROJECT_FILE_NAME + " parse:");

			var amProject = AMJSON.JsonDecode (text) as Hashtable;
			if (amProject == null)
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + " field parsing failed!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}

			var projectInfo = amProject[AMProjectInfoInside.PROJECT_INFO_KEY] as Hashtable;
			if (projectInfo == null)
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + " field parsing failed!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			#region Available Languages
			var availableLanguages = projectInfo[AMProjectInfoInside.AVAILABLE_LANGUAGES_KEY] as ArrayList;
			if (availableLanguages == null)
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.AVAILABLE_LANGUAGES_KEY + " field parsing failed!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}

			AMProjectInfoInside.availableLanguages = new string[availableLanguages.Count];
			if (availableLanguages.Count > 0)
			{
				for (int i = 0; i < availableLanguages.Count; i++)
				{
					AMProjectInfoInside.availableLanguages[i] = availableLanguages[i].ToString ();
					if (printDebug) 
						amLogger.Log ("AMProjectInfo.availableLanguages["+i+"] = " + AMProjectInfoInside.availableLanguages[i]);
				}
			}
			#endregion
			#region Custom Quality Settings
			var qualitySettings = projectInfo[AMProjectInfoInside.QUALITY_SETTINGS_KEY] as ArrayList;
			if (qualitySettings != null)
			{
				AMProjectInfoInside.customQualitySettings = new AMProjectInfoInside.CustomQualitySetting[qualitySettings.Count];
				if (qualitySettings.Count > 0)
				{
					for (int i = 0; i < qualitySettings.Count; i++)
					{
						AMProjectInfoInside.customQualitySettings[i] = new AMProjectInfoInside.CustomQualitySetting ();
						Hashtable settingHashtable = qualitySettings[i] as Hashtable;

						int ram = 0;
						try 
						{
							ram = int.Parse (settingHashtable[AMProjectInfoInside.RAM_KEY].ToString ());
						} 
						catch (Exception) {}
						AMProjectInfoInside.customQualitySettings[i].ram = ram;

						int qualityLevel = 0;
						try 
						{
							qualityLevel = int.Parse (settingHashtable[AMProjectInfoInside.QUALITY_LEVEL_KEY].ToString ());
						} 
						catch (Exception) {}
						AMProjectInfoInside.customQualitySettings[i].qualityLevel = qualityLevel;

						int masterTextureLimit = 0;
						try 
						{
							masterTextureLimit = int.Parse (settingHashtable[AMProjectInfoInside.MASTER_TEXTURE_LIMIT_KEY].ToString ());
						} 
						catch (Exception) {}
						AMProjectInfoInside.customQualitySettings[i].masterTextureLimit = masterTextureLimit;
					}
				}
			}
			#endregion
			#region Banner Method
			var bannerMethod = (string)projectInfo[AMProjectInfoInside.BANNER_METHOD_KEY];
			if (bannerMethod == null)
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.BANNER_METHOD_KEY + " field parsing failed!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			if (bannerMethod == string.Empty)
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.BANNER_METHOD_KEY + " field is empty!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			if (!IsValid (bannerMethod, validValuesBannerMethod))
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.BANNER_METHOD_KEY + " field has invalid value!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}

			AMProjectInfoInside.bannerMethod = bannerMethod;
			if (printDebug) 
				amLogger.Log ("AMProjectInfo.bannerMethod = " + AMProjectInfoInside.bannerMethod);
			#endregion
			#region Banner Position
			var bannerPosition = projectInfo[AMProjectInfoInside.BANNER_POSITION_KEY] as Hashtable;
			if (bannerPosition == null)
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.BANNER_POSITION_KEY + " field parsing failed!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			///x
			var x = (string)bannerPosition["x"];
			if (x == null)
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.BANNER_POSITION_KEY + ": x field parsing failed!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			if (x == string.Empty) 
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.BANNER_POSITION_KEY + ": x field is empty!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			if (!IsValid (x, validValuesBannerPositionX))
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.BANNER_POSITION_KEY + ": x field has invalid value!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			///y
			var y = (string)bannerPosition["y"];
			if (y == null)
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.BANNER_POSITION_KEY + ": y field parsing failed!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			if (y == string.Empty) 
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.BANNER_POSITION_KEY + ": y field is empty!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			if (!IsValid (y, validValuesBannerPositionY))
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.BANNER_POSITION_KEY + ": y field has invalid value!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}

			AMProjectInfoInside.bannerPosition = new AMProjectInfoInside.banPos ();
			AMProjectInfoInside.bannerPosition.x = x;
			AMProjectInfoInside.bannerPosition.y = y;
			#endregion
			#region Orientation
			var orientation = (string)projectInfo[AMProjectInfoInside.ORIENTATION_KEY];
			if (orientation == null)
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.ORIENTATION_KEY + " field parsing failed!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			if (orientation == string.Empty)
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.ORIENTATION_KEY + " field is empty!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			if (!IsValid (orientation, validValuesOrientation))
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.ORIENTATION_KEY + " field has invalid value!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}

			AMProjectInfoInside.orientation = orientation;
			if (printDebug) 
				amLogger.Log ("AMProjectInfo.orientation = " + AMProjectInfoInside.orientation);
			#endregion
			#region Resolutions
			var resolutions = projectInfo[AMProjectInfoInside.RESOLUTION_KEY] as ArrayList;
			if (resolutions == null)
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + ": " + 
					AMProjectInfoInside.PROJECT_INFO_KEY + ": " + 
					AMProjectInfoInside.RESOLUTION_KEY + " field parsing failed!");
				projectInfoError = true;
				Application.Quit ();
				return;
			}
			AMProjectInfoInside.resolutions = new string[resolutions.Count];
			if (resolutions.Count > 0)
			{
				for (int i = 0; i < resolutions.Count; i++)
				{
					AMProjectInfoInside.resolutions[i] = resolutions[i].ToString ();
					if (printDebug) 
						amLogger.Log ("AMProjectInfo.resolutions["+i+"] = " + AMProjectInfoInside.resolutions[i]);
				}
			}
			#endregion
			#region Disable Auto Interstitil
			bool disableAutoInterstitial = false;
			try
			{
				disableAutoInterstitial = (bool)amProject[AMProjectInfoInside.DISABLE_AUTO_INTERSTITIAL_KEY];
			}
			catch (Exception)
			{}

			AMProjectInfoInside.disableAutoInterstitial = disableAutoInterstitial;
			#endregion
			#region Cross Ad
			bool crossAd = false;
			try
			{
				crossAd = (bool)amProject[AMProjectInfoInside.CROSS_AD_KEY];
			}
			catch (Exception)
			{}
			AMProjectInfoInside.crossAd = crossAd;
			#endregion

			#region APP_ID
			int app_id = 0;
			try
			{
				app_id = int.Parse (amProject[AMProjectInfoInside.APP_ID].ToString ());
			}
			catch (Exception)
			{}
			AMProjectInfoInside.app_id = app_id;
            #endregion

            #region BALANCER_APP_ID
 			int balancer_app_id = 0;
 			try
			{
				balancer_app_id = int.Parse (amProject[AMProjectInfoInside.BALANCER_APP_ID].ToString ());
			}
			catch (Exception)
			{}
			AMProjectInfoInside.balancer_app_id = balancer_app_id;
            #endregion

			#region OFFERS_APP_ID
			int offers_app_id = 0;
			try
			{
				offers_app_id = int.Parse (amProject[AMProjectInfoInside.OFFERS_APP_ID].ToString ());
			}
			catch (Exception)
			{}
			AMProjectInfoInside.offers_app_id = offers_app_id;
            #endregion

			#region Advertising At The Start
			bool advertisingAtTheStart = true;
			try
			{
				advertisingAtTheStart = (bool)amProject[AMProjectInfoInside.AD_AT_THE_START_KEY];
			}
			catch (Exception)
			{}
			AMProjectInfoInside.advertisingAtTheStart = advertisingAtTheStart;
			#endregion
			#region Start Screen Opacity
			float startScreenOpacity = 0.95f;
			try
			{
				startScreenOpacity = float.Parse (amProject[AMProjectInfoInside.START_SCREEN_OPACITY_KEY].ToString ());
			}
			catch (Exception)
			{}
			AMProjectInfoInside.startScreenOpacity = startScreenOpacity;
            #endregion
            #region Debug Options
            try
            {
				AMProjectInfoInside.enableAMLoggerConsole = (string)amProject[AMProjectInfoInside.ENABLE_AMLOGGER_CONSOLE_KEY];
            }
            catch (Exception)
            {
				AMProjectInfoInside.enableAMLoggerConsole = null;
			}

			try
			{
				AMProjectInfoInside.customCodeDebug = ((bool)amProject[AMProjectInfoInside.CUSTOM_CODE_DEBUG_KEY]).ToString ();
			}
			catch (Exception)
			{
				AMProjectInfoInside.customCodeDebug = string.Empty;
			}

			var debugModeForPlugins = amProject[AMProjectInfoInside.DEBUG_MODE_FOR_PLUGINS_KEY] as ArrayList;
            if (debugModeForPlugins == null)
            {
                debugModeForPlugins = new ArrayList ();
            }

            AMProjectInfoInside.debugModeForPlugins = new string[debugModeForPlugins.Count];
            if (debugModeForPlugins.Count > 0)
            {
                for (int i = 0; i < debugModeForPlugins.Count; i++)
                {
                    AMProjectInfoInside.debugModeForPlugins[i] = debugModeForPlugins[i].ToString ();
                }
            }
            #endregion
			#region Interstitial Delay
			try
			{
				AMProjectInfoInside.interstitialDelay = int.Parse (amProject[AMProjectInfoInside.INTERSTITIAL_DELAY_KEY].ToString ());
			}
			catch (Exception)
			{
				AMProjectInfoInside.interstitialDelay = 60;
			}
			#endregion
			#region Auto Interstitial Scenes
			var autoInterstitialScenes = amProject[AMProjectInfoInside.AUTO_INTERSTITIAL_SCENES_KEY] as ArrayList;
			if (autoInterstitialScenes != null)
			{
				AMProjectInfoInside.autoInterstitialScenes = new List<string> ();
				if (autoInterstitialScenes.Count > 0)
				{
					for (int i = 0; i < autoInterstitialScenes.Count; i++)
					{
						AMProjectInfoInside.autoInterstitialScenes.Add (autoInterstitialScenes[i].ToString ());
					}
				}
			}
			#endregion
			#region Event Console
			bool eventConsole = false;
			try
			{
				eventConsole = (bool)amProject[AMProjectInfoInside.EVENT_CONSOLE];
			}
			catch (Exception)
			{}
			AMProjectInfoInside.eventConsole = eventConsole;
			#endregion

            if (!projectInfoError) 
			{
				if (printDebug) 
					amLogger.Log (AMProjectInfoInside.AM_PROJECT_FILE_NAME + " was parsed successfully!");
				AMProjectInfoInside._loaded = true;
			}
			else 
			{
				amLogger.LogError (AMProjectInfoInside.AM_PROJECT_FILE_NAME + " parsing was finished with error (s)!");
				AMProjectInfoInside._loaded = false;
			}
		}

		/// <summary>
		/// Метод для парсинга файла am_builds.
		/// </summary>
		/// <param name="text">Содержимое файла am_builds.</param>
		#if UNITY_EDITOR
		static void parseBuildParams (string text) 
		{
		#else
		void parseBuildParams (string text) 
		{
		#endif
			buildParamsError = false;

			if (printDebug) 
				amLogger.Log (AMBuildParamsInside.AM_BUILDS_FILE_NAME + " parse:");

			var amBuilds = AMJSON.JsonDecode (text) as Hashtable;
			if (amBuilds == null)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + " parsing failed!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}

			var buildParamsArray = amBuilds[AMBuildParamsInside.BUILD_PARAMS_KEY] as ArrayList;
			if (buildParamsArray == null)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + " field parsing failed!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}
			var buildParams = buildParamsArray[0] as Hashtable;
			#region Bundle
			var bundle = (string)buildParams[AMBuildParamsInside.BUNDLE_KEY];
			if (bundle == null)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.BUNDLE_KEY + " field parsing failed!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}
			if (bundle == string.Empty)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.BUNDLE_KEY + " field is empty!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}

			AMBuildParamsInside.bundle = bundle;
			if (printDebug) 
				amLogger.Log ("AMBuildParams.bundle = " + AMBuildParamsInside.bundle);
			#endregion
			#region Inner ID
			var innerID = (string)buildParams[AMBuildParamsInside.INNER_ID_KEY];
			if (innerID == null)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.INNER_ID_KEY + " field parsing failed!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}
			if (innerID == string.Empty)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.INNER_ID_KEY + " field is empty!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}

			AMBuildParamsInside.innerID = innerID;
			if (printDebug) 
				amLogger.Log ("AMBuildParams.innerID = " + AMBuildParamsInside.innerID);
			#endregion
			#region Platform
			var platform = (string)buildParams[AMBuildParamsInside.PLATFORM_KEY];
			if (platform == null)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.PLATFORM_KEY + " field parsing failed!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}
			if (platform == string.Empty)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.PLATFORM_KEY + " field is empty!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}

			AMBuildParamsInside.platform = platform;
			if (printDebug) 
				amLogger.Log ("AMBuildParams.platform = " + AMBuildParamsInside.platform);
			#endregion
			#region Language
			var language = buildParams[AMBuildParamsInside.LANGUAGE_KEY] as ArrayList;
			if (language == null)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.LANGUAGE_KEY + " field parsing failed!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}

			AMBuildParamsInside.language = new string[language.Count];
			if (language.Count > 0)
			{
				for (int i = 0; i < language.Count; i++)
				{
					AMBuildParamsInside.language[i] = language[i].ToString ();
					if (printDebug) 
						amLogger.Log ("AMBuildParams.language["+i+"] = " + AMBuildParamsInside.language[i]);
				}
			}
			#endregion
			#region Payment
			var payment = (string)buildParams[AMBuildParamsInside.PAYMENT_KEY];
			if (payment == null)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.PAYMENT_KEY + " field parsing failed!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}
			if (payment == string.Empty)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.PAYMENT_KEY + " field is empty!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}

			AMBuildParamsInside.payment = payment;
			if (printDebug) 
				amLogger.Log ("AMBuildParams.payment = " + AMBuildParamsInside.payment);
			#endregion
			#region Unlock
			var unlock = (bool)buildParams[AMBuildParamsInside.UNLOCK_KEY];

			AMBuildParamsInside.unlock = unlock;
			if (printDebug) 
				amLogger.Log ("AMBuildParams.unlock = " + AMBuildParamsInside.unlock);
			#endregion
			#region Has Banner
			var hasBanner = (bool)buildParams[AMBuildParamsInside.HAS_BANNER_KEY];

			AMBuildParamsInside.hasBanner = hasBanner;
			if (printDebug) 
				amLogger.Log ("AMBuildParams.hasBanner = " + AMBuildParamsInside.hasBanner);
			#endregion
			#region Build Type
			var buildType = (string)buildParams[AMBuildParamsInside.BUILD_TYPE_KEY];
			if (buildType == null)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.BUILD_TYPE_KEY + " field parsing failed!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}
			if (buildType == string.Empty)
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.BUILD_TYPE_KEY + " field is empty!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}
			if (!IsValid(buildType, validValuesBuildType))
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + ": " + 
					AMBuildParamsInside.BUILD_PARAMS_KEY + ": " +
					AMBuildParamsInside.BUILD_TYPE_KEY + " field has invalid value!");
				buildParamsError = true;
				Application.Quit ();
				return;
			}

			AMBuildParamsInside.build_type = buildType;
			if (printDebug) 
				amLogger.Log ("AMBuildParams.buildType = " + AMBuildParamsInside.build_type);
			#endregion
			if (!buildParamsError) 
			{
				if (printDebug) 
					amLogger.Log (AMBuildParamsInside.AM_BUILDS_FILE_NAME + " was parsed successfully!");
				AMBuildParamsInside._loaded = true;
			}
			else 
			{
				amLogger.LogError (AMBuildParamsInside.AM_BUILDS_FILE_NAME + " parsing was finished with error (s)!");
				AMBuildParamsInside._loaded = false;
			}
		}
	}
}