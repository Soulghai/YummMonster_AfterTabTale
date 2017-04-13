using System;
using UnityEngine;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK
{
	public class PsdkAppsFlyerService : IPsdkAppsFlyer
    {
		IPsdkAppsFlyer _impl;

		public PsdkAppsFlyerService(IPsdkServiceManager sm)
        {
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkAppsFlyer(sm.GetImplementation()); break;
				#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkAppsFlyer(sm.GetImplementation()); break;
			#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor: 	_impl = new UnityEditorPsdkAppsFlyer(sm.GetImplementation()); break;
			default: throw new System.Exception("Platform not supported for AppsFlyer.");
			}

//			#if UNITY_EDITOR            
//            _impl = new UnityEditorPsdkSplash(sm.GetImplementation());
//			#elif UNITY_ANDROID
//			_impl = new AndroidPsdkSplash(sm.GetImplementation());
//			#elif UNITY_IPHONE
//			_impl = new IphonePsdkSplash(sm.GetImplementation());
//			#else
//            throw new Exception("Platform not supported for Splash.");
//			#endif
        }

		public void ReportPurchase(string price, string currency) {
			_impl.ReportPurchase(price, currency);
		}

//		public bool AddSplashImage(string imagePathRelativeToStreamingAssets, long minMillisecondsTime) {
//			return _impl.AddSplashImage( imagePathRelativeToStreamingAssets,  minMillisecondsTime);
//		}

		public IPsdkAppsFlyer GetImplementation() {
			return _impl;
		}

		public void psdkStartedEvent() {
			_impl.psdkStartedEvent();
		}
    }
}
