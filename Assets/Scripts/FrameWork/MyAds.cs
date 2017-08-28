using System;
using UnityEngine;

public class MyAds : MonoBehaviour
{
    private static int _rewardedAdCounter;
    private bool _isRewardedVideoReadyToShow;
    private DateTime _rewardDate;
    private bool _isRewardedWaitTimer;
    private static bool _isRewardedAdCalcNext;

    private static int _videoAdCounter;
    private static bool _isVideoAdCalcNext;
    private bool _isVideoReadyToShow;
    private DateTime _videoDate;
    private bool _isVideoWaitTimer;
    private bool _isFirstTimeVideo;

    private void Start()
    {
        _rewardDate = DateTime.UtcNow;
        _isRewardedWaitTimer = true;
        _isRewardedAdCalcNext = true;
        _rewardedAdCounter = 0;

        _videoDate = DateTime.UtcNow;
        _isVideoWaitTimer = true;
        _isVideoAdCalcNext = true;
        _isFirstTimeVideo = true;
        _videoAdCounter = 0;
    }

    void OnEnable()
    {
        GlobalEvents<OnRewardedTryShow>.Happened += OnRewardedTryShow;
        GlobalEvents<OnAdsVideoTryShow>.Happened += OnAdsVideoTryShow;
        GlobalEvents<OnAdsRewardedShowing>.Happened += OnAdsRewardedShowing;
        GlobalEvents<OnAdsVideoShowing>.Happened += OnAdsVideoShowing;
    }

    void OnDisable()
    {
        GlobalEvents<OnRewardedTryShow>.Happened -= OnRewardedTryShow;
        GlobalEvents<OnAdsVideoTryShow>.Happened -= OnAdsVideoTryShow;
        GlobalEvents<OnAdsRewardedShowing>.Happened -= OnAdsRewardedShowing;
        GlobalEvents<OnAdsVideoShowing>.Happened -= OnAdsVideoShowing;
    }

    private void OnRewardedTryShow(OnRewardedTryShow obj)
    {
        Debug.Log("OnRewardedTryShow");
        if (_rewardedAdCounter >= 4 )
        {
            if (_isRewardedVideoReadyToShow)
            {
                GlobalEvents<OnShowRewarded>.Call(new OnShowRewarded());
                Debug.Log("GlobalEvents<OnShowRewarded>");
            }
//            _isRewardedAdCalcNext = false;
        }
        else
        {
            if (_isRewardedAdCalcNext) ++_rewardedAdCounter;
        }
    }

    private void OnAdsVideoTryShow(OnAdsVideoTryShow obj)
    {
        Debug.Log("OnAdsVideoTryShow" + _videoAdCounter + " " + _isVideoReadyToShow + " " + _videoDate);
        if (_isFirstTimeVideo && _videoAdCounter == 3 ||
            _videoAdCounter >= 5)
        {
            if (_isFirstTimeVideo) _isFirstTimeVideo = false;

            if (_isVideoReadyToShow)
            {
                GlobalEvents<OnShowVideoAds>.Call(new OnShowVideoAds());
                Debug.Log("GlobalEvents<OnShowVideoAds>");
            }
//            _isVideoAdCalcNext = false;
        }
        else
        {
            if (_isVideoAdCalcNext) ++_videoAdCounter;
        }
    }

    private void OnAdsVideoShowing(OnAdsVideoShowing obj)
    {
        // продолжаем считать геймлпеи, после которых можно показыавть Video рекламу
        _videoAdCounter = 1;
        _videoDate = DateTime.UtcNow;
        _videoDate = _videoDate.AddMinutes(1);
        _isVideoAdCalcNext = true;
    }
    
    private void OnAdsRewardedShowing(OnAdsRewardedShowing e)
    {
        _rewardDate = DateTime.UtcNow;
        _rewardDate = _rewardDate.AddMinutes(1);
        _isRewardedVideoReadyToShow = false;
        _isRewardedWaitTimer = true;
        _rewardedAdCounter = 1;

        //Обнуляем Video таймер и коунтер
        _videoAdCounter = 1;
        _videoDate = DateTime.UtcNow;
        _videoDate = _videoDate.AddMinutes(1);
        _isVideoAdCalcNext = true;
        // продолжаем считать геймлпеи, после которых можно показыавть Rewarded рекламу
        _isRewardedAdCalcNext = true;
    }

    private void Update()
    {
        UpdateVideoTimerEnd();
    }

    private void UpdateVideoTimerEnd()
    {
        if (_isRewardedWaitTimer)
        {
            TimeSpan difference = _rewardDate.Subtract(DateTime.UtcNow);
            if (difference.TotalSeconds <= 0f)
            {
                _isRewardedWaitTimer = false;
            }
        }

        if (!_isRewardedWaitTimer)
        {
            if (!_isRewardedVideoReadyToShow)
            {
                _isRewardedVideoReadyToShow = true;
                GlobalEvents<OnRewardedVideoAvailable>.Call(
                    new OnRewardedVideoAvailable {isAvailable = true});
            }
        }
        
        if (_isVideoWaitTimer)
        {
            TimeSpan difference = _videoDate.Subtract(DateTime.UtcNow);
            if (difference.TotalSeconds <= 0f)
            {
                _isVideoWaitTimer = false;
            }
        }
        
        if (!_isVideoWaitTimer)
        {
            if (!_isVideoReadyToShow)
                _isVideoReadyToShow = true;
        }
    }
}