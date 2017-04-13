using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DoozyUI;
using System;
using Random = UnityEngine.Random;

public class ScreenSkins : MonoBehaviour {
	public static event Action<int> OnAddCoinsVisual;
	public GameObject skin2;
	public GameObject skin3;
	public GameObject skin4;
	public GameObject skin5;
	public GameObject skin6;
	public GameObject skin7;
	public GameObject skin8;
	public GameObject skin9;
	public GameObject skin10;
	public GameObject skin11;
	public GameObject skin12;

	public GameObject haveNewSkin;

	void Awake () {
		DefsGame.screenSkins = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetSkin(int _id) {
		FlurryEventsManager.SendEvent ("candy_purchase_<" + _id.ToString() + ">");

		if (_id == DefsGame.currentFaceID)
			return;

		if (DefsGame.faceAvailable [_id] == 1) {
			DefsGame.currentFaceID = _id;
			PlayerPrefs.SetInt ("currentFaceID", DefsGame.currentFaceID);
			DefsGame.candy.SetNewSkin (_id);
		} else if (DefsGame.coinsCount >= DefsGame.facePrice [_id-1]) {
			//DefsGame.coinsIcon.UpdatePosition ();
			GameEvents.Send(OnAddCoinsVisual, -DefsGame.facePrice [_id-1]);
			DefsGame.faceAvailable [_id] = 1;
			DefsGame.currentFaceID = _id;
			PlayerPrefs.SetInt ("currentFaceID", DefsGame.currentFaceID);
			PlayerPrefs.SetInt ("faceAvailable_" + _id.ToString (), 1);
			DefsGame.candy.SetNewSkin (_id);

			++DefsGame.QUEST_CHARACTERS_Counter;
			PlayerPrefs.SetInt ("QUEST_CHARACTERS_Counter", DefsGame.QUEST_CHARACTERS_Counter);

			DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_NEW_SKIN, 1);

			DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_COLLECTION, DefsGame.QUEST_CHARACTERS_Counter);

			ChooseColorForButtons ();

			FlurryEventsManager.SendEvent ("candy_purchase_completed_<" + _id.ToString() + ">");
		} else {
			HideButtons ();
			FlurryEventsManager.SendEndEvent ("candy_shop_length");

			DefsGame.screenCoins.Show ("candy_shop");
		}
	}

	void ShowButtons() {
		UIManager.ShowUiElement ("scrSkinsBtnBack");
		UIManager.ShowUiElement ("BtnSkin1");
		UIManager.ShowUiElement ("BtnSkin2");
		UIManager.ShowUiElement ("BtnSkin3");
		UIManager.ShowUiElement ("BtnSkin4");
		UIManager.ShowUiElement ("BtnSkin5");
		UIManager.ShowUiElement ("BtnSkin6");
		UIManager.ShowUiElement ("BtnSkin7");
		UIManager.ShowUiElement ("BtnSkin8");
		UIManager.ShowUiElement ("BtnSkin9");
		UIManager.ShowUiElement ("BtnSkin10");
		UIManager.ShowUiElement ("BtnSkin11");
		UIManager.ShowUiElement ("BtnSkin12");
	}

	void HideButtons() {
		UIManager.HideUiElement ("scrSkinsBtnBack");
		UIManager.HideUiElement ("BtnSkin1");
		UIManager.HideUiElement ("BtnSkin2");
		UIManager.HideUiElement ("BtnSkin3");
		UIManager.HideUiElement ("BtnSkin4");
		UIManager.HideUiElement ("BtnSkin5");
		UIManager.HideUiElement ("BtnSkin6");
		UIManager.HideUiElement ("BtnSkin7");
		UIManager.HideUiElement ("BtnSkin8");
		UIManager.HideUiElement ("BtnSkin9");
		UIManager.HideUiElement ("BtnSkin10");
		UIManager.HideUiElement ("BtnSkin11");
		UIManager.HideUiElement ("BtnSkin12");
	}

	public void SetSkin1() {
		SetSkin (0);
	}
		
	public void SetSkin2() {
		SetSkin (1);
	}

	public void SetSkin3() {
		SetSkin (2);
	}

	public void SetSkin4() {
		SetSkin (3);
	}

	public void SetSkin5() {
		SetSkin (4);
	}

	public void SetSkin6() {
		SetSkin (5);
	}

	public void SetSkin7() {
		SetSkin (6);
	}

	public void SetSkin8() {
		SetSkin (7);
	}

	public void SetSkin9() {
		SetSkin (8);
	}

	public void SetSkin10() {
		SetSkin (9);
	}

	public void SetSkin11() {
		SetSkin (10);
	}

	public void SetSkin12() {
		SetSkin (11);
	}

	public void Show() {
		FlurryEventsManager.SendStartEvent ("candy_shop_length");

		DefsGame.currentScreen = DefsGame.SCREEN_SKINS;
		DefsGame.isCanPlay = false;
		ChooseColorForButtons ();
		ShowButtons ();
	}

	public void Hide() {
		FlurryEventsManager.SendEndEvent ("candy_shop_length");

		DefsGame.currentScreen = DefsGame.SCREEN_MENU;
		DefsGame.isCanPlay = true;
		ChooseColorForButtons ();
		HideButtons ();
	}

	public void ChooseColorForButtons() {
		if (DefsGame.faceAvailable [1] == 1) {
			skin2.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.white; 
			skin2.GetComponentInChildren<Text> ().text = "";
		} else {
			skin2.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.black;
			skin2.GetComponentInChildren<Text> ().text = DefsGame.facePrice [0].ToString ();
		}
		if (DefsGame.faceAvailable[2] == 1) {
			skin3.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.white; 
			skin3.GetComponentInChildren<Text> ().text = "";
		} else {
			skin3.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.black;
			skin3.GetComponentInChildren<Text> ().text = DefsGame.facePrice [1].ToString ();
		}
		if (DefsGame.faceAvailable[3] == 1) {
			skin4.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.white; 
			skin4.GetComponentInChildren<Text> ().text = "";
		} else {
			skin4.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.black;
			skin4.GetComponentInChildren<Text> ().text = DefsGame.facePrice [2].ToString ();
		}
		if (DefsGame.faceAvailable[4] == 1) {
			skin5.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.white; 
			skin5.GetComponentInChildren<Text> ().text = "";
		} else {
			skin5.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.black;
			skin5.GetComponentInChildren<Text> ().text = DefsGame.facePrice [3].ToString ();
		}
		if (DefsGame.faceAvailable[5] == 1) {
			skin6.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.white; 
			skin6.GetComponentInChildren<Text> ().text = "";
		} else {
			skin6.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.black;
			skin6.GetComponentInChildren<Text> ().text = DefsGame.facePrice [4].ToString ();
		}
		if (DefsGame.faceAvailable[6] == 1) {
			skin7.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.white; 
			skin7.GetComponentInChildren<Text> ().text = "";
		} else {
			skin7.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.black;
			skin7.GetComponentInChildren<Text> ().text = DefsGame.facePrice [5].ToString ();
		}
		if (DefsGame.faceAvailable[7] == 1) {
			skin8.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.white; 
			skin8.GetComponentInChildren<Text> ().text = "";
		} else {
			skin8.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.black;
			skin8.GetComponentInChildren<Text> ().text = DefsGame.facePrice [6].ToString ();
		}
		if (DefsGame.faceAvailable[8] == 1) {
			skin9.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.white; 
			skin9.GetComponentInChildren<Text> ().text = "";
		} else {
			skin9.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.black;
			skin9.GetComponentInChildren<Text> ().text = DefsGame.facePrice [7].ToString ();
		}
		if (DefsGame.faceAvailable[9] == 1) {
			skin10.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.white; 
			skin10.GetComponentInChildren<Text> ().text = "";
		} else {
			skin10.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.black;
			skin10.GetComponentInChildren<Text> ().text = DefsGame.facePrice [8].ToString ();
		}
		if (DefsGame.faceAvailable[10] == 1) {
			skin11.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.white; 
			skin11.GetComponentInChildren<Text> ().text = "";
		} else {
			skin11.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.black;
			skin11.GetComponentInChildren<Text> ().text = DefsGame.facePrice [9].ToString ();
		}
		if (DefsGame.faceAvailable[11] == 1) {
			skin12.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.white; 
			skin12.GetComponentInChildren<Text> ().text = "";
		} else {
			skin12.GetComponentInChildren<UIButton> ().GetComponent<Image>().color = Color.black;
			skin12.GetComponentInChildren<Text> ().text = DefsGame.facePrice [10].ToString ();
		}
	}

	public void CheckAvailableSkin() {
		for (int i = 1; i < DefsGame.faceAvailable.Length; i++) {
			if ((DefsGame.faceAvailable[i] == 0)&&(DefsGame.coinsCount >= DefsGame.facePrice[i-1])) {
				haveNewSkin.SetActive (true);
				//return true;
				return;
			}
		}
		haveNewSkin.SetActive (false);
		//return false;
	}

	public bool CheckAvailableSkinBool() {
		for (int i = 1; i < DefsGame.faceAvailable.Length; i++) {
			if ((DefsGame.faceAvailable[i] == 0)&&(DefsGame.coinsCount >= DefsGame.facePrice[i-1])) {
				haveNewSkin.SetActive (true);
				return true;
			}
		}
		haveNewSkin.SetActive (false);
		return false;
	}
}
