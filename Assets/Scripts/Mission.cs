using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission {

	public Dictionary<Item, float> RewardChances;
	public Dictionary<Item, int> PossibleRewards;
	public Dictionary<string, int> BuildingRequirements;

	public int Seconds;
	public int Power;
	public bool InProgress;
	GameManager gameManager;

	public Mission () {
		gameManager = GameManager.Instance;
		PossibleRewards = new Dictionary<Item, int> ();
		RewardChances = new Dictionary<Item, float> ();
		int costLength = Random.Range (1, 5);
		for (int j = 0; j < costLength; j++) {
			List<Item> validItems = new List<Item> ();
			foreach (var item in gameManager.TempItemLibrary) {
				if (!PossibleRewards.ContainsKey(item)) {
					validItems.Add (item);
				}
			}

			int index = Random.Range (0, validItems.Count);

			PossibleRewards.Add (validItems [index], Random.Range(1, 6));
			RewardChances.Add (validItems [index], Random.Range (0.3f, 0.7f));
		}

		BuildingRequirements = new Dictionary<string, int> ();
		BuildingRequirements.Add ("Lumbermill", 2);
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
