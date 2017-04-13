using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	
	public static class PsdkBannersUtils {
		
		public static void CopyHouseAdsDirToTtpsdkFolder() {
			string ttpsdkSrcFolder                   = "ttpsdk";
			string ttpsdkDestinationFolder   = Path.Combine(Application.temporaryCachePath,"ttpsdk");
			string ttpsdkHouseAdsFolder 	 = Path.Combine(ttpsdkDestinationFolder, "houseAds");
			string houseAdsSrcZip            = Path.Combine(ttpsdkSrcFolder,"houseAds.zip");
			string houseAdsDestinationFolder = Path.Combine(ttpsdkDestinationFolder,"houseAds");
			
			try {
				
				if(Directory.Exists(ttpsdkHouseAdsFolder)){
					return;
				}
				
				// creating ttpsdk directory
				if (! Directory.Exists(ttpsdkDestinationFolder)){
					Directory.CreateDirectory(ttpsdkDestinationFolder);
				}
				#if UNITY_ANDROID 
				PsdkUtils.MakeDir(ttpsdkDestinationFolder);
				#endif
				if (! Directory.Exists(houseAdsDestinationFolder)) {
					// copy houseAds to cachedir
					PsdkUtils.Unzip(houseAdsSrcZip,ttpsdkDestinationFolder);
					
					#if UNITY_ANDROID 
					string dstFile = Path.Combine(houseAdsDestinationFolder,"houseAds.json");
					#if AMAZON
					string srcFile = Path.Combine(houseAdsDestinationFolder,"houseAds.json_amazon");
					#else
					string srcFile = Path.Combine(houseAdsDestinationFolder,"houseAds.json_google");
					#endif
					if (File.Exists(srcFile)) {
						File.Delete(dstFile);
						File.Copy(srcFile,dstFile);
					}
					#endif
				}
				
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				Debug.LogError("Didn't manage to move houseAds to destination folder " + houseAdsDestinationFolder);
			}
		}
		
		
		
	}
}
