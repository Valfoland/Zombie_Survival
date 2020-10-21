using UnityEngine;
using System;
using System.Collections;

namespace AMEvents
{
	/// <summary>
	/// Класс для управления окном политики.
	/// Предоставляет функционал для показа окна политики и получения соответствующих событий.
	/// </summary>
	public class Policy
	{
		/// <summary>
		/// Класс окна политики.
		/// </summary>
		public static event System.Action Ready;
		public static event System.Action Shown;
		public static event System.Action<string> Error;
		public static event System.Action<string> Accepted;
		public static event System.Action<string> Revoked;
		public static event System.Action Unknown;

		/// <summary>
		/// Событие готовности к показу окна политики.
		/// </summary>
		public static void OnReadyEvent ()
		{
			if (Ready != null)
			{
				Ready ();
			}
		}
		/// <summary>
		/// Событие что-то пошло не так.
		/// </summary>
		public static void OnUnknownEvent ()
		{
			if (Unknown != null)
			{
				Unknown ();
			}
		}
		/// <summary>
		/// Событие показа окна политики.
		/// </summary>
		public static void OnShownEvent ()
        {
            if (Shown != null)
            {
            	Shown ();
            }
        }
		/// <summary>
		/// Метод проверки готовности окна политики к показу.
		/// </summary>
		/// <returns><c>true</c> при наличии окна политики; иначе, <c>false</c>.</returns>
		public static bool IsReady ()
		{
			return NativeBridge.IsPolicyReady ();
		}
		/// <summary>
		/// Событие ошибки при показе окна политики.
		/// </summary>
		public static void OnErrorEvent (string type)
        {
            if (Error != null)
            {
                Error (type);
            }
        }
		/// <summary>
		/// Событие успешного принятия политики пользователем.
		/// </summary>
		public static void OnAcceptedEvent (string type)
        {
            if (Accepted != null)
            {
                Accepted (type);
            }
        }
		/// <summary>
		/// Событие обработанного окна политики.
		/// </summary>
		public static void OnRevokedEvent (string type)
        {
            if (Revoked != null)
            {
                Revoked (type);
            }
        }
		/// <summary>
		/// Метод для вызова диалогового окна с кратким сообщением об изменениях в политике конфиденциальности, ссылкой на полный текст и кнопкой принятия.
		/// </summary>
		public static void ShowPolicyContent ()
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
			if (IsBasePolicyAccepted() == false)
				{
					if(IsReady() == true)
					{
						if (IsBasePolicyRequired() == true){
							NativeBridge.ShowPolicyContent ();
							Shown();
							AMEvents.fixWindow = false;
						}
					}
				}
			#else
			NativeBridge.ShowPolicyContent ();
			#endif
		}
		/// <summary>
		/// Метод для вызова диалогового окна с полным сообщением об изменениях в политике конфиденциальности и кнопками принятия и отмены.
		/// </summary>
		public static void ShowRegisterPolicyContent ()
		{
			#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
			if (IsRegisterPolicyAccepted() == false)
				{
					if(IsReady() == true)
					{
						if (IsRegisterPolicyRequired() == true)
						{
							NativeBridge.ShowRegisterPolicyContent ();
							Shown();
							AMEvents.fixWindow = false;
						}
					}
				}
			#else
			NativeBridge.ShowRegisterPolicyContent ();
			#endif
		}
		/// <summary>
		/// Метод для запроса локализованного текста для реализации авторизации в соц.сетях
		/// </summary>
		public static string GetRegisterText ()
		{
			return NativeBridge.GetRegisterText ();
		}
		/// <summary>
		/// Метод для запроса ссылки на страницу с полным текстом политики конфиденциальности для самостоятельной интеграции.
		/// </summary>
		public static string GetPrivacyUrl ()
		{
			return NativeBridge.GetPrivacyUrl ();
		}
		/// <summary>
		/// Метод для запроса ссылки на страницу с полным текстом Eula для самостоятельной интеграции.
		/// </summary>
		public static string GetEulaUrl ()
		{
			return NativeBridge.GetEulaUrl ();
		}
		/// <summary>
		/// Метод для запроса ссылки на страницу с полным текстом Tos для самостоятельной интеграции.
		/// </summary>
		public static string GetTosUrl ()
		{
			return NativeBridge.GetTosUrl ();
		}
		/// <summary>
		/// Метод для запуска инициализации модуля и выполнения запроса данных политики с сервера (Только для MAC).
		/// </summary>
		public static void StartGDPR ()
		{
			NativeBridge.StartGDPR ();
		}
		/// <summary>
		/// Начало показа окна политики (Только для MAC).
		/// </summary>
		public static bool IsPolicyShown ()
		{
			return NativeBridge.IsPolicyShown ();
		}
		/// <summary>
		/// Проверка для показа окна базовой политики в Евросоюзе (Только для MAC).
		/// </summary>
		public static bool IsBasePolicyRequired ()
		{
			return NativeBridge.IsBasePolicyRequired ();
		}
		/// <summary>
		/// Проверка для показа окна регистровой политики, если стоит галочка в APPS (Только для MAC).
		/// </summary>
		public static bool IsRegisterPolicyRequired ()
		{
			return NativeBridge.IsRegisterPolicyRequired ();
		}
		/// <summary>
		/// Проверка принятия окна базовой политики (Только для MAC).
		/// </summary>
		public static bool IsBasePolicyAccepted ()
		{
			return NativeBridge.IsBasePolicyAccepted ();
		}
		/// <summary>
		/// Проверка принятия окна регистровой политики (Только для MAC).
		/// </summary>
		public static bool IsRegisterPolicyAccepted ()
		{
			return NativeBridge.IsRegisterPolicyAccepted ();
		}
		/// <summary>
		/// Проверка отмены окна регистровой политики (Только для MAC).
		/// </summary>
		public static bool IsRegisterPolicyRevoked ()
		{
			return NativeBridge.IsRegisterPolicyRevoked ();
		}
		/// <summary>
		/// Проверка ошибки окна политики (Только для MAC).
		/// </summary>
		public static string GetPolicyError ()
		{
			return NativeBridge.GetPolicyError ();
		}
	}
}
