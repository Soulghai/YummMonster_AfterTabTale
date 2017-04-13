#if UNITY_ANDROID 
using UnityEngine;
using System.Collections;
using System.Reflection;
using TabTale.Plugins.PSDK;

public class UnitySyncDelegate : AndroidJavaProxy {

	public UnitySyncDelegate() : base("com.tabtale.publishingsdk.services.UnitySyncDelegate") { }

	public void sendSyncMessage(string methodName,string message)
	{
		Debug.Log("UnitySyncDelegate::sendSyncMessage - " + methodName + " - " + message);
		if(methodName != null){
			MethodInfo mi = PsdkEventSystem.Instance.GetType ().GetMethod (methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(mi != null){
				mi.Invoke (PsdkEventSystem.Instance,message.Length > 0 ? new object[]{ message } : null);
			}
		}
	}
}
#endif