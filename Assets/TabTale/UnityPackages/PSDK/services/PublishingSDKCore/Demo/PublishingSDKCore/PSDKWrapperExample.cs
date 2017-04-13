using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TabTale.Plugins.PSDK;

public class PSDKWrapperExample : PsdkSingleton<PSDKWrapperExample> {
		
	IPsdkServiceManager _sm;
	public event System.Action<bool> playMusicEvent;

	#region public


	public void Init() { // here only for instantiation, can be called by Instance.Init().
	}

	public bool PsdkAlreadyStarted {
		get {
			return _psdkAlreadyStarted;
		}
	}

	public string PsdkStartedWithConfig {
		get {
			return _psdkStartedWithConfig;
		}
	}



	void AndroidBackButtonPressed() {
		if (OnBackPressed()) {// internal psdk provider consumed the back button
			return;
		}

		// Back button behaviour should be written here.
		Application.Quit ();
	}

	void Update() {
		#if UNITY_ANDROID
		if (Input.GetKeyDown(KeyCode.Escape)) {
			AndroidBackButtonPressed();
		}
		#endif
	}
	
	public bool StartPsdk() {
		_psdkAlreadyStarted = true;
		bool rc = _sm.Setup();
		if (! rc) {
			Debug.LogError("_sm.Setup failed to start !!!");
		}
		return rc;
	}

	public void ShowLocation(string location) {
		IPsdkLocationManagerService locMgr = _sm.GetLocationManagerService();
		if (locMgr == null) {
			Debug.LogError("LocationMGr not initialized !");
			return;
		}

		long attributes = locMgr.LocationStatus (location);
		if ((attributes & LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC) > 0) {
			StopAppMusic ();
		}


		// Telling the psdk that this location reached.
		locMgr.ReportLocation (location);


		attributes = locMgr.Show(location);
		if (attributes == LocationMgrAttributes.LOCATION_MGR_ATTR_NO_SOURCE) {
			ContinueAppMusic();
		}
	}

	public bool OnBackPressed() {
		return _sm.OnBackPressed();
	}

	public void ShowBanner() {
		IPsdkBanners banner = _sm.GetBannersService ();
		if (banner == null) {
			Debug.LogError ("Banners are not initialized !");
			return;
		}
		Debug.Log ("Showing banner");
		banner.Show ();
	}

	public void HideBanner() {
		IPsdkBanners banner = _sm.GetBannersService ();
		if (banner == null) {
			Debug.LogError ("Banners are not initialized !");
			return;
		}
		Debug.Log ("Hiding banner");
		banner.Hide ();
	}
	


	public bool IsBannersReady() {
		IPsdkBanners ads = _sm.GetBannersService();
		if (ads == null) return false;
		return true;
	}

	public bool IsRewardedAdReady() {
		IPsdkRewardedAds ads = _sm.GetRewardedAdsService();
		if (ads == null) return false;
		if (ads.IsAdReady())
			return true;
		return false;
	}

	public void ShowRewardedAd() {
		IPsdkRewardedAds rwService = _sm.GetRewardedAdsService();
		if (rwService == null) {
			DebugLog("RewardedAd service  not initiated !");
			return;
		}
		if (! rwService.IsAdReady()) {
			DebugLog("RewardedAd not ready !");
			return;
		}
		PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent += OnRewardedAdClosed;
		if (!rwService.ShowAd()) {
			DebugLog("RewardedAd failed to show !");
			PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent  -= OnRewardedAdClosed;
		}
	}
	
	void OnRewardedAdClosed(bool rewarded) {
		PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent  -= OnRewardedAdClosed;

		if (rewarded) 
			DebugLog("Rewarded !");
		else
			DebugLog("Not Rewarded");

	}


	#region GLD
	private IPsdkGameLevelData _gldController;

	public bool IsGLDEnabled() {
		GLDInit();
		if (_gldController != null) return true;
		return false;
	}

	private void GLDInit() {
		if (_gldController != null) return;
		_gldController = _sm.GetGameLevelDataService();
	}

