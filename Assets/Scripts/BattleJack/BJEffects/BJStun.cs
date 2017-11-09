using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJStun : BJEffect {

	public override void Tick () {
		base.Tick ();
		Victim.IsStunned = true;
		Debug.Log (Victim + " is stunned!");
	}
}
