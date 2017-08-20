using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MakeAction();

public class Action {

	public string Name;
	public int Cost;

	public MakeAction makeAction;

	public void InitAction (MakeAction method) {
		makeAction = method;
	}

	public void Execute () {
		makeAction ();
	}
}
