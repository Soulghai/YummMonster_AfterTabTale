using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
    internal class IphonePsdkAppsFlyer : IPsdkAppsFlyer
    {
		[DllImport("__Internal")]
		private static extern void psdkAppsFlyerReportPurchase(string price, string currency);


		public IphonePsdkAppsFlyer(IPsdkServiceManager sm) {
		}

		public IPsdkAppsFlyer GetImplementation() {
			return this;
		}

		public void ReportPurchase(string price, string currency) {
			psdkAppsFlyerReportPurchase(price,currency);
		}

		public void psdkStartedEvent() {
		}

    }
}
