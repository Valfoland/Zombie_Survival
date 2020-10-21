using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CustomQualitySetup
{
	public class CustomQualitySetup : MonoBehaviour 
	{
		public bool CustomMode = false;

		public static bool ccCustomMode;
		public static bool customCodeEnable = false;

		public List<CustomQualitySetting> customQualitySettings = new List<CustomQualitySetting> ();

		string testInfo;

		static bool used;

		void Start () 
		{
			if (customCodeEnable)
				CustomMode = ccCustomMode;

			if (used)
			{
				Destroy (gameObject);
			}
			else
			{
				DontDestroyOnLoad (gameObject);
				used = true;
				if(AMConfigsParser.AMParseJsonConfig.instance == null)
					new GameObject ("AMConfigsParser").AddComponent (typeof (AMConfigsParser.AMParseJsonConfig));
				AMConfigsParser.AMParseJsonConfig.isParseEnd (StartCustomQuality);
			}
		}

		void StartCustomQuality ()
		{
			var AMProjectConfig = new AMConfigsParser.AMProjectInfo ();
			if ((AMProjectConfig != null) && (AMProjectConfig.customQualitySettings.Length > 0))
			{
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
				Calculate ();
			}
		}
		
		void Calculate ()
		{
			int ram = SystemInfo.systemMemorySize;

			testInfo = "RAM: " + ram;

			for (int i = 0; i < customQualitySettings.Count; i++)
			{
				if (ram < customQualitySettings[i].ram)
				{
					SetLevel (i);
					return;
				}
			}
			SetLevel (customQualitySettings.Count);
		}

		void SetLevel (int index)
		{
			testInfo += " Level " + index;

			if (index >= customQualitySettings.Count)
				index = customQualitySettings.Count - 1;
			if (index < 0)
				index = 0;

			QualitySettings.SetQualityLevel (customQualitySettings[index].qualityLevel, true);
			if (!CustomMode)
				QualitySettings.masterTextureLimit = customQualitySettings[index].overrideMasterTextureLevel;
		}

		void OnGUI ()
		{
			if (Debug.isDebugBuild)
			{
				GUIStyle debugInfoStyle = new GUIStyle (GUI.skin.box);
				debugInfoStyle.fontSize = (Screen.width > Screen.height) ? (int)Screen.height / 25 : (int)Screen.width / 25;
				debugInfoStyle.alignment = TextAnchor.UpperLeft;

				GUI.Box (new Rect (0, 0, Screen.width / 2, (Screen.width > Screen.height) ? Screen.height / 15 : Screen.width / 15), testInfo, debugInfoStyle);
			}
		}
	}

	public class CustomQualitySetting
	{
		public int ram;
		public int qualityLevel;
		public int overrideMasterTextureLevel;
	}
}
