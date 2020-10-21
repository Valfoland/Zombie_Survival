using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

/// <summary>
/// Загружает список строк из Resources/loc.xml
/// Определяем язык следующим образом:
/// - Получаем из defaultlanguage список языков
/// - Если системный язык входит в этот список, то выбираем его
/// - Иначе выбираем первый из списка
/// </summary>
public static class Localizator
{
	static bool DebugMode = true;
	static bool iOS_CustomLanguage = false;

	private static Dictionary<string,SystemLanguage> LanguageKeyCompare = new Dictionary<string, SystemLanguage>()
	{
		{ "US", SystemLanguage.English}, 
		{ "EN", SystemLanguage.English}, 
		{ "GB", SystemLanguage.English}, 
		{ "AU", SystemLanguage.English},
		{ "DE", SystemLanguage.German}, 
		{ "IT", SystemLanguage.Italian}, 
		{ "ES", SystemLanguage.Spanish}, 
		{ "PT", SystemLanguage.Portuguese}, 
		{ "FR", SystemLanguage.French}, 
		{ "CN", SystemLanguage.Chinese}, 
		{ "ZH", SystemLanguage.Chinese},
		{ "JP", SystemLanguage.Japanese}, 
		{ "JA", SystemLanguage.Japanese},
		{ "RU", SystemLanguage.Russian}, 
		{ "KR", SystemLanguage.Korean}, 
		{ "KO", SystemLanguage.Korean}
	};

	public static string DebugLanguageEntry = "debug_language";
	public static SystemLanguage DebugLanguage;

	static bool DefLangLoaded;
	static string basic_filename = "loc";
	static string[] ext_filenames = new string[]{"user_loc", "fake_update_loc"};
	public static SystemLanguage[] languages;
	public static SystemLanguage defaultLanguage = SystemLanguage.English;
	static Dictionary<string, Dictionary<SystemLanguage, string>> strings;
	public static bool isInited = false;

	static Dictionary<string, SystemLanguage> languageTags;

	public static void ForceInit()
	{
		Init();
	}

	public static void Init()
	{
		if(Application.isEditor)
			SetDebugLanguage();
		SetDefaultLanguage();
		ImportStrings();
		isInited = true;
	}

	static void SetDebugLanguage()
	{
		try
		{
			DebugLanguage = 
				(SystemLanguage)Enum.Parse(
					typeof(SystemLanguage), 
					PlayerPrefsHelper.GetString(
						DebugLanguageEntry, 
						SystemLanguage.English.ToString())
					);
			//if(DebugMode) CDebug.Log("Localizator: DebugLanguage is " + DebugLanguage);
		}
		catch
		{
			//if(DebugMode) CDebug.LogError("Localizator: Cant set debug language");
		}
	}

	static void SetDefaultLanguage()
	{
		try
		{
			if(Application.isEditor)
				SetLanguages("English");
			else
				SetLanguages(
					AMConfigsParser.AMProjectInfoInside.availableLanguages, 
					AMConfigsParser.AMBuildParamsInside.language
					);
			
		}
		catch (Exception ex)
		{
			defaultLanguage = SystemLanguage.Unknown;
			DefLangLoaded = false;
			//if(DebugMode) CDebug.LogError("Localizator: Can't load default language!");
			//if(DebugMode) CDebug.LogError("Localizator: " + ex.Message);
		}
	}

	static string WriteArray<T>(T[] array)
	{
		string s = "[";
		for(int i = 0; i < array.Length; i++)
		{
			s += "\"" + array[i].ToString() + "\"; ";
		}
		s += "]";
		return s;
	}

	static void WriteArrayToLog<T>(string prefix, T[] array)
	{
		for(int i = 0; i < array.Length; i++)
		{
			//if(DebugMode) CDebug.Log(prefix + " " + array[i].ToString());
		}
	}

	static void SetLanguages(string[] available_tokens, string[] current_tokens)
	{
		//if(DebugMode) CDebug.Log("Localizator: available_tokens (am_project):"); 
		WriteArrayToLog<string>("Localizator: ", available_tokens);

		//if(DebugMode) CDebug.Log("Localizator: current_tokens (am_builds):");
		WriteArrayToLog<string>("Localizator: ", current_tokens);

		List<SystemLanguage> current_languages = new List<SystemLanguage>();
		foreach(string current_token in current_tokens)
		{
			bool valid = false;
			for(int i = 0; i < available_tokens.Length; i++)
			{
				if(available_tokens[i] == current_token)
				{
					valid = true;
					break;
				}
			}
			if(valid)
			{
				SystemLanguage new_lang = ConvertLanguage(current_token);
				if(new_lang != SystemLanguage.Unknown)
					current_languages.Add(new_lang);
				//else
					//if(DebugMode) CDebug.LogError("Localizator: Language token " + current_token + " is invalid!");
			}
			//else
				//if(DebugMode) CDebug.LogWarning("Localizator: Language token " + current_token + " not supported.");
		}

		//if(DebugMode) CDebug.Log("Localizator: Languages converted: ");
		WriteArrayToLog<SystemLanguage>("Localizator:", current_languages.ToArray());

		SetLanguages(current_languages.ToArray());
	}

