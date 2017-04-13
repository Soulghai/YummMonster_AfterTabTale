using System;
using UnityEngine;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK
{
	public class PsdkAnalyticsService : IPsdkAnalytics
	{
		IPsdkAnalytics _impl;
		
		public PsdkAnalyticsService(IPsdkServiceManager sm)
		{
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkAnalytics(sm.GetImplementation()); break;
				#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkAnalytics(sm.GetImplementation()); break;
				#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor: 	_impl = new UnityEditorPsdkAnalytics(sm.GetImplementation()); break;
			default: throw new System.Exception("Platform not supported for Analytics.");
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
		
		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed)
		{
			_impl.LogEvent (targets, eventName, eventParams, timed);
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,object> eventParams)
		{
			_impl.EndLogEvent (eventName, eventParams);
		}
		
		public void LogEvent(string eventName, IDictionary<string,string> eventParams, bool timed){
			IDictionary<string,object> ep = null;
			if (eventParams != null) {
				ep = new Dictionary<string,object> ();
				foreach (var item in eventParams) {
					ep.Add (item.Key,item.Value);
				}
			}
			_impl.LogEvent (AnalyticsTargets.ANALYTICS_PSDK_INTERNAL_TARGETS, eventName,ep,timed);
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,string> eventParams){
			_impl.EndLogEvent (eventName,eventParams);
		}
		
		public void LogComplexEvent(string eventName, IDictionary<string, object> eventParams) {
			_impl.LogComplexEvent (eventName,eventParams);
		}

		public void ReportPurchase(string price, string currency, string productId) {
			_impl.ReportPurchase (price, currency, productId);
		}
		
		public IPsdkAnalytics GetImplementation() {
			return _impl;
		}
		
		public void psdkStartedEvent() {
			_impl.psdkStartedEvent();
		}
	}
}
