using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJStun : BJEffect {

	public override void Activate () {
		base.Activate ();
		Victim.IsStunned = true;
	}

	public override void Deactivate () {
		base.Deactivate ();
		Victim.IsStunned = false;
	}
}
