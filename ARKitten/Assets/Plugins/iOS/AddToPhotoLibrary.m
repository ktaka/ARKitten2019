//
//  AddToPhotoLibrary.m
//  
//
//  Created by Kenichi Takahashi on 2020/04/09.
//

#import <Foundation/Foundation.h>
#import <Photos/Photos.h>

void AddToPhotoLibrary (const char* path)
{
    //
    NSURL *url = [NSURL fileURLWithPath:[NSString stringWithUTF8String:path]];

    //
    [[PHPhotoLibrary sharedPhotoLibrary] performChanges:^{
            [PHAssetChangeRequest creationRequestForAssetFromImageAtFileURL:url];
        } completionHandler:^(BOOL success, NSError *error) {
            if (success) {
                UnitySendMessage("ScreenspaceUI", "AddToPhotoLibraryCompleted", [url.path UTF8String]);
            } else {
                UnitySendMessage("ScreenspaceUI", "AddToPhotoLibraryCompleted", [error.description UTF8String]);
            }
        }];
}
