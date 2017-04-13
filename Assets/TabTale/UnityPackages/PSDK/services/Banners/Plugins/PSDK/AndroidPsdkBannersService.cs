#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	public class AndroidPsdkBannersService  : IPsdkBanners, IPsdkAndroidService {
		
		AndroidPsdkServiceMgr _sm;
		AndroidJavaObject _androidBannersService;
		private AndroidJavaObject _unityLayout;
		private AndroidJavaObject _applicationContext;
		
		public AndroidPsdkBannersService(IPsdkServiceManager sm) {
			PsdkEventSystem.Instance.onResumeEvent += OnResume;
			
			_sm=sm as AndroidPsdkServiceMgr;
		}
		
		public void psdkStartedEvent() {
			if (_androidBannersService == null) {
				_androidBannersService = GetUnityJavaObject();
			}
			
		}
		
		
		public bool Setup() {
			
			PsdkBannersUtils.CopyHouseAdsDirToTtpsdkFolder();
			
			AndroidJavaObject javaBannersDelegate = new AndroidJavaObject("com.tabtale.publishingsdk.unity.UnityBannersDelegate");
			if (javaBannersDelegate == null) {
				Debug.LogError("com.tabtale.publishingsdk.unity.UnityBannersDelegate NULL");
			}
			_applicationContext = PsdkUtils.CurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
			if (null == _applicationContext)
				Debug.LogError("Null Java activity Application Context !");
			
			// 		mBannersLayout = (android.view.ViewGroup) activity.getWindow().getDecorView().findViewById(android.R.id.content);
			if (null == _unityLayout) {
				// Keep it null, there is an asignmnet in java ServiceManager::unityStart.
			}
			
			_sm.JavaClass.CallStatic("setBannersDelegateAndLayout", _unityLayout, javaBannersDelegate); 
			
			return true;
		}
		
		public bool Show() {
			if (GetUnityJavaObject() == null) return false;
			try {
				return GetUnityJavaObject().Call<bool>("show");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				throw;
			}
			return false;
		}
		
		public void Hide() {
			if (GetUnityJavaObject() == null) return;
			GetUnityJavaObject().Call("hide");
		}
		
		public float GetAdHeight() {
			if (GetUnityJavaObject() == null) return 0F;
			return GetUnityJavaObject().Call<float>("getAdHeight");
		}
		
		public bool IsBlockingViewNeeded() {
			if (GetUnityJavaObject() == null) return false;
			return GetUnityJavaObject().Call<bool>("isBlockingViewNeeded");
		}
		
		public bool IsActive() {
			if (GetUnityJavaObject() == null) return false;
			return GetUnityJavaObject().Call<bool>("isActive");
		}
		
		public bool IsAlignedToTop() {
			if (GetUnityJavaObject() == null) return false;
			return GetUnityJavaObject().Call<bool>("isAlignedToTop");
		}
		
		
		public AndroidJavaObject GetUnityJavaObject() {
			try {
				if (_androidBannersService == null) 
					_androidBannersService = _sm.GetUnityJavaObject().Call<AndroidJavaObject>("getBanners");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}
			
			return _androidBannersService;
		}
		
		public IPsdkBanners GetImplementation() {
			return this;
		}
		
		public void OnResume(AppLifeCycleResumeState state)
		{
			PsdkBannersUtils.CopyHouseAdsDirToTtpsdkFolder ();
		}
		
		~AndroidPsdkBannersService()
		{
			PsdkEventSystem.Instance.onResumeEvent -= OnResume;
		}
	}
}
#endif
