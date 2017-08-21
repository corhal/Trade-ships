using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MakeAction();

public class Action {

	public string Name;
	public int Cost;

	public MakeAction DelegateMakeAction;

	public Action (string name, int cost, MakeAction makeAction) {
		this.Name = name;
		this.Cost = cost;
		this.DelegateMakeAction = makeAction;
	}

	public void Execute () {
		DelegateMakeAction ();
	}
}