	static SystemLanguage ConvertLanguage(string token)
	{
		SystemLanguage lang = SystemLanguage.Unknown;
		if(LanguageKeyCompare.ContainsKey(token))
			lang = LanguageKeyCompare[token];
		return lang;
	}

	static SystemLanguage SafeConvertLanguage(SystemLanguage lang)
	{
		#if UNITY_5
		if(lang == SystemLanguage.ChineseSimplified)
			return SystemLanguage.Chinese;
		if(lang == SystemLanguage.ChineseTraditional)
			return SystemLanguage.Chinese;
		#endif
		return lang;
	}

	static void SetLanguages(SystemLanguage [] s)
	{
		try
		{
			if (s.Length < 1)
				throw new Exception("");
			else
			{
				//if(DebugMode) CDebug.Log("Localizator: Real SystemLanguage is " + Application.systemLanguage);

				SystemLanguage syslang = SafeConvertLanguage(Application.systemLanguage);

				//if(DebugMode) CDebug.Log("Localizator: Safe SystemLanguage is " + syslang);

				languages = new SystemLanguage[s.Length];
				Array.Copy(s,languages,s.Length);
				for(int i = 0; i < languages.Length; i++)
				{
					if(languages[i] == syslang)
					{
						//if(DebugMode) CDebug.Log("Localizator: Default Language is " + languages[i].ToString());

						defaultLanguage = languages[i];
						DefLangLoaded = true;
					}
				}
				if(!DefLangLoaded)
				{
					//if(DebugMode) CDebug.Log("Localizator: Default Language not loaded;");

					defaultLanguage = SystemLanguage.English;
					DefLangLoaded = true;
				}
			}
		}
		catch
		{
			defaultLanguage = SystemLanguage.Unknown;
			DefLangLoaded = false;
		}
	}
	
