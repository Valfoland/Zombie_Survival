using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
using System.Runtime.InteropServices;
#endif

namespace AnalytiAll
{
	public class iOSBridge : INativeBridge
	{
		/// <summary>
		/// The debug mode.
		/// </summary>
		public bool DebugMode 
		{
			get;
			set;
		}

		#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
		[DllImport("__Internal")]
		private static extern void  _ANstart (string path, string innerId, string url, string sid);

		[DllImport("__Internal")]
		private static extern void  _ANstop ();

		[DllImport("__Internal")]
		private static extern void  _ANstartWithoutGeo (string path, string innerId, string url, string sid);

		//add in version 2.0.0
		[DllImport("__Internal")]
		private static extern void _ANstartEventAnalytics ();

		[DllImport("__Internal")]
		private static extern void _ANbuttonClick (string buttonName);

		[DllImport("__Internal")]
		private static extern void _ANsceneStart (string sceneName, int sceneId);

		[DllImport("__Internal")]
		private static extern void _ANlevelStart (string levelName, string gameMode);

		[DllImport("__Internal")]
		private static extern void _ANlevelWin (string levelName, string gameMode);

		[DllImport("__Internal")]
		private static extern void _ANinappShopButtonClick ();

		[DllImport("__Internal")]
		private static extern void _ANinappItemButtonClick (string buttonName, string itemId);

		[DllImport("__Internal")]
		private static extern void _ANinappPurchaseCompleted (string itemId);

		[DllImport("__Internal")]
		private static extern void _ANinappPurchaseFailed (string itemId);

		[DllImport("__Internal")]
		private static extern void _ANinappPurchaseCancelled (string itemId);

		[DllImport("__Internal")]
		private static extern void _ANlootAppend (string lootName, int volume);

		[DllImport("__Internal")]
		private static extern void _ANlootConsume (string lootName, int volume, string targetName);

		[DllImport("__Internal")]
		private static extern void _ANlevelLose (string levelName, string gameMode);

		[DllImport("__Internal")]
		private static extern void _ANgameOver (string gameMode);

		[DllImport("__Internal")]
		private static extern void _ANinappPurchaseRefunded (string itemId);

		[DllImport("__Internal")]
		private static extern void _ANinappPurchaseRestored (bool result);

		//add in version 3.0.0
		[DllImport ("__Internal")]
		private static extern void _ANlogEvent (string logEventName);

		[DllImport ("__Internal")]
		private static extern void _ANlogEventWithData (string logEventName, string data);

		[DllImport ("__Internal")]
		private static extern void _ANlogTimedEvent (string timedEventName);

		[DllImport ("__Internal")]
		private static extern void _ANlogTimedEventWithData (string timedEventName, string data);

		[DllImport ("__Internal")]
		private static extern void _ANendTimedEvent (string timedEventName);

		[DllImport ("__Internal")]
		private static extern void _ANsetCustomData (string data);

        [DllImport ("__Internal")]
        private static extern void _ANlogRevenue(string productID, float price, int quantity);

        [DllImport ("__Internal")]
        private static extern void _ANlogRevenueWithData(string productID, float price, int quantity, string type, string receipt, string data);
		#endif

