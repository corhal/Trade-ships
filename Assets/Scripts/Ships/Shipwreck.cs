﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shipwreck : Selectable {

	public List<RewardChest> RewardChests = new List<RewardChest> ();

	public List<string> Items;
	public List<int> ItemAmounts;

	protected override void Start () {
		base.Start ();
		Dictionary<string, int> rewardAmounts = new Dictionary<string, int> ();
		for (int i = 0; i < Items.Count; i++) {
			rewardAmounts.Add (Items [i], ItemAmounts [i]);
		}
		RewardChest rewardChest = new RewardChest (rewardAmounts);
		RewardChests.Add (rewardChest);
	}
}
