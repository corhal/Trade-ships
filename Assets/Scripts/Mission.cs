using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission {

	public string Name;
	//public Dictionary<string, float> RewardChances;
	public Dictionary<string, int> PossibleRewards;

	public List<CreatureData> EnemyShips;

	public bool IsCastle;
	public int Seconds;
	public bool InProgress;

	public int Stars;

	public Mission (string name, bool isCastle, /*Dictionary<string, float> rewardChances,*/ Dictionary<string, int> possibleRewards, List<CreatureData> enemyShips) {
		Name = name;
		IsCastle = isCastle;
		PossibleRewards = possibleRewards;
		//RewardChances = rewardChances;
		EnemyShips = enemyShips;
		Seconds = 5;
		Stars = 0;
	}

	/*public Dictionary<string, int> GiveReward () {
		Dictionary<string, int> reward = new Dictionary<string, int> ();

		foreach (var chanceByItem in RewardChances) {
			if (Random.Range(0.0f, 1.0f) < chanceByItem.Value) {
				reward.Add (chanceByItem.Key, PossibleRewards [chanceByItem.Key]);
			}
		}

		return reward;
	}*/
}
