using UnityEngine;
using System;
using Random = UnityEngine.Random;
using DoozyUI;

public class ScreenGame : MonoBehaviour {
	BubbleField bubbleField;
	public GameObject screenAnimationObject;
	ScreenColorAnimation screenAnimation;
	public GameObject candyObject;
	public GameObject hintPerefab;
	public GameObject[] backgrounds;
	int currentBackgroundID = 0;
	GameObject backgroundPrev;
	GameObject backgroundNext;
	bool isBackgroundChange = false;
	Candy candy;
	Points points;
	Coins coins;
	BestScore bestScore;
	PointsBubbleManager poinsBMScript;
	GameObject hint = null;
	SpriteRenderer hintSprite;

	float time = 0f;
	bool isNextLevel;
	int state = -1;
	float missDelay = 0f;
	bool isGameOver = false;
	bool IsCanChangeColor = true;
	Vector3 camera_start_pos;

	public static event Action OnShowMenu;
	int hintCounter;
	bool isHint = false;

	bool isScreenReviveDone = false;
	bool isScreenShareDone = false;
	bool isScreenRateDone = false;

	bool isReviveUsed;

	AudioClip sndLose;
	AudioClip sndShowScreen;
	AudioClip sndGrab;
	AudioClip sndClose;

	string fail_reason;
	private bool _isWaitReward;
	private bool IsRewardedVideoReadyToShow;

	// Use this for initialization
	void Start () {
		Defs.audioSourceMusic = GetComponent<AudioSource> ();
		bubbleField = GetComponentInChildren<BubbleField> ();
		screenAnimation = screenAnimationObject.GetComponent<ScreenColorAnimation> ();
		GameObject _candyGO = (GameObject)Instantiate (candyObject, new Vector3 (0f, 0f, 0f), Quaternion.identity);
		candy = _candyGO.GetComponent<Candy> ();
		points = GetComponent<Points> ();
		coins = GetComponent<Coins> ();
		bestScore = GetComponent<BestScore> ();
		poinsBMScript = GetComponent<PointsBubbleManager> ();
		DefsGame.progressBar_x2 = transform.Find ("progressBar_x2").gameObject;
		state = 0;

		hintCounter = PlayerPrefs.GetInt ("hintCounter", 3);
		if (hintCounter >= 3) {
			isHint = true;
			hint = (GameObject)Instantiate (hintPerefab, new Vector3(0.3f, -1.0f,1), Quaternion.identity);
			hintSprite = hint.GetComponent<SpriteRenderer>();

			hint.SetActive (true);
		} 

		camera_start_pos = Camera.main.transform.position;

		sndLose = Resources.Load<AudioClip>("snd/fail");
		sndShowScreen = Resources.Load<AudioClip>("snd/showScreen");
		sndGrab = Resources.Load<AudioClip>("snd/grab");
		sndClose = Resources.Load<AudioClip>("snd/button");
	}

	void Init() {
		DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_FIRST_WIN, DefsGame.gameBestScore);

		++DefsGame.QUEST_GAMEPLAY_Counter;
		PlayerPrefs.SetInt ("QUEST_GAMEPLAY_Counter", DefsGame.QUEST_GAMEPLAY_Counter);

		DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_MASTER, DefsGame.QUEST_GAMEPLAY_Counter);

		GlobalEvents<OnAdsVideoTryShow>.Call(new OnAdsVideoTryShow());
		
		++DefsGame.gameplayCounter;
	
		PlayerPrefs.SetInt ("QUEST_THROW_CounterCounter", DefsGame.QUEST_THROW_Counter);

		DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_FiFIELD_OF_CANDIES, DefsGame.QUEST_THROW_Counter);

		// Сохраняемся тут, чтобы не тормозить игру
		PlayerPrefs.SetInt ("QUEST_BOMBS_Counter", DefsGame.QUEST_BOMBS_Counter);
		DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_EXPLOSIVE, DefsGame.QUEST_BOMBS_Counter);

		bubbleField.Init ();
		candy.Init ();
		points.UpdateVisual ();
		coins.UpdateVisual ();
		bestScore.UpdateVisual ();
		DefsGame.wowSlider.Init ();

		isNextLevel = false;

		isScreenReviveDone = false;
		isScreenShareDone = false;
		isScreenRateDone = false;

		isReviveUsed = false;

		fail_reason = "none";
	}

	void OnEnable() {
		Candy.OnTurn += Candy_OnTurn;
		Candy.OnMiss += Candy_OnMiss;
		Bubble.OnMatch += Bubble_OnMatch;
		GlobalEvents<OnGiveReward>.Happened += GetReward;
		GlobalEvents<OnRewardedVideoAvailable>.Happened += IsRewardedVideoAvailable;
	}

	void OnDisable() {
		Candy.OnTurn -= Candy_OnTurn;
		Candy.OnMiss -= Candy_OnMiss;
		Bubble.OnMatch -= Bubble_OnMatch;
		GlobalEvents<OnGiveReward>.Happened -= GetReward;
		GlobalEvents<OnRewardedVideoAvailable>.Happened -= IsRewardedVideoAvailable;
	}

	void Bubble_OnMatch (Bubble bubble)
	{
		if (isGameOver)
			return;
		D.Log ("ScreenGame.Bubble_OnMatch()");
		isNextLevel = true;
		int _pointsCount = DefsGame.bubbleMaxSize - bubble.GetStartSize () + 1;
		if (DefsGame.WOW_MEETERER_x2) {
			_pointsCount *= 2;
		} else 
			if (DefsGame.WOW_MEETERER_x3) {
				_pointsCount *= 3;
			}

		points.addPoint (_pointsCount);
		DefsGame.wowSlider.addPoints (_pointsCount);
		poinsBMScript.AddPoints (_pointsCount, bubble.id, bubble.transform.position);
	}

	void Candy_OnMiss (float delay, bool wrong_color)
	{
		if (isGameOver)
			return;

		Defs.PlaySound (sndLose);

		missDelay = delay;
		if (DefsGame.IS_ACHIEVEMENT_MISS_CLICK == 0) {
			++DefsGame.QUEST_MISS_Counter;
			PlayerPrefs.SetInt ("QUEST_MISS_Counter", DefsGame.QUEST_MISS_Counter);
			DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_MISS_CLICK, DefsGame.QUEST_MISS_Counter);
		}

		state = 3;

		if (wrong_color) fail_reason = "wrong_color"; else fail_reason = "missed";
	}
	
	private void IsRewardedVideoAvailable(OnRewardedVideoAvailable e) {
		IsRewardedVideoReadyToShow = e.isAvailable;
	}

	public void EndCurrentGame() {
		if (!isScreenReviveDone) {
			isScreenReviveDone = true;
			if (IsRewardedVideoReadyToShow && DefsGame.currentPointsCount >= Mathf.RoundToInt(DefsGame.gameBestScore * 0.5f)) {
				UIManager.ShowUiElement ("ScreenRevive");
				UIManager.ShowUiElement ("ScreenReviveBtnRevive");
				UIManager.ShowUiElement ("ScreenReviveBtnBack");
				D.Log ("isScreenReviveDone");
				Defs.PlaySound (sndShowScreen);

				FlurryEventsManager.SendEvent ("RV_revive_impression");
				return;
			}
		}

		if (!isScreenShareDone) {
			isScreenShareDone = true;
			if ((DefsGame.currentPointsCount >= 50) && (DefsGame.currentPointsCount == DefsGame.gameBestScore)) {
				UIManager.ShowUiElement ("ScreenShare");
				UIManager.ShowUiElement ("ScreenShareBtnShare");
				UIManager.ShowUiElement ("ScreenShareBtnBack");
				Defs.PlaySound (sndShowScreen);
				D.Log ("isScreenShareDone"); 

				FlurryEventsManager.SendEvent ("high_score_share_impression");
				return;
			}
		}

		if (!isScreenRateDone) {
			isScreenRateDone = true;
			if ((DefsGame.rateCounter < 3) && (DefsGame.currentPointsCount >= 100)
				&& (DefsGame.gameplayCounter % 20 == 0)) {
				++DefsGame.rateCounter;
				PlayerPrefs.SetInt ("rateCounter", DefsGame.rateCounter);
				UIManager.ShowUiElement ("ScreenRate");
				UIManager.ShowUiElement ("ScreenRateBtnRate");
				UIManager.ShowUiElement ("ScreenRateBtnBack");
				Defs.PlaySound (sndShowScreen);
				D.Log ("isScreenRateDone"); 

				FlurryEventsManager.SendEvent ("rate_us_impression", "revive_screen");
				return;
			}
		}

		state = 6;

//		PublishingService.Instance.ShowSceneTransition();
	}

	void Candy_OnTurn ()
	{
		if (isGameOver)
			return;

		++DefsGame.QUEST_THROW_Counter;

		if (DefsGame.gameplayCounter == 1) {
			points.ShowAnimation ();
		}
		if (state == 1) {
			DefsGame.currentScreen = DefsGame.SCREEN_GAME;
			points.ResetCounter ();
			UIManager.ShowUiElement ("scrMenuWowSlider");
			state = 2;
			FlurryEventsManager.SendStartEvent ("attempt_length");
		}

		isHint = false;

		// Даем разрешение на смену цвета
		IsCanChangeColor = true;
	}

	// Update is called once per frame
	void Update () {
		BtnEscapeUpdate ();
		BackgroundUpdate ();

		if (state == 0) {
			state = 1;
			Init ();
			return;
		}

		if ((!isHint)&&(hint)&&(hint.activeSelf)) {
			if (hintSprite.color.a > 0f) {
				Color _color = hintSprite.color;
				_color.a -= 0.05f;
				hintSprite.color = _color;
			} else {
				hint.SetActive (false);
			}
		}

		if (state == 1) {
			if (isHint) {
				if (hintSprite.color.a < 1f) {
					Color _color = hintSprite.color;
					_color.a += 0.05f;
					hintSprite.color = _color;
				}
			}
		} else if (state == 2) {
			if (isNextLevel) {
				bubbleField.NextLevel ();
				if (IsCanChangeColor) {
					candy.SetNextColor (bubbleField.GetRandomColor ());
					IsCanChangeColor = false;
				}
				isNextLevel = false;
			} else {
				if (!DefsGame.wowSlider.UpdateSlider ()) {
					GameOver ();
					return;
				}
			}
		} else if (state == 3) {
			time += Time.deltaTime;
			if (time >= missDelay) {
				time = 0f;
				screenAnimation.SetAlphaMax (0.93f);
				screenAnimation.SetAnimation (false, 0.1f);
				screenAnimation.Show ();
				screenAnimation.SetAnimation (true, 0.02f);
				screenAnimation.SetColor (1.0f, 0.21f, 0.21f);
				screenAnimation.SetAutoHide (true);

				state = 4;
			}
		} else if (state == 4) {
			if (!screenAnimation.isActiveAndEnabled) {
				state = 5;
				Camera.main.transform.position = new Vector3 (camera_start_pos.x,camera_start_pos.y, camera_start_pos.z);
				EndCurrentGame ();
			} else {
				Camera.main.transform.position = new Vector3 (camera_start_pos.x + Random.Range (-0.015f, 0.015f),
					camera_start_pos.y + Random.Range (-0.015f, 0.015f), camera_start_pos.z);
			}
		} else if (state == 5) {

		} else if (state == 6) {
			FlurryEventsManager.SendEndEvent ("attempt_length");
			FlurryEventsManager.SendEventPlayed (isReviveUsed, fail_reason);

			if ((DefsGame.gameBestScore == DefsGame.currentPointsCount)&&(DefsGame.gameBestScore != 0)) {
				DefsGame.gameServices.SubmitScore (DefsGame.gameBestScore);
				PlayerPrefs.SetInt ("BestScore", DefsGame.gameBestScore);
			}
			PlayerPrefs.SetInt ("coinsCount", DefsGame.coinsCount);

			HintCheck ();
			bubbleField.Hide ();
			candy.Hide ();
			isGameOver = false;
			NextBackground ();
			GameEvents.Send (OnShowMenu);
			DefsGame.currentScreen = DefsGame.SCREEN_MENU;
			state = 7;
		}else
			if (state == 7) {
				time += Time.deltaTime;
				if (time >= 0.8f) {
					time = 0f;
					Init ();
					state = 1;
				}
			}
	}

	void HintCheck(){
		if (DefsGame.currentPointsCount < 3) {
			++hintCounter;
			if (hintCounter >= 3) {
				isHint = true;
				if (!hint) {
					hint = (GameObject)Instantiate (hintPerefab, new Vector3 (0.3f, -1.0f, 1), Quaternion.identity);
					hintSprite = hint.GetComponent<SpriteRenderer> ();
				}
				Color _color = hintSprite.color;
				_color.a = 0;
				hintSprite.color = _color;
				hint.SetActive (true);
			} 
		} else {
			if (hintCounter != 0) {
				isHint = false;
				hintCounter = 0;
				PlayerPrefs.SetInt ("hintCounter", 0);
			}
		}
	}

	public void Revive() {
		FlurryEventsManager.SendEvent ("RV_revive");
		GlobalEvents<OnRewardedTryShow>.Call(new OnRewardedTryShow()); 
		_isWaitReward = true;
	}

	private void GetReward(OnGiveReward e)
	{
		if (_isWaitReward)
		{
			_isWaitReward = false;
			if (e.isAvailable)
			{
				state = 2;
				isNextLevel = true;
				isGameOver = false;
				isReviveUsed = true;
				DefsGame.wowSlider.MakeX3(1.1f);
				bubbleField.Hide();

				HideReviveScreen();
				Defs.PlaySound(sndGrab);

				FlurryEventsManager.SendEvent("RV_revive_complete");
			}
			else
			{
				HideReviveScreen();
				state = 6;
			}
		}
	}

	public void Share() {
		UIManager.HideUiElement ("ScreenShare");
		UIManager.HideUiElement ("ScreenShareBtnShare");
		UIManager.HideUiElement ("ScreenShareBtnBack");
		if (SystemInfo.deviceModel.Contains ("iPad")) {
			Defs.shareVoxel.ShareClick ();
		} else {
			Defs.share.ShareClick ();
		}
		FlurryEventsManager.SendEvent ("high_score_share");
		Defs.PlaySound (sndGrab);
		EndCurrentGame ();
	}


	public void Rate() {
		UIManager.HideUiElement ("ScreenRate");
		UIManager.HideUiElement ("ScreenRateBtnRate");
		UIManager.HideUiElement ("ScreenRateBtnBack");
		Defs.PlaySound (sndGrab);
		Defs.rate.RateUs ();
		FlurryEventsManager.SendEvent ("rate_us_impression", "revive_screen");
		EndCurrentGame ();
	}

	public void ReviveClose() {
		UIManager.HideUiElement ("ScreenRevive");
		UIManager.HideUiElement ("ScreenReviveBtnRevive");
		UIManager.HideUiElement ("ScreenReviveBtnBack");
		Defs.PlaySound (sndClose);
		EndCurrentGame ();

		FlurryEventsManager.SendEvent ("RV_revive_home");
	}

	public void ShareClose() {
		UIManager.HideUiElement ("ScreenShare");
		UIManager.HideUiElement ("ScreenShareBtnShare");
		UIManager.HideUiElement ("ScreenShareBtnBack");
		Defs.PlaySound (sndClose);
		EndCurrentGame ();

		FlurryEventsManager.SendEvent ("high_score_home");
	}


	public void RateClose() {
		UIManager.HideUiElement ("ScreenRate");
		UIManager.HideUiElement ("ScreenRateBtnRate");
		UIManager.HideUiElement ("ScreenRateBtnBack");
		Defs.PlaySound (sndClose);
		EndCurrentGame ();
	}

	void NextBackground() {
		isBackgroundChange = true;
		backgroundPrev = backgrounds [currentBackgroundID];
		++currentBackgroundID;
		if (currentBackgroundID >= backgrounds.Length)
			currentBackgroundID = 0;

		backgroundNext = backgrounds [currentBackgroundID];
		backgroundNext.SetActive (true);
		Color _color = backgroundPrev.GetComponent<SpriteRenderer> ().color;
		_color.a = 0;
		backgroundNext.GetComponent<SpriteRenderer> ().color = _color;
	}

	void BackgroundUpdate() {
		if (isBackgroundChange) {
			Color _color = backgroundPrev.GetComponent<SpriteRenderer> ().color;
			if (_color.a > 0) {
				_color.a -= 0.05f;
			}
			backgroundPrev.GetComponent<SpriteRenderer> ().color = _color;

			_color = backgroundNext.GetComponent<SpriteRenderer> ().color;
			if (_color.a < 1) {
				_color.a += 0.05f;
			}
			backgroundNext.GetComponent<SpriteRenderer> ().color = _color;

			if (_color.a >= 1) {
				isBackgroundChange = false;
				backgroundPrev.SetActive (false);
				backgroundPrev = null;
			}
		}
	}

	private void HideReviveScreen()
	{
		UIManager.HideUiElement ("ScreenRevive");
		UIManager.HideUiElement ("ScreenReviveBtnRevive");
		UIManager.HideUiElement ("ScreenReviveBtnBack");
	}

	void BtnEscapeUpdate() {
		/*if (InputController.IsTouchOnScreen(TouchPhase.Began)) {
			DefsGame.QUEST_BOMBS_Counter += 50;
			DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_EXPLOSIVE, DefsGame.QUEST_BOMBS_Counter);

			//if (DefsGame.QUEST_BOMBS_Counter % 100 == 0) {
				DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_FiFIELD_OF_CANDIES, DefsGame.QUEST_BOMBS_Counter);
			//}
		}*/

		//if (Input.GetKeyDown (KeyCode.A))
		if (InputController.IsEscapeClicked ())
		if (DefsGame.currentScreen == DefsGame.SCREEN_EXIT) {
			HideExitPanel ();
		}
		else	
		if (DefsGame.currentScreen == DefsGame.SCREEN_MENU) {
			ShowExitPanel ();
		}
		else if (DefsGame.currentScreen == DefsGame.SCREEN_GAME) {
			if (isScreenReviveDone)
				ReviveClose ();
			else if (isScreenShareDone)
				ShareClose ();
			else if (isScreenRateDone)
			RateClose (); else
				GameOver ();
		}else if (DefsGame.currentScreen == DefsGame.SCREEN_SKINS) {
			DefsGame.screenSkins.Hide ();
			bubbleField.Init ();
			GameEvents.Send (OnShowMenu);
		}else if (DefsGame.currentScreen == DefsGame.SCREEN_IAPS) {
			DefsGame.screenCoins.Hide ();
			bubbleField.Init ();
			GameEvents.Send (OnShowMenu);
		}
	}

	void GameOver() {
		isGameOver = true;
		candy.MakeActive (false);
		state = 3;

		fail_reason = "time_out";
	}

	public void HideExitPanel() {
		DefsGame.currentScreen = DefsGame.SCREEN_MENU;
		UIManager.HideUiElement ("PanelExit");
		UIManager.HideUiElement ("PanelExitBtnYes");
		UIManager.HideUiElement ("PanelExitBtnNo");
	}

	void ShowExitPanel() {
		DefsGame.currentScreen = DefsGame.SCREEN_EXIT;
		UIManager.ShowUiElement ("PanelExit");
		UIManager.ShowUiElement ("PanelExitBtnYes");
		UIManager.ShowUiElement ("PanelExitBtnNo");
	}

	public void Quit() {
		Application.Quit ();
	}
}
