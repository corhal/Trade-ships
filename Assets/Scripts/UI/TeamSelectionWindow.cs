using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectionWindow : MonoBehaviour {

	public GameObject Window;

	public GameObject AllShipsElementContainer;
	public GameObject TeamShipsElementContainer;

	public GameObject ShipElementPrefab;

	public List<GameObject> AllShipObjects;
	public List<GameObject> TeamShipObjects;

	public Button StartButton;
	public Text MightLabel;

	Mission mission;
	GameManager gameManager;

	public List<ShipData> AllShipDatas;

	void Awake () {
		gameManager = GameManager.Instance;

	}

	public void Open (Mission chosenMission) {		
		Window.SetActive (true);

		this.mission = chosenMission;

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

		Player.Instance.SaveShips (gameManager.Ships);
		AllShipDatas = new List<ShipData> ();
		foreach (var ship in Player.Instance.ShipDatas) {
			if (ship.Allegiance == "Player") {
				AllShipDatas.Add (ship);
			}
		}

		foreach (var ship in AllShipDatas) {
			GameObject shipElementObject = CreateShipElementObject (ship);

			shipElementObject.transform.SetParent (AllShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			AllShipObjects.Add (shipElementObject);
		}

		foreach (var ship in Player.Instance.CurrentTeam) {
			GameObject shipElementObject = CreateShipElementObject (ship);

			shipElementObject.transform.SetParent (TeamShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			TeamShipObjects.Add (shipElementObject);
		}
	}

	GameObject CreateShipElementObject (ShipData shipData) {
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
		if (AllShipObjects.Contains(sender.gameObject) && Player.Instance.CurrentTeam.Count < 5 && !Player.Instance.CurrentTeam.Contains(sender.ShipData)) {
			GameObject shipElementObject = CreateShipElementObject (sender.ShipData);

			shipElementObject.transform.SetParent (TeamShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			TeamShipObjects.Add (shipElementObject);

			Player.Instance.CurrentTeam.Add (sender.ShipData);
		}
		if (TeamShipObjects.Contains(sender.gameObject)) {
			Player.Instance.CurrentTeam.Remove (sender.ShipData);
			TeamShipObjects.Remove (sender.gameObject);
			Destroy (sender.gameObject);
		}
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void StartMission () {		
		Close ();
		Player.Instance.CurrentMission = mission;
		gameManager.LoadBattle ();
	}

	public void Back () {

	}
}
