using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AnalytiAll
{
	public class AndroidBridge : INativeBridge
	{
		/// <summary>
		/// The debug mode.
		/// </summary>
		public bool DebugMode 
		{
			get;
			set;
		}

		#if UNITY_ANDROID && !UNITY_EDITOR
		static AndroidJavaClass eventGenerator;
		#endif

		public void StartApp ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}

			try
			{
				AndroidJavaObject playerActivityContext = null;
				using (var actClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer"))
				{
					if (actClass != null)
						playerActivityContext = actClass.GetStatic<AndroidJavaObject> ("currentActivity");
				}
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
					EventManager.amLogger.Log ("Call Android. StartApp");
			}
		}

		public void PauseApp ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			/*try
			{
				//AndroidJavaClass _plugin = new AndroidJavaClass ("com.ssd.analytics.LifecycleListener");
				//if (_plugin != null)
				//	_plugin.CallStatic ("onApplicationStop");
			}
			catch (System.Exception ex) 
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.LogException (ex);
			}*/
			//
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. PauseApp");
			}
		}

		public void StartScene (int sceneID, string sceneName)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateStartSceneEvent", sceneID, sceneName);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. StartScene with ID: "+sceneID+" and name: "+sceneName);
			}
		}

		public void ButtonClick (string buttonName)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateButtonClickEvent", buttonName);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. ButtonClick with name: "+buttonName);
			}
		}

		public void GameOver (string mode)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateGameOverEvent", mode);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. GameOver with mode: "+mode);
			}
		}

		public void LootAppend (string name, int volume)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateLootAppendEvent", name, volume);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. LootAppend. name: "+name+" volume: "+volume.ToString ());
			}
		}

		public void LootConsume (string name, int volume, string target)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateLootConsumeEvent", name, target, volume);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. LootConsume. name: "+name+" volume: "+volume.ToString ());
			}
		}

		public void StartLevel (string name, string mode)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateStartLevelEvent", name, mode);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. StartLevel. name: "+name+" mode: "+mode);
			}
		}

		public void LevelWin (string name, string mode)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateWinLevelEvent", name, mode);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. LevelWin. name: "+name+" mode: "+mode);
			}
		}

		public void LevelLose (string name, string mode)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateLevelLoseEvent", name, mode);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. LevelLose. name: "+name+" mode: "+mode);
			}
		}

		public void InappShopButtonClick ()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateInAppShopButtonClickEvent");
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. InappShopButtonClick");
			}
		}

		public void InappItemButtonClick (string name, string inappItemID)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateInAppItemButtonClickEvent", name, inappItemID);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. InappItemButtonClick. name: "+name+" inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseCompleted (string inappItemID)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateInAppPurchaseCompletedEvent", inappItemID);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. InappPurchaseCompleted. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseFailed (string inappItemID)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateInAppPurchaseFailedEvent", inappItemID);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. InappPurchaseFailed. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseCancelled (string inappItemID)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateInAppPurchaseCanceledEvent", inappItemID);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. InappPurchaseCancelled. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseRefunded (string inappItemID)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateInAppPurchaseRefundedEvent", inappItemID);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. InappPurchaseRefunded. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseRestored (bool result)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateInAppPurchaseRestoredEvent", result);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. InappPurchaseRestored. result: "+result.ToString ());
			}
		}

		public void LogEvent (string logEventName)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateLogEvent", logEventName);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. LogEvent. name: "+logEventName);
			}
		}

		public void LogEvent (string logEventName, Dictionary<string, string> data)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");

					using (AndroidJavaObject obj_HashMap = new AndroidJavaObject ("java.util.HashMap"))
					{
						IntPtr method_Put = AndroidJNIHelper.GetMethodID (obj_HashMap.GetRawClass (), "put",
							"(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");

						object[] args = new object[2];
						foreach (KeyValuePair<string, string> kvp in data)
						{
							using (AndroidJavaObject k = new AndroidJavaObject ("java.lang.String", kvp.Key))
							{
								using (AndroidJavaObject v = new AndroidJavaObject ("java.lang.String", kvp.Value))
								{
									args[0] = k;
									args[1] = v;
									AndroidJNI.CallObjectMethod (obj_HashMap.GetRawObject (),
										method_Put, AndroidJNIHelper.CreateJNIArgArray (args));
								}
							}
						}
						eventGenerator.CallStatic ("generateLogEvent", logEventName, obj_HashMap);
					}
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. LogEvent. name: "+logEventName+" data: "+data.ToString ());
			}
		}

		public void LogTimedEvent (string timedEventName)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateLogTimedEvent", timedEventName);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. LogTimedEvent. name: "+timedEventName);
			}
		}

		public void LogTimedEvent (string timedEventName, Dictionary<string, string> data)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");

					using (AndroidJavaObject obj_HashMap = new AndroidJavaObject ("java.util.HashMap"))
					{
						IntPtr method_Put = AndroidJNIHelper.GetMethodID (obj_HashMap.GetRawClass (), "put",
							"(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");

						object[] args = new object[2];
						foreach (KeyValuePair<string, string> kvp in data)
						{
							using (AndroidJavaObject k = new AndroidJavaObject ("java.lang.String", kvp.Key))
							{
								using (AndroidJavaObject v = new AndroidJavaObject ("java.lang.String", kvp.Value))
								{
									args[0] = k;
									args[1] = v;
									AndroidJNI.CallObjectMethod (obj_HashMap.GetRawObject (),
										method_Put, AndroidJNIHelper.CreateJNIArgArray (args));
								}
							}
						}
						eventGenerator.CallStatic ("generateLogTimedEvent", timedEventName, obj_HashMap);
					}
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. LogTimedEvent. name: "+timedEventName+" data: "+data.ToString ());
			}
		}

		public void EndTimedEvent (string timedEventName)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");
					eventGenerator.CallStatic ("generateEndTimedEvent", timedEventName);
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. EndTimedEvent. name: "+timedEventName);
			}
		}

		public void SetCustomData (Dictionary<string, string> data)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (eventGenerator != null)
			{
				try
				{
					if (eventGenerator == null)
						eventGenerator = new AndroidJavaClass ("com.ssd.analytics.EventGenerator");

					using (AndroidJavaObject obj_HashMap = new AndroidJavaObject ("java.util.HashMap"))
					{
						IntPtr method_Put = AndroidJNIHelper.GetMethodID (obj_HashMap.GetRawClass (), "put",
							"(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");

						object[] args = new object[2];
						foreach (KeyValuePair<string, string> kvp in data)
						{
							using (AndroidJavaObject k = new AndroidJavaObject ("java.lang.String", kvp.Key))
							{
								using (AndroidJavaObject v = new AndroidJavaObject ("java.lang.String", kvp.Value))
								{
									args[0] = k;
									args[1] = v;
									AndroidJNI.CallObjectMethod (obj_HashMap.GetRawObject (),
										method_Put, AndroidJNIHelper.CreateJNIArgArray (args));
								}
							}
						}
						eventGenerator.CallStatic ("generateSetCustomData", obj_HashMap);
					}
				}
				catch (System.Exception ex) 
				{
					if (EventManager.amLogger != null)
						EventManager.amLogger.LogException (ex);
				}
			}
			#endif
			if (DebugMode)
			{
				if (EventManager.amLogger != null)
					EventManager.amLogger.Log ("Call Android. SetCustomData. data: "+data.ToString ());
			}
		}
	
        /// <summary>
        /// Простой лог
        /// </summary>
        public void ANlogRevenue(string productID, float price, int quantity)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
                
            #endif
            if (DebugMode)
            {
                if (EventManager.amLogger != null)
                    EventManager.amLogger.Log ("Call Android. ANlogRevenue");
            }
        }

        /// <summary>
        /// Лог с доп параметрами
        /// </summary>
        public void ANlogRevenueWithData(string productID, float price, int quantity, string type, string receipt, string data) 
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
                
            #endif
            if (DebugMode)
            {
                if (EventManager.amLogger != null)
                    EventManager.amLogger.Log ("Call Android. ANlogRevenueWithData");
            }
        }
    }
}