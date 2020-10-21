using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace AMEvents
{
	public class MockBridge : INativeBridge
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
		/// <summary>
		/// News the scene event.
		/// </summary>
		public void NewSceneEvent ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. NewSceneEvent"); 
			}
		}
		
		/// <summary>
		/// Pauses the scene event.
		/// </summary>
		public void PauseSceneEvent ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. PauseSceneEvent"); 
			}
		}
		
		/// <summary>
		/// Resumes the scene event.
		/// </summary>
		public void ResumeSceneEvent ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. ResumeSceneEvent"); 
			}
		}
		
		/// <summary>
		/// Applications the inactive event.
		/// </summary>
		public void ApplicationInactiveEvent ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. ApplicationInactiveEvent");
			}
		}
		
		/// <summary>
		/// Orientations the change event.
		/// </summary>
		public void OrientationChangeEvent ()
		{
			string orientation = Screen.orientation.ToString ();

			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. OrientationChangeEvent. orientation: " + orientation);
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
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. AddAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}
		
		/// <summary>
		/// Removes the ad listener.
		/// </summary>
		/// <param name="bannerType">Banner type.</param>
		public void RemoveAdListener (string gameObjectName, string bannerType)
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. RemoveAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}

		/// <summary>
		/// Enables the cross.
		/// </summary>
		public void EnableCross (string gameObjectName)
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. EnableCross. gameObjectName: " + gameObjectName);
			}
		}

		bool isAdDisabledResult = false;
		/// <summary>
		/// Determines whether this instance is ad disabled.
		/// </summary>
		/// <returns><c>true</c> if this instance is ad disabled; otherwise, <c>false</c>.</returns>
		public bool IsAdDisabled ()
		{
			bool result = isAdDisabledResult;
			if (DebugMode) 
			{
				AMEvents.amLogger.Log ("Call Mock. IsAdDisabled: " + result);
			}
			return result;
		}
		
		/// <summary>
		/// Disables the ad.
		/// </summary>
		public void DisableAd (string gameObjectName)
		{
			isAdDisabledResult = true;

			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. DisableAd. gameObjectName: " + gameObjectName);
			}

			if (AMEvents.Instance != null)
				AMEvents.Instance.AdDisabledMessageHandler ("success");
		}

		/// <summary>
		/// Enables ads.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public void EnableAd (string gameObjectName)
		{
			isAdDisabledResult = false;

			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. EnableAd. gameObjectName: " + gameObjectName);
			}

			if (AMEvents.Instance != null)
				AMEvents.Instance.AdEnabledMessageHandler ("success");
		}

		/// <summary>
		/// Sets the banner visibility.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		public void SetBannerVisible (bool value)
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. SetBannerVisible to " + value);
			}
		}
		
		/// <summary>
		/// Sets the interstitial listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public bool SetInterstitialListener (string gameObjectName)
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. SetInterstitialListener. gameObjectName: " + gameObjectName);
			}
			return result;
		}
		
		/// <summary>
		/// Removes the interstitial listener.
		/// </summary>
		public void RemoveInterstitialListener ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. RemoveInterstitialListener");
			}
		}

		/// <summary>
		/// Determines whether this instance is interstitial ready.
		/// </summary>
		/// <returns><c>true</c> if this instance is interstitial ready; otherwise, <c>false</c>.</returns>
		public bool IsInterstitialReady ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. IsInterstitialReady: " + result);
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
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. IsPolicyReady: " + result);
			}
			return result;
		}
		
		/// <summary>
		/// Shows the interstitial.
		/// </summary>
		public void ShowInterstitial ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. ShowInterstitial");
			}
		}

		/// <summary>
		/// Shows the Policy.
		/// </summary>
		public void ShowPolicyContent ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. ShowPolicyContent");
			}
		}

		/// <summary>
		/// Shows the manual Policy.
		/// </summary>
		public void ShowRegisterPolicyContent ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. ShowRegisterPolicyContent");
			}
		}

		public string GetRegisterText ()
		{
			string result = "";
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. GetRegisterText");
			}
			return result;
		}

		public string GetPrivacyUrl ()
		{
			string result = "";
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. GetPrivacyUrl");
			}
			return result;
		}

		public string GetEulaUrl ()
		{
			string result = "";
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. GetEulaUrl");
			}
			return result;
		}

		public string GetTosUrl ()
		{
			string result = "";
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. GetTosUrl");
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

			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. TimeAfterInterstitial: " + result);
			}
			return result;
		}
		
		public bool setInterstitialDelay (int seconds)
		{
			bool result = false;

			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. setInterstitialDelay: " + result);
			}
			return result;
		}

		public int getCurrentInterstitialDelay ()
		{
			int result = 60;

			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. getCurrentInterstitialDelay: " + result);
			}
			return result;
		}
		
		/// <summary>
		/// Shows the rewarded video.
		/// </summary>
		public void ShowRewardedVideo ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. ShowRewardedVideo");
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

			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. IsRewardedVideoReady: " + result);
			}
			return result;
		}
		
		/// <summary>
		/// Gets the height of the banner.
		/// </summary>
		/// <returns>The banner height.</returns>
		public int GetBannerWidth ()
		{
			int result = Screen.width/2;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. GetBannerWidth");
			}
			return result;
		}
		
		/// <summary>
		/// Gets the height of the banner.
		/// </summary>
		/// <returns>The banner height.</returns>
		public int GetBannerHeight ()
		{
			int result = (int) ((Screen.width * 50) / 320);
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. GetBannerHeight");
			}
			return result;
		}

		public void ShowInnerInApp ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. ShowInnerInApp");
			}
		}

		public void BuyInnerInApp ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. BuyInnerInApp");
			}
		}

		public void RestoreInnerInApp ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. RestoreInnerInApp");
			}
		}

        string INativeBridge.GetSocialNetworks()
        {
			return "[\"facebook\",\"vk\",\"twitter\"]";
        }

        public void Share(string networkName, Dictionary<string, string> parameters)
        {
          if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. Share");
			}
        }

        public bool IsCommercialReady()
        {
            bool result = false;

			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. IsCommercialReady: " + result);
			}
			return result;
        }

				public void CheckInternetAccess (string url){
		if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. CheckInternetAccess");
			}
		}
		public void StartInternetChecking (string url, int timeInterval){
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. StartInternetChecking");
			}
		}
		public void StopInternetChecking (){
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. StopInternetChecking");
			}
		}
		public void StartGDPR ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. StartGDPR");
			}
		}
		public bool IsPolicyShown ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. IsPolicyShown");
			}
			return result;
		}
		public bool IsBasePolicyRequired ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. IsBasePolicyRequired");
			}
			return result;
		}
		public bool IsRegisterPolicyRequired ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. IsRegisterPolicyRequired");
			}
			return result;
		}
		public bool IsBasePolicyAccepted ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. IsBasePolicyAccepted");
			}
			return result;
		}
		public bool IsRegisterPolicyAccepted ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. IsRegisterPolicyAccepted");
			}
			return result;
		}
		public bool IsRegisterPolicyRevoked ()
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. IsRegisterPolicyRevoked");
			}
			return result;
		}
		public string GetPolicyError ()
		{
			string result = "";
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call Mock. GetPolicyError");
			}
			return result;
		}
        #endregion
    }
}