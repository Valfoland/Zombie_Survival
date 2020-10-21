#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AMEditor
{
    public class AMEditorCustomQualitySetupAPI
    {
        struct CustomQualitySetting
        {
            public int ram;
            public int qualityLevel;
            public int overrideMasterTextureLevel;
        }

        static string PATH_TO_CONFIGS = "Assets/AMPlugins/Editor/AMEditor/cqs_defaults.json";
        static string AM_PROJECT = "Assets/StreamingAssets/am_project.txt";

        static List<CustomQualitySetting> defaultSettings;

        public static void SetDefaults ()
        {
            GetDefaults ();
            
            if (defaultSettings != null && defaultSettings.Count > 0)
            {
                if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._DefaultsForCustomQualitySetup, AMEditorSystem.ContentStatuses._SettingDefaultsForCQS, 0.9f))
                {
                    EditorUtility.ClearProgressBar ();
                }

                ArrayList quality_settings = new ArrayList ();

                foreach (var item in defaultSettings)
                {
                    Hashtable settingHashtable = new Hashtable ();

                    settingHashtable.Add ("ram", item.ram);
                    settingHashtable.Add ("quality_level", item.qualityLevel);
                    settingHashtable.Add ("master_texture_limit", item.overrideMasterTextureLevel);

                    quality_settings.Add (settingHashtable);
                }

                if (!AMEditorFileStorage.FileExist (AM_PROJECT))
                {
                    AMEditorPopupErrorWindow.ShowErrorPopup ("205", AMEditorSystem.FileSystemError._205 ("am_project.txt"));
					EditorUtility.ClearProgressBar ();
                    return;
                }

                Hashtable am_project = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AM_PROJECT)) as Hashtable;
                Hashtable project_info = am_project["project_info"] as Hashtable;

                ArrayList project_info_quality_settings = project_info["quality_settings"] as ArrayList;
                if (project_info_quality_settings != null)
                {
                    project_info.Remove ("quality_settings");
                    project_info.Add ("quality_settings", quality_settings);
                }
                else
                {
                    project_info.Add ("quality_settings", quality_settings);
                }
                am_project.Remove ("project_info");
                am_project.Add ("project_info", project_info);

                File.WriteAllText (AM_PROJECT, AMEditorJSON.JsonEncode (am_project));

                if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._DefaultsForCustomQualitySetup, AMEditorSystem.ContentStatuses._SettingDefaultsForCQS, 1f))
                {
                    EditorUtility.ClearProgressBar ();
                }

                new UI.AMDisplayDialog (AMEditorSystem.ContentCustomQualitySetup._TitleDialog, AMEditorSystem.ContentCustomQualitySetup._MessageSuccessDialog, AMEditorSystem.ContentStandardButton._Ok, "", () => {EditorUtility.ClearProgressBar ();}, () => { }, true).Show ();
            }
        }

        static void GetDefaults ()
        {
            if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._DefaultsForCustomQualitySetup, AMEditorSystem.ContentStatuses._LoadingDefaultsForCQS, 0.0f))
            {
                EditorUtility.ClearProgressBar ();
            }

            if (defaultSettings == null)
            {
                defaultSettings = new List<CustomQualitySetting> ();
            }

            if (!AMEditorFileStorage.FileExist (PATH_TO_CONFIGS))
            {
				File.WriteAllText (PATH_TO_CONFIGS, "[]");
            }

            if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._DefaultsForCustomQualitySetup, AMEditorSystem.ContentStatuses._LoadingDefaultsForCQS, 0.3f))
            {
                EditorUtility.ClearProgressBar ();
            }

            ArrayList configsList = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (PATH_TO_CONFIGS)) as ArrayList;

            if (configsList != null && configsList.Count > 0)
            {
                if (EditorUtility.DisplayCancelableProgressBar (AMEditorSystem.ContentProgressBar._DefaultsForCustomQualitySetup, AMEditorSystem.ContentStatuses._ReadingDefaultsForCQS, 0.6f))
                {
                    EditorUtility.ClearProgressBar ();
                }
                foreach (var item in configsList)
                {
                    Hashtable config = item as Hashtable;
                    int configRam = -1;
                    int configQualityLevel = -1;
                    int configOverrideMasterTextureLevel = -1;

                    try
                    {
                        configRam = int.Parse (config["ram"].ToString ());
                        configQualityLevel = int.Parse (config["quality_level"].ToString ());
                        configOverrideMasterTextureLevel = int.Parse (config["master_texture_limit"].ToString ());
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning (ex.ToString ());
                    }
                    
                    defaultSettings.Add (new CustomQualitySetting
                    {
                        ram = configRam, 
                        qualityLevel = configQualityLevel, 
                        overrideMasterTextureLevel = configOverrideMasterTextureLevel, 
                    });
                }
            }
        }

        void OnDestroy ()
        {
            defaultSettings = null;
        }
    }
}
#endif