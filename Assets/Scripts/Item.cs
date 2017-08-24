using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

	public string Name;
	public Dictionary<Item, int> CraftCost;
	Player player;

	public Item (string name, Dictionary<Item, int> craftCost) {
		this.Name = name;
		CraftCost = craftCost;
		player = Player.Instance;
	}

	public void Craft () {
		bool canCraft = player.CheckCost (CraftCost);

		if (canCraft) {
			player.GiveItems (CraftCost);
			Dictionary<Item, int> itemAsDict = new Dictionary<Item, int> { {this, 1} };
			player.TakeItems (itemAsDict);
		} else {
			Debug.Log ("Can't craft: not enough items");
		}
	}
}
