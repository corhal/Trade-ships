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

	new void Awake () {
		base.Awake ();
		Missions = new List<Mission> ();
	}

	new void Start () {
		base.Start ();

		Missions.Add (new Mission ());

		Action showMissionsAction = new Action ("Show missions", 0, ShowMissions);
		actions.Add (showMissionsAction);
	}

	void ShowMissions () {
		gameManager.OpenMissionWindow (this);
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

	void Update () {
		if (currentMission != null && currentMission.InProgress) {
			timer += Time.deltaTime;
			TimeSlider.value = timer;
			if (timer >= currentMission.Seconds) {
				currentMission.InProgress = false;
				if (Random.Range(0, 101) > successChance) {
					Debug.Log ("mission failed!");
					return;
				}
				Debug.Log ("Success!");
				Player.Instance.TakeItems (currentMission.GiveReward ());
			}
		}
	}
}
