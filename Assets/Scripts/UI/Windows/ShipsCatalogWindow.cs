using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipsCatalogWindow : MonoBehaviour {
	public GameObject Window;

	public GameObject ShipsElementContainer;

	public GameObject ShipListElementPrefab;

	public List<GameObject> ShipObjects;

	GameManager gameManager;

	public List<ShipData> AllShipDatas;

	void Awake () {
		gameManager = GameManager.Instance;
	}

	public void Open () {		
		Window.SetActive (true);

		foreach (var shipObject in ShipObjects) {
			shipObject.GetComponent<ShipListElement> ().OnShipListElementClicked -= ShipListElement_OnShipListElementClicked;
			Destroy (shipObject);
		}
		ShipObjects.Clear ();

		AllShipDatas = new List<ShipData> ();
		foreach (var ship in Player.Instance.ShipDatas) {
			//if (ship.Allegiance == "Player") {
				AllShipDatas.Add (ship);
			//}
		}
		foreach (var ship in AllShipDatas) {
			
			GameObject shipElementObject = CreateShipListElementObject (ship);

			shipElementObject.transform.SetParent (ShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			ShipObjects.Add (shipElementObject);
		}
	}

	GameObject CreateShipListElementObject (ShipData shipData) {
		GameObject shipListElementObject = Instantiate (ShipListElementPrefab) as GameObject;
		ShipElement shipElement = shipListElementObject.GetComponentInChildren<ShipElement> ();
		shipElement.ShipData = shipData;
		if (Player.Instance.DataBase.CreaturePortraitsByNames.ContainsKey(shipData.Name)) {
			shipElement.PortraitImage.sprite = Player.Instance.DataBase.CreaturePortraitsByNames [shipData.Name];
		}
		shipElement.NameLabel.text = shipData.Name;
		shipElement.LevelLabel.text = shipData.Level.ToString ();

		for (int i = 0; i < 5; i++) {
			shipElement.Stars [i].SetActive (false);
		}

		for (int i = 0; i < shipData.Stars; i++) {
			shipElement.Stars [i].SetActive (true);
		}

		ShipListElement shipListElement = shipListElementObject.GetComponent<ShipListElement> ();
		if (!shipData.IsSummoned) {
			if (!Player.Instance.Inventory.ContainsKey(shipData.Soulstone.Name)) { // temporary fix for crash!!
				Player.Instance.Inventory.Add (shipData.Soulstone.Name, 0);
			}
			if (Player.Instance.Inventory [shipData.Soulstone.Name] < Player.Instance.DataBase.EvolveCosts [shipData.Stars]) {
				shipListElement.BlueprintsSlider.maxValue = Player.Instance.DataBase.EvolveCosts [shipData.Stars];
				shipListElement.BlueprintsSlider.value = Player.Instance.Inventory [shipData.Soulstone.Name];

				shipListElement.BlueprintsSlider.GetComponentInChildren<Text>().text = Player.Instance.Inventory [shipData.Soulstone.Name] + "/" + Player.Instance.DataBase.EvolveCosts [shipData.Stars];
			} else {
				shipListElement.BlueprintsSlider.gameObject.SetActive (false);
				shipListElement.SummonButton.gameObject.SetActive (true);
			}
		} else {
			shipListElement.BlueprintsSlider.gameObject.SetActive (false);
			shipListElement.SummonButton.gameObject.SetActive (false);
			shipListElement.ItemsParent.SetActive (true);
			for (int i = 0; i < shipListElement.ItemImages.Count; i++) {
				//Debug.Log (shipData.PromoteCosts);
				shipListElement.ItemImages [i].sprite = Player.Instance.DataBase.ItemIconsByNames [shipData.PromoteCosts [(int)shipData.RankColor] [i]];
			}
		}

		shipListElement.GetComponent<Button> ().enabled = true;
		shipListElement.OnShipListElementClicked += ShipListElement_OnShipListElementClicked;

		return shipListElementObject;
	}

	void ShipListElement_OnShipListElementClicked (ShipListElement sender) {		
		if (sender.gameObject.GetComponentInChildren<ShipElement>().ShipData.IsSummoned) {
			//List<Ship> AllShips = new List<Ship> (FindObjectsOfType<Ship> ());
			//foreach (var ship in AllShips) {
				//if (ship.Name == sender.gameObject.GetComponentInChildren<ShipElement>().ShipData.Name) {
					gameManager.OpenShipWindow (sender.gameObject.GetComponentInChildren<ShipElement>().ShipData);
					//break;
				//}
			//}
		} else {
			gameManager.FindMissionForItem (sender.gameObject.GetComponentInChildren<ShipElement> ().ShipData.Soulstone.Name);
		}
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void Back () {

	}

}
