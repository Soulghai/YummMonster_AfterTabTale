using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TabTale.Plugins.PSDK;
//using Json = TabTale.Plugins.PSDK.PSDKMiniJSON;

namespace TabTale.Plugins.PSDK
{
	public class UnityEditorPsdkAppsFlyer : IPsdkAppsFlyer 
    {

		public UnityEditorPsdkAppsFlyer(IPsdkServiceManager sm) {
		}

		public IPsdkAppsFlyer GetImplementation() {
			return this;
		}
		
		public void psdkStartedEvent() {
		}

		public void ReportPurchase(string price, string currency) {
			Debug.Log ("Reporting purchase of " + price + " " + currency +" to AppsFlyer");
		}


    }
}
