using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Selectable {
	public bool UnderConstruction;
	bool initialized;
	public List<Dictionary<string, int>> BuildCosts;
	public List<int> UpgradeCosts;
	public Island MyIsland;
	Action buildAction;
	Action upgradeAction;

	protected override void Awake () {
		base.Awake ();
		MyIsland = GetComponentInParent<Island> ();
		if (MyIsland != null) {
			MyIsland.Buildings.Add (this);
		}
		BuildCosts = new List<Dictionary<string, int>> ();

		buildAction = new Action ("Build", 0, player.DataBase.ActionIconsByNames["Build"], ShowCraftWindow);
		upgradeAction = new Action ("Upgrade", 0, player.DataBase.ActionIconsByNames["Upgrade"], Upgrade);
	}

	protected override void Start () {
		base.Start ();
		RefreshActions ();

		if (initialized) {
			return;
		}
		for (int i = 0; i < MaxLevel - Level; i++) {
			int costLength = Random.Range (1, 4);
			Dictionary<string, int> cost = new Dictionary<string, int> ();
			for (int j = 0; j < costLength; j++) {
				List<Item> validItems = new List<Item> ();
				foreach (var item in player.DataBase.TempItemLibrary) {
					if (!cost.ContainsKey(item.Name) && !item.IsForSale && item.IsForCraft) {
						validItems.Add (item);
					}
				}

				int index = Random.Range (0, validItems.Count);

				cost.Add (validItems [index].Name, Random.Range(1, 6));
			}
			BuildCosts.Add (cost);
		}
	}

	public virtual void InitializeFromData (BuildingData buildingData) {
		Level = buildingData.Level;
		Name = buildingData.Name;
		UnderConstruction = buildingData.UnderConstruction;
		BuildCosts = new List<Dictionary<string, int>> (buildingData.BuildCosts); // potentially dangerous
		UpgradeCosts = new List<int> (buildingData.UpgradeCosts);
		initialized = true;
	}

	protected virtual void RefreshActions () {
		/*if (Level == MaxLevel) {
			actions.Remove (buildAction);
			actions.Remove (upgradeAction);
			return;
		}
		if (!UnderConstruction) {
			upgradeAction.Cost = UpgradeCosts [Level];
			actions.Remove (buildAction);
			actions.Add (upgradeAction);
		} else {
			actions.Remove (upgradeAction);
			actions.Add (buildAction);
		}*/
	}

	void ShowCraftWindow () {
		gameManager.OpenCraftWindow (this, null);
	}

	public void Upgrade () {
		if (player.Gold >= UpgradeCosts[Level]) {
			UnderConstruction = true;
			player.GiveGold (UpgradeCosts [Level]);
			gameManager.CloseContextButtons (true);
			RefreshActions ();
		} else {
			gameManager.OpenPopUp ("Not enough gold for upgrade");
		}
	}

	public void Build () {
		bool canBuild = player.CheckCost (BuildCosts[Level]);

		if (canBuild) {
			player.GiveItems (BuildCosts [Level]);
			Level += 1;
			UnderConstruction = false;
			RefreshActions ();
		} else {
			gameManager.OpenPopUp ("Can't upgrade: not enough items");
		}
	}
}
