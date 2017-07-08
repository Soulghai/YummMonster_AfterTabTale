// ADS
// Дать награду игроку
public struct OnGiveReward { public bool isAvailable; }
// Rewarded реклама готова к показу
public struct OnRewardedVideoAvailable { public bool isAvailable; }
// Показываем Rewarded рекламу
public struct OnRewardedShow {}

// Запрос на показ рекламы 
public struct OnShowVideoAds { public bool isAvailable; }
// Запрос на показ видео рекламы
public struct OnShowRewarded { public bool isAvailable; }

// Debug
public struct OnDebugLog { public string message; }


//example
//событие с несколькими параметрами
/*public struct GameSettingEvent
{
    public bool useAds;
    public bool useAnalytics;
    public float coinsFactor;
    public int startingCoins;
}*/