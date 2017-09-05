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
	public Text SuccessLabel;
	public Text TimeLabel;

	Mission mission;
	GameManager gameManager;
	List<Ship> ChosenShips;

	ExpeditionCenter currentExpeditionCenter;
	int successChance;

	void Awake () {
		gameManager = GameManager.Instance;
	}

	public void Open (ExpeditionCenter expeditionCenter, Mission chosenMission) {		
		Window.SetActive (true);
		currentExpeditionCenter = expeditionCenter;
		this.mission = chosenMission;

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
		ChosenShips = expeditionCenter.MyIsland.MyPort.DockedShips;
		foreach (var ship in ChosenShips) {
			GameObject shipElementObject = Instantiate (ShipElementPrefab) as GameObject;
			Text nameText = shipElementObject.GetComponentInChildren<Text> ();
			nameText.text = ship.Name;

			shipElementObject.transform.SetParent (ShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			ShipElementObjects.Add (shipElementObject);
		}

		string requirementsString = "";
		foreach (var levelByName in mission.BuildingRequirements) {
			string tickstring = " X";
			foreach (var building in gameManager.Buildings) {
				if (building.name == levelByName.Key && building.Level >= levelByName.Value) {
					tickstring = " V";
				}
			}
			requirementsString += levelByName.Key + ": lvl" + levelByName.Value + tickstring + "\n";
		}
		RequirementsLabel.text = requirementsString;

		int totalPower = 0;
		foreach (var ship in ChosenShips) {
			totalPower += ship.Power;
		}
		successChance = (int)(((float)totalPower / (float)mission.Power) * 100);
		SuccessLabel.text = "Success chance: " +  successChance + "%";
		int seconds = mission.Seconds % 60;
		int minutes = (mission.Seconds - seconds) / 60;
		int newMinutes = minutes % 60;
		int hours = (minutes - newMinutes) / 60;
		TimeLabel.text = hours + ":" + newMinutes + ":" + seconds;
		// StartButton.onClick.RemoveAllListeners ();
	}

	bool CheckRequirements () {
		foreach (var levelByName in mission.BuildingRequirements) {
			foreach (var building in gameManager.Buildings) {
				if (building.name == levelByName.Key && building.Level < levelByName.Value) {
					return false;
				}
			}
		}
		return true;
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void StartMission () {
		if (CheckRequirements ()) {
			currentExpeditionCenter.StartMission (mission, successChance);
			Close ();
		} else {
			Debug.Log ("Mission requirements not met!");
		}

	}
}
