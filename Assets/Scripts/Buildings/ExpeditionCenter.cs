using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionCenter : Building {

	public List<Mission> Missions;

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
		gameManager.OpenMissionWindow (Missions[0]);
	}
}
