#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

namespace AMEditor
{
	public class AuthWindow : EditorWindow
	{
		public static string[] listServer = new string[]{"http://pgit.digital-ecosystems.ru"};
		public static string[] authTypeList = new string[]{"LDAP", "Git Token"};
		public static int indexTypeAuth = 0;
		public static int indexServer = 0;

		static string login = string.Empty;
		static string password = string.Empty;
		static string loginString = string.Empty;

		static bool showPassword = false;
		public static string privateTokenEditBox = "";

		public static List<GitAccount> accounts;

		static GUIStyle messageStyle;
		static GUIStyle serverLabelStyle;
		static GUIStyle labelStyle;
		static GUIStyle textFieldStyle;
		static GUIStyle textFieldButtonStyle;
		static GUIStyle loginTextStyle;

		static float authAreaW = 310;
		static float authAreaH = 166;
		static float authAreaMarginMain = 5;
		static float authAreaMarginIn = 8;
		static float elementHeight = 22;
		static float labelWidth = 77;
		static Texture showPassTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_show.png"); 
		static Texture hidePassTexture = AssetDatabase.LoadAssetAtPath<Texture> ("Assets/AMPlugins/Editor/AMEditor/src/am_editor_hide.png");

		static void SaveAccounts ()
		{
			if ((accounts != null) && (accounts.Count > 0))
			{
				GitAccount.current = accounts[0];
			}
		}

		static GitAccount Authorization ()
		{
			var git = new AMEditorGit ();
			#if AM_EDITOR_DEBUG_MODE_ON
				git.printDebug = true;
			#else
				git.printDebug = false;
			#endif
			//TODO git.AuthFailed += AuthFailedHandler;
			if (indexTypeAuth == 0)
			{
				if (login.Contains ("@"))
				{
					login = login.Substring (0, login.IndexOf ("@"));
				}
				privateTokenEditBox = git.AuthByLoginAndPass (listServer [indexServer], login, password);
			}
			else
			{
				git.AuthByPT (listServer [indexServer], privateTokenEditBox);
			}
			
			var newAcc = new GitAccount ();
			if (privateTokenEditBox != string.Empty)
			{
				try 
				{
					newAcc.name = (string)((AMEditorJSON.JsonDecode (git.GetUserInfo (privateTokenEditBox)) as Hashtable)["name"]);
					newAcc.privateToken = privateTokenEditBox;
					newAcc.server = listServer [indexServer];
					
					accounts = new List<GitAccount> ();
					accounts.Add (newAcc);
					SaveAccounts ();	
				} 
				catch (System.Exception) 
				{
					//AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("101", "Git Unavailable.");
					return null;
				}
			}
			else
			{
				//AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("100", "Git Unavailable.");
				return null;
				//error
			}
			return newAcc;
		}

