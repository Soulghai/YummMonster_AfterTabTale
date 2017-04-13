using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

public class DemoAppShelf : MonoBehaviour {

	IPsdkLocationManagerService _logMgr;

	private static AudioSource _as;

	void Awake() {
		
		_as = this.GetComponent<AudioSource>();
		if (_as == null)
			_as = this.gameObject.AddComponent<AudioSource>();
		
		#region app shelf  delagates 
		
		PsdkEventSystem.Instance.onPlaySoundEvent 	+= appShelfOnPlaySound;
		PsdkEventSystem.Instance.onStartAnimationEndedEvent 	+= appShelfOnStartAnimationEnded;
		
		#endregion		
	
		
		#region location listner  delagates 
		PsdkEventSystem.Instance.onClosedEvent 				+= OnClosed;
		PsdkEventSystem.Instance.onConfigurationLoadedEvent 	+= OnConfigurationLoaded;
		PsdkEventSystem.Instance.onLocationFailedEvent 		+= OnLocationFailed;
		PsdkEventSystem.Instance.onLocationLoadedEvent 		+= OnLocationLoaded;
		PsdkEventSystem.Instance.onShownEvent 				+= OnShown;
		#endregion
		
		
		if (PSDKMgr.Instance.Setup()) {
			
			// Getting psdk AppShelf after psdk setup
			_logMgr = PSDKMgr.Instance.GetLocationManagerService();
		}
	}

	#region appshelf delegates
	
	static void appShelfOnPlaySound(AudioClip ac) {
		_as.clip = ac;
		_as.Play();
		
	}
	
	static void appShelfOnStartAnimationEnded() {
		Debug.Log ("appShelfOnStartAnimationEnded");
		_as.Stop();
	}
	
	
	#endregion




	public void Show(string location) {
		if (null == _logMgr)
			_logMgr = PSDKMgr.Instance.GetLocationManagerService();

			_logMgr.Show(location);
	}

	bool _logMgrVisible = false;

	public void Update() {
		// Check android back button
		#if UNITY_ANDROID
		if (RuntimePlatform.Android == Application.platform) {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				if (PSDKMgr.Instance.OnBackPressed()) { // back button consumed by PSDK
					return;
				}
				Debug.Log ("AndroidBackButtonPressed:: Closing the App");
				Application.Quit();
			}
		}
		#endif
	} 
	

	void OnMouseDown() {
		this.transform.localScale = this.transform.localScale * 0.9f;
		Show(PsdkLocation.moreApps);
	}
	
	void OnMouseUp() {
		if (Input.touchCount > 1) {
			return;
		}
		this.transform.localScale = this.transform.localScale / 0.9f;
	}
	


	public bool IsLocationReady(string location) {
		// Valid only for AppShelf locations.
		if (null == _logMgr) {
			Debug.LogError("Null -- appShelf");
			return false;
		}
		Debug.Log ("AppShelf::IsLocationReady:" + location);
		return PSDKMgr.Instance.GetLocationManagerService().IsLocationReady(location);
	}

	public void OnLocationLoaded(string location, long attributes) {
		Debug.Log("AppShelf : UnityLocationMgrDelegate::OnLocationLoaded " + location);
	}
	
	public void OnLocationFailed(string location, string message) {
		Debug.Log("AppShelf : UnityLocationMgrDelegate::OnLocationFailed " + location + ", msg:" + message);
	}
	
	
	public void OnShown(string location, long attributes) {
		_logMgrVisible = true;
		Debug.Log("AppShelf : UnityLocationMgrDelegate::OnShown " + location);
	}
	
	
	public void OnClosed(string location, long attributes) {
		_logMgrVisible = false;
		Debug.Log("AppShelf : UnityLocationMgrDelegate::OnClosed " + location);
	}
	
	
	public void OnConfigurationLoaded() {
		Debug.Log("AppShelf : UnityLocationMgrDelegate::OnConfigurationLoaded ");
	}


}

