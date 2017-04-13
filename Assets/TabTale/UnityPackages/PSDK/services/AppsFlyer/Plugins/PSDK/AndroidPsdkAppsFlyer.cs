#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
	public class AndroidPsdkAppsFlyer : IPsdkAppsFlyer , IPsdkAndroidService
    {

 
		private AndroidPsdkServiceMgr _psdkServiceMgr;
		private AndroidJavaObject _javaObject;

		public AndroidPsdkAppsFlyer(IPsdkServiceManager serviceMgr) {
			_psdkServiceMgr = serviceMgr as AndroidPsdkServiceMgr;
		}


		public IPsdkAppsFlyer GetImplementation() {
			return this;
		}

		public AndroidJavaObject GetUnityJavaObject() {
			try {
			if (null == _javaObject)
				_javaObject = _psdkServiceMgr.GetUnityJavaObject().Call<AndroidJavaObject>("getAppsFlyer");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}

			return _javaObject;
		}

		public void psdkStartedEvent() {
			GetUnityJavaObject();
		}

		public void ReportPurchase(string price, string currency) {
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("reportPurchase",price,currency);
			else 
				Debug.LogWarning ("Not calling android ReportPurchase !, cause object is null");
		}

    }
}
#endif
