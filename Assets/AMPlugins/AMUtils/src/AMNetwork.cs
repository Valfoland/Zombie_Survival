#pragma warning disable
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using AMLogging;

namespace AMUtils
{
	public class AMNetwork : MonoBehaviour
	{
		public static AMNetwork Instance = null;
		
		public static bool printDebug = false;

		public static bool stopDownload;
		public static string postRequestResult = "";

		public static event Action ErrorWebDownload;
		
		static AMLogger amLogger = AMLogger.GetInstance ("AMNetwork: ");

		void Awake()
		{
			if (Instance == null)
			{				
				Instance = this;
				DontDestroyOnLoad(gameObject);

				stopDownload = false;
			}
			else
				Destroy(gameObject);
		}
		
		public static bool IsConnected()
		{
			#if !UNITY_WINRT 
			bool internetPossiblyAvailable = false;
			
			switch (Application.internetReachability) {
			case NetworkReachability.ReachableViaLocalAreaNetwork:
				internetPossiblyAvailable = true;
				break;
			case NetworkReachability.ReachableViaCarrierDataNetwork:
				internetPossiblyAvailable = true;
				break;
			default:
				internetPossiblyAvailable = false;
				break;
				
			}
			return internetPossiblyAvailable;
			#else
			return true;
			#endif
		}
		
		public static string GetRequest (string url, string urlParams) {
#if !UNITY_WINRT || UNITY_EDITOR
			if (printDebug == true) 
				Debug.LogWarning ("Request to send : GET "+ url + urlParams);			
	
			if (url.Contains("gitlab"))
				ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
			
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (url + urlParams);
			request.UserAgent = "User-Agent=Mozilla/5.0 Firefox/1.0.7";
			request.ContentType = "application/x-www-form-urlencoded";
			request.Timeout = 10000;
			string result = string.Empty;
			HttpWebResponse response = null;
			
			try
			{
				response = (HttpWebResponse)request.GetResponse ();
			
				Stream stream = response.GetResponseStream ();
				StreamReader streamReader = new StreamReader (stream, Encoding.UTF8);
				result = streamReader.ReadToEnd ();
				streamReader.Close ();
			}
			catch(WebException ex)
			{
				//Debug.Log ("!!!qwewqe");
				
				if(ErrorWebDownload != null)
					ErrorWebDownload();
			}

			return result;
#else
			return "";
#endif
		}
		
		public static string FileGetRequest (string url, string urlParams, string filePathToDownload){//string filePathToDownload, string fileName) {
#if !UNITY_WINRT || UNITY_EDITOR
			if (printDebug == true) 
				Debug.LogWarning ("Request to get files : GET "+url+urlParams);
			
			if (url.Contains("gitlab"))
				ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + urlParams);
			request.Timeout = 300000;
			request.UserAgent = "User-Agent=Mozilla/5.0 Firefox/1.0.7";
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			
			Stream stream = response.GetResponseStream();
			const int size = 262144;
			byte[] bytes = new byte[262144];
			int numBytes;
			using (FileStream fileStream = new FileStream(filePathToDownload, FileMode.Create, FileAccess.Write))
			while ((numBytes = stream.Read(bytes, 0, size)) > 0) {
				if (stopDownload) {
					fileStream.Close ();
					fileStream.Dispose ();
					if (File.Exists(filePathToDownload))
						File.Delete (filePathToDownload);
				}
				else
					fileStream.Write (bytes, 0, numBytes);
			}
			string result = "";
			
			if(!stopDownload)
				result = "Downloaded file : "+ filePathToDownload;
			
			stopDownload = false;
			
			return result;
#else
			return "";
#endif
		}
		
		public struct UrlParam
		{
			public string name;
			public string value;
		}
		
		public static string PostRequest (string url, List<AMUtils.AMNetwork.UrlParam> urlParams) {
			WWWForm form = new WWWForm();
			for (int i = 0; i < urlParams.Count; i++)
				form.AddField(urlParams[i].name, urlParams[i].value);
			
			WWW w = new WWW(url, form);

			while (!w.isDone) {}

			if (w.isDone) {
				if (printDebug == true) 
					amLogger.Log("Auth result : "+w.text);
			}
			else if (w.error == null)
				amLogger.LogError(w.text);
			
			postRequestResult = w.text;

			return w.text;
		}

		public static IEnumerator PostRequestCoroutine (string url, List<AMUtils.AMNetwork.UrlParam> urlParams) {
			WWWForm form = new WWWForm();
			for (int i = 0; i < urlParams.Count; i++)
				form.AddField(urlParams[i].name, urlParams[i].value);
			
			WWW w = new WWW(url, form);
			yield return w;
			
			if (w.isDone) {
				if (printDebug == true) 
					amLogger.Log("Auth result : "+w.text);
			}
			else if (w.error == null)
				amLogger.LogError(w.text);
			
			postRequestResult = w.text;
		}

		public static void StopDownload ()
		{
			stopDownload = true;
		}
	}
}