//
//  TTUPsdkBannersDelegate.h
//  Psdk Psdk Monetization
//
//  Created by Israel Papoushado on 10/7/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#import <PublishingSDKCore/PublishingSDKCore.h>


@interface TTUPsdkBannersDelegate : NSObject<PSDKBannersDelegate>

-(void) onBannerShown;
-(void) onBannerFailed;
-(void) onBannerConfigurationUpdate;

@end