using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
using System.Reflection;
#endif

namespace AMEvents
{
	public class UWPBridge : INativeBridge 
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
		private const string NEW_SCENE_EVENT = "newSceneEvent";
		private const string RESUME_SCENE_EVENT = "resumeSceneEvent";
		private const string PAUSE_SCENE_EVENT = "pauseSceneEvent";
		private const string APPLICATION_INACTIVE_EVENT = "applicationInactiveEvent";
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
		private const string IS_COMMERCIAL_READY = "isCommercialInterstitialReady";
		private const string SHOW_INTERSTITIAL = "showInterstitial";
		private const string TIME_AFTER_INTERSTITIAL = "timeAfterInterstitial";
		private const string SET_INTERSTITIAL_DELAY = "setInterstitialDelay";
		private const string SET_CURRENT_INTERSTITIAL_DELAY = "getCurrentInterstitialDelay";
		#endregion

		#region Policy
		private const string SHOW_POLICY_CONTENT = "showPolicyContent";
		private const string SHOW_REGISTER_POLICY_CONTENT = "showRegisterPolicyContent";
		private const string GET_PRIVACY_URL = "getPrivacyUrl";
		private const string GET_EULA_URL = "getEulaUrl";
		private const string GET_TOS_URL = "getTosUrl";
		private const string GET_REGISTER_TEXT = "getRegisterText";
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

		#region Auto InApp
		private const string SHOW_INNER_INAPP = "showInnerInApp";
		private const string BUY_INNER_INAPP = "buyInnerInApp";
		private const string RESTORE_INNER_INAPP = "restoreInnerInApp";
		#endregion

		#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
		private const string adsLibEvents = "AdsLibAM.Events";
        private const string adsLibName = "AdsLibAM";
		#endif


