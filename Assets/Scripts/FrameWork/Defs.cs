using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Defs {
	public static readonly string androidApp_ID = "com.crazylabs.monsteryumm";
	public static readonly string iOSApp_ID = "id1192223024";

	public static MyNativeShare share;
	public static Share shareVoxel;
	public static Rate rate;

	// Sound
	public static AudioSource audioSource;
	public static AudioSource audioSourceMusic;
	public static bool mute = false;

	static public void PlaySound(AudioClip _audioClip, float _volume = 1f) {
		audioSource.volume = _volume * AudioListener.volume;
		if (audioSource.volume > 0f) {
			audioSource.PlayOneShot (_audioClip);
		}
	}

	static public void MuteSounds(bool _flag) {
		if (mute == _flag)
			return;

		mute = _flag;

		if (mute) {
			AudioListener.volume = 0f;
		} else {
			AudioListener.volume = PlayerPrefs.GetFloat ("SoundVolume", 1f);
			if (AudioListener.volume > 0f) {
				audioSourceMusic.Play ();
			}
		}
	}

	static public void SwitchSounds() {
		if (AudioListener.volume > 0f) {
			//Camera.main.GetComponent<AudioListener> ().enabled = false;
			AudioListener.volume = 0f;
			//audioSource.enabled = false;
			//audioSourceMusic.enabled = false;
			D.Log ("Sound OFF");
		} else {
			//Camera.main.GetComponent<AudioListener> ().enabled = true;
			AudioListener.volume = 1f;
			//audioSource.enabled = true;
			//audioSourceMusic.enabled = true;
			audioSourceMusic.Play ();
			D.Log ("Sound ON");
		}

		PlayerPrefs.SetFloat ("SoundVolume", AudioListener.volume);
	}
}