	static void SetLanguages(string s)
	{
		try
		{
			if (s.Contains (";")) 
			{
				// Multiple lines
				defaultLanguage = SystemLanguage.Unknown;
				string[] parts = s.Split(';');
				languages = new SystemLanguage[parts.Length];
				if(parts.Length > 0)
				{
					for(int i = 0; i < parts.Length; i++)
					{
						languages[i] = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), parts[i]);
						if(languages[i] == SafeConvertLanguage(Application.systemLanguage))
						{
							defaultLanguage = languages[i];
							DefLangLoaded = true;
						}
					}
					if(!DefLangLoaded)
					{
						defaultLanguage = languages[0];
						DefLangLoaded = true;
					}
				}
				else
				{
					throw new Exception("");
				}
			}
			else
			{
				// One line
				defaultLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), s);
			}
		}
		catch
		{
			defaultLanguage = SystemLanguage.Unknown;
			DefLangLoaded = false;
			//if(DebugMode) CDebug.LogError("Localizator: Can't parse languages string!");
		}
	}
	
	static void ImportStrings()
	{
		try
		{
			strings = new Dictionary<string, Dictionary<SystemLanguage, string>>();

			TextAsset text = Resources.Load(basic_filename) as TextAsset;
			ParseXml(text.text);
			foreach(string ext_filename in ext_filenames)
			{
				TextAsset ext_text = Resources.Load(ext_filename) as TextAsset;
				if(ext_text)
					ParseExtXml(ext_text.text);
			}
		}
		catch(Exception e)
		{
			strings = new Dictionary<string, Dictionary<SystemLanguage, string>>();
			//if(DebugMode) CDebug.LogError("Localizator: Can't load loc file! (" + e.Message + ")");
		}
	}
	
	static void ParseXml(string str)
	{
		XMLParser parser = new XMLParser();
		XMLNode parent = parser.Parse(str);
		
		XMLNodeList body_list = parent.GetNodeList("body");
		
		if(body_list.Count > 0)
		{
			XMLNode body = body_list.Pop();

			// Load languages
			XMLNode languages = body.GetNodeList("Languages").Pop();
			ParseLanguages(languages);
			
			// Load strings
			XMLNode text_node = body.GetNodeList("Text").Pop();
			ParseText(text_node);
		}
		else
		{
			//if(DebugMode) CDebug.LogError("Localizator: <body> tag in loc file could not found!");
		}
	}

	static void ParseExtXml(string str)
	{
		XMLParser parser = new XMLParser();
		XMLNode parent = parser.Parse(str);
		
		XMLNodeList body_list = parent.GetNodeList("body");
		
		if(body_list.Count > 0)
		{
			XMLNode body = body_list.Pop();
			
			// Load strings
			XMLNode text_node = body.GetNodeList("Text").Pop();
			if(text_node.Count > 0)
				ParseText(text_node);
		}
		else
		{
			//if(DebugMode) CDebug.LogError("Localizator: <body> tag in user_loc file could not found!");
		}
	}

	public static void ParseLanguages(XMLNode languages_node)
	{
		languageTags = new Dictionary<string, SystemLanguage>();
		foreach(string language_key in languages_node.Keys)
		{
			if((language_key != "_name") && (language_key != "_text"))
			{
				XMLNode current_lang = languages_node.GetNodeList(language_key).Pop();
				string tag = current_lang.GetValue("@tag");
				SystemLanguage lang = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), language_key);
				
				if(!languageTags.ContainsKey(tag))
					languageTags.Add(tag, lang);
			}
		}
		languageTags.Add("", SystemLanguage.English);
	}
	                               

	public static void ParseText(XMLNode text_node)
	{
		XMLNodeList lines = text_node.GetNodeList("Line");
		while((lines != null) && (lines.Count > 0))
		{
			XMLNode current_line = lines.Pop();
			string text = current_line.GetValue("@text");
			foreach(string cur_line_key in current_line.Keys)
			{
				if((cur_line_key == "_name") || (cur_line_key == "_text") || (cur_line_key == "@text"))
					continue;
				
				string localized = "";
				if(languageTags.ContainsKey(cur_line_key))
				{
					XMLNode localized_node = current_line.GetNodeList(cur_line_key).Pop();
					if(localized_node != null)
					{
						localized = localized_node.GetValue("_text");
					}
					AddString(text, localized, languageTags[cur_line_key]);
				}
			}
		}
	}
	
	static void InitStringsHard()
	{
		strings = new Dictionary<string, Dictionary<SystemLanguage, string>>();
	}
	
	public static void AddString(string type, string str = "", SystemLanguage language = SystemLanguage.English)
	{
		if(str == "")
			str = type;
		if(!strings.ContainsKey(type))
			strings.Add(type, new Dictionary<SystemLanguage, string>());
		if(!strings[type].ContainsKey(language))
		{
			strings[type].Add(language, str);
		}
	}

	public static bool HasString(string str)
	{
		return strings.ContainsKey(str);
	}

	public static bool HasString(string str, SystemLanguage lang)
	{
		if(strings.ContainsKey(str))
			return strings[str].ContainsKey(lang);
		else
			return false;
	}

	public static string GetString(string str)
	{
		SystemLanguage language = SafeConvertLanguage(Application.systemLanguage); 

		if(iOS_CustomLanguage)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer)
				language = defaultLanguage;
		}
		else
		{
			language = defaultLanguage;
		}

		if(Application.isEditor)
		{
			language = DebugLanguage;
		}

		return GetString(str, language);
	}
	
	static bool IsEastLanguage(SystemLanguage language)
	{
		if((language == SystemLanguage.Japanese) || (language == SystemLanguage.Chinese) || (language == SystemLanguage.Korean))
			return true;
		else
			return false;
	}
	
	public static string GetString(string str, SystemLanguage language)
	{
		if(strings.ContainsKey(str))
		{
			if(strings[str].ContainsKey(language))
			{
				return strings[str][language];
			}
			else
			{
				if(strings[str].ContainsKey(defaultLanguage))
					return strings[str][defaultLanguage];
				else
					return strings[str][SystemLanguage.English];
			}
		}
		else
		{
			string error_msg = "String '" + str + "' not found (dl: " + DefLangLoaded + ")";
			//if(DebugMode) CDebug.LogError("Localizator: " + error_msg);
			return error_msg;
		}
	}

	public static string Localize(string source)
	{
		string pattern = @"(?<=\[)(\w*\s*\d*)*(?=\])";
		string locText = source;
		MatchCollection matches = Regex.Matches(source, pattern);
		
		int offset = 0;
		foreach (Match match in matches)
		{
			string loc = Localizator.GetString(match.Value);
			locText = locText.Substring(0, match.Index - 1 - offset) + loc + locText.Substring(match.Index + match.Length + 1 - offset);
			offset += loc.Length - match.Length - 2;
		}
		return locText;
	}
}

