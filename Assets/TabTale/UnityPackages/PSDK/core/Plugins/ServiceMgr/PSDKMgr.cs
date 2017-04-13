using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;
using System.Reflection;
using System.Runtime.InteropServices;

//using Json = TabTale.Plugins.PSDK.PSDKMiniJSON;

namespace TabTale.Plugins.PSDK {
	public class PSDKMgr : IPsdkServiceManager {

		[System.Obsolete("onResumeEvent is deprecated, please use onResumeEvent of PsdkEventSystem instead.", true)]	
		public event System.Action<AppLifeCycleResumeState> onResumeEvent;


		private IPsdkServiceManager _impl;

//		private IPsdkAppShelf 				_appShelfService;
		private IPsdkGameLevelData 			_gldService;
		private IPsdkAppLifeCycleManager	_appLifeCycleMgrService;
		private IPsdkLocationManagerService	_locationsMgrService;
		private IPsdkRewardedAds 			_rewardedAdsService;
		private IPsdkSplash 				_splashService;
		private IPsdkYouTube 				_youTubeService;
		private IPsdkBanners 				_bannersService;
		private IPsdkAppsFlyer 				_appsFlyerService;
		private IPsdkAnalytics 				_analyticsService;
		private IPsdkCrashTool 				_crashTool;
		private IPsdkExternalConfiguration  _externalConfiguration;
		private IPsdkAudience				_audience;

		private List<IPsdkService> _registeredServices = new List<IPsdkService>();

		private string _configJson;
		private bool _debugMode = false;
		private bool _psdkSetup = false;
		private bool _nativePsdkStarted = false;
        private bool _silent = true;

		protected PSDKMgr() {
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkServiceMgr(this); break;
			#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkServiceMgr(this); break;
			#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor: 	_impl = new UnityEditorPsdkServiceMgr(this); break;
			default: throw new System.Exception("Platform not supported for PSDKMgr.");
			}
		}

		private static object _lock = new object();
		static PSDKMgr _instance = null;

		public static PSDKMgr Instance {
			get
			{
				if (_instance != null)
					return _instance;
			
				lock(_lock)
				{
					if (_instance == null)
					{
						_instance = new PSDKMgr();
					}
					
					return _instance;
				}
			}
		}

#region setup services
		/// <summary>
		/// <summary>_analyticsService
		/// Setup the specified store, domain, language and orientation.
		/// </summary>
		/// <param name="store">Store.</param>
		/// <param name="domain">Domain.</param>
		/// <param name="language">Language.</param>
		/// <param name="orientation">Orientation.</param>
		public bool Setup(string language = null, string configJson = null) {

			if (_psdkSetup) return true;
			_psdkSetup = true;

			PsdkEventSystem.Instance.Init ();
			
			_configJson = configJson;

			LogPsdkVersions();

			if (System.String.IsNullOrEmpty(_configJson)) {
				Debug.Log ("Reading psdk json configuration from steamingAssets/psdk.json file !");
				_configJson = PsdkUtils.ReadPsdkConfigFromFile();
			}

			if (System.String.IsNullOrEmpty(_configJson)) {
				Debug.LogError ("Null or empty psdk configuration json, please put it in Assets/SteamingAssets/psdk.json !");
				return false;
			}

			PsdkUtils.NativeLog("psdk config: " + _configJson);

			DebugMode = isDebugMode (_configJson);

			SetupAppLifecycleManager();
			// initial analytics listner
			
			PsdkEventSystem.Instance.Init ();

			if(!SetupAudience()){
				Debug.Log ("PSDK Audience not initialized!");
			}

			if(!SetupExternalConfiguration()){
				Debug.Log ("PSDK External Configuration not initialized!");
			}

			if (! SetupAnalyticsService()) 
				Debug.Log ("PSDK Analytics not inititalized  !");

			if (! SetupLocationManagerService())
				Debug.Log ("location manager not inititalized  !");

			if (! SetupRewardedAdsService())
					Debug.Log ("RewardedAds not initialized !");

			if (! SetupGameLevelData())
				Debug.Log ("GLD not initialized !");
			
			if (! SetupYouTube())
				Debug.Log ("YouTube not initialized !");

			
			if (! SetupSplash())
				Debug.Log ("Splash not initialized !");
			
			if (! SetupBanners())
				Debug.Log ("Banners not initialized !");
                
             if (! SetupCrashMonitoringTool()) 
                Debug.Log ("CrashMonitoringTool not initialized !");
			

//			string newConfigJson = preProcessConfigJsonAccordingToInstalledPkgs(_configJson);

			bool rc = false;
			if (null != _impl) {
				rc = _impl.Setup (_configJson, language);
				_nativePsdkStarted = true;
                _silent = false;
				PsdkEventSystem.RegisterNativeCallbacksAfterPsdkInitialization();
			}

			if (rc) {
				AppLifeCycleResumeState or = _appLifeCycleMgrService.OnResume();
				Debug.Log("PSDKMgr::Start resume state: " + or.ToString());
				
				foreach(IPsdkService ps in _registeredServices) {
					try {
						ps.psdkStartedEvent();
					}
					catch (System.Exception e) {
						Debug.LogException(e);
					}
				}
				if (or != AppLifeCycleResumeState.ALCRS_NONE) {
					PsdkEventSystem.Instance.SendMessage("OnResumeEvent",or);
				}

			}

			return rc;
		}

