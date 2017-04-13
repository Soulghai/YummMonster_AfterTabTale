//
//  TTUPsdkBannersDelegate.m
//  Unity Psdk delegate
//
//  Created by Israel Papoushado on 10/7/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#import "TTUPsdkBannersDelegate.h"


extern void UnitySendMessage(const char *, const char *, const char *);


/////////////////////////////////////////////
////      PSDKLocationsMgrDelegate      /////
/////////////////////////////////////////////
@implementation TTUPsdkBannersDelegate


-(void) onBannerShown
{
    UnitySendMessage("PsdkEventSystem","onBannerShown", "");
}

-(void) onBannerFailed
{
    UnitySendMessage("PsdkEventSystem","onBannerFailed", "");
}

-(void) onBannerConfigurationUpdate
{
    UnitySendMessage("PsdkEventSystem","onBannerConfigurationUpdate", "");
}

-(void) onBannerWillDisplay
{
    UnitySendMessage("PsdkEventSystem","onBannerWillDisplay", "");
}

-(void) onBannerClose
{
    UnitySendMessage("PsdkEventSystem","onBannerClose", "");
}

-(void) onBannerHidden
{
    UnitySendMessage("PsdkEventSystem","onBannerHidden", "");
}



@end
