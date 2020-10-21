using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_TVOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace AMEvents
{
	public class tvOSBridge : INativeBridge
	{
		#region Debug
		/// <summary>
		/// The debug mode.
		/// </summary>
		public bool DebugMode {
			get;
			set;
		}
		#endregion

		#region Ad
		private const string AD_DISABLED_MESSAGE_HANDLER = "AdDisabledMessageHandler";
		private const string AD_ENABLED_MESSAGE_HANDLER = "AdEnabledMessageHandler";
		private const string ON_CROSS_CLICKED = "onCrossClicked";
		#endregion

		#if UNITY_TVOS && !UNITY_EDITOR
		[DllImport ("__Internal")]
		private static extern void _newSceneEvent ();

		[DllImport ("__Internal")]
		private static extern void _pauseSceneEvent ();

		[DllImport ("__Internal")]
		private static extern void _resumeSceneEvent ();

		[DllImport ("__Internal")]
		private static extern void _applicationInactiveEvent ();

		[DllImport ("__Internal")]
		private static extern void _orientationChanged (string orientation);

		[DllImport ("__Internal")]
		private static extern void _showInterstitial ();

		[DllImport ("__Internal")]
		private static extern bool _showPolicyContent ();

		[DllImport ("__Internal")]
		private static extern bool _showRegisterPolicyContent ();

		[DllImport ("__Internal")]
		private static extern string _getPrivacyUrl ();

		[DllImport ("__Internal")]
		private static extern string _getEulaUrl ();

		[DllImport ("__Internal")]
		private static extern string _getTosUrl ();

		[DllImport ("__Internal")]
		private static extern string _getRegisterText ();

		[DllImport ("__Internal")]
		private static extern bool _isPolicyReady ();

		[DllImport ("__Internal")]
		private static extern int _ADtimeAfterInterstitial ();

		[DllImport ("__Internal")]
		private static extern bool _ADsetInterstitialDelay (int seconds);

		[DllImport ("__Internal")]
		private static extern int _ADgetCurrentInterstitialDelay ();

		[DllImport ("__Internal")]
		private static extern bool _setInterstitialListenerWithGameObject (string gameObjectName);

		[DllImport ("__Internal")]
		private static extern void _removeInterstitialListener ();

		//unity + SDK
		[DllImport ("__Internal")]
		private static extern void _addAdListener (string gameObject, string bannerType);

		[DllImport ("__Internal")]
		private static extern void _removeAdListener (string gameObject, string bannerType);

		[DllImport ("__Internal")]
		private static extern bool _ADisInterstitialReady ();
		
		[DllImport ("__Internal")]
		private static extern bool _ADisCommercialInterstitialReady ();

		[DllImport ("__Internal")]
		private static extern void _ADenableCross (string gameObjectName, string methodName);

		[DllImport ("__Internal")]
		private static extern void _ADdisableAd (string gameObject, string methodName);

		[DllImport ("__Internal")]
		private static extern void _ADenableAd (string gameObject, string methodName);

		[DllImport ("__Internal")]
		private static extern bool _ADisAdDisabled ();

		[DllImport ("__Internal")]
		private static extern void _ADshowRewardedVideo ();

		[DllImport ("__Internal")]
		private static extern bool _ADisRewardedVideoReady ();

		[DllImport ("__Internal")]
		private static extern int _ADgetBannerWidth ();

		[DllImport ("__Internal")]
		private static extern int _ADgetBannerHeight ();
		#endif

		#region App Events API
		/// <summary>
		/// News the scene event.
		/// </summary>
		public void NewSceneEvent ()
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try
			{
				_newSceneEvent ();
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. NewSceneEvent"); 
			}
		}

		/// <summary>
		/// Pauses the scene event.
		/// </summary>
		public void PauseSceneEvent ()
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try
			{
				_pauseSceneEvent ();
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. PauseSceneEvent"); 
			}
		}

		/// <summary>
		/// Resumes the scene event.
		/// </summary>
		public void ResumeSceneEvent ()
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try
			{
				_resumeSceneEvent ();
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. ResumeSceneEvent"); 
			}
		}

		/// <summary>
		/// Applications the inactive event.
		/// </summary>
		public void ApplicationInactiveEvent ()
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try
			{
				_applicationInactiveEvent ();
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. ApplicationInactiveEvent");
			}
		}

		/// <summary>
		/// Orientations the change event.
		/// </summary>
		public void OrientationChangeEvent ()
		{
			string orientation = Screen.orientation.ToString ();
			#if UNITY_TVOS && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. OrientationChangeEvent. orientation: " + orientation);
			}
		}
		#endregion

		#region AD API
		/// <summary>
		/// Adds the ad listener.
		/// </summary>
		/// <param name="bannerType">Banner type.</param>
		public void AddAdListener (string gameObjectName, string bannerType)
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try
			{
				_addAdListener (gameObjectName, bannerType);
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. AddAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}

		/// <summary>
		/// Removes the ad listener.
		/// </summary>
		/// <param name="bannerType">Banner type.</param>
		public void RemoveAdListener (string gameObjectName, string bannerType)
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try
			{
				_removeAdListener (gameObjectName, bannerType);
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. RemoveAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}

		/// <summary>
		/// Enables the cross.
		/// </summary>
		public void EnableCross (string gameObjectName)
		{
			#if UNITY_TVOS && !UNITY_EDITOR

			#endif

			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. EnableCross. gameObjectName: " + gameObjectName);
			}
		}

		/// <summary>
		/// Determines whether this instance is ad disabled.
		/// </summary>
		/// <returns><c>true</c> if this instance is ad disabled; otherwise, <c>false</c>.</returns>
		public bool IsAdDisabled ()
		{
			bool result = false;
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				result = _ADisAdDisabled ();
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
				AMEvents.amLogger.Log ("Call tvOS. IsAdDisabled: " + result);
			}
			return result;
		}

		/// <summary>
		/// Disables the ad.
		/// </summary>
		public void DisableAd (string gameObjectName)
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				_ADdisableAd (gameObjectName, AD_DISABLED_MESSAGE_HANDLER);
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. DisableAd. gameObjectName: " + gameObjectName);
			}
		}

		/// <summary>
		/// Enables ads.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public void EnableAd (string gameObjectName)
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				_ADenableAd (gameObjectName, AD_ENABLED_MESSAGE_HANDLER);
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. EnableAd. gameObjectName: " + gameObjectName);
			}
		}

		/// <summary>
		/// Sets the banner visibility.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		public void SetBannerVisible (bool value)
		{
			#if UNITY_TVOS && !UNITY_EDITOR
//			try 
//			{
//				_ADsetBannerVisible (value);
//			} 
//			catch (System.Exception ex) 
//			{
//				if (AMEvents.amLogger != null)
//					AMEvents.amLogger.LogException (ex);
//			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. SetBannerVisible to " + value);
			}
		}

		/// <summary>
		/// Sets the interstitial listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public bool SetInterstitialListener (string gameObjectName)
		{
			bool result = false;
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				result = _setInterstitialListenerWithGameObject (gameObjectName);
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
				AMEvents.amLogger.Log ("Call tvOS. SetInterstitialListener. gameObjectName: " + gameObjectName);
			}
			return result;
		}

		/// <summary>
		/// Removes the interstitial listener.
		/// </summary>
		public void RemoveInterstitialListener ()
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				_removeInterstitialListener ();
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. RemoveInterstitialListener");
			}
		}

		/// <summary>
		/// Determines whether this instance is interstitial ready.
		/// </summary>
		/// <returns><c>true</c> if this instance is interstitial ready; otherwise, <c>false</c>.</returns>
		public bool IsInterstitialReady ()
		{
			bool result = false;
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				result = _ADisInterstitialReady ();
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
				AMEvents.amLogger.Log ("Call tvOS. IsInterstitialReady: " + result);
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
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				result = _isPolicyReady ();
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
				AMEvents.amLogger.Log ("Call tvOS. IsPolicyReady: " + result);
			}
			return result;
		}

		/// <summary>
		/// Shows the interstitial.
		/// </summary>
		public void ShowInterstitial ()
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				_showInterstitial ();
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. ShowInterstitial");
			}
		}

		/// <summary>
		/// Shows the Policy.
		/// </summary>
		public void ShowPolicyContent()
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				_showPolicyContent ();
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. ShowPolicyContent");
			}
		}

		/// <summary>
		/// Shows the Policy.
		/// </summary>
		public void ShowRegisterPolicyContent()
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				_showRegisterPolicyContent ();
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. ShowRegisterPolicyContent");
			}
		}
		public void StartGDPR ()
		{
			if (DebugMode)
			{
				Debug.Log ("Call tvOS. StartGDPR");
			}
		}

		public bool IsPolicyShown ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. IsPolicyShown");
			}
			return result;
		}
		public bool IsBasePolicyRequired ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. IsBasePolicyRequired");
			}
			return result;
		}
		public bool IsRegisterPolicyRequired ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. IsRegisterPolicyRequired");
			}
			return result;
		}
		public bool IsBasePolicyAccepted ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. IsBasePolicyAccepted");
			}
			return result;
		}
		public bool IsRegisterPolicyAccepted ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. IsRegisterPolicyAccepted");
			}
			return result;
		}
		public bool IsRegisterPolicyRevoked ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. IsRegisterPolicyRevoked");
			}
			return result;
		}
		public string GetPolicyError ()
		{
			string result = "";
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. GetPolicyError");
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
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				result = _ADtimeAfterInterstitial ();
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. TimeAfterInterstitial: " + result);
			}
			return result;
		}

		public bool setInterstitialDelay (int seconds)
		{
			bool result = false;
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				result = _ADsetInterstitialDelay (seconds);
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. Bridge setInterstitialDelay: " + result);
			}
			return result;
		}

		public string GetRegisterText ()
		{
			string result = "";
			#if UNITY_TVOS && !UNITY_EDITOR
			try
			{
				result = _getRegisterText ();
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
				result = "";
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. GetRegisterText");
			}
			return result;
		}

		public string GetPrivacyUrl ()
		{
			string result = "";
			#if UNITY_TVOS && !UNITY_EDITOR
			try
			{
				result = _getPrivacyUrl ();
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
				result = "";
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. GetPrivacyUrl"); 
			}
			return result;
		}

		public string GetEulaUrl ()
		{
			string result = "";
			#if UNITY_TVOS && !UNITY_EDITOR
			try
			{
				result = _getEulaUrl ();
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
				result = "";
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. GetEulaUrl"); 
			}
			return result;
		}

		public string GetTosUrl ()
		{
			string result = "";
			#if UNITY_TVOS && !UNITY_EDITOR
			try
			{
				result = _getTosUrl ();
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
				result = "";
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. GetTosUrl"); 
			}
			return result;
		}

		public int getCurrentInterstitialDelay ()
		{
			int result = 0;
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				result = _ADgetCurrentInterstitialDelay ();
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. getCurrentInterstitialDelay: " + result);
			}
			return result;
		}

		/// <summary>
		/// Shows the rewarded video.
		/// </summary>
		public void ShowRewardedVideo ()
		{
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				_ADshowRewardedVideo ();
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. ShowRewardedVideo");
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
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				result = _ADisRewardedVideoReady ();
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
				AMEvents.amLogger.Log ("Call tvOS. Bridge IsRewardedVideoReady: " + result);
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
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				result = _ADgetBannerWidth ();
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
				AMEvents.amLogger.Log ("Call tvOS. GetBannerWidth");
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
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				result = _ADgetBannerHeight ();
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
				AMEvents.amLogger.Log ("Call tvOS. GetBannerHeight");
			}
			return result;
		}

		public void ShowInnerInApp ()
		{
			#if UNITY_TVOS && !UNITY_EDITOR

			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. ShowInnerInApp");
			}
		}

		public void BuyInnerInApp ()
		{
			#if UNITY_TVOS && !UNITY_EDITOR

			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. BuyInnerInApp");
			}
		}

		public void RestoreInnerInApp ()
		{
			#if UNITY_TVOS && !UNITY_EDITOR

			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. RestoreInnerInApp");
			}
		}

       public string GetSocialNetworks ()
		{
			string result = "";
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. GetSocialNetworks");
			}
			return result;
		}

		public void Share(string networkName, Dictionary<string, string> parameters)
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. Share");
			}
		}

				public void CheckInternetAccess (string url){
		if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. CheckInternetAccess");
			}
		}
		public void StartInternetChecking (string url, int timeInterval){
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. StartInternetChecking");
			}
		}
		public void StopInternetChecking (){
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call tvOS. StopInternetChecking");
			}
		}


		public bool IsCommercialReady ()
		{
			bool result = false;
			#if UNITY_TVOS && !UNITY_EDITOR
			try 
			{
				result = _ADisCommercialInterstitialReady ();
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
				AMEvents.amLogger.Log ("Call tvOS. IsCommercialReady: " + result);
			}
			return result;
		}
        #endregion
    }
}