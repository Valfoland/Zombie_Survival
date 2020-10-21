#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

namespace AMLogging
{
	[CustomEditor (typeof (AMLoggerConsole))]
	public class AMLoggingConsoleInspectorGUI : Editor 
	{
		bool isInit = false;

		int labelsWidth = 170;
		int labelsSpace = 15;

		AMLoggerConsole amLoggerConsole;

		/// <summary>
		/// Инициализация.
		/// </summary>
		void Init ()
		{
			if (!isInit)
			{
				isInit = true;
				amLoggerConsole = (AMLoggerConsole)target;
			}
		}

		/// <summary>
		/// Перерисовка GUI инспектора.
		/// </summary>
		public override void OnInspectorGUI ()
		{
			Init ();

			GUILayout.BeginHorizontal ();
			GUILayout.Space (labelsSpace);
			GUILayout.Label ("Enable Log Console", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
			amLoggerConsole.enableConsole = GUILayout.Toggle (amLoggerConsole.enableConsole, "");
			GUILayout.EndHorizontal ();

			if (amLoggerConsole.enableConsole)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Font Size", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				try 
				{
					amLoggerConsole.fontSize = int.Parse (EditorGUILayout.TextField (amLoggerConsole.fontSize.ToString ()));
					if (amLoggerConsole.fontSize < 0)
						amLoggerConsole.fontSize = amLoggerConsole.fontSize * (-1);
				} 
				catch (Exception) {}
				GUILayout.EndHorizontal ();

//				GUILayout.BeginHorizontal ();
//				GUILayout.Space (labelsSpace);
//				GUILayout.Label ("Timestamp", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
//				amLoggerConsole.useTimestamp = GUILayout.Toggle (amLoggerConsole.useTimestamp, "");
//				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Lightwieght Mode", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				amLoggerConsole.lightweight = GUILayout.Toggle (amLoggerConsole.lightweight, "");
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Console Toggle Key", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				var selectedKeyCode = EditorGUILayout.EnumPopup (amLoggerConsole.toggleKey).ToString ();
				amLoggerConsole.toggleKey = (KeyCode) System.Enum.Parse (typeof (KeyCode), selectedKeyCode);
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Console Shake Acceleration", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				try 
				{
					amLoggerConsole.shakeAcceleration = float.Parse (EditorGUILayout.TextField (amLoggerConsole.shakeAcceleration.ToString ()));
					if (amLoggerConsole.shakeAcceleration < 0)
						amLoggerConsole.shakeAcceleration = amLoggerConsole.shakeAcceleration * (-1);
				} 
				catch (Exception) {}
				GUILayout.EndHorizontal ();
			}
		}
	}
}
#endif