		public bool DebugMode {
			get { return _debugMode;}
			set { _debugMode = value; }
		}

		public void AppIsReady() {
			IPsdkAppLifeCycleManager alm = GetAppLifecycleManager();
			if ( alm != null) {
				alm.AppIsReady();
			}
		}

		public void PurchaseAd() {
			_impl.PurchaseAd ();
		}

		public void ReportLevel(int level){
			_impl.ReportLevel (level);
		}
		

		#region Unity events

		public bool NativePsdkStarted {
			get {
				return _nativePsdkStarted;
			}
		}

	public bool OnBackPressed() {
			IPsdkAppLifeCycleManager alm = GetAppLifecycleManager();
			if ( alm != null) {
				return alm.OnBackPressed();
			}
			return false;
		}




		#endregion

		public bool SetupAudience()
		{
			registerInternalService (_audience = GetAudience ());
			if(_audience != null){
				return _audience.Setup ();
			}
			return false;
		}

		public bool SetupExternalConfiguration()
		{
			registerInternalService (_externalConfiguration = GetExternalConfiguration ());
			if(_externalConfiguration != null){
				return _externalConfiguration.Setup ();
			}
			return false;
		}

		/// <summary>
		/// Setups the app lifecycle manager.
		/// </summary>
		/// <returns><c>true</c>, if app lifecycle manager was setuped, <c>false</c> otherwise.</returns>
		public bool  	SetupAppLifecycleManager() {
			
			registerInternalService(_appLifeCycleMgrService= GetAppLifecycleManager());
			if (null != _appLifeCycleMgrService)
				return _appLifeCycleMgrService.Setup();
			
			return false;
		}


		/// <summary>
		/// Setups the location manager service.
		/// </summary>
		/// <returns><c>true</c>, if location manager service was setuped, <c>false</c> otherwise.</returns>
		/// <param name="defaultDomain">Default domain.</param>
		/// <param name="serverDomain">Server domain.</param>
		public bool  	SetupLocationManagerService() {
			registerInternalService(_locationsMgrService= GetLocationManagerService());
			if (null != _locationsMgrService) {
				PsdkEventSystem.Instance.SetAnalyticsDelegate(_analyticsService);
				return _locationsMgrService.Setup ();
			}
			
			return false;
		}

		public bool SetupAnalyticsService() {
			registerInternalService(_analyticsService= GetAnalyticsService());
			if (null != _analyticsService)
				return true;
			
			return false;
		}

		/// <summary>
		/// Setups the game level data.
		/// </summary>
		/// <returns><c>true</c>, if game level data was setuped, <c>false</c> otherwise.</returns>
		/// <param name="domain">Domain.</param>
		/// <param name="localDir">Local dir.</param>
		/// <param name="timeout">Timeout.</param>
		public bool 	SetupGameLevelData() {

			registerInternalService(_gldService= GetGameLevelDataService());
			if (null != _gldService)
				return _gldService.SetupGameLevelData();
			
			return false;
		}

		public bool  SetupRewardedAdsService() {
			registerInternalService(_rewardedAdsService= GetRewardedAdsService());
			if (null != _rewardedAdsService)
				return _rewardedAdsService.Setup();
			
			return false;
		}

#if UNITY_ANDROID
		public AndroidJavaClass JavaClass {
			get {return null;}
		}
#endif

		#endregion setup services



			#region get methods
			/// <summary>
			/// Gets the implementation.
			/// </summary>
			/// <returns>The implementation.</returns>
			public IPsdkServiceManager 		GetImplementation() {
				return _impl;
			}

		public IPsdkAudience GetAudience() {
			if(!isPsdkValid()){
				return null;
			}
			if(_audience != null){
				return _audience;
			}
			_audience = GettingServiceByReflection<IPsdkAudience> ("PsdkAudience");
			if(_audience != null){
				return _audience;
			}
			if(!_silent){
				Debug.Log("Could not initiate PsdkAudience !");
			}

			return null;
		}

