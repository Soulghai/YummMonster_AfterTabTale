using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoBehaviour {
	public static event Action<TouchPhase, int, Vector2> OnTouch;

	void Update(){
		FindTouch (TouchPhase.Began);
		FindTouch (TouchPhase.Stationary);
		FindTouch (TouchPhase.Moved);
		FindTouch (TouchPhase.Ended);
		FindTouch (TouchPhase.Canceled);
	}

	static void FindTouch(TouchPhase phase){
		foreach(int index in GetTouchIndexes (phase))
			if (index >= 0)
				GameEvents.Send (OnTouch, phase, index, GetPosition(index));
	}

	static public int GetTouchIndex(TouchPhase touchPhase){
		if (!IsHasTouch ())
			return -1;
		#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
		foreach (Touch touch in Input.touches)
			if (touch.phase == touchPhase)
				return touch.fingerId; 
		#else
		if (Input.GetMouseButtonDown (0) && touchPhase == TouchPhase.Began)
			return 0;
		else if (Input.GetMouseButton (0) && (touchPhase == TouchPhase.Moved || touchPhase == TouchPhase.Stationary))
			return 0;
		else if (Input.GetMouseButtonUp (0) && (touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled))
			return 0;
		#endif
		return -1;
	}

	static public List<int> GetTouchIndexes(TouchPhase touchPhase){
		List<int> indexes = new List<int> ();
		#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
		foreach (Touch touch in Input.touches)
			if (touch.phase == touchPhase)
				indexes.Add(touch.fingerId); 
		#else
		if (Input.GetMouseButtonDown (0) && touchPhase == TouchPhase.Began)
			indexes.Add (0);
		else if (Input.GetMouseButton (0) && (touchPhase == TouchPhase.Moved || touchPhase == TouchPhase.Stationary))
			indexes.Add (0);
		else if (Input.GetMouseButtonUp (0) && (touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled))
			indexes.Add (0);
		#endif
		return indexes;
	}

	public static Vector2 GetPosition(int index = 0){
		#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
		return IsHasTouch (index) ? Input.GetTouch (index).position : (Vector2) Input.mousePosition;
		#else
		return Input.mousePosition;
		#endif
	}

	static public bool IsTouchOnScreen(TouchPhase touchPhase, bool includeUI = false, int touchIndex = -1){
		if (!includeUI && EventSystem.current && EventSystem.current.currentSelectedGameObject) {

		} else {
//				#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
			//TOUCHES
			if (touchIndex == -1) {
				foreach (Touch touch in Input.touches)
					if (touch.phase == touchPhase)
						return !(!includeUI && EventSystem.current && EventSystem.current.currentSelectedGameObject);
			}
			else if (touchIndex >= 0) {
				if (IsHasTouch (touchIndex)) {
					if (Input.touches [touchIndex].phase == touchPhase)
						return !(!includeUI && EventSystem.current && EventSystem.current.currentSelectedGameObject);
				}
			}
//				#else
			//MOUSE INPUT
			if (Input.GetMouseButtonDown (0) && touchPhase == TouchPhase.Began)
				return !(!includeUI && EventSystem.current && EventSystem.current.currentSelectedGameObject);
			else if (Input.GetMouseButton (0) && (touchPhase == TouchPhase.Moved || touchPhase == TouchPhase.Stationary))
				return true;
			else if (Input.GetMouseButtonUp (0) && (touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled))
				return true;
//				#endif
		}
		//OTHER RESULT
		return false;
	}

	public static bool IsHasTouch(int index = 0){
		#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
		return Input.touchCount > index;
		#else
		return Input.GetMouseButtonDown (0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp (0);
		#endif
	}
	public static bool IsTouchUIObject(GameObject gameObject){
		return (IsTouchUI() && EventSystem.current.currentSelectedGameObject == gameObject);
	}
	public static bool IsTouchUI(){
		return (EventSystem.current && EventSystem.current.currentSelectedGameObject);
	}


	public static bool IsEscapeClicked() {
		if (Application.platform == RuntimePlatform.Android) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				// Insert Code Here (I.E. Load Scene, Etc)
				// OR Application.Quit();

				return true;
			}	
		}
		return false;
	}

}
