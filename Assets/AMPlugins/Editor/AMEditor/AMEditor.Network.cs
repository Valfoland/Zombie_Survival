using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
#if !UNITY_5_5_OR_NEWER
using AMEditorMailDll;
#endif
using AMEditor;

namespace AMEditor
{
	public class AMEditorNetwork : MonoBehaviour
	{
		public static AMEditorNetwork Instance = null;
	
		#if AM_EDITOR_DEBUG_MODE_ON
		public static bool printDebug = true;
		#else
		public static bool printDebug = false;
		#endif

		public static bool stopDownload;
		public static string postRequestResult = string.Empty;
		public static string putRequestResult = string.Empty;
		public static string requestResult = string.Empty;

	#if !UNITY_5_5_OR_NEWER
		#if AM_EDITOR_LANGUAGE_EN
		static string MAIL_SUBJECT = "New commit to AM Editor Plugins";
		static string MAIL_TEXT = " just pushed the new commit to the AM Editor Plugins Localgit repo.\n";
		static string BRANCH_LABEL = "Branch: ";
		static string COMMIT_MESSAGE_LABEL = "Commit message: ";
		#else
		static string MAIL_SUBJECT = "Новый коммит в AM Editor Plugins";
		static string MAIL_TEXT = " только что запушил новый коммит в репозиторий AM Editor Plugins в Localgit.\n";
		static string BRANCH_LABEL = "Ветка: ";
		static string COMMIT_MESSAGE_LABEL = "Сообщение к коммиту: ";
		#endif

		static string domain = string.Empty;
	#endif
		static protected List<string> recievers;

		void Awake ()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad (gameObject);

