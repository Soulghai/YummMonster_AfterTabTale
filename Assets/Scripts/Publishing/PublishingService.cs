using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;
using TabTale.Plugins.PSDK;

public class PublishingService : MonoBehaviour
{
	#region interface
	public static PublishingService Instance { get; set; }

	public event Action<bool /*isReady*/> OnRewardedVideoReadyChanged;
	public event Action OnSceneTransitionShown;

	public event Action OnEnableMusic;
	public event Action OnDisableMusic;

	public void ShowBanners()
	{
		if (!mAreAdsEnabled)
			return;

		PSDKMgr.Instance.GetBannersService().Show();
	}

	public void HideBanners()
	{
		PSDKMgr.Instance.GetBannersService().Hide();
	}

	public bool IsRewardedVideoReady()
	{
		return PSDKMgr.Instance.GetRewardedAdsService().IsAdReady();
	}

	public void ShowRewardedVideo(Action<bool> resultDelegate)
	{
		var rewardedAdsService = PSDKMgr.Instance.GetRewardedAdsService();
		Assert.IsFalse(rewardedAdsService.IsAdPlaying(), "The game has requested a video while one was already playing. This should never happen.");

		if (rewardedAdsService.IsAdReady())
		{
			FireOnDisableMusicEvent();
			
			mRewardedVideoResultDelegate = resultDelegate;
			rewardedAdsService.ShowAd();
		}
		else
		{
			resultDelegate(false);
		}
	}

	public void ShowSessionStart()
	{
		if (mAreAdsEnabled)
			ShowLocation(SESSION_START);
	}

	public void ShowSceneTransition(Action completionDelegate = null)
	{
		if (mAreAdsEnabled)
		{
			if (PSDKMgr.Instance.GetLocationManagerService().IsLocationReady(SCENE_TRANSITIONS))
			{
				if (OnSceneTransitionShown != null)
					OnSceneTransitionShown();
			}

			ShowLocation(SCENE_TRANSITIONS, completionDelegate);
		}
		else
		{
			if (completionDelegate != null)
				completionDelegate();
		}
	}

	public void ShowAppShelf()
	{
		ShowLocation(MORE_APPS);
	}

	public void ReportPurchase(string price, string currencyCode)
	{
		PSDKMgr.Instance.GetAppsFlyerService().ReportPurchase(price, currencyCode);
	}

	public void DisableAdsPermanently()
	{
		mAreAdsEnabled = false;
		PlayerPrefs.SetInt(DISABLE_ADS_KEY, 1);
		HideBanners();
	}

	#endregion

