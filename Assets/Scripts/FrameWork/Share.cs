using UnityEngine;
using VoxelBusters.NativePlugins;

public class Share : MonoBehaviour {

	void Start() {
		Defs.shareVoxel = this;
	}

	public void ShareClick ()
	{
		string _shareLink = "http://smarturl.it/YummMonsters";

		#if UNITY_IOS

		_shareLink = "http://smarturl.it/YummMonsters";
		#endif

		string shareText = "Wow! I Just Scored ["+DefsGame.gameBestScore.ToString()+ "] in #YummMonsters! Can You Beat Me? @AppsoluteGames " + _shareLink;


		string _screenShotPath = Application.persistentDataPath + "/promo1.jpg";

		if (Random.value > 0.5f) {
			_screenShotPath = Application.persistentDataPath + "/promo2.jpg";
		}

		ShareImageAtPathUsingShareSheet (shareText, _screenShotPath);
	}

	void ShareImageAtPathUsingShareSheet(string _shareText, string _screenShotPath) {
		// Create share sheet
		ShareSheet shareSheet 	= new ShareSheet();

		shareSheet.Text = _shareText;
		shareSheet.AttachImageAtPath(_screenShotPath);

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(shareSheet, FinishedSharing);
	}

	void FinishedSharing (eShareResult _result)
	{
		Debug.Log("Finished sharing");
		Debug.Log("Share Result = " + _result);
	}
}