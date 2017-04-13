using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

public class PSDKAnalyticsDemo : MonoBehaviour {


	void Awake() {
		PsdkEventSystem.Instance.onPsdkReady += OnPsdkReady;
	}

	// Use this for initialization
	void Start () {
		string analyticsJson = "{\"global\" : {\"store\":\"google\", \"orientation\":\"portrait\"}, \"analytics\":{ \"enabled\" : \"yes\", \"flurryKey\" : \"flurryKey\"},\"configurationFetcher\": {\"enabled\":\"yes\"}}";
		PSDKMgr.Instance.Setup(analyticsJson);
	}
	
	// Update is called once per frame
	void OnPsdkReady () {
		IPsdkAnalytics analyticsService = PSDKMgr.Instance.GetAnalyticsService();
		if (analyticsService == null) {
			Debug.LogError("PSDK Analytics was not initialized, please check psdk json configuration !");
			return;
		}
	
		StartCoroutine(analyticsEventsTestCoro( analyticsService ) );
	}

	IEnumerator analyticsEventsTestCoro(IPsdkAnalytics analyticsService) {
		analyticsService.LogEvent("firstNoneTimedEvent", new Dictionary<string, string>() {{"firstParam","firstValue"},{"secondParam","secondValue"}},false);
		analyticsService.LogEvent("timedEvent", new Dictionary<string, string>() {{"waiting","2 seconds"},{"secondParam","secondValue"}},true);
		yield return new WaitForSeconds(2F);
		analyticsService.EndLogEvent("timedEvent", new Dictionary<string, string>() {{"waited","2 seconds"},{"secondParam","secondValue"}});
	}
}
