using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {

	Player player;
	GameManager gameManager;
	public static PlayerShip Instance;
	MoveOnClick mover;

	public int EnergyPerDistance;

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
		player = Player.Instance;
		gameManager = GameManager.Instance;
		mover = GetComponent<MoveOnClick> ();
	}

	void Start () {
		mover.EnergyPerDistance = EnergyPerDistance;
	}

	void OnTriggerEnter2D (Collider2D other) { // will work even when passing through
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
		} else if (other.gameObject.GetComponent<Building> () != null && other.gameObject.GetComponent<Building> ().Allegiance == Allegiance.Neutral) {
			gameManager.OpenPopUp ("You claimed the island of <color=blue>" + other.gameObject.GetComponent<Building> ().MyIsland.Name + "</color> and all its buildings!");
			foreach (var building in other.gameObject.GetComponent<Building> ().MyIsland.Buildings) {
				building.Allegiance = Allegiance.Player;
			}
		}
	}

	public void MoveToPoint (Vector2 target) {
		mover.MoveToPoint (target);
	}
}
