// ADS
// Дать награду игроку
public struct OnGiveReward { public bool IsAvailable; }

// Запрос на показ видео рекламы
public struct OnRewardedShow {}
// Показываем Video рекламу, если доступна
public struct OnAdsVideoTryShow {}

// Запрос на показ рекламы 
public struct OnShowVideoAds {}

// Rewarded реклама готова загружена
public struct OnRewardedLoaded { public bool IsAvailable; }
// Начался показ Video рекламы
public struct OnAdsVideoShowing {}

// Начался показ Rewarded рекламы
public struct OnAdsRewardedShowing {}
// Начался показ Rewarded рекламы
public struct OnRewardedWaitTimer
{
    public bool IsWait;
}

// Debug
public struct OnDebugLog { public string message; }