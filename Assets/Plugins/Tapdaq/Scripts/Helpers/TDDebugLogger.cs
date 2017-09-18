using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tapdaq;

public class TDDebugLogger {

	private static TDSettings settings = null;

	private static void Init() {
		if (settings == null && Resources.LoadAll<TDSettings> ("Tapdaq").Length > 0) {
			settings = Resources.LoadAll<TDSettings> ("Tapdaq")[0];
		}
	}

	public static void Log(object obj) {
		Init ();

		if (settings != null && settings.isDebugMode) {
			Debug.Log (obj);
		}
	}

	public static void LogWarning(object obj) {
		Init ();

		if (settings != null && settings.isDebugMode) {
			Debug.LogWarning (obj);
		}
	}

	public static void LogError(object obj) {
		Init ();

		if (settings != null && settings.isDebugMode) {
			Debug.LogError (obj);
		}
	}

	public static void LogException(System.Exception obj) {
		Init ();

		if (settings != null && settings.isDebugMode) {
			Debug.LogException (obj);
		}
	}
}
