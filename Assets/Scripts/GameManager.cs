using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public List<Adventure> Adventures;

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
		foreach (var tile in Tiles) {
			if (Player.Instance.Tiles [tile.BoardCoordsAsString] == false) {				
				tile.StopParticles ();
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

			// temp solution:
			if (Player.Instance.CurrentMission.Name != "") {
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
