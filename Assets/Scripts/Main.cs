using UnityEngine;

public class Main : MonoBehaviour {
	void Awake() {
		Application.targetFrameRate = 60;
		DefsGame.loadVariables ();
		Defs.audioSource = GetComponent<AudioSource> ();
	}
}
