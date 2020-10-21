#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace AMEditor
{
	public class AMEditorAPI 
	{
		struct Argument
		{
			public string name;
			public string value;
		}

        public const string DEFAULT_BUILD_PATH = "Builds";
		static string buildPath = string.Empty;
        static string buildTargetString = string.Empty;
		static List<string> buildTargetValidation = new List<string> ();
        static BuildOptions buildOptions = BuildOptions.None;
        static string[] commandLineArgs;
        static string[] enabledScenesPathes;
		static List<string> orientation;
		static bool symlinkLibs = false;
		static bool devBuild = false;
		static bool connectProfiler = false;
		static bool scriptDebug = false;
		static bool generateObb = false;
		static bool portraitOSX = false;
		static bool macAppStoreValidation = false;
		static bool muteMusic = false;
		static bool trueColor = false;
		static bool buildWithUpdate = false;
        
        #region Update Plugins
		public static void UpdatePlugins ()
		{
			int result = 0;
			string buildType = string.Empty;
			string token = "ewViz-mR_VEzR4dgTv8t";

			commandLineArgs = System.Environment.GetCommandLineArgs ();

			if (Array.IndexOf (commandLineArgs, "-buildType") > -1)
			{
				if (Array.IndexOf (commandLineArgs, "standard") > -1)
					buildType = "standard";
				else if (Array.IndexOf (commandLineArgs, "googleplay") > -1)
					buildType = "googleplay";
				else
					buildType = "standard";
			}
			else
			{
				buildType = "standard";
			}
			AMEditor.WindowMain.MakeMetaVisible ();
			if (!UnityEditor.EditorSettings.externalVersionControl.Equals ("Visible Meta Files"))
			{
				UnityEditor.EditorSettings.externalVersionControl = "Visible Meta Files";
				AMEditor.WindowMain.FixMetaBundle ();
				AMEditor.WindowMain.FixMetaA ();
			}
			AMEditor.WindowMain.launchMode = AMEditor.WindowMain.LaunchMode.Console;
			AMEditor.WindowMain.selectedBuildType = buildType;

			ArrayList list = null;
			if (AMEditorFileStorage.FileExist (LocalRepositoryAPI.pathToRepository + AMEditorSystem.FileNames._Account))
			{
				list = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (LocalRepositoryAPI.pathToRepository + AMEditorSystem.FileNames._Account)) as ArrayList;	 
			}
			if ((list != null) && (list.Count > 0))
			{
				GitAccount.current = new GitAccount (list [0] as Hashtable); 
			}

			if (GitAccount.current == null)
			{
				try 
				{
					AMEditorGit git = new AMEditorGit ();
					git.AuthByPT (AuthWindow.listServer[0], token);

					GitAccount newAcc = new GitAccount ();
					newAcc.name = (string)((AMEditorJSON.JsonDecode (git.GetUserInfo (token)) as Hashtable)["name"]);
					newAcc.privateToken = token;
					newAcc.server = AuthWindow.listServer[0];
					GitAccount.current = newAcc;
				} 
				catch (Exception ex) 
				{
					Debug.Log ("AM EDITOR UPDATE: AUTH ERROR: " + ex.ToString ());
					EditorApplication.Exit (1);
				}
			}

			result = AMEditor.WindowMain.CheckUpdate ();

			if (AMEditor.WindowMain.instance == null)
			{
				AMEditor.WindowMain.Init ();
				AMEditor.WindowMain.launchMode = AMEditor.WindowMain.LaunchMode.Console;
			}
			AMEditor.WindowMain.instance.Focus ();
			AMEditor.WindowMain.instance.UpdatePluginsGUI ();
			bool isAllGood = AMEditor.WindowMain.instance.IsAllGood ();
			if (isAllGood)
			{
				Debug.Log ("AM EDITOR UPDATE: CUSTOM CODE PLUGINS ALREADY UPDATED");
				if (!buildWithUpdate)
					EditorApplication.Exit (result);
				else
					ContinueBuild ();
			}
			else
			{
				Debug.Log ("AM EDITOR UPDATE: CUSTOM CODE PLUGINS PROBLEM: " + AMEditor.WindowMain.instance.pluginsStatus.ToString ());
				AMEditor.WindowMain.instance.MakeAllGoodComplete += MakeAllGoodCompleteHandler;
				AMEditor.WindowMain.instance.MakeAllGood ();
			}
		}

		static void MakeAllGoodCompleteHandler (string exitMessage)
		{
			Debug.Log ("AM EDITOR UPDATE: MAKE ALL GOOD COMPLETE");

			AMEditor.WindowMain.instance.MakeAllGoodComplete -= MakeAllGoodCompleteHandler;
			foreach (var plugin in AMEditor.WindowMain.installedPlugins)
				AMEditor.WindowMain.AutoFixConflict (plugin, true);

			int exitCode = 1;

			if (exitMessage == "0")
			{
				exitCode = 0;
				Debug.Log ("AM EDITOR UPDATE: CUSTOM CODE PLUGINS SUCCESSFULLY UPDATED");
			}
			else if (exitMessage.Contains (":"))
			{
				if (exitMessage.Substring (0, exitMessage.IndexOf (':')) != string.Empty)
				{
					try
					{
						exitCode = int.Parse (exitMessage.Substring (0, exitMessage.IndexOf (':')));
						Debug.Log ("AM EDITOR UPDATE: ERROR UPDATING PLUGINS. CODE: " + exitMessage);
					}
					catch (Exception)
					{
						exitCode = 1;
						Debug.Log ("AM EDITOR UPDATE: ERROR UPDATING PLUGINS. CODE: 1" + exitMessage);
					}
				}
				else
				{
					Debug.Log ("AM EDITOR UPDATE: ERROR UPDATING PLUGINS. CODE: 1" + exitMessage);
				}
			}
			else
			{
				Debug.Log ("AM EDITOR UPDATE: ERROR UPDATING PLUGINS. CODE: 1." + exitMessage);
			}
			if (!buildWithUpdate)
				EditorApplication.Exit (exitCode);
			else
				ContinueBuild ();
		}
        #endregion

        #region Build Project
        static bool ParseBuildArgs ()
        {
            buildTargetValidation.Add ("android");
            buildTargetValidation.Add ("ios");
            buildTargetValidation.Add ("tvos");
            buildTargetValidation.Add ("osx");
            buildTargetValidation.Add ("mac");
            buildTargetValidation.Add ("uwp");
            buildTargetValidation.Add ("wsa");

            commandLineArgs = Environment.GetCommandLineArgs ();

            int jsonKeyIndex = Array.IndexOf (commandLineArgs, "-paramsJson");

            Hashtable jsonParams = null;

            if (jsonKeyIndex > -1)
            {
                string jsonPath = commandLineArgs[jsonKeyIndex + 1];

                if (File.Exists (jsonPath))
                {
					jsonParams = AMEditorJSON.JsonDecode (File.ReadAllText (jsonPath)) as Hashtable;
                }
                else
                {
					Debug.Log ("AM EDITOR BUILD: ERROR PARSING JSON: Incorrect json path!");
                    return false;
                }
            }

            if (jsonParams == null)
            {
				Debug.Log ("AM EDITOR BUILD: ERROR PARSING JSON: Incorrect json struct!");
                return false;
            }

            try
            {
                buildTargetString = (string)jsonParams["buildTarget"];
                if (string.IsNullOrEmpty (buildTargetString))
                {
					Debug.Log ("AM EDITOR BUILD: ERROR PARSING JSON: Incorrect build target!");
                    return false;
                }
				buildTargetString = buildTargetString.ToLower ();
            }
            catch (Exception)
            {
				Debug.Log ("AM EDITOR BUILD: ERROR PARSING JSON: Incorrect build target!");
                return false;
            }

            try
            {
                buildPath = (string)jsonParams["buildPath"];
                if (string.IsNullOrEmpty (buildPath))
                    buildPath = DEFAULT_BUILD_PATH;
            }
            catch (Exception)
            {
                buildPath = DEFAULT_BUILD_PATH;
            }

            try
            {
                orientation = new List<string> ();
                ArrayList orientations = jsonParams["orientation"] as ArrayList;
                foreach (var item in orientations)
                {
                    orientation.Add (item.ToString ());
                }
                if (orientation == null)
                    orientation = new List<string> ();
            }
            catch (Exception)
            {
                orientation = new List<string> ();
            }

            try
            {
                generateObb = (bool)jsonParams["generateObb"];
            }
            catch (Exception)
            {
                generateObb = false;
            }

            try
            {
                portraitOSX = (bool)jsonParams["portraitOSX"];
            }
            catch (Exception)
            {
                portraitOSX = false;
            }

            try
            {
                macAppStoreValidation = (bool)jsonParams["macAppStoreValidation"];
            }
            catch (Exception)
            {
                if (Array.IndexOf (commandLineArgs, "-macAppStoreValidation") > -1)
                    macAppStoreValidation = true;
                else
                    macAppStoreValidation = false;
            }

            try
            {
                muteMusic = (bool)jsonParams["muteMusic"];
            }
            catch (Exception)
            {
                muteMusic = false;
            }

            try
            {
                trueColor = (bool)jsonParams["trueColor"];
            }
            catch (Exception)
            {
                trueColor = false;
            }

            try
            {
                symlinkLibs = (bool)jsonParams["symlinkLibs"];
            }
            catch (Exception)
            {
                symlinkLibs = false;
            }

            try
            {
                devBuild = (bool)jsonParams["devBuild"];
            }
            catch (Exception)
            {
                devBuild = false;
            }

            try
            {
                connectProfiler = (bool)jsonParams["connectProfiler"];
            }
            catch (Exception)
            {
                connectProfiler = false;
            }

            try
            {
                scriptDebug = (bool)jsonParams["scriptDebug"];
            }
            catch (Exception)
            {
                scriptDebug = false;
            }

			try
			{
				buildWithUpdate = (bool)jsonParams["withUpdate"];
			}
			catch (Exception)
			{
				buildWithUpdate = false;
			}

            return true;
        }

        static void ConfigureBuildOptions ()
        {
			if (EditorUserBuildSettings.development)
                buildOptions = BuildOptions.Development;
            else if (EditorUserBuildSettings.buildScriptsOnly)
                buildOptions = BuildOptions.BuildScriptsOnly;
            else if (EditorUserBuildSettings.connectProfiler)
                buildOptions = BuildOptions.ConnectWithProfiler;
            else
                buildOptions = BuildOptions.None;
        }

        static string[] GetEnabledScenesPathes ()
        {
            List<string> EditorScenes = new List<string> ();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                    EditorScenes.Add (scene.path);
            }
            return EditorScenes.ToArray ();
        }

        static void WriteBuildResult (string result, string filename)
        {
            if (filename == string.Empty)
                return;
            StreamWriter file = File.CreateText (filename);
            file.WriteLine (result);
            file.Close ();

            return;
        }

        public static void BuildProject ()
		{
			string[] invalidProductNameSymbols = new string[]{ "&", "'", "`", "–", "\u001f—" };
			if (invalidProductNameSymbols.Any (PlayerSettings.productName.Contains))
			{
				int index = PlayerSettings.productName.ToList ().FindIndex (c => {
					return invalidProductNameSymbols.Contains (c.ToString ());
				});
				if (index != -1)
				{
					Debug.Log ("AM EDITOR BUILD: ERROR: Product Name has invalid symbol '" + invalidProductNameSymbols [index] + "'!");
					EditorApplication.Exit (1);
				}
			}

			if (PlayerSettings.productName.Any (c => System.Convert.ToInt32 (c) > 128))
			{
				Debug.Log ("AM EDITOR BUILD: ERROR: Product Name field must contains latin symbols only!");
				EditorApplication.Exit (1);
			}

			if (!ParseBuildArgs ())
			{
				EditorApplication.Exit (1);
			}

			if (buildWithUpdate)
			{
				UpdatePlugins ();
			}
			else
			{
				ContinueBuild ();
			}
		}

		static void ContinueBuild ()
		{
			if (muteMusic)
			{
				bool success = AAPSettings.MuteMusic ();
				if (!success)
					Debug.Log ("AM EDITOR BUILD: ERROR: Mute music failed!");
			}

			if (trueColor)
			{
				bool success = AAPSettings.MakeTexturesTruecolor ();
				if (!success)
					Debug.Log ("AM EDITOR BUILD: ERROR: Make textures to truecolor failed!");
			}

            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

            if (!string.IsNullOrEmpty (buildTargetString) && buildTargetValidation.Contains (buildTargetString))
			{
				Debug.Log ("AM EDITOR BUILD: PREPARE FOR " + buildTargetString.ToUpper () + " BUILD");

                switch (buildTargetString)
				{
				case "android":
                    buildTarget = BuildTarget.Android;
					EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTargetGroup.Android, buildTarget);
					EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
					EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.Generic;
					EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
					PlayerSettings.Android.useAPKExpansionFiles = generateObb;
                    buildOptions = BuildOptions.AcceptExternalModificationsToPlayer;
                    break;
				case "ios":
                    buildTarget = BuildTarget.iOS;
					EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTargetGroup.iOS, BuildTarget.iOS);
					PlayerSettings.iOS.targetOSVersionString = "9.0";
					PlayerSettings.SetScriptingBackend (BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
					PlayerSettings.SetArchitecture (BuildTargetGroup.iOS, 2);
					PlayerSettings.iOS.appleEnableAutomaticSigning = false;
                    if (orientation != null && orientation.Count > 0)
					{
						if (orientation.IndexOf ("PortratUp") > -1)
							PlayerSettings.allowedAutorotateToPortrait = true;
						else PlayerSettings.allowedAutorotateToPortrait = false;

						if (orientation.IndexOf ("PortratDown") > -1)
							PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
						else PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

						if (orientation.IndexOf ("LandscapeLeft") > -1)
							PlayerSettings.allowedAutorotateToLandscapeLeft = true;
						else PlayerSettings.allowedAutorotateToLandscapeLeft = false;

						if (orientation.IndexOf ("LandscapeRight") > -1)
							PlayerSettings.allowedAutorotateToLandscapeRight = true;
						else PlayerSettings.allowedAutorotateToLandscapeRight = false;
					}
					EditorUserBuildSettings.symlinkLibraries = symlinkLibs;
                    break;
				case "tvos":
                buildTarget = BuildTarget.tvOS;
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTargetGroup.tvOS, buildTarget);
				PlayerSettings.tvOS.targetOSVersionString = "9.0";
				PlayerSettings.SetScriptingBackend (BuildTargetGroup.tvOS, ScriptingImplementation.IL2CPP);
				PlayerSettings.SetArchitecture (BuildTargetGroup.tvOS, 2);
				PlayerSettings.iOS.appleEnableAutomaticSigning = false;
                    break;
				case "mac":
				case "osx":
					PlayerSettings.usePlayerLog = false;
					PlayerSettings.useMacAppStoreValidation = macAppStoreValidation;
					buildTarget = BuildTarget.StandaloneOSX;
					EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTargetGroup.Standalone, buildTarget);
					if (portraitOSX)
					{
						PlayerSettings.defaultScreenHeight = 800;
						PlayerSettings.defaultScreenWidth = 500;
						#if UNITY_2018_2_OR_NEWER || UNITY_2018_3_OR_NEWER || UNITY_2019_1_OR_NEWER
							PlayerSettings.fullScreenMode = 0;
						#else
							PlayerSettings.defaultIsFullScreen = false;
						#endif
					}
                    break;
				case "wsa":
				case "uwp":
				EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.XAML;
				PlayerSettings.WSA.splashScreenBackgroundColor = Color.black;
				buildTarget = BuildTarget.WSAPlayer;
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTargetGroup.WSA, buildTarget);
                    break;
				default:
					Debug.Log ("ERROR: Incorrect Target Platform: " + buildTargetString.ToUpper () + "!");
                    EditorApplication.Exit (1);
                    break;
				}
			}
			PlayerSettings.SplashScreen.show = false;
			PlayerSettings.SplashScreen.showUnityLogo = false;
			PlayerSettings.SetApplicationIdentifier (BuildTargetGroup.iOS, "com.phonar.bundle");
			PlayerSettings.applicationIdentifier = "com.phonar.bundle";
			PlayerSettings.statusBarHidden = true;
			EditorUserBuildSettings.development = devBuild;

			if (connectProfiler)
				EditorUserBuildSettings.development = true; 
			EditorUserBuildSettings.connectProfiler = connectProfiler;
			EditorUserBuildSettings.buildScriptsOnly = scriptDebug;
			bool fixLoaclizationResult = AAPSettings.FixLocalization ();
			if (!fixLoaclizationResult)
			{
				Debug.Log ("AM EDITOR BUILD: ERROR: Localization fix failed!");
			}
            
			Debug.Log ("AM EDITOR BUILD: START BUILD " + buildTargetString.ToUpper ());

            ConfigureBuildOptions ();

            enabledScenesPathes = GetEnabledScenesPathes ();

			string resultPath = buildPath + Path.DirectorySeparatorChar + "result.txt";

            if (Directory.Exists (buildPath))
            {
                Directory.Delete (buildPath, true);
            }
            Directory.CreateDirectory (buildPath);

			if (buildTarget != BuildTarget.Android)
				buildPath = buildPath + Path.DirectorySeparatorChar + PlayerSettings.productName;
			else
				buildOptions = BuildOptions.AcceptExternalModificationsToPlayer;
			if (buildTarget == BuildTarget.StandaloneOSX)
            	buildPath = buildPath + ".app";

			var buildResult = BuildPipeline.BuildPlayer (enabledScenesPathes, buildPath, buildTarget, buildOptions);
			
			if (buildResult == null)
			{
				Debug.Log ("AM EDITOR BUILD: FAILED");
				WriteBuildResult ("buildResult", resultPath);
			}
			else
			{
				Debug.Log ("AM EDITOR BUILD: SUCCESSFULLY FINISHED");
				WriteBuildResult (PlayerSettings.productName, resultPath);
			}
			EditorApplication.Exit (0);
		}
        #endregion
        void OnDestroy ()
		{

		}
	}
}
#endif