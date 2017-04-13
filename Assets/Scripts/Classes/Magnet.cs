using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour {
	public float radius = 1.88f;
	public int bubbleMaxCount = 20;
	GameObject[] objects = new GameObject[20]; 

	// Use this for initialization
	void Start () {
		
	}

	public void Init() {
		GameObject _object;
		for (int i = 0; i < objects.Length; i++) {
			_object = objects[i];
			if (_object)
				Destroy (_object);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		float _dist;
		float _ang;
		float _forceCoeff = Time.deltaTime * 290f + Time.deltaTime * 650f * DefsGame.WOW_MEETERER;
		Vector2 _force;
		foreach (GameObject _object in objects) {
			if (_object) {
				Rigidbody2D _body = _object.GetComponent<Rigidbody2D>();
				if ((_body)&&(Vector2.Distance (transform.position, _body.transform.position) > 0.1f)) {
					_dist = Vector2.Distance (transform.position, _object.transform.position);
					_ang = Mathf.Atan2 (_object.transform.position.y - transform.position.y, _object.transform.position.x - transform.position.x) - Mathf.PI;

					_force = new Vector2 (_forceCoeff * (1 - (_dist / 30f)) * Mathf.Cos (_ang),
						_forceCoeff * (1 - (_dist / 30f)) * Mathf.Sin (_ang));	
					_body.AddForce (_force);
				}
			}
		}

		UpdateZ ();
	}

	void UpdateZ(){
		SpriteRenderer _spr;
		foreach (GameObject _object in objects) {
			if (_object) {
				_spr = _object.GetComponentInChildren<SpriteRenderer> ();
				_spr.sortingOrder = (int)Camera.main.WorldToScreenPoint (_spr.transform.position).y * 1;
			}
		}

		/*GameObject _object1 = null;
		GameObject _object2 = null;
		for (int j = objects.Length - 1; j >= 0; j--) {
			for (int i = 0; i < objects.Length-1; i++) {
				if (!_object1) _object1 = objects[i];
				if (!_object2) _object2 = objects[i+1];
				if (_object1 && _object2) {
					if ((_object1.transform.position.y < _object2.transform.position.y)
						&& (_object1.GetComponentInChildren<SpriteRenderer>().transform.GetSiblingIndex () 
							< _object2.GetComponentInChildren<SpriteRenderer>().transform.GetSiblingIndex ())) {
						int _order = _object1.GetComponentInChildren<SpriteRenderer>().transform.GetSiblingIndex ();
						_object1.GetComponentInChildren<SpriteRenderer>().transform.SetSiblingIndex(_object2.GetComponentInChildren<SpriteRenderer>().transform.GetSiblingIndex ());
						//_object1.GetComponentInChildren<SpriteRenderer> ().sortingOrder = _object2.GetComponentInChildren<SpriteRenderer> ().sortingOrder;
						//_object2.GetComponentInChildren<SpriteRenderer> ().sortingOrder = _order;
						_object2.GetComponentInChildren<SpriteRenderer>().transform.SetSiblingIndex (_order);
					}
					_object1 = null;
					_object2 = null;
				}
			}
		}*/
	}

		/*private function updateZCoord():void {
			var _spr:DisplayObject;
			var _spr2:DisplayObject;
			for (var j:int = Defs.GROUND.numChildren - 1; j >= 0; j--) {
				for (var i:int = 0; i < j; i++) 
				{
					_spr = Defs.GROUND.getChildAt(i);
					_spr2 = Defs.GROUND.getChildAt(i + 1);
					if (_spr.y < _spr2.y) {
						_spr.parent.swapChildrenAt(i, i + 1);
					}
				}
			}
			_spr = null;
			_spr2 = null;
		}*/

	public void addToEmptyPlace(GameObject _objectNew) {
		GameObject _object;
		for (int i = 0; i < objects.Length; i++) {
			_object = objects [i];
			if (_object == null) {
				objects [i] = _objectNew;
				//Debug.Log ("Add Bubble to Magnet");
				return;
			}
		}
	}

	bool findSameObject(GameObject _objectNew) {
		foreach (GameObject _object in objects) {
			if (_object == _objectNew)
				return true;
		}
		return false;
	}

	void deleteObject(GameObject _objectNew) {
		GameObject _object;
		for (int i = 0; i < objects.Length; i++) {
			_object = objects [i];
			if (_object == _objectNew) {
				objects [i] = null;
				//Debug.Log ("Delete Bubble from Magnet");
				return;
			}
			
		}
	}

	public bool CheckRadius(Vector2 _pos, float _radius) {
		foreach (GameObject _object in objects) {
			
			if (_object) {
				Bubble _bubble = _object.GetComponent<Bubble> ();
				if ((_bubble.isDoneAnimation == false)
				    && Vector2.Distance (_pos, _object.transform.position) <= _radius + _object.transform.localScale.x * 1.6f * 0.5f)
					return true;
			}
		}
		return false;
	}

	public int GetBubblesType(int _type = -1, bool _onlyActive = true) {
		int _count = 0;
		GameObject _object;
		for (int i = 0; i < objects.Length; i++) {
			_object = objects[i];
			if (_object != null) {
				Bubble _bubble = _object.GetComponent<Bubble> ();
				if (((_bubble.id == _type)||(_type == -1))
					&&((!_onlyActive) || ((_onlyActive)&&(_bubble.isDoneAnimation == false))))
						++_count;
			}
		}

		return _count;
	}

	public GameObject GetObject(int _id) {
		if ((_id < 0) && (_id >= objects.Length)) return null;
		return objects [_id];
	}
		
	public void Hide() {
		GameObject _object;
		for (int i = 0; i < objects.Length; i++) {
			_object = objects[i];
			if (_object != null) {
				Bubble _bubble = _object.GetComponent<Bubble> ();
				_bubble.Hide ();
			}
		}
	}

}
