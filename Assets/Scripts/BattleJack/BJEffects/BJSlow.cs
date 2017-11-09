using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJSlow : BJEffect {

	Color victimColor;

	public override void Activate () {
		Victim.Creature.Speed -= Damage;
		Victim.MoveSpeed /= 4;
		victimColor = Victim.InitialColor;
		Victim.InitialColor = Color.blue;
		Victim.CreatureImage.color = Color.blue;
	}

	public override void Deactivate () {
		base.Deactivate ();
		Victim.Creature.Speed += Damage;
		Victim.MoveSpeed *= 4;
		Victim.InitialColor = victimColor;
		Victim.CreatureImage.color = Victim.InitialColor;
	}
}
