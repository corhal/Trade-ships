using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : PointOfInterest {

	public RewardChest RewardChest;

	void Start () {
		RewardChest = new RewardChest (false, true);
		if (Player.Instance.CurrentAdventure.TreasureHunt && !POIData.Revealed) {
			gameObject.SetActive (false);
		}
	}

	public void Reveal () {
		if (!POIData.Interacted) {
			gameObject.SetActive (true);
			Tile.StopParticles ();
			POIData.Revealed = true;
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.GetComponent<PlayerShip> () != null) {
			Invoke ("GiveReward", 0.25f);
		}
	}

	void GiveReward () {
		PlayerShip playerShip = GameManager.Instance.PlayerShip;
		if (Player.Instance.RewardChests.Count < playerShip.RewardChestsCapacity) {			
			Interact ();
			UIOverlay.Instance.FlyReward (GetComponentInChildren<SpriteRenderer> ().sprite, transform, UIOverlay.Instance.ChestsLabel.gameObject /*UIOverlay.Instance.ChestButtons [0].gameObject*/);
			playerShip.TakeChestReward (RewardChest);
			gameObject.SetActive (false);
		}
		/*Player.Instance.ReceiveReward (RewardChest.RewardItems);
		Interact ();
		gameObject.SetActive (false);*/
	}
}
