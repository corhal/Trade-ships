using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJBerserk : BJEffect {
	public int DamageBuff;

	public override void Activate () {
		Victim.Creature.Armor -= Damage;
		Victim.Creature.BaseDamage += DamageBuff;
	}

	public override void Deactivate () {
		base.Deactivate ();
		Victim.Creature.Armor += Damage;
		Victim.Creature.BaseDamage -= DamageBuff;
	}
}
