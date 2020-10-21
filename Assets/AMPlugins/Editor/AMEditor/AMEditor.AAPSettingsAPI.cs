#if UNITY_EDITOR
#pragma warning disable
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace AMEditor
{
	public class AAPSettings
	{
		public static bool MuteMusic ()
		{
			string currentScenePath = string.Empty;
			currentScenePath = EditorSceneManager.GetActiveScene ().path;
			if (EditorBuildSettings.scenes.Length > 0 && EditorBuildSettings.scenes [0].path != string.Empty)
			{
				var ebsMenuScene = EditorBuildSettings.scenes.ToList ().Find ((sc) => {
					return sc.enabled && sc.path.Contains ("Menu");
				});
				if (ebsMenuScene == null)
				{
					Debug.Log ("ERROR: \"Menu\" scene not found in the Build Settings!");
					return false;
				}
				var menuScene = EditorSceneManager.OpenScene (ebsMenuScene.path);
				var musicGO = (GameObject.FindObjectsOfType (typeof(GameObject)).ToList ()).Find ((go) => {return go.name == "Music";}) as GameObject;
				if (musicGO == null)
				{
					Debug.Log ("ERROR: \"Music\" game object not found on the Menu scene!");
					return false;
				}
				var audioSourceComponent = (AudioSource)musicGO.GetComponent (typeof(AudioSource));

				if (audioSourceComponent == null)
				{
					Debug.Log ("ERROR: \"Audio Source\" component not found on the Menu game object!");
					return false;
				}
				audioSourceComponent.volume = 0;
				EditorSceneManager.SaveScene (menuScene);
				if (currentScenePath == string.Empty)
				{
					EditorSceneManager.OpenScene (currentScenePath);
				}
				return true;
			}
			else
			{
				Debug.Log ("ERROR: Incorrect scenes list in Build Settings!");
				return false;
			}
		}

		public static bool MakeTexturesTruecolor ()
		{
			bool success = false;

			List<string> foundedTexturesMeta = new List<string> ();

			var foundedTexturesGUIDs = AssetDatabase.FindAssets ("t:Texture").ToList ();

			foreach (var guid in foundedTexturesGUIDs)
			{
				foundedTexturesMeta.Add (AssetDatabase.GUIDToAssetPath (guid)+".meta");
			}
			success = foundedTexturesMeta.TrueForAll ((metafile) => { return TexturesMetaTruecolor (metafile); });

			AssetDatabase.Refresh ();

			return success;
		}

		static bool TexturesMetaTruecolor (string path)
		{
			bool success = false;

			success = ReplaceFormat (path);
			if (!success)
			{
				EditorSettings.externalVersionControl = "Visible Meta Files";
				success = ReplaceFormat (path);
			}

			return success;
		}

		static bool ReplaceFormat (string path)
		{
			if (File.Exists (path))
			{
				var currentMeta = File.ReadAllText (path);

				string oldTextureFormat = currentMeta.Substring (currentMeta.IndexOf (metaKeyword), metaKeyword.Length + 1);

				string newMeta = currentMeta.Replace (oldTextureFormat, correctTextureFormat);

				File.WriteAllText (path, newMeta);

				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool FixLocalization ()
		{
			#region CSV
			List<string> csvPathes = new List<string> {
				"Assets/Resources/xml/localization.csv", 
				"Assets/Resources/xml/localisation.csv", 
				"Assets/Resources/localization.csv", 
				"Assets/Resources/localisation.csv"};

			var csvPathIndex = csvPathes.FindIndex ((path) => { return File.Exists (path); });

			if (csvPathIndex == -1)
			{
				Debug.Log ("ERROR: localization.csv not found!");
				return false;
			}
			var actualCSVPath = csvPathes [csvPathIndex];

			var csvContent = File.ReadAllText (actualCSVPath);
			if (string.IsNullOrEmpty (csvContent))
			{
				Debug.Log ("ERROR: localization.csv reading failed!");
				return false;
			}

			csvContent = csvContent.Replace ("PO", "PT");
			csvContent = csvContent.Replace ("JP", "JA");
			csvContent = csvContent.Replace ("CN", "ZH");
			csvContent = csvContent.Replace ("KR", "KO");

			if (File.Exists (actualCSVPath))
				File.Delete (actualCSVPath);
			File.WriteAllText (actualCSVPath, csvContent);
			#endregion

			#region Script
			List<string> scriptPathes = new List<string> {
				"Assets/Scripts/Localization.cs", 
				"Assets/Scripts/Localisation.cs",
				"Assets/Scripts/Localization/Localization.cs",
				"Assets/Scripts/Localization/Localisation.cs",
				"Assets/Scripts/Localisation/Localisation.cs",
				"Assets/Scripts/Localisation/Localization.cs"};

			var scriptPathIndex = scriptPathes.FindIndex ((path) => { return File.Exists (path); });

			if (scriptPathIndex == -1)
			{
				Debug.Log ("ERROR: Localization.cs not found!");
				return false;
			}
			var actualScriptPath = scriptPathes [scriptPathIndex];

			var scriptContent = File.ReadAllText (actualScriptPath);
			if (string.IsNullOrEmpty (scriptContent))
			{
				Debug.Log ("ERROR: Localization.cs reading failed!");
				return false;
			}

			scriptContent = scriptContent.Replace (@"public const int PO = 5;", @"public const int PT = 5;");
			scriptContent = scriptContent.Replace (@"public const int JP = 6;", @"public const int JA = 6;");
			scriptContent = scriptContent.Replace (@"public const int CN = 7;", @"public const int ZH = 7;");
			scriptContent = scriptContent.Replace (@"public const int KR = 10;", @"public const int KO = 10;");
			scriptContent = scriptContent.Replace (@"Localization.instance.lang = Localization.CN;", @"Localization.instance.lang = Localization.ZH;");
			scriptContent = scriptContent.Replace (@"Localization.instance.lang = Localization.JP;", @"Localization.instance.lang = Localization.JA;");
			scriptContent = scriptContent.Replace (@"Localization.instance.lang = Localization.KR;", @"Localization.instance.lang = Localization.KO;");
			scriptContent = scriptContent.Replace (@"Localization.instance.lang = Localization.BR;", @"Localization.instance.lang = Localization.PT;");
			scriptContent = scriptContent.Replace (@"po", @"pt");
			scriptContent = scriptContent.Replace (@"jp", @"ja");
			scriptContent = scriptContent.Replace (@"cn", @"zh");
			scriptContent = scriptContent.Replace (@"kr", @"ko");
			scriptContent = scriptContent.Replace (@"case PO: return ""PO"";", @"case PT: return ""PT"";");
			scriptContent = scriptContent.Replace (@"case JP: return ""JP"";", @"case JA: return ""JA"";");
			scriptContent = scriptContent.Replace (@"case CN: return ""CN"";", @"case ZH: return ""ZH"";");
			scriptContent = scriptContent.Replace (@"case KR: return ""KR"";", @"case KO: return ""KO"";");

			if (File.Exists (actualScriptPath))
				File.Delete (actualScriptPath);
			File.WriteAllText (actualScriptPath, scriptContent);

			#endregion
			return true;
		}

		public static string CheckProjectConfigs ()
		{
			string amBuildsPath = Path.Combine (Application.streamingAssetsPath, "am_builds.txt");
			string amProjectPath = Path.Combine (Application.streamingAssetsPath, "am_project.txt");

			#region am_builds
			if (!File.Exists (amBuildsPath))
				return "ERROR: am_builds.txt not found!";
			
			Hashtable am_builds = new Hashtable ();
			try
			{
				am_builds = AMEditorJSON.JsonDecode (File.ReadAllText (amBuildsPath)) as Hashtable;
				if (am_builds == null)
					return "ERROR: am_builds.txt reading failed!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_builds.txt reading failed with exception: " + ex.ToString ();
			}

			ArrayList build_params_arraylist = new ArrayList ();
			try
			{
				build_params_arraylist = am_builds ["build_params"] as ArrayList;
				if (build_params_arraylist == null || build_params_arraylist.Count == 0 || build_params_arraylist[0] == null)
					return "ERROR: am_builds: build_params is invalid!";
			}
			catch (Exception ex)
			{
				if (build_params_arraylist == null || build_params_arraylist.Count == 0 || build_params_arraylist[0] == null)
					return "ERROR: am_builds: build_params is invalid!\nException: " + ex.ToString ();
			}
				
			Hashtable build_params = new Hashtable ();
			try
			{
				build_params = build_params_arraylist[0] as Hashtable;
				if (build_params == null)
					return "ERROR: am_builds: build_params item is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_builds: build_params item is invalid!\nException: " + ex.ToString ();
			}
				
			string bundle = string.Empty;
			try
			{
				bundle = (string)build_params ["bundle"];
				if (string.IsNullOrEmpty (bundle))
					return "ERROR: am_builds: bundle is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_builds: bundle is invalid!\nException: " + ex.ToString ();
			}
			
			string innerID = string.Empty;
			try
			{
				innerID = (string)build_params ["innerID"];
				if (string.IsNullOrEmpty (innerID))
					return "ERROR: am_builds: innerID is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_builds: innerID is invalid!\nException: " + ex.ToString ();
			}
			
			string platform = string.Empty;
			try
			{
				platform = (string)build_params ["platform"];
				if (string.IsNullOrEmpty (platform) || !IsValidParam ("platform", platform))
					return "ERROR: am_builds: platform is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_builds: platform is invalid!\nException: " + ex.ToString ();
			}
			
			ArrayList language = new ArrayList ();
			try
			{
				language = build_params ["language"] as ArrayList;
				if (language == null)
					return "ERROR: am_builds: language is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_builds: language is invalid!\nException: " + ex.ToString ();
			}
			
			if (language.Count > 0)
			{
				string langaugeCheckResult = string.Empty;
				foreach (var item in language)
				{
					if (!IsValidParam ("language", item))
					{
						langaugeCheckResult = "ERROR: am_builds: Incorrect language item!";
						break;
					}
				}
				if (langaugeCheckResult != string.Empty)
					return langaugeCheckResult;
			}
			
			string payment = string.Empty;
			try
			{
				payment = (string)build_params ["payment"];
				if (string.IsNullOrEmpty (payment) || !IsValidParam ("payment", payment))
					return "ERROR: am_builds: payment is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_builds: payment is invalid!\nException: " + ex.ToString ();
			}
			
			bool unlock = false;
			try
			{
				unlock = (bool)build_params ["unlock"];
			}
			catch (Exception ex)
			{
				return "ERROR: am_builds: unlock is invalid!\nException: " + ex.ToString ();
			}

			bool has_banner = false;
			try
			{
				has_banner = (bool)build_params ["has_banner"];
			}
			catch (Exception ex)
			{
				return "ERROR: am_builds: has_banner is invalid!\nException: " + ex.ToString ();
			}
			
			string build_type = string.Empty;
			try
			{
				build_type = (string)build_params ["build_type"];
				if (string.IsNullOrEmpty (build_type) || !IsValidParam ("build_type", build_type))
					return "ERROR: am_builds: build_type is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_builds: build_type is invalid!\nException: " + ex.ToString ();
			}
			#endregion

			#region am_project
			if (!File.Exists (amProjectPath))
				return "ERROR: am_project.txt not found!";

			Hashtable am_project = new Hashtable ();
			try
			{
				am_project = AMEditorJSON.JsonDecode (File.ReadAllText (amProjectPath)) as Hashtable;
				if (am_project == null)
					return "ERROR: am_project.txt reading failed!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_project.txt reading failed with exception: " + ex.ToString ();
			}

			Hashtable project_info = new Hashtable ();
			try
			{
				project_info = am_project ["project_info"] as Hashtable;
				if (project_info == null)
					return "ERROR: am_project: project_info is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_project: project_info is invalid!\nException: " + ex.ToString ();
			}

			ArrayList available_languages = new ArrayList ();
			try
			{
				available_languages = project_info ["available_languages"] as ArrayList;
				if (available_languages == null)
					return "ERROR: am_project: available_languages is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_project: available_languages is invalid!\nException: " + ex.ToString ();
			}

			if (available_languages.Count > 0)
			{
				string langaugeCheckResult = string.Empty;
				foreach (var item in available_languages)
				{
					if (!IsValidParam ("language", item))
					{
						langaugeCheckResult = "ERROR: Incorrect available_languages vlaue!";
						break;
					}
				}
				if (langaugeCheckResult != string.Empty)
					return langaugeCheckResult;
			}

			Hashtable banner_position = new Hashtable ();
			try
			{
				banner_position = project_info ["banner_position"] as Hashtable;
				if (banner_position == null)
					return "ERROR: am_project: banner_position is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_project: banner_position is invalid!\nException: " + ex.ToString ();
			}

			string banner_position_x = string.Empty;
			try
			{
				banner_position_x = (string)banner_position ["x"];
				if (string.IsNullOrEmpty (banner_position_x) || !IsValidParam ("banner_position_x", banner_position_x))
					return "ERROR: am_project: banner_postition : x is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_project: banner_position: x is invalid!\nException: " + ex.ToString ();
			}

			string banner_position_y = string.Empty;
			try
			{
				banner_position_y = (string)banner_position ["y"];
				if (string.IsNullOrEmpty (banner_position_y) || !IsValidParam ("banner_position_y", banner_position_y))
					return "ERROR: am_project: banner_postition : y is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_project: banner_position: y is invalid!\nException: " + ex.ToString ();
			}

			string orientation = string.Empty;
			try
			{
				orientation = (string)project_info ["orientation"];
				if (string.IsNullOrEmpty (orientation) || !IsValidParam ("orientation", orientation))
					return "ERROR: am_project: orientation is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_project: orientation is invalid!\nException: " + ex.ToString ();
			}

			string banner_method = string.Empty;
			try
			{
				banner_method = (string)project_info ["banner_method"];
				if (string.IsNullOrEmpty (banner_method) || !IsValidParam ("banner_method", banner_method))
					return "ERROR: am_project: banner_method is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_project: banner_method is invalid!\nException: " + ex.ToString ();
			}

			ArrayList resolutions = new ArrayList ();
			try
			{
				resolutions = project_info ["resolutions"] as ArrayList;
				if (resolutions == null)
					return "ERROR: am_project: resolutions is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_project: resolutions is invalid!\nException: " + ex.ToString ();
			}

			ArrayList quality_settings = new ArrayList ();
			try
			{
				quality_settings = project_info ["quality_settings"] as ArrayList;
				if (resolutions == null)
					return "ERROR: am_project: quality_settings is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_project: quality_settings is invalid!\nException: " + ex.ToString ();
			}

			if (quality_settings.Count > 0)
			{
				string qualitySettingsCheckResult = string.Empty;
				foreach (var item in quality_settings)
				{
					var qualitySettings = item as Hashtable;
					try
					{
						var ram = int.Parse (qualitySettings["ram"].ToString ());
						if (ram == -1)
						{
							qualitySettingsCheckResult = "ERROR: am_project: quality_settings: ram is invalid!";
							break;
						}
					}
					catch (Exception ex)
					{
						qualitySettingsCheckResult = "ERROR: am_project: quality_settings: ram is invalid!\nException: " + ex.ToString ();
						break;
					}
					try
					{
						var quality_level = int.Parse (qualitySettings["quality_level"].ToString ());
						if (quality_level == -1)
						{
							qualitySettingsCheckResult = "ERROR: am_project: quality_settings: quality_level is invalid!";
							break;
						}
					}
					catch (Exception ex)
					{
						qualitySettingsCheckResult = "ERROR: am_project: quality_settings: quality_level is invalid!\nException: " + ex.ToString ();
						break;
					}
					try
					{
						var master_texture_limit = int.Parse (qualitySettings["master_texture_limit"].ToString ());
						if (master_texture_limit == -1)
						{
							qualitySettingsCheckResult = "ERROR: am_project: quality_settings: master_texture_limit is invalid!";
							break;
						}
					}
					catch (Exception ex)
					{
						qualitySettingsCheckResult = "ERROR: am_project: quality_settings: master_texture_limit is invalid!\nException: " + ex.ToString ();
						break;
					}
				}
				if (qualitySettingsCheckResult != string.Empty)
					return qualitySettingsCheckResult;
			}

			ArrayList localization_path = new ArrayList ();
			try
			{
				localization_path = project_info ["localization_path"] as ArrayList;
				if (localization_path == null)
					return "ERROR: am_project: localization_path is invalid!";
			}
			catch (Exception ex)
			{
				return "ERROR: am_project: localization_path is invalid!\nException: " + ex.ToString ();
			}
			#endregion

			return "success";
		}
			
		static Dictionary <string, List<string>> validation = new Dictionary<string, List<string>> ()
		{
			{"platform", new List<string>(){"amazonbook","android","asha","blackberry","ibook","ios","blackberry","mac","nookbook","pc","web","windowsstore","windowsphone"}},
			{"payment", new List<string>(){"pro","free"}},
			{"build_type", new List<string>(){"release","test","screenshot"}},
			{"language", new List<string>(){"EN","DE","IT","ES","PT","PL","FR","ZH","JA","RU","KO","TR","HI","DA","NB","MK","FO"}},
			{"banner_position_x", new List<string>(){"left","center","right"}},
			{"banner_position_y", new List<string>(){"top","center","bottom"}},
			{"banner_method", new List<string>(){"none","file","check","mapp","xml","del","script"}},
			{"orientation", new List<string>(){"ll","lr","p","pud","ar"}}
		};

		static bool IsValidParam (string name, object value)
		{
			if (value == null)
				return false;

			try
			{
				return validation [name].Contains (value.ToString ());
			}
			catch (Exception)
			{
				return false;
			}
		}

		static string metaKeyword=@"
  textureFormat: -";

		static string correctTextureFormat=@"
  textureFormat: -3";
	}
}
#endif