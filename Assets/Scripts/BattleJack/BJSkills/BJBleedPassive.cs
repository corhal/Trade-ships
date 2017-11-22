using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJBleedPassive : BJSkill {
	// public BJEffect DodgeBuffEffect;

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {
		base.UseSkill (user, mainTarget);
		CurrentUser = user;
		CurrentMainTarget = mainTarget;

		Effects [0].Applier = user;
		mainTarget.ApplyEffect (Effects[0]);

		StartCoroutine (FinishSkill (0.1f));
	}

	public override void AssignSkillIndexes () {	
		List<BJCreatureObject> ourCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
		int userIndex = ourCreatureObjects.IndexOf (CurrentUser);	
		ValidTargetIndexes = new List<int> { userIndex };
	}

	public override string GetInfo () {		
		string turnsString = (Effects [0].Duration > 1) ? " turns" : " turn";
		return "Deals <color=blue>" + CurrentUser.Creature.BaseDamage + "</color> damage; For <color=blue>" + Effects [0].Duration + "</color>" +
			turnsString + " the target will receive <color=blue>" + Effects [0].Damage + "</color> per turn.";
	}
}
