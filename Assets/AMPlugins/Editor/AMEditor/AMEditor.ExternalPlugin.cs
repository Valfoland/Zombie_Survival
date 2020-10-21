#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace AMEditor
{
	class ExternalPlugin
	{
		public string name;
		public string version;
		public string uriDescription;
		public string uriDownload;

		private static string defaultName = "PluginName";
		private static string defaultUrl = "http://pgit.digital-ecosystems.ru";

		public ExternalPlugin ()
		{
			name = defaultName;
			uriDescription = defaultUrl;
			uriDownload = defaultUrl;
			version = string.Empty;
		}

		public ExternalPlugin (Hashtable source)
		{
			try 
			{
				name = (string)source["name"];
			} 
			catch (Exception) 
			{
				name = defaultName;
			}

			try 
			{
				uriDescription = (string)source["url_description"];
			} 
			catch (Exception) 
			{
				uriDescription = defaultUrl;
			}

			try 
			{
				uriDownload = (string)source["url_download"];
			} 
			catch (Exception) 
			{
				uriDownload = defaultUrl;
			}

			try 
			{
				version = (string)source["version"];
			} 
			catch (Exception) 
			{
				version = string.Empty;
			}
		}

		public Hashtable ToHashtable ()
		{
			var result = new Hashtable ();

			result.Add ("name", name);
			result.Add ("url_download", uriDownload);
			result.Add ("url_description", uriDescription);
			result.Add ("version", version);

			return result;
		}
	}
}
#endif