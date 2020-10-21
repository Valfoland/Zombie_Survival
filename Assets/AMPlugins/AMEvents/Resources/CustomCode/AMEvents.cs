#pragma warning disable
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using AMUtils;
using AMLogging;

namespace AMEvents
{
	/// <summary>
	/// Класс для передачи событий перехода на новую сцену, паузы и возобновления сцены, а также собственных вызовов в моменты, когда приложение неактивно в нативный код.
	/// </summary>
	public class AMEvents : MonoBehaviour
	{
		public class AutoInterstitialScene
		{
			public string name;
			public bool skip;

			#if UNITY_EDITOR
			public AutoInterstitialScene (UnityEditor.EditorBuildSettingsScene source)
			{
				name = source.path.Substring (source.path.LastIndexOf ('/') + 1);
				name = name.Substring (0, name.LastIndexOf ('.'));

				skip = false;
			}
			#endif
			public AutoInterstitialScene (string name, bool skip = false)
			{
				this.name = name;
				this.skip = skip;
			}
			public static List<AutoInterstitialScene> UpdateList (List<AutoInterstitialScene> source, bool fromInspector = false)
			{
				if (fromInspector)
				{
					if (source == null)
						source = new List<AutoInterstitialScene> ();
					#if UNITY_EDITOR
					foreach (var item in UnityEditor.EditorBuildSettings.scenes)
					{
						if (item == null || !item.enabled || string.IsNullOrEmpty (item.path) || item.path.Contains (CUSTOM_CODE_SCENE_NAME))
							continue;

						string tempSceneName = item.path.Substring (item.path.LastIndexOf ('/') + 1);
						tempSceneName = tempSceneName.Substring (0, tempSceneName.LastIndexOf ('.'));

						if (source.FindIndex (s => {return s.name == tempSceneName;}) == -1)
							source.Add (new AutoInterstitialScene (tempSceneName));
					}
					#endif
					var scenesTempList = AMConfigsParser.AMProjectInfoInside.autoInterstitialScenes;
					if (scenesTempList != null)
					{
						for (int i = 0; i < source.Count; i++)
						{
							var item = source [i];
							item.skip = (scenesTempList.FindIndex (s => {return s.ToString () == item.name;})) == -1;
							source [i] = item;
						}
					}
				}
				else
				{
					if (source == null)
					{
						source = new List<AutoInterstitialScene> ();

						if (AMConfigsParser.AMProjectInfoInside.autoInterstitialScenes != null)
						{
							for (int i = 0; i < AMConfigsParser.AMProjectInfoInside.autoInterstitialScenes.Count; i++)
							{
								var newScene = new AutoInterstitialScene (AMConfigsParser.AMProjectInfoInside.autoInterstitialScenes [i]);
								if (!source.Contains (newScene))
									source.Add (newScene);
							}
						}
					}
				}
				return source;
			}
		}

