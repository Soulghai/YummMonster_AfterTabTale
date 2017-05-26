﻿using UnityEngine;
using UnityEngine.UI;
using DoozyUI;
using System;

public class ScreenMenu : MonoBehaviour {
	//public static event Action<int> OnAddCoins;
	public GameObject coin;
	public Text timeText;
	public UIButton videoAdsButton;
	public UIButton giftButton;
	public AudioClip sndBtnClick;

	bool isBtnSettingsClicked = false;
	DateTime giftNextDate;
	DateTime _btnVideoNextDate;
	bool isWaitGiftTime;
	bool isShowBtnViveoAds = true;

	// Use this for initialization
	void Start () {
		//Grab the old time from the player prefs as a long
		string _strTime = PlayerPrefs.GetString("dateGiftClicked");
		//_strTime = "";
		//DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER = 0;
		
		_btnVideoNextDate  = System.DateTime.UtcNow;
		if (_strTime == "") {
			giftNextDate = System.DateTime.UtcNow;
			DefsGame.BTN_GIFT_HIDE_DELAY = DefsGame.BTN_GIFT_HIDE_DELAY_ARR [DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER];
			giftNextDate = giftNextDate.AddMinutes (DefsGame.BTN_GIFT_HIDE_DELAY);
			if (DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER < DefsGame.BTN_GIFT_HIDE_DELAY_ARR.Length - 1) {
				++DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER;
				PlayerPrefs.SetInt ("BTN_GIFT_HIDE_DELAY_COUNTER", DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER);
			}
		} else {
			long _timeOld = Convert.ToInt64 (_strTime);
			//Convert the old time from binary to a DataTime variable
			giftNextDate = DateTime.FromBinary(_timeOld);
		}

		DateTime _currentDate = System.DateTime.UtcNow;
		TimeSpan _difference = giftNextDate.Subtract (_currentDate);
		if (_difference.TotalSeconds <= 0f) {
			//timeText.enabled = false;
			isWaitGiftTime = false;
			//giftButton.EnableButtonClicks();
		} else {
			//timeText.enabled = true;
			isWaitGiftTime = true;
			//giftButton.DisableButtonClicks();
			timeText.text = _difference.Hours.ToString () + ":" + _difference.Minutes.ToString ();
		}

		showButtons ();
	}

	void OnEnable()
	{
		Candy.OnTurn += Candy_OnTurn;
		ScreenGame.OnShowMenu += ScreenGame_OnShowMenu;
		MyHeyzap.GiveReward += GiveReward;
	}

	void OnDisable()
	{
		Candy.OnTurn -= Candy_OnTurn;
		ScreenGame.OnShowMenu -= ScreenGame_OnShowMenu;
		MyHeyzap.GiveReward -= GiveReward;
	}

	void ScreenGame_OnShowMenu ()
	{
		showButtons ();
	}

	void Candy_OnTurn ()
	{
		hideButtons ();
	}

	public void showButtons() {
		FlurryEventsManager.SendStartEvent ("start_screen_length");

		//UIManager.ShowUiElement ("MainMenu");
		UIManager.ShowUiElement ("elementBestScore");
		UIManager.ShowUiElement ("elementCoins");
		UIManager.ShowUiElement ("BtnSkins");
		FlurryEventsManager.SendEvent ("candy_shop_impression");
		if (!isWaitGiftTime) {
			UIManager.ShowUiElement ("BtnGift");
			FlurryEventsManager.SendEvent ("collect_prize_impression");
		}
		
		DateTime currentDate = System.DateTime.UtcNow;
		TimeSpan difference = _btnVideoNextDate.Subtract (currentDate);
		if ((difference.TotalSeconds <= 0f))
		if (isShowBtnViveoAds || DefsGame.MyHeyzap.VideoAdCointer == 3)
		{
			_btnVideoNextDate = _btnVideoNextDate.AddMinutes (1);
			isShowBtnViveoAds = true;
			UIManager.ShowUiElement ("BtnVideoAds");
			FlurryEventsManager.SendEvent ("RV_strawberries_impression", "start_screen");
		}
		
		UIManager.ShowUiElement ("BtnMoreGames");
		UIManager.ShowUiElement ("BtnSound");
		UIManager.ShowUiElement ("BtnStar");
		FlurryEventsManager.SendEvent ("rate_us_impression", "start_screen");
		UIManager.ShowUiElement ("BtnLeaderboard");
		UIManager.ShowUiElement ("BtnAchievements");
		#if UNITY_ANDROID || UNITY_EDITOR
		UIManager.ShowUiElement ("BtnGameServices");
		#endif
		UIManager.ShowUiElement ("BtnMoreGames");
		UIManager.ShowUiElement ("BtnShare");
		UIManager.ShowUiElement ("BtnPlus");
		FlurryEventsManager.SendEvent ("iap_shop_impression");
		UIManager.HideUiElement ("scrMenuWowSlider");

		if (DefsGame.screenSkins) {
		if (DefsGame.screenSkins.CheckAvailableSkinBool()) UIManager.ShowUiElement ("BtnHaveNewSkin"); else
			UIManager.HideUiElement ("BtnHaveNewSkin");
		}

		isBtnSettingsClicked = false;
	}

