using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipWindow : MonoBehaviour {

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

	public List<GameObject> StatElementObjects;
	public List<SkillElement> SkillElements;

	public Button EvolveButton;
	public Button PromoteButton;

	public Text HeaderLabel;
	public Text LevelLabel;
	public Text ExpLabel;
	public Slider ExpSlider;

	public GameObject BlueprintsNodeObject;
	public Text BlueprintText;
	public Slider BlueprintSlider;

	GameManager gameManager;
	ShipData currentShipData;

	public GameObject StatsButtonObject;
	public GameObject SkillsButtonObject;

	void Awake () {
		gameManager = GameManager.Instance;
	}

	public void Open (ShipData shipData) {		
		Window.SetActive (true);
		currentShipData = shipData;
		CreatureFigurineImage.sprite = Player.Instance.DataBase.CreatureFigurinesByNames [shipData.Name];
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

		foreach (var star in StarObjects) {
			star.SetActive (false);
		}
		for (int i = 0; i < shipData.Stars; i++) {
			StarObjects [i].SetActive (true);
		}

		EvolveButton.onClick.RemoveAllListeners ();
		PromoteButton.onClick.RemoveAllListeners ();

		EvolveButton.onClick.AddListener (delegate {
			EvolveShip();
		});

		PromoteButton.onClick.AddListener (delegate {
			PromoteShip();
		});

		if (currentShipData.RankColor == RankColor.OrangeP) {
			PromoteButton.interactable = false;
		}

		if (Player.Instance.Inventory [shipData.Blueprint] < shipData.EvolveCosts [shipData.Stars]) {
			BlueprintsNodeObject.SetActive (true);
			BlueprintText.text = Player.Instance.Inventory [shipData.Blueprint] + "/" + shipData.EvolveCosts [shipData.Stars];
			BlueprintSlider.maxValue = shipData.EvolveCosts [shipData.Stars];
			BlueprintSlider.value = Player.Instance.Inventory [shipData.Blueprint];
		} else {
			BlueprintsNodeObject.SetActive (false);
			EvolveButton.gameObject.SetActive (true);
		}

		LevelLabel.text = "level " + shipData.Level;
		ExpLabel.text = shipData.Exp + "/" + shipData.LevelRequirements [shipData.Level];
		ExpSlider.maxValue = shipData.LevelRequirements [shipData.Level];
		ExpSlider.value = shipData.Exp;
		for (int i = 0; i < shipData.PromoteCosts[(int)shipData.RankColor].Count; i++) {
			Item item = shipData.PromoteCosts [(int)shipData.RankColor] [i];
			ItemImages[i].sprite = Player.Instance.DataBase.ItemIconsByNames[item.Name];

			if (!Player.Instance.Inventory.ContainsKey(item) || Player.Instance.Inventory[item] == 0) {
				ItemImages [i].color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
				int stupidLambdaCounter = i;
				if (item.CraftCost != null) {
					ItemImages [stupidLambdaCounter].GetComponent<Button> ().onClick.AddListener (delegate {
						gameManager.OpenCraftWindow(null, item);
					});
				} else {
					ItemImages [stupidLambdaCounter].GetComponent<Button> ().onClick.AddListener (delegate {
						gameManager.OpenCraftWindow(null, item);
						//gameManager.FindMissionForItem(item);
					});
				}
			}
		}

		OpenSkills ();
	}

	public void OpenSkills () {
		SkillsBlock.SetActive (true);
		StatsBlock.SetActive (false);

		StatsButtonObject.transform.SetAsFirstSibling ();
		SkillsButtonObject.transform.SetAsLastSibling ();

		if (SkillElements.Count == 0) {
			SkillElements = new List<SkillElement> (gameObject.GetComponentsInChildren<SkillElement> ()); 
		}

		foreach (var skillElement in SkillElements) {
			skillElement.SkillUpgradeButton.onClick.RemoveAllListeners ();
		}

		for (int i = 0; i < currentShipData.Skills.Count; i++) {
			SkillElements [i].SkillNameLabel.text = currentShipData.Skills [i].Name;
			int stupidLambdaCounter = i;

			if ((int)currentShipData.RankColor < (int)currentShipData.Skills[i].RankColorReuirement) {
				SkillElements [i].SkillLevelLabel.gameObject.SetActive (false);
				SkillElements [i].UpgradeCostLabel.gameObject.SetActive (false);
				SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (false);
				SkillElements [i].UnlockNode.SetActive (true);
				SkillElements [i].UnlockConditionsLabel.text = "Unlocks at " + currentShipData.Skills [i].RankColorReuirement.ToString () + " rank";
			} else {
				SkillElements [i].SkillLevelLabel.gameObject.SetActive (true);
				SkillElements [i].UpgradeCostLabel.gameObject.SetActive (true);
				SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (true);
				SkillElements [i].UnlockNode.SetActive (false);

				SkillElements [i].SkillLevelLabel.text = "level: " +  currentShipData.Skills [i].Level;
				SkillElements [i].UpgradeCostLabel.text = "$ " + currentShipData.Skills [i].UpgradeCosts [currentShipData.Skills [i].Level];


				SkillElements [stupidLambdaCounter].SkillUpgradeButton.onClick.AddListener (delegate {
					UpgradeSkill (currentShipData, currentShipData.Skills [stupidLambdaCounter]);
				});

				if (currentShipData.Skills [i].Level == currentShipData.Skills [i].MaxLevel) {
					SkillElements [i].SkillUpgradeButton.gameObject.SetActive (false);
				}
			}
		}
	}

	public void OpenStats () {
		SkillsBlock.SetActive (false);
		StatsBlock.SetActive (true);

		StatsButtonObject.transform.SetAsLastSibling ();
		SkillsButtonObject.transform.SetAsFirstSibling ();

		foreach (var statElementObject in StatElementObjects) {
			Destroy (statElementObject);
		}
		StatElementObjects.Clear ();

		foreach (var statName in currentShipData.StatNames) {
			GameObject statElementObject = Instantiate (StatElementPrefab) as GameObject;
			Text statText = statElementObject.GetComponent<Text> ();
			statText.text = statName + ": " + currentShipData.GetStatByString (statName);

			statElementObject.transform.SetParent (StatsElementContainer.transform);
			statElementObject.transform.localScale = Vector3.one;
			StatElementObjects.Add (statElementObject);
		}
	}

	void PromoteShip () {
		currentShipData.PromoteRank ();
		UpdateLabels (currentShipData);
	}

	void EvolveShip () {
		currentShipData.EvolveStar ();
		UpdateLabels (currentShipData);
	}

	void UpdateLabels (ShipData shipData) {
		if (shipData.Stars == shipData.EvolveCosts.Count) {		
			BlueprintsNodeObject.SetActive (true);
			EvolveButton.gameObject.SetActive (false);
			BlueprintText.gameObject.SetActive (false);
			BlueprintSlider.maxValue = shipData.EvolveCosts [shipData.Stars - 1];
			BlueprintSlider.value = BlueprintSlider.maxValue;
		} else if (Player.Instance.Inventory [shipData.Blueprint] < shipData.EvolveCosts [shipData.Stars]) {			
			BlueprintsNodeObject.SetActive (true);
			EvolveButton.gameObject.SetActive (false);
			BlueprintText.text = Player.Instance.Inventory [shipData.Blueprint] + "/" + shipData.EvolveCosts [shipData.Stars];
			BlueprintSlider.maxValue = shipData.EvolveCosts [shipData.Stars];
			BlueprintSlider.value = Player.Instance.Inventory [shipData.Blueprint];
		} else {
			BlueprintsNodeObject.SetActive (false);
			EvolveButton.gameObject.SetActive (true);
		}

		LevelLabel.text = "level " + shipData.Level;
		ExpLabel.text = shipData.Exp + "/" + shipData.LevelRequirements [shipData.Level];
		ExpSlider.maxValue = shipData.LevelRequirements [shipData.Level];
		ExpSlider.value = shipData.Exp;

		if (currentShipData.RankColor == RankColor.OrangeP) {
			PromoteButton.interactable = false;
		}
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

		foreach (var star in StarObjects) {
			star.SetActive (false);
		}
		for (int i = 0; i < shipData.Stars; i++) {
			StarObjects [i].SetActive (true);
		}

		if (shipData.RankColor != RankColor.OrangeP) {
			for (int i = 0; i < shipData.PromoteCosts[(int)shipData.RankColor].Count; i++) {
				Item item = shipData.PromoteCosts [(int)shipData.RankColor] [i];
				ItemImages[i].sprite = Player.Instance.DataBase.ItemIconsByNames[item.Name];

				if (!Player.Instance.Inventory.ContainsKey(item) || Player.Instance.Inventory[item] == 0) {
					ItemImages [i].color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
					int stupidLambdaCounter = i;
					if (item.CraftCost != null) {
						ItemImages [stupidLambdaCounter].GetComponent<Button> ().onClick.AddListener (delegate {
							gameManager.OpenCraftWindow(null, item);
						});
					} else {
						ItemImages [stupidLambdaCounter].GetComponent<Button> ().onClick.AddListener (delegate {
							gameManager.FindMissionForItem(item);
						});
					}
				}
			}
		} else {
			for (int i = 0; i < 6; i++) {
				ItemImages [i].color = Color.black;
			}
		}

		if (StatsBlock.activeSelf) {
			for (int i = 0; i < shipData.StatNames.Count; i++) {
				GameObject statElementObject = StatElementObjects [i];
				Text statText = statElementObject.GetComponent<Text> ();
				statText.text = shipData.StatNames [i] + ": " + shipData.GetStatByString (shipData.StatNames [i]);
			}
		}

		if (SkillsBlock.activeSelf) {
			for (int i = 0; i < shipData.Skills.Count; i++) {
				SkillElement skillElement = SkillElements [i]; 

				skillElement.SkillNameLabel.text = shipData.Skills [i].Name;
				int stupidLambdaCounter = i;

				if ((int)currentShipData.RankColor < (int)currentShipData.Skills[i].RankColorReuirement) {
					SkillElements [i].SkillLevelLabel.gameObject.SetActive (false);
					SkillElements [i].UpgradeCostLabel.gameObject.SetActive (false);
					SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (false);
					SkillElements [i].UnlockNode.SetActive (true);
					SkillElements [i].UnlockConditionsLabel.text = "Unlocks at " + currentShipData.Skills [i].RankColorReuirement.ToString () + " rank";
				} else {
					SkillElements [i].SkillLevelLabel.gameObject.SetActive (true);
					SkillElements [i].UpgradeCostLabel.gameObject.SetActive (true);
					SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (true);
					SkillElements [i].UnlockNode.SetActive (false);

					SkillElements [i].SkillLevelLabel.text = "level: " + currentShipData.Skills [i].Level;
					SkillElements [i].UpgradeCostLabel.text = "$ " + currentShipData.Skills [i].UpgradeCosts [currentShipData.Skills [i].Level];

					SkillElements [stupidLambdaCounter].SkillUpgradeButton.onClick.RemoveAllListeners ();

					SkillElements [stupidLambdaCounter].SkillUpgradeButton.onClick.AddListener (delegate {
						UpgradeSkill (currentShipData, currentShipData.Skills [stupidLambdaCounter]);
					});

					if (currentShipData.Skills [i].Level == currentShipData.Skills [i].MaxLevel) {
						SkillElements [i].SkillUpgradeButton.gameObject.SetActive (false);
					}
				}
			}
		}
	}

	public void FindItem (Item item) {
		Debug.Log ("Here we are");
		gameManager.FindMissionForItem (item);
	}

	public void FindBlueprint () {
		gameManager.FindMissionForItem (currentShipData.Blueprint);
	}

	void UpgradeSkill (ShipData shipData, Skill skill) { // smth wrong; should probably update labels instead
		shipData.UpgradeSkill(skill);
		UpdateLabels (shipData);
	}

	public void Close () {
		Window.SetActive (false);
	}
}
