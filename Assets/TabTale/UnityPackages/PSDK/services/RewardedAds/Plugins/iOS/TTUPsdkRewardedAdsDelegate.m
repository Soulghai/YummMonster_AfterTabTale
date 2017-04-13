//
//  TTUPsdkRewardeAdsDelegate.m
//
//  Created by Israel Papoushado on 7/20/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

 #import "TTUPsdkRewardedAdsDelegate.h"
#include "TTUPsdkUnitySendSyncMessage.h"

 extern void UnitySendMessage(const char *, const char *, const char *);

#if UNITY_VERSION < 500
 extern void UnityPause(bool pause);
#endif
 
 
 @implementation TTUPsdkRewardeAdsDelegate
 
 -(void) adIsReady
 {
     NSLog(@"TTUPsdkRewardeAdsDelegate:: Notification received : Ad is ready");
    UnitySendMessage("PsdkEventSystem","OnRewardedAdIsReady", "");
 }

 -(void) adIsNotReady
 {
     NSLog(@"TTUPsdkRewardeAdsDelegate:: Notification received : Ad is not ready");
    UnitySendMessage("PsdkEventSystem","OnRewardedAdIsNotReady", "");
 }

 -(void) adWillShow
 {
    NSLog(@"TTUPsdkRewardeAdsDelegate:: Notification received : Ad will show");
    PsdkUnitySendSyncMessage("OnRewardedAdWillShow", "");
    //UnitySendMessage("PsdkEventSystem","OnRewardedAdWillShow", "");
    UnityPause(true);
 }
 -(void) adDidClose
 {
    UnityPause(false);
     NSLog(@"TTUPsdkRewardeAdsDelegate:: Notification received : Ad did close");
     UnitySendMessage("PsdkEventSystem","OnRewardedAdDidClose", "");
 }

 -(void) adShouldReward
 {
     NSLog(@"TTUPsdkRewardeAdsDelegate:: Notification received : Ad should reward");
    UnitySendMessage("PsdkEventSystem","OnRewardedAdShouldReward", "");
 }
 -(void) adShouldNotReward
 {
     NSLog(@"TTUPsdkRewardeAdsDelegate:: Notification received : Ad should not reward");
    UnitySendMessage("PsdkEventSystem","OnRewardedAdShouldNotReward", "");
 }


@end
