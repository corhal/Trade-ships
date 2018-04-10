using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealPopUp : MonoBehaviour {
	public GameObject Window;

	public GameObject HeroElementPrefab;

	public GameObject CurrentHeroContainer;
	public GameObject CurrentHero;

	public List<Button> HealButtons;

	public void Open (CreatureData selectedHero) {		
		Window.SetActive (true);
			
		Destroy (CurrentHero);

		GameObject shipElementObject = CreateShipListElementObject (selectedHero);
				
		shipElementObject.transform.SetParent (CurrentHeroContainer.transform);
		CurrentHero = shipElementObject;

		shipElementObject.transform.localScale = Vector3.one;
		shipElementObject.transform.localPosition = Vector3.zero;
		CurrentHero.GetComponentInChildren<ShipListElement> ().CreatureData = selectedHero;

		HealButtons [0].GetComponent<Image> ().sprite = Player.Instance.DataBase.ItemIconsByNames ["Small healing potion"];
		HealButtons [0].GetComponentInChildren<Text> ().text = "x" + Player.Instance.Inventory ["Small healing potion"];
		HealButtons [1].GetComponent<Image> ().sprite = Player.Instance.DataBase.ItemIconsByNames ["Medium healing potion"];
		HealButtons [1].GetComponentInChildren<Text> ().text = "x" + Player.Instance.Inventory ["Medium healing potion"];
		HealButtons [2].GetComponent<Image> ().sprite = Player.Instance.DataBase.ItemIconsByNames ["Big healing potion"];
		HealButtons [2].GetComponentInChildren<Text> ().text = "x" + Player.Instance.Inventory ["Big healing potion"];
	}

	public void Heal (string potionName) {
		Dictionary<string, int> heals = new Dictionary<string, int> {
			{ 
				"Small healing potion", 10 
			},
			{
				"Medium healing potion",
				50
			},
			{
				"Big healing potion",
				100
			}
		};
		CreatureData currentCreature = CurrentHero.GetComponentInChildren<ShipListElement> ().CreatureData;
		if (Player.Instance.Inventory[potionName] > 0) {
			currentCreature.Creature.Heal (heals[potionName]);
			Player.Instance.Inventory [potionName]--;
			UpdateLabels ();
		}
	}

	public void UpdateLabels () {
		ShipListElement shipListElement = CurrentHero.GetComponent<ShipListElement> ();
		ShipElement shipElement = CurrentHero.GetComponentInChildren<ShipElement> ();

		shipListElement.DamageSlider.maxValue = shipListElement.CreatureData.MaxHP;
		shipListElement.DamageSlider.value = shipListElement.CreatureData.MaxHP - shipListElement.CreatureData.HP;

		HealButtons [0].GetComponentInChildren<Text> ().text = "x" + Player.Instance.Inventory ["Small healing potion"];
		HealButtons [1].GetComponentInChildren<Text> ().text = "x" + Player.Instance.Inventory ["Medium healing potion"];
		HealButtons [2].GetComponentInChildren<Text> ().text = "x" + Player.Instance.Inventory ["Big healing potion"];
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

		shipListElement.SoulstonesSlider.gameObject.SetActive (false);

		shipListElement.GetComponent<Button> ().enabled = false;

		return shipListElementObject;
	}

	public void Close () {
		Window.SetActive (false);
		UIOverlay.Instance.CurrentTeamShower.Open ();
	}

	public void Back () {

	}
}
