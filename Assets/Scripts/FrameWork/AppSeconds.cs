using UnityEngine;
using System.Collections;

public class AppSeconds : MonoBehaviour 
{
	void Start()
	{
		StartCoroutine (SaveSeconds());
	}

	IEnumerator SaveSeconds()
	{
		while (true)
		{
			yield return new WaitForSecondsRealtime (1f);
			SaveSeconds (GetSeconds() + 1);
		}
	}

	void SaveSeconds (int seconds)
	{
		seconds = Mathf.Clamp (seconds, 0, int.MaxValue);
		PlayerPrefs.SetInt ("GameTotalTime", seconds);
	}

	public static int GetSeconds()
	{
		int seconds = 0;
		seconds = PlayerPrefs.GetInt ("GameTotalTime", 0);
		seconds = Mathf.Clamp (seconds, 0, int.MaxValue);
		return seconds;
	}
}
