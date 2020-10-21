using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
using System.Reflection;
#endif

namespace AnalytiAll
{
	public class UWPBridge : INativeBridge
	{
		/// <summary>
		/// The debug mode.
		/// </summary>
		public bool DebugMode 
		{
			get;
			set;
		}

		#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
		private const string analytiallLibEventGenerator = "AnalytiAllLib.EventGenerator";
		private const string analytiallLibName = "AnalytiAllLib";
		#endif

		public void StartApp ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("onApplicationStart").Invoke (null, null);
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
					EventManager.amLogger.Log ("Call UWP. StartApp");
			}
		}

		public void PauseApp ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("onApplicationStop").Invoke (null, null);
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
					EventManager.amLogger.Log ("Call UWP. PauseApp");
			}
		}

		public void StartScene (int sceneID, string sceneName)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateStartSceneEvent").Invoke (null, new System.Object[]{sceneID, sceneName});
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
					EventManager.amLogger.Log ("Call UWP. StartScene with ID: "+sceneID+" and name: "+sceneName);
			}
		}

		public void ButtonClick (string buttonName)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateButtonClickEvent").Invoke (null, new System.Object[]{buttonName});
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
					EventManager.amLogger.Log ("Call UWP. ButtonClick with name: "+buttonName);
			}
		}

		public void GameOver (string mode)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateGameOverEvent").Invoke (null, new System.Object[]{mode});
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
					EventManager.amLogger.Log ("Call UWP. GameOver with mode: "+mode);
			}
		}

		public void LootAppend (string name, int volume)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateLootAppendEvent").Invoke (null, new System.Object[]{name, volume});
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
					EventManager.amLogger.Log ("Call UWP. LootAppend. name: "+name+" volume: "+volume.ToString ());
			}
		}

		public void LootConsume (string name, int volume, string target)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateLootConsumeEvent").Invoke (null, new System.Object[]{name, target, volume});
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
					EventManager.amLogger.Log ("Call UWP. LootConsume. name: "+name+" volume: "+volume.ToString ());
			}
		}

		public void StartLevel (string name, string mode)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateStartLevelEvent").Invoke (null, new System.Object[]{name, mode});
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
					EventManager.amLogger.Log ("Call UWP. StartLevel. name: "+name+" mode: "+mode);
			}
		}

		public void LevelWin (string name, string mode)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateWinLevelEvent").Invoke (null, new System.Object[]{name, mode});
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
					EventManager.amLogger.Log ("Call UWP. LevelWin. name: "+name+" mode: "+mode);
			}
		}

		public void LevelLose (string name, string mode)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateLevelLoseEvent").Invoke (null, new System.Object[]{name, mode});
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
					EventManager.amLogger.Log ("Call UWP. LevelLose. name: "+name+" mode: "+mode);
			}
		}

		public void InappShopButtonClick ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateInAppShopButtonClickEvent").Invoke (null, null);
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
					EventManager.amLogger.Log ("Call UWP. InappShopButtonClick");
			}
		}

		public void InappItemButtonClick (string name, string inappItemID)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateInAppItemButtonClickEvent").Invoke (null, new System.Object[]{name, inappItemID});
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
					EventManager.amLogger.Log ("Call UWP. InappItemButtonClick. name: "+name+" inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseCompleted (string inappItemID)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateInAppPurchaseCompletedEvent").Invoke (null, new System.Object[]{inappItemID});
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
					EventManager.amLogger.Log ("Call UWP. InappPurchaseCompleted. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseFailed (string inappItemID)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateInAppPurchaseFailedEvent").Invoke (null, new System.Object[]{inappItemID});
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
					EventManager.amLogger.Log ("Call UWP. InappPurchaseFailed. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseCancelled (string inappItemID)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateInAppPurchaseCanceledEvent").Invoke (null, new System.Object[]{inappItemID});
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
					EventManager.amLogger.Log ("Call UWP. InappPurchaseCancelled. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseRefunded (string inappItemID)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateInAppPurchaseRefundedEvent").Invoke (null, new System.Object[]{inappItemID});
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
					EventManager.amLogger.Log ("Call UWP. InappPurchaseRefunded. inappItemID: "+inappItemID);
			}
		}

		public void InappPurchaseRestored (bool result)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateInAppPurchaseRestoredEvent").Invoke (null, new System.Object[]{result});
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
					EventManager.amLogger.Log ("Call UWP. InappPurchaseRestored. result: "+result.ToString ());
			}
		}

		public void LogEvent (string logEventName)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateLogEvent").Invoke (null, new System.Object[]{logEventName});
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
					EventManager.amLogger.Log ("Call UWP. LogEvent. name: "+logEventName);
			}
		}

		public void LogEvent (string logEventName, Dictionary<string, string> data)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateLogEvent").Invoke (null, new System.Object[]{logEventName, data});
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
					EventManager.amLogger.Log ("Call UWP. LogEvent. name: "+logEventName+" data: "+data.ToString ());
			}
		}

		public void LogTimedEvent (string timedEventName)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateLogTimedEvent").Invoke (null, new System.Object[]{timedEventName});
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
					EventManager.amLogger.Log ("Call UWP. LogTimedEvent. name: "+timedEventName);
			}
		}

		public void LogTimedEvent (string timedEventName, Dictionary<string, string> data)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateLogTimedEvent").Invoke (null, new System.Object[]{timedEventName, data});
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
					EventManager.amLogger.Log ("Call UWP. LogTimedEvent. name: "+timedEventName+" data: "+data.ToString ());
			}
		}

		public void EndTimedEvent (string timedEventName)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateEndTimedEvent").Invoke (null, new System.Object[]{timedEventName});
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
					EventManager.amLogger.Log ("Call UWP. EndTimedEvent. name: "+timedEventName);
			}
		}

		public void SetCustomData (Dictionary<string, string> data)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			try
			{
				AssemblyName assmName = new AssemblyName (analytiallLibName);
				Assembly assm = Assembly.Load (assmName);
				Type type = assm.GetType (analytiallLibEventGenerator);
				type.GetMethod ("generateSetCustomData").Invoke (null, new System.Object[]{data});
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
					EventManager.amLogger.Log ("Call UWP. SetCustomData. data: "+data.ToString ());
			}
		}

        /// <summary>
        /// Простой лог
        /// </summary>
        public void ANlogRevenue(string productID, float price, int quantity)
        {
            #if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
                
            #endif
            if (DebugMode)
            {
                if (EventManager.amLogger != null)
                    EventManager.amLogger.Log ("Call UWP. ANlogRevenue.");
            }
        }

        /// <summary>
        /// Лог с доп параметрами
        /// </summary>
        public void ANlogRevenueWithData(string productID, float price, int quantity, string type, string receipt, string data) 
        {
            #if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
                
            #endif
            if (DebugMode)
            {
                if (EventManager.amLogger != null)
                    EventManager.amLogger.Log ("Call UWP. ANlogRevenue.");
            }
        }
	}
}