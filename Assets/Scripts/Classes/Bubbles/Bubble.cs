using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Bubble : MonoBehaviour {
	public static event Action<Bubble> OnMatch;
	//public static event Action<int> OnAddCoins;

	public GameObject shadowObj;
	protected GameObject shadow;
	Coin coinScript;
	public int id = 0;
	public bool isDoneAnimation;
	protected Animator animator = null;
	protected Rigidbody2D body;
	protected Transform visual;
	protected SpriteRenderer spr;
	float alphaSpd;
	protected float startScale;
	bool isShowAnimation;
	bool isHideAnimation;
	protected bool isGoodAnimation;
	bool isBadAnimation;
	bool isIdleGrow;
	float idleScaleSpeed;
	float hideWaitTime = 0f;						//Ждем пока анимация закрытия рта проиграется
	float shadowDistance;
	SpriteRenderer shadowSpr;
	const float shadowYPos = 1.3f;
	protected float shadowScaleCoeff;
	int startSize = 5;

	protected AudioClip sndShow;
	protected AudioClip sndGood;
	protected AudioClip sndBad;

	void Awake() {
		Init ();
		sndShow = DefsGame.bubbleField.GetSoundShowClip (id);
		sndGood = DefsGame.bubbleField.GetSoundGoodClip (id);
		sndBad = DefsGame.bubbleField.GetSoundBadClip (id);
		Show ();
	}

	// Use this for initialization
	void Start () {
	}

	virtual protected void Init() {
		animator = GetComponentInChildren<Animator>();
		body = GetComponent<Rigidbody2D> ();
		visual = transform.Find ("Sprite");
		shadowSpr = visual.GetComponent<SpriteRenderer> ();
		spr = GetComponentInChildren<SpriteRenderer> ();
		alphaSpd = 0.05f+Random.value*0.1f;
		isIdleGrow = true;
		visual.localScale = new Vector3 (0, 0, visual.localScale.z);
		idleScaleSpeed = 0.003f + 0.002f * Random.value;
		Color _color = spr.color;
		_color.a = 0;
		spr.color = _color;
		isDoneAnimation = false;
		isGoodAnimation = false;
		isBadAnimation = false;
		isShowAnimation = false;

		startScale = 1;

		float _y = shadowYPos + Random.Range(-0.1f, 0.1f);
		shadowDistance = -9.97f + _y;

		shadow = (GameObject)Instantiate (shadowObj, new Vector3 (transform.position.x, -_y, transform.position.z), Quaternion.identity);

		float _angle = Mathf.Atan2 (-9.97f - transform.position.y, 0 - transform.position.x) * Mathf.Rad2Deg;
		shadow.transform.position = new Vector3 (0f + shadowDistance * Mathf.Cos(_angle * Mathf.Deg2Rad),
			-9.97f + shadowDistance * Mathf.Sin (_angle * Mathf.Deg2Rad), 1f);
		shadowScaleCoeff = 1f-Vector3.Distance (transform.position, shadow.transform.position)/5f;
		shadowScaleCoeff *= 1f;
		shadow.transform.localScale = new Vector3 (0f, 0f, 1f);
		shadowSpr.color = spr.color;
	}

	void Show() {
		isShowAnimation = true;
		Defs.PlaySound (sndShow);
	}
	
	// Update is called once per frame
	void Update () {
	}

	virtual protected void BubbleUpdate() {
		if (isShowAnimation) {
			updateShowAnimation();
		}else
			if (isHideAnimation) {
				updateHideAnimation();
			}else
			if (isGoodAnimation) {
				updateGoodAnimation ();
				}  else 
					if (isBadAnimation) {
						updateBadAnimation ();
					} else {
						updateIdleAnimation ();
					}
	}

	void FixedUpdate() {
		
	}

	virtual public bool isEqualTo(int _id) 
	{
		if ((id == _id)||(_id == DefsGame.BUBBLE_COLOR_MULTI))
			return true;
		else
			return false;
	}

	public void SetCoin(Coin _coin){
		coinScript = _coin;
	}

	public void SetStartSize(int _value) {
		startSize = _value;
		startScale = _value/5f;
		transform.localScale = new Vector3 (startScale, startScale, 1);
		shadowScaleCoeff = 1f-Vector3.Distance (transform.position, shadow.transform.position)/5f;
		shadowScaleCoeff *= startScale;
	}

	public int GetStartSize() {
		return startSize;
	}

	public float GetStartScale(){ 
		return startScale;
	}

	public void Hide(){
		isHideAnimation = true;
		isDoneAnimation = true;
		Destroy (body);
	}

	virtual public void Match() {
		GameEvents.Send(OnMatch, this);
		isDoneAnimation = true;
		isGoodAnimation = true;
		animator.SetBool ("Match", true);
		//Defs.soundEngine.playSound(sndGood);
		//DefsGame.screenGame.bubbleField.pointsManager.add(DefsGame.bubbleMaxSize - size + 1, id, spr.x, spr.y);
		//DefsGame.screenGame.addPoint(DefsGame.bubbleMaxSize - size + 1);
		body.isKinematic = true;
		body.velocity = new Vector2 ();
		if (coinScript) {
			coinScript.MoveToEnd ();
			//GameEvents.Send(OnAddCoins, 1);
		}
		Defs.PlaySound (sndGood);
	}

	public void NotMatch() {
		isDoneAnimation = true;
		isBadAnimation = true;
		animator.SetBool ("notMatch", true);
		body.isKinematic = true;
		body.velocity = new Vector2 ();
		if (coinScript) {
			coinScript.Hide (true);
		}
		Defs.PlaySound (sndBad);
		//Defs.soundEngine.playSound(sndBad);
	}

	virtual protected void updateShowAnimation(){
		if (spr.color.a < 1) {
			Color _color = spr.color;
			_color.a += alphaSpd;
			spr.color = _color;
			shadowSpr.color = _color;

			//sprShadow.alpha = spr.alpha;
			if (visual.localScale.x < 1) {
				visual.localScale = new Vector3 (visual.localScale.x + alphaSpd, visual.localScale.y + alphaSpd, visual.localScale.z);
			} else {
				visual.localScale = new Vector3 (1, 1, visual.localScale.z);
			}

			shadow.transform.localScale = new Vector3 (shadowScaleCoeff*visual.localScale.x, shadowScaleCoeff*visual.localScale.y, 1f);
		} else {
			//if (!isSndShow) {
			//	Defs.soundEngine.playSound(sndShow);
			//	spr.scaleX = spr.scaleY = startScale;
			//	spr.alpha = 1;
			//	isSndShow = true;
			//}
			Color _color = spr.color;
			_color.a = 1;
			spr.color = _color;
			shadowSpr.color = _color;
			//if ((!spr.isPlaying)&&(spr.totalFrames != 1)) {
			//	spr.gotoAndPlay(0);
			//}
			//if ((spr.currentFrame == 6)||(spr.totalFrames == 1)) {
			//	spr.stop();
			//	isShowAnimation = false;
			//}
			isShowAnimation = false;
			animator.SetBool ("Start", true);
		}
	}

	virtual protected void updateHideAnimation(){
		if (spr.color.a > 0f) {
			Color _color = spr.color;
			_color.a -= alphaSpd;
			spr.color = _color;

			//sprShadow.alpha = spr.alpha;
			if (visual.localScale.x > 0f) {
				visual.localScale = new Vector3 (visual.localScale.x - alphaSpd, visual.localScale.y - alphaSpd, visual.localScale.z);
			} else {
				visual.localScale = new Vector3 (0, 0, visual.localScale.z);
			}
			shadow.transform.localScale = new Vector3 (shadowScaleCoeff*visual.localScale.x, shadowScaleCoeff*visual.localScale.y, 1f);
		} else {
			Destroy (shadow);
			Destroy (gameObject);
		}
	}

	virtual protected void updateGoodAnimation() {
		if (hideWaitTime >= 0.15f) {
			if (spr.color.a > 0) {
				Color _color = spr.color;
				_color.a -= 0.1f;
				spr.color = _color;
			}
			if (visual.localScale.x > 0) {
				visual.localScale = new Vector3 (visual.localScale.x - 0.1f, visual.localScale.y - 0.1f, visual.localScale.z);
				shadow.transform.localScale = new Vector3 (shadowScaleCoeff*visual.localScale.x, shadowScaleCoeff*visual.localScale.y, 1f);
			} else {
				//isGoodAnimation = false;
				//remove();
				Destroy (shadow);
				Destroy (gameObject);
			}
		} else {
			hideWaitTime += Time.deltaTime;
		}
	}

	virtual protected void updateBadAnimation() {
		//if (hideWaitTime >= 1.3f) {
		//	Destroy (gameObject);
		//}else {
		//	hideWaitTime += Time.deltaTime;
		//}
	}

	virtual protected void updateIdleAnimation() {
		if (isIdleGrow) {
			visual.localScale = new Vector3 (visual.localScale.x, visual.localScale.y + idleScaleSpeed, visual.localScale.z);
			if (visual.localScale.y >= 1f) {
				visual.localScale = new Vector3 (1, 1, visual.localScale.z);
				isIdleGrow = false;
			}
		}else {
			visual.localScale = new Vector3 (visual.localScale.x, visual.localScale.y - idleScaleSpeed, visual.localScale.z);
			if (visual.localScale.y <= 1f - 0.04) {
				visual.localScale = new Vector3 (1, 1 - 0.04f, visual.localScale.z);
				isIdleGrow = true;
			}
		}

		shadowUpdate ();
	}

	protected void shadowUpdate() {
		float _angle = Mathf.Atan2 (-9.97f - transform.position.y, 0 - transform.position.x) * Mathf.Rad2Deg;
		shadow.transform.position = new Vector3 (0f + shadowDistance * Mathf.Cos (_angle * Mathf.Deg2Rad),
			-9.97f + shadowDistance * Mathf.Sin (_angle * Mathf.Deg2Rad), 1f);
		shadowScaleCoeff = 1f - Vector3.Distance (transform.position, shadow.transform.position) / 5f;
		shadowScaleCoeff *= startScale;
		shadow.transform.localScale = new Vector3 (shadowScaleCoeff, shadowScaleCoeff, 1f);
	}
}
