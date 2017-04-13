using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMulti : Bubble {

	// Use this for initialization
	void Start () {

	}

	override protected void Init() {
		base.Init ();
		id = DefsGame.BUBBLE_COLOR_MULTI;
	}
	
	// Update is called once per frame
	void Update () {
		base.BubbleUpdate ();
	}

	override public bool isEqualTo(int _id) 
	{
		if (_id == id) DefsGame.wowSlider.MakeX3 (0.2f);
		DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_MULTI_PULTI, 1);
		return true;
	}
}
