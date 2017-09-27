using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThievesGuild : Building {

	protected override void Start () {
		base.Start ();
		Action showMissionsAction = new Action ("Show missions", 0, gameManager.ActionIconsByNames["Show missions"], ShowMissions);
		actions.Add (showMissionsAction);

	}

	void ShowMissions () {
		gameManager.OpenThievesWindow (this);
	}
}
