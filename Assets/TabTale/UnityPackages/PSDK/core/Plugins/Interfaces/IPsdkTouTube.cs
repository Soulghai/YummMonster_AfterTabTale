using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.Plugins.PSDK {
	/// <summary>
	/// Psdk YouTube Interface
	/// </summary>
	public interface IPsdkYouTube  : IPsdkService {
		bool Setup();
		bool LoadYouTube(string url);
		void DownloadThumbnail(string videoId, string srcPath, string dstPath);
		bool IsCreated();
		void DestroyYouTube(); 
		IPsdkYouTube GetImplementation();
	}
}
