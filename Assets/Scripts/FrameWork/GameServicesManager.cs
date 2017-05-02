using UnityEngine;
using VoxelBusters.NativePlugins;
	
public class GameServicesManager : MonoBehaviour {

	string GLOBAL_LEADERBOARD_ID = "First";

	[HideInInspector] public string ACHIEVEMENT_FIRST_WIN 			= "Beginner";

	[HideInInspector] public string ACHIEVEMENT_NEW_SKIN 			= "Newskin";
	[HideInInspector] public string ACHIEVEMENT_MULTI_PULTI 		= "MULTI_PULTI";
	[HideInInspector] public string ACHIEVEMENT_MISS_CLICK 			= "MISS_CLICK";
	[HideInInspector] public string ACHIEVEMENT_GET_MAX 			= "GET_MAX";
	[HideInInspector] public string ACHIEVEMENT_THREE_JUMPS 		= "THREE_JUMPS";

	[HideInInspector] public string ACHIEVEMENT_MASTER 				= "MASTER";
	[HideInInspector] public string ACHIEVEMENT_FiFIELD_OF_CANDIES 	= "FieldOfCandies";
	[HideInInspector] public string ACHIEVEMENT_EXPLOSIVE 			= "EXPLOSIVE";
	[HideInInspector] public string ACHIEVEMENT_COLLECTION 			= "COLLECTION";

	void Awake() {
		DefsGame.gameServices = this;
		D.Log("Game Services Start()");

		if (!IsServiceAvailable ()) {
			D.Log ("Sorry, Game Services feature is not supported on this device.");
			return;
		} else
			D.Log ("Game Services feature is supported on this device.");

		Invoke("AutoLogin", 1.0f);
	}

	private void AutoLogin()
	{
		if (!IsAuthenticated ())
			AuthenticateUser (true);
	}

	bool IsServiceAvailable()
	{
//		return NPBinding.GameServices.IsAvailable();
	    return false;
	}

	bool IsAuthenticated()
	{
//		return NPBinding.GameServices.LocalUser.IsAuthenticated;
	    return false;
	}

	// LOG IN / LOG OUT

	public void BtnLogInLogOutClick() {
		if (!IsServiceAvailable ())
			return;
		
		if (IsAuthenticated ()) {
			SignOut ();
		} else {
			AuthenticateUser ();
		}
	}

	void AuthenticateUser (bool _isFirstTime = false)
	{
		Debug.Log ("AuthenticateUser()");

		if (!_isFirstTime) {
			#if UNITY_IOS
			Invoke ("ShowGameCenterFailed", 2f);
			#endif
		}

		NPBinding.GameServices.LocalUser.Authenticate((bool _success, string _error)=>{

			Debug.Log("Local user authentication finished.");
			//Debug.Log(string.Format("Error= {0}.", _error.ToString()));

			#if UNITY_IOS
			CancelInvoke ("ShowGameCenterFailed");
			#endif

			if (_success)
			{
				Debug.Log("LogIn - success");
				LoadAchievements();
				LoadAchievementsDescription();
				//Debug.Log(string.Format("Local user details= {0}.", NPBinding.GameServices.LocalUser));
			} else {
				if (!_isFirstTime) {
					#if UNITY_IOS
					ShowGameCenterFailed();
					#endif
				}
				Debug.Log("LogIn - error:" + _error);
			}
		});
	}

	void SignOut ()
	{
		NPBinding.GameServices.LocalUser.SignOut((bool _success, string _error)=>{

			if (_success)
			{
				D.Log("Local user is signed out successfully!");
			}
			else
			{
				D.Log("Request to signout local user failed.");
				//Debug.Log(string.Format("Error= {0}. " +  _error.ToString()));
			}
		});
	}

	#if UNITY_IOS

	public void ShowGameCenterFailed ()
	{
		if (!IsAuthenticated())
		{
			NPBinding.UI.ShowAlertDialogWithSingleButton("Game Center", "You can log in to Game Center in Settings -> Game Center", "Ok", (string _buttonPressed) => {});
		}
	}
	#endif

