using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyObject : MonoBehaviour {
	float currentAngle;
	float distance;
	float speed;
	// Use this for initialization
	void Start () {
		distance = Vector3.Distance (new Vector3(0, -9.97f, transform.position.z),  transform.position); 
		currentAngle = Mathf.Atan2(-9.97f-transform.position.y, transform.position.x) * Mathf.Rad2Deg;
		speed = 0.02f + Random.Range (0.01f, 0.03f)*DefsGame.WOW_MEETERER;
	}
	
	// Update is called once per frame
	void Update () {
		currentAngle -= speed;
		transform.position = new Vector3 (distance * Mathf.Cos(currentAngle * Mathf.Deg2Rad),
			-9.97f - distance * Mathf.Sin (currentAngle * Mathf.Deg2Rad), 1f);

		if (transform.position.x < -2.6f) {
			Destroy (gameObject);
		}
	}
}
