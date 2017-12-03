using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission {

	public string Name;
	public Dictionary<string, float> RewardChances;
	public Dictionary<string, int> PossibleRewards;

	public List<ShipData> EnemyShips;

	public bool IsCastle;
	public int Seconds;
	public bool InProgress;
	GameManager gameManager;

	public int Stars;

	public Mission (string name, bool isCastle, Dictionary<string, float> rewardChances, Dictionary<string, int> possibleRewards, List<ShipData> enemyShips) {
		Name = name;
		IsCastle = isCastle;
		gameManager = GameManager.Instance;
		PossibleRewards = possibleRewards;
		RewardChances = rewardChances;
		EnemyShips = enemyShips;
		Seconds = 5;
		Stars = 0;
		Dictionary<string, int> rewards = GiveReward ();
		List<RewardChest> rewardChests = new List<RewardChest>();
		foreach (var amountByItem in rewards) {
			int [] vals = new int[enemyShips.Count];
			for (int i = 0; i < vals.Length; i++) {
				vals [i] = Mathf.RoundToInt ((float)amountByItem.Value / (float)vals.Length);
				vals [i] = (vals [i] == 0) ? 1 : vals [i];
			}
			foreach (var val in vals) {	
				rewardChests.Add (new RewardChest (new Dictionary<string, int> { { amountByItem.Key, val } }));
			}
		}

		Utility.Shuffle (rewardChests);

		do {
			AllocateRewards(EnemyShips, rewardChests);
		} while (rewardChests.Count > 0);
	}

	List<RewardChest> AllocateRewards (List<ShipData> ships, List<RewardChest> rewardChests) {
		List<RewardChest> rewardChestsToRemove = new List<RewardChest> ();
		int count = (ships.Count < rewardChests.Count) ? ships.Count : rewardChests.Count;
		for (int i = 0; i < count; i++) {
			ships [i].RewardChests.Add (rewardChests [i]);
			rewardChestsToRemove.Add (rewardChests [i]);
		}
		foreach (var rewardChest in rewardChestsToRemove) {
			rewardChests.Remove (rewardChest);
		}
		return rewardChests;
	}

	public Dictionary<string, int> GiveReward () {
		Dictionary<string, int> reward = new Dictionary<string, int> ();

		foreach (var chanceByItem in RewardChances) {
			if (Random.Range(0.0f, 1.0f) < chanceByItem.Value) {
				reward.Add (chanceByItem.Key, PossibleRewards [chanceByItem.Key]);
			}
		}

		return reward;
	}
}
