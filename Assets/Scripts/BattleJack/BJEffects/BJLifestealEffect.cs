using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJLifeStealEffect : BJEffect {
	public float LifeStealRate;

	public override void Activate () {
		
	}

	void Creature_OnDamageTaken (int amount) {
		Victim.Creature.Heal ((int)(amount * LifeStealRate));
	}

	public override void Deactivate () {
		
	}
}
