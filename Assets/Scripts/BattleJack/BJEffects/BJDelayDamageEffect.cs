using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJDelayDamageEffect : BJEffect {
	public int DelayedDamage;
	public float Multiplier;

	public override void Activate () {
		
	}

	public override void Deactivate () {
		base.Deactivate ();
		Victim.Creature.TakeDamage ((int)(DelayedDamage * Multiplier), Victim.Creature.Armor);
	}
}
