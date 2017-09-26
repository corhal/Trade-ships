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

	void Start () {
		/*foreach (var item in GameManager.Instance.TempItemLibrary) {
			if (item.CraftCost == null) {
				Dictionary<Item, int> ItemAsDict = new Dictionary<Item, int> { { item, 30 } };
				TakeItems (ItemAsDict);
			}
		}*/
	}

	public void Craft (Item item) {
		bool canCraft = CheckCost (item.CraftCost);

		if (canCraft) {
			GiveItems (item.CraftCost);
			Dictionary<Item, int> ItemAsDict = new Dictionary<Item, int> { { item, 1 } };
			TakeItems (ItemAsDict);
		} else {
			GameManager.Instance.OpenPopUp ("Can't craft: not enough items");
		}
	}

	public void TakeGold (int amount) {
		Gold += amount;
	}

	public void GiveGold (int amount) {
		if (Gold >= amount) {
			Gold -= amount;
		} else {
			GameManager.Instance.OpenPopUp ("Not enough gold");
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
			Debug.Log (Inventory [amountByItem.Key]);
			Debug.Log (amountByItem.Key.Name);
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
