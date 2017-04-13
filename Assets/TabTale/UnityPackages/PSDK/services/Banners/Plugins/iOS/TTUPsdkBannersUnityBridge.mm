#import <Foundation/Foundation.h>
#import "TTUPsdkBannersUnityBridge.h"
#import "TTUPsdkBannersDelegate.h"
#import <PublishingSDKCore/PublishingSDKCore.h>


@implementation TTUPsdkBannersUnityBridge


#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {
    
    bool psdkSetupBanners() {
        
        id<PSDKBannersDelegate> bannersDelegate = [[TTUPsdkBannersDelegate alloc] init];
        [PSDKServiceManager  setupBannersDelegate:bannersDelegate];
        NSLog(@"Setuped banners delegate !");
        return true;
    }
    
    
    BOOL psdkBannersShow()
    {
        return [[[PSDKServiceManager instance] banners] show];
    }
    
    void psdkBannersHide()
    {
        [[[PSDKServiceManager instance] banners] hide];
    }
    
    float psdkBannersGetAdHeight()
    {
        return [[[PSDKServiceManager instance] banners] getAdHeight];
    }
    
    BOOL psdkBannerIsBlockingViewNeeded()
    {
        return [[[PSDKServiceManager instance] banners] isBlockingViewNeeded];
    }
    
    BOOL psdkBannerIsActive()
    {
        return [[[PSDKServiceManager instance] banners] isActive];
    }
    
    BOOL psdkBannerIsAlignedToTop()
    {
        return [[[PSDKServiceManager instance] banners] isAlignedToTop];
    }
    
    
}

@end
