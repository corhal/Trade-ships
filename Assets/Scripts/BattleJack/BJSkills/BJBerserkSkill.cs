﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BJBerserkSkill : BJSkill {

	// public BJEffect ArmorBuffEffect;

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {	
		base.UseSkill (user, mainTarget);	
		CurrentUser = user;
		CurrentMainTarget = mainTarget;
			
		Effects [0].Applier = user;
		mainTarget.ApplyEffect (Effects [0], true);

		StartCoroutine (FinishSkill (0.1f));
	}

	public override void AssignSkillIndexes () {	
		List<BJCreatureObject> ourCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
		TargetPriorities = new Dictionary<int, int> { {0, 0}, {1, 0}, {2, 0}, {3, 0}, {4, 0} };
		int minPriority = 10;

		List<int> KeysToList = TargetPriorities.Keys.ToList ();
		for (int i = TargetPriorities.Keys.Count - 1; i >= 0; i--) {			
			if (ourCreatureObjects.Count > KeysToList[i] && ourCreatureObjects[KeysToList[i]].Creature.HP <= 0) {
				TargetPriorities.Remove (KeysToList [i]);
			} else if (ourCreatureObjects.Count > KeysToList[i] && TargetPriorities[KeysToList [i]] < minPriority) {
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

	public override string GetInfo () {
		BJBerserk berserkEffect = Effects [0] as BJBerserk;
		string turnsString = (Effects [0].Duration > 1) ? " turns" : " turn";
		return "For <color=blue>" + Effects [0].Duration + "</color>" + turnsString + " the target will have its armored lowered by <color=blue>" + berserkEffect.Damage + "</color>, but its damage raised by <color=blue>" + 
			berserkEffect.DamageBuff + "</color>.";
	}
}
