﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChestState {
	Closed, Opening, Open
}
// [System.Serializable]
public class RewardChest {

	public int SecondsLeft;
	public int SecondsToOpen;
	public ChestState ChestState;

	public Dictionary<string, int> RewardItems;

	public string Ocean;

	public Dictionary<string, List<int>> OpenTimesByOcean = new Dictionary<string, List<int>> {
		{"Ocean 1", new List<int> { 1800, 3600 }},
		{"Ocean 2", new List<int> { 7200, 14400 }},
		{"Ocean 3", new List<int> { 10800, 36000 }}
	};

	public RewardChest (bool goldOnly, bool shardsOnly) {
		Dictionary<string, int> possibleRewards = new Dictionary<string, int> ();

		// System.Random rand = new System.Random ();
		if (!goldOnly) {
			int costLength = /*rand.Next (1, 6);*/ Random.Range (1, 3);

			for (int j = 0; j < costLength; j++) {
				List<Item> validItems = new List<Item> ();
				foreach (var item in Player.Instance.DataBase.TempItemLibrary) {
					if (!possibleRewards.ContainsKey (item.Name)) {
						validItems.Add (item);
					}
				}

				int index = /*rand.Next (0, validItems.Count - 1);*/ Random.Range (0, validItems.Count - 1);
				possibleRewards.Add (validItems [index].Name, /*rand.Next (1, 10)*/Random.Range (1, 5));
			}
		}

		if (!shardsOnly) {
			possibleRewards.Add ("Gold", Random.Range (10, 100));
		}

		Ocean = Player.Instance.CurrentAdventure.Ocean;
		List<int> seconds = OpenTimesByOcean [Player.Instance.CurrentAdventure.Ocean];
		int randIndex = Random.Range(0, seconds.Count);
		SecondsToOpen = seconds [randIndex];
		RewardItems = new Dictionary<string, int> (possibleRewards);
		ChestState = ChestState.Closed;
	}

	public RewardChest (Dictionary<string, int> rewardItems) {
		RewardItems = new Dictionary<string, int> (rewardItems);
		ChestState = ChestState.Closed;
	}

	public void StartOpen () {
		SecondsLeft = SecondsToOpen;
		ChestState = ChestState.Opening;
	}

	public void TickOpen (int amount) {
		SecondsLeft -= amount;
		if (SecondsLeft <= 0) {
			Open ();
		}
	}

	public void Open () {
		ChestState = ChestState.Open;
		Player.Instance.OpenChest (this);
	}
}
