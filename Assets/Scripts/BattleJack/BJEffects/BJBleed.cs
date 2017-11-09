using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJBleed : BJEffect {

	public override void Tick () {
		base.Tick ();
		Victim.Creature.TakeDamage (Damage);
	}
}
