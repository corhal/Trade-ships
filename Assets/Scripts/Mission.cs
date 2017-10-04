using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission {

	public Dictionary<Item, float> RewardChances;
	public Dictionary<Item, int> PossibleRewards;
	public List<Shipment> Shipments;
	public List<ShipData> EnemyShips;

	public int Seconds;
	public bool InProgress;
	GameManager gameManager;

	public Mission (Dictionary<Item, float> rewardChances, Dictionary<Item, int> possibleRewards, List<ShipData> enemyShips) {
		gameManager = GameManager.Instance;
		PossibleRewards = possibleRewards;
		RewardChances = rewardChances;
		EnemyShips = enemyShips;
		Seconds = 5;

		Dictionary<Item, int> rewards = GiveReward ();
		List<Shipment> shipments = new List<Shipment> ();

		foreach (var amountByItem in rewards) {
			int [] vals = new int[enemyShips.Count];
			for (int i = 0; i < vals.Length; i++) {
				vals [i] = Mathf.RoundToInt ((float)amountByItem.Value / (float)vals.Length);
				vals [i] = (vals [i] == 0) ? 1 : vals [i];
			}
			foreach (var val in vals) {				
				shipments.Add(new Shipment (amountByItem.Key, gameManager.Islands [0].Name, gameManager.Islands [1].Name, val, Random.Range (1, 5)));
			}
		}

		Utility.Shuffle (shipments);

		for (int j = 0; j < 5; j++) {
			for (int i = shipments.Count - 1; i >= 0; i--) {
				foreach (var ship in EnemyShips) {
					if (ship.CanTakeShipment(shipments[i])) {
						ship.TakeShipment (shipments [i]);
						shipments.Remove (shipments [i]);
						break;
					}
				}
			}
			if (shipments.Count == 0) {
				break;
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
