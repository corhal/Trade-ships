using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public Selectable Selection;
	public GameObject ShipPrefab;
	public GameObject TradeShipPrefab;

	public bool InMoveMode = false;
	public List<Island> Islands;

	public List<Ship> Ships;
	public List<TradeShip> TradeShips;
	public List<Building> Buildings;

	public TeamSelectionWindow MyTeamSelectionWindow;
	public InfoWindow MyInfoWindow;
	public ShipWindow MyShipWindow;
	public ExpeditionWindow MyExpeditionWindow;
	public MissionWindow MyMissionWindow;
	public PortWindow MyPortWindow;
	public FortWindow MyFortWindow;
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

	public void RespawnShips () {

		foreach (var tradeShip in TradeShips) {
			Destroy (tradeShip.gameObject);
		}
		TradeShips.Clear ();
		foreach (var tradeShipData in Player.Instance.TradeShipDatas) {				
			if (Player.Instance.HomeTeam.Contains(tradeShipData)) {
				GameObject shipObject = Instantiate (TradeShipPrefab) as GameObject;
				shipObject.GetComponent<TradeShip> ().TradeShipData = tradeShipData;
				TradeShips.Add (shipObject.GetComponent<TradeShip> ());
			}
		}
		
		/*foreach (var ship in Ships) {
			Destroy (ship.gameObject);
		}
		Ships.Clear ();
		foreach (var shipData in Player.Instance.ShipDatas) {				
			if (Player.Instance.HomeTeam.Contains(shipData)) {
				GameObject shipObject = Instantiate (ShipPrefab) as GameObject;
				shipObject.GetComponent<Ship> ().ShipData = shipData;
				Ships.Add (shipObject.GetComponent<Ship> ());
			}
		}*/
	}

	void Start () {	
		if (isBattle) {
			foreach (var enemyData in Player.Instance.CurrentMission.EnemyShips) {
				GameObject shipObject = Instantiate (ShipPrefab) as GameObject;
				shipObject.GetComponent<Ship> ().ShipData = enemyData;
				shipObject.AddComponent<EnemyMover> ();
				shipObject.GetComponent<EnemyMover> ().SightDistance = shipObject.GetComponent<BattleShip> ().AttackRange + 1.0f;
			}
			foreach (var shipData in Player.Instance.CurrentTeam) {
				if (shipData.Allegiance != "Player") {
					continue;
				}
				GameObject shipObject = Instantiate (ShipPrefab) as GameObject;
				shipObject.GetComponent<Ship> ().ShipData = shipData;
			}

		}

		Ships = new List<Ship> ();
		// Ships = new List<Ship> (GameObject.FindObjectsOfType<Ship>());
		Buildings = new List<Building> (GameObject.FindObjectsOfType<Building>());

		if (!Player.Instance.FirstLoad && !isBattle) { // ?..
			for (int i = 0; i < Buildings.Count; i++) {
				for (int j = 0; j < Player.Instance.BuildingDatas.Count; j++) { // ОЛОЛО ОЛОЛО Я ВОДИТЕЛЬ НЛО
					Vector3 buildingPosition = new Vector3 (Player.Instance.BuildingDatas [i].Coordinates [0],
						                           Player.Instance.BuildingDatas [i].Coordinates [1],
						                           Player.Instance.BuildingDatas [i].Coordinates [2]);
					if (Buildings[i].Name == Player.Instance.BuildingDatas[j].Name) {
						Debug.Log ("-----");
						Debug.Log (buildingPosition);
						Debug.Log (Buildings [i].transform.position);
						Debug.Log ("-----");
					}
					if (Buildings[i].Name == Player.Instance.BuildingDatas[j].Name && Vector3.Distance(Buildings[i].transform.position, buildingPosition) < 0.01f) {
						Buildings [i].InitializeFromData (Player.Instance.BuildingDatas [i]);
						Debug.Log ("should init from data");
					}
				}
			}

			foreach (var tradeShip in TradeShips) {
				Destroy (tradeShip.gameObject);
			}
			TradeShips.Clear ();
			foreach (var tradeShipData in Player.Instance.TradeShipDatas) {				
				if (Player.Instance.HomeTeam.Contains(tradeShipData)) {
					GameObject shipObject = Instantiate (TradeShipPrefab) as GameObject;
					shipObject.GetComponent<TradeShip> ().TradeShipData = tradeShipData;
					TradeShips.Add (shipObject.GetComponent<TradeShip> ());
				}
			}

			/*foreach (var ship in Ships) {
				Destroy (ship.gameObject);
			}
			Ships.Clear ();
			foreach (var shipData in Player.Instance.ShipDatas) {				
				if (Player.Instance.HomeTeam.Contains(shipData)) {
					GameObject shipObject = Instantiate (ShipPrefab) as GameObject;
					shipObject.GetComponent<Ship> ().ShipData = shipData;
					Ships.Add (shipObject.GetComponent<Ship> ());
				}
			}*/
		} else if (!isBattle) {			
			Player.Instance.CreateShipDatas ();
			Player.Instance.CreateTradeShipDatas ();
			foreach (var tradeShipData in Player.Instance.TradeShipDatas) {				
				if (Player.Instance.HomeTeam.Contains(tradeShipData)) {
					GameObject shipObject = Instantiate (TradeShipPrefab) as GameObject;
					shipObject.GetComponent<TradeShip> ().TradeShipData = tradeShipData;
					TradeShips.Add (shipObject.GetComponent<TradeShip> ());
				}
			}
			/*foreach (var shipData in Player.Instance.ShipDatas) {				
				if (Player.Instance.HomeTeam.Contains(shipData)) {
					GameObject shipObject = Instantiate (ShipPrefab) as GameObject;
					shipObject.GetComponent<Ship> ().ShipData = shipData;
					Ships.Add (shipObject.GetComponent<Ship> ());
				}
			}*/
			Player.Instance.SaveBuildings (Buildings);
			Player.Instance.FirstLoad = false;
		}
	}			

	public void LoadBattle () {
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
		MyFortWindow.Close ();
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
		MyFortWindow.Close ();
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
		MyFortWindow.Close ();
	}

	public void OpenFortWindow () {
		MyFortWindow.Open ();
		MyShipWindow.Close ();
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
		MyFortWindow.Close ();
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
		MyFortWindow.Close ();
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
		MyFortWindow.Close ();
	}

	public void OpenPortWindow (Port port, TradeShip tradeShip) {
		if (InMoveMode) {
			return;
		}
		MyPortWindow.Open (port, tradeShip);
		MyCraftWindow.Close ();
		MyButtonsOverlay.Close ();
		MyExpeditionWindow.Close ();
		MyMissionWindow.Close ();
		MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
		MyFortWindow.Close ();
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
		MyFortWindow.Close ();
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
		// MyFortWindow.Close (); // here is the bug!!! think about it!
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
		MyFortWindow.Close ();
	}

	public void CloseContextButtons (bool deselect) {
		MyButtonsOverlay.Overlay.SetActive (false); // kostyll
		//MyButtonsOverlay.Close ();
		if (deselect && Selection != null) {
			Selection.Deanimate ();
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
