using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BJAggroSkill : BJSkill {

	public BJEffect AggroEffect;

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {		
		CurrentUser = user;
		CurrentMainTarget = mainTarget;	
		mainTarget.ApplyEffect (AggroEffect);
		StartCoroutine (FinishSkill (0.1f));
	}

	public override void AssignSkillIndexes () {	
		List<BJCreatureObject> ourCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
		int userIndex = ourCreatureObjects.IndexOf (CurrentUser);	
		ValidTargetIndexes = new List<int> { userIndex };
	}
}
