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
	BJCreature currentCreature;

	public GameObject StatsButtonObject;
	public GameObject SkillsButtonObject;

	void Awake () {
		gameManager = GameManager.Instance;
	}

	public void Open (BJCreature creature) {		
		Window.SetActive (true);
		currentCreature = creature;
		if (Player.Instance.DataBase.CreatureFigurinesByNames.ContainsKey(creature.Name)) {
			CreatureFigurineImage.sprite = Player.Instance.DataBase.CreatureFigurinesByNames [creature.Name];
		}
		HeaderLabel.text = creature.Name;
		ColorPanel.color = Player.Instance.DataBase.ColorsByRankColors [creature.RankColor];
		string rankString = creature.RankColor.ToString ();
		string pstring = rankString.Substring (rankString.Length - 4, 4);
		int count = 0;
		foreach (var character in pstring) {
			if (character == 'P') {
				count++;
			}
		}
		if (count > 0) {
			HeaderLabel.text = creature.Name + " +" + count;
		}

		foreach (var star in StarObjects) {
			star.SetActive (false);
		}
		for (int i = 0; i < creature.Stars; i++) {
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

		if (currentCreature.RankColor == RankColor.OrangeP) {
			PromoteButton.interactable = false;
		}

		if (Player.Instance.Inventory [creature.Soulstone] < creature.EvolveCosts [creature.Stars]) {
			BlueprintsNodeObject.SetActive (true);
			BlueprintText.text = Player.Instance.Inventory [creature.Soulstone] + "/" + creature.EvolveCosts [creature.Stars];
			BlueprintSlider.maxValue = creature.EvolveCosts [creature.Stars];
			BlueprintSlider.value = Player.Instance.Inventory [creature.Soulstone];
		} else {
			BlueprintsNodeObject.SetActive (false);
			EvolveButton.gameObject.SetActive (true);
		}

		LevelLabel.text = "level " + creature.Level;
		ExpLabel.text = creature.Exp + "/" + creature.LevelRequirements [creature.Level];
		ExpSlider.maxValue = creature.LevelRequirements [creature.Level];
		ExpSlider.value = creature.Exp;
		for (int i = 0; i < creature.PromoteCosts[(int)creature.RankColor].Count; i++) {
			Item item = creature.PromoteCosts [(int)creature.RankColor] [i];
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

		// НЕ СТИРАТЬ!!!!!!!!
		/*for (int i = 0; i < currentCreature.Skills.Count; i++) {
			SkillElements [i].SkillNameLabel.text = currentCreature.Skills [i].Name;
			int stupidLambdaCounter = i;

			if ((int)currentCreature.RankColor < (int)currentCreature.Skills[i].RankColorReuirement) {
				SkillElements [i].SkillLevelLabel.gameObject.SetActive (false);
				SkillElements [i].UpgradeCostLabel.gameObject.SetActive (false);
				SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (false);
				SkillElements [i].UnlockNode.SetActive (true);
				SkillElements [i].UnlockConditionsLabel.text = "Unlocks at " + currentCreature.Skills [i].RankColorReuirement.ToString () + " rank";
			} else {
				SkillElements [i].SkillLevelLabel.gameObject.SetActive (true);
				SkillElements [i].UpgradeCostLabel.gameObject.SetActive (true);
				SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (true);
				SkillElements [i].UnlockNode.SetActive (false);

				SkillElements [i].SkillLevelLabel.text = "level: " +  currentCreature.Skills [i].Level;
				SkillElements [i].UpgradeCostLabel.text = "$ " + currentCreature.Skills [i].UpgradeCosts [currentCreature.Skills [i].Level];


				SkillElements [stupidLambdaCounter].SkillUpgradeButton.onClick.AddListener (delegate {
					UpgradeSkill (currentCreature, currentCreature.Skills [stupidLambdaCounter]);
				});

				if (currentCreature.Skills [i].Level == currentCreature.Skills [i].MaxLevel) {
					SkillElements [i].SkillUpgradeButton.gameObject.SetActive (false);
				}
			}
		}*/
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

		// НЕ СТИРАТЬ!!!!!!!!!!!!!
		/*foreach (var statName in currentCreature.StatNames) {
			GameObject statElementObject = Instantiate (StatElementPrefab) as GameObject;
			Text statText = statElementObject.GetComponent<Text> ();
			statText.text = statName + ": " + currentCreature.GetStatByString (statName);

			statElementObject.transform.SetParent (StatsElementContainer.transform);
			statElementObject.transform.localScale = Vector3.one;
			StatElementObjects.Add (statElementObject);
		}*/
	}

	void PromoteShip () {
		// НЕ СТИРАТЬ!!!!
		// currentCreature.PromoteRank ();
		UpdateLabels (currentCreature);
	}

	void EvolveShip () {
		// НЕ СТИРАТЬ!!!!
		// currentCreature.EvolveStar ();
		UpdateLabels (currentCreature);
	}

	void UpdateLabels (BJCreature creature) {
		if (creature.Stars == creature.EvolveCosts.Count) {		
			BlueprintsNodeObject.SetActive (true);
			EvolveButton.gameObject.SetActive (false);
			BlueprintText.gameObject.SetActive (false);
			BlueprintSlider.maxValue = creature.EvolveCosts [creature.Stars - 1];
			BlueprintSlider.value = BlueprintSlider.maxValue;
		} else if (Player.Instance.Inventory [creature.Soulstone] < creature.EvolveCosts [creature.Stars]) {			
			BlueprintsNodeObject.SetActive (true);
			EvolveButton.gameObject.SetActive (false);
			BlueprintText.text = Player.Instance.Inventory [creature.Soulstone] + "/" + creature.EvolveCosts [creature.Stars];
			BlueprintSlider.maxValue = creature.EvolveCosts [creature.Stars];
			BlueprintSlider.value = Player.Instance.Inventory [creature.Soulstone];
		} else {
			BlueprintsNodeObject.SetActive (false);
			EvolveButton.gameObject.SetActive (true);
		}

		LevelLabel.text = "level " + creature.Level;
		ExpLabel.text = creature.Exp + "/" + creature.LevelRequirements [creature.Level];
		ExpSlider.maxValue = creature.LevelRequirements [creature.Level];
		ExpSlider.value = creature.Exp;

		if (currentCreature.RankColor == RankColor.OrangeP) {
			PromoteButton.interactable = false;
		}
		HeaderLabel.text = creature.Name;
		ColorPanel.color = Player.Instance.DataBase.ColorsByRankColors [creature.RankColor];
		string rankString = creature.RankColor.ToString ();
		string pstring = rankString.Substring (rankString.Length - 4, 4);
		int count = 0;
		foreach (var character in pstring) {
			if (character == 'P') {
				count++;
			}
		}
		if (count > 0) {
			HeaderLabel.text = creature.Name + " +" + count;
		}

		foreach (var star in StarObjects) {
			star.SetActive (false);
		}
		for (int i = 0; i < creature.Stars; i++) {
			StarObjects [i].SetActive (true);
		}

		if (creature.RankColor != RankColor.OrangeP) {
			for (int i = 0; i < creature.PromoteCosts[(int)creature.RankColor].Count; i++) {
				Item item = creature.PromoteCosts [(int)creature.RankColor] [i];
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

		// НЕ СТИРАТЬ!!!!!!
		/*if (StatsBlock.activeSelf) {
			for (int i = 0; i < creature.StatNames.Count; i++) {
				GameObject statElementObject = StatElementObjects [i];
				Text statText = statElementObject.GetComponent<Text> ();
				statText.text = creature.StatNames [i] + ": " + creature.GetStatByString (creature.StatNames [i]);
			}
		}

		if (SkillsBlock.activeSelf) {
			for (int i = 0; i < creature.Skills.Count; i++) {
				SkillElement skillElement = SkillElements [i]; 

				skillElement.SkillNameLabel.text = creature.Skills [i].Name;
				int stupidLambdaCounter = i;

				if ((int)currentCreature.RankColor < (int)currentCreature.Skills[i].RankColorReuirement) {
					SkillElements [i].SkillLevelLabel.gameObject.SetActive (false);
					SkillElements [i].UpgradeCostLabel.gameObject.SetActive (false);
					SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (false);
					SkillElements [i].UnlockNode.SetActive (true);
					SkillElements [i].UnlockConditionsLabel.text = "Unlocks at " + currentCreature.Skills [i].RankColorReuirement.ToString () + " rank";
				} else {
					SkillElements [i].SkillLevelLabel.gameObject.SetActive (true);
					SkillElements [i].UpgradeCostLabel.gameObject.SetActive (true);
					SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (true);
					SkillElements [i].UnlockNode.SetActive (false);

					SkillElements [i].SkillLevelLabel.text = "level: " + currentCreature.Skills [i].Level;
					SkillElements [i].UpgradeCostLabel.text = "$ " + currentCreature.Skills [i].UpgradeCosts [currentCreature.Skills [i].Level];

					SkillElements [stupidLambdaCounter].SkillUpgradeButton.onClick.RemoveAllListeners ();

					SkillElements [stupidLambdaCounter].SkillUpgradeButton.onClick.AddListener (delegate {
						UpgradeSkill (currentCreature, currentCreature.Skills [stupidLambdaCounter]);
					});

					if (currentCreature.Skills [i].Level == currentCreature.Skills [i].MaxLevel) {
						SkillElements [i].SkillUpgradeButton.gameObject.SetActive (false);
					}
				}
			}
		}*/
	}

	public void FindItem (Item item) {
		Debug.Log ("Here we are");
		gameManager.FindMissionForItem (item);
	}

	public void FindBlueprint () {
		gameManager.FindMissionForItem (currentCreature.Soulstone);
	}

	void UpgradeSkill (ShipData shipData, Skill skill) { // smth wrong; should probably update labels instead
		shipData.UpgradeSkill(skill);
		// НЕ СТИРАТЬ!!!!!!
		// UpdateLabels (shipData);
	}

	public void Close () {
		Window.SetActive (false);
	}
}