				stopDownload = false;
			}
			else
				Destroy (gameObject);
		}
		
		public static bool IsConnected ()
		{
			#if !UNITY_WINRT
			bool internetPossiblyAvailable = false;
			
			switch (Application.internetReachability) 
			{
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
		
		public static string GetRequest (string url, string urlParameters) 
		{
			if (printDebug) 
				Debug.LogWarning ("Request to send : GET " + url + urlParameters);			
			try 
			{
				if (url.Contains ("gitlab"))
					ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
			
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (url + urlParameters);
				request.UserAgent = "User-Agent=Mozilla/5.0 Firefox/1.0.7";
				request.ContentType = "application/x-www-form-urlencoded";
				request.Timeout = 10000;
				string result = string.Empty;
				HttpWebResponse response = null;
				response = (HttpWebResponse)request.GetResponse ();
			
				Stream stream = response.GetResponseStream ();
				StreamReader streamReader = new StreamReader (stream, Encoding.UTF8);
				result = streamReader.ReadToEnd ();
				streamReader.Close ();

				return result;
			} 
			catch (Exception ex) 
			{
				return ex.ToString ();
			}
		}
		
		public static string FileGetRequest (string url, string urlParameters, string filePathToDownload)
		{
			if (printDebug) 
				Debug.LogWarning ("Request to get files : GET " + url + urlParameters);
			try 
			{
				if (url.Contains ("gitlab"))
					ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
			
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url + urlParameters);
				request.Timeout = 300000;
				request.UserAgent = "User-Agent=Mozilla/5.0 Firefox/1.0.7";
				HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
			
				Stream stream = response.GetResponseStream ();
				const int size = 262144;
				byte[] bytes = new byte[262144];
				int numBytes;
				using (FileStream fileStream = new FileStream (filePathToDownload, FileMode.Create, FileAccess.Write))
				while ((numBytes = stream.Read (bytes, 0, size)) > 0) 
				{
					if (stopDownload) 
					{
						fileStream.Close ();
						fileStream.Dispose ();
						if (File.Exists (filePathToDownload))
							File.Delete (filePathToDownload);
					}
					else
						fileStream.Write (bytes, 0, numBytes);
				}
				string result = "";
			
				if (!stopDownload)
					result = "Downloaded file : " + filePathToDownload;
				else
					result = "";
			
				stopDownload = false;
			
				return result;
			} 
			catch (Exception ex) 
			{
				return ex.ToString ();
			}
		}
		
		public struct UrlParameter
		{
			public string name;
			public string value;
		}

		public static string PutRequest (string url, List<UrlParameter> urlParameters)
		{
			UrlParameter contentParameter = urlParameters.Find ((p) => {return p.name == "content";});
			string paramsString = "?";
			string contentParameterString = "&" + contentParameter.name+ "=" + contentParameter.value;

			foreach (var item in urlParameters)
			{
				if (item.name != contentParameter.name)
				{
					paramsString += item.name + "=" + item.value + "&";
				}
			}
			paramsString = paramsString.Remove (paramsString.Length - 1);

			if (printDebug)
				Debug.LogWarning ("Request to push file : PUT " + url + paramsString);

			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create (url + paramsString);
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
				httpWebRequest.Method = "PUT";

				using (var streamWriter = new StreamWriter (httpWebRequest.GetRequestStream ()))
				{
					streamWriter.Write (contentParameterString);
				}
				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
				using (var streamReader = new StreamReader (httpResponse.GetResponseStream ()))
				{
					putRequestResult = streamReader.ReadToEnd ();

					if (printDebug)
						Debug.Log (putRequestResult);

					return putRequestResult;
				}
			}
			catch (Exception ex)
			{
				if (printDebug)
					Debug.Log (ex.ToString ());
				
				return ex.ToString ();
			}
		}
		
		public static string PostRequest (string url, List<UrlParameter> urlParameters) 
		{
			WWWForm form = new WWWForm ();
			foreach (var p in urlParameters) 
			{
				form.AddField (p.name, p.value);
			}

			WWW w = new WWW (url, form);

			while (!w.isDone) {}
			
			if (w.isDone) 
			{
				if (printDebug) 
					Debug.Log ("Auth result : " + w.text);
				postRequestResult = w.text;
			} 
			else if (w.error != null || w.error != String.Empty) 
			{
				Debug.LogError (w.text);
				postRequestResult = w.error;
			} 
			else
				postRequestResult = w.text;
			
			return postRequestResult;
		}

		public static void SendMessage (string branch, string message, string url)
		{
			#if !UNITY_5_5_OR_NEWER
			recievers = new List<string> ();
			string aboutConfigString = string.Empty;

			if (AMEditorFileStorage.FileExist (LocalRepositoryAPI.pathToRepository + "/" + HelpAPI.NAME_FILE_MENU_ABOUT)) 
			{
				aboutConfigString = AMEditorFileStorage.ReadTextFile (LocalRepositoryAPI.pathToRepository + "/" + HelpAPI.NAME_FILE_MENU_ABOUT);
			}
			else
			{
				aboutConfigString = AMEditorGit.RequestGet (HelpAPI.GetLinkPublicRepo ());
				File.WriteAllText (LocalRepositoryAPI.pathToRepository + "/" + HelpAPI.NAME_FILE_MENU_ABOUT, aboutConfigString);
			}

			Hashtable about_config = AMEditorJSON.JsonDecode (aboutConfigString) as Hashtable;
			ArrayList support_mails = about_config ["support_mails"] as ArrayList;

			domain = GitAccount.current.server.Substring (16, 15);

			foreach (var address in support_mails)
			{
				recievers.Add (address.ToString ());
			}
			if (recievers.Count == 0)
			{
				recievers.Add ("artem.farafonov@" +domain);
				Debug.Log ("email from json failed");
			}
			var user = Mail.Auth ();

			try
			{
				MailMessage mail = new MailMessage ();
				mail.From = new MailAddress (user.Login + "@" + domain, "AM Editor");
				foreach (var u in recievers)
				{
					mail.To.Add (u);
				}
				mail.Subject = MAIL_SUBJECT;
				mail.Body = GitAccount.current.name + MAIL_TEXT +
							"\n" +
							url + "\n" +
							"\n" +
							BRANCH_LABEL+branch+ "\n" +
							COMMIT_MESSAGE_LABEL+message;
				
				SmtpClient client = new SmtpClient ("smtp." + domain);
				client.Port = 587;
				client.Credentials = new System.Net.NetworkCredential (user.Login + "@" + domain, user.Pass) as ICredentialsByHost;
				client.EnableSsl = true;

				ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
				{
					return true;
				};

				client.Send (mail);

				if (printDebug)
					Debug.Log ("Message sent");
			}
			catch (Exception ex)
			{
				if (printDebug)
					Debug.Log (ex.ToString ());
			}
			#endif
		}

		public static IEnumerator RequestCoroutine (string url, string urlParameters) 
		{
			if (printDebug) 
				Debug.LogWarning ("Request to send : GET " + url + urlParameters);

			WWW w = new WWW (url + urlParameters);
			yield return w;
			
			if (w.isDone) 
			{
				if (printDebug) 
					Debug.Log ("Request result : " +w.text);
			}
			else if (w.error == null)
				Debug.LogError (w.text);
			
			requestResult = w.text;
		}

		public static void StopDownload ()
		{
			stopDownload = true;
		}

		void OnDestroy ()
		{
			recievers = null;
			postRequestResult = string.Empty;
			putRequestResult = string.Empty;
			requestResult = string.Empty;
			#if !UNITY_5_5_OR_NEWER
			domain = string.Empty;
			#endif
		}
	}
}