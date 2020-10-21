using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
using System.Runtime.InteropServices;
#endif

namespace AMEvents
{
	public class MacBridge : INativeBridge
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

		#if !UNITY_EDITOR && UNITY_STANDALONE_OSX

		[DllImport ("GDPRControllerOSX")]
		private static extern void _startGDPR ();

		[DllImport ("GDPRControllerOSX")]
		private static extern bool _isPolicyReady ();

		[DllImport ("GDPRControllerOSX")]
		private static extern bool _isPolicyShown ();

		[DllImport ("GDPRControllerOSX")]
		private static extern bool _isBasePolicyRequired ();

		[DllImport ("GDPRControllerOSX")]
		private static extern bool _isRegisterPolicyRequired ();

		[DllImport ("GDPRControllerOSX")]
		private static extern bool _isBasePolicyAccepted ();

		[DllImport ("GDPRControllerOSX")]
		private static extern bool _isRegisterPolicyAccepted ();

		[DllImport ("GDPRControllerOSX")]
		private static extern bool _isRegisterPolicyRevoked ();

		[DllImport ("GDPRControllerOSX")]
		private static extern string _getPolicyError ();

		[DllImport ("GDPRControllerOSX")]
		private static extern string _getPrivacyUrl ();

		[DllImport ("GDPRControllerOSX")]
		private static extern string _getEulaUrl ();

		[DllImport ("GDPRControllerOSX")]
		private static extern string _getTosUrl ();

		[DllImport ("GDPRControllerOSX")]
		private static extern string _getRegisterText ();

		[DllImport ("GDPRControllerOSX")]
		private static extern void _showPolicyContent ();

		[DllImport ("GDPRControllerOSX")]
		private static extern void _showRegisterPolicyContent ();

		#endif
		
