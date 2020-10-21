using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AMUtils;

namespace AMEvents
{
	class NativeBridge
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
			static INativeBridge nativeBridge = new AndroidBridge ();
		#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			static INativeBridge nativeBridge = new iOSBridge ();
		#elif !UNITY_EDITOR && UNITY_STANDALONE_OSX
			static INativeBridge nativeBridge = new MacBridge ();
		#elif !UNITY_EDITOR && UNITY_TVOS
			static INativeBridge nativeBridge = new tvOSBridge ();
		#elif UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			static INativeBridge nativeBridge = new UWPBridge ();
		#else
			static INativeBridge nativeBridge = new MockBridge ();
		#endif
		public static bool GetDebugMode ()
		{
			return nativeBridge.DebugMode;
		}
		public static void SetDebugMode (bool debugMode)
		{
			nativeBridge.DebugMode = debugMode;
		}
		public static void NewSceneEvent ()
		{
			nativeBridge.NewSceneEvent ();
		}
		public static List<string> GetSocialNetworks ()
		{
			string parts = nativeBridge.GetSocialNetworks();
			List<string> listSocia = new List<string>();
			if (parts == "") {
				return listSocia;
			} 
			JSONArray listKey;
 			JSONNode jsonNode; 
			jsonNode = JSON.Parse (parts); 
			listKey = jsonNode.AsArray;
			int countKey = listKey.Count;
			for (int i = 0; i <= countKey; i++ ) {
					listSocia.Add(listKey[i].Value);
			}
			return listSocia;
		}

		#region Sharing
			public static void Share (string networkName, Dictionary<string, string> parameters)
			{
				nativeBridge.Share (networkName, parameters);
			}
		#endregion

		#region App Events API
			public static void PauseSceneEvent ()
			{
				nativeBridge.PauseSceneEvent ();
			}
			public static void ResumeSceneEvent ()
			{
				nativeBridge.ResumeSceneEvent ();
			}
			public static void ApplicationInactiveEvent ()
			{
				nativeBridge.ApplicationInactiveEvent ();
			}
			public static void OrientationChangeEvent ()
			{
				nativeBridge.OrientationChangeEvent ();
			}
		#endregion

		#region Internet
			public static void CheckInternetAccess (string url)
			{
				nativeBridge.CheckInternetAccess (url);
			}
			public static void StartInternetChecking (string url, int timeInterval)
			{
				nativeBridge.StartInternetChecking (url, timeInterval);
			}
			public static void StopInternetChecking ()
			{
				nativeBridge.StopInternetChecking ();
			}
		#endregion

		#region Ad
			public static void AddAdListener (string gameObjectName, string bannerType)
			{
				nativeBridge.AddAdListener (gameObjectName, bannerType);
			}
			public static void RemoveAdListener (string gameObjectName, string bannerType)
			{
				nativeBridge.RemoveAdListener (gameObjectName, bannerType);
			}
			public static void EnableCross (string gameObjectName)
			{
				nativeBridge.EnableCross (gameObjectName);
			}
			public static bool IsAdDisabled ()
			{
				return nativeBridge.IsAdDisabled ();
			}
			public static void DisableAd (string gameObjectName)
			{
				nativeBridge.DisableAd (gameObjectName);
			}
			public static void EnableAd (string gameObjectName)
			{
				nativeBridge.EnableAd (gameObjectName);
			}
			public static void SetBannerVisible (bool value)
			{
				nativeBridge.SetBannerVisible (value);
			}
		#endregion

		#region Interstitial
			public static bool SetInterstitialListener (string gameObjectName)
			{
				return nativeBridge.SetInterstitialListener (gameObjectName);
			}
			public static void RemoveInterstitialListener ()
			{
				nativeBridge.RemoveInterstitialListener ();
			}
			public static bool IsInterstitialReady ()
			{
				return nativeBridge.IsInterstitialReady ();
			}
			public static void ShowInterstitial ()
			{
				nativeBridge.ShowInterstitial ();
			}
			public static bool setInterstitialDelay (int seconds)
			{
				return nativeBridge.setInterstitialDelay (seconds);
			}
			public static int getCurrentInterstitialDelay ()
			{
				return nativeBridge.getCurrentInterstitialDelay ();
			}
			public static int TimeAfterInterstitial ()
			{
				return nativeBridge.TimeAfterInterstitial ();
			}
			public static bool IsCommercialReady ()
			{
				return nativeBridge.IsCommercialReady ();
			}
		#endregion