		/// <summary>
		/// News the scene event.
		/// </summary>
		public void NewSceneEvent ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type type = assm.GetType (adsLibEvents);
                type.GetMethod (NEW_SCENE_EVENT).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. NewSceneEvent"); 
			}
		}

		/// <summary>
		/// Resumes the scene event.
		/// </summary>
		public void ResumeSceneEvent ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (RESUME_SCENE_EVENT).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. ResumeSceneEvent"); 
			}
		}
		/// <summary>
		/// Pauses the scene event.
		/// </summary>
		public void PauseSceneEvent ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (PAUSE_SCENE_EVENT).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. PauseSceneEvent"); 
			}
		}

		/// <summary>
		/// Applications the inactive event.
		/// </summary>
		public void ApplicationInactiveEvent ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (APPLICATION_INACTIVE_EVENT).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. ApplicationInactiveEvent"); 
			}
		}

		/// <summary>
		/// Orientations the change event.
		/// </summary>
		/// <param name="orientation">Orientation.</param>
		public void OrientationChangeEvent ()
		{
			string orientation = Screen.orientation.ToString ();
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR

			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. OrientationChangeEvent. orientation: " + orientation);
			}
		}

		/// <summary>
		/// Adds the ad listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		/// <param name="bannerType">Banner type.</param>
		public void AddAdListener (string gameObjectName, string bannerType)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				try
				{
                	AssemblyName assmName = new AssemblyName (adsLibName);
                	Assembly assm = Assembly.Load (assmName);
                	Type advertisingControl = assm.GetType (adsLibEvents);
					advertisingControl.GetMethod (ADD_AD_LISTENER).Invoke (null, new System.Object[]{gameObjectName, bannerType});
				}
				catch (Exception ex)
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. AddAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}

		/// <summary>
		/// Removes the ad listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		/// <param name="bannerType">Banner type.</param>
		public void RemoveAdListener (string gameObjectName, string bannerType)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				try
				{
                	AssemblyName assmName = new AssemblyName (adsLibName);
                	Assembly assm = Assembly.Load (assmName);
                	Type advertisingControl = assm.GetType (adsLibEvents);
					advertisingControl.GetMethod (REMOVE_AD_LISTENER).Invoke (null, new System.Object[]{gameObjectName, bannerType});
				}
				catch (Exception ex)
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. RemoveAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}

		/// <summary>
		/// Determines whether this instance is ad disabled.
		/// </summary>
		/// <returns><c>true</c> if this instance is ad disabled; otherwise, <c>false</c>.</returns>
		public bool IsAdDisabled ()
		{
			bool result = false;
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				try
				{
                	AssemblyName assmName = new AssemblyName (adsLibName);
                	Assembly assm = Assembly.Load (assmName);
                	Type advertisingControl = assm.GetType (adsLibEvents);
					result = (bool) advertisingControl.GetMethod (IS_AD_DISABLED).Invoke (null, null);
				}
				catch (Exception ex)
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
			}
			catch (Exception ex)
			{
				result = false;
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode) 
			{
				AMEvents.amLogger.Log ("Call UWP. IsAdDisabled: " + result);
			}
			return result;
		}

		/// <summary>
		/// Enables the cross for disabling ads.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public void EnableCross (string gameObjectName) 
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (ENABLE_CROSS).Invoke (null, new System.Object[]{gameObjectName, ON_CROSS_CLICKED});
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. EnableCross. gameObjectName: " + gameObjectName);
			}
		}

		/// <summary>
		/// Disables the ad.
		/// </summary>
		public void DisableAd (string gameObjectName)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (DISABLE_AD).Invoke (null, new System.Object[]{gameObjectName, AD_DISABLED_MESSAGE_HANDLER});
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. DisableAd. gameObjectName: " + gameObjectName);
			}
		}

		/// <summary>
		/// Enables ads.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public void EnableAd (string gameObjectName)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			//			
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. EnableAd. gameObjectName: " + gameObjectName);
			}
		}

		/// <summary>
		/// Sets the banner visibility.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		public void SetBannerVisible (bool value)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			//			
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. SetBannerVisible to " + value);
			}
		}

		/// <summary>
		/// Sets the interstitial listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public bool SetInterstitialListener (string gameObjectName)
		{
			bool result = false;
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
				result = (bool) advertisingControl.GetMethod (SET_INTERSTITIAL_LISTENER).Invoke (null, new System.Object[]{gameObjectName});
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. SetInterstitialListener. gameObjectName: " + gameObjectName);
			}
			return result;
		}

		/// <summary>
		/// Removes the interstitial listener.
		/// </summary>
		public void RemoveInterstitialListener ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (REMOVE_INTERSTITIAL_LISTENER).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. RemoveInterstitialListener");
			}
		}

		/// <summary>
		/// Determines whether this instance is interstitial ready.
		/// </summary>
		/// <returns><c>true</c> if this instance is interstitial ready; otherwise, <c>false</c>.</returns>
		public bool IsInterstitialReady ()
		{
			bool result = false;
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				result = (bool) advertisingControl.GetMethod (IS_INTERSTITIAL_READY).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. IsInterstitialReady: " + result);
			}
			return result;
		}

		/// <summary>
		/// Determines whether this instance is interstitial ready.
		/// </summary>
		/// <returns><c>true</c> if this instance is interstitial ready; otherwise, <c>false</c>.</returns>
		public bool IsPolicyReady ()
		{
			bool result = false;
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				result = (bool) advertisingControl.GetMethod (IS_POLICY_READY).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. IsPolicyReady: " + result);
			}
			return result;
		}


		public bool IsCommercialReady ()
		{
			bool result = false;
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				result = (bool) advertisingControl.GetMethod (IS_COMMERCIAL_READY).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. IsCommercialReady: " + result);
			}
			return result;
		}

		/// <summary>
		/// Shows the interstitial.
		/// </summary>
		public void ShowInterstitial ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
           	try
            {
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
                advertisingControl.GetMethod (SHOW_INTERSTITIAL).Invoke (null, null);
            }
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. ShowInterstitial");
			}
		}

		/// <summary>
		/// Shows the Policy.
		/// </summary>
		public void ShowPolicyContent ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
           	try
            {
            	AssemblyName assmName = new AssemblyName (adsLibName);
            	Assembly assm = Assembly.Load (assmName);
            	Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (SHOW_POLICY_CONTENT).Invoke (null, null);
            }
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. ShowPolicyContent");
			}
		}

		/// <summary>
		/// Shows the manual Policy.
		/// </summary>
		public void ShowRegisterPolicyContent ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
           	try
            {
            	AssemblyName assmName = new AssemblyName (adsLibName);
            	Assembly assm = Assembly.Load (assmName);
            	Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (SHOW_REGISTER_POLICY_CONTENT).Invoke (null, null);
            }
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. ShowRegisterPolicyContent");
			}
		}

		public string GetRegisterText ()
		{
			string result = "";
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				result = (string) advertisingControl.GetMethod (GET_REGISTER_TEXT).Invoke (null, null);
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. GetRegisterText");
			}
			return result;
		}

		public string GetPrivacyUrl ()
		{
			string result = "";
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				result = (string) advertisingControl.GetMethod (GET_PRIVACY_URL).Invoke (null, null);
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. GetPrivacyURL");
			}
			return result;
		}

		public string GetEulaUrl ()
		{
			string result = "";
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				result = (string) advertisingControl.GetMethod (GET_EULA_URL).Invoke (null, null);
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. GetEulaUrl");
			}
			return result;
		}

		public string GetTosUrl ()
		{
			string result = "";
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				result = (string) advertisingControl.GetMethod (GET_TOS_URL).Invoke (null, null);
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. GetTosUrl");
			}
			return result;
		}

		public void StartGDPR ()
		{
			if (DebugMode)
			{
				Debug.Log ("Call UWP. StartGDPR");
			}
		}

		public bool IsPolicyShown ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. IsPolicyShown");
			}
			return result;
		}
		public bool IsBasePolicyRequired ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. IsBasePolicyRequired");
			}
			return result;
		}
		public bool IsRegisterPolicyRequired ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. IsRegisterPolicyRequired");
			}
			return result;
		}
		public bool IsBasePolicyAccepted ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. IsBasePolicyAccepted");
			}
			return result;
		}
		public bool IsRegisterPolicyAccepted ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. IsRegisterPolicyAccepted");
			}
			return result;
		}
		public bool IsRegisterPolicyRevoked ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. IsRegisterPolicyRevoked");
			}
			return result;
		}
		public string GetPolicyError ()
		{
			string result = "";
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. GetPolicyError");
			}
			return result;
		}

		/// <summary>
		/// Returns time the after latest interstitial closes in seconds.
		/// </summary>
		/// <returns>Time in seconds.</returns>
		public int TimeAfterInterstitial ()
		{
			int result = 0;
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				result = (int) advertisingControl.GetMethod (TIME_AFTER_INTERSTITIAL).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. TimeAfterInterstitial: " + result);
			}
			return result;
		}

		public bool setInterstitialDelay (int seconds)
		{
			bool result = false;
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				result = (bool) advertisingControl.GetMethod (TIME_AFTER_INTERSTITIAL).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. setInterstitialDelay: " + result);
			}
			return result;
		}

		public int getCurrentInterstitialDelay ()
		{
			int result = 0;
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				result = (int) advertisingControl.GetMethod (TIME_AFTER_INTERSTITIAL).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. getCurrentInterstitialDelay: " + result);
			}
			return result;
		}

		/// <summary>
		/// Shows the rewarded video.
		/// </summary>
		public void ShowRewardedVideo ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (SHOW_REWARD_VIDEO).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. ShowRewardedVideo");
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
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
				result = (bool) advertisingControl.GetMethod (IS_REWARD_VIDEO_READY).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. IsRewardedVideoReady: " + result);
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
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
				result = (int) advertisingControl.GetMethod (GET_BANNER_WIDTH).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. GetBannerWidth");
			}
			return result;
		}

		/// <summary>
		/// Gets the height of the banner.
		/// </summary>
		/// <returns>The banner height.</returns>
		public int GetBannerHeight ()
		{
			int result = 0;
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
                AssemblyName assmName = new AssemblyName (adsLibName);
                Assembly assm = Assembly.Load (assmName);
                Type advertisingControl = assm.GetType (adsLibEvents);
				result = (int) advertisingControl.GetMethod (GET_BANNER_HEIGHT).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
            if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. GetBannerHeight");
			}
			return result;
		}

		public void ShowInnerInApp ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (SHOW_INNER_INAPP).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. ShowInnerInApp");
			}
		}

		public void BuyInnerInApp ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (BUY_INNER_INAPP).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. BuyInnerInApp");
			}
		}

		public void RestoreInnerInApp ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (adsLibName);
				Assembly assm = Assembly.Load (assmName);
				Type advertisingControl = assm.GetType (adsLibEvents);
				advertisingControl.GetMethod (RESTORE_INNER_INAPP).Invoke (null, null);
			}
			catch (Exception ex)
			{
				AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. RestoreInnerInApp");
			}
		}
				public void CheckInternetAccess (string url){
		if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. CheckInternetAccess");
			}
		}
		public void StartInternetChecking (string url, int timeInterval){
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. StartInternetChecking");
			}
		}
		public void StopInternetChecking (){
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. StopInternetChecking");
			}
		}


       public string GetSocialNetworks ()
		{
			string result = "";
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. GetSocialNetworks");
			}
			return result;
		}

		public void Share(string networkName, Dictionary<string, string> parameters)
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call UWP. Share");
			}
		}
    }
}