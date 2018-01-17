using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shipwreck : PointOfInterest {

	public RewardChest RewardChest;

	//public List<RewardChest> RewardChests = new List<RewardChest> ();

	//public List<string> Items;
	//public List<int> ItemAmounts;

	bool revealed;

	void Start () {
		RewardChest = new RewardChest ();
		if (Player.Instance.CurrentAdventure.TreasureHunt && !revealed) {
			gameObject.SetActive (false);
		}
		/*Dictionary<string, int> rewardAmounts = new Dictionary<string, int> ();
		for (int i = 0; i < Items.Count; i++) {
			rewardAmounts.Add (Items [i], ItemAmounts [i]);
		}
		RewardChest rewardChest = new RewardChest (rewardAmounts);
		RewardChests.Add (rewardChest);*/
	}

	public void Reveal () {
		if (!POIData.Interacted) {
			gameObject.SetActive (true);
			Tile.StopParticles ();
			revealed = true;
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
			playerShip.RewardChests.Add (RewardChest);
			Interact ();
			UIOverlay.Instance.UpdateShipRewardChests (playerShip);
			gameObject.SetActive (false);
		} 
	}
}
