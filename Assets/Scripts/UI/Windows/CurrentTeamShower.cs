using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTeamShower : MonoBehaviour {
	public GameObject Window;

	public GameObject HeroElementPrefab;

	public GameObject CurrentTeamContainer;
	public List<GameObject> CurrentTeamObjects;

	public List<GameObject> ShipObjects;

	public List<CreatureData> AllShipDatas;

	bool firstTime = true;

	int summonedHeroesCount = 0;
	int imagesAmount = 1;

	public void Open () {		
		Window.SetActive (true);

		//if (firstTime) {
			foreach (var shipObject in ShipObjects) {
				shipObject.GetComponent<ShipListElement> ().OnShipListElementClicked -= ShipListElement_OnShipListElementClicked;
				shipObject.GetComponent<ShipListElement> ().OnHealButtonClicked -= ShipListElement_OnHealButtonClicked;
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
			List<GameObject> objectsToDelete = new List<GameObject> ();
			foreach (var ship in AllShipDatas) {

				GameObject shipElementObject = CreateShipListElementObject (ship);

				if (Player.Instance.CurrentTeam.Contains(ship)) {
					shipElementObject.transform.SetParent (CurrentTeamContainer.transform);
					objectsToDelete.Add (CurrentTeamObjects [Player.Instance.CurrentTeam.IndexOf (ship)]);
					Destroy (CurrentTeamObjects [Player.Instance.CurrentTeam.IndexOf(ship)]);
					shipElementObject.transform.SetSiblingIndex (Player.Instance.CurrentTeam.IndexOf(ship));
					CurrentTeamObjects.Add (shipElementObject);
				}

				shipElementObject.transform.localScale = Vector3.one;
				ShipObjects.Add (shipElementObject);
			}

			foreach (var objectToDelete in objectsToDelete) {
				CurrentTeamObjects.Remove (objectToDelete);
			}
			//firstTime = false;
		//}
	}

	public void UpdateLabels () {
		foreach (var obj in ShipObjects) {
			ShipListElement shipListElement = obj.GetComponent<ShipListElement> ();
			ShipElement shipElement = obj.GetComponentInChildren<ShipElement> ();

			shipListElement.DamageSlider.maxValue = shipListElement.CreatureData.MaxHP;
			shipListElement.DamageSlider.value = shipListElement.CreatureData.HP;

			shipElement.LevelLabel.text = "level " + shipElement.ShipData.Level.ToString ();

			shipListElement.SoulstonesSlider.maxValue = Player.Instance.DataBase.LevelCosts [shipListElement.CreatureData.Level];
			shipListElement.SoulstonesSlider.value = Player.Instance.Inventory [shipListElement.CreatureData.Soulstone.Name];

			shipListElement.SoulstonesSlider.GetComponentInChildren<Text>().text = Player.Instance.Inventory [shipListElement.CreatureData.Soulstone.Name] + "/" + Player.Instance.DataBase.LevelCosts [shipListElement.CreatureData.Level];
			if (!shipListElement.CreatureData.IsSummoned) {
				if (!Player.Instance.Inventory.ContainsKey(shipListElement.CreatureData.Soulstone.Name)) { // temporary fix for crash!!
					Player.Instance.Inventory.Add (shipListElement.CreatureData.Soulstone.Name, 0);
				}
			}

			if (Player.Instance.Inventory [shipListElement.CreatureData.Soulstone.Name] >= Player.Instance.DataBase.LevelCosts [shipListElement.CreatureData.Level]) {				
				shipListElement.InfoButton.GetComponentInChildren<Text> ().text = (shipListElement.CreatureData.IsSummoned) ? "Upgrade" : "Summon";
			} else {
				shipListElement.InfoButton.GetComponentInChildren<Text> ().text = "Information";
			}
		}
	}

	GameObject CreateShipListElementObject (CreatureData creatureData) {
		GameObject shipListElementObject = Instantiate (HeroElementPrefab) as GameObject;
		ShipElement shipElement = shipListElementObject.GetComponentInChildren<ShipElement> ();
		shipElement.ShipData = creatureData;

		if (Player.Instance.BJDataBase.FigurinesByNames.ContainsKey(creatureData.Name)) {
			shipElement.PortraitImage.sprite = Player.Instance.BJDataBase.FigurinesByNames [creatureData.Name];
			shipElement.PortraitImage.SetNativeSize ();
		}
		shipElement.NameLabel.text = creatureData.Name;
		shipElement.LevelLabel.text = "level " + creatureData.Level.ToString ();

		ShipListElement shipListElement = shipListElementObject.GetComponent<ShipListElement> ();

		shipListElement.CreatureData = creatureData;

		shipListElement.DamageSlider.maxValue = shipListElement.CreatureData.MaxHP;
		shipListElement.DamageSlider.value = shipListElement.CreatureData.MaxHP - shipListElement.CreatureData.HP;

		shipListElement.SoulstonesSlider.maxValue = Player.Instance.DataBase.LevelCosts [creatureData.Level];
		shipListElement.SoulstonesSlider.value = Player.Instance.Inventory [creatureData.Soulstone.Name];

		shipListElement.SoulstonesSlider.GetComponentInChildren<Text>().text = Player.Instance.Inventory [creatureData.Soulstone.Name] + "/" + Player.Instance.DataBase.LevelCosts [creatureData.Level];
		if (!creatureData.IsSummoned) {
			if (!Player.Instance.Inventory.ContainsKey(creatureData.Soulstone.Name)) { // temporary fix for crash!!
				Player.Instance.Inventory.Add (creatureData.Soulstone.Name, 0);
			}
		}

		if (Player.Instance.Inventory [shipListElement.CreatureData.Soulstone.Name] >= Player.Instance.DataBase.LevelCosts [shipListElement.CreatureData.Level]) {
			shipListElement.InfoButton.GetComponentInChildren<Text> ().text = (shipListElement.CreatureData.IsSummoned) ? "Upgrade" : "Summon";
		} else {
			shipListElement.InfoButton.GetComponentInChildren<Text> ().text = "Information";
		}

		shipListElement.GetComponent<Button> ().enabled = true;
		shipListElement.OnShipListElementClicked += ShipListElement_OnShipListElementClicked;
		shipListElement.OnHealButtonClicked += ShipListElement_OnHealButtonClicked;

		return shipListElementObject;
	}

	void ShipListElement_OnHealButtonClicked (ShipListElement sender) {
		
	}


	void ShipListElement_OnShipListElementClicked (ShipListElement sender) {		
		sender.InfoButton.gameObject.SetActive (!sender.InfoButton.gameObject.activeSelf);
		sender.UseButton.gameObject.SetActive (false);
		if (sender.CreatureData.HP < sender.CreatureData.MaxHP) {
			sender.HealButton.gameObject.SetActive (!sender.HealButton.gameObject.activeSelf);
		}		
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void Back () {

	}
}
