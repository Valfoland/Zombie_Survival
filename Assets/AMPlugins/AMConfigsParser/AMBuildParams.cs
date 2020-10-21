using UnityEngine;
using System.Collections;

namespace AMConfigsParser
{
	public class AMBuildParams {
		
		public string bundle;
		public string innerID;
		public string platform;
		public string[] language;
		public string payment;
		public bool unlock;
		public bool hasBanner;
		public string build_type;
		
		public bool _loaded;

		public AMBuildParams ()
		{
			bundle = AMBuildParamsInside.bundle;
			innerID = AMBuildParamsInside.innerID;
			platform = AMBuildParamsInside.platform;
			if ((AMBuildParamsInside.language != null) && (AMBuildParamsInside.language.Length != 0))
			{
				language = new string[AMBuildParamsInside.language.Length];
				AMBuildParamsInside.language.CopyTo (language, 0);
			}
			else
			{
				language = new string[]{ };
			}
			payment = AMBuildParamsInside.payment;
			unlock = AMBuildParamsInside.unlock;
			hasBanner = AMBuildParamsInside.hasBanner;
			build_type = AMBuildParamsInside.build_type;

			_loaded = AMBuildParamsInside._loaded;
		}
	}

	public class AMBuildParamsInside 
	{
		public const string AM_BUILDS_FILE_NAME = "am_builds.txt";
		public const string BUILD_PARAMS_KEY = "build_params";
		public const string BUNDLE_KEY = "bundle";
		public const string INNER_ID_KEY = "innerID";
		public const string PLATFORM_KEY = "platform";
		public const string LANGUAGE_KEY = "language";
		public const string PAYMENT_KEY = "payment";
		public const string UNLOCK_KEY = "unlock";
		public const string HAS_BANNER_KEY = "has_banner";
		public const string BUILD_TYPE_KEY = "build_type";

	    public static string bundle;
	    public static string innerID;
	    public static string platform;
	    public static string[] language;
	    public static string payment;
	    public static bool unlock;
	    public static bool hasBanner;
		public static string build_type;

	    public static bool _loaded;
	}
}