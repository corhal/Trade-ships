﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour {

	public string Name;
	public Port MyPort = null;

	public List<Building> Buildings;

	void Awake () {
		MyPort = GetComponentInChildren<Port> ();
		Buildings = new List <Building> (GetComponentsInChildren<Building> ());
	}

	public void Claim () {
		// MyPort.Claim ();
		foreach (var building in Buildings) {
			building.Claim ();
		}
	}
}
