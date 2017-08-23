using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public int Gold;

	public static Player Instance;
	public Dictionary<Item, int> Inventory;

	void Awake () {
		Instance = this;
		Inventory = new Dictionary<Item, int> ();
	}

	public void TakeGold (int amount) {
		Gold += amount;
	}

	public void GiveGold (int amount) {
		if (Gold >= amount) {
			Gold -= amount;
		} else {
			Debug.Log ("Not enough gold");
		}
	}

	public void GiveItems (Dictionary<Item, int> amountsByItems) {
		// probably should check here
		foreach (var amountByItem in amountsByItems) {
			if (!Inventory.ContainsKey(amountByItem.Key)) {
				Inventory.Add (amountByItem.Key, 0);
			}
			Inventory [amountByItem.Key] -= amountByItem.Value;
		}
	}

	public void TakeItems (Dictionary<Item, int> amountsByItems) {
		foreach (var amountByItem in amountsByItems) {
			if (!Inventory.ContainsKey(amountByItem.Key)) {
				Inventory.Add (amountByItem.Key, 0);
			}
			Inventory [amountByItem.Key] += amountByItem.Value;
		}
	}

	public bool CheckCost (Dictionary<Item, int> amountsByItems) {
		foreach (var amountByItem in amountsByItems) {
			if (!Inventory.ContainsKey(amountByItem.Key)) {
				Inventory.Add (amountByItem.Key, 0);
			}
			if (Inventory[amountByItem.Key] < amountByItem.Value) {
				return false;
			}
		}
		return true;
	}
}
