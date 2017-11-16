using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJLifestealEffect : BJEffect {
	public float LifeStealRate;

	public override void Activate () {
		
	}

	public void Creature_OnDamageTaken (int amount) {
		if (BJGameController.Instance.CurrentCreatureObject == Victim) {
			Victim.Creature.Heal ((int)(amount * LifeStealRate));
		}
	}

	public override void Deactivate () {
		
	}
}
