#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AMEditor
{
	public class CustomCodeCollector
	{
//		public static List<string> CollectExportFiles ()
//		{
//			bool result = false;
//
//			List<Plugin> newPlugins = new List<Plugin> ();
//			List<Plugin> otherPlugins = new List<Plugin> ();
//
//			WindowAMEditor.SilentCheckUpdate ();
//
//			ArrayList currentPluginsList = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (WindowAMEditor.AMEDITOR_CONFIG_FILENAME)) as ArrayList;
//
//			foreach (var item in currentPluginsList) 
//			{
//				Plugin pluginItem = new Plugin (item as Hashtable);
//				if (pluginItem.errors.oldVersion || pluginItem.errors.needUpdate)
//				{
//					newPlugins.Add (pluginItem);
//				}
//				else if (!pluginItem.errors.conflict && !pluginItem.errors.dependPlugins && !pluginItem.errors.missingFiles &&
//					!pluginItem.errors.missingFilesHash && !pluginItem.errors.needUpdate && !pluginItem.errors.oldVersion)
//				{
//					otherPlugins.Add (pluginItem);
//				}
//				else
//				{
//					EditorUtility.DisplayDialog ("Error!", "Plugins have some errors!", AMEditorSystem.ContentStandardButton._Ok);
////					return false;
//				}
//			}
//
//			//			- для не требующих апдейта поиск конфигов в локальном репозитории и сравнение их версий с актуальными
//
//			//			- подходящие конфиги копируем в проект
//
//			//			- при нехватке актуальных конфигов - дергаем их с гита, без файлов, сразу в проект
//
//			//			- собираем по этим конфигам файлы в пак
//
//			return new List<string> ();
//		}
	}
}
#endif