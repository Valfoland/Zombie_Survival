using UnityEngine;
using System;
using System.Collections;

namespace AMEvents
{
	/// <summary>
	/// Класс для управления рекламным SDK.
	/// Предоставляет функционал для показа фуллскрин баннера, видео рекламы, видео за награду, автоинапп окна, отключения рекламы и получения соответствующих событий.
	/// </summary>
	public class Ad
	{
		public static event System.Action Unknown;
		public static event System.Action CrossClicked;
		public static event System.Action<bool> FinishDisableAd;
		public static event System.Action<bool> FinishEnableAd;
		public static bool enableCrossRequired = false;
		public static bool crossLockRequired = false;
		public static bool crossIsLocked = false;
		public static bool disablingAdsIsInProgress = false;

		/// <summary>
		/// Метод для активации крестика на стандартном баннере.
		/// Функционал крестика так же используется в кнопке Remove Ads.
		/// </summary>
		public static void EnableCross (bool lockRequired = false)
		{
			crossLockRequired = lockRequired;

			if (string.IsNullOrEmpty (AMEvents.currentGameObjectName))
				enableCrossRequired = true;
			else
				NativeBridge.EnableCross (AMEvents.currentGameObjectName);
		}
		/// <summary>
		/// Корутина активации крестика.
		/// Дожидается готовности плагина для выполнения метода EnableCross
		/// </summary>
		static IEnumerator EnableCrossCoroutine ()
		{
			while (string.IsNullOrEmpty (AMEvents.currentGameObjectName)) 
			{
				yield return null;
			}
			if (AMEvents.Instance != null && AMEvents.Instance.DebugMode)
				AMEvents.amLogger.Log ("Enable cross from coroutine");
			NativeBridge.EnableCross (AMEvents.currentGameObjectName);

		}
		/// <summary>
		/// Событие окончания возобновления рекламы.
		/// </summary>
		public static void OnFinishDisableAd (bool success)
		{
			UnlockCross ();

			if (FinishDisableAd != null)
			{
				FinishDisableAd (success);
			}

			disablingAdsIsInProgress = false;
		}
		/// <summary>
		/// Событие окончания восстановления рекламы.
		/// </summary>
		/// <param name="success">If set to <c>true</c> success.</param>
		public static void OnFinishEnableAd (bool success)
		{
			if (FinishEnableAd != null)
			{
				FinishEnableAd (success);
			}
		}
		/// <summary>
		/// Событие нажатия на крестик (кнопку отключения рекламы).
		/// </summary>
		public static void OnCrossClicked ()
		{
			if (crossLockRequired)
			{
				if (crossIsLocked)
					return;
				else
					crossIsLocked = true;
			}
			if (AMEvents.Instance.DebugMode)
			{
				if (AMEvents.amLogger != null)
					AMEvents.amLogger.Log ("Cross clicked event handled");
			}
			if (CrossClicked != null)
			{
				CrossClicked ();
			}
		}
		/// <summary>
		/// Метод разблокировки повторных нажатий на крестик.
		/// </summary>
		public static void UnlockCross ()
		{
			crossIsLocked = false;
		}
		/// <summary>
		/// Неизвестное событие получено из натива. Что-то пошло не так
		/// </summary>
		public static void OnAdUnknownEvent ()
		{
			if (Unknown != null)
			{
				Unknown ();
			}
		}
		/// <summary>
		/// Класс стандартного баннера.
		/// Так же включает методы отключения рекламы.
		/// </summary>
		public class Banner
		{

