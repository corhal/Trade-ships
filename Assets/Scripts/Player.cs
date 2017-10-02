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

	public List<Item> TempItemLibrary;

	public Dictionary<RankColor, Color> ColorsByRankColors;

	void Start () {
		ColorsByRankColors = new Dictionary<RankColor, Color> {
			{ RankColor.White, Color.white },
			{ RankColor.Green, Color.green },
			{ RankColor.GreenP, Color.green },
			{ RankColor.Blue, Color.blue },
			{ RankColor.BlueP, Color.blue },
			{ RankColor.BluePP, Color.blue },
			{ RankColor.Purple, Color.cyan },
			{ RankColor.PurpleP, Color.cyan },
			{ RankColor.PurplePP, Color.cyan },
			{ RankColor.PurplePPP, Color.cyan },
			{ RankColor.PurplePPPP, Color.cyan },
			{ RankColor.Orange, Color.yellow },
			{ RankColor.OrangeP, Color.yellow },
		};

		Item wood = new Item ("Wood", null, GameManager.Instance.ItemIconsByNames["Wood"], false);
		Item food = new Item ("Food", null, GameManager.Instance.ItemIconsByNames["Food"], true);
		Item steel = new Item ("Steel", null, GameManager.Instance.ItemIconsByNames["Steel"], false);
		Item nails = new Item ("Nails", new Dictionary<Item, int> { { steel, 2 } }, GameManager.Instance.ItemIconsByNames["Nails"], false);
		Item hammers = new Item ("Picks", new Dictionary<Item, int> { { steel, 1 }, {wood, 1} }, GameManager.Instance.ItemIconsByNames["Picks"], false);
		Item saws = new Item ("Shovels", new Dictionary<Item, int> { { steel, 2 }, {wood, 1} }, GameManager.Instance.ItemIconsByNames["Shovels"], false);
		Item tools = new Item ("Tools", new Dictionary<Item, int> { { hammers, 1 }, {saws, 1} }, GameManager.Instance.ItemIconsByNames["Tools"], false);
		Item spices = new Item ("Spices", null, GameManager.Instance.ItemIconsByNames["Spices"], true);
		Item ale = new Item ("Ale", null, GameManager.Instance.ItemIconsByNames["Ale"], true);
		Item fish = new Item ("Fish", null, GameManager.Instance.ItemIconsByNames["Fish"], true);

		TempItemLibrary = new List<Item> {
			wood,
			food,
			steel,
			nails,
			hammers,
			saws,
			tools,
			spices,
			ale,
			fish,
		};

		foreach (var item in TempItemLibrary) {
			TakeItems (new Dictionary<Item, int> { { item, 0 } });
		}
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
		ShipDatas.Clear ();
		for (int i = 0; i < ships.Count; i++) {
			ShipData shipData = new ShipData ();
			shipData.InitializeFromShip (ships [i]);
			ShipDatas.Add (shipData);
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
			//Debug.Log (Inventory [amountByItem.Key]);
			//Debug.Log (amountByItem.Key.Name);
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
