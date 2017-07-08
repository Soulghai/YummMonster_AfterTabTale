using UnityEngine;

public class DestroyByTime : MonoBehaviour {
	public float lifeTime = 3f;

	void Awake() {
		Destroy (gameObject,lifeTime);
	}
}