		public List<AutoInterstitialScene> autoInterstitialScenes;
		#region Inspector Settings
		public bool DebugMode = false;
		public bool DebugMenu = false;
		public bool HandleController = false;
		public bool AutoInterstitial = true;
		public bool CrossAd = false;
		public bool AdAtStart = true;
		public float StartScreenOpacity = 0.95f;
		public int InterstitialDelay = 60;
		public bool AutoPause = false;
		#endregion
		#if UNITY_TVOS
		public DateTime btnPressedTime = new DateTime ();
		public event System.Action exitFromMenu;
		#endif
		public string btnIdxOut = string.Empty;
		public static List<AutoInterstitialScene> ccAutoInterstitialScenes;
		public static bool ccDebugMode;
		public static bool ccDebugMenu;
		public static bool ccHandleController = false;
		public static bool ccAutoInterstitial;
		public static bool ccCrossAd;
		public static bool ccAdAtStart;
		public static float ccStartScreenOpacity = 0.95f;
		public static bool ccAutoPause = false;
		public static int ccInterstitialDelay = 60;
		public static bool customCodeEnable = false;
		public static AMLogger amLogger = AMLogger.GetInstance ("AMEvents: ");
		string currentLevel = "";
		float time;
		float currentTimescale = 1;
		public static AMEvents Instance = null;
		public static bool isInit = false;
		//add version 1.1.0
		static string[] statuses = null;
		static int indexStatus;
		static bool isProcessStatus;
		static bool fastChange = false;
		static int ALL_TIME = 30;
		static bool shown;
		public static bool ListenerReady = false;
		public static string currentGameObjectName = "";
		static bool adsLoaded = false;
		public const string CUSTOM_CODE_SCENE_NAME = "SceneCustomCode";
		private const string NAME_LAST_STARTUP_DAY = "LastStartupDay";
		private const string NO_NEW_SCENE_EVENT = "NoNewSceneInterstitial";
		static string[] PERMISSION_PAYMENT = new string[]{"_ALL"};
		static bool firstShow = false;
		#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
		public static bool lockEvents = false;
		public static bool lockCrossEvent = false;
		#endif

		/// <summary>
		/// Событие смены сцены
		/// </summary>
		public static event System.Action NewSceneEvent;

		/// <summary>
		/// Событие паузы сцены
		/// </summary>
		public static event System.Action PauseEvent;

		/// <summary>
		/// Событие возобновления сцены
		/// </summary>
		public static event System.Action ResumeEvent;

		/// <summary>
		/// Событие того, что приложение неактивно
		/// </summary>
		public static event System.Action ApplicationInactiveEvent;

		/// <summary>
		/// Событие окна политики
		/// </summary>
		public static event System.Action PolicyHandler;

		/// <summary>
		/// Событие того, что поменялась ориентация приложения
		/// </summary>
		public static event System.Action OrientationChangeEvent;

		/// <summary>
		/// Fired when an interstitial ad is loaded
		/// </summary>
		public static event System.Action InterstitialLoadedEvent;

		/// <summary>
		/// Fired when an interstitial ad is failed
		/// </summary>
		public static event System.Action InterstitialFailedEvent;

		/// <summary>
		/// Occurs when end show status.
		/// </summary>
		public static event System.Action EndShowStatus;

		/// <summary>
		/// Occurs when change status.с
		/// </summary>
		public static event System.Action<string, float> ChangeStatus;

		/// <summary>
		/// Occurs when end init.
		/// </summary>
		public static event System.Action EndInit;

