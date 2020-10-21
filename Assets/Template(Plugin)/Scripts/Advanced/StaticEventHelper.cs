#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections;

public delegate void HelperDelegate();

public class StaticEventHelper : MonoBehaviour {
	public static event HelperDelegate ShowBannerEvent = delegate {};
	public static event HelperDelegate PauseEvent = delegate {};
	public static event HelperDelegate ResumeEvent = delegate {};

	public static void CallShowBannerEvent()
	{
		ShowBannerEvent();
	}

	public static void CallPauseEvent()
	{
		PauseEvent();
	}

	public static void CallResumeEvent()
	{
		ResumeEvent();
	}
}
#endif
