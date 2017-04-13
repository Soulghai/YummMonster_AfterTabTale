using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using TabTale.PSDK.Editor.Installer;

namespace TabTale.PSDK.Editor.Installer
{
	public class PsdkImportWindow : EditorWindow
	{

		// Add menu named "My Window" to the Window menu
		[MenuItem ("TabTale/PSDK/Update PSDK ...")]
		static void Init ()
		{
			// Get existing open window or if none, make a new one:
			PsdkImportWindow window = (PsdkImportWindow)EditorWindow.GetWindow (typeof(PsdkImportWindow));
			window.Focus ();
		}

		string _remotePsdkLatestMinorVersion = null;
		IDictionary<string,string> _installedPkgs = null;

		void OnEnable ()
		{
			EditorUtility.DisplayCancelableProgressBar("Reading relased psdk pkgs from Git","reading",0.5F);

			_coreVersion = PsdkPkgsMgr.PsdkCoreVersion ();
			_installedPkgs = PsdkPkgsMgr.LocalIntstalledPsdkPkgsAndVersions ();
			_remoteCompatibleMinorVersion = PsdkPkgsMgr.RemoteCompatibleBranchMinorVersion ();
			if (_remoteCompatibleMinorVersion != null) {
				_noneInstalledCompatiblePkgs = PsdkPkgsMgr.NoneInstalledRemotePkgs (_remoteCompatibleMinorVersion);
			}

			string remoteLatestMajorBranch = PsdkPkgsMgr.FetchRemoteLatestMajorBranch ();
			if (remoteLatestMajorBranch != null) {
				Debug.LogWarning ("remoteLatestMajorBranch " + remoteLatestMajorBranch);
				string latestMinorVersion = PsdkPkgsMgr.FetchRemoteMinorVersion (remoteLatestMajorBranch);
				if (latestMinorVersion != null) {
					Debug.LogWarning ("latestMinorVersion " + latestMinorVersion);
					_remotePsdkLatestMinorVersion = latestMinorVersion;
					if (_remotePsdkLatestMinorVersion != null) {
						_noneInstalledLatestPkgs = PsdkPkgsMgr.NoneInstalledRemotePkgs (_remotePsdkLatestMinorVersion);
					}
					EditorUtility.ClearProgressBar();
					return;
				}
			}

			EditorUtility.ClearProgressBar();
			Debug.LogWarning ("Didn't find remote psdk site !");
		}

		string updateToMajorVersionStr = "fetch latest version";
		string _remoteCompatibleMinorVersion = null;
		IList<string> _noneInstalledCompatiblePkgs = null;
		IList<string> _noneInstalledLatestPkgs = null;
		string _coreVersion = null;

		void Update() {
			//FocusWindowIfItsOpen<PsdkImportWindow>();
		}

		void OnGUI ()
		{
			if (_coreVersion != null) {
				GUILayout.Label ("Current PSDK Core Version: " + _coreVersion, EditorStyles.boldLabel);
			}

			// update to latest same branch (compatible) version.
			if (_remoteCompatibleMinorVersion != null && PsdkPkgsMgr.MinimumInstalledPsdkVersion () != _remoteCompatibleMinorVersion) {
				if (GUILayout.Button ("Update installed packages to " + _remoteCompatibleMinorVersion)) {
					PsdkPkgsMgr.ImportRemotePkgsToMinorVersion (_remoteCompatibleMinorVersion);				
				}
			}

			// update to latest major version
			if (_remotePsdkLatestMinorVersion != null) {
				updateToMajorVersionStr = "Update to major version " + _remotePsdkLatestMinorVersion;
			
				if (_remoteCompatibleMinorVersion == null || _remotePsdkLatestMinorVersion != _remoteCompatibleMinorVersion) {
					if (GUILayout.Button (updateToMajorVersionStr)) {
						PsdkPkgsMgr.ImportRemotePkgsToMinorVersion (_remotePsdkLatestMinorVersion);
					}
				}
			}

			if (_installedPkgs != null && _installedPkgs.Count > 0) {
				GUILayout.Label ("PSDK installed packages:", EditorStyles.boldLabel);
				foreach (KeyValuePair<string,string> item in _installedPkgs) {
					if (item.Key.Contains("PSDKCore")) {
						if (GUILayout.Button ("Update " + item.Key + "\t: " + item.Value)) {
							PsdkPkgsMgr.ImportRemotePkgWithMinorVersion(item.Value,item.Key);
						}
					}
					else {
						if (GUILayout.Button ("Delete " + item.Key + "\t: " + item.Value)) {
							PSDKInstallerUninstall.UninstallPsdkUnityPackage(item.Key);
						}
					}
				}
			}
			// uninstalled compatible pkgs
			if (_noneInstalledCompatiblePkgs != null && _noneInstalledCompatiblePkgs.Count > 0) {
				GUILayout.Label ("PSDK uninstalled compatible packages:", EditorStyles.boldLabel);
				foreach (string item in _noneInstalledCompatiblePkgs) {
					if (GUILayout.Button ("Import " + item)) {
						PsdkPkgsMgr.ImportRemotePkgWithMinorVersion(_remoteCompatibleMinorVersion,item);
					}
				}
			}
			// uninstalled latest pkgs

			bool labeled = false;
			if (_noneInstalledCompatiblePkgs != null && _noneInstalledCompatiblePkgs.Count > 0) {
				foreach (string item in _noneInstalledLatestPkgs) {
					if (_noneInstalledCompatiblePkgs.Contains(item)) continue;
					if (! labeled) {
						labeled = true;
						GUILayout.Label ("PSDK uninstalled latest packages:", EditorStyles.boldLabel);
					}
					GUILayout.Label (item);
				}
			}

	
		}

	}
}
