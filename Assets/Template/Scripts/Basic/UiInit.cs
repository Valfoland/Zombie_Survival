using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// Класс инициализации UI
/// </summary>
public class UiInit : MonoBehaviour
{
	public static Action<bool> OnInitComplete;
	public static bool InitComplete;

	private void Awake()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		OnInitComplete?.Invoke(false);
		if (!InitComplete)
		{
			WaitForCustomCode();
		}
		else
		{
			OnInitComplete?.Invoke(true);
		}
	}

	private IEnumerator WaitingForLocalizator()
	{
		while (!Localizator.isInited)
		{
			yield return null;
		}
		OnInitComplete?.Invoke(true);
		InitComplete = true;
	}

	private void WaitForCustomCode()
	{
		CustomCode.CustomResourcesManager.isInitPlugin(OnCustomCodeLoaded);
	}

	private void OnCustomCodeLoaded()
	{
		Localizator.Init();
		WaitingForLocalizator();
	}
}
