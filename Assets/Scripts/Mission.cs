using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission {

	public Dictionary<Item, float> RewardChances;
	public Dictionary<Item, int> PossibleRewards;
	public List<Shipment> Shipments;

	public int Seconds;
	public bool InProgress;
	GameManager gameManager;

	public Mission (Dictionary<Item, float> rewardChances, Dictionary<Item, int> possibleRewards, List<ShipData> enemyShips) {
		gameManager = GameManager.Instance;
		PossibleRewards = possibleRewards;
		RewardChances = rewardChances;

		Seconds = 5;

		Dictionary<Item, int> rewards = GiveReward ();
		List<Shipment> shipments = new List<Shipment> ();

		foreach (var amountByItem in rewards) {
			int [] vals = new int[enemyShips.Count];
			for (int i = 0; i < vals.Length; i++) {
				vals [i] = Mathf.RoundToInt (amountByItem.Value / vals.Length);
			}
			foreach (var val in vals) {
				shipments.Add(new Shipment (amountByItem.Key, gameManager.Islands [0].Name, gameManager.Islands [1].Name, val, Random.Range (1, 5)));
			}
		}
	}

	public Dictionary<Item, int> GiveReward () {
		Dictionary<Item, int> reward = new Dictionary<Item, int> ();

		foreach (var chanceByItem in RewardChances) {
			if (Random.Range(0.0f, 1.0f) < chanceByItem.Value) {
				reward.Add (chanceByItem.Key, PossibleRewards [chanceByItem.Key]);
			}
		}

		return reward;
	}
}
