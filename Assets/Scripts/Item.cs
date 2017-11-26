using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item {

	public string Name;
	public Sprite Icon;
	public Dictionary<string, int> CraftCost;
	public bool IsForCraft = false;
	public bool IsForSale = true;
	Player player;
	public Dictionary<string, int> StatsByNames;

	public Item (string name, Dictionary<string, int> craftCost, Sprite icon, bool isForSale, bool isForCraft, Dictionary<string, int> statsByNames) {
		this.Name = name;
		CraftCost = craftCost;
		player = Player.Instance;
		this.Icon = icon;
		this.IsForSale = isForSale;
		this.IsForCraft = isForCraft;
		//if (statsByNames != null) {
			StatsByNames = new Dictionary<string, int> (statsByNames);
		/*} else {
			StatsByNames = new Dictionary<string, int> ();
			if (CraftCost != null && CraftCost.Count > 0) {
				foreach (var amountByItem in this.CraftCost) {
					foreach (var statByName in Player.Instance.DataBase.ItemsByNames[amountByItem.Key].StatsByNames) {
						if (!StatsByNames.ContainsKey(statByName.Key)) {
							StatsByNames.Add (statByName.Key, statByName.Value);
						} else {
							StatsByNames [statByName.Key] = statByName.Value;
						}
					}
				}
			}
		}*/
	}

	public void Craft () {
		bool canCraft = player.CheckCost (CraftCost);

		if (canCraft) {
			player.GiveItems (CraftCost);
			Dictionary<string, int> itemAsDict = new Dictionary<string, int> { {this.Name, 1} };
			player.TakeItems (itemAsDict);
		} else {
			Debug.Log ("Can't craft: not enough items");
		}
	}
}
