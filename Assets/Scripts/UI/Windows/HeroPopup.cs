﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroPopup : MonoBehaviour {

	public GameObject Window;

	public GameObject[] StarObjects;

	public Image CreatureFigurineImage;
	public Image[] ItemImages;

	public GameObject StatsBlock;
	public GameObject SkillsBlock;

	public Image ColorPanel;

	public GameObject StatsElementContainer;
	public GameObject SkillsElementContainer;

	public GameObject StatElementPrefab;
	public GameObject SkillElementPrefab;

	public List<GameObject> StatElementObjects;
	public List<SkillElement> SkillElements;

	public Button EvolveButton;

	public Text HeaderLabel;
	public Text LevelLabel;
	public Text ExpLabel;
	public Slider ExpSlider;

	public GameObject BlueprintsNodeObject;
	public Text BlueprintText;
	public Slider BlueprintSlider;

	CreatureData currentShipData;

	public List<RankColor> RankColorRequirements = new List<RankColor> {RankColor.White, RankColor.Green, RankColor.Blue, RankColor.Purple};

	public void Open (CreatureData shipData) {		
		Window.SetActive (true);
		currentShipData = shipData;
		if (Player.Instance.BJDataBase.FigurinesByNames.ContainsKey (shipData.Name)) {
			CreatureFigurineImage.sprite = Player.Instance.BJDataBase.FigurinesByNames [shipData.Name];
			CreatureFigurineImage.SetNativeSize ();
			CreatureFigurineImage.rectTransform.sizeDelta = new Vector2 (CreatureFigurineImage.rectTransform.rect.width / 7, 
				CreatureFigurineImage.rectTransform.rect.height / 7);
		}

		EvolveButton.onClick.RemoveAllListeners ();

		EvolveButton.onClick.AddListener (delegate {
			EvolveShip();
		});

		UpdateLabels (currentShipData);
		OpenStats ();
	}

	public void OpenStats () {
		SkillsBlock.SetActive (false);
		StatsBlock.SetActive (true);

		foreach (var statElementObject in StatElementObjects) {
			Destroy (statElementObject);
		}
		StatElementObjects.Clear ();

		foreach (var statName in currentShipData.StatNames) {
			GameObject statElementObject = Instantiate (StatElementPrefab) as GameObject;
			Text statText = statElementObject.GetComponentInChildren <Text> ();
			statText.text = statName + ": " + currentShipData.GetStatByString (statName);

			statElementObject.transform.SetParent (StatsElementContainer.transform);
			statElementObject.transform.localScale = Vector3.one;
			StatElementObjects.Add (statElementObject);
		}

		foreach (var skillElement in SkillElements) {
			Destroy (skillElement.gameObject);
		}

		SkillElements.Clear ();

		for (int i = 1; i < currentShipData.Skills.Count; i++) {		
			GameObject skillElementObject = Instantiate (SkillElementPrefab) as GameObject;
			SkillElement skillElement = skillElementObject.GetComponent<SkillElement> ();

			skillElement.StatLabel.text = currentShipData.Skills [i].Name;
			if (Player.Instance.BJDataBase.BJSkillsByNames.ContainsKey (currentShipData.Skills [i].Name)) {
				skillElement.SkillImage.sprite = Player.Instance.BJDataBase.BJSkillsByNames [currentShipData.Skills [i].Name].SkillIcon;
			}

			skillElement.UnlockNode.SetActive (false);
			skillElement.StatLabel.gameObject.SetActive (true);
			skillElement.SkillStatLabel.gameObject.SetActive (true);


			skillElementObject.transform.SetParent (StatsElementContainer.transform);
			skillElementObject.transform.localScale = Vector3.one;
			SkillElements.Add (skillElement);
		}
	}

	void EvolveShip () {
		currentShipData.LevelUp ();
		UpdateLabels (currentShipData);
	}

	void UpdateLabels (CreatureData shipData) {
		if (shipData.Level == shipData.LevelCosts.Count) {		
			BlueprintsNodeObject.SetActive (true);
			EvolveButton.gameObject.SetActive (false);
			BlueprintText.gameObject.SetActive (false);
			BlueprintSlider.maxValue = shipData.LevelCosts [shipData.Level - 1];
			BlueprintSlider.value = BlueprintSlider.maxValue;
		} else if (Player.Instance.Inventory [shipData.Soulstone.Name] < shipData.LevelCosts [shipData.Level]) {			
			BlueprintsNodeObject.SetActive (true);
			EvolveButton.gameObject.SetActive (false);
			BlueprintText.text = Player.Instance.Inventory [shipData.Soulstone.Name] + "/" + shipData.LevelCosts [shipData.Level];
			BlueprintSlider.maxValue = shipData.LevelCosts [shipData.Level];
			BlueprintSlider.value = Player.Instance.Inventory [shipData.Soulstone.Name];
		} else {
			BlueprintText.text = Player.Instance.Inventory [shipData.Soulstone.Name] + "/" + shipData.LevelCosts [shipData.Level];
			BlueprintSlider.maxValue = shipData.LevelCosts [shipData.Level];
			BlueprintSlider.value = Player.Instance.Inventory [shipData.Soulstone.Name];
			EvolveButton.gameObject.SetActive (true);
			EvolveButton.GetComponentInChildren<Text> ().text = "level up\n$" + shipData.LevelGoldCosts [shipData.Level];
		}

		LevelLabel.text = "level " + shipData.Level;

		HeaderLabel.text = shipData.Name;
		ColorPanel.color = Player.Instance.DataBase.ColorsByRankColors [shipData.RankColor];
		string rankString = shipData.RankColor.ToString ();
		string pstring = rankString.Substring (rankString.Length - 4, 4);
		int count = 0;
		foreach (var character in pstring) {
			if (character == 'P') {
				count++;
			}
		}
		if (count > 0) {
			HeaderLabel.text = shipData.Name + " +" + count;
		}

		OpenStats ();
	}

	public void FindBlueprint () {
		// gameManager.FindMissionForItem (currentShipData.Soulstone.Name);
	}

	void UpgradeSkill (CreatureData shipData, Skill skill) { // smth wrong; should probably update labels instead
		shipData.UpgradeSkill(skill);
		UpdateLabels (shipData);
	}

	public void Close () {
		Window.SetActive (false);
		UIOverlay.Instance.UpdateHeroCatalogWindow ();
	}
}
