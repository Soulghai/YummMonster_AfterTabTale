using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {

	public class PsdkBannersService  : IPsdkBanners {

		IPsdkBanners _impl;


		public PsdkBannersService(IPsdkServiceManager sm) {
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkBannersService(sm.GetImplementation()); break;
			#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkBannersService(sm.GetImplementation()); break;
			#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor: 	_impl = new UnityEditorPsdkBannersService(sm.GetImplementation()); break;
			default: throw new System.Exception("Platform not supported for PsdkBannersService.");
			}
		}

		public bool Setup() {
			return _impl.Setup();
		}

		public bool Show() {
			return _impl.Show();
		}
		
		public void Hide() {
			_impl.Hide();
		}
		
		public float GetAdHeight() {
			return _impl.GetAdHeight();
		}

		public bool IsBlockingViewNeeded() {
			return _impl.IsBlockingViewNeeded();
		}

		public bool IsActive() {
			return _impl.IsActive();
		}

		public bool IsAlignedToTop() {
			return _impl.IsAlignedToTop();
		}


		public void psdkStartedEvent() {
			_impl.psdkStartedEvent();
		}
	
		public IPsdkBanners GetImplementation() {
			return _impl;
		}
	}
}
