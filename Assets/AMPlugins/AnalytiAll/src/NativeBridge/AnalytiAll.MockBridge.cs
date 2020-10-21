using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AnalytiAll
{
	public class MockBridge : INativeBridge
	{
		/// <summary>
		/// The debug mode.
		/// </summary>
		public bool DebugMode 
		{
			get;
			set;
		}

		public void StartApp ()
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. StartApp");
			}
		}

		public void PauseApp ()
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. PauseApp");
			}
		}

		public void ResumeApp ()
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. ResumeApp");
			}
		}

		public string GetAppID ()
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. GetAppID");
			}
			return string.Empty;
		}

		public string GetURL ()
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. GetURL");
			}
			return string.Empty;
		}

		public string GetDeviceInfo ()
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. GetDeviceInfo");
			}
			return string.Empty;
		}

		public string GetGeoInfo ()
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. GetGeoInfo");
			}
			return string.Empty;
		}

		public string GetAppInfo()
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. GetAppInfo");
			}
			return string.Empty;
		}

		public void StartScene (int sceneID, string sceneName)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. StartScene with ID: "+sceneID+" and name: "+sceneName);
			}
		}

		public void ButtonClick (string buttonName)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. ButtonClick with name: "+buttonName);
			}
		}

		public void GameOver (string mode)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. GameOver with mode: "+mode);
			}
		}

		public void LootAppend (string name, int volume)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. LootAppend. name: "+name+" volume: "+volume.ToString ());
			}
		}

		public void LootConsume (string name, int volume, string target)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. LootConsume. name: "+name+" volume: "+volume.ToString ());
			}
		}

		public void StartLevel (string name, string mode)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. StartLevel. name: "+name+" mode: "+mode);
			}
		}

		public void LevelWin (string name, string mode)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. LevelWin. name: "+name+" mode: "+mode);
			}
		}

		public void LevelLose (string name, string mode)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. LevelLose. name: "+name+" mode: "+mode);
			}
		}

		public void InappShopButtonClick ()
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. InappShopButtonClick");
			}
		}

		public void InappItemButtonClick (string name, string inappItemID)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. InappItemButtonClick. name: "+name+" inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseCompleted (string inappItemID)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. InappPurchaseCompleted. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseFailed (string inappItemID)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. InappPurchaseFailed. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseCancelled (string inappItemID)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. InappPurchaseCancelled. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseRefunded (string inappItemID)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. InappPurchaseRefunded. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseRestored (bool result)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. InappPurchaseRestored. result: "+result.ToString ());
			}
		}

		public void LogEvent (string logEventName)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. LogEvent. name: "+logEventName);
			}
		}

		public void LogEvent (string logEventName, Dictionary<string, string> data)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. LogEvent. name: "+logEventName+" data: "+data.ToString ());
			}
		}

		public void LogTimedEvent (string timedEventName)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. LogTimedEvent. name: "+timedEventName);
			}
		}

		public void LogTimedEvent (string timedEventName, Dictionary<string, string> data)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. LogTimedEvent. name: "+timedEventName+" data: "+data.ToString ());
			}
		}

		public void EndTimedEvent (string timedEventName)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. EndTimedEvent. name: "+timedEventName);
			}
		}

		public void SetCustomData (Dictionary<string, string> data)
		{
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Mock. SetCustomData. data: "+data.ToString ());
			}
		}

        /// <summary>
        /// Простой лог
        /// </summary>
        public void ANlogRevenue(string productID, float price, int quantity)
        {
            if (DebugMode)
            {
                if (EventManager.amLogger != null)
                    EventManager.amLogger.Log("Call Mock. ANlogRevenue.");
            }
        }

        /// <summary>
        /// Лог с доп параметрами
        /// </summary>
        public void ANlogRevenueWithData(string productID, float price, int quantity, string type, string receipt, string data)
        {
            if (DebugMode)
            {
                if (EventManager.amLogger != null)
                    EventManager.amLogger.Log("Call Mock. ANlogRevenueWithData.");
            }
        }
    }
}