using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
	public Text _debugText;

	void OnEnable () {
		GlobalEvents<OnDebugLog>.Happened += UpdateText;
	}

	private void UpdateText(OnDebugLog e)
	{
		_debugText.text += e.message + "\n";
	}
}
