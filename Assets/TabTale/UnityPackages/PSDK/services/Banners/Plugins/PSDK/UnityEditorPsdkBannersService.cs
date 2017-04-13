using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {

	public class UnityEditorPsdkBannersService  : IPsdkBanners {

		public UnityEditorPsdkBannersService(IPsdkServiceManager sm) {
		}

		public bool Setup() {

			return PsdkBannersDebug.Instance.Setup();
		}

		public bool Show() {
			return PsdkBannersDebug.Instance.Show();
		}

		public void Hide() {
			PsdkBannersDebug.Instance.Hide();
		}
		
		public float GetAdHeight() {
			return PsdkBannersDebug.Instance.GetAdHeight ();;
		}
		
		public bool IsBlockingViewNeeded() {
			return PsdkBannersDebug.Instance.IsBlockingViewNeeded();
		}

		public bool IsActive() {
			return PsdkBannersDebug.Instance.IsActive();
		}
		
		public bool IsAlignedToTop() {
			return PsdkBannersDebug.Instance.IsAlignedToTop();
		}
		

		public void psdkStartedEvent() {
			PsdkBannersDebug.Instance.psdkStartedEvent ();
		}
		
		public IPsdkBanners GetImplementation() {
			return this;
		}


	}
}
