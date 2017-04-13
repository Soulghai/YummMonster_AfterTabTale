#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
	public class AndroidPsdkAnalytics : IPsdkAnalytics , IPsdkAndroidService
	{
		
		
		private AndroidPsdkServiceMgr _psdkServiceMgr;
		private AndroidJavaObject _javaObject;
		
		public AndroidPsdkAnalytics(IPsdkServiceManager serviceMgr) {
			_psdkServiceMgr = serviceMgr as AndroidPsdkServiceMgr;
		}
		
		
		public IPsdkAnalytics GetImplementation() {
			return this;
		}
		
		public AndroidJavaObject GetUnityJavaObject() {
			try {
				if (null == _javaObject)
					_javaObject = _psdkServiceMgr.GetUnityJavaObject().Call<AndroidJavaObject>("getAnalytics");
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
		
		
		public void LogEvent(long targets, string eventName, IDictionary<string, object> eventParams, bool timed) {
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("logEvent",targets,eventName,PsdkUtils.CreateJavaJSONObjectFromDictionary(eventParams), timed);
			else {
				Debug.LogWarning ("Not calling android psdk analytics LogComplexEvent !, cause object is null");
				Debug.Log ("Event was not sent: " + eventName + " -> " + Json.Serialize(eventParams));
			}
		}
		
		public void EndLogEvent(string eventName, IDictionary<string, object> eventParams) {
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("endLogEvent",eventName,PsdkUtils.CreateJavaJSONObjectFromDictionary(eventParams));
			else {
				Debug.LogWarning ("Not calling android psdk analytics LogComplexEvent !, cause object is null");
				Debug.Log ("EndLogEvent public void ReportPurchase(string price, string currency, string productId) {was not sent: " + eventName + " -> " + Json.Serialize(eventParams));
			}
		}
		
		
		public void LogEvent(string eventName, IDictionary<string,string> eventParams, bool timed){
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("logEvent",eventName,PsdkUtils.CreateJavaHashMapFromDictionary(eventParams),timed);
			else {
				Debug.LogWarning ("Not calling android psdk analytics LogEvent !, cause object is null");
				Debug.Log ("Event was not sent: " + eventName + " -> " + PsdkUtils.BuildJsonStringFromDict(eventParams) + ", timed:" + timed);
			}
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,string> eventParams){
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("endLogEvent",eventName,PsdkUtils.CreateJavaHashMapFromDictionary(eventParams));
			else {
				Debug.LogWarning ("Not calling android psdk analytics LogEvent !, cause object is null");
				Debug.Log ("End timed Event was not sent: " + eventName + " -> " + PsdkUtils.BuildJsonStringFromDict(eventParams));
			}
		}
		
		public void LogComplexEvent(string eventName, IDictionary<string, object> eventParams) {
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("logComplexEvent",eventName,PsdkUtils.CreateJavaJSONObjectFromDictionary(eventParams));
			else {
				Debug.LogWarning ("Not calling android psdk analytics LogComplexEvent !, cause object is null");
				Debug.Log ("Complex Event was not sent: " + eventName + " -> " + Json.Serialize(eventParams));
			}
		}

		public void ReportPurchase(string price, string currency, string productId) {
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("reportPurchase",price,currency,productId);
			else {
				Debug.LogWarning ("Not calling android psdk analytics reportPurchase !, cause object is null");
			}
		}
		
		
	}
}
#endif
