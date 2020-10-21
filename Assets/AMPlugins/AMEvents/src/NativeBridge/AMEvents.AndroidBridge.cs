using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace AMEvents
{
	public class AndroidBridge : INativeBridge
	{
		#region Debug
		/// <summary>
		/// The debug mode.
		/// </summary>
		public bool DebugMode
		{
			get;
			set;
		}
        #endregion
		
		#region App Events API
			private const string NEW_SCENE_EVENT = "newScene";
			private const string RESUME_SCENE = "resumeScene";
			private const string PAUSE_SCENE = "pauseScene";
			private const string APPLICATION_INACTIVE = "applicationInactive";
		#endregion
		
		#region Ad
			private const string ADD_AD_LISTENER = "addAdListener";
			private const string REMOVE_AD_LISTENER = "removeAdListener";
			
			private const string ENABLE_CROSS = "enableCross";
			private const string ON_CROSS_CLICKED = "onCrossClicked";

			private const string SET_BANNER_VISIBLE = "setBannerVisible";
			private const string DISABLE_AD = "disableAd";
			private const string ENABLE_AD = "enableAd";
			private const string IS_AD_DISABLED = "isAdDisabled";
			private const string AD_DISABLED_MESSAGE_HANDLER = "AdDisabledMessageHandler";
			private const string AD_ENABLED_MESSAGE_HANDLER = "AdEnabledMessageHandler";
		#endregion
		
		#region Interstitial
			private const string SET_INTERSTITIAL_LISTENER = "setInterstitialListener";
			private const string REMOVE_INTERSTITIAL_LISTENER = "removeInterstitialListener";
			private const string IS_INTERSTITIAL_READY = "isInterstitialReady";
			private const string SET_INTERSTITIAL_DELAY = "setInterstitialDelay";
			private const string GET_CURRENT_INTERSTITIAL_DELAY = "getCurrentInterstitialDelay";
			private const string IS_COMMERCIAL_READY = "isCommercialInterstitialReady";
			private const string SHOW_INTERSTITIAL = "showInterstitial";
			private const string TIME_AFTER_INTERSTITIAL = "timeAfterInterstitial";
		#endregion

		#region Policy
			private const string GET_PRIVACY_URL = "getPrivacyUrl";
			private const string GET_EULA_URL = "getEulaUrl";
			private const string GET_TOS_URL = "getTosUrl";
			private const string GET_REGISTER_TEXT = "getRegisterText";
			private const string SHOW_POLICY_CONTENT = "showPolicyContent";
			private const string SHOW_REGISTER_POLICY_CONTENT = "showRegisterPolicyContent";
			private const string IS_POLICY_READY = "isPolicyReady";
		#endregion
		
		#region Reward Video
			private const string SHOW_REWARD_VIDEO = "showRewardedVideo";
			private const string IS_REWARD_VIDEO_READY = "isRewardedVideoReady";
		#endregion
		
		#region Standard Banner
			private const string GET_BANNER_WIDTH = "getBannerWidth";
			private const string GET_BANNER_HEIGHT = "getBannerHeight";
		#endregion

		#region Inner InApp
			private const string SHOW_INNER_INAPP = "showInnerInApp";
			private const string BUY_INNER_INAPP = "buyInnerInApp";
		#endregion

		#region SocialAPI
		private const string GET_SOCIAL_NET = "getSocialNetworks";
		#endregion


		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass playerActivityContext;
		#endif
		
		public void InitAndroidClass ()
		{	
			#if UNITY_ANDROID && !UNITY_EDITOR
			playerActivityContext = new AndroidJavaClass ("com.ssd.events.Event");
			#endif
		}
		
		/// <summary>
		/// News the scene event.
		/// </summary>
		public void NewSceneEvent ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (playerActivityContext == null)
			{
				InitAndroidClass ();
			}
			if (playerActivityContext != null)
			{
				try
				{
					playerActivityContext.CallStatic (NEW_SCENE_EVENT);
				}
				catch (Exception ex)
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. NewScene. From: com.ssd.events.Event."+NEW_SCENE_EVENT); 
				AMEvents.amLogger.Log ("Type: " +  DebugMode.GetType ().ToString ());
			}
		}
		
		/// <summary>
		/// Resumes the scene event.
		/// </summary>
		public void ResumeSceneEvent ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (playerActivityContext == null)
			{
				InitAndroidClass ();
			}
			if (playerActivityContext != null)
			{
				try
				{
					playerActivityContext.CallStatic (RESUME_SCENE);
				}
				catch (Exception ex)
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. ResumeSceneEvent"); 
			}
		}
		
		/// <summary>
		/// Pauses the scene event.
		/// </summary>
		public void PauseSceneEvent ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (playerActivityContext == null)
			{
				InitAndroidClass ();
			}
			if (playerActivityContext != null)
			{
				try
				{
					playerActivityContext.CallStatic (PAUSE_SCENE);
				}
				catch (Exception ex)
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. PauseSceneEvent"); 
			}
		}
		
		/// <summary>
		/// Applications the inactive event.
		/// </summary>
		public void ApplicationInactiveEvent ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (playerActivityContext == null)
			{
				InitAndroidClass ();
			}
			if (playerActivityContext != null)
			{
				try
				{
					playerActivityContext.CallStatic (APPLICATION_INACTIVE);
				}
				catch (Exception ex)
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. ApplicationInactiveEvent"); 
			}
		}
		
		/// <summary>
		/// Orientations the change event.
		/// </summary>
		/// <param name="orientation">Orientation.</param>
		public void OrientationChangeEvent ()
		{
			string orientation = Screen.orientation.ToString ();
			#if UNITY_ANDROID && !UNITY_EDITOR
			
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. OrientationChangeEvent. orientation: " + orientation);
			}
		}
		
		
		/// <summary>
		/// Adds the ad listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		/// <param name="bannerType">Banner type.</param>
		public void AddAdListener (string gameObjectName, string bannerType)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (playerActivityContext == null)
			{
				InitAndroidClass ();
			}
			if (playerActivityContext != null)
			{
				try
				{
					AMEvents.amLogger.Log ("TRYING TO ADD LISTENER");
					playerActivityContext.CallStatic<bool> (ADD_AD_LISTENER, gameObjectName, bannerType);
				}
				catch (Exception ex)
				{
					AMEvents.amLogger.Log ("EXCEPTION AT ADD LISTENER");
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. AddAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}
		
		/// <summary>
		/// Removes the ad listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		/// <param name="bannerType">Banner type.</param>
		public void RemoveAdListener (string gameObjectName, string bannerType)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (playerActivityContext == null)
			{
				InitAndroidClass ();
			}
			if (playerActivityContext != null)
			{
				try
				{
					playerActivityContext.CallStatic <bool> (REMOVE_AD_LISTENER, gameObjectName, bannerType);
				}
				catch (Exception ex)
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. RemoveAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}
		
		/// <summary>
		/// Determines whether this instance is ad disabled.
		/// </summary>
		/// <returns><c>true</c> if this instance is ad disabled; otherwise, <c>false</c>.</returns>
		public bool IsAdDisabled ()
		{
			bool result = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic<bool> (IS_AD_DISABLED);
			}
			catch (System.Exception ex)
			{
				result = false;
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif	
			if (DebugMode) 
			{
				AMEvents.amLogger.Log ("Call Android. IsAdDisabled: " + result);
			}
			return result;
		}
		
		/// <summary>
		/// Enables the cross for disabling ads.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public void EnableCross (string gameObjectName)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				playerActivityContext.CallStatic (ENABLE_CROSS, gameObjectName, ON_CROSS_CLICKED);
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. EnableCross. gameObjectName: " + gameObjectName);
			}
		}
		
		/// <summary>
		/// Disables ads.
		/// </summary>
		public void DisableAd (string gameObjectName)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (playerActivityContext == null)
			{
				InitAndroidClass ();
			}
			if (playerActivityContext != null)
			{
				try
				{
					playerActivityContext.CallStatic (DISABLE_AD, gameObjectName, AD_DISABLED_MESSAGE_HANDLER);
				}
				catch (Exception ex)
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. DisableAd. gameObjectName: " + gameObjectName);
			}
		}

		/// <summary>
		/// Enables ads.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public void EnableAd (string gameObjectName)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (playerActivityContext == null)
			{
				InitAndroidClass ();
			}
			if (playerActivityContext != null)
			{
				try
				{
					playerActivityContext.CallStatic (ENABLE_AD, gameObjectName, AD_ENABLED_MESSAGE_HANDLER);
				}
				catch (Exception ex)
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. EnableAd. gameObjectName: " + gameObjectName);
			}
		}

		/// <summary>
		/// Sets the banner visibility.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		public void SetBannerVisible (bool value)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (playerActivityContext == null)
			{
				InitAndroidClass ();
			}
			if (playerActivityContext != null)
			{
				try
				{
					playerActivityContext.CallStatic (SET_BANNER_VISIBLE, value);
				}
				catch (Exception ex)
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. SetBannerVisible to " + value);
			}
		}
		
		/// <summary>
		/// Sets the interstitial listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public bool SetInterstitialListener (string gameObjectName)
		{
			bool result = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic <bool> (SET_INTERSTITIAL_LISTENER, gameObjectName);
			}
			catch (System.Exception ex)
			{
				result = false;
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. SetInterstitialListener. gameObjectName: " + gameObjectName);
			}
			return result;
		}
		
		/// <summary>
		/// Removes the interstitial listener.
		/// </summary>
		public void RemoveInterstitialListener ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				playerActivityContext.CallStatic (REMOVE_INTERSTITIAL_LISTENER);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. RemoveInterstitialListener");
			}
		}

		/// <summary>
		/// Determines whether this instance is interstitial ready.
		/// </summary>
		/// <returns><c>true</c> if this instance is interstitial ready; otherwise, <c>false</c>.</returns>
		public bool IsInterstitialReady ()
		{
			bool result = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic <bool> (IS_INTERSTITIAL_READY);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. IsInterstitialReady: " + result);
			}
			return result;
		}

		/// <summary>
		/// Determines whether this instance is policy ready.
		/// </summary>
		/// <returns><c>true</c> if this instance is policy ready; otherwise, <c>false</c>.</returns>
		public bool IsPolicyReady ()
		{
			bool result = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic <bool> (IS_POLICY_READY);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. IsPolicyReady: " + result);
			}
			return result;
		}

		public bool setInterstitialDelay (int seconds)
		{
			bool result = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic <bool> (SET_INTERSTITIAL_DELAY, seconds);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. setInterstitialDelay: " + result);
			}
			return result;
		}

		public int getCurrentInterstitialDelay ()
		{
			int result = 0;
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic <int> (GET_CURRENT_INTERSTITIAL_DELAY);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. getCurrentInterstitialDelay: " + result);
			}
			return result;
		}

		/// <summary>
		/// Shows the interstitial.
		/// </summary>
		public void ShowInterstitial ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				playerActivityContext.CallStatic (SHOW_INTERSTITIAL);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif	
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. ShowInterstitial");
			}
		}

		/// <summary>
		/// Показ окна политики
		/// </summary>
		public void ShowPolicyContent ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				playerActivityContext.CallStatic (SHOW_POLICY_CONTENT);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif	
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. ShowPolicyContent");
			}
		}

		/// <summary>
		/// Показ окна политики перед авторизацией
		/// </summary>
		public void ShowRegisterPolicyContent ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				playerActivityContext.CallStatic (SHOW_REGISTER_POLICY_CONTENT);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif	
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. ShowRegisterPolicyContent");
			}
		}

		/// <summary>
		/// Returns time the after latest interstitial closes in seconds.
		/// </summary>
		/// <returns>Time in seconds.</returns>
		public int TimeAfterInterstitial ()
		{
			int result = 0;
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic <int> (TIME_AFTER_INTERSTITIAL);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. TimeAfterInterstitial: " + result);
			}
			return result;
		}

		/// <summary>
		/// Shows the rewarded video.
		/// </summary>
		public void ShowRewardedVideo ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				playerActivityContext.CallStatic (SHOW_REWARD_VIDEO);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif	
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. ShowRewardedVideo");
			}
		}
		
		/// <summary>
		/// Determines whether this instance is rewarded video ready.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool IsRewardedVideoReady ()
		{
			bool result = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic<bool> (IS_REWARD_VIDEO_READY);
			}
			catch (System.Exception ex)
			{
				result = false;
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif	
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. IsRewardedVideoReady: " + result);
			}
			return result;
		}
		
		/// <summary>
		/// Gets the height of the banner.
		/// </summary>
		/// <returns>The banner height.</returns>
		public int GetBannerWidth ()
		{
			int result = 0;
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic<int> (GET_BANNER_WIDTH);
			}
			catch (System.Exception ex)
			{
				result = 0;
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. GetBannerWidth");
			}
			return result;
		}

		public string GetPrivacyUrl()
		{
			string result = "";
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic<string> (GET_PRIVACY_URL);
			}
			catch (System.Exception ex)
			{
				result = "";
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. GetPrivacyUrl");
			}
			return result;		
		}

		public string GetEulaUrl()
		{
			string result = "";
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic<string> (GET_EULA_URL);
			}
			catch (System.Exception ex)
			{
				result = "";
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. GetEulaUrl");
			}
			return result;		
		}

		public string GetTosUrl()
		{
			string result = "";
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic<string> (GET_TOS_URL);
			}
			catch (System.Exception ex)
			{
				result = "";
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. GetTosUrl");
			}
			return result;		
		}
		
		public string GetRegisterText() 
		{ 
			string result = "";
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic<string> (GET_REGISTER_TEXT);
			}
			catch (System.Exception ex)
			{
				result = "";
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. GetRegisterText");
			}
			return result;			
		}

		/// <summary>
		/// Gets the height of the banner.
		/// </summary>
		/// <returns>The banner height.</returns>
		public int GetBannerHeight ()
		{
			int result = 0;//
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic<int> (GET_BANNER_HEIGHT);
			}
			catch (System.Exception ex)
			{
				result = 0;
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. GetBannerHeight");
			}
			return result;
		}

		public void ShowInnerInApp ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				playerActivityContext.CallStatic (SHOW_INNER_INAPP);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. ShowInnerInApp");
			}
		}

		public void BuyInnerInApp ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				playerActivityContext.CallStatic (BUY_INNER_INAPP);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. BuyInnerInApp");
			}
		}

		public void RestoreInnerInApp ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			//Android uses auto restore
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. RestoreInnerInApp");
			}
		}

		public string GetSocialNetworks ()
		{
			string result = "";
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic<string>(GET_SOCIAL_NET);
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			
			}
			#endif	
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. GetSocialNetworks");
			}
			return result;
		}

		public void Share(string networkName, Dictionary<string, string> parameters)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (playerActivityContext != null)
			{
				try
				{
					if (playerActivityContext == null)
					InitAndroidClass ();
					using (AndroidJavaObject obj_HashMap = new AndroidJavaObject ("java.util.HashMap"))
					{
						IntPtr method_Put = AndroidJNIHelper.GetMethodID (obj_HashMap.GetRawClass (), "put",
							"(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");

						object[] args = new object[2];
						foreach (KeyValuePair<string, string> kvp in parameters)
						{
							using (AndroidJavaObject k = new AndroidJavaObject ("java.lang.String", kvp.Key))
							{
								using (AndroidJavaObject v = new AndroidJavaObject ("java.lang.String", kvp.Value))
								{
									args[0] = k;
									args[1] = v;
									AndroidJNI.CallObjectMethod (obj_HashMap.GetRawObject (),
										method_Put, AndroidJNIHelper.CreateJNIArgArray (args));
								}
							}
						}
						playerActivityContext.CallStatic ("share", networkName, obj_HashMap);
					}
				}
				catch (System.Exception ex) 
				{
						if (AMEvents.amLogger != null)
								AMEvents.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. Share");
			}
		}

		public void CheckInternetAccess (string url)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				playerActivityContext.CallStatic("checkInternetAccess", url);

			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. CheckInternetAccess");
			}
		}
		
		public void StartInternetChecking (string url, int timeInterval){
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
								if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				playerActivityContext.CallStatic("startInternetChecking", url, timeInterval);
				
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. StartInternetChecking");
			}
		}

		public void StopInternetChecking (){
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				playerActivityContext.CallStatic("stopInternetChecking");
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif

			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. StopInternetChecking");
			}
		}

        public bool IsCommercialReady()
        {
            bool result = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (playerActivityContext == null)
				{
					InitAndroidClass ();
				}
				result = playerActivityContext.CallStatic<bool> (IS_COMMERCIAL_READY);
			}
			catch (System.Exception ex)
			{
				result = false;
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif	
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Android. IS_COMMERCIAL_READY: " + result);
			}
			return result;
        }
		public void StartGDPR ()
		{
			if (DebugMode)
			{
				Debug.Log ("Call Android. StartGDPR");
			}
		}
		public bool IsPolicyShown ()
		{
			bool result = false;
			if (DebugMode)
			{
				Debug.Log ("Call Android. IsPolicyShown");
			}
			return result;
		}
		public bool IsBasePolicyRequired ()
		{
			bool result = false;
			if (DebugMode)
			{
				Debug.Log ("Call Android. IsBasePolicyRequired");
			}
			return result;
		}
		public bool IsRegisterPolicyRequired ()
		{
			bool result = false;
			if (DebugMode)
			{
				Debug.Log ("Call Android. IsRegisterPolicyRequired");
			}
			return result;
		}
		public bool IsBasePolicyAccepted ()
		{
			bool result = false;
			if (DebugMode)
			{
				Debug.Log ("Call Android. IsBasePolicyAccepted");
			}
			return result;
		}
		public bool IsRegisterPolicyAccepted ()
		{
			bool result = false;
			if (DebugMode)
			{
				Debug.Log ("Call Android. IsRegisterPolicyAccepted");
			}
			return result;
		}
		public bool IsRegisterPolicyRevoked ()
		{
			bool result = false;
			if (DebugMode)
			{
				Debug.Log ("Call Android. IsRegisterPolicyRevoked");
			}
			return result;
		}
		public string GetPolicyError ()
		{
			string result = "";
			if (DebugMode)
			{
				Debug.Log ("Call Android. GetPolicyError");
			}
			return result;
		}
    }
}