#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace AMEditor
{
	class WindowAbout : EditorWindow
	{
		static EditorWindow window;

		public static string readme = "";
		static List<string> changeLog = new List<string> ();
		static string version = "";

		Vector2 scrollPosition;

		#if UNITY_2018_1_OR_NEWER || UNITY_2018_2_OR_NEWER
			Texture logoTexture;
			public Texture LogoTexture
        	{
            	get
            	{
             		return logoTexture;
            	}

            	set
            	{
                	logoTexture = value;
         		}
        	}
		#else
			Texture logoTexture = new Texture ();
		#endif
		
        public static void Init ()
		{
			readme = AMEditorGit.RequestGet ("http://pgit.digital-ecosystems.ru/unity-plugins/am-editor/raw/master/Readme.md?private_token=" + GitAccount.current.privateToken);

			if (readme.Contains ("AM Editor"))
			{
				changeLog = readme.Split (new string[]{ "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList ();
			}

			window = EditorWindow.GetWindow<WindowAbout> (true, AMEditorSystem.ContentWindowChangeLog._Title);
			window.minSize = new Vector2 (600, 700);
			window.maxSize = new Vector2 (600, Screen.currentResolution.height);
			window.position = new Rect (Screen.currentResolution.width / 2 - window.minSize.x / 2, 
										Screen.currentResolution.height / 2 - window.minSize.y / 2, 
										window.minSize.x, 
										window.minSize.y);
			window.Repaint ();
		}

		void OnEnable ()
		{
		#if (UNITY_5 && !UNITY_5_0_1) || UNITY_2017_1_OR_NEWER || UNITY_2017_2_OR_NEWER || UNITY_2018_1_OR_NEWER || UNITY_2018_2_OR_NEWER
			logoTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_logo.png");
		#else
			logoTexture = Resources.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_logo.png");
		#endif
		}

		void Update ()
		{
			if (EditorApplication.isCompiling)
			{
				window.Close ();
			}
		}
	
		void OnGUI ()
		{
			GUIStyle logoStyle = new GUIStyle (GUI.skin.label);
			logoStyle.fontSize = 32;
			logoStyle.fontStyle = FontStyle.Bold;

			GUIStyle versionStyle = new GUIStyle (GUI.skin.label);
			versionStyle.fontSize = 12;

			GUIStyle changeLogTitleStyle = new GUIStyle (GUI.skin.label);
			changeLogTitleStyle.fontSize = 16;
			changeLogTitleStyle.fontStyle = FontStyle.Bold;

			version = changeLog[5].Substring (5, 5);

			GUILayout.Space (10);

			GUILayout.Label (new GUIContent (" AM Editor", logoTexture), logoStyle);
			GUILayout.Label ("\tVersion " + version, versionStyle);

			string changeLogTitle = changeLog[4].Replace ("## ", "");
			GUILayout.Space (5);
			GUILayout.Label (changeLogTitle, changeLogTitleStyle);

			GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect().y + GUILayoutUtility.GetLastRect ().height + 2, Screen.width + 2, 1), "");

			scrollPosition = GUILayout.BeginScrollView (scrollPosition);
			for (int i = 5; i < changeLog.Count; i++)
			{
				GUIStyle itemStyle = new GUIStyle (GUI.skin.label);
				itemStyle.wordWrap = true;

				var item = changeLog [i];

				if (item.Contains ("**"))
				{
					item = item.Replace ("**", "\"");
				} 
				else if (item.Contains ("*Windows*"))
				{
					item = item.Replace ("*Windows*", "Windows");
				}

				if (item.Contains ("#### "))
				{
					GUILayout.Space (5);
					item = item.Replace ("#### ", "");
					itemStyle.fontSize = 16;
					itemStyle.fontStyle = FontStyle.Bold;
				}
				else
				{
					itemStyle.fontSize = 12;
					itemStyle.fontStyle = FontStyle.Normal;
				}
				GUILayout.Label (item, itemStyle, GUILayout.Height ((float)itemStyle.fontSize + 8));
			}
			GUILayout.EndScrollView ();
			GUILayout.Space (40);

			if (GUI.Button (new Rect (Screen.width / 2 - 50, Screen.height - 30, 100, 20), AMEditorSystem.ContentStandardButton._Ok))
			{
				window.Close ();
			}
			GUI.Box (new Rect (-1, GUILayoutUtility.GetLastRect().y - 2, Screen.width + 2, 1), "");
		}
	}
}
#endif