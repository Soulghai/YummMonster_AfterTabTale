// ADS
// Дать награду игроку
public struct OnGiveReward { public bool isAvailable; }

// Показыаем Rewarded рекламу, если доступна
public struct OnRewardedTryShow {}
// Показываем Video рекламу, если доступна
public struct OnAdsVideoTryShow {}

// Запрос на показ рекламы 
public struct OnShowVideoAds { public bool isAvailable; }
// Запрос на показ видео рекламы
public struct OnShowRewarded { public bool isAvailable; }

// Rewarded реклама готова к показу (Время ожидания завершилось)
public struct OnRewardedVideoAvailable { public bool isAvailable; }
// Начался показ Video рекламы
public struct OnAdsVideoShowing {}

// Начался показ Rewarded рекламы
public struct OnAdsRewardedShowing {}

// Debug
public struct OnDebugLog { public string message; }