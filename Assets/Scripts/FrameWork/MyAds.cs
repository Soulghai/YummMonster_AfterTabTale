using System;
using UnityEngine;

public class MyAds : MonoBehaviour
{
    private DateTime _rewardDate;
    private bool _isRewardedWaitTimer;

    private static int _videoAdCounter;
    private static bool _isVideoAdCalcNext;
    private DateTime _videoDate;
    private bool _isVideoWaitTimer;
    private bool _isFirstTimeVideo;

    private void Start()
    {
        _rewardDate = DateTime.UtcNow;
        _isRewardedWaitTimer = true;

        _videoDate = DateTime.UtcNow;
        _isVideoWaitTimer = true;
        _isVideoAdCalcNext = true;
        _isFirstTimeVideo = true;
        _videoAdCounter = 0;
    }

    void OnEnable()
    {
        GlobalEvents<OnAdsRewardedShowing>.Happened += OnAdsRewardedShowing;
        
        GlobalEvents<OnAdsVideoTryShow>.Happened += OnAdsVideoTryShow;
        GlobalEvents<OnAdsVideoShowing>.Happened += OnAdsVideoShowing;
    }

    private void OnAdsVideoTryShow(OnAdsVideoTryShow obj)
    {
        Debug.Log("OnAdsVideoTryShow" + _videoAdCounter + " " + _isVideoWaitTimer + " " + _videoDate);
        if (_isFirstTimeVideo && _videoAdCounter == 5 ||
            _videoAdCounter >= 5)
        {
            if (_isFirstTimeVideo) _isFirstTimeVideo = false;

            if (!_isVideoWaitTimer)
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
        StartWaitingVideo();
    }
    
    private void OnAdsRewardedShowing(OnAdsRewardedShowing e)
    {
        _rewardDate = DateTime.UtcNow;
        _rewardDate = _rewardDate.AddMinutes(2);
        _isRewardedWaitTimer = true;
        GlobalEvents<OnRewardedWaitTimer>.Call(new OnRewardedWaitTimer {IsWait = true}); 

        //Обнуляем Video таймер и коунтер
        StartWaitingVideo();
    }

    private void StartWaitingVideo()
    {
        _videoDate = DateTime.UtcNow;
        _videoDate = _videoDate.AddMinutes(2);
        _videoAdCounter = 1;
        _isVideoAdCalcNext = true;
        _isVideoWaitTimer = true;
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
                GlobalEvents<OnRewardedWaitTimer>.Call(new OnRewardedWaitTimer {IsWait = false}); 
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
    }
}