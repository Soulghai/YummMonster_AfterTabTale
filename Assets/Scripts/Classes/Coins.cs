using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System;
using Random = UnityEngine.Random;

public class Coins : MonoBehaviour {

	public Text textField;
	public Image img;
	int pointsCount = 0;
	bool isShowAnimation = true;
	float startScale;
	AudioSource audioSource;
	AudioClip sndCoin;

	// Use this for initialization
	void Start () {
		audioSource = GetComponentInParent<AudioSource> ();
		sndCoin = Resources.Load<AudioClip>("snd/bonus");
		textField.text = DefsGame.coinsCount.ToString();
		//DefsGame.coinsIcon.UpdatePosition ();
		pointsCount = DefsGame.coinsCount;
		Color _color = textField.color;
		_color.a = 0f;
		textField.color = _color;
		img.color = new Color(img.color.r, img.color.g, img.color.b, _color.a);
		startScale = img.transform.localScale.x;
	}

	void OnEnable() {
		//Bubble.OnAddCoins += Bubble_OnAddCoins;
		Coin.OnAddCoinsVisual += Coin_OnAddCoinsVisual;
		ScreenSkins.OnAddCoinsVisual += Coin_OnAddCoinsVisual;
		ScreenCoins.OnAddCoinsVisual += Coin_OnAddCoinsVisual;
		BillingManager.OnAddCoinsVisual += Coin_OnAddCoinsVisual;
		//ScreenMenu.OnAddCoins += Bubble_OnAddCoins;
	}

	void OnDisable() {
		//Bubble.OnAddCoins -= Bubble_OnAddCoins;
		Coin.OnAddCoinsVisual -= Coin_OnAddCoinsVisual;
		ScreenSkins.OnAddCoinsVisual -= Coin_OnAddCoinsVisual;
		ScreenCoins.OnAddCoinsVisual -= Coin_OnAddCoinsVisual;
		BillingManager.OnAddCoinsVisual -= Coin_OnAddCoinsVisual;
		//ScreenMenu.OnAddCoins -= Bubble_OnAddCoins;
	}

	void Coin_OnAddCoinsVisual (int _value)
	{
		addPointVisual (_value);
		audioSource.PlayOneShot (sndCoin);
	}

	void Bubble_OnAddCoins (int _value)
	{
		addPoint (_value);
	}

	public void ResetCounter() {
		pointsCount = 0;
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
			img.color = new Color(img.color.r, img.color.g, img.color.b, _color.a);
		}

		if (img.transform.localScale.x > startScale) {
			img.transform.localScale = new Vector3 (img.transform.localScale.x - 2.0f*Time.deltaTime, img.transform.localScale.y - 2.0f*Time.deltaTime, 1f);
		}
	}

	public void addPoint(int _count) 
	{
		pointsCount += _count;
		DefsGame.coinsCount += _count;
		PlayerPrefs.SetInt ("coinsCount", DefsGame.coinsCount);
		//DefsGame.coinsIcon.UpdatePosition ();
	}

	void addPointVisual(int _value) 
	{
		addPoint(_value);
		textField.text = pointsCount.ToString ();
		img.transform.localScale = new Vector3 (startScale*1.4f, startScale*1.4f, 1f);
	}

	public void UpdateVisual() {
		textField.text = pointsCount.ToString ();
	}
}
