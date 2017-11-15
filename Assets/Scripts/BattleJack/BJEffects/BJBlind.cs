using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJBlind : BJEffect {

	public override void Activate () {
		Victim.Creature.Precision -= (float)Damage / 100.0f;
	}

	public override void Deactivate () {
		base.Deactivate ();
		Victim.Creature.Precision += (float)Damage / 100.0f;
	}
}
