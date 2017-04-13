using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Points : MonoBehaviour {
	public Text textField;
	bool isShowAnimation = false;
	float startScale;

	bool isPointAdded;
	float time = 0f;
	const float delay = 0.8f;

	// Use this for initialization
	void Start () {
		textField.text = "0";
		Color _color = textField.color;
		_color.a = 0f;
		textField.color = _color;
		startScale = textField.transform.localScale.x;
	}

	public void ShowAnimation() {
		isShowAnimation = true;
	}

	public void ResetCounter() {
		DefsGame.currentPointsCount = 0;
		textField.text = "0";
	}
	
	// Update is called once per frame
	void Update () {
		if (isShowAnimation) {
			Color _color = textField.color;
			if (textField.color.a < 1f) {
				_color.a += 0.1f;
			} else {
				isShowAnimation = false;
				_color.a = 1f;
			}
			textField.color = _color;
		}

		if (isPointAdded) {
			time += Time.deltaTime;
			if (time > delay) {
				time = 0f;
				isPointAdded = false;
				addPointVisual ();
			}
		}

		if (textField.transform.localScale.x > startScale) {
			textField.transform.localScale = new Vector3 (textField.transform.localScale.x - 2.5f*Time.deltaTime, textField.transform.localScale.y - 2.5f*Time.deltaTime, 1f);
		}
	}

	public void addPoint(int _count) 
	{
		DefsGame.currentPointsCount += _count;
		if (DefsGame.gameBestScore < DefsGame.currentPointsCount) {
			DefsGame.gameBestScore = DefsGame.currentPointsCount;
		}
		isPointAdded = true;
	}

	void addPointVisual() 
	{
		textField.text = DefsGame.currentPointsCount.ToString ();
		textField.transform.localScale = new Vector3 (startScale*1.4f, startScale*1.4f, 1f);
	}

	public void UpdateVisual() {
		textField.text = DefsGame.currentPointsCount.ToString ();
	}
}
