using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

	public string Name;
	public Dictionary<Item, int> CraftCost;
	Player player;

	public Item (string name, Dictionary<Item, int> craftCost) {
		this.Name = name;
		CraftCost = new Dictionary<Item, int> (craftCost);
		player = Player.Instance;
	}

	public void Craft () {
		bool canCraft = player.CheckCost (CraftCost);

		if (canCraft) {
			player.GiveItems (CraftCost);
			player.TakeItems (new Dictionary<Item, int> { this, 1 });
		} else {
			Debug.Log ("Can't craft: not enough items");
		}
	}
}