		public IPsdkExternalConfiguration GetExternalConfiguration()
		{
			if(!isPsdkValid()){
				return null;
			}

			if(_externalConfiguration != null){
				return _externalConfiguration;
			}

			_externalConfiguration = GettingServiceByReflection<IPsdkExternalConfiguration> ("PsdkExternalConfiguration");

			if(_externalConfiguration != null){
				return _externalConfiguration;
			}

			if(!_silent){
				Debug.Log("Please install PsdkExternalConfiguration !");
			}

			return null;
		}

		/// <summary>
		/// Gets the app lifecycle manager.
		/// </summary>
		/// <returns>The app lifecycle manager.</returns>
		public IPsdkAppLifeCycleManager  	GetAppLifecycleManager() {

			if (! isPsdkValid()) 
                return null;

			if (null != _appLifeCycleMgrService)
				return _appLifeCycleMgrService;

			_appLifeCycleMgrService = GettingServiceByReflection<IPsdkAppLifeCycleManager>("PsdkAppLifeCycleManager");

			if (null != _appLifeCycleMgrService)
				return _appLifeCycleMgrService;

            if (! _silent)
			    Debug.Log("Please install PsdkAppLifeCycleManager !");
			return null;
		}

		/// <summary>
		/// Gets the location manager service.
		/// </summary>
		/// <returns>The location manager service.</returns>
		public IPsdkLocationManagerService GetLocationManagerService(){
			
			if (! isPsdkValid()) return null;

			if (null != _locationsMgrService)
				return _locationsMgrService;

			_locationsMgrService = GettingServiceByReflection<IPsdkLocationManagerService>( "PsdkLocationManagerService");
			
			if (null != _locationsMgrService)
				return _locationsMgrService;
			
            if (! _silent)
			    Debug.Log("Please import Monetization.unitypackage !");
			return null;
		}

		/// <summary>
		/// Gets the game level data service.
		/// </summary>
		/// <returns>The game level data service.</returns>
		public IPsdkGameLevelData 					GetGameLevelDataService(){
			
            if (! isPsdkValid())
                return null;
            
			if (null != _gldService)
				return _gldService;
			
			_gldService = GettingServiceByReflection<IPsdkGameLevelData>("PsdkGameLevelDataService");
			
			if (null != _gldService)
				return _gldService;

            if (! _silent)
			    UnityEngine.Debug.LogError("Please import GameLevelData.unitypackage !");
			return null;
		}

		public IPsdkRewardedAds 					GetRewardedAdsService(){
			
			if (! isPsdkValid()) return null;

			if (null != _rewardedAdsService)
				return _rewardedAdsService;
			
			_rewardedAdsService = GettingServiceByReflection<IPsdkRewardedAds>("PsdkRewardedAds");
			
			if (null != _rewardedAdsService)
				return _rewardedAdsService;
			
           if (! _silent)
			    Debug.Log("Please import PSDKRewardedAds.unitypackage !");
			return null;
		}

		public bool SetupSplash() {
			registerInternalService(_splashService = GetSplashService());
			if (null != _splashService) 
				return _splashService.SetupSplash();

			return false;
		}


		public IPsdkSplash 					GetSplashService(){
			
			if (! isPsdkValid()) return null;

			if (null != _splashService)
				return _splashService;
			
			_splashService = GettingServiceByReflection<IPsdkSplash>("PsdkSplashService");
			
			if (null != _splashService)
				return _splashService;
			
            if (! _silent)
			    Debug.Log("Please import Splash.unitypackage before SetupSplash !");
			return null;
		}

		public bool SetupYouTube() {
			registerInternalService(_youTubeService = GetYouTubeService());
			if (null != _youTubeService) 
				return _youTubeService.Setup();
			
			return false;
		}
		
		public IPsdkYouTube 					GetYouTubeService(){
			
			if (! isPsdkValid()) return null;

			if (null != _youTubeService)
				return _youTubeService;
			
			_youTubeService = GettingServiceByReflection<IPsdkYouTube>("PsdkYouTubeService");
			
			if (null != _youTubeService)
				return _youTubeService;
			
            if (! _silent)
			    Debug.Log("Please import PSDKYouTube.unitypackage before SetupYouTube !");
			return null;
		}

		public bool SetupBanners() {
			registerInternalService(_bannersService = GetBannersService());
			if (null != _bannersService) 
				return _bannersService.Setup();
			
			return false;
		}
		
