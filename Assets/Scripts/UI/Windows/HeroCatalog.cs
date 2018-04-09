using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCatalog : MonoBehaviour {
	public GameObject Window;

	public GameObject LowerDeckParent;
	public GameObject LowerDeckPrefab;

	public GameObject HeroElementPrefab;

	public GameObject CurrentTeamContainer;
	public List<GameObject> CurrentTeamObjects;
	public GameObject SummonedHeroesContainer;
	public GameObject NotSummonedHeroesContainer;

	public List<GameObject> ShipObjects;

	public List<CreatureData> AllShipDatas;

	public ScrollRect Scroll;

	bool inSwapMode;
	ShipListElement elementReadyToSwap;
	bool firstTime = true;

	int summonedHeroesCount = 0;
	int imagesAmount = 1;

	public void Open () {		
		Window.SetActive (true);

		if (firstTime) {
			foreach (var shipObject in ShipObjects) {
				shipObject.GetComponent<ShipListElement> ().OnShipListElementClicked -= ShipListElement_OnShipListElementClicked;
				shipObject.GetComponent<ShipListElement> ().OnShipListElementReadyToSwap -= ShipListElement_OnShipListElementReadyToSwap;
				shipObject.GetComponent<ShipListElement> ().OnInfoButtonClicked -= ShipListElement_OnInfoButtonClicked;
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
				else if (ship.IsSummoned) {
					shipElementObject.transform.SetParent (SummonedHeroesContainer.transform);
					summonedHeroesCount++;
				} else {
					shipElementObject.transform.SetParent (NotSummonedHeroesContainer.transform);
				}

				shipElementObject.transform.localScale = Vector3.one;
				ShipObjects.Add (shipElementObject);
			}

			foreach (var objectToDelete in objectsToDelete) {
				CurrentTeamObjects.Remove (objectToDelete);
			}

			firstTime = false;
		}

		Scroll.verticalNormalizedPosition = 1.0f;

		UpdateBackgroundImages ();
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

		UpdateBackgroundImages ();
	}

	void UpdateBackgroundImages () {
		int rowsAmount = Mathf.CeilToInt (summonedHeroesCount / 4.0f);
		if (rowsAmount > imagesAmount) {
			GameObject lowerDeckObject = Instantiate (LowerDeckPrefab) as GameObject;
			lowerDeckObject.transform.SetParent (LowerDeckParent.transform);
			lowerDeckObject.transform.localScale = LowerDeckPrefab.transform.localScale;
			lowerDeckObject.GetComponent<RectTransform> ().anchoredPosition = LowerDeckPrefab.GetComponent<RectTransform> ().anchoredPosition;
			lowerDeckObject.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (lowerDeckObject.GetComponent<RectTransform> ().anchoredPosition.x, (-81.8f - 131.09974f * imagesAmount));
			imagesAmount++;
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
		shipListElement.OnShipListElementReadyToSwap += ShipListElement_OnShipListElementReadyToSwap;
		shipListElement.OnInfoButtonClicked += ShipListElement_OnInfoButtonClicked;
		shipListElement.OnHealButtonClicked += ShipListElement_OnHealButtonClicked;

		return shipListElementObject;
	}

	void ShipListElement_OnHealButtonClicked (ShipListElement sender) {
		
	}

	void ShipListElement_OnInfoButtonClicked (ShipListElement sender) {
		sender.InfoButton.gameObject.SetActive (false);
		sender.UseButton.gameObject.SetActive (false);
		if (sender.CreatureData.IsSummoned) {			
			UIOverlay.Instance.OpenShipWindow (sender.gameObject.GetComponentInChildren<ShipElement>().ShipData);
		} else {
			Player.Instance.GiveItems(new Dictionary<string, int> { { sender.CreatureData.Soulstone.Name, Player.Instance.DataBase.LevelCosts [sender.CreatureData.Level]}});
			sender.CreatureData.IsSummoned = true;
			UpdateLabels ();
			sender.gameObject.transform.SetParent (SummonedHeroesContainer.transform);
			summonedHeroesCount++;
		}
	}

	void ShipListElement_OnShipListElementReadyToSwap (ShipListElement sender) {
		sender.InfoButton.gameObject.SetActive (false);
		sender.UseButton.gameObject.SetActive (false);
		inSwapMode = true;
		elementReadyToSwap = sender;
		Scroll.verticalNormalizedPosition = 1.0f;
	}

	void ShipListElement_OnShipListElementClicked (ShipListElement sender) {	
		if (inSwapMode && (Player.Instance.CurrentTeam.Contains(sender.CreatureData) || sender.CreatureData.Name == "")) {			
			int index = CurrentTeamObjects.IndexOf (sender.gameObject);
			Debug.Log(CurrentTeamObjects.IndexOf (sender.gameObject).ToString() + ": " + index);
			Player.Instance.CurrentTeam.RemoveAt (index);
			Player.Instance.CurrentTeam.Insert (index, elementReadyToSwap.CreatureData);

			if (sender.CreatureData.Name != "") {
				CurrentTeamObjects [index].transform.SetParent (SummonedHeroesContainer.transform);
			} else {
				Destroy (CurrentTeamObjects [index]);
				summonedHeroesCount--;
			}

			CurrentTeamObjects.RemoveAt (index);

			elementReadyToSwap.transform.SetParent (CurrentTeamContainer.transform);
			CurrentTeamObjects.Insert (index, elementReadyToSwap.gameObject);
			elementReadyToSwap.transform.SetSiblingIndex (index);
			inSwapMode = false;
		} else if (!inSwapMode) {
			if (sender.CreatureData.Name == "") {
				Scroll.verticalNormalizedPosition = 0.7f;
			} else {
				sender.InfoButton.gameObject.SetActive (!sender.InfoButton.gameObject.activeSelf);
				if (sender.CreatureData.IsSummoned) {
					sender.UseButton.gameObject.SetActive (!sender.UseButton.gameObject.activeSelf);
				}
				if (sender.CreatureData.HP < sender.CreatureData.MaxHP) {
					sender.HealButton.gameObject.SetActive (!sender.HealButton.gameObject.activeSelf);
				}
			}
		}
	}

	public void Close () {
		Window.SetActive (false);
		UIOverlay.Instance.CurrentTeamShower.Open ();
	}

	public void Back () {

	}

}
