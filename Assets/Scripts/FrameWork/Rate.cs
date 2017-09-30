using UnityEngine;


public class Rate : MonoBehaviour {

	void Awake() {
		Defs.rate = this;
	}

	public void RateUs()
	{
		#if UNITY_ANDROID
		Application.OpenURL("http://smarturl.it/YummMonsters");
		#elif UNITY_IPHONE
		Application.OpenURL("http://smarturl.it/YummMonsters");
		#endif
	}
}
