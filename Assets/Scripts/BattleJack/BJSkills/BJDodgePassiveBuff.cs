using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJDodgePassiveBuff : BJSkill {
	public BJEffect DodgeBuffEffect;

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {		
		CurrentUser = user;
		CurrentMainTarget = mainTarget;

		mainTarget.ApplyEffect (DodgeBuffEffect);

		StartCoroutine (FinishSkill (0.1f));
	}

	public override void AssignSkillIndexes () {	
		List<BJCreatureObject> ourCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
		int userIndex = ourCreatureObjects.IndexOf (CurrentUser);	
		ValidTargetIndexes = new List<int> { userIndex };
	}

}
