using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour {
	public Sprite spriteOn;
	public Sprite spriteOff;
	// Use this for initialization

	void Awake()
	{
//		PublishingService.Instance.OnEnableMusic += OnEnableMusic;
//		PublishingService.Instance.OnDisableMusic += OnDisableMusic;
	}

	void OnDestroy()
	{
//		PublishingService.Instance.OnEnableMusic -= OnEnableMusic;
//		PublishingService.Instance.OnDisableMusic -= OnDisableMusic;
	}

	private void OnEnableMusic()
	{
		Defs.MuteSounds (false);
	}

	private void OnDisableMusic()
	{
		Defs.MuteSounds (true);
	}

	void Start () {
		AudioListener.volume = PlayerPrefs.GetFloat ("SoundVolume", 1f);
		SetSoundImage ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Click() {
		Defs.SwitchSounds ();
		SetSoundImage ();
	}

	void SetSoundImage() {
		if (AudioListener.volume > 0f) {
			GetComponent<Image> ().sprite = spriteOn;
		} else {
			GetComponent<Image> ().sprite = spriteOff;
		}
	}
}
