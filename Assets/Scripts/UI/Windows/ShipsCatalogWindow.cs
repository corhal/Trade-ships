using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipsCatalogWindow : MonoBehaviour {
	public GameObject Window;

	public GameObject ShipsElementContainer;

	public GameObject CreatureListElementPrefab;

	public List<GameObject> ShipObjects;

	GameManager gameManager;

	public List<BJCreature> AllCreatures;

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

		AllCreatures = new List<BJCreature> ();
		foreach (var ship in Player.Instance.Creatures) {
			//if (ship.Allegiance == "Player") {
				AllCreatures.Add (ship);
			//}
		}
		foreach (var creature in AllCreatures) {
			
			GameObject shipElementObject = CreateCreatureListElementObject (creature);

			shipElementObject.transform.SetParent (ShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			ShipObjects.Add (shipElementObject);
		}
	}

	GameObject CreateCreatureListElementObject (BJCreature creature) {
		GameObject creatureListElementObject = Instantiate (CreatureListElementPrefab) as GameObject;
		CreatureElement creatureElement = creatureListElementObject.GetComponentInChildren<CreatureElement> ();
		creatureElement.Creature = creature;
		if (Player.Instance.DataBase.CreaturePortraitsByNames.ContainsKey(creature.Name)) {
			creatureElement.PortraitImage.sprite = Player.Instance.DataBase.CreaturePortraitsByNames [creature.Name];
		}
		creatureElement.NameLabel.text = creature.Name;
		Debug.Log (creatureElement.NameLabel.text);
		creatureElement.LevelLabel.text = creature.Level.ToString ();

		for (int i = 0; i < 5; i++) {
			creatureElement.Stars [i].SetActive (false);
		}

		for (int i = 0; i < creature.Stars; i++) {
			creatureElement.Stars [i].SetActive (true);
		}

		ShipListElement shipListElement = creatureListElementObject.GetComponent<ShipListElement> ();
		if (!creature.IsSummoned) {
			if (!Player.Instance.Inventory.ContainsKey(creature.Soulstone)) { // temporary fix for crash!!
				Player.Instance.Inventory.Add (creature.Soulstone, 0);
			}
			if (Player.Instance.Inventory [creature.Soulstone] < Player.Instance.DataBase.EvolveCosts [creature.Stars]) {
				shipListElement.BlueprintsSlider.maxValue = Player.Instance.DataBase.EvolveCosts [creature.Stars];
				shipListElement.BlueprintsSlider.value = Player.Instance.Inventory [creature.Soulstone];

				shipListElement.BlueprintsSlider.GetComponentInChildren<Text>().text = Player.Instance.Inventory [creature.Soulstone] + "/" + Player.Instance.DataBase.EvolveCosts [creature.Stars];
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
				shipListElement.ItemImages [i].sprite = Player.Instance.DataBase.ItemIconsByNames [creature.PromoteCosts [(int)creature.RankColor] [i].Name];
			}
		}

		shipListElement.GetComponent<Button> ().enabled = true;
		shipListElement.OnShipListElementClicked += ShipListElement_OnShipListElementClicked;

		return creatureListElementObject;
	}

	void ShipListElement_OnShipListElementClicked (ShipListElement sender) {		
		if (sender.gameObject.GetComponentInChildren<CreatureElement>().Creature.IsSummoned) {
			//List<Ship> AllShips = new List<Ship> (FindObjectsOfType<Ship> ());
			//foreach (var ship in AllShips) {
				//if (ship.Name == sender.gameObject.GetComponentInChildren<ShipElement>().ShipData.Name) {
					gameManager.OpenShipWindow (sender.gameObject.GetComponentInChildren<CreatureElement>().Creature);
					//break;
				//}
			//}
		} else {
			gameManager.FindMissionForItem (sender.gameObject.GetComponentInChildren<CreatureElement> ().Creature.Soulstone);
		}
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void Back () {

	}

}
