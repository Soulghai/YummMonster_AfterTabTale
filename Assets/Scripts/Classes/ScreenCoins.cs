using UnityEngine;
using System;
using DoozyUI;

public class ScreenCoins : MonoBehaviour {
	public static event Action<int> OnAddCoinsVisual;

	private bool isShowBtnViveoAds;

	[HideInInspector] public string PrevScreenName;
	private bool _isWaitReward;

	void Start () {
		DefsGame.screenCoins = this;
	}
	
	void OnEnable() {
		GlobalEvents<OnGiveReward>.Happened += GetReward;
		GlobalEvents<OnRewardedVideoAvailable>.Happened += IsRewardedVideoAvailable;
	}

	void OnDisable() {
		GlobalEvents<OnGiveReward>.Happened -= GetReward;
		GlobalEvents<OnRewardedVideoAvailable>.Happened -= IsRewardedVideoAvailable;
	}

	public void ShowButtons() {
		UIManager.ShowUiElement ("scrCoinsBtnBack");
		UIManager.ShowUiElement ("BtnTier1");
		UIManager.ShowUiElement ("BtnTier2");
		if (isShowBtnViveoAds) {
			UIManager.ShowUiElement ("ScreenCoinsBtnVideo");
			FlurryEventsManager.SendEvent ("RV_strawberries_impression", "shop");
		}
		if (DefsGame.noAds < 1) UIManager.ShowUiElement ("ScreenCoinsBtnNoAds");
		#if UNITY_IPHONE
		UIManager.ShowUiElement ("ScreenCoinsBtnRestore");
		#endif
	}

	public void HideButtons() {
		UIManager.HideUiElement ("scrCoinsBtnBack");
		UIManager.HideUiElement ("BtnTier1");
		UIManager.HideUiElement ("BtnTier2");
		UIManager.HideUiElement ("ScreenCoinsBtnVideo");
		UIManager.HideUiElement ("ScreenCoinsBtnNoAds");
		UIManager.HideUiElement ("ScreenCoinsBtnRestore");
	}

	private void IsRewardedVideoAvailable(OnRewardedVideoAvailable e) {
		isShowBtnViveoAds = e.isAvailable;
		if (isShowBtnViveoAds) {
			if (DefsGame.currentScreen == DefsGame.SCREEN_IAPS) {
				UIManager.ShowUiElement ("ScreenCoinsBtnVideo");
				FlurryEventsManager.SendEvent ("RV_strawberries_impression", "shop");
			}
		} else {
			UIManager.HideUiElement ("ScreenCoinsBtnVideo");
		}
	}

	public void BtnTier3() {
		FlurryEventsManager.SendEvent ("RV_strawberries", "shop");
		GlobalEvents<OnRewardedTryShow>.Call(new OnRewardedTryShow()); 
		_isWaitReward = true;
//		if (!PublishingService.Instance.IsRewardedVideoReady())
//		{
//			NPBinding.UI.ShowAlertDialogWithSingleButton("Ads not available", "Check your Internet connection or try later!", "Ok", (string _buttonPressed) => {});
//			return;
//		}
//
//		FlurryEventsManager.SendEndEvent ("iap_shop_length");
//
//		//Defs.MuteSounds (true);
//		PublishingService.Instance.ShowRewardedVideo(isSuccess => {
//			if (isSuccess) {
//				GameEvents.Send(OnAddCoinsVisual, 25);
//				FlurryEventsManager.SendEvent ("RV_strawberries_complete", "shop");
//			}
//			//Defs.MuteSounds (false);
//			FlurryEventsManager.SendStartEvent ("iap_shop_length");
//		});
	}
	
	private void GetReward(OnGiveReward e)
	{
		if (_isWaitReward)
		{
			_isWaitReward = false;
			if (e.isAvailable)
			{
				GameEvents.Send(OnAddCoinsVisual, 25);
//				FlurryEventsManager.SendEvent ("RV_strawberries_complete", "shop");
			}
		}
	}

	public void Show(string prevScreenName) {
		PrevScreenName = prevScreenName;
		FlurryEventsManager.SendStartEvent ("iap_shop_length");
		FlurryEventsManager.SendEvent ("iap_shop", PrevScreenName);

		DefsGame.currentScreen = DefsGame.SCREEN_IAPS;
		DefsGame.isCanPlay = false;
		ShowButtons ();
	}

	public void Hide() {
		FlurryEventsManager.SendEndEvent ("iap_shop_length");

		DefsGame.currentScreen = DefsGame.SCREEN_MENU;
		DefsGame.isCanPlay = true;
		HideButtons ();

		FlurryEventsManager.SendEvent ("iap_shop_home", PrevScreenName);
	}

}