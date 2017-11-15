﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BJMeleeSkill : BJSkill {

	// public BJEffect Effect;

	int moveCounter;

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {
		base.UseSkill (user, mainTarget);
		moveCounter = 0;
		CurrentUser = user;
		CurrentMainTarget = mainTarget;
		float xCoord = (user.Creature.Allegiance == Allegiance.Player) ? 0.5f : -0.5f;
		user.MoveToPoint (mainTarget.transform.position - new Vector3(xCoord, 0.0f, 0.0f));
	}

	public override void AssignSkillIndexes () {		
		List<BJCreatureObject> ourCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
		List<BJCreatureObject> enemyCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.EnemyCreatureObjects : BJGameController.Instance.PlayerCreatureObjects;
		int userIndex = ourCreatureObjects.IndexOf (CurrentUser);

		switch (userIndex) { // кошмарный говнокод
		case 0:
			TargetPriorities = new Dictionary<int, int> { {0, 0}, {1, 1}, {2, 2}, {3, 3}, {4, 4} };
			// ValidTargetIndexes = new List<int> { 0, 1 };
			break;
		case 1:
			TargetPriorities = new Dictionary<int, int> { {0, 1}, {1, 0}, {2, 1}, {3, 2}, {4, 2} };
			// ValidTargetIndexes = new List<int> { 0, 1, 2 };
			break;
		case 2:
			TargetPriorities = new Dictionary<int, int> { {0, 2}, {1, 1}, {2, 0}, {3, 4}, {4, 3} };
			// ValidTargetIndexes = new List<int> { 1, 2 };
			break;
		case 3:
		case 4:
		case 5:
			TargetPriorities = new Dictionary<int, int> { {0, 0}, {1, 0}, {2, 0}, {3, 1}, {4, 1} };
			// ValidTargetIndexes = new List<int> { 0, 1, 2};
			break;
		default:
			// ValidTargetIndexes = new List<int> { 0, 1, 2, 3, 4};
			break;
		}

		int minPriority = 10;
		List<int> KeysToList = TargetPriorities.Keys.ToList ();
		for (int i = TargetPriorities.Keys.Count - 1; i >= 0; i--) {
			if (enemyCreatureObjects[KeysToList[i]].Creature.HP <= 0) {
				TargetPriorities.Remove (KeysToList [i]);
			} else if (TargetPriorities[KeysToList [i]] < minPriority) {
				minPriority = TargetPriorities [KeysToList [i]];
			}
		}
		KeysToList = TargetPriorities.Keys.ToList ();
		for (int i = TargetPriorities.Keys.Count - 1; i >= 0; i--) {
			if (TargetPriorities[KeysToList[i]] > minPriority) {
				TargetPriorities.Remove (KeysToList [i]);
			}
		}

		ValidTargetIndexes = new List<int> (TargetPriorities.Keys);
			
	}

	public override void User_OnCreatureMovementFinished (BJCreatureObject creatureObject) {
		base.User_OnCreatureMovementFinished (creatureObject);
		if (creatureObject.CurrentSkill != this) { // whoops :(
			return;
		}
		moveCounter++;
		if (moveCounter == 1) {
			CurrentUser.DealDamage (Damage, 1.0f, CurrentMainTarget);
			for (int i = 0; i < Effects.Count; i++) {
				if (Random.Range(0.0f, 0.99f) < EffectChances [i]) {
					Effects [i].Applier = CurrentUser;
					CurrentMainTarget.ApplyEffect (Effects [i]);
				}
			}
			CurrentUser.MoveToPoint (CurrentUser.InitialPosition);
		}
		if (moveCounter == 2) {
			StartCoroutine(FinishSkill (0.1f));
		}
	}
}
