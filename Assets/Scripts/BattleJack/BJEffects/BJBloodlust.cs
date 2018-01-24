using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJBloodlust : BJEffect {

	public override void Activate () {
		List<BJCreatureObject> enemyCreatureObjects = (Applier.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.EnemyCreatureObjects : BJGameController.Instance.PlayerCreatureObjects;

		foreach (var creatureObject in enemyCreatureObjects) {
			if (creatureObject != Applier && creatureObject.Creature.HP > 0) {
				creatureObject.Creature.OnCreatureDied += CreatureObject_Creature_OnCreatureDied;
			}
		}
	}

	void CreatureObject_Creature_OnCreatureDied (BJCreature sender) {
		if (Victim.CurrentSkill.CurrentMainTarget.Creature == sender && BJGameController.Instance.CurrentCreatureObject == Victim) {
			int tempSpeed = Victim.Creature.Speed;
			Victim.Creature.Speed = 100;
			foreach (var skill in Victim.Skills) {
				skill.CurrentCooldown = 0;
			}
			BJGameController.Instance.ReformQueue (Victim);
			Victim.Creature.Speed = tempSpeed;
		}
	}

	public override void Deactivate () {
		base.Deactivate ();
	}
}
