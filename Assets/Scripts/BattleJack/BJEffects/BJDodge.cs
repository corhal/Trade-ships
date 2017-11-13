using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJDodge : BJEffect {

	public override void Activate () {
		Victim.Creature.Dodge += (float)Damage * 0.01f;
	}

	public override void Deactivate () {
		base.Deactivate ();
		Victim.Creature.Dodge -= (float)Damage * 0.01f;
	}
}
