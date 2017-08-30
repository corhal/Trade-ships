using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour {

	public string Name;
	public Port MyPort = null;

	void Awake () {
		MyPort = GetComponentInChildren<Port> ();
	}
}
