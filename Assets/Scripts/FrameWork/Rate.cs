using UnityEngine;
//using System.IO;
//using System.Runtime.InteropServices;


public class Rate : MonoBehaviour {

	void Awake() {
		Defs.rate = this;
	}

	public void RateUs()
	{
		#if UNITY_ANDROID
		Application.OpenURL("https://play.google.com/store/apps/details?id="+Defs.androidApp_ID);
		#elif UNITY_IPHONE
		Application.OpenURL("http://itunes.apple.com/app/"+Defs.iOSApp_ID);
		#endif
	}

	/*public void RateUs()
	{
		#if UNITY_ANDROID
		Application.OpenURL("market://details?id=YOUR_ID");
		#elif UNITY_IPHONE
		Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
		#endif
	}*/
}
