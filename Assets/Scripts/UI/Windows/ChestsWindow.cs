using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestsWindow : MonoBehaviour {

	public GameObject Window;

	public void Open () {
		Window.SetActive (true);
	}

	public void Close () {
		Window.SetActive (false);
	}
}
