using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fort : Building {


	protected override void Awake () {
		base.Awake ();
	}

	protected override void Start () {
		base.Start ();

		Action showShips = new Action ("Show ships", 0, player.DataBase.ActionIconsByNames["Show missions"], ShowShips);
		actions.Add (showShips);
	}

	void ShowShips () {
		//gameManager.OpenFortWindow ();
	}
}
