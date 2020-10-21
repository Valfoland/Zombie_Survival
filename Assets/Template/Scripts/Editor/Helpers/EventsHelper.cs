using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public static class EventsHelper 
{
	[MenuItem("Custom/Events/Show Banner Event")]
	static void MakeBannerShow()
	{
		if(Application.isPlaying)
		{
			Debug.Log("ShowBannerEvent called");
			StaticEventHelper.CallShowBannerEvent();
		}
	}

	[MenuItem("Custom/Events/App Pause Event")]
	static void MakePause()
	{
		if(Application.isPlaying)
		{
			Debug.Log("AppPause called");
			StaticEventHelper.CallPauseEvent();
		}
	}

	[MenuItem("Custom/Events/App Resume Event")]
	static void MakeResume()
	{
		if(Application.isPlaying)
		{
			Debug.Log("AppResume called");
			StaticEventHelper.CallResumeEvent();
		}
	}
}
