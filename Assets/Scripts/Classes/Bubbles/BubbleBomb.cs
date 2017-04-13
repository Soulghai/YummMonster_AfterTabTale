using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BubbleBomb : Bubble {
	public static event Action OnBoom;
	public GameObject explosion;
	public GameObject explosionAnimation;

	int matchCounter = 0;


	// Use this for initialization
	void Start () {

	}

	override protected void Init() {
		base.Init ();
		id = DefsGame.BUBBLE_COLOR_TIMER;
	}

	override public bool isEqualTo(int _id) 
	{
		return false;
	}

	void OnEnable() {
		Bubble.OnMatch += Bubble_OnMatch;
	}

	void OnDisable() {
		Bubble.OnMatch -= Bubble_OnMatch; 
	}

	void Bubble_OnMatch (Bubble _bubble)
	{
		if ((id == _bubble.id)||(isDoneAnimation))
			return;
		Match ();
	}

	override public void Match() {
		++matchCounter;
		if (matchCounter == 1) {
			animator.SetBool ("Match_1", true);
		} else 
			if (matchCounter == 2) {
				animator.SetBool ("Match", true);
			}
			else if (matchCounter == 3) {
			base.Match ();
			Instantiate (explosion, transform.position, Quaternion.identity);
			Explosion explosionScript = explosion.GetComponent<Explosion>();
			explosionScript.SetScale (startScale);
			Instantiate (explosionAnimation, transform.position, Quaternion.identity);
			isGoodAnimation = true;

			++DefsGame.QUEST_BOMBS_Counter;
		}
	}

	// Update is called once per frame
	void Update () {
		base.BubbleUpdate ();
	}

	override protected void updateGoodAnimation(){
		shadow.transform.localScale = new Vector3 (shadowScaleCoeff*visual.localScale.x, shadowScaleCoeff*visual.localScale.y, 1f);
		if (spr.color.a > 0) {
			Color _color = spr.color;
			_color.a -= 0.1f;
			spr.color = _color;
		} else {
			Destroy (shadow);
			Destroy (gameObject); 
			GameEvents.Send (OnBoom);
		}
	}

	override protected void updateIdleAnimation() {
		if (matchCounter == 2) { 
			visual.transform.position = new Vector3 (body.position.x + Random.Range(-0.04f, 0.04f), body.position.y + Random.Range(-0.04f, 0.04f), visual.transform.position.z); 
		}
		else
			if (matchCounter == 1) {
				visual.transform.position = new Vector3 (body.position.x + Random.Range(-0.02f, 0.02f), body.position.y + Random.Range(-0.02f, 0.02f), visual.transform.position.z);
			}

		shadowUpdate ();
	}
}
