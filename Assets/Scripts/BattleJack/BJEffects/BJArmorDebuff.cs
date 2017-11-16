using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJArmorDebuff : BJEffect {

	public override void Activate () {
		Victim.Creature.Armor -= Damage;
	}

	public override void Deactivate () {
		base.Deactivate ();
		Victim.Creature.Armor += Damage;
	}
}
