﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlay : MonoBehaviour {

	public bool InMoveMode;
	public Selectable Selection;

	public Text GoldLabel;
	public Text EnergyLabel;
	Player player;
	public static UIOverlay Instance;

	public TeamSelectionWindow MyTeamSelectionWindow;
	public InfoWindow MyInfoWindow;
	public HeroPopup MyShipWindow;
	public MissionWindow MyMissionWindow;
	public ContextButtonsOverlay MyButtonsOverlay;
	public PopUp MyPopUp;
	public ImagesPopUp MyImagesPopUp;
	public HeroCatalog MyShipsCatalogWindow;
	// public HeroCatalog HeroCatalog;

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
	}

	void Start () {
		player = Player.Instance;
	}

	void Update () { // OMG
		GoldLabel.text = "Gold: " + player.Gold;
		EnergyLabel.text = "Energy: " + player.Energy + "/" + player.MaxEnergy;
	}

	public void OpenSelectableInfo (Selectable selectable) {
		MyInfoWindow.Open (selectable);
		MyShipWindow.Close ();
		MyMissionWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyPopUp.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void OpenShipWindow (CreatureData shipData) {
		MyShipWindow.Open (shipData);
		MyMissionWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
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
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
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
		MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void OpenShipsCatalogWindow () {
		MyShipsCatalogWindow.Open ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
	}

	public void CloseContextButtons (bool deselect) {
		MyButtonsOverlay.Overlay.SetActive (false); // kostyll
		if (deselect && Selection != null) {
			Selection.Deanimate ();
		}
	}
}
