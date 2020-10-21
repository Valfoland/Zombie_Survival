using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

namespace AMLogging
{
	/// <summary>
	/// Класс для логирования, а так же сбора и передачи Unity логов в консоль в приложении.
	/// </summary>
	public class AMLogger
	{
		public static readonly AMLogger Logger = new AMLogger ();

		private string logPrefix = string.Empty;
		private StringBuilder logBuilder;

		private AMLogger () {}
		private AMLogger (string logPrefix) 
		{
			if (logPrefix != null)
				this.logPrefix = !logPrefix.EndsWith (" ") ? logPrefix + " " : logPrefix;
		}

		/// <summary>
		/// Метод для создания инстанса логгера с префиксом.
		/// </summary>
		/// <ret<returns>Инстанс логгера с префиксом.</returns>
		/// <param name="logPrefix">Префикс логгера.</param>
		public static AMLogger GetInstance (string logPrefix) 
		{
			return new AMLogger (logPrefix);
		}

		/// <summary>
		/// Метод для сбора Unity логов.
		/// </summary>
		/// <param name="logString">Текст лога.</param>
		/// <param name="stackTrace">Стэк трэйс лога.</param>
		/// <param name="type">Тип лога.</param>
		public void HandleLog (string logString, string stackTrace, LogType type) 
		{
			if (AMLoggerConsole.logs == null)
			{
				AMLoggerConsole.logs = new List<AMLoggerConsole.Log> ();
			}
			if (AMLoggerConsole.collapsedLogs == null)
			{
				AMLoggerConsole.collapsedLogs = new List<AMLoggerConsole.Log> ();
			}

			if (AMLoggerConsole.logs.Count == 30) 
			{
				AMLoggerConsole.logs.RemoveAt (0);
			}
			if (AMLoggerConsole.collapsedLogs.Count == 30) 
			{
				AMLoggerConsole.collapsedLogs.RemoveAt (0);
			}

			List<AMLoggerConsole.Log> tempLogs = AMLoggerConsole.logs;

			string newMessage;
			string newTimestamp;
			try
			{
				logBuilder = new StringBuilder ();
				logBuilder.Append ("[");
				logBuilder.Append (DateTime.Parse (logString.Substring (1, 19)).ToString ());
				logBuilder.Append ("] ");
				newTimestamp = logBuilder.ToString ();
				newMessage = logString.Substring (22);
			}
			catch (Exception)
			{
				newTimestamp = string.Empty;
				newMessage = logString;
			}

			AMLoggerConsole.Log newLog = new AMLoggerConsole.Log {
				timestamp = newTimestamp,
				message = newMessage,
				stackTrace = stackTrace,
				type = type,
				logCount = 1,
			};

			tempLogs.Add (newLog);

			int newLogCount = 1;
			var sameLogs = tempLogs.FindAll (log => {
				return log.message == newLog.message && log.stackTrace == newLog.stackTrace;
			});
			if (sameLogs != null)
			{
				newLogCount = sameLogs.Count;
			}

			int sameCollapsedLogIndex = AMLoggerConsole.collapsedLogs.FindIndex (log => {
				return log.message == newLog.message && log.stackTrace == newLog.stackTrace;
			});
			if (sameCollapsedLogIndex == -1)
			{
				AMLoggerConsole.collapsedLogs.Add (new AMLoggerConsole.Log {
					timestamp = newLog.timestamp,
					message = newLog.message,
					stackTrace = newLog.stackTrace,
					type = newLog.type,
					logCount = newLogCount,
				});
			}
			else
			{
				AMLoggerConsole.collapsedLogs[sameCollapsedLogIndex] = new AMLoggerConsole.Log {
					timestamp = AMLoggerConsole.collapsedLogs[sameCollapsedLogIndex].timestamp, 
					message = AMLoggerConsole.collapsedLogs[sameCollapsedLogIndex].message,
					stackTrace = AMLoggerConsole.collapsedLogs[sameCollapsedLogIndex].stackTrace,
					type = AMLoggerConsole.collapsedLogs[sameCollapsedLogIndex].type,
					logCount = newLogCount,
				};
			}

			AMLoggerConsole.logs = tempLogs;
			tempLogs = null;
		}

