using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureSelectionWindow : MonoBehaviour {

	public GameObject Window;
	public Text WindowText;

	public void Open () {		
		Window.SetActive (true);
		if (Player.Instance.OnAdventure) {
			WindowText.text = "Let's go back!";
		} else {
			WindowText.text = "Let's go on adventure!";
		}
	}

	public void StartAdventure () {
		Close ();
		if (Player.Instance.OnAdventure) {
			Player.Instance.LoadVillage ();
		} else {
			Player.Instance.LoadAdventure (7327.0f);
		}
	}


	public void Close () {
		Window.SetActive (false);
	}

	public void Back () {

	}
}
