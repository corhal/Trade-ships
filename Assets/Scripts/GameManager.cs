using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public PlayerShip PlayerShip;
	public GameObject CityHUD;
	public GameObject BattleHUD;

	public Selectable Selection;
	public GameObject ShipPrefab;
	public GameObject TradeShipPrefab;

	public bool InMoveMode = false;
	public List<Island> Islands;

	public List<Ship> Ships;
	public List<TradeShip> TradeShips;
	public List<Building> Buildings;
	public List<MissionObject> MissionObjects;

	public BattleSkillsWindow MyBattleSkillsWindow;
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
	public ImagesPopUp MyImagesPopUp;

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

	public void AddEnergy () {
		Player.Instance.Energy += 100;
	}

	public Island GetIslandByName (string islandName) {
		Island resultIsland = null;
		foreach (var island in Islands) {
			if (island.Name == islandName) {
				resultIsland = island;
			}
		}
		return resultIsland;
	}

	void Start () {	
		Buildings = new List<Building> (GameObject.FindObjectsOfType<Building>());
		MissionObjects = new List<MissionObject> (GameObject.FindObjectsOfType<MissionObject> ());

		if (!isBattle) {
			CityHUD.SetActive (true);
			BattleHUD.SetActive (false);
		}

		if (!Player.Instance.FirstLoad && !isBattle) { // ?..
			for (int i = 0; i < Buildings.Count; i++) {
				for (int j = 0; j < Player.Instance.BuildingDatas.Count; j++) { // ОЛОЛО ОЛОЛО Я ВОДИТЕЛЬ НЛО
					Vector3 buildingPosition = new Vector3 (Player.Instance.BuildingDatas [j].Coordinates [0],
						                           Player.Instance.BuildingDatas [j].Coordinates [1],
						                           Player.Instance.BuildingDatas [j].Coordinates [2]);
					if (Buildings[i].Name == Player.Instance.BuildingDatas[j].Name && Vector3.Distance(Buildings[i].transform.position, buildingPosition) < 0.001f) {
						Buildings [i].InitializeFromData (Player.Instance.BuildingDatas [j]);
					}
				}
			}
			for (int i = 0; i < MissionObjects.Count; i++) {
				for (int j = 0; j < Player.Instance.Missions.Count; j++) { // ОЛОЛО ОЛОЛО Я ВОДИТЕЛЬ НЛО					
					if (MissionObjects [i].Name == Player.Instance.Missions [j].Name) {
						MissionObjects [i].Mission = Player.Instance.Missions [j];
					}
				}
			}


			PlayerShip.gameObject.transform.position = Player.Instance.PlayerShipCoordinates;

			Player.Instance.TradeShips.Clear ();
			foreach (var tradeShipData in Player.Instance.TradeShipDatas) {
				GameObject tradeShipObject = Instantiate (TradeShipPrefab) as GameObject;
				// tradeShipObject.transform.position = transform.position;
				TradeShip tradeShip = tradeShipObject.GetComponent<TradeShip> ();
				tradeShip.TradeShipData = tradeShipData;

				tradeShip.StartIsland = GetIslandByName (tradeShipData.StartIslandName);
				Player.Instance.TradeShips.Add (tradeShip);
			}

			// temp solution:
			if (Player.Instance.CurrentMission != null) {
				Dictionary<string, int> reward = Player.Instance.CurrentMission.GiveReward ();
				Player.Instance.TakeItems (reward);
				OpenImagesPopUp ("Your reward:", reward);
			}

		} else if (!isBattle) {			
			Player.Instance.CreateShipDatas ();
			Player.Instance.SaveBuildings (Buildings);
			Player.Instance.SavePlayerShip (PlayerShip);
			Player.Instance.SaveTradeShipDatas ();
			Player.Instance.FirstLoad = false;
		}
	}			

	public void LoadBattle () {
		Player.Instance.SaveBuildings (Buildings);
		Player.Instance.SavePlayerShip (PlayerShip);
		Player.Instance.SaveTradeShipDatas ();
		Player.Instance.LoadBattle ();
	}

	public void LoadVillage () {
		List<Ship> PlayerShips = new List<Ship> ();
		foreach (var ship in Ships) {
			if (ship.Allegiance == Allegiance.Player) {
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
			Ship ship = selectable as Ship;
			OpenShipWindow (ship.ShipData);
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

	public void OpenShipWindow (ShipData shipData) {
		MyShipWindow.Open (shipData);
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

	public void OpenImagesPopUp (string message, Dictionary<string, int> itemNames) {
		MyImagesPopUp.Open (message, itemNames);
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

	public void OpenMissionWindow (/*ExpeditionCenter expeditionCenter, */ Mission chosenMission) {
		MyMissionWindow.Open (/*expeditionCenter,*/ chosenMission);
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

	public void OpenCraftWindow (Building building, string item) {
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

	public void FindMissionForItem (string item) {
		Mission farmMission = null;
		ExpeditionCenter expeditionCenter = FindObjectOfType<ExpeditionCenter> ();
		foreach (var mission in expeditionCenter.Missions) {
			if (mission.PossibleRewards.ContainsKey(item)) {
				farmMission = mission;
				break;
			}
		}

		if (farmMission != null) {
			OpenMissionWindow (/*FindObjectOfType<ExpeditionCenter> (),*/ farmMission);
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
