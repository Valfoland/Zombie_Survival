#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace AnalytiAll
{
	/// <summary>
	/// Класс для передачи событий перехода на новую сцену, паузы и возобновления сцены, а также собственных вызовов в моменты, когда приложение неактивно в нативный код.
	/// </summary>
	[CustomEditor (typeof (EventManager))]
	public class EventManagerInspectorGUI : Editor
	{
		bool isInit = false;

		int labelsWidth = 170;
		int labelsSpace = 15;

		EventManager eventManager;

		/// <summary>
		/// Инициализация.
		/// </summary>
		void Init ()
		{
			if (!isInit)
			{
				isInit = true;
				eventManager = (EventManager)target;
			}
		}

		/// <summary>
		/// Перерисовка GUI инспектора.
		/// </summary>
		public override void OnInspectorGUI ()
		{
			Init ();

			GUIStyle delayFieldStyle = new GUIStyle (GUI.skin.textField);
			delayFieldStyle.alignment = TextAnchor.MiddleRight;
			GUIStyle delayLabelStyle = new GUIStyle (GUI.skin.label);
			delayLabelStyle.alignment = TextAnchor.MiddleLeft;

			GUILayout.BeginHorizontal ();
			GUILayout.Space (labelsSpace);
			GUILayout.Label ("Debug Mode", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
			eventManager.DebugMode = GUILayout.Toggle (eventManager.DebugMode, "");
			GUILayout.EndHorizontal ();

			if (eventManager.DebugMode)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (labelsSpace);
				GUILayout.Label ("Debug Menu", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
				eventManager.DebugMenu = GUILayout.Toggle (eventManager.DebugMenu, "");
				GUILayout.EndHorizontal ();
				#if UNITY_TVOS
				if (eventManager.DebugMenu)
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Space (labelsSpace);
					GUILayout.Label ("Handle Controller", new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.MinWidth (labelsWidth), GUILayout.MaxWidth (labelsWidth)});
					eventManager.HandleController = GUILayout.Toggle (eventManager.HandleController, "");
					GUILayout.EndHorizontal ();
				}
				#endif
			}
		}
	}
}
#endif