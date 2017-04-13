using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsBubbleManager : MonoBehaviour {
	public GameObject pointsObject;
	// Use this for initialization
	bool isAddPoint = false;
	float time = 0;
	float delay = 0.2f;
	int count = 0;
	int colorID;
	Vector3 pos;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isAddPoint) {
			time += Time.deltaTime;
			if (time >= delay) {
				isAddPoint = false;
				time = 0;
				GameObject _go = (GameObject)Instantiate (pointsObject, pos, Quaternion.identity);
				Text _text = _go.GetComponentInChildren<Text> ();
				_text.text = "+" + count.ToString ();
				/*
				Utils.stringBuilder.Length = 0;
				Utils.stringBuilder.Append("+");
				Utils.stringBuilder.Append (count);
				_text.text = Utils.stringBuilder.ToString ();
				*/
				if (DefsGame.WOW_MEETERER_x3)
					_go.transform.localScale = new Vector3 (_go.transform.localScale.x * 2f, _go.transform.localScale.x * 2f, 1);
				else if (DefsGame.WOW_MEETERER_x2)
					_go.transform.localScale = new Vector3 (_go.transform.localScale.x * 1.5f, _go.transform.localScale.x * 1.5f, 1);
				else
					_go.transform.localScale = new Vector3 (_go.transform.localScale.x * 0.5f, _go.transform.localScale.x * 1.0f, 1);

				if (colorID == DefsGame.BUBBLE_COLOR_ONE)
					_text.color = new Color (63f / 255f, 131f / 255f, 233f / 255f);
				else if (colorID == DefsGame.BUBBLE_COLOR_TWO)
					_text.color = new Color (236f / 255f, 49f / 255f, 69f / 255f);
				else if (colorID == DefsGame.BUBBLE_COLOR_THREE)
					_text.color = new Color (52f / 255f, 160f / 255f, 103f / 255f);
				else if (colorID == DefsGame.BUBBLE_COLOR_FOUR)
					_text.color = new Color (151f / 255f, 88f / 255f, 154f / 255f);
				else if (colorID == DefsGame.BUBBLE_COLOR_HEAVY)
					_text.color = new Color (116f / 255f, 128f / 255f, 161f / 255f);
				else if (colorID == DefsGame.BUBBLE_COLOR_TIMER)
					_text.color = new Color (73f / 255f, 187f / 255f, 177f / 255f);
				else
					_text.color = new Color (228f, 152f, 254f);
			}
		}
	}

	public void AddPoints(int _count, int _colorID, Vector3 _pos) {
		isAddPoint = true;
		colorID = _colorID;
		count = _count;
		pos = _pos;
	}
}
