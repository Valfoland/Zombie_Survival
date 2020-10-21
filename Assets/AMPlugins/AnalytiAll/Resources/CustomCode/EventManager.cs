#pragma warning disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using UnityEngine;
using AMLogging;
using UnityEngine.SceneManagement;

namespace AnalytiAll
{
    public delegate void EmptyEventHandler ();
    /// <summary>
	/// Класс для сбора статистики использования приложения.
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        /// <summary>
        /// Instance
        /// </summary>
        public static EventManager Instance = null;

		public static AMLogger amLogger = AMLogger.GetInstance ("AnalytiAll: ");

		static string[] PERMISSION_PAYMENT = new string[]{"_ALL"};

		const string CUSTOM_EVENT_NAME_KEY = "custom_event_name";
		const string END_TIMED_EVENT_KEY = "end_timed_event";
		const string BALANCER_CONFIG_VERSION_KEY = "version_config";

		const string BALANCER_CONFIG_VERSION_FIELD = "version_congig_save";
		const string SCENE_START = "scene_start";
		const string BUTTON_CLICK = "button_click";
		const string LEVEL_START = "level_start";
		const string LEVEL_WIN = "level_win";
		const string LEVEL_LOSE = "level_lose";
		const string LOOT_APPEND = "loot_append";
		const string LOOT_CONSUME = "loot_consume";
		const string GAME_OVER = "game_over";
		const string INAPP_SHOP_BUTTON_CLICK = "inapp_shop_button_click";
		const string INAPP_ITEM_BUTTON_CLICK = "inapp_item_button_click";
		const string INAPP_PURCHASE_COMPLETED = "inapp_purchase_completed";
		const string INAPP_PURCHASE_FAILED = "inapp_purchase_failed";
		const string INAPP_PURCHASE_CANCELLED = "inapp_purchase_cancelled";
		const string INAPP_PURCHASE_REFUNDED = "inapp_purchase_refunded";
		const string INAPP_PURCHASE_RESTORED = "inapp_purchase_restored";
        const string REVENUE_LOG = "amplitude_revenue_logged";
        const string REVENUE_LOG_WITH_DATA = "amplitude_revenue_with_data_logged";

        public static bool permision = true;
        string currentLevel = "";
        float currentTimescale = 1.0f;
		public static Dictionary<string, string> custoData;

		static List<Hashtable> cachedLogEvents;
        /// <summary>
        /// Событие смены сцены
        /// </summary>
        public event EmptyEventHandler NewSceneEvent;
        /// <summary>
        /// Событие паузы сцены
        /// </summary>
        public event EmptyEventHandler PauseEvent;
        /// <summary>
        /// Событие возобновления сцены
        /// </summary>
        public event EmptyEventHandler ResumeEvent;
        /// <summary>
        /// Событие того, что приложение неактивно
        /// </summary>
        //public event EmptyEventHandler ApplicationInactiveEvent;
        /// <summary>
        /// Событие того, что поменялась ориентация приложения
        /// </summary>
        //public event EmptyEventHandler OrientationChangeEvent;
        /// <summary>
        /// Происходит при старте нового уровня
        /// </summary>
        public event EmptyEventHandler StartLevelEvent;
        /// <summary>
        /// Происходит при выигрыше этого уровня
        /// </summary>
        public event EmptyEventHandler LevelWinEvent;
         /// <summary>
        /// Происходит при проигрыше этого уровня
        /// </summary>
        public event EmptyEventHandler LevelLoseEvent;
         /// <summary>
        /// Происходит при нажатии на кнопку входа в магазин приложения
        /// </summary>
        public event EmptyEventHandler InappShopButtonClickEvent;
        /// <summary>
        /// Происходит при нажатии на кнопку покупки определенного товара
        /// </summary>
        public event EmptyEventHandler InappItemButtonClickEvent;
        /// <summary>
        /// Происходит при успешной покупки
        /// </summary>
        public event EmptyEventHandler InappPurchaseCompletedEvent;
        /// <summary>
        /// Происходит при безуспешной покупки
        /// </summary>
        public event EmptyEventHandler InappPurchaseFailedEvent;
        /// <summary>
        /// Происходит при отмене покупки
        /// </summary>
        public event EmptyEventHandler InappPurchaseCancelledEvent;
        /// <summary>
        /// Просиходит при возвращеннии внутриигровой покупки
        /// </summary>
        public event EmptyEventHandler InappPurchaseRefundedEvent;
        /// <summary>
        /// Просиходит при восстановлении внутриигровой покупки
        /// </summary>
        public event EmptyEventHandler InappPurchaseRestoredEvent;
        /// <summary>
        /// Происходит при получении игровых бонусов
        /// </summary>
        public event EmptyEventHandler LootAppendEvent;
        /// <summary>
        /// Происходит при использовании игровых бонусов
        /// </summary>
        public event EmptyEventHandler LootConsumeEvent;
        /// <summary>
        /// Происходит при старте сцены
        /// </summary>
        public event EmptyEventHandler StartSceneEvent;
        /// <summary>
        /// Происходит при нажатии на кнопку
        /// </summary>
        public event EmptyEventHandler ButtonClickEvent;
        /// <summary>
        /// Происходит когда игра пройдена
        /// </summary>
        public event EmptyEventHandler GameOverEvent;
        /// <summary>
        /// Происходит когда доход записывается в логи (Amplitude)
        /// </summary>
        public event EmptyEventHandler RevenueLogEvent;
        /// <summary>
        /// Происходит когда доход c параметрами записывается в логи (Amplitude)
        /// </summary>
        public event EmptyEventHandler RevenueLogWithDataEvent;