		#region App Events API
		/// <summary>
		/// News the scene event.
		/// </summary>
		public void NewSceneEvent ()
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. NewSceneEvent");
			}
		}
			

		/// <summary>
		/// Pauses the scene event.
		/// </summary>
		public void PauseSceneEvent ()
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. PauseSceneEvent");
			}
		}
		
		/// <summary>
		/// Resumes the scene event.
		/// </summary>
		public void ResumeSceneEvent ()
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. ResumeSceneEvent");
			}
		}
		
		/// <summary>
		/// Applications the inactive event.
		/// </summary>
		public void ApplicationInactiveEvent ()
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. ApplicationInactiveEvent");
			}
		}
		
		/// <summary>
		/// Orientations the change event.
		/// </summary>
		public void OrientationChangeEvent ()
		{
			string orientation = Screen.orientation.ToString ();
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. OrientationChangeEvent. orientation: " + orientation);
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
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. AddAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}
		/// <summary>
		/// Removes the ad listener.
		/// </summary>
		/// <param name="bannerType">Banner type.</param>
		public void RemoveAdListener (string gameObjectName, string bannerType)
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. RemoveAdListener. gameObjectName: " + gameObjectName + "; bannerType: " + bannerType);
			}
		}
		/// <summary>
		/// Enables the cross.
		/// </summary>
		public void EnableCross (string gameObjectName)
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif

			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. EnableCross. gameObjectName: " + gameObjectName);
			}
		}
		/// <summary>
		/// Determines whether this instance is ad disabled.
		/// </summary>
		/// <returns><c>true</c> if this instance is ad disabled; otherwise, <c>false</c>.</returns>
		public bool IsAdDisabled ()
		{
			bool result = false;
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode) 
			{
				AMEvents.amLogger.Log ("Call macOS. IsAdDisabled: " + result);
			}
			return result;
		}
		/// <summary>
		/// Disables the ad.
		/// </summary>
		public void DisableAd (string gameObjectName)
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. DisableAd. gameObjectName: " + gameObjectName);
			}
		}
		/// <summary>
		/// Enables ads.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public void EnableAd (string gameObjectName)
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. EnableAd. gameObjectName: " + gameObjectName);
			}
		}
		/// <summary>
		/// Sets the banner visibility.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		public void SetBannerVisible (bool value)
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. SetBannerVisible to " + value);
			}
		}
		/// <summary>
		/// Sets the interstitial listener.
		/// </summary>
		/// <param name="gameObjectName">Game object name.</param>
		public bool SetInterstitialListener (string gameObjectName)
		{
			bool result = false;
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. SetInterstitialListener. gameObjectName: " + gameObjectName);
			}
			return result;
		}
		/// <summary>
		/// Removes the interstitial listener.
		/// </summary>
		public void RemoveInterstitialListener ()
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. RemoveInterstitialListener");
			}
		}
		/// <summary>
		/// Determines whether this instance is interstitial ready.
		/// </summary>
		/// <returns><c>true</c> if this instance is interstitial ready; otherwise, <c>false</c>.</returns>
		public bool IsInterstitialReady ()
		{
			bool result = false;
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. IsInterstitialReady: " + result);
			}
			return result;
		}
		/// <summary>
		/// Shows the interstitial.
		/// </summary>
		public void ShowInterstitial ()
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. ShowInterstitial");
			}
		}
		/// <summary>
		/// Returns time the after latest interstitial closes in seconds.
		/// </summary>
		/// <returns>Time in seconds.</returns>
		public int TimeAfterInterstitial ()
		{
			int result = 0;
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. TimeAfterInterstitial: " + result);
			}
			return result;
		}
		public bool setInterstitialDelay (int seconds)
		{
			bool result = false;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. setInterstitialDelay: " + result);
			}
			return result;
		}
		public int getCurrentInterstitialDelay ()
		{
			int result = 0;
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. getCurrentInterstitialDelay: " + result);
			}
			return result;
		}
		/// <summary>
		/// Shows the rewarded video.
		/// </summary>
		public void ShowRewardedVideo ()
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. ShowRewardedVideo");
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
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. Bridge IsRewardedVideoReady: " + result);
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
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. GetBannerWidth");
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
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. GetBannerHeight");
			}
			return result;
		}
		public void ShowInnerInApp ()
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. ShowInnerInApp");
			}
		}
		public void BuyInnerInApp ()
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. BuyInnerInApp");
			}
		}
		public void RestoreInnerInApp ()
		{
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. RestoreInnerInApp");
			}
		}
        public string GetSocialNetworks ()
		{
			string result = "";
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. GetSocialNetworks");
			}
			return result;
		}
		public void Share(string networkName, Dictionary<string, string> parameters)
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. Share");
			}
		}
        public bool IsCommercialReady()
        {
            bool result = false;
			#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			//
			#endif
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. Bridge IsCommercialReady: " + result);
			}
			return result;
        }
		public void CheckInternetAccess (string url)
		{
		if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. CheckInternetAccess");
			}
		}
		public void StartInternetChecking (string url, int timeInterval)
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. StartInternetChecking");
			}
		}
		public void StopInternetChecking ()
		{
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Call macOS. StopInternetChecking");
			}
		}

		#region Policy
			/// <summary>
			/// Метод запускает инициализацию модуля и выполняет запрос данных политики с сервера.
			/// </summary>
			public void StartGDPR ()
			{
				#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
				try
				{
					_startGDPR();
				}
				catch (System.Exception ex) 
				{
					if (AMEvents.amLogger != null)
						AMEvents.amLogger.LogException (ex);
				}
				#endif
				if (DebugMode)
				{
					Debug.Log ("Call macOS. StartGDPR");
				}
			}
			/// <summary>
			/// Получение статуса готовности политики к показу.
			/// </summary>
			public bool IsPolicyReady ()
			{
				bool result = false;
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call macOS. IsPolicyReady: " + result);
				}
				return result;
			}
			/// <summary>
			/// Событие начала показа окна политики.
			/// </summary>
			public bool IsPolicyShown ()
			{
				bool result = false;
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
				try 
				{
					result = _isPolicyShown ();
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
					AMEvents.amLogger.Log ("Call macOS. IsPolicyShown: " + result);
				}
				return result;
			}
			/// <summary>
			/// Надо ли показывать ShowPolicyContent.
			/// </summary>
			public bool IsBasePolicyRequired ()
			{
				bool result = false;
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
				try 
				{
					result = _isBasePolicyRequired ();
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
					AMEvents.amLogger.Log ("Call macOS. IsBasePolicyRequired: " + result);
				}
				return result;
			}
			/// <summary>
			/// Надо ли показывать ShowRegisterPolicyContent (стоит галочка в APPS).
			/// </summary>
			public bool IsRegisterPolicyRequired()
			{
				bool result = false;
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
				try 
				{
					result = _isRegisterPolicyRequired ();
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
					AMEvents.amLogger.Log ("Call macOS. IsRegisterPolicyRequired: " + result);
				}
				return result;
			}
			/// <summary>
			/// Была ли принята ShowPolicyContent.
			/// </summary>
			public bool IsBasePolicyAccepted()
			{
				bool result = false;
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
				try 
				{
					result = _isBasePolicyAccepted ();
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
					AMEvents.amLogger.Log ("Call macOS. IsBasePolicyAccepted: " + result);
				}
				return result;
			}
			/// <summary>
			/// Была ли принята ShowRegisterPolicyContent.
			/// </summary>
			public bool IsRegisterPolicyAccepted()
			{
				bool result = false;
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
				try 
				{
					result = _isRegisterPolicyAccepted ();
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
					AMEvents.amLogger.Log ("Call macOS. IsRegisterPolicyAccepted: " + result);
				}
				return result;
			}
			/// <summary>
			/// Был ли нажат отказ ShowRegisterPolicyContent.
			/// </summary>
			public bool IsRegisterPolicyRevoked()
			{
				bool result = false;
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
				try 
				{
					result = _isRegisterPolicyRevoked ();
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
					AMEvents.amLogger.Log ("Call macOS. IsRegisterPolicyRevoked: " + result);
				}
				return result;
			}
			/// <summary>
			/// Возвращает текущую ошибку в string или пустую строку.
			/// </summary>
			public string GetPolicyError ()
			{
				string result = "";
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
				try
				{
					result = _getPolicyError ();
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
					AMEvents.amLogger.Log ("Call macOS. GetPolicyError: " + result);
				}
				return result;
			}
			/// <summary>
			/// Получение ссылки на полный текст
			/// </summary>
			public string GetPrivacyUrl ()
			{
				string result = "";
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call macOS. GetPrivacyUrl: " + result);
				}
				return result;
			}
			/// <summary>
			/// Получение ссылки на Eula
			/// </summary>
			public string GetEulaUrl ()
			{
				string result = "";
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call macOS. GetEulaUrl: " + result);
				}
				return result;
			}
			/// <summary>
			/// Получение ссылки на Tos
			/// </summary>
			public string GetTosUrl ()
			{
				string result = "";
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call macOS. GetTosUrl: " + result);
				}
				return result;
			}
			/// <summary>
			/// Получение текста для регистрации.
			/// </summary>
			public string GetRegisterText ()
			{
				string result = "";
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call macOS. GetRegisterText: " + result);
				}
				return result;
			}
			/// <summary>
			/// Показ окна для принятия политики конфиденциальности.
			/// </summary>
			public void ShowPolicyContent ()
			{
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call macOS. ShowPolicyContent");
				}
						
			}

			/// <summary>
			/// Показ окна для принятия политики конфиденциальности для авторизации.
			/// </summary>
			public void ShowRegisterPolicyContent ()
			{
				#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
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
					AMEvents.amLogger.Log ("Call macOS. ShowRegisterPolicyContent");
				}
			}
			#endregion
        #endregion
    }
}