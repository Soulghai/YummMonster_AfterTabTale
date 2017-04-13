using UnityEngine;
using System.Collections;


class AndroidPermissionsWrapperDelegate : AndroidJavaProxy
{
	public AndroidPermissionsWrapperDelegate() : base("com.tabtale.publishingsdk.core.utils.permissions.AndroidPermissionsWrapperDelegate") { }

	public void onRequestPermissionsResult(string message)
	{
		var mi = AndroidPermissionsWrapper.Instance.GetType ().GetMethod ("OnRequestPermissionsResult", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		if (mi != null) {
			mi.Invoke (AndroidPermissionsWrapper.Instance, new object[] {message});
		}
	}
}

public class AndroidPermissionsWrapper {

	public event System.Action<string[],bool[]> OnRequestPermissionsResultEvent;

	private static AndroidPermissionsWrapper _instace = null;

	private AndroidJavaObject _androidJavaObject;
		
	public static AndroidPermissionsWrapper Instance {
		get {
			if(_instace == null){
				_instace = new AndroidPermissionsWrapper ();
				if(!_instace.createJavaObject ()){
					_instace = null;
					return null;
				}
			}
			return _instace;
		}
	}

	private bool createJavaObject()
	{
		if(_androidJavaObject == null){
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			if (jo != null) {
				AndroidPermissionsWrapperDelegate permissionsDelegate = new AndroidPermissionsWrapperDelegate ();
				if (permissionsDelegate != null) {
					_androidJavaObject = new AndroidJavaObject ("com.tabtale.publishingsdk.core.utils.permissions.AndroidPermissionsWrapper", jo,permissionsDelegate);
				} else {
					Debug.LogError ("failed to create delegate java object.");
					return false;
				}
			} 
			else {
				Debug.LogError ("failed to get activity java object.");
				return false;
			}
		}

		if(_androidJavaObject == null){
			Debug.LogError ("was not able to initiate java object.");
			return false;
		}
		return true;
	}

	private void OnRequestPermissionsResult(string message)
	{
		string[] messageArr = message.Split (';');
		string[] permissions = new string[messageArr.Length/2];
		bool[] granted = new bool[messageArr.Length / 2];

		int i = 0;
		int j = 0;
		foreach (string str in messageArr){
			if(i % 2 == 0){
				permissions [i] = str;
			}
			else {
				bool isGranted = false;
				bool.TryParse (messageArr [i], out isGranted);
				granted [j] = isGranted;
				j++;
			}
			i++;
		}

		OnRequestPermissionsResultEvent (permissions, granted);
	}

	public bool CheckSelfPermission(string permission)
	{
		if(_androidJavaObject != null){
			return _androidJavaObject.Call<bool> ("checkSelfPermission", permission);
		}
		else {
			Debug.LogError ("java object is not initiated.");
		}
		return false;
	}

	public bool ShouldShowRequestPermissionRationale(string permission)
	{
		if(_androidJavaObject != null){
			return _androidJavaObject.Call<bool> ("shouldShowRequestPermissionRationale", permission);
		}
		else {
			Debug.LogError ("java object is not initiated.");
		}
		return false;
	}

	public void RequestPermissions(string[] permissions)
	{
		if(_androidJavaObject != null && permissions != null){
			string permissionStr = "";
			foreach(string permission in permissions){
				permissionStr += permission + ";";
			}
			permissionStr.TrimEnd (';');
			_androidJavaObject.Call("requestPermissions", permissionStr);
		}
		else {
			Debug.LogError ("java object is not initiated.");
		}
	}
}
