using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MakeAction();

public class Action {

	public string Name;
	public int Cost;
	public Sprite Icon;

	public MakeAction DelegateMakeAction;

	public Action (string name, int cost, Sprite icon, MakeAction makeAction) {
		this.Name = name;
		this.Cost = cost;
		this.Icon = icon;
		this.DelegateMakeAction = makeAction;
	}

	public void Execute () {
		DelegateMakeAction ();
	}
}
