using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour {
	public GameObject firework;
	public GameObject star;
	public GameObject flyObject;
	public GameObject flyObject2;
	public GameObject flyObject3;

	// Use this for initialization
	void Start () {
		StartCoroutine (spawnFirework ());
		StartCoroutine (spawnStar ());
		StartCoroutine (spawnFlyObject ());
	}

	IEnumerator spawnFirework() {
		while (true) {
			Vector3 _pos = new Vector3(Random.Range(-1.5f, 1.5f), -0.9f, Camera.main.farClipPlane/2f);
			Instantiate (firework, _pos, Quaternion.identity);
			yield return new WaitForSeconds (3f + Random.value*5.0f*(1-DefsGame.WOW_MEETERER));
		}
	}

	IEnumerator spawnStar() {
		while (true) {
			Vector3 _pos = new Vector3(Random.Range(-1.77f, 1.77f), Random.Range(-0.7f,3.2f), Camera.main.farClipPlane/2f);
			GameObject _gameObject = (GameObject)Instantiate (star, _pos, Quaternion.identity);
			float _scale = _gameObject.transform.localScale.x + Random.Range (-0.3f, 0.3f);
			_gameObject.transform.localScale = new Vector3(_scale, _scale, 1f);
			yield return new WaitForSeconds (0.4f + Random.value*0.8f*(1-DefsGame.WOW_MEETERER));
		}
	}

	IEnumerator spawnFlyObject() {
		while (true) {
			Vector3 _pos = new Vector3( 2.6f, Random.Range(-0.7f,2.6f), Camera.main.farClipPlane/2f);
			GameObject _gameObject = null;
			float _ran = Random.value;
			if (_ran <= 0.333f) 
			_gameObject = (GameObject)Instantiate (flyObject, _pos, Quaternion.identity); else
				if (_ran <= 0.666f)
				_gameObject = (GameObject)Instantiate (flyObject2, _pos, Quaternion.identity); else
					_gameObject = (GameObject)Instantiate (flyObject3, _pos, Quaternion.identity);
			float _scale = _gameObject.transform.localScale.x + Random.Range (-0.3f, 0.0f);
			_gameObject.transform.localScale = new Vector3(_scale, _scale, 1f);
			yield return new WaitForSeconds (11.0f + Random.value*6f*(1-DefsGame.WOW_MEETERER));
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
