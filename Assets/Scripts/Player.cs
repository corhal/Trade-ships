using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public int MaxEnergy;
	public int Energy;

	public List<Mission> Missions;
	public Mission CurrentMission;
	public List<BuildingData> BuildingDatas;
	public List<CreatureData> ShipDatas;
	public List<TradeShipData> TradeShipDatas;
	public List<TradeShip> TradeShips;
	public bool FirstLoad = true;
	public int Gold;

	public static Player Instance;

	public DataBase DataBase;
	public BJDataBase BJDataBase;
	public Dictionary<string, int> Inventory;

	public List<CreatureData> CurrentTeam;
	public List<TradeShipData> HomeTeam;

	public Vector3 PlayerShipCoordinates;

	public Dictionary<int, bool> Tiles;

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
		DontDestroyOnLoad(gameObject);
		CurrentTeam = new List<CreatureData> ();
		Inventory = new Dictionary<string, int> ();
		Tiles = new Dictionary<int, bool> ();
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

	public void SaveTradeShipDatas () {
		foreach (var tradeShip in TradeShips) {
			tradeShip.TradeShipData.Coordinates = tradeShip.gameObject.transform.position;
		}
	}

	public void SavePlayerShip (PlayerShip playerShip) {
		PlayerShipCoordinates = playerShip.gameObject.transform.position;
	}

	public void CreateShipDatas () {
		ShipDatas.Clear ();

		List<BJCreature> creatures = BJDataBase.Creatures;
		for (int j = 0; j < creatures.Count; j++) {
			bool summoned = (j > 0 && j < 6) ? true : false;

			List<Skill> skills = new List<Skill> ();
			foreach (var skillName in creatures[j].SkillNames) {
				
				skills.Add (BJDataBase.SkillsByNames [skillName]);
			}
			Sprite soulstoneSprite = null;
			if (Player.Instance.BJDataBase.CreaturePortraitsByNames.ContainsKey (creatures [j].Name)) {
				soulstoneSprite = Player.Instance.BJDataBase.CreaturePortraitsByNames [creatures [j].Name];
			}

			Item soulstone = new Item ((creatures [j].Name + " soulstone"), null, soulstoneSprite, false, false, new Dictionary<string, int> ());
			if (!DataBase.ItemIconsByNames.ContainsKey (soulstone.Name)) {
				DataBase.ItemIconsByNames.Add (soulstone.Name, soulstoneSprite);
			}
			if (!DataBase.TempItemLibrary.Contains (soulstone)) {
				DataBase.TempItemLibrary.Add (soulstone);
				DataBase.ItemsByNames.Add (soulstone.Name, soulstone);
				Inventory.Add (soulstone.Name, 0);
			}

			List<List<string>> promoteCosts = new List<List<string>> ();
			for (int k = 0; k < (int)RankColor.OrangeP - (int)RankColor.White; k++) {
				int costLength = 6;
				List<string> cost = new List<string> ();
				for (int l = 0; l < costLength; l++) {
					List<Item> validItems = new List<Item> ();
					foreach (var item in Player.Instance.DataBase.TempItemLibrary) {
						string nameString = item.Name;
						if (!item.IsForSale && item.IsForCraft) {
							validItems.Add (item);
						}
					}

					int index = Random.Range (0, validItems.Count);

					cost.Add (validItems [index].Name);
				}
				promoteCosts.Add (cost);
			}

			List<int> levelRequirements = new List<int> { 10, 20, 30, 40, 50 };
			CreatureData newShipData = new CreatureData (creatures [j], 1, 1,
				skills, soulstone, promoteCosts, RankColor.White, summoned, levelRequirements);

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
		for (int i = CurrentTeam.Count - 1; i >= 0; i--) {
			if (CurrentTeam [i].IsDead) {
				CurrentTeam.Remove (CurrentTeam [i]);
			}
		}
		SceneManager.LoadScene (0);
	}

	public void Craft (string item) {
		bool canCraft = CheckCost (DataBase.ItemsByNames [item].CraftCost);

		if (canCraft) {
			GiveItems (DataBase.ItemsByNames [item].CraftCost);
			Dictionary<string, int> ItemAsDict = new Dictionary<string, int> { { item, 1 } };
			TakeItems (ItemAsDict);
		} else {
			UIOverlay.Instance.OpenPopUp ("Can't craft: not enough items");
		}
	}

	public void TakeGold (int amount) {
		Gold += amount;
	}

	public void GiveGold (int amount) {
		if (Gold >= amount) {
			Gold -= amount;
		} else {
			UIOverlay.Instance.OpenPopUp ("Not enough gold");
		}
	}

	public void GiveItems (Dictionary<string, int> amountsByItemNames) {
		// probably should check here
		foreach (var amountByItemName in amountsByItemNames) {
			if (!Inventory.ContainsKey(amountByItemName.Key)) {
				Inventory.Add (amountByItemName.Key, 0);
			}
			Inventory [amountByItemName.Key] -= amountByItemName.Value;
		}
		// Debug.Log("Giving items: " +
	}

	public void TakeItems (Dictionary<string, int> amountsByItemNames) {
		foreach (var amountByItemName in amountsByItemNames) {
			if (!Inventory.ContainsKey(amountByItemName.Key)) {
				Inventory.Add (amountByItemName.Key, 0);
			}
			Inventory [amountByItemName.Key] += amountByItemName.Value;
		}
	}

	public bool CheckCost (Dictionary<string, int> amountsByItemNames) {
		foreach (var amountByItemName in amountsByItemNames) {
			if (!Inventory.ContainsKey(amountByItemName.Key)) {
				Inventory.Add (amountByItemName.Key, 0);
			}
			if (Inventory[amountByItemName.Key] < amountByItemName.Value) {
				return false;
			}
		}
		return true;
	}
}
