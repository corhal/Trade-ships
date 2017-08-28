using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionWindow : MonoBehaviour {

	public GameObject Window;

	public GameObject RewardsElementContainer;
	public GameObject ShipsElementContainer;

	public GameObject RewardElementPrefab;
	public GameObject ShipElementPrefab;

	public List<GameObject> RewardElementObjects;
	public List<GameObject> ShipElementObjects;

	public Button StartButton;
	public Button ChangeShipsButton;

	public Text HeaderLabel;
	public Text RequirementsLabel;
	public Text RecomendationsLabel;

	Mission mission;
	GameManager gameManager;

	void Awake () {
		gameManager = GameManager.Instance;
	}

	public void Open (Mission mission) {
		Window.SetActive (true);
		this.mission = mission;

		foreach (var rewardElementObject in RewardElementObjects) {
			Destroy (rewardElementObject);
		}
		RewardElementObjects.Clear ();

		foreach (var shipElementObject in ShipElementObjects) {
			Destroy (shipElementObject);
		}
		ShipElementObjects.Clear ();

		foreach (var amountByItem in mission.PossibleRewards) {
			GameObject rewardElementObject = Instantiate (RewardElementPrefab) as GameObject;
			Text[] texts = rewardElementObject.GetComponentsInChildren<Text> ();
			texts [0].text = amountByItem.Key.Name;
			texts [1].text = amountByItem.Value.ToString ();

			rewardElementObject.transform.SetParent (RewardsElementContainer.transform);
			rewardElementObject.transform.localScale = Vector3.one;
			RewardElementObjects.Add (rewardElementObject);
		}

		foreach (var ship in gameManager.Ships) {
			GameObject shipElementObject = Instantiate (ShipElementPrefab) as GameObject;
			Text nameText = shipElementObject.GetComponentInChildren<Text> ();
			nameText.text = ship.Name;

			shipElementObject.transform.SetParent (ShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			ShipElementObjects.Add (shipElementObject);
		}

		string requirementsString = "";
		foreach (var levelByName in mission.BuildingRequirements) {
			requirementsString += levelByName.Key + ": lvl" + levelByName.Value + "\n";
		}
		requirementsString += "Cargo: " + mission.CargoRequirements;
		RequirementsLabel.text = requirementsString;

		// StartButton.onClick.RemoveAllListeners ();
	}

	public void Close () {
		Window.SetActive (false);
	}
}
