using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BJAggroSkill : BJSkill {

	// public BJEffect AggroEffect;

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
		string endString = (Effects [0].Duration > 1) ? " turns" : " turn";
		return "Makes all enemies attack only this hero for <color=blue>" + Effects[0].Duration + "</color>" + endString;
	}
}