	#region Life Cycle
	private void Awake()
	{
		if (Instance != null)
		{
			DestroyImmediate(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		Instance = this;
	}

	private void Start()
	{
		if (!PSDKMgr.Instance.Setup())
		{
			Debug.LogError("PSDK failed to initialize.");
			return;
		}

		mAreAdsEnabled = (PlayerPrefs.GetInt(DISABLE_ADS_KEY, 0) == 0);

		PSDKMgr.Instance.GetAppLifecycleManager().SetConfigParams(300, 3600);
		SubscribeToPsdkEvents();
		PSDKMgr.Instance.AppIsReady();
		ShowBanners();
	}

	private void OnDestroy()
	{
		if (Instance != this)
			return;

		UnsubscribeFromPsdkEvents();
	}

	private void SubscribeToPsdkEvents()
	{
		PsdkEventSystem.Instance.onPsdkReady += OnPsdkReady;
		PsdkEventSystem.Instance.onResumeEvent += OnResumeEvent;
		PsdkEventSystem.Instance.onLocationLoadedEvent += OnLocationLoadedEvent;
		PsdkEventSystem.Instance.onLocationFailedEvent += OnLocationFailedEvent;
		PsdkEventSystem.Instance.onClosedEvent += OnClosedEvent;
		PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent += OnRewardedAdDidClosedWithResultEvent;
		PsdkEventSystem.Instance.onRewardedAdIsReadyEvent += OnRewardedAdIsReadyEvent;
		PsdkEventSystem.Instance.onRewardedAdIsNotReadyEvent += OnRewardedAdIsNotReadyEvent;
	}

	private void UnsubscribeFromPsdkEvents()
	{
		if (PsdkEventSystem.Instance == null)
			return;

		PsdkEventSystem.Instance.onPsdkReady -= OnPsdkReady;
		PsdkEventSystem.Instance.onResumeEvent -= OnResumeEvent;
		PsdkEventSystem.Instance.onLocationLoadedEvent -= OnLocationLoadedEvent;
		PsdkEventSystem.Instance.onLocationFailedEvent -= OnLocationFailedEvent;
		PsdkEventSystem.Instance.onClosedEvent -= OnClosedEvent;
		PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent -= OnRewardedAdDidClosedWithResultEvent;
		PsdkEventSystem.Instance.onRewardedAdIsReadyEvent -= OnRewardedAdIsReadyEvent;
		PsdkEventSystem.Instance.onRewardedAdIsNotReadyEvent -= OnRewardedAdIsNotReadyEvent;

	}

	#endregion

	#region Event Handlers
	private void OnPsdkReady()
	{
		ShowSessionStart();
	}

	private void OnResumeEvent(AppLifeCycleResumeState resumeState)
	{
		PSDKMgr.Instance.AppIsReady();
	}

	private void OnLocationLoadedEvent(string location, long attributes)
	{
		if (location == SESSION_START)
			ShowSessionStart();
	}

	void OnLocationFailedEvent(string location, string psdkError)
	{
		if (location == SESSION_START)
			ShowBanners();
	}

	void OnClosedEvent(string location, long attributes)
	{
		FireOnEnableMusicEvent();

		Action completionDelegate = null;
		if (mLocationManagerCompletionDelegates.TryGetValue(location, out completionDelegate))
		{
			mLocationManagerCompletionDelegates.Remove(location);
			if (completionDelegate != null)
				completionDelegate();
		}

		ShowBanners();
	}

	private void OnRewardedAdDidClosedWithResultEvent(bool isSuccess)
	{
		FireOnEnableMusicEvent();
		
		if (mRewardedVideoResultDelegate != null)
			mRewardedVideoResultDelegate(isSuccess);

		mRewardedVideoResultDelegate = null;
	}
	#endregion

	#region Implementation
	private void ShowLocation(string location, Action completionDelegate = null)
	{
		var locationManager = PSDKMgr.Instance.GetLocationManagerService();
		locationManager.ReportLocation(location);
		if (locationManager.IsLocationReady(location))
		{
			FireOnDisableMusicEvent();
			mLocationManagerCompletionDelegates.Add(location, completionDelegate);
			locationManager.Show(location);
		}
		else
		{
			if (completionDelegate != null)
				completionDelegate();
		}
	}

	private void OnRewardedAdIsReadyEvent()
	{
		if (OnRewardedVideoReadyChanged != null)
			OnRewardedVideoReadyChanged(true);
	}

	private void OnRewardedAdIsNotReadyEvent()
	{
		if (OnRewardedVideoReadyChanged != null)
			OnRewardedVideoReadyChanged(false);
	}

	private void FireOnEnableMusicEvent()
	{
		if (OnEnableMusic != null)
			OnEnableMusic();
	}

	private void FireOnDisableMusicEvent()
	{
		if (OnDisableMusic != null)
			OnDisableMusic();
	}

	private Action<bool> mRewardedVideoResultDelegate;
	private Dictionary<string, Action> mLocationManagerCompletionDelegates = new Dictionary<string, Action>();
	private bool mAreAdsEnabled = true;

	private const string SESSION_START = "sessionStart";
	private const string SCENE_TRANSITIONS = "sceneTransitions";
	private const string MORE_APPS = "moreApps";

	private const string DISABLE_ADS_KEY = "publishing_service/are_ads_disabled";
	#endregion
}