	// LEADERBOARD

	public void SubmitScore(int SCORE_TO_REPORT) {
		if (!IsServiceAvailable () || (!IsAuthenticated()))
			return;
		
		NPBinding.GameServices.ReportScoreWithGlobalID(GLOBAL_LEADERBOARD_ID, SCORE_TO_REPORT, (bool _success, string _error)=>{

			if (_success)
			{
				//Debug.Log(string.Format("Request to report score to leaderboard with GID= {0} finished successfully.", GLOBAL_LEADERBOARD_ID));
				D.Log(string.Format("SubmitScore() => "+ SCORE_TO_REPORT));
			}
			else
			{
				//Debug.Log(string.Format("Request to report score to leaderboard with GID= {0} failed.", GLOBAL_LEADERBOARD_ID));
				D.Log(string.Format("Error= " + _error));
			}
		});
	}

	public void ShowLeaderboard() {
		Debug.Log ("ShowLeaderboard - TRY TO SHOW");
		if (!IsServiceAvailable ())
			return;
		Debug.Log ("ShowLeaderboard - IsServiceAvailable");
		if (!IsAuthenticated ()) {
			Debug.Log ("ShowLeaderboard - !IsAuthenticated");
			AuthenticateUser ();
		}

		Debug.Log("Game Services ShowLeaderboard()");

		//Show Leaderboards UI
		NPBinding.GameServices.ShowLeaderboardUIWithGlobalID (GLOBAL_LEADERBOARD_ID, eLeaderboardTimeScope.ALL_TIME, (string _error) => {
			D.Log ("Closed leaderboard UI." + _error);
		});

		//eLeaderboardTimeScope.TODAY    // Fetch leaderboards existing TODAY
		//eLeaderboardTimeScope.WEEK     // Fetch leaderboards existing Past WEEK
		//eLeaderboardTimeScope.ALL_TIME // Fetch All Leaderboards

		FlurryEventsManager.SendEvent ("leaderboard");
	}

	// ACHIEVEMENtS

	void LoadAchievements() {
		NPBinding.GameServices.LoadAchievements((Achievement[] _achievements, string _error)=>{

			if (_achievements == null)
			{
				D.Log("Couldn't load achievement list with error = " + _error);
				return;
			}

			int        _achievementCount    = _achievements.Length;
			D.Log(string.Format("Successfully loaded achievement list. Count = " + _achievementCount));

			for (int _iter = 0; _iter < _achievementCount; _iter++)
			{
				D.Log(string.Format("[Index {0}]: {1}", _iter, _achievements[_iter]));
			}
		});
	}

	void LoadAchievementsDescription() {
		NPBinding.GameServices.LoadAchievementDescriptions((AchievementDescription[] _descriptions, string _error)=>{

			if (_descriptions == null)
			{
				D.Log("Couldn't load achievement description list with error = " + _error);
				return;
			}

			int        _descriptionCount    = _descriptions.Length;
			D.Log(string.Format("Successfully loaded achievement description list. Count={0}.", _descriptionCount));

			for (int _iter = 0; _iter < _descriptionCount; _iter++)
			{
				D.Log(string.Format("[Index {0}]: {1}", _iter, _descriptions[_iter]));
			}
		});
	}

	public void ShowAchievements() {
		if (!IsServiceAvailable ())
			return;

		if (!IsAuthenticated ())
			AuthenticateUser ();


		NPBinding.GameServices.ShowAchievementsUI((string _error) =>
			{
				D.Log("ShowAchievements()" +  _error) ;
			});
		FlurryEventsManager.SendEvent ("achievements");
	}

