using UnityEngine;
using System.Collections;

public class FlurryEventsManager : MonoBehaviour {
	static float realTimeOnEnterBackground = 0f;
	static float realTimeOnExitBackground = 0f;

	bool coldSessionStarted = false;

	static public bool dontSendLengthtEvent = false;

	static bool startScreenLengthOpened = false;
	static bool attemptLength = false;
	static bool iapShopLengthOpened = false;
	static bool candyShopOpened = false;


	void Awake()
	{
//		PublishingService.Instance.OnSceneTransitionShown += OnSceneTransitionShown;
	}

	void OnDestroy()
	{
//		PublishingService.Instance.OnSceneTransitionShown -= OnSceneTransitionShown;
	}

	private void OnSceneTransitionShown()
	{
		AdShow();
	}

	public static void AdShow()
	{
//		FlurryEvent flurryEvent = new FlurryEvent("ad_show");
//		flurryEvent.AddParameter("game_time", GetTimeTotalInMin());
//		FlurryEvents.LogEvent(flurryEvent);
	}

	void OnApplicationPause (bool pauseStatus)
	{
		if (pauseStatus)
		{
			//if (!dontSendLengthtEvent) {
				if (startScreenLengthOpened)
					FlurryEventsManager.SendEndEvent ("start_screen_length", true);
				if (attemptLength)
					FlurryEventsManager.SendEndEvent ("attempt_length", true);
				if (iapShopLengthOpened)
					FlurryEventsManager.SendEndEvent ("iap_shop_length", true);
				if (candyShopOpened)
					FlurryEventsManager.SendEndEvent ("candy_shop_length", true);
			//}

			//if (dontSendLengthtEvent)
			//	dontSendLengthtEvent = false;

			OnEnterBackground();
		}
		else
		{
			//if (!dontSendLengthtEvent) {
				if (startScreenLengthOpened)
					FlurryEventsManager.SendStartEvent ("start_screen_length");
				if (attemptLength)
					FlurryEventsManager.SendStartEvent ("attempt_length");
				if  (iapShopLengthOpened)
					FlurryEventsManager.SendStartEvent ("iap_shop_length");
				if (candyShopOpened)
					FlurryEventsManager.SendStartEvent ("candy_shop_length");
			//}

			//if (dontSendLengthtEvent)
			//	dontSendLengthtEvent = false;

			OnExitBackground();
		}
	}

	void OnApplicationQuit()
	{
		if (Application.platform == RuntimePlatform.Android) 
		{
			OnEnterBackground();
		} else {
			SendEndSessionEvent ();
		}
	}

	void OnEnterBackground()
	{
		Debug.Log ("OnEnterBackground");
		realTimeOnEnterBackground = Time.realtimeSinceStartup;
		float realSessionTime = realTimeOnEnterBackground - realTimeOnExitBackground;
		SetRealSessionTime (realSessionTime);
	}

	void OnExitBackground()
	{
		Debug.Log ("OnExitBackground");
		realTimeOnExitBackground = Time.realtimeSinceStartup;

		float realBackgroundTime = realTimeOnExitBackground - realTimeOnEnterBackground;
		if (realBackgroundTime >= 3600f) 
		{
			if (GetRealSessionTime() > 0) SendEndSessionEvent();
			ResetRealSessionTime();

			SendStartSesionEvent();
		}
	}

	void Update()
	{
		if (!coldSessionStarted)
		{
//			if (FlurryEvents.Analytics != null)
//			{
//				if (GetRealSessionTime() > 0f){
//					SendEndSessionEvent ();
//				}
//				ResetRealSessionTime();
//
//				SendStartSesionEvent ();
//				coldSessionStarted = true;
//			}
		}
	}

	// Real session time

	static private float GetRealSessionTime()
	{
		return PlayerPrefs.GetFloat ("RealSessionTime", 0f);
	}

	static private void SetRealSessionTime (float time)
	{
		PlayerPrefs.SetFloat ("RealSessionTime", time);
	}

