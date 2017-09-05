using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission {

	public Dictionary<Item, float> RewardChances;
	public Dictionary<Item, int> PossibleRewards;
	public Dictionary<string, int> BuildingRequirements;

	public int Seconds;
	public int Power;
	public bool InProgress;
	GameManager gameManager;

	public Mission (Dictionary<Item, float> rewardChances, Dictionary<Item, int> possibleRewards) {
		gameManager = GameManager.Instance;
		PossibleRewards = possibleRewards;
		RewardChances = rewardChances;

		BuildingRequirements = new Dictionary<string, int> ();
		BuildingRequirements.Add ("Lumbermill", 1);
		BuildingRequirements.Add ("Quarry", 1);
		Seconds = 20;
		Power = Random.Range (100, 200);
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
