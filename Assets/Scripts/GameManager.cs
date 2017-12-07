using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public PlayerShip PlayerShip;
	public GameObject CityHUD;

	public Selectable Selection;
	public GameObject ShipPrefab;
	public GameObject TradeShipPrefab;

	public bool InMoveMode = false;
	public List<Island> Islands;

	public List<TradeShip> TradeShips;
	public List<Building> Buildings;
	public List<MissionObject> MissionObjects;

	public static GameManager Instance;

	public bool CameraDragged;

	public void MoveMode () {
		InMoveMode = true;
		UIOverlay.Instance.CloseContextButtons (false);
	}

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
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
				UIOverlay.Instance.OpenImagesPopUp ("Your reward:", reward);
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

	public void FindMissionForItem (string item) {
		UIOverlay.Instance.OpenPopUp ("Not implemented yet");
		/*Mission farmMission = null;
		ExpeditionCenter expeditionCenter = FindObjectOfType<ExpeditionCenter> ();
		foreach (var mission in expeditionCenter.Missions) {
			if (mission.PossibleRewards.ContainsKey(item)) {
				farmMission = mission;
				break;
			}
		}*/

		//if (farmMission != null) {
			//OpenMissionWindow (/*FindObjectOfType<ExpeditionCenter> (),*/ farmMission);
		//} else {
			//OpenPopUp ("No such mission, try button in top left corner");
		//}
	}

	public void RefreshMissions () {
		ExpeditionCenter expeditionCenter = FindObjectOfType<ExpeditionCenter> ();
		expeditionCenter.CreateMissions ();
	}

	public void UnderConstruction () {
		UIOverlay.Instance.OpenPopUp ("This functionality is under construction yet");
	}
}