	bool UpdateAchievementsInfo (string _achievementGID) {
		if ((_achievementGID == ACHIEVEMENT_FIRST_WIN) && (DefsGame.IS_ACHIEVEMENT_FIRST_WIN != 1))
			return true;
		if ((_achievementGID == ACHIEVEMENT_NEW_SKIN) && (DefsGame.IS_ACHIEVEMENT_NEW_SKIN != 1))
			return true;
		else
			if ((_achievementGID == ACHIEVEMENT_MULTI_PULTI) && (DefsGame.IS_ACHIEVEMENT_MULTI_PULTI != 1))
				return true;
			else
				if ((_achievementGID == ACHIEVEMENT_MISS_CLICK) && (DefsGame.IS_ACHIEVEMENT_MISS_CLICK != 1))
					return true;
				else
					if ((_achievementGID == ACHIEVEMENT_GET_MAX) && (DefsGame.IS_ACHIEVEMENT_GET_MAX != 1))
						return true;
					else
						if ((_achievementGID == ACHIEVEMENT_THREE_JUMPS) && (DefsGame.IS_ACHIEVEMENT_THREE_JUMPS != 1))
							return true;
						
		else if ((_achievementGID == ACHIEVEMENT_MASTER) && (DefsGame.IS_ACHIEVEMENT_MASTER != 1))
			return true;
		else if ((_achievementGID == ACHIEVEMENT_FiFIELD_OF_CANDIES) && (DefsGame.IS_ACHIEVEMENT_FiFIELD_OF_CANDIES != 1))
			return true;
		else if ((_achievementGID == ACHIEVEMENT_EXPLOSIVE) && (DefsGame.IS_ACHIEVEMENT_EXPLOSIVE != 1))
			return true;
		else if ((_achievementGID == ACHIEVEMENT_COLLECTION) && (DefsGame.IS_ACHIEVEMENT_COLLECTION != 1))
			return true;

		return false;
	}

