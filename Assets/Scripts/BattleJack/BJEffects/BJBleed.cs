using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJBleed : BJEffect {

	public override void Tick () {
		base.Tick ();
		Debug.Log ("Should take " + Damage + " damage " + " with armor piercing of " + Victim.Creature.Armor);
		Victim.Creature.TakeDamage (Damage, Victim.Creature.Armor);
	}
}
