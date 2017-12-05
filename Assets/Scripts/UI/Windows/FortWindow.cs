﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FortWindow : MonoBehaviour {

	public GameObject Window;

	public GameObject AllShipsElementContainer;
	public GameObject TeamShipsElementContainer;

	public GameObject ShipElementPrefab;

	public List<GameObject> AllShipObjects;
	public List<GameObject> TeamShipObjects;

	public Button StartButton;
	public Text MightLabel;

	GameManager gameManager;

	public List<CreatureData> AllShipDatas;

	//bool teamChanged;
	List<CreatureData> initialTeam;

	void Awake () {
		gameManager = GameManager.Instance;

	}

	public void Open () {		
		/*Window.SetActive (true);
		initialTeam = new List<ShipData> (Player.Instance.HomeTeam);
		foreach (var shipObject in AllShipObjects) {
			shipObject.GetComponent<ShipElement> ().OnShipElementClicked -= ShipElement_OnShipElementClicked;
			Destroy (shipObject);
		}
		AllShipObjects.Clear ();

		foreach (var shipObject in TeamShipObjects) {
			shipObject.GetComponent<ShipElement> ().OnShipElementClicked -= ShipElement_OnShipElementClicked;
			Destroy (shipObject);
		}
		TeamShipObjects.Clear ();

		AllShipDatas = new List<ShipData> ();
		foreach (var ship in Player.Instance.ShipDatas) {
			if (ship.Allegiance == "Player" && ship.IsSummoned) {
				AllShipDatas.Add (ship);
			}
		}

		foreach (var ship in AllShipDatas) {
			GameObject shipElementObject = CreateShipElementObject (ship);

			shipElementObject.transform.SetParent (AllShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			AllShipObjects.Add (shipElementObject);
		}

		foreach (var ship in Player.Instance.HomeTeam) {
			GameObject shipElementObject = CreateShipElementObject (ship);

			shipElementObject.transform.SetParent (TeamShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			TeamShipObjects.Add (shipElementObject);
		}*/
	}

	GameObject CreateShipElementObject (CreatureData shipData) {
		GameObject shipElementObject = Instantiate (ShipElementPrefab) as GameObject;
		ShipElement shipElement = shipElementObject.GetComponent<ShipElement> ();
		shipElement.ShipData = shipData;
		shipElement.NameLabel.text = shipData.Name;
		shipElement.LevelLabel.text = shipData.Level.ToString ();

		for (int i = 0; i < 5; i++) {
			shipElement.Stars [i].SetActive (false);
		}

		for (int i = 0; i < shipData.Stars; i++) {
			shipElement.Stars [i].SetActive (true);
		}

		shipElementObject.GetComponent<Button> ().enabled = true;
		shipElement.OnShipElementClicked += ShipElement_OnShipElementClicked;

		return shipElementObject;
	}

	void ShipElement_OnShipElementClicked (ShipElement sender) {		
		/*if (AllShipObjects.Contains(sender.gameObject) && Player.Instance.HomeTeam.Count < 5 && !Player.Instance.HomeTeam.Contains(sender.ShipData)) {
			GameObject shipElementObject = CreateShipElementObject (sender.ShipData);

			shipElementObject.transform.SetParent (TeamShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			TeamShipObjects.Add (shipElementObject);

			Player.Instance.HomeTeam.Add (sender.ShipData);
		}
		if (TeamShipObjects.Contains(sender.gameObject)) {
			Player.Instance.HomeTeam.Remove (sender.ShipData);
			TeamShipObjects.Remove (sender.gameObject);
			Destroy (sender.gameObject);
		}*/
	}

	public void Close () {
		Window.SetActive (false);
		/*bool teamChanged = false;
		if (initialTeam == null) {
			return;
		}
		if (Player.Instance.HomeTeam.Count != initialTeam.Count) {
			teamChanged = true;
		}
		foreach (var shipData in Player.Instance.HomeTeam) {
			if (!initialTeam.Contains(shipData)) {
				teamChanged = true;
			}
		}
		if (teamChanged) {
			gameManager.RespawnShips ();
		}*/
	}


	public void Back () {

	}
}
