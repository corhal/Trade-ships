using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public Mission CurrentMission;
	public List<BuildingData> BuildingDatas;
	public List<ShipData> ShipDatas;
	public bool FirstLoad = true;
	public int Gold;

	public static Player Instance;

	public DataBase DataBase;
	public Dictionary<Item, int> Inventory;

	public List<ShipData> CurrentTeam;

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
		DontDestroyOnLoad(gameObject);
		CurrentTeam = new List<ShipData> ();
		Inventory = new Dictionary<Item, int> ();
	}

	public void SaveBuildings (List<Building> buildings) {
		BuildingDatas.Clear ();
		for (int i = 0; i < buildings.Count; i++) {
			if (buildings[i] is Port) { // как-то все неловко
				PortData portData = new PortData ();
				portData.InitializeFromBuilding (buildings [i]);
				BuildingDatas.Add (portData);
			} else {
				BuildingData buildingData = new BuildingData ();
				buildingData.InitializeFromBuilding (buildings [i]);
				BuildingDatas.Add (buildingData);
			}
		}
	}

	public void SaveShips (List<Ship> ships) {
		//ShipDatas.Clear ();
		Dictionary<Ship, bool> alreadySeen = new Dictionary<Ship, bool>();
		for (int i = 0; i < ships.Count; i++) {
			foreach (var shipData in ShipDatas) {
				if (shipData.Name == ships[i].Name) {
					shipData.InitializeFromShip (ships [i]);
					alreadySeen.Add (ships [i], true);
				}
			}
		}

		foreach (var ship in ships) {
			if (!alreadySeen.ContainsKey(ship) && !(GameManager.Instance.isBattle && ship.Allegiance == "Enemy")) { // temporary fix!!
				ShipData newShipData = new ShipData ();
				newShipData.InitializeFromShip (ship);
				ShipDatas.Add (newShipData);
			}
		}
	}

	public void LoadBattle () {
		SceneManager.LoadScene (1);
	}

	public void LoadVillage () {
		SceneManager.LoadScene (0);
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