		#region Reward Video
			public static void ShowRewardedVideo ()
			{	
				nativeBridge.ShowRewardedVideo ();
			}
			public static bool IsRewardedVideoReady ()
			{
				return nativeBridge.IsRewardedVideoReady ();
			}
		#endregion

		#region Standard Banner
			public static int GetBannerWidth ()
			{
				return nativeBridge.GetBannerWidth ();
			}
			public static int GetBannerHeight ()
			{
				return nativeBridge.GetBannerHeight ();
			}
		#endregion

		#region Inner InApp
			public static void ShowInnerInApp ()
			{
				nativeBridge.ShowInnerInApp ();
			}
			public static void BuyInnerInApp ()
			{
				nativeBridge.BuyInnerInApp ();
			}
			public static void RestoreInnerInApp ()
			{
				nativeBridge.RestoreInnerInApp ();
			}
		#endregion
		
		#region Policy
			public static void ShowPolicyContent ()
			{
				nativeBridge.ShowPolicyContent ();
			}
			public static void ShowRegisterPolicyContent ()
			{
				nativeBridge.ShowRegisterPolicyContent ();
			}
			public static string GetRegisterText ()
			{
				return nativeBridge.GetRegisterText ();
			}
			public static string GetPrivacyUrl ()
			{
				return nativeBridge.GetPrivacyUrl ();
			}
			public static string GetEulaUrl ()
			{
				return nativeBridge.GetEulaUrl ();
			}
			public static string GetTosUrl ()
			{
				return nativeBridge.GetTosUrl ();
			}
			public static bool IsPolicyReady ()
			{
				return nativeBridge.IsPolicyReady ();
			}
			public static void StartGDPR ()
			{
				nativeBridge.StartGDPR ();
			}
			public static bool IsPolicyShown ()
			{
				return nativeBridge.IsPolicyShown ();
			}
			public static bool IsBasePolicyRequired ()
			{
				return nativeBridge.IsBasePolicyRequired ();
			}
			public static bool IsRegisterPolicyRequired ()
			{
				return nativeBridge.IsRegisterPolicyRequired ();
			}
			public static bool IsBasePolicyAccepted ()
			{
				return nativeBridge.IsBasePolicyAccepted ();
			}
			public static bool IsRegisterPolicyAccepted ()
			{
				return nativeBridge.IsRegisterPolicyAccepted ();
			}
			public static bool IsRegisterPolicyRevoked ()
			{
				return nativeBridge.IsRegisterPolicyRevoked ();
			}
			public static string GetPolicyError ()
			{
				return nativeBridge.GetPolicyError ();
			}
		#endregion
	}

	interface INativeBridge
	{
		#region Debug
			/// <summary>
			/// The debug mode.
			/// </summary>
			bool DebugMode 
			{
				get;
				set;
			}
		#endregion

		#region App Events API
			/// <summary>
			/// News the scene event.
			/// </summary>
			void NewSceneEvent ();

			/// <summary>
			/// Pauses the scene event.
			/// </summary>
			void PauseSceneEvent ();

			/// <summary>
			/// Resumes the scene event.
			/// </summary>
			void ResumeSceneEvent ();

			/// <summary>
			/// Applications the inactive event.
			/// </summary>
			void ApplicationInactiveEvent ();

			/// <summary>
			/// Orientations the change event.
			/// </summary>
			void OrientationChangeEvent ();
		#endregion

		#region Ad
			/// <summary>
			/// Adds the ad listener.
			/// </summary>
			/// <param name="bannerType">Banner type.</param>
			void AddAdListener (string gameObjectName, string bannerType);

			/// <summary>
			/// Removes the ad listener.
			/// </summary>
			/// <param name="bannerType">Banner type.</param>
			void RemoveAdListener (string gameObjectName, string bannerType);

			/// <summary>
			/// Enables the cross for disabling ads.
			/// </summary>
			/// <param name="gameObjectName">Game object name.</param>
			void EnableCross (string gameObjectName);

			/// <summary>
			/// Determines whether this instance is ad disabled.
			/// </summary>
			/// <returns><c>true</c> if this instance is ad disabled; otherwise, <c>false</c>.</returns>
			bool IsAdDisabled ();

			/// <summary>
			/// Disables ads.
			/// </summary>
			/// <param name="gameObjectName">Game object name.</param>
			void DisableAd (string gameObjectName);

			/// <summary>
			/// Enables ads.
			/// </summary>
			/// <param name="gameObjectName">Game object name.</param>
			void EnableAd (string gameObjectName);

			/// <summary>
			/// Sets the banner visibility.
			/// </summary>
			/// <param name="value">If set to <c>true</c> value.</param>
			void SetBannerVisible (bool value);

			/// <summary>
			/// Sets the interstitial listener.
			/// </summary>
			/// <param name="gameObjectName">Game object name.</param>
			bool SetInterstitialListener (string gameObjectName);

			/// <summary>
			/// Removes the interstitial listener.
			/// </summary>
			void RemoveInterstitialListener ();

			/// <summary>
			/// Determines whether this instance is interstitial ready.
			/// </summary>
			/// <returns><c>true</c> if this instance is interstitial ready; otherwise, <c>false</c>.</returns>
			bool IsInterstitialReady ();

			/// <summary>
			/// Shows the interstitial.
			/// </summary>
			void ShowInterstitial ();

			/// <summary>
			/// Shows the interstitial.
			/// </summary>
			bool setInterstitialDelay (int Delay);

			int getCurrentInterstitialDelay ();

			/// <summary>
			/// Returns time the after latest interstitial closes in seconds.
			/// </summary>
			/// <returns>Time in seconds.</returns>
			int TimeAfterInterstitial ();

			/// <summary>
			/// Shows the rewarded video.
			/// </summary>
			void ShowRewardedVideo ();

			/// <summary>
			/// Determines whether this instance is rewarded video ready.
			/// </summary>
			/// <returns><c>true</c> if this instance is rewarded video ready; otherwise, <c>false</c>.</returns>
			bool IsRewardedVideoReady ();
			bool IsCommercialReady ();

			/// <summary>
			/// Gets the height of the banner.
			/// </summary>
			/// <returns>The banner height.</returns>
			int GetBannerWidth ();

			/// <summary>
			/// Gets the height of the banner.
			/// </summary>
			/// <returns>The banner height.</returns>
			int GetBannerHeight ();

			/// <summary>
			/// Shows the inner in app window if Inner InApp is available.
			/// </summary>
			void ShowInnerInApp ();

			/// <summary>
			/// Make a purchase of the Inner InApp.
			/// </summary>
			void BuyInnerInApp ();

			/// <summary>
			/// Restores the Inner InApp purchase.
			/// </summary>
			void RestoreInnerInApp ();
		#endregion

		#region Policy
			/// <summary>
			/// Показ окна политики.
			/// </summary>
			void ShowPolicyContent ();

			/// <summary>
			/// Показ окна политики при авторизации.
			/// </summary>
			void ShowRegisterPolicyContent ();

			/// <summary>
			/// Получение ссылки на полный текст.
			/// </summary>
			string GetPrivacyUrl();

			/// <summary>
			/// Получение ссылки на Eula.
			/// </summary>
			string GetEulaUrl();

			/// <summary>
			/// Получение ссылки на Tos.
			/// </summary>
			string GetTosUrl();

			/// <summary>
			/// Получение текста политики.
			/// </summary>
			string GetRegisterText();

			/// <summary>
			/// Получение готовности окна политики к показу.
			/// </summary>
			bool IsPolicyReady ();

			/// <summary>
			/// Метод запускает инициализацию модуля и выполняет запрос данных политики с сервера.
			/// </summary>
			void StartGDPR ();

			/// <summary>
			/// Событие начала показа окна политики.
			/// </summary>
			bool IsPolicyShown ();

			/// <summary>
			/// Надо ли показывать ShowPolicyContent.
			/// </summary>
			bool IsBasePolicyRequired ();

			/// <summary>
			/// Надо ли показывать ShowRegisterPolicyContent.
			/// </summary>
			bool IsRegisterPolicyRequired ();

			/// <summary>
			/// Была ли принята ShowPolicyContent.
			/// </summary>
			bool IsBasePolicyAccepted ();

			/// <summary>
			/// Была ли принята ShowRegisterPolicyContent.
			/// </summary>
			bool IsRegisterPolicyAccepted ();

			/// <summary>
			/// Был ли нажат отказ ShowRegisterPolicyContent.
			/// </summary>
			bool IsRegisterPolicyRevoked ();

			/// <summary>
			/// Возвращает текущую ошибку в string или пустую строку.
			/// </summary>
			string GetPolicyError ();
		#endregion

		#region Internet
			void CheckInternetAccess (string url);
			void StartInternetChecking (string url, int timeInterval);
			void StopInternetChecking ();
		#endregion

		#region SocialAPI
			string GetSocialNetworks ();
		#endregion

		#region Sharing
			void Share (string networkName, Dictionary<string, string> parameters);
		#endregion
	}
}