		/// <summary>
		/// Awake this instance.
		/// </summary>
		void Awake ()
		{
			if (AMConfigsParser.AMParseJsonConfig.instance == null)
				new GameObject ("AMConfigsParser").AddComponent (typeof (AMConfigsParser.AMParseJsonConfig));

			AMConfigsParser.AMParseJsonConfig.isParseEnd (LoadPlugin);
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update ()
		{
			if (customCodeEnable)
			{
				DebugMode = ccDebugMode;
				DebugMenu = ccDebugMenu;
				HandleController = ccHandleController;
				CrossAd = ccCrossAd;
				AdAtStart = ccAdAtStart;
				StartScreenOpacity = ccStartScreenOpacity;
				InterstitialDelay = ccInterstitialDelay;
				if (DebugMode && !NativeBridge.GetDebugMode ())
				{
					NativeBridge.SetDebugMode (true);
				}
				if (AutoInterstitial != ccAutoInterstitial)
				{
					AutoInterstitial = ccAutoInterstitial;
					ChangeAutoInterstitial ();
				}
				if (autoInterstitialScenes != ccAutoInterstitialScenes)
				{
					autoInterstitialScenes = ccAutoInterstitialScenes;
				}
				if (AutoPause != ccAutoPause)
				{
					AutoPause = ccAutoPause;
				}
			}
			if (time >= Time.timeSinceLevelLoad) 
			{
				SetNewLevel ();
			}
			else 
			{
				time += Time.deltaTime;
			}
			if (Time.timeScale != currentTimescale) 
			{
				ProcessTimescale ();
			}
			CheckOrientation ();

			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			if (lockEvents && (cachedEvents != null && cachedEvents.Count > 0))
			{
				lockEvents = false;
				continueOnAdEvents (cachedEvents[0]);
			}
			if (lockCrossEvent)
			{
				lockCrossEvent = false;
				continueOnCrossClicked ();
			}
			#endif
		}

		/// <summary>
		/// Load plugin.
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
					gameObject.name = "AMEvents";
					currentGameObjectName = gameObject.name;

					if (customCodeEnable)
					{
						DebugMode = ccDebugMode;
						DebugMenu = ccDebugMenu;
						HandleController = ccHandleController;
						CrossAd = ccCrossAd;
						AdAtStart = ccAdAtStart;
						StartScreenOpacity = ccStartScreenOpacity;
						InterstitialDelay = ccInterstitialDelay;
						if (AutoInterstitial != ccAutoInterstitial)
						{
							AutoInterstitial = ccAutoInterstitial;
							ChangeAutoInterstitial ();
						}
						if (autoInterstitialScenes != ccAutoInterstitialScenes)
						{
							autoInterstitialScenes = ccAutoInterstitialScenes;
						}
						if (AutoPause != ccAutoPause)
						{
							AutoPause = ccAutoPause;
						}
					}

					if (AutoInterstitial)
					{
						AMEvents.NewSceneEvent += NativeBridge.NewSceneEvent;
						AMEvents.PauseEvent += NativeBridge.PauseSceneEvent;
						AMEvents.ResumeEvent += NativeBridge.ResumeSceneEvent;

						AMEvents.ApplicationInactiveEvent += NativeBridge.ApplicationInactiveEvent;
						AMEvents.OrientationChangeEvent += NativeBridge.OrientationChangeEvent;

						autoInterstitialScenes = AutoInterstitialScene.UpdateList (autoInterstitialScenes);
					}

					if (AutoPause)
					{
						SceneManager.sceneLoaded += (Scene arg0, LoadSceneMode arg1) => {if (PauseController.needToPause) StartCoroutine (PauseController.SetPauseIfNecessery ());};
						PauseController.Init ();
					}

					InterstitialLoadedEvent += interstitialLoadedEvent;
					InterstitialFailedEvent += interstitialFailedEvent;

					NativeBridge.SetDebugMode (DebugMode);

					if (Ad.enableCrossRequired)
					{
						NativeBridge.EnableCross (currentGameObjectName);
					}

					NativeBridge.AddAdListener (currentGameObjectName, "banner");
					NativeBridge.AddAdListener (currentGameObjectName, "interstitial");
					NativeBridge.AddAdListener (currentGameObjectName, "video");
					NativeBridge.AddAdListener (currentGameObjectName, "rewardedVideo");
					NativeBridge.AddAdListener (currentGameObjectName, "innerInApp");
					NativeBridge.AddAdListener (currentGameObjectName, "sharing");
					NativeBridge.AddAdListener (currentGameObjectName, "internet");
					NativeBridge.AddAdListener (currentGameObjectName, "policy");

                    isInit = true;
					if (EndInit != null)
						EndInit ();
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

		public static void isInitPlugin (System.Action callback)
		{
			if (isInit)
				callback ();
			else
				EndInit += callback;
		}

		/// <summary>
		/// Raises the level was loaded event.
		/// </summary>
		void OnLevelWasLoaded ()
		{
			if (AutoPause && PauseController.needToPause)
				StartCoroutine (PauseController.SetPauseIfNecessery ());
		}

		#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
		static List<string> cachedEvents;
		#endif
		public void onAdEvent (string events)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			if (cachedEvents == null)
			{
				cachedEvents = new List<string> ();
			}
			if (!cachedEvents.Contains (events))
			{
				cachedEvents.Add (events);
			}
			if (!lockEvents)
				lockEvents = true;
			#else
			continueOnAdEvents (events);
			#endif
		}