        // [System.Obsolete (@"Method is obsolete and will be removed in future update! Use AnalytiAll.EventManager.LogEventEvent instead")]
        // public event EmptyEventHandler CustomEvent;

        /// <summary>
        /// Происходит когда вызвано кастомное событие
        /// </summary>
        public event EmptyEventHandler LogEventEvent;

		/// <summary>
		/// Происходит когда вызвано timed кастомное событие
		/// </summary>
		public event EmptyEventHandler LogTimedEventEvent;

		/// <summary>
		/// Происходит при вызове остановки кастомного timed события
		/// </summary>
		public event EmptyEventHandler EndTimedEventEvent;

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start ()
        {
			if (AMConfigsParser.AMParseJsonConfig.instance == null)
				new GameObject ("AMConfigsParser").AddComponent (typeof (AMConfigsParser.AMParseJsonConfig));
			AMConfigsParser.AMParseJsonConfig.isParseEnd (LoadPlugin);          
        }
		/// <summary>
		/// Loads the plugin.
		/// </summary>
		void LoadPlugin ()
		{
			if (Instance == null)
			{
				var buildParams = AMConfigsParser.AMParseJsonConfig.GetAMBuildParams ();
				string currentPayment = "";
				if (buildParams != null)
				{
					currentPayment = buildParams.payment;
				}
				if (Array.Exists (PERMISSION_PAYMENT, (s) => {return (currentPayment == s) || ("_ALL" == s);}))
				{
					Instance = this;

					DontDestroyOnLoad (gameObject);
					AppStart ();

					if (customCodeEnable) {
						DebugMenu = ccDebugMenu;
						DebugMode = ccDebugMode;
						HandleController = ccHandleController;
					}

					if (DebugMode)
					{
						NativeBridge.SetDebugMode (true);
					}

					SendCachedEvents ();
					CacheCutomData();
				}
				else
				{
					Destroy (gameObject);
				}
			}
			else
			{
				Destroy (gameObject);
			}
		}

		static void CacheEvent (string name, Dictionary<string, string> data = null, string timed = null)
		{
			if (cachedLogEvents == null)
				cachedLogEvents = new List<Hashtable> ();

			Hashtable cachedEvent = new Hashtable ();
			cachedEvent.Add ("name", name);

			if (data != null)
				cachedEvent.Add ("data", data);

			if (!string.IsNullOrEmpty (timed))
				cachedEvent.Add ("timed", timed);

			cachedLogEvents.Add (cachedEvent);
		}

		static void CacheCutomData ()
		{
			if (custoData != null)
				SetCustomData(custoData);
		}

		void SendCachedEvents ()
		{
			if (cachedLogEvents == null)
				return;

			foreach (var item in cachedLogEvents)
			{
				if (item.ContainsKey ("timed"))
				{
					if (item["timed"].ToString ().Equals ("start"))
					{
						if (item.ContainsKey ("data"))
							LogTimedEvent (item["name"].ToString (), item["data"] as Dictionary<string, string>);
						else
							LogTimedEvent (item["name"].ToString ());
					}
					else
						EndTimedEvent (item["name"].ToString ());
				}
				else
				{
					if (item.ContainsKey ("data"))
						LogEvent (item["name"].ToString (), item["data"] as Dictionary<string, string>);
					else
						LogEvent (item["name"].ToString ());
				}
			}
			cachedLogEvents = null;
		}

