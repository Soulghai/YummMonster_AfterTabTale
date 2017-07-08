using UnityEngine;

public class MyAds : MonoBehaviour {
	private static int _videoAdCounter;

	void OnEnable()
	{
		GlobalEvents<OnRewardedVideoAvailable>.Happened += IsRewardedVideoAvailable;
		GlobalEvents<OnRewardedShow>.Happened += OnRewardedShow;
	}

	void OnDisable()
	{
		GlobalEvents<OnRewardedVideoAvailable>.Happened -= IsRewardedVideoAvailable;
		GlobalEvents<OnRewardedShow>.Happened -= OnRewardedShow;
	}
	
	private void IsRewardedVideoAvailable(OnRewardedVideoAvailable e) {
	}
	
	private void OnRewardedShow(OnRewardedShow e) {
		_videoAdCounter = 0;
	}

	public static void ShowVideoAds()
	{
		if (_videoAdCounter >= 4)
		{
			GlobalEvents<OnShowVideoAds>.Call(new OnShowVideoAds());
		}
		else
		{
			++_videoAdCounter;
		}
	}
	
	public static void ShowRewardedAds()
	{
		GlobalEvents<OnShowRewarded>.Call(new OnShowRewarded());
	}
}
