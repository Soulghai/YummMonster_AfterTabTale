using System;
using UnityEngine;
using Tapdaq;

public class MyTapdaq : MonoBehaviour {
	[HideInInspector] public static bool IsRewardedVideoReadyToShow;
	private DateTime rewardVideoDate;
	private bool _isVideoReadyToShow;
	private bool _isWaitTimer = true;

	void Start ()
	{
	    AdManager.Init();
		rewardVideoDate = DateTime.UtcNow;
	}
	
	private void OnEnable() {
		TDCallbacks.TapdaqConfigLoaded += OnTapdaqConfigLoaded;
		TDCallbacks.AdAvailable += OnAdAvailable;
		TDCallbacks.AdNotAvailable += OnAdNotAvailable;
		TDCallbacks.AdClosed += OnAdClosed;
		TDCallbacks.RewardVideoValidated += OnRewardVideoValidated;

		GlobalEvents<OnShowVideoAds>.Happened += ShowVideo;
		GlobalEvents<OnShowRewarded>.Happened += ShowRewarded;
	}

	private void OnDisable() {
		TDCallbacks.TapdaqConfigLoaded -= OnTapdaqConfigLoaded;
		TDCallbacks.AdAvailable -= OnAdAvailable;
		TDCallbacks.AdNotAvailable -= OnAdNotAvailable;
		TDCallbacks.AdClosed -= OnAdClosed;
		TDCallbacks.RewardVideoValidated -= OnRewardVideoValidated;
		
		GlobalEvents<OnShowVideoAds>.Happened -= ShowVideo;
		GlobalEvents<OnShowRewarded>.Happened -= ShowRewarded;
	}

	private void OnTapdaqConfigLoaded() {
		AdManager.LoadInterstitialWithTag("app-launch");
		AdManager.LoadVideoWithTag("video");
		AdManager.LoadRewardedVideoWithTag("rewarded");
//		AdManager.LaunchMediationDebugger ();
	}
	
	private void OnAdAvailable(TDAdEvent e) {
		Debug.Log("OnAdAvailable + " + e.adType  + " " + e.tag);
		GlobalEvents<OnDebugLog>.Call(
        			new OnDebugLog {message = "OnAdAvailable \n" + e.adType  + " " + e.tag + " " + e.message});
		if (e.adType == "INTERSTITIAL" && e.tag == "app-launch")
		{
			AdManager.ShowInterstitial("app-launch");
		} else
		if (e.adType == "VIDEO" && e.tag == "video")
		{
		} else
		if (e.adType == "REWARD_AD" && e.tag == "rewarded")
		{
		}
	}
	
	private void OnAdNotAvailable(TDAdEvent e) {
		Debug.Log("OnAdNotAvailable" + e.adType  + " " + e.tag + " " + e.message);
		GlobalEvents<OnDebugLog>.Call(
			new OnDebugLog {message = "OnAdNotAvailable \n" + e.adType  + " " + e.tag + " " + e.message});
		if (e.adType == "INTERSTITIAL" && e.tag == "app-launch") {
			// Interstitial has failed to load
		} else if (e.adType == "VIDEO" && e.tag == "video") {
			// Video has failed to load
		} else if (e.adType == "REWARD_AD" && e.tag == "rewarded") {
			// Rewarded video has failed to load
		} else if (e.adType == "NATIVE_AD" && e.tag == "ingame") {
			// Native ad has failed to load
		} else if (e.adType == "BANNER") {
			// Banner has failed to load
		}
	}
	
	private void OnAdClosed(TDAdEvent e) {
		Debug.Log("OnAdClosed " + e.adType);
		if (e.adType == "INTERSTITIAL") {
			// Interstitial closed
		} else if (e.adType == "VIDEO") {
			Defs.MuteSounds (false);
		} else if (e.adType == "REWARD_AD") {
			Defs.MuteSounds (false);
		} else if (e.adType == "NATIVE_AD") {
			// Native ad closed
		} else if (e.adType == "BANNER") {
			// Banner closed
		}
	}

	private void ShowVideo(OnShowVideoAds e)
	{
		if (!_isVideoReadyToShow) return;
		if (AdManager.IsVideoReadyWithTag("video"))
		{
			AdManager.ShowVideo("video");
			Defs.MuteSounds(true);
			rewardVideoDate = DateTime.UtcNow;
			rewardVideoDate = rewardVideoDate.AddMinutes(1);
			_isVideoReadyToShow = false;
			_isWaitTimer = true;
//			GameEvents.Send(OnRewardedVideoAvailable, false);
			GlobalEvents<OnRewardedVideoAvailable>.Call(
				new OnRewardedVideoAvailable { isAvailable = false });
			GlobalEvents<OnRewardedShow>.Call(
				new OnRewardedShow());
		}
	}

	private void ShowRewarded(OnShowRewarded e)
	{
		/*if (!AdManager.IsRewardedVideoReadyWithTag("rewarded"))
		{
			NPBinding.UI.ShowAlertDialogWithSingleButton("Ads not available", "Check your Internet connection or try later!", "Ok", (string _buttonPressed) => {});
			return;
		}*/
		GlobalEvents<OnDebugLog>.Call(new OnDebugLog {message = "OnShowRewarded \n"});
		if (AdManager.IsRewardedVideoReadyWithTag("rewarded")) {
			AdManager.ShowRewardVideo("rewarded");
			Defs.MuteSounds (true);
			rewardVideoDate = DateTime.UtcNow;
			rewardVideoDate = rewardVideoDate.AddMinutes(1);
			IsRewardedVideoReadyToShow = false;
			_isWaitTimer = true;
			GlobalEvents<OnRewardedVideoAvailable>.Call(
				new OnRewardedVideoAvailable {isAvailable = false});
		}
	}

	private void UpdateVideoTimerEnd()
	{
		if (IsRewardedVideoReadyToShow && AdManager.IsRewardedVideoReadyWithTag("rewarded")) return;

		if (_isWaitTimer)
		{
			TimeSpan difference = rewardVideoDate.Subtract(DateTime.UtcNow);
			D.Log(difference.TotalSeconds);
			if (difference.TotalSeconds <= 0f)
			{
				_isWaitTimer = false;
				GlobalEvents<OnDebugLog>.Call(new OnDebugLog {message = "_isWaitTimer = false \n"});
			}
		}

		if (!_isWaitTimer)
		{
//			Debug.Log("UpdateVideoTimerEnd - " + IsRewardedVideoReadyToShow);
			if (!IsRewardedVideoReadyToShow && AdManager.IsRewardedVideoReadyWithTag("rewarded"))
			{
				IsRewardedVideoReadyToShow = true;
				GlobalEvents<OnRewardedVideoAvailable>.Call(
					new OnRewardedVideoAvailable {isAvailable = true});
				GlobalEvents<OnDebugLog>.Call(
					new OnDebugLog {message = "OnRewardedReady \n"});
//				Debug.Log("UpdateVideoTimerEnd - REWARDED AVAILABLE");
			}
			
			if (!_isVideoReadyToShow&&AdManager.IsVideoReadyWithTag("video"))
				_isVideoReadyToShow = true;
		}
	}
	
	private void OnRewardVideoValidated(TDVideoReward videoReward) {
//		GameEvents.Send(OnGiveReward, true);
		GlobalEvents<OnGiveReward>.Call(
			new OnGiveReward { isAvailable = true });
		Debug.Log("OnRewardVideoValidated");
//		D.Log("I got " + videoReward.RewardAmount + " of " + videoReward.RewardName);
	}

	void Update()
	{
		UpdateVideoTimerEnd();
	}
}