		/// <summary>
		/// Ises the need start.
		/// </summary>
		/// <returns><c>true</c>, if need start was ised, <c>false</c> otherwise.</returns>
		bool isNeedStart ()
		{
			return false;
		}

		static string BalancerConfigVersion ()
		{
			string result = string.Empty;

			#if UNITY_WSA_10_0 && NETFX_CORE
			Assembly assembly = Assembly.Load (new AssemblyName ("Assembly-CSharp"));
			var gccClassType = assembly.GetType ("GCCconfigsHandler");
			if (gccClassType == null)
				return result;
				
			FieldInfo configVersionFieldInfo = TypeExtensions.GetField (gccClassType, BALANCER_CONFIG_VERSION_FIELD, BindingFlags.Static | BindingFlags.Public);
			if (configVersionFieldInfo != null)
				result = configVersionFieldInfo.GetValue (EventManager.Instance).ToString ();
			#else
			var gccClassType = System.Type.GetType ("GCCconfigsHandler, Assembly-CSharp");
			if (gccClassType == null)
				return result;

			FieldInfo configVersionFieldInfo = gccClassType.GetField (BALANCER_CONFIG_VERSION_FIELD, BindingFlags.Static | BindingFlags.Public);
			if (configVersionFieldInfo != null)
				result = configVersionFieldInfo.GetValue (EventManager.Instance).ToString ();
			#endif

			return result;
		}

		/// <summary>
		/// Apps the start.
		/// </summary>
		void AppStart ()
		{
			NativeBridge.StartApp ();
		}
		/// <summary>
		/// Update this instance.
		/// </summary>
        void Update ()
        {
			if (customCodeEnable) 
			{
				DebugMenu = ccDebugMenu;
				DebugMode = ccDebugMode;
				HandleController = ccHandleController;
			}
            SetNewLevel ();
            ProcessTimescale ();
        }
		/// <summary>
		/// Raises the application pause event.
		/// </summary>
		/// <param name="isPause">If set to <c>true</c> is pause.</param>
        void OnApplicationPause (bool isPause)
        {
            if (isPause)
            {
				if (amLogger != null)
					amLogger.Log ("Pause");
                NativeBridge.PauseApp ();
            }
            else
            {
				if (amLogger != null)
					amLogger.Log ("Resume");
                NativeBridge.ResumeApp ();
            }
        }

		/// <summary>
		/// Sets the new level.
		/// </summary>
        void SetNewLevel ()
        {
			string newLevel = string.Empty;
			newLevel = SceneManager.GetActiveScene ().name;

	        if (newLevel != currentLevel)
	        {
	            currentLevel = newLevel;
				EventManager.StartScene (SceneManager.GetActiveScene ().buildIndex, newLevel);
	            if (NewSceneEvent != null)
	            {
	                NewSceneEvent ();
	            }
	        }
        }
		/// <summary>
		/// Processes the timescale.
		/// </summary>
        void ProcessTimescale ()
        {
            if (Time.timeScale == 0)
            {
                if (PauseEvent != null)
                {
                    PauseEvent ();
                }
                currentTimescale = Time.timeScale;
            }
            else
            {
                if (currentTimescale == 0)
                {
                    currentTimescale = Time.timeScale;
                    if (ResumeEvent != null)
                    {
                        ResumeEvent ();
                    }
                }
                else
                {
                    currentTimescale = Time.timeScale;
                }
            }
        }

