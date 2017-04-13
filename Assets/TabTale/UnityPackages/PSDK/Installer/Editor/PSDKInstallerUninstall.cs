using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace TabTale.PSDK.Editor.Installer
{
	public class PSDKInstallerUninstall {

               public static void UninstallPsdkFunc() {
                        UninstallPsdkFunc(false);
		}

                public static void UninstallPsdkFunc(bool full) {
			string[] pkgs = getPsdkPackagesFileSetsFiles();
			List<string> sortedPkgs = sortpsdkPackagesFileSetsFiles(pkgs);
			foreach (string fileSetPath in sortedPkgs) {
				UninstallFileSet(fileSetPath, full);
			}
		}

		static string[] getPsdkPackagesFileSetsFiles() {
			List<string> list = new List<string>();
			string path = Path.Combine(Application.dataPath,"PSDK");
			if (Directory.Exists (path)) {
				foreach(var item in System.IO.Directory.GetFiles( path , "*.files.txt", SearchOption.AllDirectories )) {
					if (! item.Contains("FileSet")) continue;
					list.Add (item);
				}
			}
			path = unity5PSdkRootPath;
			if (Directory.Exists (path)) {
				foreach(var item in System.IO.Directory.GetFiles( path , "*.files.txt", SearchOption.AllDirectories )) {
					list.Add (item);
				}
			}
			return list.ToArray();
		}

		static List<string> sortpsdkPackagesFileSetsFiles(string[] list) {
			List<string> output = new List<string>();
			List<string> corePkgs = new List<string>();
			foreach(string item in list) {
				if (item.Contains("PSDKCore"))
					corePkgs.Add(item);
				else
					output.Add(item);
			}
			output.Sort();
			output.AddRange(corePkgs);
			return output;
		}

		static void UninstallFileSet(string pathToFileSet, bool full = false) {
			try {
				string[] filesToDelete = File.ReadAllLines(pathToFileSet);
				foreach(var fileToDelete in filesToDelete) {
					DeleteAFile(fileToDelete, ! full);
				}
				if (File.Exists(pathToFileSet)) {
					AssetDatabase.DeleteAsset(pathToFileSet);
				}
			}
			catch (System.Exception e) {
				Debug.LogException(e);
			}
		}

		static bool InIgnoreList(string filePath) {
			if (filePath.StartsWith(Path.Combine("Assets","StreamingAssets")))  {
				if (filePath.Contains("versions")) // Delete the psdk versions files
					return false;
				return true;
			}
			if (filePath.Contains("UninstallPsdk.cs")) return true;
			if (filePath.Contains ("Plugins")) {
				if (filePath.Contains ("PSDK")) {
					if (filePath.Contains ("Interfaces"))
						return true;
					if (filePath.Contains ("EventsListeners"))
						return true;
					if (filePath.Contains ("Utils"))
						return true;
					if (filePath.Contains ("ServiceMgr"))
						return true;
				}
			}

			return false;
		}
		public static void UninstallPsdkUnityPackage(string pkgName, bool fullyUninstall = false) {
			Debug.LogError("UninstallPsdkUnityPackage: " + pkgName + ", " + fullyUninstall);

			if (pkgName == null) {
				Debug.LogError("null psdk unity package name !!!");
				return;
			}

			if (pkgName.EndsWith (".unity.zip")) {
				pkgName.Replace(".unity.zip",".unitypackage");
			}

			if (! pkgName.EndsWith (".unitypackage")) {
				pkgName += ".unitypackage";
			}

			if (!fullyUninstall && pkgName == "PSDKCore.unitypackage") {
				Debug.LogError("PSDKCore.unitypackage cannot be uninstalled individually, please use PSDK/Uninstall PSDK ...");
				return;
			}

			string unity5pkgname =  pkgName.Replace (".unitypackage","").Replace ("PSDK","");
			string unity5psdkServicesPath = System.IO.Path.Combine (unity5PSdkRootPath, "services");
			string unity5psdkCorePath = System.IO.Path.Combine (unity5PSdkRootPath, "core");

			Debug.Log ("pkg for delete " + pkgName);
			IList<string> filesToDeleteList = new List<string>();
			IList<string> filesToPreserveList = new List<string>();
			string[] fileSetsFiles = getPsdkPackagesFileSetsFiles ();
			foreach (string fileSetPath in fileSetsFiles) {
				if (fileSetPath.Contains(pkgName) 
				    || (fileSetPath.Contains(unity5pkgname) && fileSetPath.Contains(unity5psdkServicesPath))
				    ) {
					// FileSets to delete
					Debug.Log ("filepath to read for delete " + fileSetPath);
					string[] filesToDeleteArray = File.ReadAllLines(fileSetPath);
					foreach(var fileToDelete in filesToDeleteArray) {
						if (!fullyUninstall && InIgnoreList(fileToDelete)) 
							continue;
						filesToDeleteList.Add(fileToDelete);
					}
					filesToDeleteList.Add(fileSetPath);
				}
				else {
					if (fullyUninstall) // not preserving others packages files
						continue;

					// FileSets to preserve
					string[] filesToPreserveArray = File.ReadAllLines(fileSetPath);
					foreach(var fileToPreserve in filesToPreserveArray) {
						filesToPreserveList.Add(fileToPreserve);
							filesToDeleteList.Remove(fileToPreserve);
					}

				}
			}
			foreach(var fileToPreserve in filesToPreserveList) {
				filesToDeleteList.Remove(fileToPreserve);
			}
			foreach(var fileToDelete in filesToDeleteList) {
				DeleteAFile(fileToDelete, ! fullyUninstall);
			}


		}

	
		static string _unity5PSdkRootPath = null;
		static string unity5PSdkRootPath {
			get {
				if (_unity5PSdkRootPath == null) {
					_unity5PSdkRootPath = "PSDK";
					_unity5PSdkRootPath = System.IO.Path.Combine ("UnityPackages", unity5PSdkRootPath);
					_unity5PSdkRootPath = System.IO.Path.Combine ("TabTale", unity5PSdkRootPath);
					_unity5PSdkRootPath = System.IO.Path.Combine ("Assets", unity5PSdkRootPath);
				}
				return _unity5PSdkRootPath;
			}
		}

		static void DeleteAFile(string fileToDelete, bool checkIgnoreList = true) {

			if (checkIgnoreList) {
				if (InIgnoreList (fileToDelete)) 
					return;
			}

			string fullPathToDelete = Path.Combine(Application.dataPath.Substring(0,Application.dataPath.LastIndexOf("Assets")),fileToDelete);
			if (File.Exists(fullPathToDelete)) {
				Debug.Log(fullPathToDelete + " will be deleted !");
				AssetDatabase.DeleteAsset(fileToDelete);
			}
		}

		static string _psdkRelativePath = null; // don't use it directly; us PsdkRelativePath
		static string PsdkRelativePath {
			get {
				if (_psdkRelativePath == null) {
					_psdkRelativePath = "";
					string[] pathArray = { "Assets", "TabTale","UnityPackages","PSDK"};
					foreach(string str in pathArray)
					_psdkRelativePath = Path.Combine(_psdkRelativePath,str);
				}
				return _psdkRelativePath;
			}
		}

		public static void DeleteAllPsdkServicesFolder(){
			#if UNITY_5
			string servicesPath = Path.Combine (PsdkRelativePath, "services");
			if (AssetDatabase.IsValidFolder (servicesPath))
				AssetDatabase.DeleteAsset (servicesPath);
			#endif
		}
	}


}
