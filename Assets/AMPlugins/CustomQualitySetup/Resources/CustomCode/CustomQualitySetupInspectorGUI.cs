#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

namespace CustomQualitySetup
{
	/// <summary>
	/// Инструмент для загрузки кастомных скриптов из ресурсов
	/// </summary>
	[CustomEditor (typeof (CustomQualitySetup))]
	public class CustomQualitySetupInspectorGUI : Editor
	{
		static List<CustomQualitySetting> customQualitySettings = new List<CustomQualitySetting> ();

		public override void OnInspectorGUI ()
		{
			CustomQualitySetup customQualitySetup = (CustomQualitySetup)target;

			base.OnInspectorGUI ();

			DrawInspector ();

			if (GUILayout.Button ("Refresh list Custom Quality Settings"))
			{
				UpdateData ();
			}

			EditorUtility.SetDirty (customQualitySetup);
		}

		public static void UpdateData ()
		{
			AMConfigsParser.AMParseJsonConfig.DevelopParse ();
			var AMProjectConfig = new AMConfigsParser.AMProjectInfo ();
			
			customQualitySettings = new List<CustomQualitySetting> ();
			foreach (var item in AMProjectConfig.customQualitySettings) 
			{
				var CQS = new CustomQualitySetting ();
				CQS.ram = item.ram;
				CQS.qualityLevel = item.qualityLevel;
				CQS.overrideMasterTextureLevel = item.masterTextureLimit;
				customQualitySettings.Add (CQS);
			}
			customQualitySettings.Sort ((x, y) => { 
				return x.ram.CompareTo (y.ram);
			});
		}
		public static void DrawInspector ()
		{
			GUILayout.BeginVertical ();
			if (customQualitySettings.Count > 0)
			{
				GUILayout.Label ("Custom Quality Settings: ");
			}
			
			foreach (var item in customQualitySettings) 
			{
				GUI.enabled = false;
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("   Max Memory: ", GUILayout.MaxWidth (200));
				GUILayout.TextField (item.ram.ToString ());
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("   Quality Level: ", GUILayout.MaxWidth (200));
				GUILayout.TextField (item.qualityLevel.ToString ());
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("   Master Texture Level: ", GUILayout.MaxWidth (200));
				GUILayout.TextField (item.overrideMasterTextureLevel.ToString ());
				GUILayout.EndHorizontal ();

				GUI.enabled = true;

				GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (Screen.width));
			}
			GUILayout.EndVertical ();
		}
	}
}
#endif