using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public Mission CurrentMission;
	public List<BuildingData> BuildingDatas;
	public List<ShipData> ShipDatas;
	public List<TradeShipData> TradeShipDatas;
	public bool FirstLoad = true;
	public int Gold;

	public static Player Instance;

	public DataBase DataBase;
	public Dictionary<Item, int> Inventory;

	public List<ShipData> CurrentTeam;
	public List<TradeShipData> HomeTeam;

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

	public void CreateTradeShipDatas () {
		TradeShipDatas.Clear ();
		List<string> playerNames = new List<string> {
			"Sindbad", "Marco Pollo", "Nasreddin"
		};

		Utility.Shuffle (playerNames);
		for (int j = 0; j < playerNames.Count; j++) {			
			float[] coordinates = new float[3];
			coordinates [0] = Random.Range (0.0f, 5.0f);
			coordinates [1] = Random.Range (-5.0f, 0.0f);
			coordinates [2] = -2.0f;

			TradeShipData newShipData = new TradeShipData (playerNames [j], "Player", Random.Range (5, 10), coordinates, null);

			TradeShipDatas.Add (newShipData);

			HomeTeam.Add (newShipData);
		}
	}

	public void CreateShipDatas () {
		ShipDatas.Clear ();
		List<string> playerNames = new List<string> {
			"Vulpecula", "Manticora", "Herr Mannelig", "Nexus", "Liguria", "Elephant", "Morgenstern"
		};

		Utility.Shuffle (playerNames);
		for (int j = 0; j < playerNames.Count; j++) {
			bool summoned = (j < 3) ? true : false;

			int maxHp = Random.Range (200, 300);
			float[] coordinates = new float[3];
			coordinates [0] = Random.Range (0.0f, 5.0f);
			coordinates [1] = Random.Range (-5.0f, 0.0f);
			coordinates [2] = -2.0f;

			float coinToss = Random.Range (0.0f, 1.0f);
			List<Skill> skills = new List<Skill> { (coinToss > 0.5f) ? DataBase.SkillsByNames ["Slow down"] : DataBase.SkillsByNames ["Speed up"],
				DataBase.SkillsByNames ["Trade"], DataBase.SkillsByNames ["Cannons"], DataBase.SkillsByNames ["Dummy"]
			};
			Item blueprint = new Item ((playerNames [j] + " blueprint"), null, null, false, false, null);
			if (!DataBase.ItemIconsByNames.ContainsKey (blueprint.Name)) {
				DataBase.ItemIconsByNames.Add (blueprint.Name, null);
			}
			if (!DataBase.TempItemLibrary.Contains (blueprint)) {
				DataBase.TempItemLibrary.Add (blueprint);
				Inventory.Add (blueprint, 0);
			}

			List<List<Item>> promoteCosts = new List<List<Item>> ();
			for (int k = 0; k < (int)RankColor.OrangeP - (int)RankColor.White; k++) {
				int costLength = 6;
				List<Item> cost = new List<Item> ();
				for (int l = 0; l < costLength; l++) {
					List<Item> validItems = new List<Item> ();
					foreach (var item in Player.Instance.DataBase.TempItemLibrary) {
						string nameString = item.Name;
						string firstName = nameString.Split (' ') [0];
						if (/*!cost.ContainsKey(item) &&*/ !item.IsForSale && item.IsForCraft /*firstName != "Blueprint"*/) {
							validItems.Add (item);
						}
					}

					int index = Random.Range (0, validItems.Count);

					cost.Add (validItems [index]);
				}
				promoteCosts.Add (cost);
			}

			List<int> levelRequirements = new List<int> { 10, 20, 30, 40, 50 };
			ShipData newShipData = new ShipData (playerNames [j], "Player", 1, 1,
				                       maxHp, maxHp, Random.Range (10, 20), coordinates, skills, null, blueprint, promoteCosts, RankColor.White, summoned, levelRequirements, null, 1.5f, 3.0f);

			ShipDatas.Add (newShipData);

			if (summoned) {
				//HomeTeam.Add (newShipData);
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
