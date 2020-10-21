using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AnalytiAll.UI
{
	public class Menu 
	{
		MenuFolder mainMenuFolder;
		Stack stackCalls;

		void InitStackCalls ()
		{
			stackCalls = new Stack ();
		}

		void InitMenuFolder ()
		{
			InitStackCalls ();

			var back = new MenuFolder (@"Back", new List<MenuFolder> ());
			back.click = () => {
				stackCalls.Pop ();
			};

			var SetCustomData = new MenuFolder ("Set Custom\nData", new List<MenuFolder> {});
			SetCustomData.click = () => {
				Dictionary<string, string> customData = new Dictionary<string, string> ();
				customData.Add ("player_name", "username");
				customData.Add ("player_level", "11");
				customData.Add ("own_gold", "1000");
				EventManager.SetCustomData (customData);
			};

			#region Generate Events
			var GenerateEventsStart = new MenuFolder ("Generate Events\nStart", new List<MenuFolder> {});
			GenerateEventsStart.click = () => {
				EventManager.Instance.GenerateEventsStart ();
			};

			var GenerateEventsStop = new MenuFolder ("Generate Events\nStop", new List<MenuFolder> {});
			GenerateEventsStop.click = () => {
				EventManager.Instance.GenerateEventsStop ();
			};

			var GenerateEvents = new MenuFolder ("Generate Events", new List<MenuFolder> {back, 
																						GenerateEventsStart, 
																						GenerateEventsStop});
			GenerateEvents.click = () => {
				stackCalls.Push (GenerateEvents);
			};
			#endregion

			#region Log Events
			var LogEvent = new MenuFolder (@"Log Event", new List<MenuFolder> ());
			LogEvent.click = () => {
				var eventData = new Dictionary<string, string> ();
				eventData.Add ("key1", "value1");
				eventData.Add ("key2", "value2");
				EventManager.LogEvent ("LogEvent", eventData);
			};

			var LogTimedEvent = new MenuFolder (@"Log Timed Event", new List<MenuFolder> ());
			LogTimedEvent.click = () => {
				var timedData = new Dictionary<string, string> ();
				timedData.Add ("key1", "value1");
				timedData.Add ("key2", "value2");
				AnalytiAll.EventManager.LogTimedEvent ("TestTimedEvent", timedData);
			};

			var EndTimedEvent = new MenuFolder (@"End Timed Event", new List<MenuFolder> ());
			EndTimedEvent.click = () => {
				AnalytiAll.EventManager.EndTimedEvent ("TestTimedEvent");
			};

			var CustomEvents = new MenuFolder (@"Custom Events", new List<MenuFolder> {back, 
																						LogEvent,
																						LogTimedEvent,
																						EndTimedEvent});
			CustomEvents.click = () => {
				stackCalls.Push (CustomEvents);
			};
			#endregion

			#region Inner Events
			//Regular Events
			var StartScene = new MenuFolder (@"Start Scene", new List<MenuFolder> ());
			StartScene.click = () => {
				AnalytiAll.EventManager.StartScene (404, "TestScene");
			};
			var ButtonClick = new MenuFolder (@"Button Click", new List<MenuFolder> ());
			ButtonClick.click = () => {
				AnalytiAll.EventManager.ButtonClick ("TestClick");
			};
			var GameOver = new MenuFolder (@"Game Over", new List<MenuFolder> ());
			GameOver.click = () => {
				AnalytiAll.EventManager.GameOver ("TestMode");
			};
			var RegularEvents = new MenuFolder (@"Regular Events", new List<MenuFolder> {back, 
																						StartScene, 
																						ButtonClick, 
																						GameOver});
			RegularEvents.click = () => {
				stackCalls.Push (RegularEvents);
			};

			//Level Events
			var StartLevel = new MenuFolder (@"Start Level", new List<MenuFolder> ());
			StartLevel.click = () => {
				AnalytiAll.EventManager.StartLevel ("TestLevel", "TestMode");
			};
			var LevelWin = new MenuFolder (@"Level Win", new List<MenuFolder> ());
			LevelWin.click = () => {
				AnalytiAll.EventManager.LevelWin ("TestLevel", "TestMode");
			};
			var LevelLoose = new MenuFolder (@"Level Lose", new List<MenuFolder> ());
			LevelLoose.click = () => {
				AnalytiAll.EventManager.LevelLose ("TestLevel", "TestMode");
			};
			var LevelEvents = new MenuFolder (@"Level Events", new List<MenuFolder> {back, 
																					StartLevel, 
																					LevelWin, 
																					LevelLoose});
			LevelEvents.click = () => {
				stackCalls.Push (LevelEvents);
			};

			//LootEvents
			var LootConsume = new MenuFolder (@"Loot Consume", new List<MenuFolder> ());
			LootConsume.click = () => {
				AnalytiAll.EventManager.LootConsume ("TestLoot", 666, "waste_of_money");
			};
			var LootAppend = new MenuFolder (@"Loot Append", new List<MenuFolder> ());
			LootAppend.click = () => {
				AnalytiAll.EventManager.LootAppend ("TestLoot", 777);
			};
			var LootEvents = new MenuFolder (@"Loot Events", new List<MenuFolder> {back, 
																				LootConsume, 
																				LootAppend});
			LootEvents.click = () => {
				stackCalls.Push (LootEvents);
			};

			//InApp Events
			var InAppShopBtnClick = new MenuFolder ("InApp Shop\nBtn Click", new List<MenuFolder> ());
			InAppShopBtnClick.click = () => {
				AnalytiAll.EventManager.InappShopButtonClick ();
			};
			var InAppShopItemClick = new MenuFolder ("InApp Shop\nItem Click", new List<MenuFolder> ());
			InAppShopItemClick.click = () => {
				AnalytiAll.EventManager.InappItemButtonClick ("TestNameInApp", "TestInappID");
			};
			var InAppPurchaseCompleted = new MenuFolder ("InApp Purchase\nCompleted", new List<MenuFolder> ());
			InAppPurchaseCompleted.click = () => {
				AnalytiAll.EventManager.InappPurchaseCompleted ("TestInappID");
			};
			var InAppPurchaseFailed = new MenuFolder ("InApp Purchase\nFailed", new List<MenuFolder> ());
			InAppPurchaseFailed.click = () => {
				AnalytiAll.EventManager.InappPurchaseFailed ("TestInappID");
			};
			var InAppPurchaseCancelled = new MenuFolder ("InApp Purchase\nCancelled", new List<MenuFolder> ());
			InAppPurchaseCancelled.click = () => {
				AnalytiAll.EventManager.InappPurchaseCancelled ("TestInappID");
			};
			var InAppPurchaseRefunded = new MenuFolder ("InApp Purchase\nRefunded", new List<MenuFolder> ());
			InAppPurchaseRefunded.click = () => {
				AnalytiAll.EventManager.InappPurchaseRefunded ("TestInappID");
			};
			var InAppPurchaseRestored = new MenuFolder ("InApp Purchase\nRestored", new List<MenuFolder> ());
			InAppPurchaseRestored.click = () => {
				AnalytiAll.EventManager.InappPurchaseRestored (true);
			};
			var InAppEvents = new MenuFolder (@"InApp Events", new List<MenuFolder> {back, 
																					InAppShopBtnClick,
																					InAppShopItemClick,
																					InAppPurchaseCompleted,
																					InAppPurchaseFailed,
																					InAppPurchaseCancelled,
																					InAppPurchaseRefunded,
																					InAppPurchaseRestored});
			InAppEvents.click = () => {
				stackCalls.Push (InAppEvents);
			};

			var InnerEvents = new MenuFolder (@"Inner Events", new List<MenuFolder> {back, 
																					RegularEvents, 
																					LevelEvents,
																					LootEvents,
																					InAppEvents});
			InnerEvents.click = () => {
				stackCalls.Push (InnerEvents);
			};
			#endregion

			mainMenuFolder = new MenuFolder ("Main", new List<MenuFolder> {SetCustomData, GenerateEvents, CustomEvents, InnerEvents});
			stackCalls.Push (mainMenuFolder);
		}

		public Menu ()
		{
			InitMenuFolder ();
		}

		public void Show ()
		{
			((MenuFolder)stackCalls.Peek ()).Show ();
		}

		public void Reset ()
		{
			stackCalls.Push (mainMenuFolder);
		}
	}

	class GUIButton
	{
		public Rect rect;
		public string text;
		public Action click;
	}

	class MenuFolder
	{
		public List<MenuFolder> menuFolders;
		List<GUIButton> buttons;
		Rect MenuRect; 
		public Action click;
		public string name;

		int gridX = 3;
		int gridY = 6;

		int dx;
		int dy;

		int btnIdx = -1;
		#if UNITY_TVOS
		DateTime idxMovedTime = new DateTime ();
		TimeSpan responseTime = new TimeSpan (0, 0, 0, 0, 200);
		#endif
		GUIStyle currentFolderLableStyle = null;
		GUIStyle normalButtonStyle = null;
		GUIStyle selectedButtonStyle = null;

		public MenuFolder (string name, List<MenuFolder> menuFolders)
		{
			this.menuFolders = new List<MenuFolder> (menuFolders);
			this.buttons = new List<GUIButton> ();
			this.name = name;

			var index = 0;

			dx = Screen.width / (2 * (gridX));
			dy = Screen.height / (gridY + 2);

			MenuRect = new Rect (Screen.width / 2, 0, gridX * dx, dy * 5);

			if (menuFolders != null) 
			{
				foreach (var item in menuFolders) 
				{
					var indexX = index % gridX;
					var indexY = index / gridX;

					var guiButton = new GUIButton ();
					guiButton.rect = new Rect (Screen.width / 2 + dx * (indexX), dy * (indexY + 1), dx, dy);
					guiButton.text = item.name;
					guiButton.click = item.click;

					buttons.Add (guiButton);
					index++;
				}
			}
		}

		#if UNITY_TVOS
		void RemoteControllerHandler ()
		{
			if (!EventManager.Instance.HandleController)
				return;

			if (btnIdx == -1)
				btnIdx = 0;
			if (btnIdx == buttons.Count)
				btnIdx = buttons.Count - 1;

			float tempAxisH = Input.GetAxisRaw ("Horizontal");
			if (tempAxisH != 0)
			{
				if (DateTime.Now - idxMovedTime > responseTime && string.IsNullOrEmpty (EventManager.Instance.btnIdxOut))
				{
					if (tempAxisH > 0 && btnIdx < buttons.Count - 1)
					{
						btnIdx++;
						idxMovedTime = DateTime.Now;
					}
					if (tempAxisH < 0 && btnIdx > 0)
					{
						btnIdx--;
						idxMovedTime = DateTime.Now;
					}
				}
			}

			float tempAxisV = Input.GetAxisRaw ("Vertical");
			if (tempAxisV != 0)
			{
				if (DateTime.Now - idxMovedTime > responseTime)
				{
					if (tempAxisV > 0)
					{
						if (EventManager.Instance.btnIdxOut == "down")
						{
							EventManager.Instance.btnIdxOut = string.Empty;
							idxMovedTime = DateTime.Now;
						}
						else
						{
							if (btnIdx > 2)
							{
								btnIdx -= 3;
								idxMovedTime = DateTime.Now;
							}
							else
							{
								EventManager.Instance.btnIdxOut = "up";
								idxMovedTime = DateTime.Now;
							}
						}
					}
					if (tempAxisV < 0)
					{
						if (EventManager.Instance.btnIdxOut == "up")
						{
							EventManager.Instance.btnIdxOut = string.Empty;
							btnIdx = 0;
							idxMovedTime = DateTime.Now;
						}
						else
						{
							if (btnIdx < buttons.Count - 3)
							{
								btnIdx += 3;
								idxMovedTime = DateTime.Now;
							}
							else
							{
								EventManager.Instance.btnIdxOut = "down";
								idxMovedTime = DateTime.Now;
							}
						}
					}
				}
			}

			if (string.IsNullOrEmpty (EventManager.Instance.btnIdxOut) && (Input.GetKeyUp (KeyCode.JoystickButton14) || Input.GetKeyUp (KeyCode.Return)))
			{
				if (DateTime.Now - EventManager.Instance.btnPressedTime > responseTime)
				{
					if (buttons [btnIdx].click != null)
					{
						EventManager.Instance.btnPressedTime = DateTime.Now;
						buttons [btnIdx].click ();
					}
				}
			}

			if (Input.GetKeyUp (KeyCode.JoystickButton0) || Input.GetKeyUp (KeyCode.Escape))
			{
				int backIdx = buttons.FindIndex (b => b.text.ToLower () == "back");
				if (backIdx != -1)
				{
					if (DateTime.Now - EventManager.Instance.btnPressedTime > responseTime)
					{
						if (buttons [backIdx].click != null)
						{
							EventManager.Instance.btnPressedTime = DateTime.Now;
							buttons [backIdx].click ();
						}
					}
				}
				else
				{
					if (DateTime.Now - EventManager.Instance.btnPressedTime > responseTime)
					{
						EventManager.Instance.btnPressedTime = DateTime.Now;
						EventManager.Instance.OnExitFromMenu ();
					}
				}
			}
		}
		#endif

		public void Show ()
		{
			if (currentFolderLableStyle == null)
			{
				currentFolderLableStyle = new GUIStyle (GUI.skin.box);
				currentFolderLableStyle.fontSize = (MenuRect.height < MenuRect.width) ? (int)(MenuRect.height / 18) : (int)(MenuRect.width / 18);
				currentFolderLableStyle.alignment = TextAnchor.UpperLeft;
			}
			if (normalButtonStyle == null)
				normalButtonStyle = new GUIStyle (GUI.skin.button);
			if (selectedButtonStyle == null)
			{
				selectedButtonStyle = new GUIStyle (GUI.skin.button);
				selectedButtonStyle.normal.background = selectedButtonStyle.hover.background;
			}

			GUI.Box (MenuRect, "Custom Code Debug Menu\nPlugin: AnalytiAll\nCurrent Position: " + name, currentFolderLableStyle);
			if (buttons == null)
				return;

			#if UNITY_TVOS
			RemoteControllerHandler ();
			#endif

			foreach (var item in buttons) 
			{
				if (GUI.Button (item.rect, item.text, ((btnIdx != -1 && item.text == buttons[btnIdx].text && string.IsNullOrEmpty (EventManager.Instance.btnIdxOut)) ? selectedButtonStyle : normalButtonStyle)))
				{
					if (item.click != null)
						item.click ();
				}
			}
		}
	}
}
