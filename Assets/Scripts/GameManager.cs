using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public Selectable Selection;
	public GameObject ShipPrefab;

	public bool InMoveMode = false;
	public List<Island> Islands;

	public List<Ship> Ships;
	public List<Building> Buildings;

	public TeamSelectionWindow MyTeamSelectionWindow;
	public InfoWindow MyInfoWindow;
	public ShipWindow MyShipWindow;
	public ExpeditionWindow MyExpeditionWindow;
	public MissionWindow MyMissionWindow;
	public PortWindow MyPortWindow;
	public CraftWindow MyCraftWindow;
	public ContextButtonsOverlay MyButtonsOverlay;
	public PopUp MyPopUp;

	public static GameManager Instance;

	public List<BattleShip> Battleships;

	public void MoveMode () {
		InMoveMode = true;
		CloseContextButtons (false);
	}

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}

		Battleships = new List<BattleShip> (FindObjectsOfType<BattleShip> ());

		foreach (var battleShip in Battleships) {
			battleShip.OnBattleShipDestroyed += BattleShip_OnBattleShipDestroyed;
		}
	}

	void BattleShip_OnBattleShipDestroyed (BattleShip sender) {
		Battleships.Remove (sender);
		Ships.Remove (sender.gameObject.GetComponent<Ship> ());
		sender.OnBattleShipDestroyed -= BattleShip_OnBattleShipDestroyed;
	}

	public bool isBattle; // ew

	void Start () {	
		if (isBattle) {
			foreach (var shipData in Player.Instance.CurrentTeam) {
				if (shipData.Allegiance != "Player") {
					continue;
				}
				GameObject shipObject = Instantiate (ShipPrefab) as GameObject;
				Ship ship = shipObject.GetComponent<Ship> ();
				ship.InitializeFromData (shipData);
			}
			foreach (var enemyData in Player.Instance.CurrentMission.EnemyShips) {
				GameObject shipObject = Instantiate (ShipPrefab) as GameObject;
				Ship ship = shipObject.GetComponent<Ship> ();
				ship.InitializeFromData (enemyData);
				shipObject.AddComponent<EnemyMover> ();
				shipObject.GetComponent<EnemyMover> ().SightDistance = shipObject.GetComponent<BattleShip> ().AttackRange + 1.0f;
			}
		}

		Ships = new List<Ship> (GameObject.FindObjectsOfType<Ship>());
		Buildings = new List<Building> (GameObject.FindObjectsOfType<Building>());

		if (!Player.Instance.FirstLoad && !isBattle) { // ?..
			for (int i = 0; i < Buildings.Count; i++) {
				for (int j = 0; j < Player.Instance.BuildingDatas.Count; j++) { // ОЛОЛО ОЛОЛО Я ВОДИТЕЛЬ НЛО
					Vector3 buildingPosition = new Vector3 (Player.Instance.BuildingDatas [i].Coordinates [0],
						                           Player.Instance.BuildingDatas [i].Coordinates [1],
						                           Player.Instance.BuildingDatas [i].Coordinates [2]);
					if (Buildings[i].Name == Player.Instance.BuildingDatas[j].Name && Vector3.Distance(Buildings[i].transform.position, buildingPosition) < 0.001f) {
						Buildings [i].InitializeFromData (Player.Instance.BuildingDatas [i]);
					}
				}
			}
			List<Ship> ShipsToRemove = new List<Ship> ();
			foreach (var ship in Ships) {
				Destroy (ship.gameObject);
			}
			Ships.Clear ();
			foreach (var shipData in Player.Instance.ShipDatas) {
				if (shipData.IsSummoned) {
					GameObject shipObject = Instantiate (ShipPrefab) as GameObject;
					shipObject.GetComponent<Ship>().InitializeFromData (shipData);
				}
			}
		} else {			
			Player.Instance.CreateShipDatas ();
			foreach (var shipData in Player.Instance.ShipDatas) {				
				if (shipData.IsSummoned) {
					GameObject shipObject = Instantiate (ShipPrefab) as GameObject;
					shipObject.GetComponent<Ship> ().ShipData = shipData;
					shipObject.GetComponent<Ship>().InitializeFromData (shipData); // this is probably obsolete now
				}
			}
			Player.Instance.SaveShips (Ships);
			Player.Instance.SaveBuildings (Buildings);
			Player.Instance.FirstLoad = false;
		}
	}			

	public void LoadBattle () {
		Player.Instance.SaveShips (Ships);
		Player.Instance.SaveBuildings (Buildings);
		Player.Instance.LoadBattle ();
	}

	public void LoadVillage () {
		List<Ship> PlayerShips = new List<Ship> ();
		foreach (var ship in Ships) {
			if (ship.Allegiance == "Player") {
				PlayerShips.Add (ship);
			}
		}
		Player.Instance.SaveShips (PlayerShips);
		Player.Instance.LoadVillage ();
	}

	public Item GetRandomItem (bool craftable) {
		List<Item> items = new List<Item> ();
		foreach (var item in Player.Instance.DataBase.TempItemLibrary) {
			if (!craftable && item.CraftCost != null) {
				continue;
			}
			items.Add (item);
		}
		int index = Random.Range (0, items.Count);
		return items [index];
	}

	public Item GetItemByName (string itemName) {
		foreach (var item in Player.Instance.DataBase.TempItemLibrary) {
			if (item.Name == itemName) {
				return item;
			}
		}
		return null;
	}

	public List<BattleShip> GetEnemies (string allegiance) {
		List<BattleShip> enemyShips = new List<BattleShip> ();
		foreach (var battleShip in Battleships) {
			if (battleShip.Allegiance != allegiance) {
				enemyShips.Add (battleShip);
			}
		}
		return enemyShips;
	}

	public void OpenThievesWindow (ThievesGuild thievesGuild) {

		MyInfoWindow.Close ();
		MyShipWindow.Close ();
		MyExpeditionWindow.Close ();
		MyMissionWindow.Close ();
		MyPortWindow.Close ();
		MyCraftWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyPopUp.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void OpenSelectableInfo (Selectable selectable) {
		if (selectable is Ship) {
			OpenShipWindow (selectable as Ship);
			return;
		}
		MyInfoWindow.Open (selectable);
		MyShipWindow.Close ();
		MyExpeditionWindow.Close ();
		MyMissionWindow.Close ();
		MyPortWindow.Close ();
		MyCraftWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyPopUp.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void OpenShipWindow (Ship ship) {
		MyShipWindow.Open (ship);
		MyExpeditionWindow.Close ();
		MyMissionWindow.Close ();
		MyPortWindow.Close ();
		MyCraftWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void OpenPopUp (string message) {
		MyPopUp.Open (message);
	}

	public void OpenExpeditionWindow (ExpeditionCenter expeditionCenter) {
		MyExpeditionWindow.Open (expeditionCenter);
		MyMissionWindow.Close ();
		MyPortWindow.Close ();
		MyCraftWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void CloseExpeditionWindow () {
		MyExpeditionWindow.Close ();
	}

	public void OpenMissionWindow (ExpeditionCenter expeditionCenter, Mission chosenMission) {
		MyMissionWindow.Open (expeditionCenter, chosenMission);
		MyPortWindow.Close ();
		MyCraftWindow.Close ();
		MyButtonsOverlay.Close ();
		MyExpeditionWindow.Close ();
		MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void CloseMissionWindow () {
		MyMissionWindow.Close ();
	}

	public void OpenTeamSelectionWindow (Mission mission) {
		MyTeamSelectionWindow.Open (mission);
		MyPortWindow.Close ();
		MyCraftWindow.Close ();
		MyButtonsOverlay.Close ();
		MyExpeditionWindow.Close ();
		MyMissionWindow.Close ();
		MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void OpenPortWindow (Port port, Ship ship) {
		if (InMoveMode) {
			return;
		}
		MyPortWindow.Open (port, ship);
		MyCraftWindow.Close ();
		MyButtonsOverlay.Close ();
		MyExpeditionWindow.Close ();
		MyMissionWindow.Close ();
		MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void ClosePortWindow () {
		MyPortWindow.Close ();

		if (MyPortWindow.CurrentPort != null && MyPortWindow.CurrentPort.Name == "Shipwreck" && MyPortWindow.CurrentPort.Shipments.Count == 0) { // as reliable as bullets made of chocolate
			Destroy (MyPortWindow.CurrentPort.gameObject);
		}
	}

	public void OpenCraftWindow (Building building, Item item) {
		MyCraftWindow.Open (building, item);
		MyButtonsOverlay.Close ();
		MyPortWindow.Close ();
		MyExpeditionWindow.Close ();
		MyMissionWindow.Close ();
		//MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void CloseCraftWindow () {
		MyCraftWindow.Close ();
	}

	public void OpentContextButtons (Selectable selectable) {
		if (InMoveMode) {
			return;
		}
		CloseContextButtons (true);
		Selection = selectable;
		Selection.Animate ();
		MyButtonsOverlay.Open (selectable);
		MyPortWindow.Close ();
		MyCraftWindow.Close ();
		MyExpeditionWindow.Close ();
		MyMissionWindow.Close ();
		MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public ShipsCatalogWindow MyShipsCatalogWindow;

	public void OpenShipsCatalogWindow () {
		MyShipsCatalogWindow.Open ();
		MyCraftWindow.Close ();
		MyButtonsOverlay.Close ();
		MyPortWindow.Close ();
		MyExpeditionWindow.Close ();
		MyMissionWindow.Close ();
		MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
	}

	public void CloseContextButtons (bool deselect) {
		MyButtonsOverlay.Overlay.SetActive (false); // kostyll
		//MyButtonsOverlay.Close ();
		if (deselect && Selection != null) {
			Selection.Deanimate ();
		}
	}

	public List<Mission> Missions;

	void GenerateMissions () {
		List<string> enemyNames = new List<string> {
			"Dragon", "Brave", "Nebula", "Theseus", "Sagittarius", "Lion", "Sir Galahad"
		};

		Utility.Shuffle (enemyNames);

		List<ShipData> enemyShips = new List<ShipData> ();
		int enemiesCount = Random.Range (1, 6);
		int maxHp = Random.Range (150, 200);

		int rankCol = Random.Range(0, System.Enum.GetNames (typeof(RankColor)).Length);
		for (int i = 0; i < enemiesCount; i++) {
			float[] coordinates = new float[3];
			coordinates [0] = Random.Range(0.0f, 5.0f);
			coordinates [1] = Random.Range(-5.0f, 0.0f);
			coordinates [2] = 0.0f;
			ShipData enemy = new ShipData(enemyNames[i], "Enemy", Random.Range(1, 4), Random.Range(1, 6), Random.Range(1, 10), 
				maxHp, maxHp, Random.Range(10, 20), coordinates, null, null, null, null, null, (RankColor)rankCol, false, null);
			enemyShips.Add (enemy);
		}

		for (int i = 0; i < 5; i++) {
			int costLength = Random.Range (1, 6);
			Dictionary<Item, float> rewardChances = new Dictionary<Item, float> ();
			Dictionary<Item, int> possibleRewards = new Dictionary<Item, int> ();
			if (i < Ships.Count) { // !!! Replace with something more sensible
				possibleRewards.Add (Ships[i].Blueprint, Random.Range(1, 6));
				rewardChances.Add (Ships[i].Blueprint, Random.Range (0.3f, 0.7f));
			}
			for (int j = 0; j < costLength; j++) {
				List<Item> validItems = new List<Item> ();
				foreach (var item in Player.Instance.DataBase.TempItemLibrary) {
					if (!possibleRewards.ContainsKey(item)) {
						validItems.Add (item);
					}
				}

				int index = Random.Range (0, validItems.Count);
				possibleRewards.Add (validItems [index], Random.Range(1, 6));
				rewardChances.Add (validItems [index], Random.Range (0.3f, 0.7f));
			}
			Missions.Add (new Mission (rewardChances, possibleRewards, enemyShips));
		}
	}

	public void FindMissionForItem (Item item) {
		Mission farmMission = null;
		ExpeditionCenter expeditionCenter = FindObjectOfType<ExpeditionCenter> ();
		foreach (var mission in expeditionCenter.Missions) {
			if (mission.PossibleRewards.ContainsKey(item)) {
				farmMission = mission;
				break;
			}
		}

		if (farmMission != null) {
			OpenMissionWindow (FindObjectOfType<ExpeditionCenter> (), farmMission);
		} else {
			OpenPopUp ("No such mission, try button in top left corner");
		}
	}

	public void RefreshMissions () {
		ExpeditionCenter expeditionCenter = FindObjectOfType<ExpeditionCenter> ();
		expeditionCenter.CreateMissions ();
	}

	public void UnderConstruction () {
		OpenPopUp ("This functionality is under construction yet");
	}
}
