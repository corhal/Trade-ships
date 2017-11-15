using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJRegen : BJEffect {

	public override void Activate () {
		
	}

	public override void Tick () {
		Victim.Creature.Heal (Damage);
	}

	public override void Deactivate () {
		base.Deactivate ();

	}
}