			static int defaultBannerWidth 
			{
				get
				{
					return NativeBridge.GetBannerHeight ();
				}
			}
			static int defaultBannerHeight
			{
				get
				{
					return NativeBridge.GetBannerHeight ();
				}
			}			
			/// <summary>
			/// Метод для получения ширины стандартного баннера.
			/// </summary>
			public static int GetBannerWidth ()
			{
				int bw = NativeBridge.GetBannerWidth ();
				return bw != 0?bw:defaultBannerWidth;
			}
			/// <summary>
			/// Метод для получения высоты стандартного баннера.
			/// </summary>
			public static int GetBannerHeight ()
			{
				int bh = NativeBridge.GetBannerHeight ();
				return bh != 0?bh:defaultBannerHeight;
			}
			/// <summary>
			/// Метод проверки отключена ли реклама.
			/// </summary>
			public static bool IsAdDisabled ()
			{
				return NativeBridge.IsAdDisabled ();
			}
			/// <summary>
			/// Метод для отключения рекламы.
			/// </summary>
			public static void DisableAd ()
			{
				disablingAdsIsInProgress = true;

				NativeBridge.DisableAd (AMEvents.currentGameObjectName);
			}
			/// <summary>
			/// Метод для возобновления рекламы.
			/// </summary>
			public static void EnableAd ()
			{
				NativeBridge.EnableAd (AMEvents.currentGameObjectName);
			}
			/// <summary>
			/// Метод для изменения отображения баннера.
			/// </summary>
			public static void SetBannerVisible (bool value)
			{
				NativeBridge.SetBannerVisible (value);
			}
		}

		/// <summary>
		/// Класс фуллскрин рекламы.
		/// </summary>
		public class Interstitial
		{
			#region Fullscreen Banner
			public static event System.Action Ready;
			public static event System.Action CommercialReady;
			public static event System.Action Impression;
			public static event System.Action Closed;
			public static event System.Action Unknown;

			/// <summary>
			/// Событие готовности к показу фуллскрин рекламы. Срабатывает при наличии баннера и прошествии минимального временного ограничения между показами.
			/// </summary>
			public static void OnReadyEvent ()
			{
				if (Ready != null)
				{
					Ready ();
				}
			}
			/// <summary>
			/// Событие готовности к показу коммерческой фуллскрин рекламы. Срабатывает при наличии баннера и прошествии минимального временного ограничения между показами.
			/// </summary>
			public static void OnCommercialReady ()
			{
				if (CommercialReady != null)
				{
					CommercialReady ();
				}
			}
			/// <summary>
			/// Событие показа статичной рекламы.
			/// </summary>
			public static void OnImpressionEvent ()
			{
				if (Impression != null)
				{
					Impression ();
				}
			}
			/// <summary>
			/// Событие закрытия фуллскрин рекламы.
			/// </summary>
			public static void OnClosedEvent ()
			{
				if (Closed != null)
				{
					Closed ();
				}
			}
			/// <summary>
			/// Неизвестное событие статичной рекламы. Что-то пошло не так
			/// </summary>
			public static void OnUnknownEvent ()
			{
				if (Unknown != null)
				{
					Unknown ();
				}
			}
			/// <summary>
			/// Метод проверки готовности фуллскрин рекламы к показу.
			/// </summary>
			/// <returns><c>true</c> при наличии баннера и прошествии минимального интервала между показами; иначе, <c>false</c>.</returns>
			public static bool IsReady ()
			{
				return NativeBridge.IsInterstitialReady ();
			}
			/// <summary>
			/// Метод проверки готовности коммерческого видео к показу.
			/// </summary>
			public static bool IsCommercialReady ()
			{
				return NativeBridge.IsCommercialReady ();
			}
			/// <summary>
			/// Метод для установки нового значения минимального интервала (в секундах) между показами рекламы.
			/// </summary>
			public static bool setInterstitialDelay (int seconds)
			{
				return NativeBridge.setInterstitialDelay (seconds);
			}
			/// <summary>
			/// Метод для получения текущего значения минимального интервала между показами рекламы.
			/// </summary>
			public static int getCurrentInterstitialDelay ()
			{
				return NativeBridge.getCurrentInterstitialDelay ();
			}
			#endregion
			
			/// <summary>
			/// Метод для запроса на показ фуллскрин рекламы.
			/// Реклама будет показана не чаще, чем 1 раз в 1 минуту.
			/// Если готово к показу видео - будет показано видео, иначе - статичная реклама.
			/// </summary>
			public static void Show ()
			{
				NativeBridge.ShowInterstitial ();
			}
			/// <summary>
			/// Метод для получения времени в секундах с момента закрытия последней фуллскрин рекламы.
			/// </summary>
			/// <returns>The after interstitial.</returns>
			public static int TimeAfterInterstitial ()
			{
				return NativeBridge.TimeAfterInterstitial ();
			}
		}

