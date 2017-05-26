using UnityEngine;

public struct DefsGame {

	public static int noAds = 0;
	public static BillingManager IAPs;
	public static GameServicesManager gameServices;
	public static ScreenCoins screenCoins;
	public static ScreenSkins screenSkins;
	public static CoinsIconScript coinsIcon;
	public static Candy candy;
	public static BubbleField bubbleField;
	public static WowSlider wowSlider;
	public static float WOW_MEETERER				= 0;
	public static bool WOW_MEETERER_x2 = false;
	public static bool WOW_MEETERER_x3 = false;
	public static GameObject progressBar_x2;
	public static int gameplayCounter 				= 0;		// Считает количество игр сыгранных в этой игровой сессии

	public static int currentPointsCount			= 0;
	public static int gameBestScore					= 0;		// Лучший счет
	public static int coinsCount					= 0;		// Количество очков игрока
	public static int currentFaceID = 0;
	public static bool isCanPlay = true;
	public static readonly int bubbleMaxSize = 5;
	public static int[] faceAvailable = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
	public static readonly int[] facePrice = new int[] { 50, 100, 150, 200, 250, 300, 350, 400, 450, 500, 550, 600 };
	public static int BTN_GIFT_HIDE_DELAY 			= 0;
	public static int BTN_GIFT_HIDE_DELAY_COUNTER	= 0;
	public static readonly int[] BTN_GIFT_HIDE_DELAY_ARR = new int[] {1, 2, 5, 10, 15, 20, 25, 30, 60};
	//public static readonly int[] BTN_GIFT_HIDE_DELAY_ARR = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0};



	public static int currentScreen = 0;
	public static int SCREEN_MENU = 0;
	public static int SCREEN_GAME = 1;
	public static int SCREEN_SKINS = 2;
	public static int SCREEN_IAPS = 3;
	public static int SCREEN_EXIT = 10;


	public static int IS_ACHIEVEMENT_FIRST_WIN = 0;
	public static int IS_ACHIEVEMENT_NEW_SKIN = 0;
	public static int IS_ACHIEVEMENT_MULTI_PULTI = 0;
	public static int IS_ACHIEVEMENT_MISS_CLICK = 0;
	public static int IS_ACHIEVEMENT_GET_MAX = 0;
	public static int IS_ACHIEVEMENT_THREE_JUMPS = 0;

	public static int IS_ACHIEVEMENT_MASTER = 0;
	public static int IS_ACHIEVEMENT_FiFIELD_OF_CANDIES = 0;
	public static int IS_ACHIEVEMENT_EXPLOSIVE = 0;
	public static int IS_ACHIEVEMENT_COLLECTION = 0;

	public static int QUEST_GAMEPLAY_Counter 				= 0;
	public static int QUEST_THROW_Counter 					= 0;
	public static int QUEST_CHARACTERS_Counter 				= 0;
	public static int QUEST_BOMBS_Counter 					= 0;
	public static int QUEST_MISS_Counter 					= 0;

	public const int BUBBLE_COLOR_ONE 						= 0;
	public const int BUBBLE_COLOR_TWO 						= 1;
	public const int BUBBLE_COLOR_THREE 					= 2;
	public const int BUBBLE_COLOR_FOUR 						= 3;
	public const int BUBBLE_COLOR_MULTI 					= 4;
	public const int BUBBLE_COLOR_TIMER 					= 5;
	public const int BUBBLE_COLOR_HEAVY 					= 6;

	static public float heroMoveTop 							= 80;
	static public int rateCounter 								= 0;
	public static MyHeyzap MyHeyzap { get; set; }

	static public void loadVariables() {
		noAds = PlayerPrefs.GetInt ("noAds", 0);
		currentFaceID = PlayerPrefs.GetInt ("currentFaceID", 0);
		//currentFaceID = 0;
		gameBestScore = PlayerPrefs.GetInt ("BestScore", 0);
		//gameBestScore = 0;
		coinsCount = PlayerPrefs.GetInt ("coinsCount", 0);
		//coinsCount = 0;
		rateCounter = PlayerPrefs.GetInt ("rateCounter", 0);

		//loadRewardedClock();
		//loadGiftClock();

		for (int i = 0; i < faceAvailable.Length; i++)  {
			if (i == 0)
				faceAvailable [0] = 1;
			else {
				faceAvailable [i] = PlayerPrefs.GetInt ("faceAvailable_" + i, 0);
				//PlayerPrefs.SetInt ("faceAvailable_" + i, 0);
			}
		}

		BTN_GIFT_HIDE_DELAY_COUNTER = PlayerPrefs.GetInt ("BTN_GIFT_HIDE_DELAY_COUNTER", 0);
		//BTN_GIFT_HIDE_DELAY_COUNTER = 0;

		IS_ACHIEVEMENT_FIRST_WIN = PlayerPrefs.GetInt ("IS_ACHIEVEMENT_FIRST_WIN", 0);
		IS_ACHIEVEMENT_NEW_SKIN = PlayerPrefs.GetInt ("IS_ACHIEVEMENT_NEW_SKIN", 0);
		IS_ACHIEVEMENT_MULTI_PULTI = PlayerPrefs.GetInt ("IS_ACHIEVEMENT_MULTI_PULTI", 0);
		IS_ACHIEVEMENT_MISS_CLICK = PlayerPrefs.GetInt ("IS_ACHIEVEMENT_MISS_CLICK", 0);
		IS_ACHIEVEMENT_GET_MAX = PlayerPrefs.GetInt ("IS_ACHIEVEMENT_GET_MAX", 0);
		IS_ACHIEVEMENT_THREE_JUMPS = PlayerPrefs.GetInt ("IS_ACHIEVEMENT_THREE_JUMPS", 0);
		IS_ACHIEVEMENT_MASTER = PlayerPrefs.GetInt ("IS_ACHIEVEMENT_MASTER", 0);
		IS_ACHIEVEMENT_FiFIELD_OF_CANDIES = PlayerPrefs.GetInt ("IS_ACHIEVEMENT_FiFIELD_OF_CANDIES", 0);
		IS_ACHIEVEMENT_EXPLOSIVE = PlayerPrefs.GetInt ("IS_ACHIEVEMENT_EXPLOSIVE", 0);
		IS_ACHIEVEMENT_COLLECTION = PlayerPrefs.GetInt ("IS_ACHIEVEMENT_COLLECTION", 0);

		QUEST_GAMEPLAY_Counter = PlayerPrefs.GetInt ("QUEST_GAMEPLAY_Counter", 0);
		QUEST_THROW_Counter = PlayerPrefs.GetInt ("QUEST_THROW_Counter", 0);
		QUEST_CHARACTERS_Counter = PlayerPrefs.GetInt ("QUEST_CHARACTERS_Counter", 0);
		QUEST_BOMBS_Counter = PlayerPrefs.GetInt ("QUEST_BOMBS_Counter", 0);
		QUEST_MISS_Counter = PlayerPrefs.GetInt ("QUEST_MISS_Counter", 0);
	}		

}
