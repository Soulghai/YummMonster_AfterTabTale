using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestScore : MonoBehaviour {

	public Text textField;
	public Image img;
	int pointsCount = 0;
	float startScale;

	bool isPointAdded;
	float time = 0f;
	const float delay = 0.9f;
	bool isShowAnimation = true;
	bool isHideAnimation = false;

	// Use this for initialization
	void Start () {
		Color _color = textField.color;
		_color.a = 0f;
		textField.color = _color;
		img.color = new Color(img.color.r, img.color.g, img.color.b, _color.a);
		startScale = img.transform.localScale.x;
		pointsCount = DefsGame.gameBestScore;
		textField.text = pointsCount.ToString ();
	}

	public void ShowAnimation() {
		isHideAnimation = false;
		isShowAnimation = true;
	}

	public void HideAnimation() {
		isShowAnimation = false;
		isHideAnimation = true;
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
			img.color = new Color(img.color.r, img.color.g, img.color.b, _color.a);
		}

		if (isHideAnimation) {
			Color _color = textField.color;
			if (textField.color.a > 0f) {
				_color.a -= 0.1f;
			} else {
				isHideAnimation = false;
				_color.a = 0f;
			}
			textField.color = _color;
			img.color = new Color(img.color.r, img.color.g, img.color.b, _color.a);
		}

		if (isPointAdded) {
			time += Time.deltaTime;
			if (time > delay) {
				time = 0f;
				isPointAdded = false;
				MakeAnimation ();
			}
		}

		if (img.transform.localScale.x > startScale) {
			img.transform.localScale = new Vector3 (img.transform.localScale.x - 2.0f*Time.deltaTime, img.transform.localScale.y - 2.0f*Time.deltaTime, 1f);
		}
	}

	void MakeAnimation() {
		pointsCount = DefsGame.gameBestScore;
		textField.text = pointsCount.ToString ();
		img.transform.localScale = new Vector3 (startScale * 1.4f, startScale * 1.4f, 1f);
	}

	public void UpdateVisual() {
		// Здесь только визуальная обработка. Изменение BestScore в Points
		if (DefsGame.gameBestScore > pointsCount) {
			isPointAdded = true;
		}
	}
}
