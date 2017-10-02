using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionCenter : Building {

	public Slider TimeSlider;
	public List<Mission> Missions;
	Mission currentMission;
	float timer;
	int successChance;

	protected override void Awake () {
		base.Awake ();
		Missions = new List<Mission> ();
	}

	protected override void Start () {
		base.Start ();

		List<ShipData> enemyShips = new List<ShipData> ();
		int enemiesCount = Random.Range (1, 6);
		int maxHp = Random.Range (150, 200);
		float[] coordinates = new float[3];
		coordinates [0] = 0.0f;
		coordinates [1] = 0.0f;
		coordinates [2] = 0.0f;
		for (int i = 0; i < enemiesCount; i++) {
			ShipData enemy = new ShipData("Dragon", "Enemy", Random.Range(1, 4), Random.Range(1, 6), Random.Range(1, 10), 
				maxHp, maxHp, Random.Range(10, 20), coordinates, null, null, null);
			enemyShips.Add (enemy);
		}

		for (int i = 0; i < 5; i++) {
			int costLength = Random.Range (1, 6);
			Dictionary<Item, float> rewardChances = new Dictionary<Item, float> ();
			Dictionary<Item, int> possibleRewards = new Dictionary<Item, int> ();
			for (int j = 0; j < costLength; j++) {
				List<Item> validItems = new List<Item> ();
				foreach (var item in gameManager.TempItemLibrary) {
					if (!possibleRewards.ContainsKey(item)) {
						validItems.Add (item);
					}
				}

				int index = Random.Range (0, validItems.Count);
				possibleRewards.Add (validItems [index], Random.Range(1, 6));
				rewardChances.Add (validItems [index], Random.Range (0.3f, 0.7f));
			}
			Missions.Add (new Mission (rewardChances, possibleRewards, enemyShips));
		}

		Action showMissionsAction = new Action ("Show missions", 0, gameManager.ActionIconsByNames["Show missions"], ShowMissions);
		actions.Add (showMissionsAction);
	}

	void ShowMissions () {
		gameManager.OpenExpeditionWindow (this);
	}

	public void StartMission (Mission mission, int successChance) {
		currentMission = mission;
		this.successChance = successChance;
		currentMission.InProgress = true;
		timer = 0.0f;
		TimeSlider.gameObject.SetActive (true);
		TimeSlider.value = timer;
		TimeSlider.maxValue = currentMission.Seconds;
	}

	protected override void Update () {
		base.Update ();
		if (currentMission != null && currentMission.InProgress) {
			timer += Time.deltaTime;
			TimeSlider.value = timer;
			if (timer >= currentMission.Seconds) {
				currentMission.InProgress = false;
				if (Random.Range(0, 101) > successChance) {
					gameManager.OpenPopUp ("Mission failed!");
					return;
				}
				gameManager.OpenPopUp ("Mission success!");
				TimeSlider.value = 0;
				TimeSlider.gameObject.SetActive (false);
				Player.Instance.TakeItems (currentMission.GiveReward ());
			}
		}
	}
}
