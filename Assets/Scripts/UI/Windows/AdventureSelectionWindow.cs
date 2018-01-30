using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureSelectionWindow : MonoBehaviour {

	public GameObject Window;
	public GameObject StashChestsButton;
	public Text WindowText;

	public void Open () {		
		Window.SetActive (true);
		if (Player.Instance.OnAdventure) {
			WindowText.text = "Go back?";
			//StashChestsButton.SetActive (true);
		} else {
			WindowText.text = "Let's go on adventure!";
			//StashChestsButton.SetActive (false);
		}
	}

	public void StartAdventure () {
		Close ();
		if (Player.Instance.OnAdventure) {
			StashChests ();
			Player.Instance.LoadVillage ();
		} else {
			Player.Instance.NewBoard = true;
			Player.Instance.LoadAdventure ();
		}
	}

	public void StashChests () {
		foreach (var rewardChest in GameManager.Instance.PlayerShip.RewardChests) {
			Player.Instance.RewardChests.Add (rewardChest);
		}
		GameManager.Instance.PlayerShip.RewardChests.Clear ();
		// UIOverlay.Instance.UpdateShipRewardChests (GameManager.Instance.PlayerShip);
		Close ();
	}


	public void Close () {
		Window.SetActive (false);
	}

	public void Back () {

	}
}
