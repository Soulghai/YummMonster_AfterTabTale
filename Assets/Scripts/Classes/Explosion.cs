using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	public float delay = 0.1f;
	float time = 0f;
	PointEffector2D pointEffector;
	Transform visual;
	float startScale = 1f;
	SpriteRenderer spr;

	// Use this for initialization
	void Start () {
		pointEffector = GetComponent<PointEffector2D> ();
		visual = transform.Find ("Sprite");
		spr = GetComponentInChildren<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time >= delay) {
			pointEffector.enabled = false;
		}

		if (spr.color.a > 0) {
			Color _color = spr.color;
			_color.a -= 0.1f;
			spr.color = _color;
		}

		if (visual.localScale.x < startScale*4f) {
			visual.localScale = new Vector3 (visual.localScale.x + 0.3f, visual.localScale.y + 0.3f, visual.localScale.z);
		} else {
			Destroy (gameObject);
		}
	}

	public void SetScale(float _value) {
		startScale = _value;
	}
}
