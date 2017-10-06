using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipWindow : MonoBehaviour {

	public GameObject Window;

	public GameObject[] StarObjects;

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
	Ship currentShip;

	public GameObject StatsButtonObject;
	public GameObject SkillsButtonObject;

	void Awake () {
		gameManager = GameManager.Instance;
	}

	public void Open (Ship ship) {		
		Window.SetActive (true);
		currentShip = ship;
		HeaderLabel.text = ship.Name;
		ColorPanel.color = Player.Instance.ColorsByRankColors [ship.RankColor];
		string rankString = ship.RankColor.ToString ();
		string pstring = rankString.Substring (rankString.Length - 4, 4);
		int count = 0;
		foreach (var character in pstring) {
			if (character == 'P') {
				count++;
			}
		}
		if (count > 0) {
			HeaderLabel.text = ship.Name + " +" + count;
		}

		foreach (var star in StarObjects) {
			star.SetActive (false);
		}
		for (int i = 0; i < ship.Stars; i++) {
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

		if (currentShip.RankColor == RankColor.OrangeP) {
			PromoteButton.interactable = false;
		}

		if (Player.Instance.Inventory [ship.Blueprint] < ship.EvolveCosts [ship.Stars]) {
			BlueprintsNodeObject.SetActive (true);
			BlueprintText.text = Player.Instance.Inventory [ship.Blueprint] + "/" + ship.EvolveCosts [ship.Stars];
			BlueprintSlider.maxValue = ship.EvolveCosts [ship.Stars];
			BlueprintSlider.value = Player.Instance.Inventory [ship.Blueprint];
		} else {
			BlueprintsNodeObject.SetActive (false);
			EvolveButton.gameObject.SetActive (true);
		}

		Debug.Log (ship.LevelRequirements.Count + "");

		LevelLabel.text = "level " + ship.Level;
		ExpLabel.text = ship.Exp + "/" + ship.LevelRequirements [ship.Level];
		ExpSlider.maxValue = ship.LevelRequirements [ship.Level];
		ExpSlider.value = ship.Exp;

		for (int i = 0; i < ship.PromoteCosts[(int)ship.RankColor].Count; i++) {
			Item item = ship.PromoteCosts [(int)ship.RankColor] [i];
			ItemImages[i].sprite = gameManager.ItemIconsByNames[item.Name];

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

		for (int i = 0; i < currentShip.Skills.Count; i++) {
			SkillElements [i].SkillNameLabel.text = currentShip.Skills [i].Name;
			int stupidLambdaCounter = i;

			if ((int)currentShip.RankColor < (int)currentShip.Skills[i].RankColorReuirement) {
				SkillElements [i].SkillLevelLabel.gameObject.SetActive (false);
				SkillElements [i].UpgradeCostLabel.gameObject.SetActive (false);
				SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (false);
				SkillElements [i].UnlockNode.SetActive (true);
				SkillElements [i].UnlockConditionsLabel.text = "Unlocks at " + currentShip.Skills [i].RankColorReuirement.ToString () + " rank";
			} else {
				SkillElements [i].SkillLevelLabel.gameObject.SetActive (true);
				SkillElements [i].UpgradeCostLabel.gameObject.SetActive (true);
				SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (true);
				SkillElements [i].UnlockNode.SetActive (false);

				SkillElements [i].SkillLevelLabel.text = "level: " +  currentShip.Skills [i].Level;
				SkillElements [i].UpgradeCostLabel.text = "$ " + currentShip.Skills [i].UpgradeCosts [currentShip.Skills [i].Level];


				SkillElements [stupidLambdaCounter].SkillUpgradeButton.onClick.AddListener (delegate {
					UpgradeSkill (currentShip, currentShip.Skills [stupidLambdaCounter]);
				});

				if (currentShip.Skills [i].Level == currentShip.Skills [i].MaxLevel) {
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

		foreach (var statName in currentShip.StatNames) {
			GameObject statElementObject = Instantiate (StatElementPrefab) as GameObject;
			Text statText = statElementObject.GetComponent<Text> ();
			statText.text = statName + ": " + currentShip.GetStatByString (statName);

			statElementObject.transform.SetParent (StatsElementContainer.transform);
			statElementObject.transform.localScale = Vector3.one;
			StatElementObjects.Add (statElementObject);
		}
	}

	void PromoteShip () {
		currentShip.PromoteRank ();
		UpdateLabels (currentShip);
	}

	void EvolveShip () {
		currentShip.EvolveStar ();
		UpdateLabels (currentShip);
	}

	void UpdateLabels (Ship ship) {
		if (ship.Stars == ship.EvolveCosts.Count) {		
			BlueprintsNodeObject.SetActive (true);
			EvolveButton.gameObject.SetActive (false);
			BlueprintText.gameObject.SetActive (false);
			BlueprintSlider.maxValue = ship.EvolveCosts [ship.Stars - 1];
			BlueprintSlider.value = BlueprintSlider.maxValue;
		} else if (Player.Instance.Inventory [ship.Blueprint] < ship.EvolveCosts [ship.Stars]) {			
			BlueprintsNodeObject.SetActive (true);
			EvolveButton.gameObject.SetActive (false);
			BlueprintText.text = Player.Instance.Inventory [ship.Blueprint] + "/" + ship.EvolveCosts [ship.Stars];
			BlueprintSlider.maxValue = ship.EvolveCosts [ship.Stars];
			BlueprintSlider.value = Player.Instance.Inventory [ship.Blueprint];
		} else {
			BlueprintsNodeObject.SetActive (false);
			EvolveButton.gameObject.SetActive (true);
		}

		LevelLabel.text = "level " + ship.Level;
		ExpLabel.text = ship.Exp + "/" + ship.LevelRequirements [ship.Level];
		ExpSlider.maxValue = ship.LevelRequirements [ship.Level];
		ExpSlider.value = ship.Exp;

		if (currentShip.RankColor == RankColor.OrangeP) {
			PromoteButton.interactable = false;
		}
		HeaderLabel.text = ship.Name;
		ColorPanel.color = Player.Instance.ColorsByRankColors [ship.RankColor];
		string rankString = ship.RankColor.ToString ();
		string pstring = rankString.Substring (rankString.Length - 4, 4);
		int count = 0;
		foreach (var character in pstring) {
			if (character == 'P') {
				count++;
			}
		}
		if (count > 0) {
			HeaderLabel.text = ship.Name + " +" + count;
		}

		foreach (var star in StarObjects) {
			star.SetActive (false);
		}
		for (int i = 0; i < ship.Stars; i++) {
			StarObjects [i].SetActive (true);
		}

		if (ship.RankColor != RankColor.OrangeP) {
			for (int i = 0; i < ship.PromoteCosts[(int)ship.RankColor].Count; i++) {
				Item item = ship.PromoteCosts [(int)ship.RankColor] [i];
				ItemImages[i].sprite = gameManager.ItemIconsByNames[item.Name];

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
			for (int i = 0; i < ship.StatNames.Count; i++) {
				GameObject statElementObject = StatElementObjects [i];
				Text statText = statElementObject.GetComponent<Text> ();
				statText.text = ship.StatNames [i] + ": " + ship.GetStatByString (ship.StatNames [i]);
			}
		}

		if (SkillsBlock.activeSelf) {
			for (int i = 0; i < ship.Skills.Count; i++) {
				SkillElement skillElement = SkillElements [i]; 

				skillElement.SkillNameLabel.text = ship.Skills [i].Name;
				int stupidLambdaCounter = i;

				if ((int)currentShip.RankColor < (int)currentShip.Skills[i].RankColorReuirement) {
					SkillElements [i].SkillLevelLabel.gameObject.SetActive (false);
					SkillElements [i].UpgradeCostLabel.gameObject.SetActive (false);
					SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (false);
					SkillElements [i].UnlockNode.SetActive (true);
					SkillElements [i].UnlockConditionsLabel.text = "Unlocks at " + currentShip.Skills [i].RankColorReuirement.ToString () + " rank";
				} else {
					SkillElements [i].SkillLevelLabel.gameObject.SetActive (true);
					SkillElements [i].UpgradeCostLabel.gameObject.SetActive (true);
					SkillElements [stupidLambdaCounter].SkillUpgradeButton.gameObject.SetActive (true);
					SkillElements [i].UnlockNode.SetActive (false);

					SkillElements [i].SkillLevelLabel.text = currentShip.Skills [i].Level.ToString ();
					SkillElements [i].UpgradeCostLabel.text = "$ " + currentShip.Skills [i].UpgradeCosts [currentShip.Skills [i].Level];

					SkillElements [stupidLambdaCounter].SkillUpgradeButton.onClick.RemoveAllListeners ();

					SkillElements [stupidLambdaCounter].SkillUpgradeButton.onClick.AddListener (delegate {
						UpgradeSkill (currentShip, currentShip.Skills [stupidLambdaCounter]);
					});

					if (currentShip.Skills [i].Level == currentShip.Skills [i].MaxLevel) {
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
		gameManager.FindMissionForItem (currentShip.Blueprint);
	}

	void UpgradeSkill (Ship ship, Skill skill) { // smth wrong; should probably update labels instead
		ship.UpgradeSkill(skill);
		UpdateLabels (ship);
	}

	public void Close () {
		Window.SetActive (false);
	}
}
