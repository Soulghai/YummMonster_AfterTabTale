using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour {
	public GameObject fireworkSplash;


	float xSpeed;
	float ySpeed;
	float time = 0f;
	float lifeTime;
	SpriteRenderer spr;

	// Use this for initialization
	void Start () {
		xSpeed = (Random.value * 0.9f - Random.value * 0.9f)/64f; //* Defs.mobileScreenScaleFactor;
		ySpeed = (2.3f + Random.value * 1.7f)/64f;// * Defs.mobileScreenScaleFactor;
		float _size = 0.6f + Random.value*0.4f;
		transform.localScale = new Vector3(_size, _size, 1f);
		spr = GetComponent<SpriteRenderer> ();
		Color _color = spr.color;
		_color.a = 0;
		spr.color = _color;
		lifeTime = 0.5f + Random.value * 0.8f;
		transform.Rotate (0, 0, Mathf.Atan2 ((transform.position.y+ySpeed) - transform.position.y, (transform.position.x+xSpeed) - transform.position.x) * Mathf.Rad2Deg - 90f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (transform.position.x + xSpeed, transform.position.y + ySpeed);
		if (spr.color.a < 1f) {
			Color _color = spr.color;
			_color.a += 0.05f;
			spr.color = _color;
		}

		time += Time.deltaTime;
		if (time >= lifeTime) {
			Instantiate (fireworkSplash, transform.position, Quaternion.identity);
			Destroy (gameObject);
		}
	}
}
