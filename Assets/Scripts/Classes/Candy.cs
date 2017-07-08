using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Candy : MonoBehaviour {
	Sprite[] sprites = new Sprite[5];
	public GameObject splash;
	public GameObject shadowGameObject;

	public static event Action OnTurn;
	public static event Action<float,bool> OnMiss;

	bool isIdleGrow = false;
 	bool isFly = false;
 	bool isShowAnimation = false;
 	bool isMoveToOutside = false;
	bool isMoveToBubble = false;
	bool isMoveToBubble2 = false;
	bool isTrowMade = false;
	Vector3[] pointsArr = new Vector3[8]; 
	int pointsArrLastIndex = 0;
	const float VELOCITY_MAX = 2200f;
	const float VELOCITY_REAL_MAX = 1400f;
	const float VELOCITY_REAL_MIN = 600f;
	bool isFindBubble = false;
	GameObject targetBubble;
	SpriteRenderer spriteRenderer;
	Rigidbody2D body;
	int currentColorID;
	Animator animator;
	bool isFindBubbleTried;
	int respownCounter = 0;
	float growSpeed;
	bool isActive;
	bool isHide = false;
	float startVelocity;
	Bubble[] collidedGameObjects = new Bubble[10]; 

	public AudioClip sndThrow;
	public AudioClip sndBounce;
	int moveOutsideCounter;

	// Use this for initialization
	void Awake() {
		DefsGame.candy = this;
		spriteRenderer = GetComponent<SpriteRenderer>();
		body = GetComponent<Rigidbody2D> ();

		animator = GetComponentInChildren<Animator> ();
		animator.enabled = false;

		transform.localScale = new Vector3 (0f,0f,1f);
		loadSprites (DefsGame.currentFaceID);
	}

	public void Init() {
		MakeActive (false);
		respownCounter = 0;
		SetNextColor (DefsGame.BUBBLE_COLOR_ONE);
	}

	public void SetNewSkin(int _id) {
		loadSprites (DefsGame.currentFaceID);
		changeSprite (DefsGame.BUBBLE_COLOR_ONE);
	}

	void loadSprites(int _id){
		//Resources.Load<Sprite>("Sprites/_avatars/avatar" + (int)_avatarType);
		if ((Sprite)sprites[0]) Resources.UnloadAsset(sprites[0]);
		if ((Sprite)sprites[1]) Resources.UnloadAsset(sprites[1]);
		if ((Sprite)sprites[2]) Resources.UnloadAsset(sprites[2]);
		if ((Sprite)sprites[3]) Resources.UnloadAsset(sprites[3]);
		sprites[0] = Resources.Load<Sprite>("gfx/Candies/candy" +(_id+1).ToString()+ "_blue");
		sprites[1] = Resources.Load<Sprite>("gfx/Candies/candy" +(_id+1).ToString()+ "_red");
		sprites[2] = Resources.Load<Sprite>("gfx/Candies/candy" +(_id+1).ToString()+ "_green");
		sprites[3] = Resources.Load<Sprite>("gfx/Candies/candy" +(_id+1).ToString()+ "_purple");
		if (sprites[4] == null) sprites[4] = Resources.Load<Sprite>("gfx/Candies/candy_5");
	}

	public void SetNextColor(int _colorID) {
		changeSprite(_colorID);

		Respown ();
	}

	void changeSprite(int _colorID) {
		D.Log ("changeSprite " + _colorID);
		currentColorID = _colorID;
		spriteRenderer.sprite = sprites [_colorID];
		//spriteRenderer.material.SetTexture("texture", textures[_colorID]);
		if (_colorID == DefsGame.BUBBLE_COLOR_MULTI) 
			animator.enabled = true;
		else 
			animator.enabled = false;
	}

	public void Launch(float _startVelocity) {
		shadowGameObject.SetActive(false);
		if (body)
			Defs.PlaySound(sndThrow, 0.5f + 0.5f * (_startVelocity / VELOCITY_MAX));
		isFly = true;

		moveOutsideCounter = 0;

		GameEvents.Send(OnTurn);
	}

	public void show() {
		transform.localScale = new Vector3 (0f,0f,1f); 
		transform.position = new Vector3(0, -2f);
		transform.localRotation = new Quaternion ();
		spriteRenderer.sortingOrder = 10000;
		isShowAnimation = true;
	}

	void FixedUpdate() {
		if (!isActive) return;

		if (isFly) {
			body.velocity = new Vector2 (body.velocity.x * 0.92f, body.velocity.y);
			if (startVelocity == 10000 && body.velocity.y != 0)
				startVelocity = body.velocity.y;
			if (body.velocity.y == 0)
				return;
			if (body.velocity.y > 0) {
				transform.localScale = new Vector3 (0.5f + 0.5f * (body.velocity.y / startVelocity),
					0.5f + 0.5f * (body.velocity.y / startVelocity), 1f);
			} else {
				transform.localScale = new Vector3 (0.5f + 0.5f * (body.velocity.y / startVelocity),
					0.5f + 0.5f * (body.velocity.y / startVelocity), 1f);
			}

			if ((!isFindBubbleTried) && (body.velocity.y <= 0f) && (body.velocity.y > -2f)) {
				isFindBubble = true;
				CheckCollision ();
			} else {
				if ((spriteRenderer.sortingOrder != 0) && (body.velocity.y < -2f))
					spriteRenderer.sortingOrder = 0;
				isFindBubble = false;
			}

			if ((transform.localScale.x <= 0.01f)) {

				isFly = false;
				MakeActive (false);
				GameEvents.Send (OnMiss, 0f, false);
				transform.localScale = new Vector3 (0f, 0f, 1f);
				return;
			}
		}
	}

	// Update is called once per frame
	void Update () {	
		if (!isActive) return;

		if (isShowAnimation) {
			transform.localScale = new Vector3 (transform.localScale.x + 0.1f, transform.localScale.y + 0.1f, 1f);
			if (transform.localScale.x >= 1) {
				isShowAnimation = false;
				transform.localScale = new Vector3 (1f, 1f, 1f);
			}
			shadowGameObject.transform.localScale = transform.localScale;
			return;
		} else if (isHide) {
			if (transform.localScale.x > 0) {
				transform.localScale = new Vector3 (transform.localScale.x - 0.1f, transform.localScale.y - 0.1f, 1f);
			} else {
				transform.localScale = new Vector3 (0f, 0f, 1f);
				//isHide = false;
			}
			shadowGameObject.transform.localScale = transform.localScale;
			return;
		} else if (isMoveToOutside) {
			growSpeed -= 70.0f * Time.deltaTime;
			if (growSpeed <= -1.5f)
				growSpeed = -1.5f;
			
			transform.localScale = new Vector3 (transform.localScale.x + growSpeed * Time.deltaTime,
				transform.localScale.x + growSpeed * Time.deltaTime, 1f);

			if ((!isFindBubbleTried) && (transform.localScale.x <= 0.5f) && (transform.localScale.x >= 0.35f)) {
				isFindBubble = true;
				CheckCollision ();
				isFindBubbleTried = false;
				return;
			} else {
				isFindBubble = false;
			}

			if ((transform.localScale.x <= 0.0f)) {
				D.Log ("Отскок от камня или бомбы");
				GameEvents.Send (OnMiss, 0.0f, false);
				isFly = false;
				isMoveToOutside = false;
				MakeActive (false);
				return;
			}
			return;
		} else if (isMoveToBubble) {
			transform.position = new Vector3 (
				Mathf.Lerp (transform.position.x, targetBubble.transform.position.x, Mathf.Abs (targetBubble.transform.position.x - transform.position.x) / 3f),
				Mathf.Lerp (transform.position.y, targetBubble.transform.position.y, Mathf.Abs (targetBubble.transform.position.y - transform.position.y) / 3f),
				1f);

			if (transform.localScale.x > 0) {
				transform.localScale = new Vector3 (transform.localScale.x - 0.05f, transform.localScale.y - 0.05f, 1f);
			} else {
				
				isMoveToBubble = false;
				transform.localScale = new Vector3 (0f, 0f, 1f);
				Bubble _bubble = targetBubble.GetComponent<Bubble> ();
				if (_bubble.isEqualTo (currentColorID)) {
					_bubble.Match ();
					targetBubble = null;
					MakeActive (false);
					return;
				} else {
					_bubble.NotMatch ();
					isMoveToBubble2 = true;

					transform.rotation = Quaternion.identity;
					GameEvents.Send (OnMiss, 0.8f, true);
					return;
				}
			}
			return;
		} else if (isMoveToBubble2) {
			if (!targetBubble) {
				isMoveToBubble2 = false;
				return;
			}

			Bubble _bubble = targetBubble.GetComponent<Bubble> ();
			float _scaleCoeff = _bubble.GetStartScale ();
			transform.position = new Vector3 (targetBubble.transform.position.x, 
				Mathf.Lerp (transform.position.y, targetBubble.transform.position.y - 0.6f * _scaleCoeff, 0.1f), 1f);
			
			if (transform.localScale.x < 1 * _scaleCoeff) {
				transform.localScale = new Vector3 (transform.localScale.x + 0.08f * _scaleCoeff,
					transform.localScale.y + 0.08f * _scaleCoeff, 1f);
			} else {
				//isMoveToBubble2 = false;
				//targetBubble = null;
			}
			return;
		} else if (isFly) {
			return;
		} else {
			if (!body.simulated) {
				updateIdleAnimation ();
				mouseDown ();
				mouseUp ();
				mouseMove (); 
				makeTrow ();

				if (!isTrowMade) {
					float scrHalfWidth = 0f;
					transform.position = new Vector3 (Mathf.Lerp (transform.position.x, scrHalfWidth, Mathf.Abs (scrHalfWidth - transform.position.x) / 7),
						Mathf.Lerp (transform.position.y, -2f, Mathf.Abs (-2f - transform.position.y) / 7));
				}
			}
		}
	}

	public void mouseDown() {
		if (!DefsGame.isCanPlay)
			return;
		Vector3 _mousePostion = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if ((InputController.IsTouchOnScreen(TouchPhase.Began))&&(_mousePostion.y < -0.8f))
		{
			ClearPointsArray ();
			isTrowMade = true;
		}
	}

	void ClearPointsArray() {
		for(int i = 0; i < pointsArr.Length; i++) {	
			pointsArr [i] = new Vector3 ();
		}
		pointsArrLastIndex = 0;
	}

	public void mouseUp() {
		if (!DefsGame.isCanPlay)
			return;
		if (InputController.IsTouchOnScreen(TouchPhase.Ended)) {
			if (pointsArrLastIndex < 3) {
				ClearPointsArray ();
			}
			isTrowMade = false;
		} 
	}


	void mouseMove() {
		if (!DefsGame.isCanPlay)
			return;
		if (isTrowMade) {
			Vector3 _mousePostion = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			if (Input.GetMouseButton(0)) {
				
				float _prevPointY = -10000f;
				if (pointsArrLastIndex > 0) _prevPointY = pointsArr[pointsArrLastIndex-1].y;
				if (_prevPointY < _mousePostion.y){
					pointsArr [pointsArrLastIndex] = _mousePostion;
					++pointsArrLastIndex;
							
					if (pointsArrLastIndex > 7) {
						for(int i = 0; i < pointsArrLastIndex-1; i++) {
						pointsArr[i] = pointsArr[i+1];
					}
					pointsArrLastIndex = 7;
					}
				}

				if (_prevPointY > _mousePostion.y){
					ClearPointsArray();
					pointsArr [pointsArrLastIndex] = _mousePostion;
					++pointsArrLastIndex;
				}

				transform.position = new Vector3(Mathf.Lerp(transform.position.x, _mousePostion.x, Mathf.Abs(_mousePostion.x - transform.position.x) / 7),
					Mathf.Lerp(transform.position.y, _mousePostion.y, Mathf.Abs(_mousePostion.y - transform.position.y) / 7));
			} 
				
			if ((_mousePostion.y >= -0.6f)
				&&(pointsArrLastIndex >= 2)) {

				if (pointsArrLastIndex < 3) {
					ClearPointsArray ();
				}
				isTrowMade = false;
			}
		}
	}

	void makeTrow() {
		Vector3 _mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if (((InputController.IsTouchOnScreen(TouchPhase.Ended) || ((InputController.IsTouchOnScreen(TouchPhase.Moved)) && (_mousePos.y >= -0.6f))))
			&&(pointsArrLastIndex >= 2)) {

			float _time = pointsArrLastIndex*Time.deltaTime;

			Vector3 _point1 = pointsArr[0];
			Vector3 _point2 = pointsArr[pointsArrLastIndex-1];

			float _dist = Vector3.Distance(_point1, _point2);
			startVelocity = ((_dist / _time)*120.0f);

			float moveAngle = Mathf.Atan2(_point2.y-_point1.y, _point2.x-_point1.x) * Mathf.Rad2Deg;
			if (moveAngle > 155f) moveAngle = 155f; else
				if (moveAngle < 25f) moveAngle = 25f;

			moveAngle *= Mathf.Deg2Rad;

//			Debug.Log("MAIN velocity "+ startVelocity);
			if (Mathf.Sin (moveAngle) * startVelocity > 100.0f) {
				// ускорение в рамках нашего поля
				if (startVelocity > VELOCITY_MAX) startVelocity = VELOCITY_MAX;
				float _velocitySoundVolume = startVelocity;

				float _velCoeff = (VELOCITY_REAL_MAX-VELOCITY_REAL_MIN)*(startVelocity/VELOCITY_MAX);

				// прибавляем это число, чтобы шарик долетел до поля
				startVelocity = 500.0f + _velCoeff;
			
				body.simulated = true;
				body.AddForce (new Vector2(Mathf.Cos (moveAngle) * startVelocity*1.0f, Mathf.Sin (moveAngle) * startVelocity));
				body.AddTorque (-Mathf.Cos (moveAngle) * startVelocity*4f);
				Launch(_velocitySoundVolume);
				startVelocity = 10000;
			}
			ClearPointsArray();
			isTrowMade = false;
		} 
	}

	void Respown() {
		respownCounter++;
			
		show();
		//body.isKinematic = false;
		body.simulated = false;
		body.velocity = new Vector2 ();
		body.angularVelocity = 0f;

		shadowGameObject.SetActive (true);
		ClearCollidedArr ();

		isFly = false;
		isHide = false;
		isMoveToBubble = false;
		isMoveToBubble2 = false;
		isFindBubble = false;
		isFindBubbleTried = false;
		MakeActive (true);
	}

	public void MakeActive (bool _flag) {
		isActive = _flag;
	}

	void OnTriggerEnter2D(Collider2D _other) {
		AddGameObjectToArr (_other.gameObject);
	}

	void OnTriggerExit2D(Collider2D _other) {
		DeleteGameObjectFromArr (_other.gameObject);
	}

	void CheckCollision() {
		if (isFindBubble && !isFindBubbleTried) {
			isFindBubbleTried = true; 
			isFindBubble = false;
		} else
			return;

		Bubble _bubble;
		Bubble _bubbleCollided = null;
		float _dist;
		float _distMin = 10000000;
		float _objRadius;
		int _gotcha = 0; // Поймали свой цвет (0 - не поймали, 1 - мультицвет, 2 - поймали)
		for (int i = 0; i < collidedGameObjects.Length; i++) {
			_bubble = collidedGameObjects [i];
			if (_bubble) {
				_objRadius = _bubble.transform.localScale.x * 1.6f * 0.52f;
				_dist = Vector3.Distance (transform.position, _bubble.transform.position);
				if (_dist < _objRadius) {
					if ((_gotcha == 0)
					   || ((_gotcha == 1) && (_bubble.id == currentColorID) && (_dist - _objRadius < _distMin))
					   || ((_gotcha == 2) && (_dist - _objRadius < _distMin))) {
						_distMin = _dist - _objRadius;
						//trace("_distMin", _distMin);
						_bubbleCollided = _bubble;
						if (currentColorID == _bubbleCollided.id) {
							_gotcha = 2;
							D.Log ("Gotcha 2");
							Gotcha (_bubble);
							return;
						} else if ((_gotcha != 2) && (_bubbleCollided.id == DefsGame.BUBBLE_COLOR_MULTI)) {
							_gotcha = 1;
							D.Log ("Gotcha 1");
						}
					}
				}
			}
		}

		_distMin = 10000000;
		//Если не поймали, пробуем дотянуться до ближайшего
		for (int i = 0; i < collidedGameObjects.Length; i++) {
			_bubble = collidedGameObjects [i];
			if (_bubble) {
				_objRadius = _bubble.transform.localScale.x * 1.6f * 0.52f;
				_dist = Vector3.Distance (transform.position, _bubble.transform.position);
				if (_dist < _objRadius + 0.5f + 0.12f * _bubble.GetStartScale()) {
					if ((_gotcha == 0)
					   || ((_gotcha == 1) && (_bubble.id == currentColorID) && (_dist - _objRadius < _distMin))
					   || ((_gotcha == 2) && (_dist - _objRadius < _distMin))) {
						_distMin = _dist - _objRadius + 0.4f + 0.12f * _bubble.GetStartScale();
						//trace("_distMin", _distMin);
						_bubbleCollided = _bubble;
						if (currentColorID == _bubbleCollided.id) {
							_gotcha = 2;
							D.Log ("Gotcha 4");
							Gotcha (_bubble);
							return;
						} else if ((_gotcha != 2) && (_bubbleCollided.id == DefsGame.BUBBLE_COLOR_MULTI)) {
							_gotcha = 1;
							D.Log ("Gotcha 3");
						}
					}
				}
			}
		}

		if (_bubbleCollided != null) {
			Gotcha (_bubbleCollided);
		} else {
			D.Log ("Miss");
		}
	}

	void Gotcha(Bubble _bubble) {
		isFly = false;
		if ((_bubble.id == DefsGame.BUBBLE_COLOR_TIMER) || (_bubble.id == DefsGame.BUBBLE_COLOR_HEAVY)) {
			MoveToOutside(_bubble.gameObject); 
		} else {
			MoveToBubble (_bubble.gameObject);
		}
	}

	void MoveToBubble(GameObject _gameObject) {
		D.Log ("Candy.MoveToBubble()");
		isMoveToBubble = true;
		isMoveToOutside = false;
		targetBubble = _gameObject;
		body.simulated = false;
	}

	void MoveToOutside(GameObject _gameObject) {
		D.Log ("Candy.MoveToOutside()");
		Defs.PlaySound(sndBounce);
		++moveOutsideCounter;
		if (moveOutsideCounter >= 2) DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_THREE_JUMPS, moveOutsideCounter);
		isMoveToOutside = true;
		isMoveToBubble = false;
		isFindBubbleTried = false;
		growSpeed = 7.0f;
		body.velocity = new Vector2 ();
		if (transform.position.x < _gameObject.transform.position.x) {
			body.AddForce (new Vector2 (-150f, 600.0f));
		} else {
			body.AddForce (new Vector2 (150f, 600.0f));
		}
		Instantiate (splash, new Vector3(transform.position.x,transform.position.y, 0), Quaternion.identity);
	}

	void updateIdleAnimation() {
		float spd = 0.15f * Time.deltaTime;
		if (isIdleGrow) {
			spriteRenderer.transform.localScale = new Vector3 (spriteRenderer.transform.localScale.x - spd, spriteRenderer.transform.localScale.y + spd, 1f);
			if (spriteRenderer.transform.localScale.y >= 1.04f) {
				spriteRenderer.transform.localScale = new Vector3 (0.96f, 1.04f, 1f);
				isIdleGrow = false;
			}
		}else {
			spriteRenderer.transform.localScale = new Vector3 (spriteRenderer.transform.localScale.x + spd, spriteRenderer.transform.localScale.y - spd, 1f);
			if (spriteRenderer.transform.localScale.y <= 0.98f) {
				spriteRenderer.transform.localScale = new Vector3 (1.02f, 0.98f, 1f);
				isIdleGrow = true;
			}
		}
		shadowGameObject.transform.localScale = spriteRenderer.transform.localScale;
	}

	public void Hide() {
		isHide = true;
	}	

	Bubble IsBubble (GameObject _gameObject) {
		Bubble _bubble = _gameObject.GetComponent<Bubble> ();
		return _bubble;
	}

	void AddGameObjectToArr(GameObject _gameObject) {
		Bubble _bubble = IsBubble (_gameObject);
		if (!_bubble)
			return;
		
		int _id = FindSameGameObjectInArr (_bubble);
		if (_id == -1) {
			AddToCollidedEmptyPlace (_bubble);
		}
	}

	void DeleteGameObjectFromArr(GameObject _gameObject) {
		Bubble _bubble = IsBubble (_gameObject);
		if (!_bubble)
			return;
		
		int _id = FindSameGameObjectInArr (_bubble);
		if (_id != -1 ) {
			collidedGameObjects[_id] = null;
		}
	}

	int FindSameGameObjectInArr(Bubble _bubble) {
		for (int i = 0; i < collidedGameObjects.Length; i++) {
			if (collidedGameObjects [i] == _bubble) {
				return i;
			}
		}
		return -1;
	}

	void AddToCollidedEmptyPlace(Bubble _bubble) {
		int _id = FindSameGameObjectInArr (null);
		if (_id != -1 ) {
			collidedGameObjects[_id] = _bubble;
		}
	}

	void ClearCollidedArr() {
		for (int i = 0; i < collidedGameObjects.Length; i++)
		collidedGameObjects [i] = null; 
	}
}