		public IPsdkBanners 					GetBannersService(){
			
			if (! isPsdkValid()) return null;
			
			if (null != _bannersService)
				return _bannersService;
			
			_bannersService = GettingServiceByReflection<IPsdkBanners>("PsdkBannersService");
			
			if (null != _bannersService)
				return _bannersService;
			
			if (! _silent)
                Debug.Log("Please import PSDKBanners.unitypackage before Banners setup !");
			return null;
		}

		public IPsdkAppsFlyer 					GetAppsFlyerService(){
			if (! _psdkSetup) return null;
			
			if (null != _appsFlyerService)
				return _appsFlyerService;
			
			_appsFlyerService = GettingServiceByReflection<IPsdkAppsFlyer>("PsdkAppsFlyerService");
			
			if (null != _appsFlyerService)
				return _appsFlyerService;

			if (! _silent)
				Debug.Log("Please import PSDKAppsFlyer.unitypackage to use AppsFlyer services !");
			return null;
		}

		public IPsdkAnalytics 					GetAnalyticsService(){
			if (! _psdkSetup) return null;
			
			if (null != _analyticsService)
				return _analyticsService;
			
			_analyticsService = GettingServiceByReflection<IPsdkAnalytics>("PsdkAnalyticsService");
			
			if (null != _analyticsService)
				return _analyticsService;

		    if (! _silent)
				Debug.Log("Please import PSDKAnalytics.unitypackage to use Analytics services !");
			return null;
		}

		public IPsdkCrashTool 					GetCrashMonitoringToolService(){
			if (! _psdkSetup) return null;
			
			if (null != _crashTool)
				return _crashTool;
			
			_crashTool = GettingServiceByReflection<IPsdkCrashTool>("PsdkCrashToolService");
			
			if (null != _crashTool)
				return _crashTool;
			
			if (! _silent)
			    Debug.Log("Please import PSDKCrashTool.unitypackage to use CrashMonitoringTool services !");
			return null;
		}
		
		public bool SetupCrashMonitoringTool() {
			registerInternalService(_crashTool = GetCrashMonitoringToolService());
			if (null != _crashTool) 
				return true;
			return false;
		}


		public string GetAppID() {

			if (! NativePsdkStarted) 
				return  null;

			return _impl.GetAppID();
		}

		public string BundleIdentifier {
			get {
				return PsdkUtils.BundleIdentifier;
			}
		}

		public string ConfigJson {
			get { return _configJson; }
		}
		
		/// <summary>
		/// Gettings the service by reflection.
		/// </summary>
		/// <returns>The service by reflection.</returns>
		/// <param name="serviceName">Service name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private T GettingServiceByReflection<T>(string serviceName) {
			System.Type psdkMgrType = this.GetType();
			if (psdkMgrType == null) {
				Debug.LogError("psdkMgrType NULL !!");
				return default(T);
			}
//			System.Type serviceType = Types.GetType("TabTale.Plugins.PSDK." + serviceName,psdkMgrType.Assembly.ToString());
			System.Type serviceType = System.Type.GetType("TabTale.Plugins.PSDK." + serviceName);
//						Debug.Log("PSDKMgr::GettingServiceByReflection: " + serviceName + " type " + serviceType + " ! assembly:" + psdkMgrType.Assembly.ToString());
//			if (serviceType == null) {
//				System.Type[] services = getTypeByName("TabTale.Plugins.PSDK." + serviceName);
//				if (services.Length > 0)
//					serviceType = services[0];
//			} 
			if (serviceType == null) {
				Debug.Log("PSDKMgr::GettingServiceByReflection: " + serviceName + " type NULL ! assembly:" + psdkMgrType.Assembly.ToString());
				return default(T);
			} 
			// try instantiating the class by reflection;
			System.Type[] types = new System.Type[1];
//			types[0] = Types.GetType("TabTale.Plugins.PSDK.IPsdkServiceManager", psdkMgrType.Assembly.ToString());
			types[0] = System.Type.GetType("TabTale.Plugins.PSDK.IPsdkServiceManager");
			ConstructorInfo ctor = serviceType.GetConstructor(types);
			if(ctor != null)
			{
				object service = ctor.Invoke(new object[] { this });
				if (service is T) {
					return (T)service;
				} 
				else 
				{
					try {
						return (T)System.Convert.ChangeType(service, typeof(T));
					} catch (System.InvalidCastException) {
						return default(T);
					}
				}
			}

			return default(T);
		}


		private void registerInternalService(IPsdkService ps) {
			if (null != ps) 
				_registeredServices.Add(ps);
		}

