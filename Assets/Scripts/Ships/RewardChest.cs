using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChest {

	public Dictionary<Item, int> RewardItems;

	public RewardChest (Dictionary<Item, int> rewardItems) {
		RewardItems = new Dictionary<Item, int> (rewardItems);
	}
}
