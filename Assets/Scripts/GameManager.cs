using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public Selectable Selection;

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
		Instance = this;
		ActionIconsByNames = new Dictionary<string, Sprite> ();
		for (int i = 0; i < ActionIcons.Count; i++) {
			ActionIconsByNames.Add (ActionNames [i], ActionIcons [i]);
		}

		ItemIconsByNames = new Dictionary<string, Sprite> ();
		for (int i = 0; i < ItemIcons.Count; i++) {
			ItemIconsByNames.Add (ItemNames [i], ItemIcons [i]);
		}

		foreach (var battleShip in Battleships) {
			battleShip.OnBattleShipDestroyed += BattleShip_OnBattleShipDestroyed;
		}
	}

	void BattleShip_OnBattleShipDestroyed (BattleShip sender) {
		Battleships.Remove (sender);
		sender.OnBattleShipDestroyed -= BattleShip_OnBattleShipDestroyed;
	}

	void Start () {
		Item wood = new Item ("Wood", null, ItemIconsByNames["Wood"], false);
		Item food = new Item ("Food", null, ItemIconsByNames["Food"], true);
		Item steel = new Item ("Steel", null, ItemIconsByNames["Steel"], false);
		Item nails = new Item ("Nails", new Dictionary<Item, int> { { steel, 2 } }, ItemIconsByNames["Nails"], false);
		Item hammers = new Item ("Picks", new Dictionary<Item, int> { { steel, 1 }, {wood, 1} }, ItemIconsByNames["Picks"], false);
		Item saws = new Item ("Shovels", new Dictionary<Item, int> { { steel, 2 }, {wood, 1} }, ItemIconsByNames["Shovels"], false);
		Item tools = new Item ("Tools", new Dictionary<Item, int> { { hammers, 1 }, {saws, 1} }, ItemIconsByNames["Tools"], false);
		Item spices = new Item ("Spices", null, ItemIconsByNames["Spices"], true);
		Item ale = new Item ("Ale", null, ItemIconsByNames["Ale"], true);
		Item fish = new Item ("Fish", null, ItemIconsByNames["Fish"], true);

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

		Ships = new List<Ship> (GameObject.FindObjectsOfType<Ship>());
		Buildings = new List<Building> (GameObject.FindObjectsOfType<Building>());
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
