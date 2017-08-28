//
//  TapdaqOfferwall.m
//  Unity-iPhone
//
//  Created by Andriy Medvid on 03.08.17.
//
//

#import "TapdaqStandardAd.h"

@implementation TapdaqOfferwall

+ (TapdaqOfferwall*)sharedInstance
{
    static dispatch_once_t once;
    static TapdaqOfferwall* sharedInstance;
    dispatch_once(&once, ^{
        sharedInstance = [[self alloc] init];
    });
    return sharedInstance;
}

- (BOOL)isReady {
    return [[Tapdaq sharedSession] isOfferwallReady];
}

- (void)show {
    [[Tapdaq sharedSession] showOfferwall];
}

- (void)load {
    [[Tapdaq sharedSession] loadOfferwall];
}

@end