	public IDictionary<string,object>  ReadGld(string gldName)
	{
		GLDInit();
		try
		{

			if (null == _gldController) {
				Debug.LogError("GLD implementation is null.");
				return null;
			}
			
			var plist = _gldController.GetData(gldName);
			if (plist == null)
			{
				Debug.LogError("GLD plist is null.");
				return null;
			}
			DebugLog("GLD File Contents:");
			foreach (var o in plist)
			{
				DebugLog("GLD Key: " + o.Key + ", GLD Value: " + o.Value);
			}
			return plist;
		}
		catch (System.Exception ex)
		{
			DebugLog ("GLD Got exception: " + ex.ToString());
			Debug.LogException(ex);
		}
		
		return null;
	}

	#endregion

	#endregion


	private bool _psdkAlreadyStarted = false;
	private List<string> _loadedLocations = new List<string>();



	protected string _rewardedAdStatus = "";
	protected string _isRewarded = "";
	protected bool _locationReady = false;
	protected string _wantedLocation = "";
	protected bool psdkReady = false;
	private string _psdkStartedWithConfig = "";

	protected override void Awake() {
		base.Awake();
		_sm = PSDKMgr.Instance;
		playMusicEvent += playBgMusic;
		RegisterListners();
		Debug.Log ("PSDKWrpper started !");
	}
	
	protected override void OnDestroy() {
		playMusicEvent -= playBgMusic;
		UnregisterListners();
		base.OnDestroy();
	}

	void playBgMusic(bool play) {
		if (play) {
			// play game music
		} else {
			// pause game music
		}
			// 
	}

	void StopAppMusic() {
		if (playMusicEvent != null) 
			playMusicEvent (false);
	}

	void ContinueAppMusic() {
		if (playMusicEvent != null) 
			playMusicEvent (true);
	}
	
	void RegisterListners() {

		Debug.Log (this.name + " RegisterListners");

		#region location listner  delagates 
		
		PsdkEventSystem.Instance.onLocationLoadedEvent 	+= OnLocationLoaded;
		PsdkEventSystem.Instance.onLocationFailedEvent 	+= OnLocationFailed;
		PsdkEventSystem.Instance.onShownEvent 			+= OnShown;
		PsdkEventSystem.Instance.onClosedEvent 			+= OnClosed;
		PsdkEventSystem.Instance.onConfigurationLoadedEvent += OnConfigurationLoaded;
		
		#endregion		
		
		#region appshelf listner  delagates 
		
		PsdkEventSystem.Instance.onPlaySoundEvent 	+= appShelfOnPlaySound;
		PsdkEventSystem.Instance.onStartAnimationEndedEvent 	+= appShelfOnStartAnimationEnded;
		
		#endregion		
		
		
		
		
		#region RewardedAds listner  delagates 
		
		PsdkEventSystem.Instance.onRewardedAdWillShowEvent += adWillShow;
		PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent += adDidClose;
		PsdkEventSystem.Instance.onRewardedAdIsNotReadyEvent += adIsNotReady;
		PsdkEventSystem.Instance.onRewardedAdIsReadyEvent += adIsReady;

		#endregion		
		
		#region Startup listner  delagates 
		
		PsdkEventSystem.Instance.onConfigurationReady += OnSplashConfigurationLoaded;
		PsdkEventSystem.Instance.onPsdkReady += OnPsdkReady;
		
		#endregion		
		
		#region YouTube listner  delagates 
		
		PsdkEventSystem.Instance.onYouTubeLoad += youTubeOnLoad;
		PsdkEventSystem.Instance.onYouTubeError += youtYubeOnError;
		PsdkEventSystem.Instance.onYouTubeClose += youtYubeOnClose;
		PsdkEventSystem.Instance.onYouTubeImageDownload += youtYubeOnImageDownload;
		
		#endregion		
		
		#region Banners listner  delagates 
		
		PsdkEventSystem.Instance.onBannerConfigurationUpdateEvent += onBannerConfigurationUpdateEvent;
		PsdkEventSystem.Instance.onBannerFailedEvent += onBannerFailedEvent;
		PsdkEventSystem.Instance.onBannerShownEvent += onBannerShownEvent;
		PsdkEventSystem.Instance.onBannerCloseEvent += onBannerCloseEvent;
		PsdkEventSystem.Instance.onBannerWillDisplayEvent += onBannerWillDisplayEvent;

		#endregion		

		#region Splash listner  delagates 
		
		PsdkEventSystem.Instance.onSplashAdded += OnSplashAdded;
		PsdkEventSystem.Instance.onSplashRemoved += OnSplashRemoved;
		
		#endregion		

		PsdkEventSystem.Instance.onResumeEvent+= onPsdkResume;
		
	}

