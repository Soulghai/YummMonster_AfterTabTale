using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

/*
 * https://github.com/ChrisMaire/unity-native-sharing 
 */

public class MyNativeShare : MonoBehaviour {
	//public string ScreenshotName = "promo1.jpg";
	public Texture2D image;
	public Texture2D image2;

	void Awake() {
		Defs.share = this;
		//Save Image
		byte[] bytes = image.EncodeToPNG();
		string path = Application.persistentDataPath + "/promo1.jpg";
		File.WriteAllBytes(path, bytes);

		bytes = image2.EncodeToPNG();
		path = Application.persistentDataPath + "/promo2.jpg";
		File.WriteAllBytes(path, bytes);
		D.Log ("NativeShare.Awake()");
	}

	public void ShareScreenshotWithText(string text, string _url)
    {
        //string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
		string screenShotPath = Application.persistentDataPath + "/promo1.jpg";

		if (Random.value > 0.5f) {
			screenShotPath = Application.persistentDataPath + "/promo2.jpg";
		}

        //Application.CaptureScreenshot(ScreenshotName);

		Share(text,screenShotPath,_url);
    }

	public void ShareClick() {
		D.Log ("NativeShare.ShareClick()");


		string _shareLink = "https://play.google.com/store/apps/details?id=com.crazylabs.monsteryumm";

		#if UNITY_IOS
		_shareLink = "http://itunes.apple.com/app/id1192223024";
		#endif

		string _shareText = "Wow! I Just Scored ["+DefsGame.gameBestScore.ToString()+ "] in #YummMonsters! Can You Beat Me? @tabtale " + _shareLink;
		ShareScreenshotWithText (_shareText, "");
	}

	public void Share(string shareText, string imagePath, string url, string subject = "")
	{
#if UNITY_ANDROID
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + imagePath);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
		intentObject.Call<AndroidJavaObject>("setType", "image/png");
		
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);
		
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		
		AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, subject);
		currentActivity.Call("startActivity", jChooser);
#elif UNITY_IOS
		CallSocialShareAdvanced(shareText, subject, url, imagePath);
#else
		Debug.Log("No sharing set up for this platform.");
#endif
	}

#if UNITY_IOS
	public struct ConfigStruct
	{
		public string title;
		public string message;
	}

	[DllImport ("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);
	
	public struct SocialSharingStruct
	{
		public string text;
		public string url;
		public string image;
		public string subject;
	}
	
	[DllImport ("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);
	
	public static void CallSocialShare(string title, string message)
	{
		ConfigStruct conf = new ConfigStruct();
		conf.title  = title;
		conf.message = message;
		showAlertMessage(ref conf);
	}

	public static void CallSocialShareAdvanced(string defaultTxt, string subject, string url, string img)
	{
		SocialSharingStruct conf = new SocialSharingStruct();
		conf.text = defaultTxt; 
		conf.url = url;
		conf.image = img;
		conf.subject = subject;
		
		showSocialSharing(ref conf);
	}
#endif
}
