using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJLifestealEffect : BJEffect {
	public float LifeStealRate;

	public override void Activate () {
		List<BJCreatureObject> enemyCreatureObjects = (Victim.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.EnemyCreatureObjects : BJGameController.Instance.PlayerCreatureObjects;
		foreach (var enemyCreatureObject in enemyCreatureObjects) {
			enemyCreatureObject.Creature.OnDamageTaken += Creature_OnDamageTaken;
		}
	}

	public void Creature_OnDamageTaken (int amount) {
		if (BJGameController.Instance.CurrentCreatureObject == Victim) {
			Victim.Creature.Heal ((int)(amount * LifeStealRate));
		}
	}

	public override void Deactivate () {
		
	}
}
