using UnityEngine;

public class Main : MonoBehaviour {
	void Awake() {
		Application.targetFrameRate = 60;
		DefsGame.loadVariables ();
		Defs.audioSource = GetComponent<AudioSource> ();
	}
	// Use this for initialization
	void Start () {
		//float _width = Mathf.Max(Screen.width, Screen.height);
		//float _height = Mathf.Min(Screen.width, Screen.height);

		//float _xRatio = _width / Defs.scrHeightOriginal;
		//float _yRatio = _height / Defs.scrWidthOriginal;

		//Defs.mobileScreenScaleFactor =  Mathf.Max(_xRatio, _yRatio);

		//Defs.mobileScreenScaleFactor = Screen.height / Defs.scrHeightOriginal;
		//Debug.Log ("Screen resolution: " + Screen.height + " " + Screen.height + " Defs.mobileScreenScaleFactor " + Defs.mobileScreenScaleFactor);

	}
}
