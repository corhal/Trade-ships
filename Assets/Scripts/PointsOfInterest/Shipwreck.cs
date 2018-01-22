using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shipwreck : PointOfInterest {

	public RewardChest RewardChest;

	void Start () {
		RewardChest = new RewardChest ();
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
		if (playerShip.RewardChests.Count < playerShip.RewardChestsCapacity) {			
			Interact ();
			playerShip.TakeChestReward (RewardChest);
			gameObject.SetActive (false);
		} 
	}
}
