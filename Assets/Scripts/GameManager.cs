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

	public List<SelectableTile> Tiles;
	public List<MissionObject> MissionObjects;

	public static GameManager Instance;

	public bool CameraDragged;

	public void MoveMode () {
		InMoveMode = true;
		UIOverlay.Instance.CloseContextButtons (false);
	}

	public void StartAdventure () {
		if (Player.Instance.OnAdventure) {
			Player.Instance.LoadVillage ();
		} else {
			Player.Instance.NewBoard = true;
			Player.Instance.LoadAdventure ();
		}
	}

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
		Board.OnBoardGenerationFinished += Board_OnBoardGenerationFinished;
	}

	void Board_OnBoardGenerationFinished () {
		int treasuresToReveal = 0;
		while (Player.Instance.CurrentAdventure.TreasureHunt && Player.Instance.Inventory["Map"] >= Player.Instance.CurrentAdventure.MapsForTreasure) {
			treasuresToReveal++;
			Player.Instance.GiveItems (new Dictionary<string, int> { { "Map", Player.Instance.CurrentAdventure.MapsForTreasure } });
		}
		foreach (var tile in Tiles) {
			if (Player.Instance.Tiles [tile.BoardCoordsAsString] == false) {				
				tile.StopParticles ();
			} else if (treasuresToReveal > 0 && tile.PointOfInterest == POIkind.Chest) {	
				tile.gameObject.GetComponentInChildren<Chest> ().FindTiles ();
				tile.gameObject.GetComponentInChildren<Chest> ().Reveal ();
				treasuresToReveal--;
			}
		}
	}

	void OnDestroy () {
		Board.OnBoardGenerationFinished -= Board_OnBoardGenerationFinished;
	}

	public bool isBattle; // ew

	public void AddEnergy () {
		Player.Instance.Energy += 100;
	}

	void Start () {	
		if (!isBattle) {
			CityHUD.SetActive (true);
		}

		if (!Player.Instance.FirstLoad && !isBattle) { // ?..

			PlayerShip.gameObject.transform.position = Player.Instance.PlayerShipCoordinates;

			Camera.main.transform.position = new Vector3 (PlayerShip.gameObject.transform.position.x, PlayerShip.gameObject.transform.position.y, Camera.main.transform.position.z);

			PlayerShip.RewardChests = new List<RewardChest> (Player.Instance.PlayerShipRewardChests);
			// temp solution:
			if (Player.Instance.CurrentMission.Name != "" && Player.Instance.OnAdventure && !Player.Instance.ReceivedReward) {
				Player.Instance.ReceivedReward = true;
				Dictionary<string, int> reward = Player.Instance.CurrentMission.GiveReward ();
				Player.Instance.TakeItems (reward);
				UIOverlay.Instance.OpenImagesPopUp ("Your reward:", reward);
			}

		} else if (!isBattle) {			
			Player.Instance.CreateShipDatas ();
			Player.Instance.SavePlayerShip (PlayerShip);
			Player.Instance.FirstLoad = false;
		}
	}			

	public void LoadBattle () {
		Player.Instance.SavePlayerShip (PlayerShip);
		Player.Instance.LoadBattle ();
	}

	public void LoadVillage () {
		Player.Instance.LoadVillage ();
	}

	public Item GetItemByName (string itemName) {
		foreach (var item in Player.Instance.DataBase.TempItemLibrary) {
			if (item.Name == itemName) {
				return item;
			}
		}
		return null;
	}
}