		public static void DrawGUI ()
		{
			if (messageStyle == null)
			{
				messageStyle = new GUIStyle (GUI.skin.label);
				messageStyle.fontSize = 16;
				messageStyle.alignment = TextAnchor.MiddleCenter;
			}

			if (serverLabelStyle == null)
			{
				serverLabelStyle = new GUIStyle (GUI.skin.label);
				serverLabelStyle.alignment = TextAnchor.MiddleCenter;
			}

			if (labelStyle == null)
			{
				labelStyle = new GUIStyle (GUI.skin.label);
				labelStyle.alignment = TextAnchor.MiddleLeft;
			}

			if (textFieldStyle == null)
			{
				textFieldStyle = new GUIStyle (GUI.skin.textField);
				textFieldStyle.alignment = TextAnchor.MiddleLeft;
			}

			if (textFieldButtonStyle == null)
			{
				textFieldButtonStyle = new GUIStyle (GUI.skin.box);
				textFieldButtonStyle.active.background = new GUIStyle (GUI.skin.textField).normal.background;
				textFieldButtonStyle.alignment = TextAnchor.MiddleCenter;
				textFieldButtonStyle.fontStyle = FontStyle.Bold;
			}

			Rect authAreaRect = new Rect ((Screen.width / 2) - (authAreaW / 2), (Screen.height / 2) - (authAreaH / 2), authAreaW, authAreaH);

			GUI.Box (authAreaRect, "");

			GUI.Label (new Rect (authAreaRect.x, authAreaRect.y, authAreaRect.width, elementHeight), listServer[indexServer], serverLabelStyle);

			indexTypeAuth = GUI.SelectionGrid (new Rect (authAreaRect.x + authAreaMarginMain, authAreaRect.y + elementHeight + 2, authAreaRect.width - (authAreaMarginMain * 2), elementHeight), 
				indexTypeAuth, authTypeList, 2);

			Rect innerBoxRect = new Rect (authAreaRect.x, authAreaRect.y + 44, authAreaRect.width, authAreaRect.height - 44);

			GUI.Box (innerBoxRect, "");

			float textFieldX = authAreaRect.x + authAreaMarginIn + labelWidth;
			float textFieldW = authAreaRect.width - labelWidth - (authAreaMarginIn * 2);
			float textFieldWithButtonW = authAreaRect.width - labelWidth - elementHeight - (authAreaMarginIn * 2);
			float textFieldButtonX = textFieldX + textFieldWithButtonW;
			float authButtonY = authAreaRect.y + authAreaRect.height - 38;
			float authButtonW = authAreaRect.width - (authAreaMarginMain * 2);

			float inputAreaH = authButtonY - innerBoxRect.y;
			float loginLineY = innerBoxRect.y + (inputAreaH / 2) - elementHeight - 5;
			float passLineY = loginLineY + elementHeight + 10;
			float tokenLineY = innerBoxRect.y + (inputAreaH / 2) - (elementHeight / 2);
			switch (indexTypeAuth)
			{
			case 0:
				if (loginTextStyle == null)
				{
					loginTextStyle = new GUIStyle (GUI.skin.textField);
					loginTextStyle.alignment = TextAnchor.MiddleLeft;
					loginTextStyle.fontSize = textFieldStyle.fontSize;
				}

				GUI.Label (new Rect (authAreaRect.x + authAreaMarginIn, loginLineY, labelWidth, elementHeight), 
					new GUIContent (AMEditorSystem.ContentAuth._Login, AMEditorSystem.ContentAuth._LoginHelp), labelStyle);
				
				GUI.SetNextControlName ("loginField");
				loginString = GUI.TextField (new Rect (textFieldX, loginLineY, textFieldW, elementHeight), loginString, loginTextStyle);

				GUI.Label (new Rect (authAreaRect.x + authAreaMarginIn, loginLineY + elementHeight + 10, labelWidth, elementHeight), 
					new GUIContent (AMEditorSystem.ContentAuth._Password, AMEditorSystem.ContentAuth._PasswordHelp), labelStyle);
				if (showPassword)
				{
					GUI.SetNextControlName ("passwordField");
					password = GUI.TextField (new Rect (textFieldX, passLineY, textFieldWithButtonW, elementHeight), password, textFieldStyle);
				}
				else
				{
					GUI.SetNextControlName ("passwordField");
					password = GUI.PasswordField (new Rect (textFieldX, passLineY, textFieldWithButtonW, elementHeight), password, '*', textFieldStyle);
				}
				if (GUI.Button (new Rect (textFieldButtonX, passLineY, elementHeight, elementHeight), 
					showPassword ? showPassTexture : hidePassTexture, textFieldButtonStyle))
				{
					showPassword = !showPassword;
				}
				break;
			case 1:
				GUI.Label (new Rect (authAreaRect.x + authAreaMarginIn, tokenLineY, labelWidth, elementHeight), 
					new GUIContent (AMEditorSystem.ContentAuth._PrivateToken, AMEditorSystem.ContentAuth._PrivateTokenHelp), labelStyle);
				
				GUI.SetNextControlName ("privateTokenField");
				privateTokenEditBox = GUI.TextField (new Rect (textFieldX, tokenLineY, textFieldWithButtonW, elementHeight), privateTokenEditBox, textFieldStyle);

				if (GUI.Button (new Rect (textFieldButtonX, tokenLineY, elementHeight, elementHeight), 
					new GUIContent ("?", AMEditorSystem.ContentAuth._PrivateTokenHelp), textFieldButtonStyle))
				{
					Application.OpenURL ("http://pgit.digital-ecosystems.ru/profile/account");
				}
				break;
			default:
				break;
			}

			if (GUI.Button (new Rect (authAreaRect.x + authAreaMarginMain, authButtonY, authButtonW, elementHeight - 2), 
				new GUIContent (AMEditorSystem.ContentAuth._Auth, AMEditorSystem.ContentAuth._AuthHelp)))
			{
				login = loginString;
				var current = Authorization ();
				if (current!=null)
				{
					AMEditor.WindowMain.ChangeCurrentWindowToListPlugins ();
				}
				else
				{
					AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("101", AMEditor.AMEditorSystem.WebError._101);
				}	
			}

			var e = UnityEngine.Event.current;
			#if UNITY_EDITOR_OSX
			if (e.command && e.keyCode == KeyCode.V)
			#else
			if (e.control && e.keyCode == KeyCode.V)
			#endif
			{	
				string nameCurrentFocus = GUI.GetNameOfFocusedControl ();
				switch (nameCurrentFocus) 
				{
				case "passwordField":
					password = EditorGUIUtility.systemCopyBuffer;
					break;
				case "loginField":
					login = EditorGUIUtility.systemCopyBuffer;
					break;
				case "privateTokenField":
					privateTokenEditBox = EditorGUIUtility.systemCopyBuffer;
					break;
				default:
					break;
				}
			}
		}
	}
}
#endif