		public void StartApp ()
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANstartEventAnalytics ();
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. StartApp");
			}
		}

		public void PauseApp ()
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANstop ();
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. PauseApp");
			}
		}

		public void StartScene (int sceneID, string sceneName)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANsceneStart (sceneName, sceneID);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. StartScene with ID: "+sceneID+" and name: "+sceneName);
			}
		}

		public void ButtonClick (string buttonName)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANbuttonClick (buttonName);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. ButtonClick with name: "+buttonName);
			}
		}

		public void GameOver (string mode)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANgameOver (mode);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. GameOver with mode: "+mode);
			}
		}

		public void LootAppend (string name, int volume)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANlootAppend (name, volume);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. LootAppend. name: "+name+" volume: "+volume.ToString ());
			}
		}

		public void LootConsume (string name, int volume, string target)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANlootConsume (name, volume, target);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. LootConsume. name: "+name+" volume: "+volume.ToString ());
			}
		}

		public void StartLevel (string name, string mode)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANlevelStart (name, mode);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. StartLevel. name: "+name+" mode: "+mode);
			}
		}

		public void LevelWin (string name, string mode)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANlevelWin (name, mode);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. LevelWin. name: "+name+" mode: "+mode);
			}
		}

		public void LevelLose (string name, string mode)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANlevelLose (name, mode);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. LevelLose. name: "+name+" mode: "+mode);
			}
		}

		public void InappShopButtonClick ()
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANinappShopButtonClick ();
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. InappShopButtonClick");
			}
		}

		public void InappItemButtonClick (string name, string inappItemID)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANinappItemButtonClick (name, inappItemID);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. InappItemButtonClick. name: "+name+" inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseCompleted (string inappItemID)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANinappPurchaseCompleted (inappItemID);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. InappPurchaseCompleted. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseFailed (string inappItemID)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANinappPurchaseFailed (inappItemID);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. InappPurchaseFailed. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseCancelled (string inappItemID)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANinappPurchaseCancelled (inappItemID);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. InappPurchaseCancelled. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseRefunded (string inappItemID)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANinappPurchaseRefunded (inappItemID);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. InappPurchaseRefunded. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseRestored (bool result)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANinappPurchaseRestored (result);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. InappPurchaseRestored. result: "+result.ToString ());
			}
		}

		public void LogEvent (string logEventName)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANlogEvent (logEventName);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. LogEvent. name: "+logEventName);
			}
		}

		public void LogEvent (string logEventName, Dictionary<string, string> data)
		{
			string jsonData = AMUtils.AMJSON.JsonEncode (new Hashtable(data));
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANlogEventWithData (logEventName, jsonData);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. LogEvent. name: "+logEventName+" data: "+jsonData);
			}
		}

		public void LogTimedEvent (string timedEventName)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANlogTimedEvent (timedEventName);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. LogTimedEvent. name: "+timedEventName);
			}
		}

		public void LogTimedEvent (string timedEventName, Dictionary<string, string> data)
		{
			string jsonData = AMUtils.AMJSON.JsonEncode (new Hashtable(data));
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANlogTimedEventWithData (timedEventName, jsonData);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. LogTimedEvent. name: "+timedEventName+" data: "+jsonData);
			}
		}

		public void EndTimedEvent (string timedEventName)
		{
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANendTimedEvent (timedEventName);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. EndTimedEvent. name: "+timedEventName);
			}
		}

		public void SetCustomData (Dictionary<string, string> data)
		{
			string jsonData = AMUtils.AMJSON.JsonEncode (new Hashtable(data));
			#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
			try
			{
				_ANsetCustomData (jsonData);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call iOS. SetCustomData. data: "+jsonData);
			}
		}

        /// <summary>
        /// Простой лог
        /// </summary>
        public void ANlogRevenue(string productID, float price, int quantity)
        {
            #if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
                try 
                {
                    _ANlogRevenue(productID, price, quantity);
                } 
                catch (System.Exception ex)
                {
                    if (EventManager.amLogger != null)
                        EventManager.amLogger.LogException (ex);
                }
            #endif
            if (DebugMode)
            {
                if (EventManager.amLogger != null)
                    EventManager.amLogger.Log("Call iOS. ANlogRevenue. ProductID:" + productID
                    + " price:" + price.ToString() + " quantity:" + quantity.ToString());
            }
        }

        /// <summary>
        /// Лог с доп параметрами
        /// </summary>
        public void ANlogRevenueWithData(string productID, float price, int quantity, string type, string receipt, string data) 
        {
            #if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
                try 
                {
                    _ANlogRevenueWithData(productID, price, quantity, type, receipt, data);
                } 
                catch (System.Exception ex)
                {
                    if (EventManager.amLogger != null)
                        EventManager.amLogger.LogException (ex);
                }
            #endif
            if (DebugMode)
            {
                if (EventManager.amLogger != null)
                    EventManager.amLogger.Log("Call iOS. ANlogRevenueWithData. ProductID:" + productID
                    + " price:" + price.ToString() + " quantity:" + quantity.ToString() 
                    + " type:" + type + " receipt:" + receipt + " data;" + data);
            }
        }
	}
}