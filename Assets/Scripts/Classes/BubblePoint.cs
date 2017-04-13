using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubblePoint : MonoBehaviour {
	Text text;
	float speedAlpha;

	// Use this for initialization
	void Start () {
		text = GetComponentInChildren<Text> ();
		speedAlpha = Random.Range (2.5f, 3.4f); 
	}
	
	// Update is called once per frame
	void Update () {


		//if (transform.localScale.x > 1f) {
			transform.localScale = new Vector3 (transform.localScale.x + 2.5f*Time.deltaTime, transform.localScale.x + 2.5f*Time.deltaTime, 1);
		//}

		transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f*Time.deltaTime, 1);

		Color _color = text.color;
		if (text.color.a > 0) {
			_color.a -= speedAlpha*Time.deltaTime;
			text.color = _color;
		} else
			Destroy (gameObject);
	}
}
