using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipWindow : MonoBehaviour {

	public GameObject Window;

	public GameObject StatsElementContainer;
	public GameObject SkillsElementContainer;

	public GameObject StatElementPrefab;
	public GameObject SkillElementPrefab;

	public List<GameObject> StatElementObjects;
	public List<GameObject> SkillElementObjects;

	public Text HeaderLabel;
	public Text LevelLabel;
	public Text ExpLabel;
	public Slider ExpSlider;

	GameManager gameManager;
	Ship currentShip;

	void Awake () {
		gameManager = GameManager.Instance;
	}

	public void Open (Ship ship) {		
		Window.SetActive (true);
		currentShip = ship;
		HeaderLabel.text = ship.Name;
		foreach (var statElementObject in StatElementObjects) {
			Destroy (statElementObject);
		}
		StatElementObjects.Clear ();

		foreach (var skillElementObject in SkillElementObjects) {
			Destroy (skillElementObject);
		}
		SkillElementObjects.Clear ();

		foreach (var statName in ship.StatNames) {
			GameObject statElementObject = Instantiate (StatElementPrefab) as GameObject;
			Text statText = statElementObject.GetComponent<Text> ();
			statText.text = statName + ": " + ship.GetStatByString (statName);

			statElementObject.transform.SetParent (StatsElementContainer.transform);
			statElementObject.transform.localScale = Vector3.one;
			StatElementObjects.Add (statElementObject);
		}

		foreach (var skill in ship.Skills) {
			GameObject skillElementObject = Instantiate (SkillElementPrefab) as GameObject;
			SkillElement skillElement = skillElementObject.GetComponentInChildren<SkillElement> ();

			skillElement.SkillNameLabel.text = skill.Name;
			skillElement.SkillLevelLabel.text = skill.Level.ToString ();
			skillElement.UpgradeButtonLabel.text = "$ " + skill.UpgradeCosts[skill.Level];

			skillElement.SkillUpgradeButton.onClick.AddListener (delegate {
				UpgradeSkill(ship, skill);
			});

			if (skill.Level == skill.MaxLevel) {
				skillElement.SkillUpgradeButton.gameObject.SetActive (false);
			}

			skillElementObject.transform.SetParent (SkillsElementContainer.transform);
			skillElementObject.transform.localScale = Vector3.one;
			SkillElementObjects.Add (skillElementObject);
		}
	}

	void UpdateLabels (Ship ship) {
		for (int i = 0; i < ship.StatNames.Count; i++) {
			GameObject statElementObject = StatElementObjects [i];
			Text statText = statElementObject.GetComponent<Text> ();
			statText.text = ship.StatNames [i] + ": " + ship.GetStatByString (ship.StatNames [i]);
		}

		for (int i = 0; i < ship.Skills.Count; i++) {
			GameObject skillElementObject = SkillElementObjects [i];
			SkillElement skillElement = skillElementObject.GetComponentInChildren<SkillElement> ();

			skillElement.SkillNameLabel.text = ship.Skills [i].Name;
			skillElement.SkillLevelLabel.text = ship.Skills [i].Level.ToString ();

			if (ship.Skills [i].Level == ship.Skills [i].MaxLevel) {
				skillElement.SkillUpgradeButton.gameObject.SetActive (false);
			} else {
				skillElement.UpgradeButtonLabel.text = "$ " + ship.Skills [i].UpgradeCosts[ship.Skills [i].Level];
			}
		}
	}

	void UpgradeSkill (Ship ship, Skill skill) { // smth wrong; should probably update labels instead
		ship.UpgradeSkill(skill);
		UpdateLabels (ship);
	}

	public void Close () {
		Window.SetActive (false);
	}
}
