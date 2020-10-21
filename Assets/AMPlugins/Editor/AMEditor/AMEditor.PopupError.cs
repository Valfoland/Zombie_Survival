#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

namespace AMEditor
{
	public class AMEditorPopupErrorWindow : EditorWindow
	{
		private static string popupTitle = "Error Popup";
		private static string popupMessage = "Нет ошибок.";
		
		public static event System.Action CopyToProject;
		
		private static bool showAccessErrorPopup = false;
		
		public static string FILE_NAME_STATE_ERROR = "ameditor_state_error.json";
		
		private Vector2 scrollPosition;
		
		public static AMEditorPopupErrorWindow instance = null;

		static void InitErrorPopup ()
		{
			if (showAccessErrorPopup) {
				new AMEditor.UI.AMDisplayDialog (popupTitle, popupMessage, "Создать каталог", "Загружать в проект", () => {}, () => {}, true).Show ();
			} else if (!showAccessErrorPopup){
				new AMEditor.UI.AMDisplayDialog (popupTitle, popupMessage, AMEditorSystem.ContentStandardButton._Ok, "", () => {}, () => {}, true).Show ();
			}
			
			CreateState ();
		}
		
		public static void ShowAccessErrorPopup (string errorMessage)
		{
			showAccessErrorPopup = true;
			
			popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "205";
			popupMessage = errorMessage;
			
			
			InitErrorPopup ();
		}
		
		void OnDestroy ()
		{
			AMEditorFileStorage.RemoveFile (FILE_NAME_STATE_ERROR);
		}
		
		void OnFocus ()
		{
			if (instance == null) 
			{
				instance = this;
			}
			LoadState ();
			
		}
		
		static void LoadState ()
		{
			try 
			{
				var json = AMEditorJSON.JsonDecode( AMEditorFileStorage.ReadTextFile (FILE_NAME_STATE_ERROR)) as Hashtable;
				popupTitle = (string)json ["popup_title"];
				popupMessage = (string)json ["popup_message"];
				
			}
			catch (Exception) 
			{}
		}
		static void CreateState ()
		{
			try 
			{
				var hash = new Hashtable ();
				hash.Add ("popup_title", popupTitle);
				hash.Add ("popup_message", popupMessage);
				
				File.WriteAllText (FILE_NAME_STATE_ERROR, AMEditorJSON.JsonEncode (hash));
			}
			catch (Exception) 
			{}
		}
		
		void OnLostFocus ()
		{
			
		}
		