		public void onShareHandler (string resultJsonString) 
		{
			JSONNode jsonNode = JSON.Parse (resultJsonString);

			string networkText = jsonNode ["network"].ToString();
			string resultText = jsonNode ["result"].ToString();
			string errorText = jsonNode ["errorText"].ToString();

			if (networkText == null) {
				networkText = "error";	
				Debug.Log("Ненайден \"" + networkText + "\" параметр");
				}

			if (resultText == null) {
				resultText = "\"error\"";	
				Debug.Log("Ненайден \"" + networkText + "\" параметр");
				}
			else {
				if (resultText == "\"complete\"") {
					Sharing.OnCompletedEvent();
				}

			    else if (resultText == "\"canceled\"") { 
					Sharing.OnCaneledEvent();
				}

				else if (resultText == "\"error\""){
					if (errorText == null) {
						Debug.Log("Ненайден \"" + networkText + "\" параметр");
						}
					else {
						Sharing.OnErrorEvent(errorText);
					}
				}
			}
		}
		public void onInternetConnection  (string isConnection) {
			if (isConnection == "true") {
				Internet.OnSucceededEvent();			
			}
			else {
				Internet.OnNotEstablishedEvent();
			}
		}
		void continueOnAdEvents (string events)
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			if (lockEvents)
				return;
			cachedEvents.RemoveAt (0);
			#endif

			string bType = "none";
			string eventName = "none";
			try 
			{
				var ev = AMJSON.JsonDecode (events) as Hashtable;
				bType = (string)ev["bannerType"];
				eventName = (string)ev ["event"];
			} 
			catch (Exception)
			{
				if (amLogger != null)
					amLogger.Log ("Error Parse Event. json : " + events);
			}