		/// <summary>
		/// Метод для проброса пользовательских параметров во все события
		/// </summary>
		/// <param name="data">System.Collections.Generic.Dictionary параметров (key-value)</param>
		public static void SetCustomData (Dictionary <string, string> data)
		{
			if (data == null) 
			{
				data = new Dictionary<string, string> ();
			}
			if (data.ContainsKey (CUSTOM_EVENT_NAME_KEY))
			{
				throw new Exception ("Нельзя использовать " + CUSTOM_EVENT_NAME_KEY + " в качестве ключа кастомного события!");
			}
			if (data.ContainsKey (END_TIMED_EVENT_KEY))
			{
				throw new Exception ("Нельзя использовать " + END_TIMED_EVENT_KEY + " в качестве ключа кастомного события!");
			}

			// if (!string.IsNullOrEmpty (BalancerConfigVersion ()))
			// {
			// 	if (data.ContainsKey (BALANCER_CONFIG_VERSION_KEY)){
			// 		data[BALANCER_CONFIG_VERSION_KEY] = BalancerConfigVersion ();
			// 	}
			// 	else{
			// 		data.Add (BALANCER_CONFIG_VERSION_KEY, BalancerConfigVersion ());
			// }
			// }

			if (data.Keys.Count == 0)
				return;

			if (data.Keys.Count >= 9) {
			#if UNITY_EDITOR
			Debug.LogError("Превышено колличество параметров");
			#endif
			List<string> nameEvent = new List<string>();
			int i = 0;
			int j = data.Keys.Count-9;
				foreach (KeyValuePair<string, string> kvp in data) { 
					if (i <= j){
					nameEvent.Add(kvp.Key);
					}
					else 
						break;
					i++;
				}
				foreach (string ke in nameEvent) { 
					data.Remove(ke);
					Debug.LogWarning("Был удален эвент: " + ke);
				}
			}
			
			if (Instance == null)
			{
				custoData = data;
				return;
			}

			NativeBridge.SetCustomData (data);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0} {1}", "SetCustomData", string.Format (" : {0}", AMUtils.AMJSON.JsonEncode (new Hashtable (data)))));
		}

        /// <summary>
        /// Событие начала уровня игры
        /// </summary>
        /// <param name="nameLevel">название уровня</param>
        /// <param name="mode">режим игры</param>
        public static void StartLevel (string nameLevel, string mode)
        {
			NativeBridge.StartLevel (nameLevel, mode);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "StartLevel", string.Format ("nameLevel={0}, mode={1}", nameLevel, mode)));

			if (Instance != null && Instance.StartLevelEvent != null)
				Instance.StartLevelEvent ();
        }
        /// <summary>
        /// Событие выигрыша этого уровня
        /// </summary>
        /// <param name="nameLevel">название уровня</param>
        /// <param name="mode">режим игры</param>
        public static void LevelWin (string nameLevel, string mode)
        {
			NativeBridge.LevelWin (nameLevel, mode);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "LevelWin", string.Format ("nameLevel={0}, mode={1}", nameLevel, mode)));
			
			if (Instance != null && Instance.LevelWinEvent != null)
				Instance.LevelWinEvent ();
        }
        /// <summary>
        /// Событие проигрыша этого уровня
        /// </summary>
        /// <param name="nameLevel">название уровня</param>
        /// <param name="mode">режим игры</param>
        public static void LevelLose (string nameLevel, string mode)
        {
			NativeBridge.LevelLose (nameLevel, mode);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "LevelLose", string.Format ("nameLevel={0}, mode={1}", nameLevel, mode)));

			if (Instance != null && Instance.LevelLoseEvent != null)
				Instance.LevelLoseEvent ();
        }
        /// <summary>
        /// Событие нажатия на кнопку входа в магазин покупок в приложении
        /// </summary>
        public static void InappShopButtonClick ()
        {
			NativeBridge.InappShopButtonClick ();

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "InappShopButtonClick", string.Format ("")));

			if (Instance != null && Instance.InappShopButtonClickEvent != null)
				Instance.InappShopButtonClickEvent ();
        }
        /// <summary>
        /// Событие нажатия на кнопку покупки определенного товара
        /// </summary>
        /// <param name="name">название кнопки</param>
        /// <param name="inappItemID">идентификатор покупки</param>
        public static void InappItemButtonClick (string name, string inappItemID)
        {
			NativeBridge.InappItemButtonClick (name, inappItemID);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "InappItemButtonClick", string.Format ("name={0}, inappItemID={1}", name, inappItemID)));

			if (Instance != null && Instance.InappItemButtonClickEvent != null)
				Instance.InappItemButtonClickEvent ();
        }
        /// <summary>
        /// Событие успешной покупки
        /// </summary>
        /// <param name="inappItemID">идентификатор покупки</param>
        public static void InappPurchaseCompleted (string inappItemID)
        {
			NativeBridge.InappPurchaseCompleted (inappItemID);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "InappPurchaseCompleted", string.Format ("inappItemID={0}", inappItemID)));

			if (Instance != null && Instance.InappPurchaseCompletedEvent != null)
				Instance.InappPurchaseCompletedEvent ();
        }
        /// <summary>
        /// Событие безуспешной  покупки
        /// </summary>
        /// <param name="inappItemID">идентификатор покупки</param>
        public static void InappPurchaseFailed (string inappItemID)
        {
			NativeBridge.InappPurchaseFailed (inappItemID);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "InappPurchaseFailed", string.Format ("inappItemID={0}", inappItemID)));

			if (Instance != null && Instance.InappPurchaseFailedEvent != null)
				Instance.InappPurchaseFailedEvent ();
        }
        /// <summary>
        /// Событие отмененной покупки
        /// </summary>
        /// <param name="inappItemID">идентификатор покупки</param>
        public static void InappPurchaseCancelled (string inappItemID)
        {
			NativeBridge.InappPurchaseCancelled (inappItemID);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "InappPurchaseCancelled", string.Format ("inappItemID={0}", inappItemID)));

			if (Instance != null && Instance.InappPurchaseCancelledEvent != null)
				Instance.InappPurchaseCancelledEvent ();
        }
        /// <summary>
        /// Событие возвращенной покупки 
        /// </summary>
        /// <param name="inappItemID">идентификатор покупки</param>
        public static void InappPurchaseRefunded (string inappItemID)
        {
			NativeBridge.InappPurchaseRefunded (inappItemID);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "InappPurchaseRefunded", string.Format ("inappItemID={0}", inappItemID)));

			if (Instance != null && Instance.InappPurchaseRefundedEvent != null)
				Instance.InappPurchaseRefundedEvent ();
        }
        /// <summary>
        /// Событие восстановленной покупки
        /// </summary>
        /// <param name="result">Результат успешного восстановления</param>
        public static void InappPurchaseRestored (bool result)
        {
			NativeBridge.InappPurchaseRestored (result);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "InappPurchaseRestored", string.Format ("result={0}", result)));

			if (Instance != null && Instance.InappPurchaseRestoredEvent != null)
				Instance.InappPurchaseRestoredEvent ();
        }
        /// <summary>
        /// Событие получения игровых бонусов
        /// </summary>
        /// <param name="name">название бонусов</param>
        /// <param name="volume">количество</param>
        public static void LootAppend (string name, int volume)
        {
			NativeBridge.LootAppend (name, volume);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "LootAppend", string.Format ("name={0}, volume={1}", name, volume)));

			if (Instance != null && Instance.LootAppendEvent != null)
				Instance.LootAppendEvent ();
        }
        /// <summary>
        /// Событие использования игровых бонусов
        /// </summary>
        /// <param name="name">название бонусов</param>
        /// <param name="volume">количество</param>
        /// <param name="target">Название предмета, полученного за игровой бонус</param>
        public static void LootConsume (string name, int volume, string target)
        {
			NativeBridge.LootConsume (name, volume, target);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "LootConsume", string.Format ("name={0}, volume={1}, target={2}", name, volume, target)));

			if (Instance != null && Instance.LootConsumeEvent != null)
				Instance.LootConsumeEvent ();
        }
        /// <summary>
        ///  Событие создания новой сцены (генерируется автоматически)
        /// </summary>
        /// <param name="sceneID"></param>
        /// <param name="sceneName"></param>
        public static void StartScene (int sceneID, string sceneName)
        {
			NativeBridge.StartScene (sceneID, sceneName);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "StartScene", string.Format ("sceneName={0}, sceneID={1}", sceneName, sceneID)));

			if (Instance != null && Instance.StartSceneEvent != null)
				Instance.StartSceneEvent ();
        }
        /// <summary>
        /// Пустое событие
        /// </summary>
        public static void EmptyEvent ()
        {
        }
        /// <summary>
        /// Событие нажатия на кнопку
        /// </summary>
        /// <param name="nameButton">имя кнопки</param>
        public static void ButtonClick (string nameButton)
        {
			NativeBridge.ButtonClick (nameButton);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "ButtonClick", string.Format ("nameButton={0}", nameButton)));

			if (Instance != null && Instance.ButtonClickEvent != null)
				Instance.ButtonClickEvent ();
        }
        /// <summary>
        /// Событие прохождения всех уровней в игре
        /// </summary>
        /// <param name="mode">режим игры</param>
        public static void GameOver (string mode)
        {
			NativeBridge.GameOver (mode);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, {1}", "GameOver", string.Format ("mode={0}", mode)));

			if (Instance != null && Instance.GameOverEvent != null)
				Instance.GameOverEvent ();
        }

		// [System.Obsolete (@"Method is obsolete and will be removed in future update! Use AnalytiAll.EventManager.LogEvent instead")]
		// public static void Custom (string customEventName)
		// {
		// 	Custom (customEventName, new Dictionary<string, string> ());
		// }
		/// <summary>
		/// Метод для генерации кастомного события без доп параметров
		/// </summary>
		/// <param name="logEventName">Имя кастомного события.</param>
		public static void LogEvent (string logEventName)
		{
			if (string.IsNullOrEmpty (logEventName)) 
			{
				if (amLogger != null)
					amLogger.Log (string.Format ("ERROR Empty logEventName; LogEvent"));
				return;
			}

			if (Instance == null)
			{
				CacheEvent (logEventName);
				return;
			}

			NativeBridge.LogEvent (logEventName);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, with name: {1}", "LogEvent", logEventName));

			if (Instance != null && Instance.LogEventEvent != null)
				Instance.LogEventEvent ();
		}
		/// <summary>
		/// Метод для генерации кастомного Timed события без доп параметров
		/// </summary>
		/// <param name="timedEventName">Имя кастомного timed события.</param>
		public static void LogTimedEvent (string timedEventName)
		{
			if (string.IsNullOrEmpty (timedEventName)) 
			{
				if (amLogger != null)
					amLogger.Log (string.Format ("ERROR Empty timedEventName at LogTimedEvent"));
				return;
			}

			if (Instance == null)
			{
				CacheEvent (timedEventName, null, "start");
				return;
			}

			NativeBridge.LogTimedEvent (timedEventName);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, with name: {1}", "LogTimedEvent", timedEventName));

			if (Instance != null && Instance.LogTimedEventEvent != null)
				Instance.LogTimedEventEvent ();
		}
        /// <summary>
        /// Записывает в amplitude log доход
        /// </summary>
        /// <param name="productID">Product identifier.</param>
        /// <param name="price">Price.</param>
        /// <param name="quantity">Quantity.</param>
        public static void ANlogRevenue(string productID, float price, int quantity)
        {
            NativeBridge.ANlogRevenue(productID, price, quantity);
            if (amLogger != null)
                amLogger.Log(string.Format("{0}, {1}", "ANlogRevenue", 
                string.Format("productID={0}, price={1}, quantity={2}", productID, price, quantity)));
            if (Instance != null && Instance.RevenueLogEvent != null)
                Instance.RevenueLogEvent();
        }
        /// <summary>
        /// Записывает в amplitude log доход (с доп параметрами)
        /// </summary>
        /// <param name="productID">Product identifier.</param>
        /// <param name="price">Price.</param>
        /// <param name="quantity">Quantity.</param>
        /// <param name="type">Type.</param>
        /// <param name="receipt">Receipt.</param>
        /// <param name="data">Data.</param>
        public static void ANlogRevenueWithData(string productID, float price, int quantity, string type, string receipt, string data)
        {
            NativeBridge.ANlogRevenueWithData(productID, price, quantity, type, receipt, data);
            if (amLogger != null)
                amLogger.Log(string.Format("{0}, {1}", "ANlogRevenueWithData",
                string.Format("productID={0}, price={1}, quantity={2}, type={3}, receipt={4}, data={5}",
                     productID, price, quantity, type, receipt, data)));

            if (Instance != null && Instance.RevenueLogWithDataEvent != null)
                Instance.RevenueLogWithDataEvent();
        }
		
        /// <summary>
        /// Метод для генерации кастомного события с доп параметрами
        /// </summary>
        /// <param name="logEventName">Имя кастомного события.</param>
        /// <param name="data">System.Collections.Generic.Dictionary параметров события (key-value)</param>
        public static void LogEvent (string logEventName, Dictionary<string, string> data)
		{
			if (string.IsNullOrEmpty (logEventName)) 
			{
				if (amLogger != null)
					amLogger.Log (string.Format ("ERROR Empty logEventName at LogEvent"));
				return;
			}

			if (data == null) 
			{
				if (amLogger != null)
					amLogger.Log (string.Format ("ERROR Data is null at LogEvent"));
				return;
			}

			if (data.ContainsKey (CUSTOM_EVENT_NAME_KEY))
			{
				throw new Exception ("Нельзя использовать " + CUSTOM_EVENT_NAME_KEY + " в качестве ключа кастомного события!");
			}
			if (data.ContainsKey (END_TIMED_EVENT_KEY))
			{
				throw new Exception ("Нельзя использовать " + END_TIMED_EVENT_KEY + " в качестве ключа кастомного события!");
			}

			if (Instance == null)
			{
				CacheEvent (logEventName, data);
				return;
			}

			
			
			NativeBridge.LogEvent (logEventName, data);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, with name: {1}, {2}", "LogEvent", logEventName, string.Format ("data: {0}", AMUtils.AMJSON.JsonEncode (new Hashtable (data)))));

			if (Instance != null && Instance.LogEventEvent != null)
				Instance.LogEventEvent ();
		}
		/// <summary>
		/// Метод для генерации кастомного Timed события с доп параметрами
		/// </summary>
		/// <param name="timedEventName">Имя кастомного timed события.</param>
		/// <param name="data">System.Collections.Generic.Dictionary параметров события (key-value)</param>
		public static void LogTimedEvent (string timedEventName, Dictionary <string, string> data)
		{
			if (string.IsNullOrEmpty (timedEventName)) 
			{
				if (amLogger != null)
					amLogger.Log (string.Format ("ERROR Empty timedEventName at LogTimedEvent"));
				return;
			}

			if (data == null) 
			{
				if (amLogger != null)
					amLogger.Log (string.Format ("ERROR Data is null at LogTimedEvent"));
				return;
			}

			if (data.ContainsKey (CUSTOM_EVENT_NAME_KEY))
			{
				throw new Exception ("Нельзя использовать " + CUSTOM_EVENT_NAME_KEY + " в качестве ключа кастомного события!");
			}
			if (data.ContainsKey (END_TIMED_EVENT_KEY))
			{
				throw new Exception ("Нельзя использовать " + END_TIMED_EVENT_KEY + " в качестве ключа кастомного события!");
			}

			if (Instance == null)
			{
				CacheEvent (timedEventName, data, "start");
				return;
			}

				if (data.Keys.Count >= 9) {
			#if UNITY_EDITOR
			Debug.LogError("Превышено колличество параметров");
			#endif
			List<string> nameEvent = new List<string>();
			int i = 0;
			int j = data.Keys.Count-9;
				foreach (KeyValuePair<string, string> kvp in data) { 
					if (i <= j){
					nameEvent.Add(kvp.Key);
					}
					else 
						break;
					i++;
				}
				foreach (string ke in nameEvent) { 
					data.Remove(ke);
					Debug.LogWarning("Был удален эвент: " + ke);
				}
			}
			
			NativeBridge.LogTimedEvent (timedEventName, data);

			if (amLogger != null)
				amLogger.Log (string.Format ("{0}, with name: {1}, {2}", "LogTimedEvent", timedEventName, string.Format ("data: {0}", AMUtils.AMJSON.JsonEncode (new Hashtable (data)))));

			if (Instance != null && Instance.LogTimedEventEvent != null)
				Instance.LogTimedEventEvent ();
		}

		/// <summary>
		/// Метод для остановки Timed кастомного события
		/// </summary>
		/// <param name="timedEventName">Имя ранее запущенного Timed события.</param>
		public static void EndTimedEvent (string timedEventName)
		{
			if (string.IsNullOrEmpty (timedEventName)) 
			{
				if (amLogger != null)
					amLogger.Log (string.Format ("ERROR Empty timedEventName at EndTimedEvent"));
				return;
			}

			if (Instance == null)
			{
				CacheEvent (timedEventName, null, "stop");
				return;
			}

			NativeBridge.EndTimedEvent (timedEventName);

			if (amLogger != null)
				amLogger.Log (string.Format ("EndTimedEvent: {0}", timedEventName));

			if (Instance != null && Instance.EndTimedEventEvent != null)
				Instance.EndTimedEventEvent ();
		}

		#region Inspector Settings
		public bool DebugMode = false;
		public bool DebugMenu = false;
		public bool HandleController = false;
		#endregion

		#if UNITY_TVOS
		public DateTime btnPressedTime = new DateTime ();
		public event System.Action exitFromMenu;
		#endif
		public string btnIdxOut = string.Empty;

		public static bool customCodeEnable = false;
		public static bool ccDebugMode;
		public static bool ccDebugMenu;
		public static bool ccHandleController = false;
		public bool isGenerateEvent = false;
		
		string[] listGenerateEvents = new string[]
		{
			"StartScene", 
			"ButtonClick", 
			"LootAppend", 
			"LootConsume", 
			"StartLevel", 
			"LevelWin", 
			"LevelLose", 
			"GameOver", 
			"InappShopButtonClick", 
			"InappItemButtonClick", 
			"InappPurchaseCompleted", 
			"InappPurchaseFailed", 
			"InappPurchaseCancelled", 
			"InappPurchaseRefunded", 
			"InappPurchaseRestored", 
			"LogEvent",
            "ANlogRevenue",
            "ANlogRevenueWithData"
        };

		static UI.Menu newMenu = new UI.Menu ();
		public void DrawDebugMenu ()
		{
			newMenu.Show ();
		}

		public void CloseDebugMenu ()
		{
			DebugMenu = false;
			newMenu.Reset ();
		}
		#if UNITY_TVOS
		public void OnExitFromMenu ()
		{
			if (exitFromMenu != null)
				exitFromMenu ();
			exitFromMenu = null;
		}
		#endif
		/// <summary>
		/// Raises the GU event.
		/// </summary>
		void OnGUI ()
		{
			if (DebugMenu && !customCodeEnable) 
			{
				DrawDebugMenu ();
			}
		}
		/// <summary>
		/// Генерация эвента.
		/// </summary>
		/// <param name="e">Имя эвента.</param>
		void GenerateEvent (string e)
		{	
			switch (e) {
			case "ButtonClick":
				AnalytiAll.EventManager.ButtonClick ("TestClick");
				break;
			case "StartLevel":
				AnalytiAll.EventManager.StartLevel ("TestLevel", "TestMode");
				break;
			case "LootConsume":
				AnalytiAll.EventManager.LootConsume ("TestLoot", 666, "waste_of_money");
				break;
			case "LootAppend":
				AnalytiAll.EventManager.LootAppend ("TestLoot", 777);
				break;
			case "LevelWin":
				AnalytiAll.EventManager.LevelWin ("TestLevel", "TestMode");
				break;
			case "LevelLose":
				AnalytiAll.EventManager.LevelLose ("TestLevel", "TestMode");
				break;
			case "InappShopButtonClick":
				AnalytiAll.EventManager.InappShopButtonClick ();
				break;
			case "InappPurchaseRestored":
				AnalytiAll.EventManager.InappPurchaseRestored (true);
				break;
			case "InappPurchaseRefunded":
				AnalytiAll.EventManager.InappPurchaseRefunded ("TestInappID");
				break;
			case "InappPurchaseFailed":
				AnalytiAll.EventManager.InappPurchaseFailed ("TestInappID");
				break;
			case "InappPurchaseCancelled":
				AnalytiAll.EventManager.InappPurchaseCancelled ("TestInappID");
				break;
			case "InappPurchaseCompleted":
				AnalytiAll.EventManager.InappPurchaseCompleted ("TestInappID");
				break;
			case "InappItemButtonClick":
				AnalytiAll.EventManager.InappItemButtonClick ("TestNameInApp", "TestInappID");
				break;
			case "GameOver":
				AnalytiAll.EventManager.GameOver ("TestMode");
				break;
			case "StartScene":
				AnalytiAll.EventManager.StartScene (404, "TestScene");
				break;
            case "ANlogRevenue":
                AnalytiAll.EventManager.ANlogRevenue("TestProductID", 1.0f, 1);
                break;
            case "ANlogRevenueWithData":
                    AnalytiAll.EventManager.ANlogRevenueWithData
                    ("TestProductID", 1.0f, 1, "TestType", "TestReceipt", "TestData");
                break;
                case "LogEvent":
                    var data = new Dictionary<string, string>();
                    data.Add("key1", "value1");
                    data.Add("key2", "value2");
                    //              data.Add ("custom_event_name", "value2"); // bad key
                    AnalytiAll.EventManager.LogEvent("LogEventName", data);
                    break;
                case "AppStart":
				//AnalytiAll.EventManager.AppStart ();
				break;
			default:
				break;
			}
		}
		/// <summary>
		/// Генерация рандомного эвента.
		/// </summary>
		void RandomGenerate ()
		{
			int rand = UnityEngine.Random.Range (0, listGenerateEvents.Count ());
			GenerateEvent (listGenerateEvents [rand]);
		}
		/// <summary>
		/// Старт генерации эвентов.
		/// </summary>
		public void GenerateEventsStart ()
		{
			isGenerateEvent = true;
			InvokeRepeating ("RandomGenerate", 0.3f, 0.1f);
		}
		/// <summary>
		/// Остановка генерации эвентов.
		/// </summary>
		public void GenerateEventsStop ()
		{
			isGenerateEvent = false;
			CancelInvoke ("RandomGenerate");
		}
    }
}