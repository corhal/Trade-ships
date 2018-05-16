﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJDelayDamageBuff : BJSkill {
	// public BJEffect DodgeBuffEffect;

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
		int userIndex = ourCreatureObjects.IndexOf (CurrentUser);	
		ValidTargetIndexes = new List<int> { userIndex };
	}

	public override string GetInfo () {		
		string turnsString = (Effects [0].Duration > 1) ? " turns" : " turn";
		return "Delays all damage for <color=blue>" + Effects [0].Duration + "</color>" + turnsString + ". In the end of next turn, all the damage is dealt at once.";
	}
}