	static private void ResetRealSessionTime()
	{
		SetRealSessionTime (0f);
	}


	//---------------------------
	// SERVICE
	//---------------------------

	static private int GetBalance() {
		int _value = DefsGame.coinsCount;
		_value -= _value % 5;
		return Mathf.Clamp (_value, 0, 1495);
	}

	static private int GetTimeTotalInMin() {
		int time = AppSeconds.GetSeconds();
		time = Mathf.FloorToInt (time / 60f);
		time = Mathf.Clamp (time, 0, 299);
		return time;
	}

	static private int GetScore() {
		int _value = DefsGame.currentPointsCount;
		_value -= _value % 2;
		return Mathf.Clamp (_value, 0, 600);
	}

	static private int TimeToSessionTime (float time)
	{
		int sessionTime = Mathf.RoundToInt (time / 15f) * 15;
		sessionTime = Mathf.Clamp (sessionTime, 15, 4500);
		return sessionTime;
	}

	//---------------------------
	// EVENTS
	//---------------------------

	static public void SendEvent(string _eventName, string _origin = null, bool _isBalance = true, int _balanceAdd = 0) {
//		FlurryEvent flurryEvent = new FlurryEvent(_eventName);
//		if (_isBalance) {
//			flurryEvent.AddParameter ("strawberries_balance", GetBalance () + _balanceAdd);
//		}
//		flurryEvent.AddParameter("game_time", GetTimeTotalInMin());
//		if (_origin != null) flurryEvent.AddParameter("origin", _origin);
//
//		FlurryEvents.LogEvent (flurryEvent);
	}
		
	static public void SendEventPlayed(bool revive, string fail_reason) {
//		FlurryEvent flurryEvent = new FlurryEvent("played");
//		flurryEvent.AddParameter("strawberries_balance", GetBalance());
//		flurryEvent.AddParameter("game_time", GetTimeTotalInMin());
//		flurryEvent.AddParameter("score", GetScore());
//		flurryEvent.AddParameter("revive", revive);
//		flurryEvent.AddParameter("fail_reason", fail_reason);
//
//		FlurryEvents.LogEvent (flurryEvent);
	}

	static public void SendStartEvent(string _eventName) {

		if (_eventName == "start_screen_length") startScreenLengthOpened = true; else
			if (_eventName == "attempt_length") attemptLength = true; else
				if (_eventName == "iap_shop_length") iapShopLengthOpened = true; else
					if (_eventName == "candy_shop_length") candyShopOpened = true;


//		FlurryEvent flurryEvent = new FlurryEvent(_eventName, true);
//		FlurryEvents.LogEvent (flurryEvent);
	}

	static public void SendEndEvent(string _eventName, bool _onAppPaused = false) {
		if (!_onAppPaused) {
			if (_eventName == "start_screen_length")
				startScreenLengthOpened = false;
			else if (_eventName == "attempt_length")
				attemptLength = false;
			else if (_eventName == "iap_shop_length")
				iapShopLengthOpened = false;
			else if (_eventName == "candy_shop_length")
				candyShopOpened = false;
		}

		FlurryEndEvent flurryEndEvent = new FlurryEndEvent (_eventName);
		FlurryEvents.EndLogEvent (flurryEndEvent);
	}

	static private void SendStartSesionEvent() {
//		FlurryEvent flurryEvent = new FlurryEvent("session_start");
//		flurryEvent.AddParameter("strawberries_balance", GetBalance());
//
//		FlurryEvents.LogEvent (flurryEvent);
	}

	static public void SendEndSessionEvent() {
//		FlurryEvent flurryEvent = new FlurryEvent("session_end");
//		flurryEvent.AddParameter("strawberries_balance", GetBalance());
//		flurryEvent.AddParameter("session_time", TimeToSessionTime (GetRealSessionTime()));
//
//		FlurryEvents.LogEvent (flurryEvent);

		ResetRealSessionTime ();
	}
}
