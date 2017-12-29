using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstaclePopUp : MonoBehaviour {

	public GameObject Window;

	public Text MessageText;
	public Text ButtonText;

	int energyPerTry;
	int tries;

	public void Open (Obstacle obstacle) {
		Window.SetActive (true);

		energyPerTry = obstacle.EnergyPerTry;
		tries = obstacle.Tries;
		RefreshLabels ();
	}

	void RefreshLabels () {
		MessageText.text = "Try " + tries + " more times to get free!";
		ButtonText.text = "Try for " + energyPerTry + " energy";
	}

	public void Try () {
		if (Player.Instance.Energy - energyPerTry >= 0) {
			Player.Instance.Energy -= energyPerTry;
			tries--;
			RefreshLabels ();
		} else {
			UIOverlay.Instance.OpenPopUp ("Not enough energy, but I'll give you 10");
			Player.Instance.Energy += 10;
			RefreshLabels ();
		}

		if (tries == 0) {
			Close ();
		}
	}

	public void Close () {
		Window.SetActive (false);
	}
}
