using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCatalog : MonoBehaviour {
	public GameObject Window;

	public GameObject HeroElementPrefab;

	public GameObject CurrentTeamContainer;
	public List<GameObject> CurrentTeamObjects;
	public GameObject SummonedHeroesContainer;
	public GameObject NotSummonedHeroesContainer;

	public List<GameObject> ShipObjects;

	public List<CreatureData> AllShipDatas;
	bool inSwapMode;
	ShipListElement elementReadyToSwap;
	bool firstTime = true;

	public void Open () {		
		Window.SetActive (true);

		if (firstTime) {
			foreach (var shipObject in ShipObjects) {
				shipObject.GetComponent<ShipListElement> ().OnShipListElementClicked -= ShipListElement_OnShipListElementClicked;
				shipObject.GetComponent<ShipListElement> ().OnShipListElementReadyToSwap -= ShipListElement_OnShipListElementReadyToSwap;
				shipObject.GetComponent<ShipListElement> ().OnInfoButtonClicked -= ShipListElement_OnInfoButtonClicked;
				Destroy (shipObject);
			}
			ShipObjects.Clear ();

			foreach (var obj in CurrentTeamObjects) {
				obj.GetComponent<ShipListElement> ().OnShipListElementClicked += ShipListElement_OnShipListElementClicked;
			}

			AllShipDatas = new List<CreatureData> ();
			foreach (var ship in Player.Instance.ShipDatas) {
				AllShipDatas.Add (ship);
			}
			foreach (var ship in AllShipDatas) {

				GameObject shipElementObject = CreateShipListElementObject (ship);

				if (Player.Instance.CurrentTeam.Contains(ship)) {
					shipElementObject.transform.SetParent (CurrentTeamContainer.transform);
					Destroy (CurrentTeamObjects [Player.Instance.CurrentTeam.IndexOf(ship)]);
					//CurrentTeamObjects.Insert (0, shipElementObject);
					shipElementObject.transform.SetSiblingIndex (Player.Instance.CurrentTeam.IndexOf(ship));
				}
				else if (ship.IsSummoned) {
					shipElementObject.transform.SetParent (SummonedHeroesContainer.transform);
				} else {
					shipElementObject.transform.SetParent (NotSummonedHeroesContainer.transform);
				}

				shipElementObject.transform.localScale = Vector3.one;
				ShipObjects.Add (shipElementObject);
			}
			firstTime = false;
		}

	}

	GameObject CreateShipListElementObject (CreatureData creatureData) {
		GameObject shipListElementObject = Instantiate (HeroElementPrefab) as GameObject;
		ShipElement shipElement = shipListElementObject.GetComponentInChildren<ShipElement> ();
		shipElement.ShipData = creatureData;

		if (Player.Instance.BJDataBase.CreaturePortraitsByNames.ContainsKey(creatureData.Name)) {
			shipElement.PortraitImage.sprite = Player.Instance.BJDataBase.CreaturePortraitsByNames [creatureData.Name];
		}
		shipElement.NameLabel.text = creatureData.Name;
		shipElement.LevelLabel.text = "level " + creatureData.Level.ToString ();

		for (int i = 0; i < 5; i++) {
			shipElement.Stars [i].SetActive (false);
		}

		for (int i = 0; i < creatureData.Stars; i++) {
			shipElement.Stars [i].SetActive (true);
		}

		ShipListElement shipListElement = shipListElementObject.GetComponent<ShipListElement> ();
		shipListElement.CreatureData = creatureData;
		shipListElement.SoulstonesSlider.maxValue = Player.Instance.DataBase.EvolveCosts [creatureData.Stars];
		shipListElement.SoulstonesSlider.value = Player.Instance.Inventory [creatureData.Soulstone.Name];

		shipListElement.SoulstonesSlider.GetComponentInChildren<Text>().text = Player.Instance.Inventory [creatureData.Soulstone.Name] + "/" + Player.Instance.DataBase.EvolveCosts [creatureData.Stars];
		if (!creatureData.IsSummoned) {
			if (!Player.Instance.Inventory.ContainsKey(creatureData.Soulstone.Name)) { // temporary fix for crash!!
				Player.Instance.Inventory.Add (creatureData.Soulstone.Name, 0);
			}
			if (Player.Instance.Inventory [creatureData.Soulstone.Name] > Player.Instance.DataBase.EvolveCosts [creatureData.Stars]) {
				shipListElement.SoulstonesSlider.gameObject.SetActive (false);
				// shipListElement.SummonButton.gameObject.SetActive (true);
			}
		} else {			
			// shipListElement.SummonButton.gameObject.SetActive (false);
		}

		shipListElement.GetComponent<Button> ().enabled = true;
		shipListElement.OnShipListElementClicked += ShipListElement_OnShipListElementClicked;
		shipListElement.OnShipListElementReadyToSwap += ShipListElement_OnShipListElementReadyToSwap;
		shipListElement.OnInfoButtonClicked += ShipListElement_OnInfoButtonClicked;

		return shipListElementObject;
	}

	void ShipListElement_OnInfoButtonClicked (ShipListElement sender) {
		sender.InfoButton.gameObject.SetActive (false);
		sender.UseButton.gameObject.SetActive (false);
		UIOverlay.Instance.OpenShipWindow (sender.gameObject.GetComponentInChildren<ShipElement>().ShipData);
	}

	void ShipListElement_OnShipListElementReadyToSwap (ShipListElement sender) {
		sender.InfoButton.gameObject.SetActive (false);
		sender.UseButton.gameObject.SetActive (false);
		inSwapMode = true;
		elementReadyToSwap = sender;
	}

	void ShipListElement_OnShipListElementClicked (ShipListElement sender) {	
		if (inSwapMode && (Player.Instance.CurrentTeam.Contains(sender.CreatureData) || sender.CreatureData.Name == "")) {			
			int index = CurrentTeamObjects.IndexOf (sender.gameObject);

			Player.Instance.CurrentTeam.RemoveAt (index);
			Player.Instance.CurrentTeam.Insert (index, elementReadyToSwap.CreatureData);

			if (sender.CreatureData.Name != "") {
				CurrentTeamObjects [index].transform.SetParent (SummonedHeroesContainer.transform);
			} else {
				Destroy (CurrentTeamObjects [index]);
			}

			CurrentTeamObjects.RemoveAt (index);

			elementReadyToSwap.transform.SetParent (CurrentTeamContainer.transform);
			CurrentTeamObjects.Insert (index, elementReadyToSwap.gameObject);
			elementReadyToSwap.transform.SetSiblingIndex (index);
			inSwapMode = false;
		} else if (!inSwapMode) {
			sender.InfoButton.gameObject.SetActive (!sender.InfoButton.gameObject.activeSelf);
			if (sender.CreatureData.IsSummoned) {
				sender.UseButton.gameObject.SetActive (!sender.UseButton.gameObject.activeSelf);
			}
		}
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void Back () {

	}

}
