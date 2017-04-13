using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WowSlider : MonoBehaviour {
	Slider slider;
	float freezeTime = 0f;
	public AudioClip x2;
	public AudioClip x3;

	void Awake() {
		DefsGame.wowSlider = this;
	}

	// Use this for initialization
	void Start () {
		slider = GetComponent<Slider> ();
	}

	public void Init() {
		DefsGame.WOW_MEETERER = 1f;
		slider.value = DefsGame.WOW_MEETERER;
		DefsGame.WOW_MEETERER_x2 = false;
		DefsGame.WOW_MEETERER_x3 = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (freezeTime > 0) {
			freezeTime -= Time.deltaTime;
			if (freezeTime <= 0) {
				DefsGame.progressBar_x2.GetComponent<Animator> ().Play ("show_x3");
				Defs.PlaySound(x3);
				DefsGame.WOW_MEETERER_x2 = false;
				DefsGame.WOW_MEETERER_x3 = true;
			}
		}
	}

	public bool UpdateSlider() {
		if (freezeTime > 0f)
			return true;
		
		if (DefsGame.WOW_MEETERER > 0f) {
			DefsGame.WOW_MEETERER -= (0.0007f + 0.0020f*DefsGame.WOW_MEETERER)*(1+DefsGame.currentPointsCount/200f) ;

			if (DefsGame.WOW_MEETERER > 0.85f) {
				DefsGame.WOW_MEETERER_x2 = false;
				DefsGame.WOW_MEETERER_x3 = true;
			} else if (DefsGame.WOW_MEETERER > 0.7f) {
				DefsGame.WOW_MEETERER_x2 = true;
				DefsGame.WOW_MEETERER_x3 = false;
			} else {
				DefsGame.WOW_MEETERER_x2 = false;
				DefsGame.WOW_MEETERER_x3 = false;
			}

			if (DefsGame.WOW_MEETERER < 1f)
				slider.value = DefsGame.WOW_MEETERER;
			else
				DefsGame.WOW_MEETERER = 1f;
			return true;
		}

		if (DefsGame.WOW_MEETERER < 1f)
			slider.value = DefsGame.WOW_MEETERER;
		else
			DefsGame.WOW_MEETERER = 1f;
		return false;
	}

	public void addPoints(int _value) {
		float _prevValue = DefsGame.WOW_MEETERER;

		float _mulCoeff = ((1f - Mathf.Min(1f,DefsGame.WOW_MEETERER)) / 0.5f)* 0.09f;
		if (_mulCoeff <= 0.03f)
			_mulCoeff = 0.03f;
		
		DefsGame.WOW_MEETERER += _value * _mulCoeff;

		if ((DefsGame.WOW_MEETERER >= 0.85f)&&(_prevValue < 0.85f)) {
			DefsGame.progressBar_x2.GetComponent<Animator> ().Play ("show_x3");
			Defs.PlaySound(x3);
			DefsGame.WOW_MEETERER_x2 = false;
			DefsGame.WOW_MEETERER_x3 = true;
		} else 
			if ((DefsGame.WOW_MEETERER >= 0.7f)&&(_prevValue < 0.7f)) {
				DefsGame.progressBar_x2.GetComponent<Animator> ().Play ("show_x2");
				Defs.PlaySound(x2);
				DefsGame.WOW_MEETERER_x2 = true;
				DefsGame.WOW_MEETERER_x3 = false;
			}

		if (DefsGame.WOW_MEETERER < 1f)
			slider.value = DefsGame.WOW_MEETERER;
		else
			DefsGame.WOW_MEETERER = 1f;
	}

	public void MakeX3(float _freezeTime = 0f) {
		freezeTime = _freezeTime;
		DefsGame.WOW_MEETERER = 1f;
		slider.value = DefsGame.WOW_MEETERER;

		DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_GET_MAX, 1);
	}

}
