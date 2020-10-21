using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AMEvents.UI
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

			var FBAPI = new MenuFolder (@"FB API", new List<MenuFolder> ());
			FBAPI.click = () => {
				var FB = new SocialAPI.FB ();
				FB.SetBirthday ("10/02/2000");
				FB.SetGender ("male");
				FB.SetMaritalStatus ("single");
				FB.ConfigureFinished ();
			};

			var VKAPI = new MenuFolder (@"VK API", new List<MenuFolder> ());
			VKAPI.click = () => {
				new SocialAPI.VK ().SetBirthday ("20.02.1990").SetGender (1).SetMaritalStatus (6).ConfigureFinished ();
			};

			var socialAPI = new MenuFolder (@"Social API", new List<MenuFolder>{back, 
																				FBAPI, 
																				VKAPI});
			socialAPI.click = () => {
				stackCalls.Push (socialAPI);
			};

			#region banner
			var bannerRemoveListener = new MenuFolder ("Remove\nListener ", new List<MenuFolder>{});
			bannerRemoveListener.click = () => {
				NativeBridge.RemoveAdListener (AMEvents.currentGameObjectName, "banner");
			};

			var bannerAddListener = new MenuFolder ("Add\nListener ", new List<MenuFolder>{});
			bannerAddListener.click = () => {
				NativeBridge.AddAdListener (AMEvents.currentGameObjectName, "banner");
			};

			var getWidthBanner = new MenuFolder ("Get\nWidth", new List<MenuFolder>{});
			getWidthBanner.click = () => {
				Ad.Banner.GetBannerWidth ();
			};

			var getHeightBanner = new MenuFolder ("Get\nHeight", new List<MenuFolder>{});
			getHeightBanner.click = () => {
				Ad.Banner.GetBannerHeight ();
			};

			var showBanner = new MenuFolder ("Show\nBanner", new List<MenuFolder>{});
			showBanner.click = () => {
				Ad.Banner.SetBannerVisible (true);
			};

			var hideBanner = new MenuFolder ("Hide\nBanner", new List<MenuFolder>{});
			hideBanner.click = () => {
				Ad.Banner.SetBannerVisible (false);
			};

			var banner = new MenuFolder ("Banner", new List<MenuFolder>{back, 
																		bannerAddListener, 
																		bannerRemoveListener, 
																		getWidthBanner, 
																		getHeightBanner,
																		showBanner,
																		hideBanner});
			banner.click = () => {
				stackCalls.Push (banner);
			};
			#endregion

			#region intersitial
			var isInterstitialReady = new MenuFolder ("Is Ready", new List<MenuFolder>{});
			isInterstitialReady.click = () => {
				Ad.Interstitial.IsReady ();
			};

			var showInterstitial = new MenuFolder ("Show ", new List<MenuFolder>{});
			showInterstitial.click = () => {
				Ad.Interstitial.Show ();
			};

			var interstitialRemoveListener = new MenuFolder ("Remove\nListener ", new List<MenuFolder>{});
			interstitialRemoveListener.click = () => {
				NativeBridge.RemoveAdListener (AMEvents.currentGameObjectName, "interstitial");
			};
			
			var interstitialAddListener = new MenuFolder ("Add\nListener ", new List<MenuFolder>{});
			interstitialAddListener.click = () => {
				NativeBridge.AddAdListener (AMEvents.currentGameObjectName, "interstitial");
			};

			var timeAfterInterstitial = new MenuFolder ("Time After\nInterstitial ", new List<MenuFolder>{});
			timeAfterInterstitial.click = () => {
				Ad.Interstitial.TimeAfterInterstitial ();
			};

			var interstitial = new MenuFolder ("Interstitial", new List<MenuFolder>{back, 
																					interstitialAddListener, 
																					interstitialRemoveListener, 
																					isInterstitialReady,
																					showInterstitial,
																					timeAfterInterstitial});

			interstitial.click = () => {
				stackCalls.Push (interstitial);
			};
			#endregion

			#region rewarded
			var rewardedRemoveListener = new MenuFolder ("Remove\nListener ", new List<MenuFolder>{});
			rewardedRemoveListener.click = () => {
				NativeBridge.RemoveAdListener (AMEvents.currentGameObjectName, "rewardedVideo");
			};
			
			var rewardedAddListener = new MenuFolder ("Add\nListener ", new List<MenuFolder>{});
			rewardedAddListener.click = () => {
				NativeBridge.AddAdListener (AMEvents.currentGameObjectName, "rewardedVideo");
			};

			var showRewarded = new MenuFolder ("Show ", new List<MenuFolder>{});
			showRewarded.click = () => {
				bool isReady = Ad.RewardedVideo.IsReady ();
				if (isReady)
				{
					Ad.RewardedVideo.Show ();
				}
			};

			var isLoadedRewarded = new MenuFolder ("Is Ready", new List<MenuFolder>{});
			isLoadedRewarded.click = () => {
				Ad.RewardedVideo.IsReady ();
			};
			
			var rewarded = new MenuFolder ("Rewarded", new List<MenuFolder>{back, 
																			rewardedAddListener, 
																			rewardedRemoveListener, 
																			showRewarded, 
																			isLoadedRewarded});
			rewarded.click = () => {
				stackCalls.Push (rewarded);
			};
			#endregion

			#region cross
			var enableCross = new MenuFolder ("Enable\nCross", new List<MenuFolder>{});
			enableCross.click = () => {
				Ad.EnableCross ();
			};
			#endregion

			var disableAd = new MenuFolder ("Disable\nAds", new List<MenuFolder>{});
			disableAd.click = () => {
				if (!Ad.Banner.IsAdDisabled ())
				{
					Ad.Banner.DisableAd ();
				}
				else
				{
					AMEvents.amLogger.Log ("Ad is already disabled");
				}
			};

			#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
			var enableAd = new MenuFolder ("Enable\nAds", new List<MenuFolder>{});
			enableAd.click = () => {
				if (Ad.Banner.IsAdDisabled ())
				{
					Ad.Banner.EnableAd ();
				}
				else
				{
					AMEvents.amLogger.Log ("Ad is already enabled");
				}
			};
			#endif

			#region inner inapp
			var innerInAppRemoveListener = new MenuFolder ("Remove\nListener ", new List<MenuFolder>{});
			innerInAppRemoveListener.click = () => {
				NativeBridge.RemoveAdListener (AMEvents.currentGameObjectName, "innerInApp");
			};

			var innerInAppAddListener = new MenuFolder ("Add\nListener ", new List<MenuFolder>{});
			innerInAppAddListener.click = () => {
				NativeBridge.AddAdListener (AMEvents.currentGameObjectName, "innerInApp");
			};

			var showInnerInapp = new MenuFolder ("Show", new List<MenuFolder>{});
			showInnerInapp.click = () => {
				Ad.InnerInApp.Show ();
			};

			var buyInnerInapp = new MenuFolder ("Buy", new List<MenuFolder>{});
			buyInnerInapp.click = () => {
				Ad.InnerInApp.Buy ();
			};

			var restoreInnerInapp = new MenuFolder ("Restore", new List<MenuFolder>{});
			restoreInnerInapp.click = () => {
				Ad.InnerInApp.Restore ();
			};

			var innerInApp = new MenuFolder ("Inner InApp", new List<MenuFolder>{back,
																				innerInAppAddListener,
																				innerInAppRemoveListener,
																				showInnerInapp,
																				buyInnerInapp,
																				restoreInnerInapp});
			innerInApp.click = () => {
				stackCalls.Push (innerInApp);
			};
			#endregion

			var ADAPI = new MenuFolder (@"AD API", new List<MenuFolder>{back, 
																		disableAd,
																		#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
																		enableAd,
																		#endif
																		enableCross, 
																		banner, 
																		interstitial, 
																		rewarded,
																		innerInApp});
			ADAPI.click = () => {
				stackCalls.Push (ADAPI);
			};

			mainMenuFolder = new MenuFolder ("Main", new List<MenuFolder> {socialAPI, ADAPI});
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
		public string name;
		public Action click;
		public List<MenuFolder> menuFolders;
		List<GUIButton> buttons;
		Rect MenuRect;

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
			if (!AMEvents.Instance.HandleController)
				return;

			if (btnIdx == -1)
				btnIdx = 0;
			if (btnIdx == buttons.Count)
				btnIdx = buttons.Count - 1;

			float tempAxisH = Input.GetAxisRaw ("Horizontal");
			if (tempAxisH != 0)
			{
				if (DateTime.Now - idxMovedTime > responseTime && string.IsNullOrEmpty (AMEvents.Instance.btnIdxOut))
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
						if (AMEvents.Instance.btnIdxOut == "down")
						{
							AMEvents.Instance.btnIdxOut = string.Empty;
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
								AMEvents.Instance.btnIdxOut = "up";
								idxMovedTime = DateTime.Now;
							}
						}
					}
					if (tempAxisV < 0)
					{
						if (AMEvents.Instance.btnIdxOut == "up")
						{
							AMEvents.Instance.btnIdxOut = string.Empty;
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
								AMEvents.Instance.btnIdxOut = "down";
								idxMovedTime = DateTime.Now;
							}
						}
					}
				}
			}

			if (string.IsNullOrEmpty (AMEvents.Instance.btnIdxOut) && (Input.GetKeyUp (KeyCode.JoystickButton14) || Input.GetKeyUp (KeyCode.Return)))
			{
				if (DateTime.Now - AMEvents.Instance.btnPressedTime > responseTime)
				{
					if (buttons [btnIdx].click != null)
					{
						AMEvents.Instance.btnPressedTime = DateTime.Now;
						buttons [btnIdx].click ();
					}
				}
			}

			if (Input.GetKeyUp (KeyCode.JoystickButton0) || Input.GetKeyUp (KeyCode.Escape))
			{
				int backIdx = buttons.FindIndex (b => b.text.ToLower () == "back");
				if (backIdx != -1)
				{
					if (DateTime.Now - AMEvents.Instance.btnPressedTime > responseTime)
					{
						if (buttons [backIdx].click != null)
						{
							AMEvents.Instance.btnPressedTime = DateTime.Now;
							buttons [backIdx].click ();
						}
					}
				}
				else
				{
					if (DateTime.Now - AMEvents.Instance.btnPressedTime > responseTime)
					{
						AMEvents.Instance.btnPressedTime = DateTime.Now;
						AMEvents.Instance.OnExitFromMenu ();
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

			GUI.Box (MenuRect, "Custom Code Debug Menu\nPlugin: AM Events\nCurrent Position: " + name, currentFolderLableStyle);
			if (buttons == null)
				return;

			#if UNITY_TVOS
			RemoteControllerHandler ();
			#endif

			foreach (var item in buttons) 
			{
				if (GUI.Button (item.rect, item.text, ((btnIdx != -1 && item.text == buttons[btnIdx].text && string.IsNullOrEmpty (AMEvents.Instance.btnIdxOut)) ? selectedButtonStyle : normalButtonStyle)))
				{
					if (item.click != null)
						item.click ();
				}
			}
		}
	}
}
