using UnityEngine;
using UnityEditor;
using System.Collections;

public class LocalizatorHelper : MonoBehaviour {

	[MenuItem("Custom/Language/English", false)]
	public static void SetEnglish()
	{
		SetLanguage(SystemLanguage.English);
	}
 
	[MenuItem("Custom/Language/German", false)]
	public static void SetGerman()
	{
		SetLanguage(SystemLanguage.German);
	}

	[MenuItem("Custom/Language/Italian", false)]
	public static void SetItalian()
	{
		SetLanguage(SystemLanguage.Italian);
	}

	[MenuItem("Custom/Language/Spanish", false)]
	public static void SetSpanish()
	{
		SetLanguage(SystemLanguage.Spanish);
	}

	[MenuItem("Custom/Language/Portuguese", false)]
	public static void SetPortuguese()
	{
		SetLanguage(SystemLanguage.Portuguese);
	}

	[MenuItem("Custom/Language/French", false)]
	public static void SetFrench()
	{
		SetLanguage(SystemLanguage.French);
	}

	[MenuItem("Custom/Language/Chinese", false)]
	public static void SetChinese()
	{
		SetLanguage(SystemLanguage.Chinese);
	}

	[MenuItem("Custom/Language/Japanese", false)]
	public static void SetJapanese()
	{
		SetLanguage(SystemLanguage.Japanese);
	}
	 
	[MenuItem("Custom/Language/Russian", false)]
	public static void SetRussian()
	{
		SetLanguage(SystemLanguage.Russian);
	}

	[MenuItem("Custom/Language/Korean", false)]
	public static void SetKorean()
	{
		SetLanguage(SystemLanguage.Korean);
	}

	[MenuItem("Custom/Language/Reset", false, 99)]
	public static void ResetLanguage()
	{
		SetLanguage(Application.systemLanguage);
	}

	static void SetLanguage(SystemLanguage lang)
	{
		//CDebug.Log("Set Debug Language to: " + lang);
		Localizator.DebugLanguage = lang;
		PlayerPrefsHelper.SetString(Localizator.DebugLanguageEntry, lang.ToString());
	}
}
