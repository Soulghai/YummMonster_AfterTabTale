using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace TabTale.PSDK.Editor.Installer
{

	//[InitializeOnLoad]
	public static class PsdkPkgsMgr
	{

		public static float progress = 0f;
		public static string currentStatus = "";
			
		static PsdkPkgsMgr ()
		{

//			if (! UnityEditorInternal.InternalEditorUtility.inBatchMode)
//				return;
		}

		//[MenuItem("PSDK/Update from latest build")]
		public static void UpdatePsdkFromMenu ()
		{
		}

		public static void UpdatePsdk (string majorPsdkBranch)
		{

			string latestVersion = LatestRemoteBuildVersionPerMajorBranch (majorPsdkBranch);
			if (latestVersion == null) {
				UnityEngine.Debug.LogError ("Didn't find remote psdk version !");
				return;
			}
			string localVersion = MinimumInstalledPsdkVersion ();
			if (localVersion != null && (System.String.CompareOrdinal (localVersion, latestVersion)) == 0) {
				UnityEngine.Debug.Log ("Current version is the same as remote: " + localVersion);
				return;
			}

			UnityEngine.Debug.Log ("Current version " + localVersion + ", remote version:" + latestVersion);
			ImportRemoteBuildPkgs (majorPsdkBranch);
		}

		static void ImportRemoteBuildPkgs (string majorPsdkBranch)
		{
			UnityEngine.Debug.Log ("Importing PSDK uniy packages of branch: " + majorPsdkBranch);
			IList<string> pkgsToImport = listRemotePkgsIntersectedByLocal (listRemotePkgs (PsdkLatestBuildPath (majorPsdkBranch)), ListLocalPsdkInstalledPkgs ());
			ImportRemotePkgsPkgsWithCoroutine (pkgsToImport);
			// this is only for making sure the version is updated. the parameters are not relevant.
			//PsdkVersionsWriterPostProcess.OnPostProcessBuild (BuildTarget.Android,"");
		}

		public static void ImportBuilderPkg (string pkgPath)
		{
			UnityEngine.Debug.Log ("Importing " + pkgPath);
			AssetDatabase.ImportPackage (pkgPath, false);
			AssetDatabase.Refresh (ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
			AssetDatabase.SaveAssets ();
			UnityEngine.Debug.Log ("Done importing " + pkgPath);
		}

		static string PsdkLatestBuildPath (string majorPsdkBranch)
		{
			UnityEngine.Debug.Log ("Checking major psdk branch:" + majorPsdkBranch);
			string pathToMajorBranch = "/Volumes/REPO/unity/PublishingSDK/" + majorPsdkBranch;
			string latestBuild = GetLastUpdatedDirInDirectory (pathToMajorBranch);
			if (latestBuild == null) {
				UnityEngine.Debug.LogError ("Didn't find latest build !");
				return null;
			}
			UnityEngine.Debug.Log ("latest build:" + latestBuild);
			return latestBuild;
		}

		static string LatestRemoteBuildVersionPerMajorBranch (string majorPsdkBranch)
		{
			string pathToRemoteBuild = PsdkLatestBuildPath (majorPsdkBranch);

			if (pathToRemoteBuild == null)
				return null;

			return Path.GetFileName (pathToRemoteBuild);
		}

		private static string GetLastUpdatedDirInDirectory (string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo (path);
			if (directoryInfo == null) {
				UnityEngine.Debug.LogError ("Didn't find path " + path);
				return null;
			}
			DirectoryInfo[] files = directoryInfo.GetDirectories ();
			List<DirectoryInfo> lastUpdatedFile = new List<DirectoryInfo> ();
			System.DateTime lastUpdate = System.DateTime.MinValue;
			foreach (DirectoryInfo file in files) {
				if (file.LastWriteTimeUtc > lastUpdate) {
					lastUpdatedFile.Add (file);
					lastUpdate = file.LastWriteTimeUtc;
				}
			}
			
			if (lastUpdatedFile.Count == 0)
				return null;
			
			return lastUpdatedFile [lastUpdatedFile.Count - 1].FullName;
		}

		private static string GetLastUpdatedFileInDirectory (string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo (path);
			FileInfo[] files = directoryInfo.GetFiles ();
			List<FileInfo> lastUpdatedFile = new List<FileInfo> ();
			System.DateTime lastUpdate = System.DateTime.MinValue;
			foreach (FileInfo file in files) {
				if (file.LastAccessTime > lastUpdate) {
					lastUpdatedFile.Add (file);
					lastUpdate = file.LastAccessTime;
				}
			}

			if (lastUpdatedFile.Count == 0)
				return null;

			return lastUpdatedFile [lastUpdatedFile.Count - 1].FullName;
		}

		static void PrintList (IList<string> list)
		{
			UnityEngine.Debug.Log ("List " + list.Count + ":");
			foreach (var item in list) {
				UnityEngine.Debug.Log (item.ToString ());
			}
			UnityEngine.Debug.Log ("list end.");
		}

		static IList<string> listRemotePkgsIntersectedByLocal (IList<string> remotePkgsPaths, IList<string> localPkgs)
		{
			IList<string> intersectedRemotePackagesPaths = new List<string> ();
			foreach (string remotePkgPath in remotePkgsPaths) {
				foreach (string localPkgName in localPkgs) {
					if (remotePkgPath.Contains (localPkgName)) {
						intersectedRemotePackagesPaths.Add (remotePkgPath);
					}
				}
			}
			return intersectedRemotePackagesPaths;
		}

		static IList<string> listRemotePkgs (string remotePath)
		{
			IList<string> list = new List<string> ();
			string[] files = Directory.GetFiles (remotePath);
			foreach (var file in files)
				list.Add (file);
			return list;
		}

		public static IDictionary<string, string> LocalIntstalledPsdkPkgsAndVersions ()
		{
			IDictionary<string,string> dict = new Dictionary<string, string> ();
			foreach (string fullPathToFile in ListLocalPsdkVersionsFiles()) {
				string version = File.ReadAllText (fullPathToFile).Trim ().Replace ("\n", "").Replace (" ", "");
				string file = Path.GetFileName (fullPathToFile);
				string unityPackageName = file.Substring (0, file.IndexOf (".version"));
				if (dict.ContainsKey(unityPackageName))
					dict[unityPackageName] = version;
				else
					dict.Add (unityPackageName, version);
			}
			return dict;
		}

		public static string PsdkCoreVersion ()
		{
			IDictionary<string,string> pkgs = LocalIntstalledPsdkPkgsAndVersions ();
			if (pkgs.ContainsKey ("PSDKCore.unitypackage"))
				return pkgs ["PSDKCore.unitypackage"];
			return null;
		}

		static IList<string> ListLocalPsdkInstalledPkgs ()
		{
			IList<string> list = new List<string> ();
			foreach (string fullPathToFile in ListLocalPsdkVersionsFiles()) {
				string file = Path.GetFileName (fullPathToFile);
				string unityPackageName = file.Substring (0, file.IndexOf (".version"));
				list.Add (unityPackageName);
			}
			return list;
		}

		static IList<string> ListLocalPsdkVersionsFiles ()
		{
			IList<string> list = new List<string> ();
			try {
				string pathToInstalled = Path.Combine (Application.streamingAssetsPath, "psdk");
				pathToInstalled = Path.Combine (pathToInstalled, "versions");

				if (! Directory.Exists (pathToInstalled))
					return list;

				string[] files = Directory.GetFiles (pathToInstalled);
				foreach (string fullPathToFile in files) {
					if (fullPathToFile.EndsWith (".meta"))
						continue;
					string file = Path.GetFileName (fullPathToFile);
					if (! file.Contains (".unitypackage"))
						continue;
					list.Add (fullPathToFile);
				}
			} 
			catch (System.Exception) {
			} 
			return list;
		}

		public static string MinimumInstalledPsdkVersion ()
		{
			string minVersion = null;
			foreach (string pkg in ListLocalPsdkVersionsFiles()) {
				string version = File.ReadAllText (pkg).Trim ();
				if (minVersion == null) { 
					minVersion = version;
					continue;
				}

				if (System.String.CompareOrdinal (version, minVersion) < 0)
					minVersion = version;
			}

			return minVersion;
		}


		/////////// Remote git ///////////////
		/// 
		static string repoOwnerAndName = "/TabTale/TabtalePublishing";
		static string GitRepo {
			get {
				return "https://github.com" + repoOwnerAndName;
			}
		}
		
		static string GitRepoApi {
			get {
				return "https://api.github.com/repos" + repoOwnerAndName;
			}
		}
		

		static string fetchRemoteLatestBranchPerUnityVersion(string unityVersion) {
			WWW www = new WWW (GitRepo + "/blob/master/"+unityVersion+"/latestMajorBranch.txt?raw=true");
			while (! www.isDone)
					;
			if (String.IsNullOrEmpty (www.error)) {
				return www.text.Replace ("\n", "").Replace (" ", "").Trim ();
			}
			return null;
		}
			
		static string _latestMajorBranch = null;
		public static string FetchRemoteLatestMajorBranch ()
		{
			if (_latestMajorBranch != null) 
					return _latestMajorBranch;

			string unityVersion = Application.unityVersion;
			do {
				_latestMajorBranch = fetchRemoteLatestBranchPerUnityVersion(unityVersion);
				if (_latestMajorBranch != null) 
					return _latestMajorBranch;
				int index = unityVersion.LastIndexOf ('.');
				if (index < 0) {
					_latestMajorBranch = fetchRemoteLatestBranchPerUnityVersion("");
					if (_latestMajorBranch != null) 
						return _latestMajorBranch;
				}
				else 
					unityVersion = unityVersion.Substring (0, index);
				} while (unityVersion.Length > 0) ;

			return null;
		}

		public static string FetchRemoteMinorVersion (string majorBranch)
		{
			string minorUrl = GitRepo + "/blob/" + majorBranch + "/latestBuildNumber.txt";
			if (! minorUrl.EndsWith ("?raw=true"))
				minorUrl += "?raw=true";
			UnityEngine.Debug.LogWarning ("FetchRemoteMinorVersion " + minorUrl);
			WWW www = new WWW (minorUrl);
			while (! www.isDone)
				;
			if (String.IsNullOrEmpty (www.error)) {
				return www.text.Replace ("\n", "").Replace (" ", "");
			}
			
			return null;
		}
		
		public static string PkgUrl (string minorVersion, string unityPkgName)
		{

			if (minorVersion == null || unityPkgName == null) 
				return null;
			string url = GitRepo + "/blob/" + minorVersion + "/" + unityPkgName;
			url = url.Replace (".unitypackage",".unity.zip");
			if (! url.EndsWith (".unity.zip"))
				url += ".unity.zip";
			return url;
		}
		
		static void ImportRemotePkg (string url, string pkgname = null, bool shouldDelete = true)
		{
			UnityEngine.Debug.Log ("importing unity pkg:" + url);
			if (! url.EndsWith ("?raw=true"))
				url += "?raw=true";
			UnityEngine.Debug.Log ("importing unity url:" + url);
			WWW www = new WWW (url);
			while (! www.isDone)
				;
			if (String.IsNullOrEmpty (www.error)) {
				System.IO.File.WriteAllBytes ("/tmp/tmp.unity.zip", www.bytes);
				if (pkgname != null && shouldDelete) 
					PSDKInstallerUninstall.UninstallPsdkUnityPackage (pkgname,true);
				if (! Unzip("/tmp/tmp.unity.zip",Path.Combine(Application.dataPath,".."))) {
					UnityEngine.Debug.LogError("Failed to import unity zip " + url);
					return;
				}
				//AssetDatabase.ImportPackage ("/tmp/tmp.unitypackage", false);
				UnityEngine.Debug.Log ("pkg imported !");
				return;
			}
			UnityEngine.Debug.LogError ("Fail to import unity pkg " + www.error + ":  " + url);
		}

		public static string PsdkLocalBranch ()
		{
			string coreVersion = PsdkCoreVersion ();
			if (coreVersion == null)
				return null;

			if (! coreVersion.StartsWith ("PSDK-"))
				coreVersion = "PSDK-" + coreVersion;
			return coreVersion.Substring (0, coreVersion.LastIndexOf ("."));
		}

		public static string RemoteCompatibleBranchMinorVersion ()
		{
			string version = PsdkLocalBranch ();
			if (version == null)
				return null;
			version = FetchRemoteMinorVersion (version);
			if (version == null)
				return null;

			return version;
		}

		public static void ImportRemotePkgWithMinorVersion (string remoteMinorVersion, string pkgName){
			if (remoteMinorVersion == null)
				return;
			if (pkgName == null)
				return;
			if (! pkgName.EndsWith (".unitypackage"))
				pkgName += ".unitypackage";
			IList<string> pkgUrls = new List<string> ();
			pkgUrls.Add (PkgUrl(remoteMinorVersion,pkgName));
			ImportRemotePkgsPkgsWithCoroutine (pkgUrls);
		}

		public static void ImportRemotePkgsToMinorVersion (string remoteMinorVersion)
		{
			UnityEngine.Debug.Log ("Importing " + remoteMinorVersion);
			IDictionary<string,string> localPkgsList = LocalIntstalledPsdkPkgsAndVersions ();

			IList<string> pkgUrls = new List<string> ();

			if (localPkgsList == null || localPkgsList.Count == 0) {
				pkgUrls.Add(PkgUrl (remoteMinorVersion, "PSDKCore.unitypackage"));
			}

			if (localPkgsList.ContainsKey ("PSDKCore.unitypackage")) {
				pkgUrls.Add(PkgUrl (remoteMinorVersion, "PSDKCore.unitypackage"));
			}

			foreach (KeyValuePair<string,string> item in localPkgsList) {
				if (System.String.CompareOrdinal (item.Value, remoteMinorVersion) < 0) {
					string remotePkg = PsdkPkgsMgr.PkgUrl (remoteMinorVersion, item.Key);
					if (! pkgUrls.Contains(remotePkg)) {
						pkgUrls.Add(remotePkg);
					}

				}
			}
			ImportRemotePkgsPkgsWithCoroutine (pkgUrls);
		}


		static void ImportRemotePkgsPkgsWithCoroutine (IList<string> unityPkgsUrls)
		{
			IEnumerator e = ImportRemotePkgsPkgsCoroutine (unityPkgsUrls);
			while (e.MoveNext())
				;
		}
		
		static IEnumerator ImportRemotePkgsPkgsCoroutine (IList<string> unityPkgs)
		{
			UnityEngine.Debug.Log ("Started unity packages import");

			progress = 0.5f;
			EditorApplication.LockReloadAssemblies ();
			if (unityPkgs.Count > 1) 
				PSDKInstallerUninstall.DeleteAllPsdkServicesFolder ();

			float imported = 0f;
			foreach (string unityPkgPath in unityPkgs) {
				imported++;
				progress = imported/unityPkgs.Count;
				currentStatus = "Importing " + Path.GetFileName(unityPkgPath);
				UnityEngine.Debug.Log (progress + ":" + currentStatus);
				EditorUtility.DisplayProgressBar("Importing PSDK packages",currentStatus,progress);
				ImportRemotePkg (unityPkgPath, Path.GetFileName(unityPkgPath).Replace(".unity.zip",".unitypackage"),true);
				yield return new WaitForSeconds(1f);
			}
			
			UnityEngine.Debug.Log ("Finished unity packages import ");
			yield return new WaitForSeconds(1f);
			EditorApplication.UnlockReloadAssemblies ();
			yield return new WaitForSeconds(1f);
			AssetDatabase.Refresh ();
			EditorUtility.ClearProgressBar ();
			progress = 0f;
		}
		



		public static IList<string> ListRemotePkgsAccordingToMinorVersion(string minorVersion) {
			if (! minorVersion.StartsWith ("PSDK-"))
				minorVersion = "PSDK-" + minorVersion;

			string gitUrl = GitRepoApi + "/branches/" + minorVersion;
			WWW www = new WWW (gitUrl);
			while (! www.isDone)
				;
			if (String.IsNullOrEmpty (www.error)) {
				IList<string> tokenList = new List<string>() {"commit","commit","tree"};
				IList<string> list = new List<string> ();
				IDictionary<string,object> jsonDict = TabTale.PSDK.Editor.Installer.Json.Deserialize(www.text) as Dictionary<string,object>;
				if (jsonDict == null) return null;
				foreach(string token in tokenList) {
					if (! jsonDict.ContainsKey(token)) return null;
					jsonDict = jsonDict[token] as IDictionary<string, object>;
				}
				if (! jsonDict.ContainsKey("url")) return null;
				string url = jsonDict["url"] as string;

				www.Dispose();
				www = new WWW (url);
				while (! www.isDone)
					;
				if (String.IsNullOrEmpty (www.error)) {
					jsonDict = TabTale.PSDK.Editor.Installer.Json.Deserialize(www.text) as Dictionary<string,object>;
					if (jsonDict == null) return null;
					if (! jsonDict.ContainsKey("tree")) return null;
					IList<object> objectsList  = jsonDict["tree"] as IList<object>;
					foreach(object obj in objectsList) {
						IDictionary<string, object> item = obj as IDictionary<string, object>;
						if (item == null) continue;
						if (item.ContainsKey("path")) {
							string pkg = item["path"] as string;
							if (pkg == null) continue;
							if (! pkg.EndsWith(".unitypackage")) continue; 
							list.Add(pkg);
						}
					}
					www.Dispose();
					return list;
				}
					else {
						UnityEngine.Debug.LogError ("ListRemotePkgsAccordingToMinorVersion  " + minorVersion + " " + www.error+ "\n" + url);
					}
					www.Dispose();
					
				}
				else {
					UnityEngine.Debug.LogError ("ListRemotePkgsAccordingToMinorVersion  " + minorVersion + " " + www.error + "\n" + gitUrl);
					www.Dispose();
				}
				return null;
		}


		static bool Unzip(string absoluleZipFilePath, string toLocalDirectory = ".") {

			Process myCustomProcess = new Process ();
			myCustomProcess.StartInfo.FileName = "unzip";
			myCustomProcess.StartInfo.Arguments = string.Format ("{0} \"{1}\" {2} \"{3}\" ", "-u",absoluleZipFilePath, "-d",toLocalDirectory);
			UnityEngine.Debug.Log (myCustomProcess.StartInfo.FileName + " " + myCustomProcess.StartInfo.Arguments); 		
			myCustomProcess.StartInfo.UseShellExecute = false;
			myCustomProcess.StartInfo.RedirectStandardOutput = true;
			myCustomProcess.StartInfo.RedirectStandardError = true;
			myCustomProcess.Start (); 
			myCustomProcess.WaitForExit ();
			string strOutput = myCustomProcess.StandardOutput.ReadToEnd();
			string strErrorOutput = myCustomProcess.StandardError.ReadToEnd();
			int rc = myCustomProcess.ExitCode;
			myCustomProcess.Close ();
			if (rc != 0) { // failed
				UnityEngine.Debug.LogError ("Failed in "+ myCustomProcess.StartInfo.FileName  + " " + myCustomProcess.StartInfo.Arguments +", rc:" + rc);
				UnityEngine.Debug.LogError (strErrorOutput);
				UnityEngine.Debug.Log (strOutput);
				if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
					EditorApplication.Exit(rc);
			} else {
				if (strErrorOutput.Length > 0)
					UnityEngine.Debug.Log(strErrorOutput);
				if (strOutput.Length > 0)
					UnityEngine.Debug.Log(strOutput);
			}


			return true;
		}

		public static IList<string> NoneInstalledRemotePkgs(string minorVersion) {
			IDictionary<string,string> local = LocalIntstalledPsdkPkgsAndVersions ();
			IList<string> remotePkgs = ListRemotePkgsAccordingToMinorVersion (minorVersion);
			if (local == null || local.Count == 0 || remotePkgs == null) {
				return remotePkgs;
			}
			IList<string> list = new List<string>();
			foreach (string pkg in remotePkgs) {
				if (! local.ContainsKey(pkg)) {
					if (pkg.EndsWith(".unitypackage")) {
						list.Add(pkg);
					}
				}
			}
			return list;
		}
	}
}
