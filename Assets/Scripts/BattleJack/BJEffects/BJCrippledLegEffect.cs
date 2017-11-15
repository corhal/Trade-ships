using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJCrippledLegEffect : BJEffect {


	public override void Activate () {
		Victim.Creature.Speed -= Damage;
		Victim.MoveSpeed /= 4;
	}

	public override void Deactivate () {
		base.Deactivate ();
		Victim.Creature.Speed += Damage;
		Victim.MoveSpeed *= 4;
	}
}