	public void ReportProgressWithGlobalID (string _achievementGID, float _value)
	{
		bool _needUpdate = UpdateAchievementsInfo (_achievementGID);


		if (!((_needUpdate)&&(IsServiceAvailable () && IsAuthenticated())))
			return;

		double	_progress = 0;

		//int 	_noOfSteps	= NPBinding.GameServices.GetNoOfStepsForCompletingAchievement(_achievementGID);
		float 	_noOfSteps = 0f;
		if (_achievementGID == ACHIEVEMENT_FIRST_WIN) {
			if (_value >= 25f) {
				_noOfSteps = 1f;
				_progress = 100;
			}
		} else if (_achievementGID == ACHIEVEMENT_NEW_SKIN) {
			if (_value >= 1f) {
				_noOfSteps = 1f;
				_progress = 100;
			}
		} else if (_achievementGID == ACHIEVEMENT_MULTI_PULTI) {
			if (_value >= 1f) {
				_noOfSteps = 1f;
				_progress = 100;
			}
		} else if (_achievementGID == ACHIEVEMENT_MISS_CLICK) {
			if (_value >= 50f) {
				_noOfSteps = 1f;
				_progress = 100;
			}
		} else if (_achievementGID == ACHIEVEMENT_GET_MAX) {
			if (_value >= 1f) {
				_noOfSteps = 1f;
				_progress = 100;
			}
		} else if (_achievementGID == ACHIEVEMENT_THREE_JUMPS) {
			if (_value >= 1f) {
				_noOfSteps = 1f;
				_progress = 100;
			}
		}else if (_achievementGID == ACHIEVEMENT_MASTER) {
			if (_value >= 100f) {
				_noOfSteps = 1f;
				_progress = 100;
			}
		} else if (_achievementGID == ACHIEVEMENT_FiFIELD_OF_CANDIES) {
			if (_value >= 1000f) {
				_noOfSteps = 1f;
				_progress = 100;
			}
		} else if (_achievementGID == ACHIEVEMENT_EXPLOSIVE) {
			if (_value >= 100f) {
				_noOfSteps = 1f;
				_progress = 100;
			}
		} else if (_achievementGID == ACHIEVEMENT_COLLECTION) {
			if (_value >= 5f) {
				_noOfSteps = 1f;
				_progress = 100;
			}
		}
			
		if (_noOfSteps == 0f)
			return;

		if (_progress < 100) {
			if (_value >= _noOfSteps) {
				_value = _noOfSteps;
				_progress = 100;
			} else
				_progress = (_value / _noOfSteps) * 100;
		}

		if (_progress >= 100)
		// If its an incremental achievement, make sure you send a incremented cumulative value everytime you call this method
		NPBinding.GameServices.ReportProgressWithGlobalID(_achievementGID, _progress, (bool _status, string _error)=>{

			if (_status)
			{
				if (_progress >= 100) {
					D.Log("ACHIEVEMENT UNLOCKED progress =",_progress, "ID = ", _achievementGID);
					if (_achievementGID == ACHIEVEMENT_FIRST_WIN) {
						DefsGame.IS_ACHIEVEMENT_FIRST_WIN = 1;
						PlayerPrefs.SetInt("IS_ACHIEVEMENT_FIRST_WIN", 1);
					}else
							if (_achievementGID == ACHIEVEMENT_NEW_SKIN) {
								DefsGame.IS_ACHIEVEMENT_NEW_SKIN = 1;
								PlayerPrefs.SetInt("IS_ACHIEVEMENT_NEW_SKIN", 1);
							}else
								if (_achievementGID == ACHIEVEMENT_MULTI_PULTI) {
									DefsGame.IS_ACHIEVEMENT_MULTI_PULTI = 1;
									PlayerPrefs.SetInt("IS_ACHIEVEMENT_MULTI_PULTI", 1);
								}else
									if (_achievementGID == ACHIEVEMENT_MISS_CLICK) {
										DefsGame.IS_ACHIEVEMENT_MISS_CLICK = 1;
										PlayerPrefs.SetInt("IS_ACHIEVEMENT_MISS_CLICK", 1);
									}else
										if (_achievementGID == ACHIEVEMENT_GET_MAX) {
											DefsGame.IS_ACHIEVEMENT_GET_MAX = 1;
											PlayerPrefs.SetInt("IS_ACHIEVEMENT_GET_MAX", 1);
										}else
											if (_achievementGID == ACHIEVEMENT_THREE_JUMPS) {
												DefsGame.IS_ACHIEVEMENT_THREE_JUMPS = 1;
												PlayerPrefs.SetInt("IS_ACHIEVEMENT_THREE_JUMPS", 1);
											}else
						if (_achievementGID == ACHIEVEMENT_MASTER) {
							DefsGame.IS_ACHIEVEMENT_MASTER = 1;
							PlayerPrefs.SetInt("IS_ACHIEVEMENT_MASTER", 1);
						} else
							if (_achievementGID == ACHIEVEMENT_FiFIELD_OF_CANDIES) {
								DefsGame.IS_ACHIEVEMENT_FiFIELD_OF_CANDIES = 1;
								PlayerPrefs.SetInt("IS_ACHIEVEMENT_FiFIELD_OF_CANDIES", 1);
							}else
								if (_achievementGID == ACHIEVEMENT_EXPLOSIVE) {
									DefsGame.IS_ACHIEVEMENT_EXPLOSIVE = 1;
									PlayerPrefs.SetInt("IS_ACHIEVEMENT_EXPLOSIVE", 1);
								}else
									if (_achievementGID == ACHIEVEMENT_COLLECTION) {
										DefsGame.IS_ACHIEVEMENT_COLLECTION = 1;
										PlayerPrefs.SetInt("IS_ACHIEVEMENT_COLLECTION", 1);
									}
				}


				//Debug.Log("Request to report progress of achievement with GID= {0} finished successfully. " + _achievementGID);
					D.Log(string.Format("ReportProgressWithGlobalID() - achievement = " + _achievementGID + " Percentage completed = "+ _progress));
			}
			else
			{
				//Debug.Log(string.Format("Request to report progress of achievement with GID= {0} failed. " + _achievementGID));
				D.Log(string.Format("Error= " + _error));
			}
		});
	}

	bool IsAchievementAlreadyUnlocked (string globalId, Achievement[] achievements)
	{
		if (achievements == null) return false;

		foreach (Achievement achievement in achievements)
		{
			if (achievement.GlobalIdentifier == globalId)
			{
				return achievement.Completed;
			}
		}

		return false;
	}
}