	public void hideButtons() {
		FlurryEventsManager.SendEndEvent ("start_screen_length");
		//UIManager.HideUiElement ("MainMenu");
		UIManager.HideUiElement ("elementBestScore");
		//UIManager.HideUiElement ("elementCoins");
		UIManager.HideUiElement ("BtnSkins");
		UIManager.HideUiElement ("BtnGift");
		UIManager.HideUiElement ("BtnVideoAds");
		UIManager.HideUiElement ("BtnAchievements");
		UIManager.HideUiElement ("BtnMoreGames");
		UIManager.HideUiElement ("BtnSound");
		UIManager.HideUiElement ("BtnStar");
		UIManager.HideUiElement ("BtnLeaderboard");
		UIManager.HideUiElement ("BtnShare");
		UIManager.HideUiElement ("BtnSound");
		UIManager.HideUiElement ("BtnPlus");
		UIManager.HideUiElement ("BtnGameServices");

		UIManager.HideUiElement ("BtnHaveNewSkin");
	}

	public void BtnSettingsClick() {
		isBtnSettingsClicked = !isBtnSettingsClicked;

		if (isBtnSettingsClicked){
			UIManager.ShowUiElement ("BtnSound");
			UIManager.ShowUiElement ("BtnInaps");
			UIManager.ShowUiElement ("BtnGameServices");
		} else {
			UIManager.HideUiElement ("BtnSound");
			UIManager.HideUiElement ("BtnInaps");
			UIManager.HideUiElement ("BtnGameServices");
		}
	}

	public void add10Coins() {
		FlurryEventsManager.SendEvent ("collect_prize");

		for (int i = 0; i < 10; i++) {
			GameObject _coin = (GameObject)Instantiate (coin, Camera.main.ScreenToWorldPoint (giftButton.transform.position), Quaternion.identity); 
			Coin coinScript = _coin.GetComponent<Coin> ();
			coinScript.MoveToEnd();
		}
		//Savee the current system time as a string in the player prefs class
		giftNextDate = DateTime.UtcNow;
		DefsGame.BTN_GIFT_HIDE_DELAY = DefsGame.BTN_GIFT_HIDE_DELAY_ARR [DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER];
		if (DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER < DefsGame.BTN_GIFT_HIDE_DELAY_ARR.Length-1) {
			++DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER;
			PlayerPrefs.SetInt ("BTN_GIFT_HIDE_DELAY_COUNTER", DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER);
		}
		giftNextDate = giftNextDate.AddMinutes (DefsGame.BTN_GIFT_HIDE_DELAY);
		PlayerPrefs.SetString("dateGiftClicked", giftNextDate.ToBinary().ToString());
		UIManager.HideUiElement ("BtnGift");
		//timeText.enabled = true;
		//giftButton.enabled = false;
		isWaitGiftTime = true;
		//giftButton.DisableButtonClicks();
		D.Log ("Disable Button Clicks");
	}

	void Update() {
		if (isWaitGiftTime) {
			DateTime _currentDate = System.DateTime.UtcNow;
			TimeSpan _difference = giftNextDate.Subtract (_currentDate);
			if ((_difference.TotalSeconds <= 0f) &&(DefsGame.currentScreen == DefsGame.SCREEN_GAME)){
				isWaitGiftTime = false;
				UIManager.ShowUiElement ("BtnGift");
				FlurryEventsManager.SendEvent ("collect_prize_impression");
				//timeText.enabled = false;
				//giftButton.enabled = true;
				//giftButton.EnableButtonClicks();
				D.Log ("Enable Button Clicks");
			} else {
				string _minutes = _difference.Minutes.ToString ();
				if (_difference.Minutes < 10) {
					_minutes = "0" + _minutes;
				}
				string _seconds = _difference.Seconds.ToString ();
				if (_difference.Seconds < 10) {
					_seconds = "0" + _seconds;
				}
				timeText.text = _minutes + ":" + _seconds;
			}
		}
	}

	public void OnMoreAppsClicked()
	{
//		PublishingService.Instance.ShowAppShelf();
		FlurryEventsManager.SendEvent ("more_games");
	}

	public void OnVideoAdsClicked()
	{
		FlurryEventsManager.SendEvent ("RV_strawberries", "start_screen");
		DefsGame.MyHeyzap.ShowRewarded();
		isShowBtnViveoAds = false;
		UIManager.HideUiElement ("BtnVideoAds");
	}

	private void GiveReward()
	{
		//	(rewarded video would reset counter / timer to 0).
		for (int i = 0; i < 25; i++) {
			GameObject _coin = (GameObject)Instantiate (coin, Camera.main.ScreenToWorldPoint (videoAdsButton.transform.position), Quaternion.identity);
			Coin coinScript = _coin.GetComponent<Coin> ();
			coinScript.MoveToEnd ();
		}
		FlurryEventsManager.SendEvent ("RV_strawberries_complete", "start_screen", true, 25);
		
		_btnVideoNextDate = System.DateTime.UtcNow;
	}

	public void RateUs() {
		FlurryEventsManager.SendEvent ("rate_us_impression", "start_screen");
		Defs.rate.RateUs ();
		Defs.PlaySound(sndBtnClick, 1f);
	}

	public void Share() {
		FlurryEventsManager.SendEvent ("share");
		if (SystemInfo.deviceModel.Contains ("iPad")) {
			Defs.shareVoxel.ShareClick ();
		} else {
			Defs.share.ShareClick ();
		}
		Defs.PlaySound(sndBtnClick, 1f);
	}

	public void BtnPlusClick() {
		hideButtons ();
		DefsGame.screenCoins.Show ("start_screen");
		Defs.PlaySound(sndBtnClick, 1f);
	}

	public void BtnSkinsClick() {
		FlurryEventsManager.SendEvent ("candy_shop");
		hideButtons ();
		Defs.PlaySound(sndBtnClick, 1f);
	}
}
