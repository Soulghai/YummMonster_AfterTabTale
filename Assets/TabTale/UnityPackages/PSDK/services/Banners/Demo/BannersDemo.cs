using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

public class BannersDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {

		PsdkEventSystem.Instance.onBannerConfigurationUpdateEvent += onBannerConfigurationUpdateEvent;
		PsdkEventSystem.Instance.onBannerFailedEvent += onBannerFailedEvent;
		PsdkEventSystem.Instance.onBannerShownEvent += onBannerShownEvent;

		PsdkEventSystem.Instance.onPsdkReady += onPsdkReady;

		// Please see the Banners are covered under psdk.json/psdk_ios.json/psdk_google.json/psdk_amazon.json at Assets/StreamingAssets.
		if (PSDKMgr.Instance.Setup()) {
			IPsdkBanners banner = PSDKMgr.Instance.GetBannersService();
			if (banner != null) {
				banner.Show();
				banner.Hide();
			}
		}
	}

	void onPsdkReady() {
		StartCoroutine(showingHidingBannersCoro());
	}


	void onBannerConfigurationUpdateEvent() {
		Debug.Log("Banners - onBannerConfigurationUpdateEvent");
	}

	void onBannerFailedEvent() {
		Debug.Log("Banners - onBannerFailedEvent");
	}

	void onBannerShownEvent() {
		Debug.Log("Banners - onBannerShownEvent");
	}


	IEnumerator showingHidingBannersCoro() {
		IPsdkBanners banner = PSDKMgr.Instance.GetBannersService();
		if (banner == null) yield break;

		while (true) {
			Debug.Log("Showing Banners");
			banner.Show();
			yield return new WaitForSeconds(40F);
			Debug.Log("Hiding Banners");
			banner.Hide();
			yield return new WaitForSeconds(5F);
		}
	}
	
}
