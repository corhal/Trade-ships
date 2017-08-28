using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission {

	public Dictionary<Item, int> PossibleRewards;
	public Dictionary<string, int> BuildingRequirements;

	public int CargoRequirements;
	GameManager gameManager;

	public Mission () {
		gameManager = GameManager.Instance;
		PossibleRewards = new Dictionary<Item, int> ();

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
		}

		BuildingRequirements = new Dictionary<string, int> ();
		BuildingRequirements.Add ("Lumbermill", 2);
		BuildingRequirements.Add ("Quarry", 1);

		CargoRequirements = 40;
	}
}
