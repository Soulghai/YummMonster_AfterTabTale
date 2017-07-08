using UnityEngine;

public class ScreenColorAnimation : MonoBehaviour {
	SpriteRenderer spr;
	bool isShowAnimation;
	bool isHideAnimation;
	float alphaMax;
	float speed;
	//private var funcClose:Function;
	bool isAutoHide;
	bool isAnimation;

	// Use this for initialization
	void Start () {
		spr = GetComponent<SpriteRenderer> ();
		isShowAnimation = false;
		isHideAnimation = false;
		alphaMax = 0.8f;
		isAutoHide = false;
		speed = 0.1f;
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

		if (isShowAnimation) {
			Color _color = spr.color;
				if (_color.a < alphaMax) _color.a += speed;
			else {
					_color.a = alphaMax;
				isShowAnimation = false;
				if (isAutoHide) Hide();
			}
			spr.color = _color;
		}

		if (isHideAnimation) {
			Color _color = spr.color;
			if (_color.a > 0f) _color.a -= speed;
			else {
				_color.a = 0f;
				isHideAnimation = false;
				gameObject.SetActive(false);
			}
			spr.color = _color;
		}

	}

	public void SetColor(float _red, float _green, float _blue) {
		spr.color = new Color (_red, _green, _blue, spr.color.a);
	}

	public void SetAlphaMax(float _value) {
		alphaMax = _value;
	}

	public void SetAutoHide(bool _flag) {
		isAutoHide = _flag;
	}

	public void SetAnimation(bool _flag, float _speed = 0.05f) {
		isAnimation = _flag;
		speed = _speed;
	}

	//public void setExitByClick(_func:Function) {
	//	funcClose = _func;
	//	bmp.addEventListener(MouseEvent.CLICK, funcMouseClick, false, 0, true);
	//}

	void OnMouseUp() { 
		//if (funcClose != null) {
		//	funcClose();
		//	funcClose = null;
		//}
	}

	public void Show() {
		isShowAnimation = true;
		isHideAnimation = false;
		Color _color = spr.color;
		if (isAnimation) {
			_color.a = 0f;
		} else {
			_color.a = alphaMax;
		}
		spr.color = _color;
		gameObject.SetActive(true);
	}

	public void Hide() {
		isHideAnimation = true;
		isShowAnimation = false;
		Color _color = spr.color;
		if (isAnimation) {
			_color.a = alphaMax;
		} else {
			_color.a = 0f;
			gameObject.SetActive(false);
		}
		spr.color = _color;
	}
}
