#pragma warning disable
using UnityEngine;
using System.Collections;

namespace AMEvents
{
	public class PauseController : MonoBehaviour 
	{
		public static bool needToPause = false;
		private static bool previousAudioListenerPauseState = false;
		private static float previousTimeScale = 1f;

		public static void Init ()
		{
			Ad.Interstitial.Impression += PauseController.PauseApp;
			Ad.Interstitial.Closed += PauseController.ResumeApp;
			Ad.InnerInApp.Impression += PauseController.PauseApp;
			Ad.InnerInApp.Closed += PauseController.ResumeApp;
			Ad.InnerInApp.Success += PauseController.ResumeApp;
		}

		public static IEnumerator SetPauseIfNecessery ()
		{
			while (needToPause) 
			{
				if (Time.timeScale > 0)
					PauseApp ();
				yield return null;
			}
		}

		public static void PauseApp ()
		{
			needToPause = true;
			if (Time.timeScale > 0)
				previousTimeScale = Time.timeScale;
			if (!AudioListener.pause)
				previousAudioListenerPauseState = AudioListener.pause;
			Time.timeScale = 0f;
			AudioListener.pause = true;
		}

		public static void ResumeApp ()
		{
			needToPause = false;
			Time.timeScale = previousTimeScale;
			AudioListener.pause = previousAudioListenerPauseState;
		}
	}
}