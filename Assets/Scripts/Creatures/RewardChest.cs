using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
public class RewardChest {

	public Dictionary<string, int> RewardItems;

	public RewardChest () {
		// System.Random rand = new System.Random ();
		int costLength = Random.Range (1, 6);

		Dictionary<string, int> possibleRewards = new Dictionary<string, int> ();

		for (int j = 1; j < costLength; j++) {
			List<Item> validItems = new List<Item> ();
			foreach (var item in Player.Instance.DataBase.TempItemLibrary) {
				if (!possibleRewards.ContainsKey (item.Name)) {
					validItems.Add (item);
				}
			}

			int index = Random.Range (0, validItems.Count - 1);
			possibleRewards.Add (validItems [index].Name, Random.Range (1, 10));
		}
		RewardItems = new Dictionary<string, int> (possibleRewards);
	}

	public RewardChest (Dictionary<string, int> rewardItems) {
		RewardItems = new Dictionary<string, int> (rewardItems);
	}
}
