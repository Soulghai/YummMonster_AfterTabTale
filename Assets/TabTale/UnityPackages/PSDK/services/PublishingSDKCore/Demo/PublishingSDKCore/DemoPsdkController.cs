using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DemoPsdkController : MonoBehaviour {

	public void ShowLocation(string location) {
		PSDKWrapperExample.Instance.ShowLocation (location);
	}

	public void BannerShow() {
		PSDKWrapperExample.Instance.ShowBanner ();
	}

	public void BannerHide() {
		PSDKWrapperExample.Instance.HideBanner ();
	}

	public void ShowRewardedAd() {
		PSDKWrapperExample.Instance.ShowRewardedAd ();
	}

}