		/// <summary>
		/// Класс видео за награду.
		/// </summary>
		public class RewardedVideo
		{
			public static event System.Action Ready;
			public static event System.Action Impression;
			public static event System.Action<int> Success;
			public static event System.Action Closed;
			public static event System.Action Unknown;

			/// <summary>
			/// Событие готовности видео к показу.
			/// </summary>
			public static void OnReadyEvent ()
			{
				if (Ready != null)
				{
					Ready ();
				}
			}
			/// <summary>
			/// Событие показа видео за награду.
			/// </summary>
			public static void OnImpressionEvent ()
			{
				if (Impression != null)
				{
					Impression ();
				}
			}
			/// <summary>
			/// Событие успешного завершения просмотра видео.
			/// При этом событии пользователю выдается награда за просмотр.
			/// </summary>
			public static void OnSuccessEvent (int reward)
			{
				if (Success != null)
				{
					Success (reward);
				}
			}
			/// <summary>
			/// Событие закрытия видео за награду.
			/// Происходит как при успешном завершении просмотра, так и при отмене.
			/// </summary>
			public static void OnClosedEvent ()
			{
				if (Closed != null)
				{
					Closed ();
				}
			}
			/// <summary>
			/// Неизвестное событие видео за награду. Что-то пошло не так
			/// </summary>
			public static void OnUnknownEvent ()
			{
				if (Unknown != null)
				{
					Unknown ();
				}
			}
			/// <summary>
			/// Метод проверки готовности видео к показу.
			/// </summary>
			public static bool IsReady ()
			{
				return NativeBridge.IsRewardedVideoReady ();
			}
			/// <summary>
			/// Метод для показа видео за награду.
			/// </summary>
			public static void Show ()
			{
				NativeBridge.ShowRewardedVideo ();
			}
		}

		/// <summary>
		/// Класс автоинаппа (встроенного нативного инапп окна для отключения рекламы).
		/// </summary>
        public class InnerInApp
        {
            public static event System.Action Impression;
            public static event System.Action Success;
			public static event System.Action Failed;
            public static event System.Action Closed;
            public static event System.Action Unknown;

			/// <summary>
			/// Событие показа автоинапп окна (или алерта, при отсутствии сети).
			/// </summary>
			public static void OnImpressionEvent ()
            {
                if (Impression != null)
                {
                    Impression ();
                }
            }
			/// <summary>
			/// Событие успешной покупки или восстановления инаппа на отключение рекламы.
			/// </summary>
            public static void OnSuccessEvent ()
            {
                if (Success != null)
                {
                    Success ();
                }
            }
			/// <summary>
			/// Событие ошибки при покупке или восстановлении инаппа на отключение рекламы.
			/// </summary>
			public static void OnFailedEvent ()
			{
				if (Failed != null)
				{
					Failed ();
				}
			}
			/// <summary>
			/// Событие закрытия автоинапп окна (или алерта, при отсутствии сети).
			/// </summary>
			public static void OnClosedEvent ()
            {
                if (Closed != null)
                {
                    Closed ();
                }
            }
			/// <summary>
			/// Неизвестное событие. Что-то пошло не так
			/// </summary>
			public static void OnUnknownEvent ()
            {
                if (Unknown != null)
                {
                    Unknown ();
                }
            }
			/// <summary>
			/// Метод для показа автоинапп окна.
			/// При отсутствии сети будет показан аллерт.
			/// </summary>
			public static void Show ()
			{
				NativeBridge.ShowInnerInApp ();
			}
			/// <summary>
			/// Метод для совершения покупки автоинаппа на отключение рекламы.
			/// </summary>
			public static void Buy ()
			{
				NativeBridge.BuyInnerInApp ();
			}
			/// <summary>
			/// Метод для восстановления купленного автоинаппа на отключение рекламы.
			/// </summary>
			public static void Restore ()
			{
				NativeBridge.RestoreInnerInApp ();
			}
        }
    }
}