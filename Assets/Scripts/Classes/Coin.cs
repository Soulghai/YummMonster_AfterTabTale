using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour {
	public static event Action<int> OnAddCoinsVisual;

	public GameObject parentObj;
	Vector3 targetPos;
	float velocity;
	float moveAngle;
	bool isAnglePlus;
	float addAngleCoeff;
	bool isShowAnimation;
	bool isHideAnimation;
	bool isMoveToTarget = false;
	const float velocityMax = 0.20f; 
	float showTime = 0f;

	// Use this for initialization
	void Start () {
		
	}

	public void Show(bool _isAnimation) {
		isShowAnimation = _isAnimation;

		if (isShowAnimation) {
			transform.localScale = new Vector3 (0f, 0f, 1f);
		}
	}

	public void Hide(bool _isAnimation) {
		isHideAnimation = _isAnimation;

		if (!isHideAnimation) {
			Destroy (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!isMoveToTarget) {
			if (parentObj) transform.position = parentObj.transform.position;
		}

		if (isShowAnimation)
		{
			if (showTime >= 0.5f) {
				transform.localScale = new Vector3 (transform.localScale.x + 0.1f, transform.localScale.y + 0.1f, 1f);
				if (transform.localScale.x >= 1f) {
					isShowAnimation = false;
					transform.localScale = new Vector3 (1f, 1f, 1f);
				}
			} else {
				showTime += Time.deltaTime;
			}
		} else
			if (isHideAnimation)
			{
				transform.localScale = new Vector3 (transform.localScale.x - 0.1f, transform.localScale.y - 0.1f, 1f);
				if (transform.localScale.x <= 0f) {
					//GameEvents.Send(OnAddCoinsVisual, 1);
					Destroy (gameObject);
				}
			}else
				if (isMoveToTarget) {
					if (transform.localScale.x >= 1) transform.localScale = new Vector3(transform.localScale.x - 0.1f,
						transform.localScale.y - 0.1f, 1f);
					float _ang = Mathf.Atan2 (targetPos.y - transform.position.y, targetPos.x - transform.position.x);

					if (isAnglePlus) {
						moveAngle += addAngleCoeff* Mathf.Deg2Rad;
						if (moveAngle >= 180f* Mathf.Deg2Rad) moveAngle -= 360f* Mathf.Deg2Rad;
					} else {
						moveAngle -= addAngleCoeff* Mathf.Deg2Rad;
						if (moveAngle <= -180f* Mathf.Deg2Rad) moveAngle += 360f* Mathf.Deg2Rad;
					}

					if (addAngleCoeff < 35f) addAngleCoeff += 0.5f;

					if (Mathf.Abs(moveAngle - _ang) < addAngleCoeff*1.5f* Mathf.Deg2Rad) moveAngle = _ang;

					if (velocity < velocityMax) velocity += 0.005f;
					transform.position = new Vector3(transform.position.x + velocity * Mathf.Cos(moveAngle),
						transform.position.y + velocity * Mathf.Sin(moveAngle), 1f);

					if (Vector3.Distance(transform.position, targetPos) <= 0.1f) {
						isMoveToTarget = false;

						GameEvents.Send(OnAddCoinsVisual, 1);
						Destroy(gameObject);
					//DefsGame.btnBuyCoinsComponent.addOneCoinVisual();
					//Defs.soundEngine.playSound(sndTouch);
					}
				}
	}

	public void MoveToEnd() {
		targetPos = new Vector3 (2.00f, 2.87f, 1); ;
		velocity = 0.03f + Random.value*0.03f;
		if (Random.value < 0.5f) moveAngle = Random.value * 180f* Mathf.Deg2Rad;
		else moveAngle = -Random.value * 180f* Mathf.Deg2Rad;

		if (Random.value < 0.5f) isAnglePlus = false; else isAnglePlus = true;

		addAngleCoeff = 5f;

		isMoveToTarget = true;

		transform.localScale = new Vector3 (2f, 2f);
	} 
}
