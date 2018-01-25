﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlay : MonoBehaviour {

	public bool InMoveMode;
	public Selectable Selection;

	public Text GoldLabel;
	public Text EnergyLabel;
	public Text TimeLabel;
	public Text RewardChestsLabel;
	Player player;
	public static UIOverlay Instance;

	public List<GameObject> HideInAdventureObjects;
	public List<GameObject> HideOffAdventureObjects;

	public TeamSelectionWindow MyTeamSelectionWindow;
	public InfoWindow MyInfoWindow;
	public HeroPopup MyShipWindow;
	public MissionWindow MyMissionWindow;
	public ContextButtonsOverlay MyButtonsOverlay;
	public PopUp MyPopUp;
	public ImagesPopUp MyImagesPopUp;
	public HeroCatalog MyShipsCatalogWindow;
	public AdventureSelectionWindow AdventureSelectionWindow;

	public GameObject MapNode;

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
	}

	public List<GameObject> ShipRewardChestImages;
	public List<GameObject> ShipRewardChestTexts;

	public void UpdateShipRewardChests (PlayerShip playerShip) {
		for (int i = 0; i < ShipRewardChestImages.Count; i++) {
			if (i < playerShip.RewardChests.Count) {
				ShipRewardChestImages [i].SetActive (true);
				ShipRewardChestTexts [i].SetActive (false);
			} else {
				ShipRewardChestImages [i].SetActive (false);
				ShipRewardChestTexts [i].SetActive (true);
			}
		}
	}

	public GameObject FlyingRewardPrefab;

	public float JourneyTime;
	float startTime;

	Vector3 startPosition;
	Vector3 targetPosition;
	bool flyReward;
	GameObject flyingReward;

	public void FlyReward (Sprite sprite, Transform initialPosition, GameObject destinationNode) {
		flyingReward = Instantiate (FlyingRewardPrefab) as GameObject;
		flyingReward.transform.position = initialPosition.position;
		flyingReward.GetComponentInChildren<SpriteRenderer> ().sprite = sprite;

		flyingReward.GetComponentInChildren<SpriteRenderer> ().transform.localScale *= 0.15f; // kostyll

		startTime = Time.time;
		startPosition = flyingReward.transform.position;
		targetPosition = destinationNode.transform.position;
		flyReward = true;
	}

	void Start () {
		player = Player.Instance;
		if (player.OnAdventure) {
			foreach (var hideObject in HideInAdventureObjects) {
				hideObject.SetActive (false);
			}
			foreach (var hideObject in HideOffAdventureObjects) {
				hideObject.SetActive (true);
			}
			if (Player.Instance.CurrentAdventure.TreasureHunt) {
				MapNode.SetActive (true);
			} else {
				MapNode.SetActive (false);
			}	
			UpdateShipRewardChests (GameManager.Instance.PlayerShip);
		} else {
			MapNode.SetActive (false);
			foreach (var hideObject in HideInAdventureObjects) {
				hideObject.SetActive (true);
			}
			foreach (var hideObject in HideOffAdventureObjects) {
				hideObject.SetActive (false);
			}
		}
	}

	void Update () { // OMG
		GoldLabel.text = "" + player.Gold;
		EnergyLabel.text = "" + player.Energy + "/" + player.MaxEnergy;
		RewardChestsLabel.text = "" + player.RewardChests.Count;
		if (player.OnAdventure) {			
			int time = (int)player.AdventureTimer;
			int hours = time / 3600;
			int minutes = (time - hours * 3600) / 60;
			int seconds = time - hours * 3600 - minutes * 60;
			TimeLabel.text = hours + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
			if (player.CurrentAdventure.TreasureHunt) {
				MapNode.GetComponentInChildren<Text> ().text = player.Inventory ["Map"] + "/" + player.CurrentAdventure.MapsForTreasure;
			}
		}

		if (flyReward) {
			float dTime = (Time.time - startTime) / JourneyTime;			
			flyingReward.transform.position = Vector3.Lerp(startPosition, targetPosition, dTime);
			if (Vector3.Distance (flyingReward.transform.position, targetPosition) < 0.01f) {
				flyReward = false;
				Destroy (flyingReward);				
			}
		}
	}

	public GameObject PreviousAdventureButtonObject;
	public GameObject NextAdventureButtonObject;
	public Image AdventureImage;
	public List<Sprite> AdventureSprites;

	public void ChangeAdventure (bool next) {
		Player.Instance.ChangeAdventure (next);
		if (Player.Instance.Adventures.IndexOf(Player.Instance.CurrentAdventure) == 0) {
			PreviousAdventureButtonObject.SetActive (false);
			if (Player.Instance.Adventures.Count > 1) {
				NextAdventureButtonObject.SetActive (true);
			}
		} else if (Player.Instance.Adventures.IndexOf(Player.Instance.CurrentAdventure) == Player.Instance.Adventures.Count - 1) {
			NextAdventureButtonObject.SetActive (false);
			if (Player.Instance.Adventures.Count > 1) {
				PreviousAdventureButtonObject.SetActive (true);
			}
		} else {
			if (Player.Instance.Adventures.Count > 1) {
				NextAdventureButtonObject.SetActive (true);
				PreviousAdventureButtonObject.SetActive (true);
			}
		}
		AdventureImage.sprite = AdventureSprites [Player.Instance.Adventures.IndexOf (Player.Instance.CurrentAdventure)];
	}

	public void OpenAdventureSelectionWindow () {
		AdventureSelectionWindow.Open ();
		MyInfoWindow.Close ();
		// MyShipWindow.Close ();
		MyMissionWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyPopUp.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void CloseAdventureSelectionWindow () {
		if (AdventureSelectionWindow != null) {
			AdventureSelectionWindow.Close ();
		}
	}

	public void OpenSelectableInfo (Selectable selectable) {
		MyInfoWindow.Open (selectable);
		// MyShipWindow.Close ();
		MyMissionWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyPopUp.Close ();
		MyShipsCatalogWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void OpenShipWindow (CreatureData shipData) {
		MyShipWindow.Open (shipData);
		MyMissionWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void OpenPopUp (string message) {
		MyPopUp.Open (message);
	}

	public void OpenImagesPopUp (string message, Dictionary<string, int> itemNames) {
		MyImagesPopUp.Open (message, itemNames);
	}

	public void OpenMissionWindow (Mission chosenMission) {
		MyMissionWindow.Open (chosenMission);
		MyButtonsOverlay.Close ();
		// MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void CloseMissionWindow () {
		MyMissionWindow.Close ();
	}

	public void OpenTeamSelectionWindow (Mission mission) {
		MyTeamSelectionWindow.Open (mission);
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		// MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void OpenContextButtons (Selectable selectable) {
		if (InMoveMode) {
			return;
		}
		CloseContextButtons (true);
		Selection = selectable;
		Selection.Animate ();
		MyButtonsOverlay.Open (selectable);
		MyMissionWindow.Close ();
		// MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void OpenShipsCatalogWindow () {
		MyShipsCatalogWindow.Open ();
		MyButtonsOverlay.Close ();
		// MyMissionWindow.Close ();
		// MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void UpdateHeroCatalogWindow () {
		MyShipsCatalogWindow.UpdateLabels ();
	}

	public void CloseContextButtons (bool deselect) {
		MyButtonsOverlay.Overlay.SetActive (false); // kostyll
		if (deselect && Selection != null) {
			Selection.Deanimate ();
		}
	}

	public void CloseAllWindows () {
		MyMissionWindow.Reload ();
		MyButtonsOverlay.Close ();
		// MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
		CloseAdventureSelectionWindow ();
	}
}
