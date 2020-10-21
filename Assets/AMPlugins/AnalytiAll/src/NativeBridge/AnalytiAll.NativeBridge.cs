#pragma warning disable
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS)
using System.Runtime.InteropServices;
#endif

namespace AnalytiAll
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

		public static void SetDebugMode (bool debugMode)
		{
			nativeBridge.DebugMode = debugMode;
		}

		public static void StartApp ()
		{
			nativeBridge.StartApp ();
		}

		public static void PauseApp ()
		{
			nativeBridge.PauseApp ();
		}

		public static void ResumeApp ()
		{
			nativeBridge.StartApp ();
		}

		public static string GetAppID ()
		{
			return NativeInfo.GetAppID ();
		}

		public static string GetURL ()
		{
			return NativeInfo.GetURL ();
		}

		public static string GetDeviceInfo ()
		{
			return NativeInfo.GetDeviceInfo ();
		}

		public static string GetGeoInfo ()
		{
			return NativeInfo.GetGeoInfo ();
		}

		public static string GetAppInfo()
		{
			return NativeInfo.GetAppInfo ();
		}

		public static void StartScene (int sceneID, string sceneName)
		{
			nativeBridge.StartScene (sceneID, sceneName);
		}

		public static void ButtonClick (string buttonName)
		{
			nativeBridge.ButtonClick (buttonName);
		}

		public static void GameOver (string mode)
		{
			nativeBridge.GameOver (mode);
		}

		public static void LootAppend (string name, int volume)
		{
			nativeBridge.LootAppend (name, volume);
		}

		public static void LootConsume (string name, int volume, string target)
		{
			nativeBridge.LootConsume (name, volume, target);
		}

		public static void StartLevel (string name, string mode)
		{
			nativeBridge.StartLevel (name, mode);
		}

		public static void LevelWin (string name, string mode)
		{
			nativeBridge.LevelWin (name, mode);
		}

		public static void LevelLose (string name, string mode)
		{
			nativeBridge.LevelLose (name, mode);
		}

		public static void InappShopButtonClick ()
		{
			nativeBridge.InappShopButtonClick ();
		}

		public static void InappItemButtonClick (string name, string inappItemID)
		{
			nativeBridge.InappItemButtonClick (name, inappItemID);
		}

		public static void InappPurchaseCompleted (string inappItemID)
		{
			nativeBridge.InappPurchaseCompleted (inappItemID);
		}

		public static void InappPurchaseFailed (string inappItemID)
		{
			nativeBridge.InappPurchaseFailed (inappItemID);
		}

		public static void InappPurchaseCancelled (string inappItemID)
		{
			nativeBridge.InappPurchaseCancelled (inappItemID);
		}

		public static void InappPurchaseRefunded (string inappItemID)
		{
			nativeBridge.InappPurchaseRefunded (inappItemID);
		}

		public static void InappPurchaseRestored (bool result)
		{
			nativeBridge.InappPurchaseRestored (result);
		}

		public static void LogEvent (string logEventName)
		{
			nativeBridge.LogEvent (logEventName);
		}

		public static void LogEvent (string logEventName, Dictionary<string, string> data)
		{
			nativeBridge.LogEvent (logEventName, data);
		}

		public static void LogTimedEvent (string timedEventName)
		{
			nativeBridge.LogTimedEvent (timedEventName);
		}

		public static void LogTimedEvent (string timedEventName, Dictionary<string, string> data)
		{
			nativeBridge.LogTimedEvent (timedEventName, data);
		}

		public static void EndTimedEvent (string timedEventName)
		{
			nativeBridge.EndTimedEvent (timedEventName);
		}

		public static void SetCustomData (Dictionary<string, string> data)
		{
			nativeBridge.SetCustomData (data);
		}

        public static void ANlogRevenue(string productID, float price, int quantity)
        {
            nativeBridge.ANlogRevenue(productID, price, quantity);
        }

        public static void ANlogRevenueWithData(string productID, float price, int quantity, string type, string receipt, string data)
        {
            nativeBridge.ANlogRevenueWithData(productID, price, quantity, type, receipt, data);
        }
    }

	interface INativeBridge
	{
		bool DebugMode 
		{
			get;
			set;
		}

		void StartApp ();

		void PauseApp ();

		void StartScene (int sceneID, string sceneName);

		void ButtonClick (string buttonName);

		void GameOver (string mode);

		void LootAppend (string name, int volume);

		void LootConsume (string name, int volume, string target);

		void StartLevel (string name, string mode);

		void LevelWin (string name, string mode);

		void LevelLose (string name, string mode);

		void InappShopButtonClick ();

		void InappItemButtonClick (string name, string inappItemID);

		void InappPurchaseCompleted (string inappItemID);

		void InappPurchaseFailed (string inappItemID);

		void InappPurchaseCancelled (string inappItemID);

		void InappPurchaseRefunded (string inappItemID);

		void InappPurchaseRestored (bool result);

		void LogEvent (string logEventName);

		void LogEvent (string logEventName, Dictionary<string, string> data);

		void LogTimedEvent (string timedEventName);

		void LogTimedEvent (string timedEventName, Dictionary<string, string> data);

		void EndTimedEvent (string timedEventName);

		void SetCustomData (Dictionary<string, string> data);

        /// <summary>
        /// Простой лог
        /// </summary>
        void ANlogRevenue(string productID, float price, int quantity);

        /// <summary>
        /// Лог с доп параметрами
        /// </summary>
        void ANlogRevenueWithData(string productID, float price, int quantity, string type, string receipt, string data);
    }

	public class NativeInfo
	{
		static string appId = string.Empty;
		static string statUrl = string.Empty;
		static string deviceInfo = string.Empty;
		static string geoInfo = string.Empty;
		static string appInfo = string.Empty;
		static string payment = string.Empty;

		static List<string> ALLOWED_TYPES_OF_PAYMENT = new List<string> {};

		/// <summary>
		/// Установка параметров
		/// </summary>
		/// <param name="_appID">ID приложения</param>
		/// <param name="_payment">Тип оплаты</param>
		public static void SetParams (string _appID,string _payment)
		{
			appId = _appID;
			payment = _payment;
		}

		/// <summary>
		/// Gets the app ID.
		/// </summary>
		/// <returns>The app ID.</returns>
		public static string GetAppID ()
		{
			return appId;
		}
		/// <summary>
		/// Gets the URL.
		/// </summary>
		/// <returns>The URL.</returns>
		public static string GetURL ()
		{
			return statUrl;
		}
		/// <summary>
		/// Gets the device info.
		/// </summary>
		/// <returns>The device info.</returns>
		public static string GetDeviceInfo ()
		{
			return deviceInfo;
		}
		/// <summary>
		/// Gets the geo info.
		/// </summary>
		/// <returns>The geo info.</returns>
		public static string GetGeoInfo ()
		{
			return geoInfo;
		}
		/// <summary>
		/// Gets the app info.
		/// </summary>
		/// <returns>The app info.</returns>
		public static string GetAppInfo ()
		{
			return appInfo;
		}
	}
}