using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewardChest {

	public Dictionary<string, int> RewardItems;

	public RewardChest (Dictionary<string, int> rewardItems) {
		RewardItems = new Dictionary<string, int> (rewardItems);
	}
}
