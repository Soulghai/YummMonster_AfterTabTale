using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoinsIconScript : MonoBehaviour {
	public Text coinsText;
	Image image;
	//float startPosX;
	// Use this for initialization
	void Start () {
		DefsGame.coinsIcon = this;
		image = GetComponent<Image> ();
		//startPosX = transform.position.x;

		//transform.position = new Vector3 (coinsText.transform.position.x - 65 + coinsText.preferredWidth, transform.position.y,  transform.position.z);
		//StartCoroutine (WaitOneFrame ());
	}

	//public void UpdatePosition() {
		//StartCoroutine (WaitOneFrame ());
		//transform.position = new Vector3 (coinsText.transform.position.x - 65 + coinsText.preferredWidth, transform.position.y,  transform.position.z);
	//}

	//private IEnumerator WaitOneFrame()
	//{
		//Wait a frame until you can use the new width (sizeDelta.x) of the Label
		//yield return null;
		//transform.position = new Vector3 (coinsText.transform.position.x - coinsText.rectTransform.rect.width*0.5f + image.rectTransform.rect.width*0.6f + coinsText.preferredWidth, transform.position.y,  transform.position.z);
	//}
}
