﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJSacrificeEffect : BJEffect {

	public override void Activate () {
		List<BJCreatureObject> ourCreatureObjects = (Applier.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
		int userIndex = ourCreatureObjects.IndexOf (Applier);	
		List<BJCreatureObject> secondaryTargets = new List<BJCreatureObject> ();

		foreach (var creatureObject in ourCreatureObjects) {
			if (creatureObject != Applier && creatureObject.Creature.HP > 0) {
				creatureObject.Creature.OnCreatureDied += CreatureObject_Creature_OnCreatureDied;
			}
		}
	}

	void CreatureObject_Creature_OnCreatureDied (BJCreature sender) {
		Victim.Creature.Speed += Damage;
		// BJGameController.Instance.ReformQueue ();
	}

	public override void Deactivate () {
		base.Deactivate ();
	}
}
