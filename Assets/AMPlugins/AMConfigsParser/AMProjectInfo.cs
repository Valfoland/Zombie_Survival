using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AMConfigsParser
{
	public class AMProjectInfo 
	{
		public struct banPos
		{
			public string x;
			public string y;
		}
		public struct CustomQualitySetting
		{
			public int ram;
			public int qualityLevel;
			public int masterTextureLimit;
		}

		public string[] availableLanguages;
		public string[] resolutions;
		public string bannerMethod;
		public banPos bannerPosition;
		public string orientation;
		public CustomQualitySetting[] customQualitySettings;
		public bool disableAutoInterstitial;
		public bool crossAd;
		public bool advertisingAtTheStart;
		public float startScreenOpacity;
        public string enableAMLoggerConsole;
		public string customCodeDebug;
        public string[] debugModeForPlugins;
		public int interstitialDelay;
		public List<string> autoInterstitialScenes;
		public bool eventConsole;

		public bool _loaded;

        public AMProjectInfo ()
		{
			if ((AMProjectInfoInside.availableLanguages != null) && (AMProjectInfoInside.availableLanguages.Length != 0))
			{
				availableLanguages = new string[AMProjectInfoInside.availableLanguages.Length];
				AMProjectInfoInside.availableLanguages.CopyTo (availableLanguages, 0);
			}
			else
			{
				availableLanguages = new string[0];
			}

			if ((AMProjectInfoInside.resolutions != null) && (AMProjectInfoInside.resolutions.Length != 0))
			{
				resolutions = new string[AMProjectInfoInside.resolutions.Length];
				AMProjectInfoInside.resolutions.CopyTo (resolutions, 0);
			}
			else
			{
				resolutions = new string[0];
			}

			bannerMethod = AMProjectInfoInside.bannerMethod;
			bannerPosition = new banPos ();
			bannerPosition.x = AMProjectInfoInside.bannerPosition.x;
			bannerPosition.y = AMProjectInfoInside.bannerPosition.y;

			orientation = AMProjectInfoInside.orientation;

			_loaded = AMBuildParamsInside._loaded;

			if ((AMProjectInfoInside.customQualitySettings != null) && (AMProjectInfoInside.customQualitySettings.Length != 0))
			{
				customQualitySettings = new CustomQualitySetting[AMProjectInfoInside.customQualitySettings.Length];
				for (int i = 0; i < AMProjectInfoInside.customQualitySettings.Length; i++) 
				{
					customQualitySettings[i] = new CustomQualitySetting ();
					customQualitySettings[i].ram = AMProjectInfoInside.customQualitySettings[i].ram;
					customQualitySettings[i].qualityLevel = AMProjectInfoInside.customQualitySettings[i].qualityLevel;
					customQualitySettings[i].masterTextureLimit = AMProjectInfoInside.customQualitySettings[i].masterTextureLimit;
				}
			}
			else
			{
				customQualitySettings = new CustomQualitySetting[0];
			}

			disableAutoInterstitial = AMProjectInfoInside.disableAutoInterstitial;
			crossAd = AMProjectInfoInside.crossAd;
			advertisingAtTheStart = AMProjectInfoInside.advertisingAtTheStart;
			startScreenOpacity = AMProjectInfoInside.startScreenOpacity;

            enableAMLoggerConsole = AMProjectInfoInside.enableAMLoggerConsole;

			customCodeDebug = AMProjectInfoInside.customCodeDebug;

            if (AMProjectInfoInside.debugModeForPlugins != null && AMProjectInfoInside.debugModeForPlugins.Length != 0)
            {
                debugModeForPlugins = new string[AMProjectInfoInside.debugModeForPlugins.Length];
                for (int i = 0; i < AMProjectInfoInside.debugModeForPlugins.Length; i++)
                {
                    debugModeForPlugins[i] = AMProjectInfoInside.debugModeForPlugins[i];
                }
            }
            else
            {
                debugModeForPlugins = new string[0];
            }

			interstitialDelay = AMProjectInfoInside.interstitialDelay;

			autoInterstitialScenes = AMProjectInfoInside.autoInterstitialScenes;

			eventConsole = AMProjectInfoInside.eventConsole;
    	}
	}

	static class AMProjectInfoInside 
	{
		public const string AM_PROJECT_FILE_NAME = "am_project.txt";
		public const string PROJECT_INFO_KEY = "project_info";
		public const string BANNER_METHOD_KEY = "banner_method";
		public const string QUALITY_SETTINGS_KEY = "quality_settings";
		public const string MASTER_TEXTURE_LIMIT_KEY = "master_texture_limit";
		public const string QUALITY_LEVEL_KEY = "quality_level";
		public const string RAM_KEY = "ram";
		public const string LOCALIZATION_PATH_KEY = "localization_path";
		public const string BANNER_POSITION_KEY = "banner_position";
		public const string AVAILABLE_LANGUAGES_KEY = "available_languages";
		public const string ORIENTATION_KEY = "orientation";
		public const string RESOLUTION_KEY = "resolutions";
		public const string DISABLE_AUTO_INTERSTITIAL_KEY = "disable_auto_interstitial";
		public const string CROSS_AD_KEY = "cross_ad";
		public const string AD_AT_THE_START_KEY = "advertising_at_the_start";
		public const string START_SCREEN_OPACITY_KEY = "start_screen_opacity";
		public const string ENABLE_AMLOGGER_CONSOLE_KEY = "enable_amlogger_console";
		public const string CUSTOM_CODE_DEBUG_KEY = "custom_code_debug";
		public const string DEBUG_MODE_FOR_PLUGINS_KEY = "debug_mode_for_plugins";
		public const string INTERSTITIAL_DELAY_KEY = "interstitial_delay";
		public const string AUTO_INTERSTITIAL_SCENES_KEY = "auto_interstitial_scenes";
		public const string EVENT_CONSOLE = "event-console";
		public const string APP_ID = "app_id";
		public const string BALANCER_APP_ID = "balancer_app_id";
		public const string OFFERS_APP_ID = "offers_app_id";

		public struct banPos
		{
			public string x;
			public string y;
		}
		public struct CustomQualitySetting
		{
			public int ram;
			public int qualityLevel;
			public int masterTextureLimit;
		}

	    public static string[] availableLanguages;
	    public static string[] resolutions;
	    public static string bannerMethod;
	    public static banPos bannerPosition;
	    public static string orientation;
		public static CustomQualitySetting[] customQualitySettings;
		public static bool disableAutoInterstitial;
		public static bool crossAd;
		public static bool advertisingAtTheStart;
		public static float startScreenOpacity;
        public static string enableAMLoggerConsole;
		public static string customCodeDebug;
        public static string[] debugModeForPlugins;
		public static int interstitialDelay;
		public static List<string> autoInterstitialScenes;
		public static bool eventConsole;
		public static int app_id;
		public static int balancer_app_id;
		public static int offers_app_id;
		public static bool _loaded;
    }
}