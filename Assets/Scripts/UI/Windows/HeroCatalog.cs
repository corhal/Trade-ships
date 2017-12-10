using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCatalog : MonoBehaviour {
	public GameObject Window;

	public GameObject HeroElementPrefab;

	public GameObject CurrentTeamContainer;
	public GameObject SummonedHeroesContainer;
	public GameObject NotSummonedHeroesContainer;

	public List<GameObject> ShipObjects;

	public List<CreatureData> AllShipDatas;


	public void Open () {		
		Window.SetActive (true);

		foreach (var shipObject in ShipObjects) {
			shipObject.GetComponent<ShipListElement> ().OnShipListElementClicked -= ShipListElement_OnShipListElementClicked;
			Destroy (shipObject);
		}
		ShipObjects.Clear ();

		AllShipDatas = new List<CreatureData> ();
		foreach (var ship in Player.Instance.ShipDatas) {
			AllShipDatas.Add (ship);
		}
		foreach (var ship in AllShipDatas) {
			
			GameObject shipElementObject = CreateShipListElementObject (ship);

			if (Player.Instance.CurrentTeam.Contains(ship)) {
				shipElementObject.transform.SetParent (CurrentTeamContainer.transform);
			}
			else if (ship.IsSummoned) {
				shipElementObject.transform.SetParent (SummonedHeroesContainer.transform);
			} else {
				shipElementObject.transform.SetParent (NotSummonedHeroesContainer.transform);
			}

			shipElementObject.transform.localScale = Vector3.one;
			ShipObjects.Add (shipElementObject);
		}
	}

	GameObject CreateShipListElementObject (CreatureData shipData) {
		GameObject shipListElementObject = Instantiate (HeroElementPrefab) as GameObject;
		ShipElement shipElement = shipListElementObject.GetComponentInChildren<ShipElement> ();
		shipElement.ShipData = shipData;
		if (Player.Instance.BJDataBase.CreaturePortraitsByNames.ContainsKey(shipData.Name)) {
			shipElement.PortraitImage.sprite = Player.Instance.BJDataBase.CreaturePortraitsByNames [shipData.Name];
		}
		shipElement.NameLabel.text = shipData.Name;
		shipElement.LevelLabel.text = "level " + shipData.Level.ToString ();

		for (int i = 0; i < 5; i++) {
			shipElement.Stars [i].SetActive (false);
		}

		for (int i = 0; i < shipData.Stars; i++) {
			shipElement.Stars [i].SetActive (true);
		}

		ShipListElement shipListElement = shipListElementObject.GetComponent<ShipListElement> ();
		shipListElement.SoulstonesSlider.maxValue = Player.Instance.DataBase.EvolveCosts [shipData.Stars];
		shipListElement.SoulstonesSlider.value = Player.Instance.Inventory [shipData.Soulstone.Name];

		shipListElement.SoulstonesSlider.GetComponentInChildren<Text>().text = Player.Instance.Inventory [shipData.Soulstone.Name] + "/" + Player.Instance.DataBase.EvolveCosts [shipData.Stars];
		if (!shipData.IsSummoned) {
			if (!Player.Instance.Inventory.ContainsKey(shipData.Soulstone.Name)) { // temporary fix for crash!!
				Player.Instance.Inventory.Add (shipData.Soulstone.Name, 0);
			}
			if (Player.Instance.Inventory [shipData.Soulstone.Name] > Player.Instance.DataBase.EvolveCosts [shipData.Stars]) {
				shipListElement.SoulstonesSlider.gameObject.SetActive (false);
				shipListElement.SummonButton.gameObject.SetActive (true);
			}
		} else {			
			shipListElement.SummonButton.gameObject.SetActive (false);
		}

		shipListElement.GetComponent<Button> ().enabled = true;
		shipListElement.OnShipListElementClicked += ShipListElement_OnShipListElementClicked;

		return shipListElementObject;
	}

	void ShipListElement_OnShipListElementClicked (ShipListElement sender) {		
		if (sender.gameObject.GetComponentInChildren<ShipElement>().ShipData.IsSummoned) {
			UIOverlay.Instance.OpenShipWindow (sender.gameObject.GetComponentInChildren<ShipElement>().ShipData);
		} else {
			// gameManager.FindMissionForItem (sender.gameObject.GetComponentInChildren<ShipElement> ().ShipData.Soulstone.Name);
		}
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void Back () {

	}

}
