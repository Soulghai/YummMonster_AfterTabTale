//
//  TTUPsdkSplashDelegate.m
//  RuntimeConfigTestApp
//
//  Created by Israel Papoushado on 7/20/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#import "TTUPsdkStartupDelegate.h"

extern void UnitySendMessage(const char *, const char *, const char *);

@implementation TTUPsdkStartupDelegate

- (void) onConfigurationReady
{
    NSLog(@"TTUPsdkStartupDelegate::onConfigurationReady");
    UnitySendMessage("PsdkEventSystem","OnConfigurationReady", "");
}

- (void) onPSDKReady
{
    NSLog(@"TTUPsdkStartupDelegate::onPSDKReady");
    UnitySendMessage("PsdkEventSystem","OnPSDKReady", "");
}


@end
