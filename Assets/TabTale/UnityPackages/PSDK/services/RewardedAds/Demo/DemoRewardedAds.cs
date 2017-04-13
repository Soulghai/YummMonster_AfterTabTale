using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

public class DemoRewardedAds : MonoBehaviour {
	
	IPsdkRewardedAds _rewardedAdsService;

	public void Awake () {

		PsdkEventSystem.Instance.onRewardedAdWillShowEvent += adWillShow;
		PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent += adDidClose;
		PsdkEventSystem.Instance.onRewardedAdIsNotReadyEvent += adIsNotReady;
		PsdkEventSystem.Instance.onRewardedAdIsReadyEvent += adIsReady;


		if (PSDKMgr.Instance.Setup()) {

//			Dictionary<string, string> keysDict = new Dictionary<string, string>() 
//			{
//				{ "ultraApplicationKey" , "2f558505"},
//				{ "ultraApplicationUserId" , "test id"},
//				{ "supersonicAdsApplicationKey" , "2aee6109"},
//				{ "supersonicAdsUserId" , "111"},
//				{ "adColonyApplicationId" , "appbdee68ae27024084bb334a"},
//				{ "adColonyZoneId" , "vzf8e4e97704c4445c87504e"},
//				{ "flurryVideoSpace" , "Cheating Tom IOS RV"},
//				{ "vungleApplicationId" , "53de45f423755cf021000027"},
//				{ "unityAdsGameId" , "14050"},
//				{ "unityAdsZoneId" , "14050-1399966087"}
//			};
//

			// Getting psdk AppShelf after psdk start
			_rewardedAdsService = PSDKMgr.Instance.GetRewardedAdsService();
		}
	}


	public void Show() {
		if (null == _rewardedAdsService)
			_rewardedAdsService = PSDKMgr.Instance.GetRewardedAdsService();

		if (null == _rewardedAdsService) {
			Debug.LogError("Null --- rewardedAds service");
			return;
		}

		Debug.Log ("1) Is Rewarded Ad playing before show : " + _rewardedAdsService.IsAdPlaying() );
		if (_rewardedAdsService.IsAdReady()) {
			_rewardedAdsService.ShowAd();
		}
		else {
			Debug.Log ("RewardedAd  not ready !!");
		}
	}


	void OnMouseDown() {
		this.transform.localScale = this.transform.localScale * 0.9f;
		Show();
	}

	void OnMouseUp() {
		if (Input.touchCount > 1) {
			return;
		}
		this.transform.localScale = this.transform.localScale / 0.9f;
	}


	public bool IsAdReady() {
		if (null == _rewardedAdsService) {
			Debug.LogError("Null -- appShelf");
			return false;
		}
		return _rewardedAdsService.IsAdReady();
	}
	
	public bool IsAdPlaying() {
		if (null == _rewardedAdsService) {
			Debug.LogError("Null -- RewardedAds");
			return false;
		}
		return _rewardedAdsService.IsAdPlaying();
	}



	void adIsReady() {
		Debug.Log("DemoRewardedAds::adIsReady !");
	}
	
	void adIsNotReady() {
		Debug.Log("DemoRewardedAds::adIsNotReady !");
	}
	
	void adWillShow() {
		Debug.Log("DemoRewardedAds::adWillShow !");
	}
	
	void adDidClose(bool rewarded) {
		if (rewarded)
			adShouldReward();
		else
			adShouldNotReward();

		Debug.Log("DemoRewardedAds::adDidClose !");
	}
	
	void adShouldReward() {
		Debug.Log("DemoRewardedAds::adShouldReward !");
	}
	
	void adShouldNotReward() {
		Debug.Log("DemoRewardedAds::adShouldNotReward !");
	}




}
