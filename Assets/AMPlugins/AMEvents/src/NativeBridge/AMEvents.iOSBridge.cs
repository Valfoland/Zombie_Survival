using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace AMEvents
{
	public class iOSBridge : INativeBridge
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

        #region Ad
        private const string AD_DISABLED_MESSAGE_HANDLER = "AdDisabledMessageHandler";
		private const string AD_ENABLED_MESSAGE_HANDLER = "AdEnabledMessageHandler";
		private const string ON_CROSS_CLICKED = "onCrossClicked";
		#endregion

		#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
		private static extern void _ADsetBannerVisible (bool value);

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

		[DllImport ("__Internal")]
		private static extern void _ADshowInnerInApp ();

		[DllImport ("__Internal")]
		private static extern void _ADbuyInnerInApp ();

		[DllImport ("__Internal")]
		private static extern void _ADrestoreInnerInApp ();

		[DllImport ("__Internal")]
		private static extern string _getSocialNetworks ();

		[DllImport ("__Internal")]
		private static extern void _checkInternetAccess (string url);

		[DllImport ("__Internal")]
		private static extern void _startInternetChecking (string url, int timeInterval);

		[DllImport ("__Internal")]
		private static extern void _stopInternetChecking ();

		[DllImport ("__Internal")]
		private static extern void _share (string networkName, string jsonMap);

		#region Policy
			[DllImport ("__Internal")]
			private static extern void _showPolicyContent ();

			[DllImport ("__Internal")]
			private static extern string _getPrivacyUrl ();

			[DllImport ("__Internal")]
			private static extern string _getEulaUrl ();

			[DllImport ("__Internal")]
			private static extern string _getTosUrl ();

			[DllImport ("__Internal")]
			private static extern string _getRegisterText ();

			[DllImport ("__Internal")]
			private static extern void _showRegisterPolicyContent ();

			[DllImport ("__Internal")]
			private static extern bool _isPolicyReady ();
		#endregion

		#endif

		/// <summary>
		/// News the scene event.
		/// </summary>
		public void NewSceneEvent ()
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. NewSceneEvent. Type: " +  DebugMode.GetType ().ToString ());
			}
		}

		/// <summary>
		/// Resumes the scene event.
		/// </summary>
		public void ResumeSceneEvent ()
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. ResumeSceneEvent"); 
			}
		}

		public string GetSocialNetworks ()
		{
			string result = "";
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			try
			{
				result = _getSocialNetworks ();
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
				result = "";
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call iOS. GetSocialNetworks"); 
			}
			return result;
		}

		public void Share(string networkName, Dictionary<string, string> parameters)
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			string jsonData = AMUtils.AMJSON.JsonEncode (new Hashtable(parameters));
			try
			{
				_share (networkName, jsonData);
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call iOS. Share"); 
			}
		}

		/// <summary>
		/// Pauses the scene event.
		/// </summary>
		public void PauseSceneEvent ()
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. PauseSceneEvent"); 
			}
		}
		

				public void CheckInternetAccess (string url){
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			try
			{
				_checkInternetAccess (url);
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call CheckInternetAccess");
			}
		}

		public void StartInternetChecking (string url, int timeInterval){

			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			try
			{
				_startInternetChecking (url, timeInterval);
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call StartInternetChecking");
			}
		}
		public void StopInternetChecking (){
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			try
			{
				_stopInternetChecking ();
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call StopInternetChecking");
			}
		}



		/// <summary>
		/// Applications the inactive event.
		/// </summary>
		public void ApplicationInactiveEvent ()
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. ApplicationInactiveEvent"); 
			}
		}
		
		/// <summary>
		/// Orientations the change event.
		/// </summary>
		/// <param name="orientation">Orientation.</param>
		public void OrientationChangeEvent ()
		{
			string orientation = Screen.orientation.ToString ();
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			try
			{
				_orientationChanged (orientation);
			}
			catch (Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call iOS. OrientationChangeEvent. orientation: " + orientation);
			}
		}

		/// <summary>
		/// Adds the ad listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		/// <param name="bannerType">Banner type.</param>
		public void AddAdListener (string gameObjectName, string bannerType)
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. AddAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}
		
		/// <summary>
		/// Removes the ad listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		/// <param name="bannerType">Banner type.</param>
		public void RemoveAdListener (string gameObjectName, string bannerType)
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. RemoveAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}

		/// <summary>
		/// Enables the cross.
		/// </summary>
		public void EnableCross (string gameObjectName)
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			try 
			{
				_ADenableCross (gameObjectName, ON_CROSS_CLICKED);
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call iOS. EnableCross. gameObjectName: " + gameObjectName);
			}
		}
		
		/// <summary>
		/// Determines whether this instance is ad disabled.
		/// </summary>
		/// <returns><c>true</c> if this instance is ad disabled; otherwise, <c>false</c>.</returns>
		public bool IsAdDisabled ()
		{
			bool result = false;
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. IsAdDisabled. result: " + result);
			}
			return result;
		}
		
		/// <summary>
		/// Disables ads.
		/// </summary>
		public void DisableAd (string gameObjectName)
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. DisableAd. gameObjectName: " + gameObjectName);
			}
		}

		/// <summary>
		/// Enables ads.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public void EnableAd (string gameObjectName)
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. EnableAd. gameObjectName: " + gameObjectName);
			}
		}

		/// <summary>
		/// Sets the banner visibility.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		public void SetBannerVisible (bool value)
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			try 
			{
				_ADsetBannerVisible (value);
			} 
			catch (System.Exception ex) 
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call iOS. SetBannerVisible to " + value);
			}
		}
		
		/// <summary>
		/// Sets the interstitial listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public bool SetInterstitialListener (string gameObjectName)
		{
			bool result = false;
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. SetInterstitialListener. gameObjectName: " + gameObjectName + "; result: " + result);
			}
			return result;
		}

		/// <summary>
		/// Removes the interstitial listener.
		/// </summary>
		public void RemoveInterstitialListener ()
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. RemoveInterstitialListener");
			}
		}

		/// <summary>
		/// Determines whether this instance is interstitial ready.
		/// </summary>
		/// <returns><c>true</c> if this instance is interstitial ready; otherwise, <c>false</c>.</returns>
		public bool IsInterstitialReady ()
		{
			bool result = false;
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. IsInterstitialReady: " + result);
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
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. IsPolicyReady: " + result);
			}
			return result;
		}


		public bool IsCommercialReady ()
		{
			bool result = false;
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. IsCommercialReady: " + result);
			}
			return result;
		}

		
		/// <summary>
		/// Shows the interstitial.
		/// </summary>
		public void ShowInterstitial ()
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. ShowInterstitial");
			}
		}

		/// <summary>
		/// Returns time the after latest interstitial closes in seconds.
		/// </summary>
		/// <returns>Time in seconds.</returns>
		public int TimeAfterInterstitial ()
		{
			int result = 0;
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. TimeAfterInterstitial: " + result);
			}
			return result;
		}

		public bool setInterstitialDelay (int seconds)
		{
			bool result = false;
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			try 
			{
				result = _ADsetInterstitialDelay (seconds);
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
				AMEvents.amLogger.Log ("Call iOS. Bridge setInterstitialDelay. result: " + result);
			}
			return result;
		}

		public int getCurrentInterstitialDelay ()
		{
			int result = 0;
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. getCurrentInterstitialDelay: " + result);
			}
			return result;
		}

		/// <summary>
		/// Shows the rewarded video.
		/// </summary>
		public void ShowRewardedVideo ()
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. ShowRewardedVideo");
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
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. Bridge IsRewardedVideoReady. result: " + result);
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
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. GetBannerWidth. result: " + result);
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
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
				AMEvents.amLogger.Log ("Call iOS. GetBannerHeight. result: " + result);
			}
			return result;
		}

		public void ShowInnerInApp ()
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			try
			{
				_ADshowInnerInApp ();
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call iOS. ShowInnerInApp");
			}
		}

		public void BuyInnerInApp ()
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			try
			{
				_ADbuyInnerInApp ();
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call iOS. BuyInnerInApp");
			}
		}

		public void RestoreInnerInApp ()
		{
			#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
			try
			{
				_ADrestoreInnerInApp ();
			}
			catch (System.Exception ex)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call iOS. RestoreInnerInApp");
			}
		}

		#region Policy
			/// <summary>
			/// Показ окна политики.
			/// </summary>
			public void ShowPolicyContent ()
			{
				#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call iOS. ShowPolicyContent");
				}
			}

			/// <summary>
			/// Показ окна политики перед авторизацией.
			/// </summary>
			public void ShowRegisterPolicyContent ()
			{
				#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call iOS. ShowRegisterPolicyContent");
				}
			}

			/// <summary>
			/// Получение текста для регистрации
			/// </summary>
			public string GetRegisterText ()
			{
				string result = "";
				#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call iOS. GetRegisterText");
				}
				return result;
			}

			/// <summary>
			/// Получение ссылки на полный текст политики
			/// </summary>
			public string GetPrivacyUrl ()
			{
				string result = "";
				#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call iOS. GetPrivacyUrl");
				}
				return result;
			}
			/// <summary>
			/// Получение ссылки на Eula
			/// </summary>
			public string GetEulaUrl ()
			{
				string result = "";
				#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call iOS. GetEulaUrl");
				}
				return result;
			}
			/// <summary>
			/// Получение ссылки на Tos
			/// </summary>
			public string GetTosUrl ()
			{
				string result = "";
				#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call iOS. GetTosUrl");
				}
				return result;
			}
			public void StartGDPR ()
			{
				if (DebugMode)
				{
					Debug.Log ("Call iOS. StartGDPR");
				}
			}
			public bool IsPolicyShown ()
			{
				bool result = false;
				if (DebugMode)
				{
					Debug.Log ("Call iOS. IsPolicyShown");
				}
				return result;
			}
			public bool IsBasePolicyRequired ()
			{
				bool result = false;
				if (DebugMode)
				{
					Debug.Log ("Call iOS. IsBasePolicyRequired");
				}
				return result;
			}
			public bool IsRegisterPolicyRequired ()
			{
				bool result = false;
				if (DebugMode)
				{
					Debug.Log ("Call iOS. IsRegisterPolicyRequired");
				}
				return result;
			}
			public bool IsBasePolicyAccepted ()
			{
				bool result = false;
				if (DebugMode)
				{
					Debug.Log ("Call iOS. IsBasePolicyAccepted");
				}
				return result;
			}
			public bool IsRegisterPolicyAccepted ()
			{
				bool result = false;
				if (DebugMode)
				{
					Debug.Log ("Call iOS. IsRegisterPolicyAccepted");
				}
				return result;
			}
			public bool IsRegisterPolicyRevoked ()
			{
				bool result = false;
				if (DebugMode)
				{
					Debug.Log ("Call iOS. IsRegisterPolicyRevoked");
				}
				return result;
			}
			public string GetPolicyError ()
			{
				string result = "";
				if (DebugMode)
				{
					Debug.Log ("Call iOS. GetPolicyError");
				}
				return result;
			}
		#endregion
    }
}
