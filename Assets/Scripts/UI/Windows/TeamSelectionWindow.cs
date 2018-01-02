using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectionWindow : MonoBehaviour {

	public bool HealMode;
	public bool ResurrectMode;
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

	public List<CreatureData> AllShipDatas;

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

		AllShipDatas = new List<CreatureData> ();
		foreach (var ship in Player.Instance.ShipDatas) {
			if (ship.Allegiance == Allegiance.Player && ship.IsSummoned) {
				AllShipDatas.Add (ship);
			}
		}

		foreach (var ship in AllShipDatas) {
			GameObject shipElementObject = CreateShipElementObject (ship);

			shipElementObject.transform.SetParent (AllShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;

			ShipElement shipElement = shipElementObject.GetComponent<ShipElement> ();

			shipElement.DamageSlider.maxValue = shipElement.ShipData.MaxHP;
			shipElement.DamageSlider.value = shipElement.ShipData.MaxHP - shipElement.ShipData.HP;
			AllShipObjects.Add (shipElementObject);
		}

		foreach (var ship in Player.Instance.CurrentTeam) {
			if (ship == null || ship.Name == "") {
				continue;
			}
			GameObject shipElementObject = CreateShipElementObject (ship);

			shipElementObject.transform.SetParent (TeamShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;

			ShipElement shipElement = shipElementObject.GetComponent<ShipElement> ();

			shipElement.DamageSlider.maxValue = shipElement.ShipData.MaxHP;
			shipElement.DamageSlider.value = shipElement.ShipData.MaxHP - shipElement.ShipData.HP;

			TeamShipObjects.Add (shipElementObject);
		}
	}

	GameObject CreateShipElementObject (CreatureData shipData) {
		GameObject shipElementObject = Instantiate (ShipElementPrefab) as GameObject;
		ShipElement shipElement = shipElementObject.GetComponent<ShipElement> ();
		shipElement.ShipData = shipData;
		if (Player.Instance.BJDataBase.CreaturePortraitsByNames.ContainsKey(shipData.Name)) {
			shipElement.PortraitImage.sprite = Player.Instance.BJDataBase.CreaturePortraitsByNames [shipData.Name];
		}
		shipElement.NameLabel.text = shipData.Name;
		shipElement.LevelLabel.text = shipData.Level.ToString ();

		for (int i = 0; i < 5; i++) {
			shipElement.Stars [i].SetActive (false);
		}

		for (int i = 0; i < shipData.Level; i++) {
			shipElement.Stars [i].SetActive (true);
		}

		shipElementObject.GetComponent<Button> ().enabled = true;
		shipElement.OnShipElementClicked += ShipElement_OnShipElementClicked;

		return shipElementObject;
	}

	void RefreshDamageSliders () {
		foreach (var shipObject in AllShipObjects) {
			ShipElement shipElement = shipObject.GetComponent<ShipElement> ();

			shipElement.DamageSlider.maxValue = shipElement.ShipData.MaxHP;
			shipElement.DamageSlider.value = shipElement.ShipData.MaxHP - shipElement.ShipData.HP;
		}

		foreach (var shipObject in TeamShipObjects) {
			ShipElement shipElement = shipObject.GetComponent<ShipElement> ();

			shipElement.DamageSlider.maxValue = shipElement.ShipData.MaxHP;
			shipElement.DamageSlider.value = shipElement.ShipData.MaxHP - shipElement.ShipData.HP;
		}
	}

	public void Heal () {
		HealMode = true;
	}

	public void Resurrect () {
		ResurrectMode = true;
	}

	void ShipElement_OnShipElementClicked (ShipElement sender) {

		if (HealMode && sender.ShipData.HP < sender.ShipData.MaxHP) {
			sender.ShipData.Creature.Heal (sender.ShipData.MaxHP - sender.ShipData.HP);
			HealMode = false;
			RefreshDamageSliders ();
			return;
		}

		if (sender.ShipData.IsDead) {
			if (ResurrectMode) {
				sender.ShipData.Creature.Resurrect ();
				ResurrectMode = false;
			}
			RefreshDamageSliders ();
			return;
		}

		if (AllShipObjects.Contains(sender.gameObject) && Player.Instance.CurrentTeam.Count < 5 && !Player.Instance.CurrentTeam.Contains(sender.ShipData)) {
			GameObject shipElementObject = CreateShipElementObject (sender.ShipData);

			shipElementObject.transform.SetParent (TeamShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;

			ShipElement shipElement = shipElementObject.GetComponent<ShipElement> ();

			shipElement.DamageSlider.maxValue = shipElement.ShipData.MaxHP;
			shipElement.DamageSlider.value = shipElement.ShipData.MaxHP - shipElement.ShipData.HP;

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
		int counter = 0;
		foreach (var ship in Player.Instance.CurrentTeam) {
			if (ship == null || ship.Name == "") {
				counter++;
			}
		}
		if (counter == 0) {
			Close ();
			Player.Instance.CurrentMission = mission;
			gameManager.LoadBattle ();
		} else {
			UIOverlay.Instance.OpenPopUp ("Choose at least one ship!");
		}
	}

	public void Back () {

	}
}
