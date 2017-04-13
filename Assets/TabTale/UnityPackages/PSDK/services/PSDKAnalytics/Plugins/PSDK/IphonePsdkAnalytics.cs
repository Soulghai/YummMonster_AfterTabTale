using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
	internal class IphonePsdkAnalytics : IPsdkAnalytics
	{
		[DllImport("__Internal")]
		private static extern void psdkAnalyticsLogEvent(long targets, string eventName, string eventParamsJson, bool timed);
		
		[DllImport("__Internal")]
		private static extern void psdkAnalyticsEndLogEvent(string eventName, string eventParamsJson);
		
		[DllImport("__Internal")]
		private static extern void psdkAnalyticsLogComplexEvent(string eventName, string eventParamsJson);

		[DllImport("__Internal")]
		private static extern void psdkAnalyticsReportPurchase (string price, string currency, string productId);
		
		
		public IphonePsdkAnalytics(IPsdkServiceManager sm) {
		}
		
		public IPsdkAnalytics GetImplementation() {
			return this;
		}
		
		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed){
			psdkAnalyticsLogEvent(targets, eventName, PsdkUtils.BuildJsonStringFromDict(eventParams),timed);
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,object> eventParams){
			psdkAnalyticsEndLogEvent(eventName, eventParams != null ? Json.Serialize (eventParams) : null);
		}
		
		public void LogEvent(string eventName, IDictionary<string,string> eventParams, bool timed){
			psdkAnalyticsLogEvent(AnalyticsTargets.ANALYTICS_PSDK_INTERNAL_TARGETS, eventName, eventParams != null ? Json.Serialize (eventParams) : null,timed);
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,string> eventParams){
			psdkAnalyticsEndLogEvent(eventName, eventParams != null ? Json.Serialize (eventParams) : null);
		}
		
		public void LogComplexEvent(string eventName, IDictionary<string, object> eventParams) {
			psdkAnalyticsLogComplexEvent(eventName, eventParams != null ? Json.Serialize (eventParams) : null);
		}

		public void ReportPurchase(string price, string currency, string productId) {
			psdkAnalyticsReportPurchase (price, currency, productId);
		}

		public void psdkStartedEvent() {
		}
		
	}
}
