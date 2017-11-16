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
		Debug.Log ("Should take delayed damage of " + DelayedDamage + " with multiplier of " + Multiplier.ToString ("0.00"));
		Victim.Creature.TakeDamage ((int)(DelayedDamage * Multiplier), Victim.Creature.Armor);
	}
}
