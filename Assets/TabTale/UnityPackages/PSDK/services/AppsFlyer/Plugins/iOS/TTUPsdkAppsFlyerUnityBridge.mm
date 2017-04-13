
#import "TTUPsdkAppsFlyerUnityBridge.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@implementation TTUPsdkAppsFlyerUnityBridge


#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {
    
    void psdkAppsFlyerReportPurchase(const char* price, const char* currency) {
        id<PSDKAppsFlyer> service = [[PSDKServiceManager instance] appsFlyer];
        if (nil != service)
            [service reportPurchase:[[NSString alloc] initWithUTF8String:price] currency:[[NSString alloc] initWithUTF8String:currency]];
    }
}

@end
