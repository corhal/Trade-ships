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

		foreach (var stat in ship.Stats) {
			GameObject statElementObject = Instantiate (StatElementPrefab) as GameObject;
			Text statText = statElementObject.GetComponent<Text> ();
			statText.text = stat.Name + ": " + stat.Value;

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

	void UpgradeSkill (Ship ship, Skill skill) { // smth wrong; should probably update labels instead
		ship.UpgradeSkill(skill);
		if (Player.Instance.Gold >= skill.UpgradeCosts[skill.Level]) {
			Open (ship);
		} 
	}

	public void Close () {
		Window.SetActive (false);
	}
}
