
#import "TTUPsdkAnalyticsUnityBridge.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@implementation TTUPsdkAnalyticsUnityBridge


#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {
    
    void psdkAnalyticsLogEvent(int64_t targets, const char* eventName, const char* eventParamsJsonStr, BOOL timed) {
        id<PSDKAnalytics> service = [[PSDKServiceManager instance] analytics];
        if (nil != service) {
            [service logEvent:(AnalyticsType)targets
                         name:[[NSString alloc] initWithUTF8String:eventName]
                       params:[TTUPsdkAnalyticsUnityBridge psdkAnalyticsDictionaryFromJsonStr:eventParamsJsonStr]
                        timed: timed];
        }
    }
    
    
    void psdkAnalyticsEndLogEvent(const char* eventName, const char* eventParamsJsonStr) {
        id<PSDKAnalytics> service = [[PSDKServiceManager instance] analytics];
        if (nil != service) {
            [service endTimedEvent:[[NSString alloc] initWithUTF8String:eventName]
                            params:[TTUPsdkAnalyticsUnityBridge psdkAnalyticsDictionaryFromJsonStr:eventParamsJsonStr]];
        }
    }
    
    void psdkAnalyticsLogComplexEvent(const char* eventName, const char* eventParamsJsonStr) {
        id<PSDKAnalytics> service = [[PSDKServiceManager instance] analytics];
        if (nil != service) {
            [service logComplexEvent:[[NSString alloc] initWithUTF8String:eventName]
                              params:[TTUPsdkAnalyticsUnityBridge psdkAnalyticsDictionaryFromJsonStr:eventParamsJsonStr]
             ];
        }
    }
    
    void psdkAnalyticsReportPurchase(const char* price, const char* currency, const char* productId){
        id<PSDKAnalytics> service = [[PSDKServiceManager instance] analytics];
        if (nil != service) {
            [service reportPurchase:[[NSString alloc] initWithUTF8String:price]
                           currency:[[NSString alloc] initWithUTF8String:currency]
                          productID:[[NSString alloc] initWithUTF8String:productId]];
        }
        
        
    }
    
    
}

+ (NSDictionary *) psdkAnalyticsDictionaryFromJsonStr: (const char*) json {
    if (json == nil) return nil;
    NSData *data = [[[NSString alloc] initWithUTF8String:json] dataUsingEncoding:NSUnicodeStringEncoding];
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:kNilOptions error:nil];
    return dict;
}

@end
