using UnityEditor;
using System.IO;
using Tapdaq;

namespace TDEditor {

	public class TDManifestResolver {

		private const string androidManifestCopyPath = TDPaths.TapdaqResourcesPath + "/TDDefaultAndroidManifest.xml";

		private const string androidManifestDestinationPath = TDPaths.TapdaqAndroidPath + "/AndroidManifest.xml";

		public static void FixAndroidManifest() {
			if (!File.Exists (androidManifestDestinationPath)) {
				if (File.Exists (androidManifestCopyPath) && Directory.Exists(TDPaths.TapdaqAndroidPath)) {
					FileUtil.CopyFileOrDirectory (androidManifestCopyPath, androidManifestDestinationPath);
					AssetDatabase.Refresh ();
				}
			}
		}

		public static void RemoveMainManifest() {
			if (File.Exists (androidManifestDestinationPath)) {
				FileUtil.DeleteFileOrDirectory (androidManifestDestinationPath);
			}
		}
	}
}