		public static void ShowErrorPopup (string errorCode, string errorMessage)
		{			
			showAccessErrorPopup = false;
			
			if (errorCode == "") {
				//ответы от Git в формате {message : 401 Unauthorized}
				if (errorMessage == "" || errorMessage == null) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "100";
					popupMessage = AMEditorSystem.WebError._100;
				} else if (errorMessage.Contains ("404")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "104";
					popupMessage = AMEditorSystem.WebError._104("");
				} else if (errorMessage.Contains ("400")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "106";
					popupMessage = AMEditorSystem.WebError._106("");
				} else if (errorMessage.Contains ("403")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "107";
					popupMessage = AMEditorSystem.WebError._107("");
				}
				else if (errorMessage.Contains ("500")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "110";
					popupMessage = AMEditorSystem.WebError._110;
				}
				else if (errorMessage.Contains ("401")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "101";
					popupMessage = AMEditorSystem.WebError._101;
					//popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "118";
					//popupMessage = AMEditorSystem.WebError._118;
				}
				//exception при выполнении методов AMEditorNetwork
				else if (errorMessage.Contains ("timed out")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "408";
					popupMessage = AMEditorSystem.HTTPClientError._408 ("");
				} else if (errorMessage.Contains ("NameResolutionFailure")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "110";
					popupMessage = AMEditorSystem.WebError._110;
				} else if (errorMessage.Contains ("ConnectFailure")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "110";
					popupMessage = AMEditorSystem.WebError._110;
				} else if (errorMessage.Contains ("NotSupportedException")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "111";
					popupMessage = AMEditorSystem.WebError._111;
				} else if (errorMessage.Contains ("ArgumentNullException")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "112";
					popupMessage = AMEditorSystem.WebError._112;
				} else if (errorMessage.Contains ("SecurityException")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "113";
					popupMessage = AMEditorSystem.WebError._113;
				} else if (errorMessage.Contains ("UriFormatException")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "114";
					popupMessage = AMEditorSystem.WebError._114;
				} else if (errorMessage.Contains ("NotImplementedException")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "115";
					popupMessage = AMEditorSystem.WebError._115;
				} else if (errorMessage.Contains ("OutOfMemoryException")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "116";
					popupMessage = AMEditorSystem.WebError._116;
				} else if (errorMessage.Contains ("IOException")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "117";
					popupMessage = AMEditorSystem.WebError._117;
				}
				//остальные возможные ошибки web-клиента
				else if (errorMessage.Contains ("405")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "405";
					popupMessage = AMEditorSystem.HTTPClientError._405 ("");
				} else if (errorMessage.Contains ("406")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "406";
					popupMessage = AMEditorSystem.HTTPClientError._406 ("");
				} else if (errorMessage.Contains ("407")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "407";
					popupMessage = AMEditorSystem.HTTPClientError._407 ("");
				} else if (errorMessage.Contains ("408")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "408";
					popupMessage = AMEditorSystem.HTTPClientError._408 ("");
				} else if (errorMessage.Contains ("409")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "409";
					popupMessage = AMEditorSystem.HTTPClientError._409 ("");
				} else if (errorMessage.Contains ("410")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "410";
					popupMessage = AMEditorSystem.HTTPClientError._410 ("");
				} else if (errorMessage.Contains ("411")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "411";
					popupMessage = AMEditorSystem.HTTPClientError._411 ("");
				} else if (errorMessage.Contains ("412")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "412";
					popupMessage = AMEditorSystem.HTTPClientError._412 ("");
				} else if (errorMessage.Contains ("413")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "413";
					popupMessage = AMEditorSystem.HTTPClientError._413 ("");
				} else if (errorMessage.Contains ("414")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "414";
					popupMessage = AMEditorSystem.HTTPClientError._414 ("");
				} else if (errorMessage.Contains ("415")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "415";
					popupMessage = AMEditorSystem.HTTPClientError._415 ("");
				} else if (errorMessage.Contains ("416")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "416";
					popupMessage = AMEditorSystem.HTTPClientError._416 ("");
				} else if (errorMessage.Contains ("417")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "417";
					popupMessage = AMEditorSystem.HTTPClientError._417 ("");
				} else if (errorMessage.Contains ("418")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "418";
					popupMessage = AMEditorSystem.HTTPClientError._418 ("");
				} else if (errorMessage.Contains ("422")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "422";
					popupMessage = AMEditorSystem.HTTPClientError._422 ("");
				} else if (errorMessage.Contains ("423")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "423";
					popupMessage = AMEditorSystem.HTTPClientError._423 ("");
				} else if (errorMessage.Contains ("424")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "424";
					popupMessage = AMEditorSystem.HTTPClientError._424 ("");
				} else if (errorMessage.Contains ("425")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "425";
					popupMessage = AMEditorSystem.HTTPClientError._425 ("");
				} else if (errorMessage.Contains ("426")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "426";
					popupMessage = AMEditorSystem.HTTPClientError._426 ("");
				} else if (errorMessage.Contains ("428")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "428";
					popupMessage = AMEditorSystem.HTTPClientError._428 ("");
				} else if (errorMessage.Contains ("429")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "429";
					popupMessage = AMEditorSystem.HTTPClientError._429 ("");
				} else if (errorMessage.Contains ("431")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "431";
					popupMessage = AMEditorSystem.HTTPClientError._431 ("");
				} else if (errorMessage.Contains ("434")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "434";
					popupMessage = AMEditorSystem.HTTPClientError._434 ("");
				} else if (errorMessage.Contains ("449")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "449";
					popupMessage = AMEditorSystem.HTTPClientError._449 ("");
				} else if (errorMessage.Contains ("451")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "451";
					popupMessage = AMEditorSystem.HTTPClientError._451 ("");
				} else if (errorMessage.Contains ("456")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "456";
					popupMessage = AMEditorSystem.HTTPClientError._456 ("");
				} else if (errorMessage.Contains ("499")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "499";
					popupMessage = AMEditorSystem.HTTPClientError._499 ("");
				}
				//остальные возможные ошибки web-сервера
				else if (errorMessage.Contains ("501")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "501";
					popupMessage = AMEditorSystem.HTTPServerError._501 ("");
				} else if (errorMessage.Contains ("502")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "502";
					popupMessage = AMEditorSystem.HTTPServerError._502 ("");
				} else if (errorMessage.Contains ("503")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "503";
					popupMessage = AMEditorSystem.HTTPServerError._503 ("");
				} else if (errorMessage.Contains ("504")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "504";
					popupMessage = AMEditorSystem.HTTPServerError._504 ("");
				} else if (errorMessage.Contains ("505")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "505";
					popupMessage = AMEditorSystem.HTTPServerError._505 ("");
				} else if (errorMessage.Contains ("506")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "506";
					popupMessage = AMEditorSystem.HTTPServerError._506 ("");
				} else if (errorMessage.Contains ("507")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "507";
					popupMessage = AMEditorSystem.HTTPServerError._507 ("");
				} else if (errorMessage.Contains ("508")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "508";
					popupMessage = AMEditorSystem.HTTPServerError._508 ("");
				} else if (errorMessage.Contains ("509")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "509";
					popupMessage = AMEditorSystem.HTTPServerError._509 ("");
				} else if (errorMessage.Contains ("510")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "510";
					popupMessage = AMEditorSystem.HTTPServerError._510 ("");
				} else if (errorMessage.Contains ("511")) {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + "511";
					popupMessage = AMEditorSystem.HTTPServerError._511 ("");
				} 
				//неизвестные ошибки сразу в popup
				else {
					popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup;
					popupMessage = errorMessage;
				}
			} else {//вывод своих ошибок с кодом напрямую в popup
				popupTitle = AMEditorSystem.ContentOtherWindow._TitleErrorPopup + errorCode;
				popupMessage = errorMessage;
			}
			InitErrorPopup ();
		}
	}
}
#endif