			switch (bType)
			{
			case "interstitial":
				switch (eventName) 
				{
				case "ready":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Interstitial ready event handled");
					}
					Ad.Interstitial.OnReadyEvent ();
					break;
				case "commercialReady":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Interstitial commercial ready event handled");
					}
					Ad.Interstitial.OnCommercialReady ();
					break;
				case "imp":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Interstitial impression event handled");
					}
					Ad.Interstitial.OnImpressionEvent ();
					break;
				case "closed":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Interstitial closed event handled");
					}

					Ad.Interstitial.OnClosedEvent ();

					break;
				default:
					Ad.Interstitial.OnUnknownEvent ();

					break;
				}
				break;
			case "rewardedVideo":
				switch (eventName) 
				{
				case "ready":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Rewarded video ready event handled");
					}
					Ad.RewardedVideo.OnReadyEvent ();
					break;
				case "imp":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Rewarded video impression event handled");
					}
					Ad.RewardedVideo.OnImpressionEvent ();
					break;
				case "success":
					int reward = 0;
					try 
					{
						var ev = AMJSON.JsonDecode (events) as Hashtable;
						var data = (Hashtable)ev["data"];

						reward = Convert.ToInt32 (data["reward"]);
					} 
					catch (Exception ex)
					{
						if (amLogger != null)
							amLogger.LogError ("Error Parse Event. json : " + events + "\n" + ex.ToString ());
					}
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Rewarded video success event handled with value: " + reward.ToString ());
					}
					Ad.RewardedVideo.OnSuccessEvent (reward);
					break;
				case "closed":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Rewarded video closed event handled");
					}
					Ad.RewardedVideo.OnClosedEvent ();
					break;
				default:
					Ad.RewardedVideo.OnUnknownEvent ();
					break;
				}
				break;
            case "innerInApp":
                switch (eventName)
                {
				case "imp":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Inner InApp impression event handled");
					}
					Ad.InnerInApp.OnImpressionEvent ();
                    break;
				case "closed":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Inner InApp closed event handled");
					}
					Ad.InnerInApp.OnClosedEvent ();
					break;
				case "success":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Inner InApp success event handled");
					}
					Ad.InnerInApp.OnSuccessEvent ();
                    break;
				case "failed":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Inner InApp failed event handled");
					}
					Ad.InnerInApp.OnFailedEvent ();
					break;
                default:
					Ad.InnerInApp.OnUnknownEvent();
                    break;
                }
                break;
			default:
				Ad.OnAdUnknownEvent ();
			break;
			}	
		}
		public void onCrossClicked ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			lockCrossEvent = true;
			#else
			continueOnCrossClicked ();
			#endif
		}
		void continueOnCrossClicked ()
		{
			#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_EDITOR
			if (lockCrossEvent)
				return;
			#endif
			Ad.OnCrossClicked ();
		}

		public void AdDisabledMessageHandler (string message)
		{
			Ad.OnFinishDisableAd (message.ToLower ().Equals ("success"));
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Disable Ads event handled with message: " + message);
			}
		}
		public void AdEnabledMessageHandler (string message)
		{
			Ad.OnFinishEnableAd (message.ToLower ().Equals ("success"));
			if (DebugMode)
			{
				AMEvents.amLogger.Log ("Enable Ads event handled with message: " + message);
			}
		}
		public void onPolicyHandler (string events)
		{
			string type = "none";
			string eventName = "none";
			try 
			{
				var ev = AMJSON.JsonDecode (events) as Hashtable;
				eventName = (string)ev ["event"];
				var tp = AMJSON.JsonDecode (events) as Hashtable;
				type = (string)tp ["type"];
			} 
			catch (Exception)
			{
				if (amLogger != null)
					amLogger.Log ("Error Parse Event. json: " + events);
			}

			switch (eventName)
			{
				case "ready":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Policy ready event handled");
					}
					Policy.OnReadyEvent();
					break;
				case "shown":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Policy shown event handled");
					}
					Policy.OnShownEvent();
					break;
				case "error":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Policy error event handled: " + type);
					}
					Policy.OnErrorEvent(type);
					break;
				case "accepted":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Policy accepted event handled: " + type);
					}
					Policy.OnAcceptedEvent(type);
					break;
				case "revoked":
					if (DebugMode)
					{
						if (amLogger != null)
							amLogger.Log ("Policy revoked event handled: " + type);
					}
					Policy.OnRevokedEvent(type);
					break;
			}
		}
		/// <summary>
		/// Raises the application inactive event.
		/// </summary>
		public static void OnApplicationInactive ()
		{
			if (ApplicationInactiveEvent != null)
			{
				ApplicationInactiveEvent ();
			}
		}
		/// <summary>
		/// Raises the new scene event.
		/// </summary>
		public void OnNewScene ()
		{
			if (NewSceneEvent != null)
			{
				NewSceneEvent ();
			}
		}
		/// <summary>
		/// Raises the pause event event.
		/// </summary>
		public void OnPauseEvent ()
		{
			if (PauseEvent != null)
			{
				PauseEvent ();
			}
		}
		/// <summary>
		/// Raises the resume event event.
		/// </summary>
		public void OnResumeEvent ()
		{
			if (ResumeEvent != null)
			{
				ResumeEvent ();
			}
		}
		void ChangeAutoInterstitial ()
		{
			AMEvents.NewSceneEvent -= NativeBridge.NewSceneEvent;
			AMEvents.PauseEvent -= NativeBridge.PauseSceneEvent;
			AMEvents.ResumeEvent -= NativeBridge.ResumeSceneEvent;

			AMEvents.ApplicationInactiveEvent -= NativeBridge.ApplicationInactiveEvent;
			AMEvents.OrientationChangeEvent -= NativeBridge.OrientationChangeEvent;
			if (AutoInterstitial)
			{
				AMEvents.NewSceneEvent += NativeBridge.NewSceneEvent;
				AMEvents.PauseEvent += NativeBridge.PauseSceneEvent;
				AMEvents.ResumeEvent += NativeBridge.ResumeSceneEvent;

				AMEvents.ApplicationInactiveEvent -= NativeBridge.ApplicationInactiveEvent;
				AMEvents.OrientationChangeEvent -= NativeBridge.OrientationChangeEvent;
			}
		}
		/// <summary>
		/// Sets the status.
		/// </summary>
		/// <param name="_statuses">_statuses.</param>
		/// <param name="isProcess">If set to <c>true</c> is process.</param>
		void SetStatusInside (string[] _statuses, bool isProcess)
		{
			statuses = _statuses;
			isProcessStatus = isProcess;
			indexStatus = -1;
			adsLoaded = false;
			NextStatus ();
		}
		/// <summary>
		/// Nexts the status.
		/// </summary>
		void NextStatus ()
		{
			if (adsLoaded) 
			{
				if (isProcessStatus)
				{
					fastChange = true;
				}
				else
				{
					indexStatus = statuses.Length;
				}
			}
			indexStatus++;
			if (indexStatus < statuses.Length) 
			{
				float maxTime = ALL_TIME/ (float)statuses.Length;
				float randomTime = UnityEngine.Random.Range (maxTime/2.0f, maxTime);

				if (fastChange)
				{
					randomTime = UnityEngine.Random.Range (2, 4);
				}
				maxTime -= maxTime/4.0f;
				float speed = maxTime/randomTime;
				if (indexStatus != 0)
				if (ChangeStatus != null)
					ChangeStatus (statuses[indexStatus], speed);
				StartCoroutine (StatusTimer (randomTime, NextStatus));
			}
			else
			{
				InterstitialLoadedEvent -= interstitialLoadedEvent;
				InterstitialFailedEvent -= interstitialFailedEvent;

				NativeBridge.RemoveInterstitialListener ();
				if (EndShowStatus!= null)
				{
					EndShowStatus ();
				}
				if (adsLoaded)
				{
					PlayerPrefs.SetInt (NAME_LAST_STARTUP_DAY, System.DateTime.Now.DayOfYear);
					PlayerPrefs.Save ();
					shown = true;
				}
				firstShow = true;
			}
		}
		public static bool isShowStartPopup ()
		{
			try 
			{
				if (Instance != null)
					return Instance.isShowStartPopupInside ();
				else
					return false;
			}
			catch (Exception ex) 
			{
				if (amLogger != null)
					amLogger.Log ("isShowStartPopup Error. Message: " + ex.Message);
				return false;
			}
		}
		/// <summary>
		/// Ises the start update.
		/// </summary>
		/// <returns><c>true</c>, if start update was ised, <c>false</c> otherwise.</returns>
		bool isShowStartPopupInside ()
		{
			if (firstShow)
				return false;
			#if UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS
			ListenerReady = NativeBridge.SetInterstitialListener (currentGameObjectName);

			if (shown || !AMNetwork.IsConnected () || !ListenerReady)
			{
				return false;
			}
			else
			{
				int lastStartupDay = PlayerPrefs.GetInt (NAME_LAST_STARTUP_DAY, -1);
				if (lastStartupDay == System.DateTime.Now.DayOfYear)
				{
					shown = true;//для показа один раз в сутки
					return false;
				}
				else 
					return true;
			}
			#else
			return false;
			#endif
		}
		/// <summary>
		/// Ises the show exit.
		/// </summary>
		/// <returns><c>true</c>, if show exit was ised, <c>false</c> otherwise.</returns>
		public static bool isShowExitPopup ()
		{
			#if UNITY_ANDROID
			return ListenerReady;
			#else
			return false;
			#endif
		}
		/// <summary>
		/// Interstitials the loaded event.
		/// </summary>
		void interstitialLoadedEvent ()
		{
			amLogger.Log ("interstitialLoadedEvent");
			adsLoaded = true;
			TokenCancel = true;
		}
		void interstitialFailedEvent ()
		{
		}
		static float acum = 0.0f;
		static float deltaInterval = 0.5f;
		static bool TokenCancel = false;

		/// <summary>
		/// Statuses the timer.
		/// </summary>
		/// <returns>The timer.</returns>
		/// <param name="interval">Interval.</param>
		/// <param name="callback">Callback.</param>
		IEnumerator StatusTimer (float interval, System.Action callback)
		{
			while ((interval > acum) && (!TokenCancel))
			{
				acum += deltaInterval;
				yield return new WaitForSeconds (deltaInterval);
			}
			acum = 0.0f;
			TokenCancel = false;
			callback ();
		}
		/// <summary>
		/// Ons the interstitial loaded.
		/// </summary>
		void onInterstitialLoaded ()
		{
			if (InterstitialLoadedEvent != null)
			{
				InterstitialLoadedEvent ();
			}
		}
		/// <summary>
		/// Ons the interstitial failed.
		/// </summary>
		public static void onInterstitialFailedStatic ()
		{
			if (Instance != null)
				Instance.onInterstitialFailed ();
		}
		/// <summary>
		/// Ons the interstitial failed.
		/// </summary>
		void onInterstitialFailed ()
		{
			if (InterstitialFailedEvent != null)
			{
				InterstitialFailedEvent ();
			}
		}
		ScreenOrientation CurrentOrientation = ScreenOrientation.Unknown;
		/// <summary>
		/// Checks the orientation.
		/// </summary>
		void CheckOrientation ()
		{
			if (CurrentOrientation != Screen.orientation) 
			{
				CurrentOrientation = Screen.orientation;
				if (OrientationChangeEvent != null)
					OrientationChangeEvent ();
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
				Scene newScene = SceneManager.GetActiveScene ();
				var noNewSceneInterstitialObjects = 
				newScene.GetRootGameObjects ().ToList ().FindAll ((go)=>{return go.GetType () == (typeof (GameObject)) && go.name == NO_NEW_SCENE_EVENT;});
				bool skipNewSceneEvent = false;
				if (noNewSceneInterstitialObjects != null && noNewSceneInterstitialObjects.Count != 0)
				{
					skipNewSceneEvent = true;
				}
				else if (currentLevel == CUSTOM_CODE_SCENE_NAME || currentLevel == string.Empty)
				{
					skipNewSceneEvent = true;
				}
				else if (autoInterstitialScenes != null && autoInterstitialScenes.FindIndex (s => {return s.name == newLevel && !s.skip;}) == -1)
				{
					skipNewSceneEvent = true;
				}

				time = 0;
				currentLevel = newLevel;

				if (NewSceneEvent != null && AutoInterstitial && !skipNewSceneEvent && !Ad.disablingAdsIsInProgress)
				{
					OnNewScene ();
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
		static UI.Menu newMenu;
		public void DrawDebugMenu ()
		{
			if (newMenu == null)
				newMenu = new UI.Menu ();
			
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
		void OnEnable()
		{

		}
		public static bool fixWindow = true;
		void FixedUpdate()
		{
			if (fixWindow == false) 
			{
				if (Policy.IsPolicyShown() == false)
				{
					fixWindow = true;
					Invoke("CheckStatusPolicy", 1f);
				}
			}
		}
		void CheckStatusPolicy(string check)
		{
			JSONNode jsonNode = JSON.Parse (check);

			string type = jsonNode ["type"].ToString();

			if (Policy.IsBasePolicyAccepted())
			{
				Policy.OnAcceptedEvent(type);
			}
			if (Policy.IsRegisterPolicyRevoked())
			{
				Policy.OnRevokedEvent(type);
			}
			if (Policy.IsRegisterPolicyAccepted())
			{
				Policy.OnAcceptedEvent(type);
			}
		}
	}
}