	void UnregisterListners() {

		Debug.Log (this.name + " UnregisterListners");

		#region location listner  delagates 

		if (PsdkEventSystem.Instance != null) {
			PsdkEventSystem.Instance.onLocationLoadedEvent 	-= OnLocationLoaded;
			PsdkEventSystem.Instance.onLocationFailedEvent 	-= OnLocationFailed;
			PsdkEventSystem.Instance.onShownEvent 			-= OnShown;
			PsdkEventSystem.Instance.onClosedEvent 			-= OnClosed;
			PsdkEventSystem.Instance.onConfigurationLoadedEvent -= OnConfigurationLoaded;
		}
		
		#endregion		
		
		#region appshelf listner  delagates 
		
		if (PsdkEventSystem.Instance != null) {
			PsdkEventSystem.Instance.onPlaySoundEvent 	-= appShelfOnPlaySound;
			PsdkEventSystem.Instance.onStartAnimationEndedEvent 	-= appShelfOnStartAnimationEnded;
		}
		#endregion		
		
		
		
		
		#region RewardedAds listner  delagates 

		if (PsdkEventSystem.Instance != null) {
			PsdkEventSystem.Instance.onRewardedAdWillShowEvent -= adWillShow;
			PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent -= adDidClose;
			PsdkEventSystem.Instance.onRewardedAdIsNotReadyEvent -= adIsNotReady;
			PsdkEventSystem.Instance.onRewardedAdIsReadyEvent -= adIsReady;
		}
		#endregion		
		
		#region Stratup listner  delagates 
		
		if (PsdkEventSystem.Instance != null) {
			PsdkEventSystem.Instance.onConfigurationReady -= OnSplashConfigurationLoaded;
			PsdkEventSystem.Instance.onPsdkReady -= OnPsdkReady;
			PsdkEventSystem.Instance.onResumeEvent -= onPsdkResume;
		}
		#endregion		
		
		#region YouTube listner  delagates 
		
		if (PsdkEventSystem.Instance != null) {
			PsdkEventSystem.Instance.onYouTubeLoad -= youTubeOnLoad;
			PsdkEventSystem.Instance.onYouTubeError -= youtYubeOnError;
			PsdkEventSystem.Instance.onYouTubeClose -= youtYubeOnClose;
			PsdkEventSystem.Instance.onYouTubeImageDownload -= youtYubeOnImageDownload;
		}
		#endregion		
		
		#region Banners listner  delagates 
		
		if (PsdkEventSystem.Instance != null) {
			PsdkEventSystem.Instance.onBannerConfigurationUpdateEvent -= onBannerConfigurationUpdateEvent;
			PsdkEventSystem.Instance.onBannerFailedEvent -= onBannerFailedEvent;
			PsdkEventSystem.Instance.onBannerShownEvent -= onBannerShownEvent;
			PsdkEventSystem.Instance.onBannerHiddenEvent -= onBannerHiddenEvent;
		}

		if ( PsdkEventSystem.Instance != null) {
			PsdkEventSystem.Instance.onBannerCloseEvent -= onBannerCloseEvent;
			PsdkEventSystem.Instance.onBannerWillDisplayEvent -= onBannerWillDisplayEvent;
		}

		#endregion		

		#region Splash listner  delagates 
		
		if (PsdkEventSystem.Instance != null) {
			PsdkEventSystem.Instance.onSplashAdded -= OnSplashAdded;
			PsdkEventSystem.Instance.onSplashRemoved -= OnSplashRemoved;
		}
		#endregion		

	}
	
	
	// ============================== delegates ==============================================
	

	void DebugLog(string msg) {
		if (Debug.isDebugBuild) {
			Debug.Log (msg);
		}
	}
	
	
	#region listners
	
	void onPsdkResume(AppLifeCycleResumeState rs) {
		DebugLog("psdk resumed with state: " + rs);
		_sm.AppIsReady ();
	}
	
	void onBannerConfigurationUpdateEvent() {
		DebugLog ("onBannerConfigurationUpdateEvent");
	}
	
	void onBannerFailedEvent() {
		DebugLog ("onBannerFailedEvent");
	}
	
