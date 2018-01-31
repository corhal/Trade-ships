using System.Collections;
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

	public RewardChest () {
		// System.Random rand = new System.Random ();
		int costLength = /*rand.Next (1, 6);*/ Random.Range (1, 3);

		Dictionary<string, int> possibleRewards = new Dictionary<string, int> ();

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
		List<int> seconds = new List<int> { 10, /*600, 10800, 36000*/ };
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

	public void TickOpen () {
		SecondsLeft--;
		if (SecondsLeft <= 0) {
			Open ();
		}
	}

	public void Open () {
		ChestState = ChestState.Open;
		Player.Instance.OpenChest (this);
	}
}
