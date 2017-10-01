using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public Selectable Selection;
	public GameObject ShipPrefab;

	public Dictionary<string, Sprite> ItemIconsByNames;
	public List<Sprite> ItemIcons;
	public List<string> ItemNames;

	public Dictionary<string, Sprite> ActionIconsByNames;
	public List<Sprite> ActionIcons;
	public List<string> ActionNames;

	public bool InMoveMode = false;
	public List<Island> Islands;
	public List<Item> TempItemLibrary;

	public List<Ship> Ships;
	public List<Building> Buildings;

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
		ActionIconsByNames = new Dictionary<string, Sprite> ();
		for (int i = 0; i < ActionIcons.Count; i++) {
			ActionIconsByNames.Add (ActionNames [i], ActionIcons [i]);
		}

		ItemIconsByNames = new Dictionary<string, Sprite> ();
		for (int i = 0; i < ItemIcons.Count; i++) {
			ItemIconsByNames.Add (ItemNames [i], ItemIcons [i]);
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

	public Effect SlowDown;
	public Effect SpeedUpFire;
	public Effect Trade;
	public Effect Cannons;

	public List<GameObject> EffectParticlePrefabs;
	public List<string> EffectParticleNames;

	public Skill SlowDownSkill;
	public Skill SpeedUpFireSkill;

	public Skill TradeSkill;
	public Skill CannonsSkill;
	public Skill DummySkill;

	public Dictionary<string, Skill> SkillsByNames;

	public bool isBattle; // ew

	void Start () {	
		if (isBattle) {
			foreach (var shipData in Player.Instance.ShipDatas) {
				if (shipData.Allegiance != "Player") {
					continue;
				}
				GameObject shipObject = Instantiate (ShipPrefab) as GameObject;
				Ship ship = shipObject.GetComponent<Ship> ();
				ship.InitializeFromData (shipData);
				/*if (ship.Allegiance == "Player") {
					PlayerShips.Add (ship);
				}*/
			}
		}
		TempItemLibrary = new List<Item> (Player.Instance.TempItemLibrary);

		Ships = new List<Ship> (GameObject.FindObjectsOfType<Ship>());
		Buildings = new List<Building> (GameObject.FindObjectsOfType<Building>());

		SlowDown = new Effect ("Slow down", 0, 10.0f, new List<Dictionary<string, int>> {
			new Dictionary<string, int> { { "Speed", -1000 } },
			new Dictionary<string, int> { { "Speed", -2000 } },
			new Dictionary<string, int> { { "Speed", -3000 } },
			new Dictionary<string, int> { { "Speed", -4000 } },
			new Dictionary<string, int> { { "Speed", -5000 } },
			new Dictionary<string, int> { { "Speed", -6000 } },
		});
		SpeedUpFire = new Effect ("Speed up", 0, 10.0f, new List<Dictionary<string, int>> {
			new Dictionary<string, int> { { "Attack speed", -1000 } },
			new Dictionary<string, int> { { "Attack speed", -2000 } },
			new Dictionary<string, int> { { "Attack speed", -3000 } },
			new Dictionary<string, int> { { "Attack speed", -4000 } },
			new Dictionary<string, int> { { "Attack speed", -5000 } },
			new Dictionary<string, int> { { "Attack speed", -6000 } },
		});
		Trade = new Effect ("Trade", 0, -1.0f, new List<Dictionary<string, int>> {
			new Dictionary<string, int> { { "Cargo", 0 } },
			new Dictionary<string, int> { { "Cargo", 1 } },
			new Dictionary<string, int> { { "Cargo", 2 } },
			new Dictionary<string, int> { { "Cargo", 3 } },
			new Dictionary<string, int> { { "Cargo", 4 } },
			new Dictionary<string, int> { { "Cargo", 5 } },
		});
		Cannons = new Effect ("Cannons", 0, -1.0f, new List<Dictionary<string, int>> {
			new Dictionary<string, int> { { "Firepower", 0 } },
			new Dictionary<string, int> { { "Firepower", 10 } },
			new Dictionary<string, int> { { "Firepower", 20 } },
			new Dictionary<string, int> { { "Firepower", 30 } },
			new Dictionary<string, int> { { "Firepower", 40 } },
			new Dictionary<string, int> { { "Firepower", 50 } },
		});
						
		SlowDownSkill = new Skill ("Slow down", 0, 5, new List<int> { 0, 10, 20, 30, 50 }, new Dictionary<string, Effect> { {
				"enemy",
				SlowDown
			} });
		SpeedUpFireSkill = new Skill ("Speed up", 0, 5, new List<int> { 0, 10, 20, 30, 50 }, new Dictionary<string, Effect> { {
				"self",
				SpeedUpFire
			} });
		TradeSkill =	new Skill ("Trade", 0, 5, new List<int> { 0, 10, 20, 30, 50 }, new Dictionary<string, Effect> { {
				"self",
				Trade
			} });
		CannonsSkill =	new Skill ("Cannons", 0, 5, new List<int> { 0, 10, 20, 30, 50 }, new Dictionary<string, Effect> { {
				"self",
				Cannons
			} });
		DummySkill = new Skill ("Dummy", 0, 5, new List<int> { 0, 10, 20, 30, 50 }, null);

		SkillsByNames = new Dictionary<string, Skill> {
			{SlowDownSkill.Name, SlowDownSkill},
			{SpeedUpFireSkill.Name, SpeedUpFireSkill},
			{TradeSkill.Name, TradeSkill},
			{CannonsSkill.Name, CannonsSkill},
			{DummySkill.Name, DummySkill},
		};

		if (!Player.Instance.FirstLoad) {
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
				bool initialized = false;
				foreach (var shipData in Player.Instance.ShipDatas) {
					if (ship.Name == shipData.Name) {
						ship.InitializeFromData (shipData);
						initialized = true;
					}
				}
				if (!initialized) {					
					ShipsToRemove.Add (ship);
				}
			}
			foreach (var ship in ShipsToRemove) {
				Ships.Remove (ship);
				Battleships.Remove (ship.gameObject.GetComponent<BattleShip> ());
				Destroy (ship.gameObject);
			}
		} else {
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
		Debug.Log (PlayerShips.Count.ToString ());
		Player.Instance.SaveShips (PlayerShips);
		Player.Instance.LoadVillage ();
	}

	public Item GetRandomItem (bool craftable) {
		List<Item> items = new List<Item> ();
		foreach (var item in TempItemLibrary) {
			if (!craftable && item.CraftCost != null) {
				continue;
			}
			items.Add (item);
		}
		int index = Random.Range (0, items.Count);
		return items [index];
	}

	public Item GetItemByName (string itemName) {
		foreach (var item in TempItemLibrary) {
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
	}

	public void CloseMissionWindow () {
		MyMissionWindow.Close ();
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
		MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
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
	}

	public void CloseContextButtons (bool deselect) {
		MyButtonsOverlay.Overlay.SetActive (false); // kostyll
		//MyButtonsOverlay.Close ();
		if (deselect && Selection != null) {
			Selection.Deanimate ();
		}
	}
}
