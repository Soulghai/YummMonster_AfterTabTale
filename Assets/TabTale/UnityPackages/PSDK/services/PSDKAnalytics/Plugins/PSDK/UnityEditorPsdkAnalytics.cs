using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TabTale.Plugins.PSDK;
//using Json = TabTale.Plugins.PSDK.PSDKMiniJSON;

namespace TabTale.Plugins.PSDK
{
	public class UnityEditorPsdkAnalytics : IPsdkAnalytics 
    {

		public UnityEditorPsdkAnalytics(IPsdkServiceManager sm) {
		}

		public IPsdkAnalytics GetImplementation() {
			return this;
		}

		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed){}
		
		public void EndLogEvent(string eventName, IDictionary<string,object> eventParams){
		}

		public void psdkStartedEvent() {
		}

		public void LogEvent(string eventName, IDictionary<string,string> eventParams, bool timed){
			Debug.Log ("PSDK Analytic Event: " + eventName + " -> " + PsdkUtils.BuildJsonStringFromDict(eventParams) + ", timed:" + timed);
		}
		
		public void EndLogEvent(string eventName, IDictionary<string,string> eventParams){
			Debug.Log ("PSDK Analytic End Event: " + eventName + " -> " + PsdkUtils.BuildJsonStringFromDict(eventParams));
		}

		public void LogComplexEvent(string eventName, IDictionary<string, object> eventParams) {
			Debug.Log ("PSDK Analytic Log Complex Event: " + eventName + " -> " + Json.Serialize(eventParams));
		}

		public void ReportPurchase(string price, string currency, string productId) {
		}
	}
}
