using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
using System.Runtime.InteropServices;
#endif

namespace AnalytiAll
{
	public class MacBridge : INativeBridge
	{
		/// <summary>
		/// The debug mode.
		/// </summary>
		public bool DebugMode 
		{
			get;
			set;
		}

		#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
		[DllImport ("AnalytiAllOSX")]
		private static extern void  _ANstart (string path, string innerId, string url, string sid);

		[DllImport ("AnalytiAllOSX")]
		private static extern void  _ANstop ();

		[DllImport ("AnalytiAllOSX")]
		private static extern void  _ANstartWithoutGeo (string path, string innerId, string url, string sid);

		//add version 2.0.0
		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANstartEventAnalytics ();

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANbuttonClick (string buttonName);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANsceneStart (string sceneName, int sceneId);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANlevelStart (string levelName, string gameMode);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANlevelWin (string levelName, string gameMode);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANinappShopButtonClick ();

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANinappItemButtonClick (string buttonName, string itemId);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANinappPurchaseCompleted (string itemId);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANinappPurchaseFailed (string itemId);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANinappPurchaseCancelled (string itemId);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANlootAppend (string lootName, int volume);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANlootConsume (string lootName, int volume, string targetName);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANlevelLose (string levelName, string gameMode);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANgameOver (string gameMode);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANinappPurchaseRefunded (string itemId);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANinappPurchaseRestored (bool result);

		[DllImport ("AnalytiAllOSX")]
		private static extern void _ANcustom (string data);

//		[DllImport ("AnalytiAllOSX")]
//		private static extern void _ANsetCustomData (string data);
		#endif

		public void StartApp ()
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. StartApp");
			}
		}

		public void PauseApp ()
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. PauseApp");
			}
		}

		public void StartScene (int sceneID, string sceneName)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. StartScene with ID: " + sceneID + " and name: " + sceneName);
			}
		}

		public void ButtonClick (string buttonName)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. ButtonClick with name: " + buttonName);
			}
		}

		public void GameOver (string mode)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. GameOver with mode: " + mode);
			}
		}

		public void LootAppend (string name, int volume)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. LootAppend. name: " + name + " volume: " + volume.ToString ());
			}
		}

		public void LootConsume (string name, int volume, string target)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. LootConsume. name: " + name + " volume: " + volume.ToString ());
			}
		}

		public void StartLevel (string name, string mode)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. StartLevel. name: " + name + " mode: " + mode);
			}
		}

		public void LevelWin (string name, string mode)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. LevelWin. name: " + name + " mode: " + mode);
			}
		}

		public void LevelLose (string name, string mode)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. LevelLose. name: " + name + " mode: " + mode);
			}
		}

		public void InappShopButtonClick ()
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. InappShopButtonClick");
			}
		}

		public void InappItemButtonClick (string name, string inappItemID)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. InappItemButtonClick. name: " + name + " inappItemID: " + inappItemID);
			}
		}

		public void InappPurchaseCompleted (string inappItemID)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. InappPurchaseCompleted. inappItemID: " + inappItemID);
			}
		}

		public void InappPurchaseFailed (string inappItemID)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. InappPurchaseFailed. inappItemID: " + inappItemID);
			}
		}

		public void InappPurchaseCancelled (string inappItemID)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. InappPurchaseCancelled. inappItemID: " + inappItemID);
			}
		}

		public void InappPurchaseRefunded (string inappItemID)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. InappPurchaseRefunded. inappItemID: " + inappItemID);
			}
		}

		public void InappPurchaseRestored (bool result)
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
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
				Debug.Log ("Call macOS. InappPurchaseRestored. result: "+result.ToString ());
			}
		}

		public void LogEvent (string logEventName)
		{
//			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
//			try
//			{
//				_ANlogEvent (logEventName);
//			}
//			catch (System.Exception ex) 
//			{
//				if (EventManager.amLogger != null)
//					EventManager.amLogger.LogException (ex);
//			}
//			#endif
//			if (DebugMode)
//			{
//				Debug.Log ("Call macOS. LogEvent. name: " + logEventName);
//			}
		}

		public void LogEvent (string logEventName, Dictionary<string, string> data)
		{
			string jsonData = AMUtils.AMJSON.JsonEncode (new Hashtable(data));
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
			try
			{
				_ANcustom (jsonData);
//				_ANlogEventWithData (logEventName, jsonData);
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}
			#endif
			if (DebugMode)
			{
				Debug.Log ("Call macOS. _ANcustom. name: " + logEventName + " data: " + jsonData);
			}
		}

		public void LogTimedEvent (string timedEventName)
		{
//			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
//			try
//			{
//				_ANlogTimedEvent (timedEventName);
//			}
//			catch (System.Exception ex) 
//			{
//				if (EventManager.amLogger != null)
//					EventManager.amLogger.LogException (ex);
//			}
//			#endif
//			if (DebugMode)
//			{
//				Debug.Log ("Call macOS. LogTimedEvent. name: "+timedEventName);
//			}
		}

		public void LogTimedEvent (string timedEventName, Dictionary<string, string> data)
		{
//			string jsonData = AMUtils.AMJSON.JsonEncode (new Hashtable(data));
//			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
//			try
//			{
//				_ANlogTimedEventWithData (timedEventName, jsonData);
//			}
//			catch (System.Exception ex) 
//			{
//				if (EventManager.amLogger != null)
//					EventManager.amLogger.LogException (ex);
//			}
//			#endif
//			if (DebugMode)
//			{
//				Debug.Log ("Call macOS. LogTimedEvent. name: " + timedEventName + " data: " + jsonData);
//			}
		}

		public void EndTimedEvent (string timedEventName)
		{
//			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
//			try
//			{
//				_ANendTimedEvent (timedEventName);
//			}
//			catch (System.Exception ex) 
//			{
//				if (EventManager.amLogger != null)
//					EventManager.amLogger.LogException (ex);
//			}
//			#endif
//			if (DebugMode)
//			{
//				Debug.Log ("Call macOS. EndTimedEvent. name: " + timedEventName);
//			}
		}

		public void SetCustomData (Dictionary<string, string> data)
		{
//			string jsonData = AMUtils.AMJSON.JsonEncode (new Hashtable(data));
//			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
//			try
//			{
//				_ANsetCustomData (jsonData);
//			}
//			catch (System.Exception ex) 
//			{
//				if (EventManager.amLogger != null)
//					EventManager.amLogger.LogException (ex);
//			}
//			#endif
//			if (DebugMode)
//			{
//				Debug.Log ("Call macOS. SetCustomData. data: " + jsonData);
//			}			
		}

        /// <summary>
        /// Простой лог
        /// </summary>
        public void ANlogRevenue(string productID, float price, int quantity)
        {
            #if UNITY_STANDALONE_OSX && !UNITY_EDITOR
                
            #endif
            if (DebugMode)
            {
                Debug.Log("Call macOS. ANlogRevenue. ");
            }
        }

        /// <summary>
        /// Лог с доп параметрами
        /// </summary>
        public void ANlogRevenueWithData(string productID, float price, int quantity, string type, string receipt, string data) 
        {
            #if UNITY_STANDALONE_OSX && !UNITY_EDITOR
                
            #endif
            if (DebugMode)
            {
                Debug.Log("Call macOS. ANlogRevenueWithData. ");
            }
        }
	}
}