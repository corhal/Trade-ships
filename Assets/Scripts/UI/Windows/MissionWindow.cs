using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionWindow : MonoBehaviour {

	public GameObject Window;

	public GameObject RewardsElementContainer;
	public GameObject EnemiesElementContainer;

	public GameObject RewardElementPrefab;
	public GameObject EnemyElementPrefab;

	public List<GameObject> RewardElementObjects;
	public List<GameObject> EnemyElementObjects;

	public Button StartButton;
	public Text TimeLabel;
	public Button RaidButton;
	public Text RaidCostLabel;
	public Button RaidX3Button;
	public Text RaidX3CostLabel;

	public Text HeaderLabel;

	Mission mission;

	// ExpeditionCenter expeditionCenter;

	void Awake () {
	}

	public void Open (/*ExpeditionCenter expeditionCenter,*/ Mission chosenMission) {		
		Window.SetActive (true);

		//this.expeditionCenter = expeditionCenter;
		this.mission = chosenMission;

		foreach (var rewardElementObject in RewardElementObjects) {
			Destroy (rewardElementObject);
		}
		RewardElementObjects.Clear ();

		foreach (var enemyElementObject in EnemyElementObjects) {
			Destroy (enemyElementObject);
		}
		EnemyElementObjects.Clear ();

		foreach (var amountByItem in mission.PossibleRewards) {
			GameObject rewardElementObject = Instantiate (RewardElementPrefab) as GameObject;
			Text[] texts = rewardElementObject.GetComponentsInChildren<Text> ();
			texts [0].text = amountByItem.Key;
			/*if (amountByItem.Key.Name == "") {
				Debug.Log (amountByItem.Key);
			}*/
			texts [1].text = amountByItem.Value.ToString ();
			Image rewardImage = rewardElementObject.GetComponentInChildren<Image> ();
			rewardImage.sprite = Player.Instance.DataBase.ItemIconsByNames [amountByItem.Key];
			rewardElementObject.transform.SetParent (RewardsElementContainer.transform);
			rewardElementObject.transform.localScale = Vector3.one;
			RewardElementObjects.Add (rewardElementObject);
		}
		List<CreatureData> Enemies = new List<CreatureData> (chosenMission.EnemyShips);

		foreach (var enemy in Enemies) {
			GameObject shipElementObject = Instantiate (EnemyElementPrefab) as GameObject;
			ShipElement shipElement = shipElementObject.GetComponent<ShipElement> ();
			if (Player.Instance.DataBase.CreaturePortraitsByNames.ContainsKey(enemy.Name)) {
				shipElement.PortraitImage.sprite = Player.Instance.DataBase.CreaturePortraitsByNames [enemy.Name];
			}
			shipElement.NameLabel.text = enemy.Name;
			shipElement.LevelLabel.text = enemy.Level.ToString ();

			for (int i = 0; i < 5; i++) {
				shipElement.Stars [i].SetActive (false);
			}

			for (int i = 0; i < enemy.Stars; i++) {
				shipElement.Stars [i].SetActive (true);
			}

			shipElementObject.transform.SetParent (EnemiesElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			EnemyElementObjects.Add (shipElementObject);
		}

		int seconds = mission.Seconds % 60;
		int minutes = (mission.Seconds - seconds) / 60;
		int newMinutes = minutes % 60;
		int hours = (minutes - newMinutes) / 60;
		TimeLabel.text = hours + ":" + newMinutes + ":" + seconds;
	}
	public void Close () {
		Window.SetActive (false);
	}

	public void StartMission () {		
		UIOverlay.Instance.OpenTeamSelectionWindow (mission);
	}

	public void Back () {
		//gameManager.OpenExpeditionWindow(expeditionCenter);
	}
}
