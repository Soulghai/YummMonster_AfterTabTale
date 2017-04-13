using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGreen : Bubble {

	// Use this for initialization
	void Start () {
		
	}

	override protected void Init() {
		base.Init ();
		id = DefsGame.BUBBLE_COLOR_THREE;
	}
	
	// Update is called once per frame
	void Update () {
		base.BubbleUpdate ();
	}
}
