using UnityEngine;
using Tapdaq;

public class MyTapdaq : MonoBehaviour {
	private bool _isShowStartInterstitial;

	void Start ()
	{
	    AdManager.Init();
	}
	
	private void OnEnable() {
		TDCallbacks.TapdaqConfigLoaded += OnTapdaqConfigLoaded;
		TDCallbacks.AdAvailable += OnAdAvailable;
		TDCallbacks.AdNotAvailable += OnAdNotAvailable;
		TDCallbacks.AdClosed += OnAdClosed;
		TDCallbacks.RewardVideoValidated += OnRewardVideoValidated;

		GlobalEvents<OnShowVideoAds>.Happened += ShowVideo;
		GlobalEvents<OnRewardedShow>.Happened += ShowRewarded;
	}

	private void OnTapdaqConfigLoaded() {
		AdManager.LoadInterstitial("app-launch");
		AdManager.LoadVideo("video");
		AdManager.LoadRewardedVideo("rewarded");
		AdManager.RequestBanner(TDMBannerSize.TDMBannerStandard);
		AdManager.LaunchMediationDebugger ();
	}
	
	private void OnAdAvailable(TDAdEvent e) {
		Debug.Log("OnAdAvailable + " + e.adType  + " " + e.tag);
		GlobalEvents<OnDebugLog>.Call(
        			new OnDebugLog {message = "OnAdAvailable \n" + e.adType  + " " + e.tag + " " + e.message});
		if (e.adType == "BANNER")
		{
			AdManager.ShowBanner(TDBannerPosition.Bottom);
		} else
		if (e.adType == "INTERSTITIAL" && e.tag == "app-launch")
		{
			
			if (!_isShowStartInterstitial && DefsGame.QUEST_GAMEPLAY_Counter > 1)
			{
				#if UNITY_IPHONE
					AdManager.ShowInterstitial("app-launch");
				#endif
				_isShowStartInterstitial = true;
			}
		} else
		if (e.adType == "VIDEO" && e.tag == "video")
		{
		} else
		if (e.adType == "REWARD_AD" && e.tag == "rewarded")
		{
			GlobalEvents<OnRewardedLoaded>.Call(
				new OnRewardedLoaded {IsAvailable = true});
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
			GlobalEvents<OnRewardedLoaded>.Call(
				new OnRewardedLoaded {IsAvailable = false});
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
		Debug.Log("ShowVideo(OnShowVideoAds e)");
		if (AdManager.IsVideoReady("video"))
		{
			AdManager.ShowVideo("video");
			Defs.MuteSounds(true);
			GlobalEvents<OnAdsVideoShowing>.Call(new OnAdsVideoShowing());
			Debug.Log("Show ADS_VIDEO");
		}
	}

	private void ShowRewarded(OnRewardedShow e)
	{
		if (AdManager.IsRewardedVideoReady("rewarded")) {
			AdManager.ShowRewardVideo("rewarded");
			Defs.MuteSounds (true);
			
			GlobalEvents<OnAdsRewardedShowing>.Call(new OnAdsRewardedShowing());
		}
	}
	
	private void OnRewardVideoValidated(TDVideoReward videoReward) {
		GlobalEvents<OnGiveReward>.Call(new OnGiveReward { IsAvailable = true });
	}
}
