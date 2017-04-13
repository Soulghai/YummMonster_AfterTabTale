using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BubbleHeavy : Bubble {
	float timeShake = 0;


	// Use this for initialization
	void Start () {

	}

	override protected void Init() {
		base.Init ();
		id = DefsGame.BUBBLE_COLOR_HEAVY;
	}

	override public bool isEqualTo(int _id) 
	{
		return false;
	}

	void OnEnable() {
		BubbleBomb.OnBoom += BubbleBomb_OnBoom;
	}

	void BubbleBomb_OnBoom ()
	{
		Match ();
	}

	void OnDisable() {
		BubbleBomb.OnBoom -= BubbleBomb_OnBoom;
	}
	
	// Update is called once per frame
	void Update () {
		base.BubbleUpdate ();
	}

	override protected void updateGoodAnimation() {
		timeShake += Time.deltaTime;
		if (timeShake >= 0.5f) {
			//if (!isGoodSoundPlayed) {
			//	Defs.soundEngine.playSound(sndGood);
			//	isGoodSoundPlayed = true;
			//}
			//timeShake = 0;
			visual.transform.position = new Vector3 (body.position.x + Random.Range(-0.02f, 0.02f), body.position.y + Random.Range(-0.02f, 0.02f), visual.transform.position.z);
			visual.localScale = new Vector3 (visual.localScale.x + 0.12f, visual.localScale.y + 0.12f, visual.localScale.z);
			if (spr.color.a > 0) {
				Color _color = spr.color;
				_color.a -= 0.2f;
				spr.color = _color;
			}
			else {
				//isGoodAnimation = false;
				//remove();
				Destroy (shadow);
				Destroy(gameObject);
			}
		} else {
			visual.transform.position = new Vector3 (body.position.x + Random.Range(-0.01f, 0.01f), body.position.y + Random.Range(-0.01f, 0.01f), visual.transform.position.z);
			visual.localScale = new Vector3 (visual.localScale.x - 0.01f, visual.localScale.y - 0.01f, visual.localScale.z);
		}
		shadow.transform.localScale = new Vector3 (shadowScaleCoeff*visual.localScale.x, shadowScaleCoeff*visual.localScale.y, 1f);
	}
}
