using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJLifestealPassive : BJSkill {
	// public BJEffect DodgeBuffEffect;

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {	
		base.UseSkill (user, mainTarget);	
		CurrentUser = user;
		CurrentMainTarget = mainTarget;

		Effects [0].Applier = user;
		user.ApplyEffect (Effects [0]);
		List<BJCreatureObject> enemyCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.EnemyCreatureObjects : BJGameController.Instance.PlayerCreatureObjects;
		foreach (var enemyCreatureObject in enemyCreatureObjects) {
			enemyCreatureObject.Creature.OnDamageTaken += (user.Effects [0] as BJLifestealEffect).Creature_OnDamageTaken;
		}

		StartCoroutine (FinishSkill (0.1f));
	}

	public override void AssignSkillIndexes () {	
		List<BJCreatureObject> ourCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
		int userIndex = ourCreatureObjects.IndexOf (CurrentUser);	
		ValidTargetIndexes = new List<int> { userIndex };
	}

}