#endregion get methods


		/// <summary>
		/// Gets the store.
		/// </summary>
		/// <returns>The store.</returns>
		public string GetStore() {
			return _impl.GetStore();
		}

		public void SetLanguage(string language) {
			_impl.SetLanguage(language);
		}

		private void LogPsdkVersions() {
			try {
				PsdkUtils.NativeLog (PsdkSerializedData.Instance.ToString());
 			}
			catch (System.Exception e) {
				Debug.LogException(e);
			}
		}


		private string preProcessConfigJsonAccordingToInstalledPkgs(string configJson) {
			if (configJson == null)
				return null;

			try {
				Dictionary<string,object> psdkConfig = TabTale.Plugins.PSDK.Json.Deserialize (configJson) as Dictionary<string,object>;
				if (psdkConfig == null) {
					Debug.LogError("PSDKMgr::sisn't manage to read config json !");
					return null;
				}

				foreach (string key in psdkConfig.Keys) {
					Dictionary<string,object> psdkContent = psdkConfig[key] as Dictionary<string,object>;
					if (psdkContent == null) continue;
					object obj;
					if (psdkContent.TryGetValue("enabled", out obj)) {
						string enabledValue = obj as string;
						if (enabledValue == null) {
							Debug.LogError ("enabledValue null");
							continue;
						}
						if (enabledValue == "yes") {
							// Check if package installed, otherwise change to NO.
							switch (key) {
							case "appsFlyer": if (! PsdkVersionsMgr.IsAppsFlyerInstalled()) { Debug.LogWarning ("Changed appsFlyer to no in json, cause its not installed !"); psdkContent["enabled"] = "no"; } break;
							case "referrals": if (! PsdkVersionsMgr.IsGoogleAnalyticsInstalled()) { Debug.LogWarning ("Changed referrals to no in json, cause its not installed !"); psdkContent["enabled"] = "no"; }break;
							case "runtimeConfig": if (! PsdkVersionsMgr.IsGLDInstalled()) { Debug.LogWarning ("Changed runtimeConfig to no in json, cause its not installed !"); psdkContent["enabled"] = "no"; }break;
							case "locationMgr": if (! PsdkVersionsMgr.IsMonetizationInstalled()){ Debug.LogWarning ("Changed locationMgr to no in json, cause its not installed !"); psdkContent["enabled"] = "no"; } break;
							case "splash": if (! PsdkVersionsMgr.IsSplashInstalled()) { Debug.LogWarning ("Changed splash to no in json, cause its not installed !"); psdkContent["enabled"] = "no"; }break;
							case "gameLevelData": if (! PsdkVersionsMgr.IsGLDInstalled()) { Debug.LogWarning ("Changed gameLevelData to no in json, cause its not installed !"); psdkContent["enabled"] = "no"; } break;
							case "banners": if (! PsdkVersionsMgr.IsBannersInstalled()) { Debug.LogWarning ("Changed banners to no in json, cause its not installed !"); psdkContent["enabled"] = "no"; }break;
							case "crashMonitoringTool": if (! PsdkVersionsMgr.IsCrashMonitoringToolInstalled()) { Debug.LogWarning ("Changed crashMonitoringTool to no in json, cause its not installed !"); psdkContent["enabled"] = "no"; }break;
							}
						}
					}
				}
				return Json.Serialize(psdkConfig);
			}
			catch (System.Exception e) {
				Debug.LogException(e);
			}
			return null;
		}

        private bool isPsdkValid() {
			if (! _psdkSetup) {
                UnityEngine.Debug.LogError("psdk setup was not finished yet, psdk services are not registered !");
                return false;
            }
            return true;
        }
        
		private bool isDebugMode(string configJson) {
			if (configJson == null)
								return false;
			try {
			Dictionary<string,object> psdkConfig = TabTale.Plugins.PSDK.Json.Deserialize (configJson) as Dictionary<string,object>;
			if (psdkConfig != null) {
				if (psdkConfig.ContainsKey("global")) {
					Dictionary<string,object> psdkGlobal = psdkConfig["global"] as Dictionary<string,object>;
					object obj;
					if (psdkGlobal.TryGetValue("debug", out obj)) {
						if (obj.ToString() == "yes") {
							Debug.Log ("PSDK in debug mode !!");
							return true;
						}
					}
				}
			}
			}
			catch (System.Exception e) {
				Debug.LogException(e);
			}
			return false;
		}

		public void ValidateReceiptAndReport (string receipt, string price, string currency, string productId)
		{
			_impl.ValidateReceiptAndReport (receipt, price, currency, productId);

			#if UNITY_EDITOR
			PsdkEventSystem.Instance.SendMessage("");
			#endif
		}


	}
}
