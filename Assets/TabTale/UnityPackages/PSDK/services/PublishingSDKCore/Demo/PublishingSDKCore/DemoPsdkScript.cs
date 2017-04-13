using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

// Unity package 1.0.2

/// <summary>
/// Demo psdk script.
/// Please aatch me to the Main Camera
/// </summary>
public class DemoPsdkScript : MonoBehaviour {


	void Awake() {
	}

	 void Start() {

		// Registering to PSDK events. 
		PSDKWrapperExample.Instance.Init ();

		
		PsdkEventSystem.Instance.onPsdkReady += onPsdkReady;
		
		PsdkEventSystem.Instance.onResumeEvent += onPsdkResumeEvent;
		

		if (! PSDKWrapperExample.Instance.StartPsdk ()) {
			Debug.LogError ("PSDK failed to inititalize");
		}
	}

	void onPsdkReady() {
		// Unregistering the event
		PsdkEventSystem.Instance.onPsdkReady -= onPsdkReady;
		Debug.Log ("PSDK Ready !!");
	}

	void onPsdkResumeEvent(AppLifeCycleResumeState rs) {
		// Unregistering the event
		PsdkEventSystem.Instance.onResumeEvent -= onPsdkResumeEvent;

		Debug.Log ("PSDK resumed with state: " + rs);
	}
}