		/// <summary>
		/// Метод для вывода обычного сообщения в лог Unity и отправки в натив для вывода на OSX.
		/// </summary>
		/// <param name="message">Текст лога.</param>
		public void Log (string message) 
		{
#if !UNITY_STANDALONE_OSX
			if (message == null) 
				return;
			
			try 
			{
				logBuilder = new StringBuilder ();
				if (Application.isEditor && AMLoggerConsole.Instance != null && AMLoggerConsole.Instance.useTimestamp)
				{
					logBuilder.Append ("[");
					logBuilder.Append (DateTime.Now.ToString ());
					logBuilder.Append ("] ");
				}
				logBuilder.Append (logPrefix);
				logBuilder.Append (message);
				message = logBuilder.ToString ();

				Debug.Log (message);
				if (AMLoggerConsole.customCodeEnable && !AMLoggerConsole.customCodeIsInit)
					HandleLog (message, string.Empty, LogType.Log);
			} 
			catch (Exception e) 
			{
				Debug.LogException (e);
			}
#endif
		}

		/// <summary>
		/// Метод для вывода предупреждающего сообщения в лог Unity и отправки в натив для вывода на OSX.
		/// </summary>
		/// <param name="message">Текст лога.</param>
		public void LogWarning (string message) 
		{
#if !UNITY_STANDALONE_OSX
			if (message == null) 
				return;
			
			try 
			{
				logBuilder = new StringBuilder ();
				if (Application.isEditor && AMLoggerConsole.Instance != null && AMLoggerConsole.Instance.useTimestamp)
				{
					logBuilder.Append ("[");
					logBuilder.Append (DateTime.Now.ToString ());
					logBuilder.Append ("] ");
				}
				logBuilder.Append (logPrefix);
				logBuilder.Append (message);
				message = logBuilder.ToString ();

				Debug.LogWarning (message);
				if (AMLoggerConsole.customCodeEnable && !AMLoggerConsole.customCodeIsInit)
					HandleLog (message, string.Empty, LogType.Warning);
			} 
			catch (Exception e) 
			{
				Debug.LogException (e);
			}
#endif
		}

		/// <summary>
		/// Метод для вывода сообщения об ошибке в лог Unity и отправки в натив для вывода на OSX.
		/// </summary>
		/// <param name="message">Текст лога.</param>
		public void LogError (string message) 
		{
#if !UNITY_STANDALONE_OSX
			if (message == null) 
				return;
			
			try 
			{
				logBuilder = new StringBuilder ();
				if (Application.isEditor && AMLoggerConsole.Instance != null && AMLoggerConsole.Instance.useTimestamp)
				{
					logBuilder.Append ("[");
					logBuilder.Append (DateTime.Now.ToString ());
					logBuilder.Append ("] ");
				}
				logBuilder.Append (logPrefix);
				logBuilder.Append (message);
				message = logBuilder.ToString ();

				Debug.LogError (message);
				if (AMLoggerConsole.customCodeEnable && !AMLoggerConsole.customCodeIsInit)
					HandleLog (message, string.Empty, LogType.Error);
			} 
			catch (Exception e) 
			{
				Debug.LogException (e);
			}
#endif
		}

		/// <summary>
		/// Метод для вывода исключения в лог Unity.
		/// </summary>
		/// <param name="message">Текст лога.</param>
		public void LogException (Exception exception) 
		{
			if (exception == null) 
				return;
			
			try 
			{
				Debug.LogException (exception);
				if (AMLoggerConsole.customCodeEnable && !AMLoggerConsole.customCodeIsInit)
					HandleLog (exception.ToString (), string.Empty, LogType.Exception);
			}
			catch (Exception e) 
			{
				Debug.LogException (e);
			}
		}
	}
}