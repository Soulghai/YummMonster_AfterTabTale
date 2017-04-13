using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

public class AppsFlyerDemoScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (PSDKMgr.Instance.Setup()) {
			IPsdkAppsFlyer appsFlyerService = PSDKMgr.Instance.GetAppsFlyerService();
			appsFlyerService.ReportPurchase("555","USD");
		}

	}
	
}
