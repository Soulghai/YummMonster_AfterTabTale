using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	public class IphonePsdkBannersService  : IPsdkBanners {
		
		[DllImport ("__Internal")]
		private static extern bool psdkSetupBanners();
		
		[DllImport ("__Internal")]
		private static extern bool psdkBannersShow();
		
		[DllImport ("__Internal")]
		private static extern void psdkBannersHide();
		
		[DllImport ("__Internal")]
		private static extern float psdkBannersGetAdHeight();
		
		[DllImport ("__Internal")]
		private static extern bool psdkBannerIsBlockingViewNeeded();

		[DllImport ("__Internal")]
		private static extern bool psdkBannerIsActive();
		
		[DllImport ("__Internal")]
		private static extern bool psdkBannerIsAlignedToTop();
		


		public IphonePsdkBannersService(IPsdkServiceManager sm) {
		}



		public bool Setup() {
			PsdkBannersUtils.CopyHouseAdsDirToTtpsdkFolder();
			return psdkSetupBanners();
		}

		public bool Show() {
			return psdkBannersShow();
		}

		public void Hide() {
			psdkBannersHide();
		}

		public float GetAdHeight() {
			return psdkBannersGetAdHeight();
		}

		public bool IsBlockingViewNeeded() {
			return psdkBannerIsBlockingViewNeeded();
		}

		public bool IsActive() {
			return psdkBannerIsActive();
		}
		
		public bool IsAlignedToTop() {
			return psdkBannerIsAlignedToTop();
		}


		public void psdkStartedEvent() {
		}

		public IPsdkBanners GetImplementation() {
			return this;
		}

	}
}