	void onBannerShownEvent() {
		DebugLog ("onBannerShownEvent");
	}

	void onBannerHiddenEvent() {
		DebugLog ("onBannerHiddenEvent");
	}
	
	void onBannerCloseEvent() {
		DebugLog ("onBannerCloseEvent");
	}
	
	void onBannerWillDisplayEvent() {
		DebugLog ("onBannerWillDisplayEvent");
	}
	

	#region RewardedAds listner events
	
	
	void adIsReady() {
		_rewardedAdStatus = "Ready";
		DebugLog("DemoRewardedAds::adIsReady !");
	}
	
	void adIsNotReady() {
		_rewardedAdStatus = "Not Ready";
		DebugLog("DemoRewardedAds::adIsNotReady !");
	}
	
	void adWillShow() {
		_rewardedAdStatus = "Will Show";
		DebugLog("DemoRewardedAds::adWillShow !");
		StopAppMusic ();
	}
	
	void adDidClose(bool rewarded) {
		if (rewarded)
			adShouldReward();
		else
			adShouldNotReward();

		_rewardedAdStatus = "Closed";
		DebugLog("DemoRewardedAds::adDidClose !");
		ContinueAppMusic ();
	}
	
	void adShouldReward() {
		_isRewarded = "Rewarded";
		_rewardedAdStatus = "";
		DebugLog("DemoRewardedAds::adShouldReward !");
	}
	
	void adShouldNotReward() {
		_isRewarded = "Not Rewarded";
		_rewardedAdStatus = "";
		DebugLog("DemoRewardedAds::adShouldNotReward !");
	}
	
	#endregion
	
	
	
	#region Splash listner events
	
	
	void OnSplashConfigurationLoaded() {
		DebugLog("DemoAll::OnSplashConfigurationLoaded !");
	}
	
	void OnPsdkReady() {
		_psdkAlreadyStarted = true;
		DebugLog("psdkReady !");
		PsdkEventSystem.Instance.onPsdkReady -= OnPsdkReady;
		psdkReady = true;
	}
	
	
	#endregion
	
	
	#region location listner events (AppShelf events)
	
	void pauseGameMusic(bool pause) {
		if (pause)
			StopAppMusic ();
		else
			ContinueAppMusic ();
	}

	void OnLocationLoaded(string location, long attributes) {
		if (! _loadedLocations.Contains(location))
			_loadedLocations.Add(location);

		if (_wantedLocation == location) 
			_locationReady = true;

		DebugLog("Location loaded " + location);
	}
	
	void OnLocationFailed(string location, string psdkError) {
		if (_loadedLocations.Contains(location))
			_loadedLocations.Remove(location);

		DebugLog("Location failed  " + location + ", error:" + psdkError);
	}
	
	
	void OnShown(string location, long attributes) {
		DebugLog("Location shown " + location);
	}
	
	
	void OnClosed(string location, long attributes) {
		if ((attributes & LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC) > 0) {
			ContinueAppMusic ();
		}
		DebugLog("Location closed " + location);
	}
	
	
	void OnConfigurationLoaded() {
		DebugLog("location configuration loaded ");
	}
	
	
	#endregion
	
	#region youtube delegates
	
	void youTubeOnLoad() {
		DebugLog("YouTube : OnLoad ");
	}
	
	void youtYubeOnError() {
		DebugLog("YouTube : OnError ");
	}
	
	void youtYubeOnClose() {
		DebugLog("YouTube : OnClose ");
	}
	
	void youtYubeOnImageDownload(string srcPath, string dstPath, bool success) {
		DebugLog("YouTube : OnImageDownload srcPath:" + srcPath +", dstPath:" + dstPath +",success:" + success);
	}
	
	#endregion
	
	#region appshelf delegates

	void appShelfOnPlaySound(AudioClip ac) {
		DebugLog("appShelfOnPlaySound");
		AudioSource.PlayClipAtPoint(ac,Vector3.zero);
	}
	
	void appShelfOnStartAnimationEnded() {
		DebugLog("appShelfOnStartAnimationEnded");
	}

	#endregion

	
	#region Splash delegates
	void OnSplashAdded() {
		DebugLog("OnSplashAdded");
	}

	void OnSplashRemoved() {
		DebugLog("OnSplashRemoved");
	}

	#endregion


	#endregion
	



}
