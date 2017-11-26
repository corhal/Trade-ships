using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {

	Player player;
	GameManager gameManager;

	void Awake () {
		player = Player.Instance;
		gameManager = GameManager.Instance;
	}

	void OnTriggerEnter2D (Collider2D other) { // will work even when passing through another port
		if (other.gameObject.GetComponent<Shipwreck> () != null) {
			Dictionary<string, int> rewards = new Dictionary<string, int> ();
			foreach (var rewardChest in other.gameObject.GetComponent<Shipwreck> ().RewardChests) {
				player.TakeItems (rewardChest.RewardItems);
				foreach (var amountByItem in rewardChest.RewardItems) {
					if (!rewards.ContainsKey(amountByItem.Key)) {
						rewards.Add (amountByItem.Key, amountByItem.Value);
					} else {
						rewards [amountByItem.Key] += amountByItem.Value;
					}
				}
			}
			gameManager.OpenImagesPopUp ("Reward: ", rewards);
			Destroy (other.gameObject);
		}
	}
}
