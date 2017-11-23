using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJAdjustmentFirePassiveBuff : BJSkill {
	// public BJEffect DodgeBuffEffect;

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {	
		base.UseSkill (user, mainTarget);	
		CurrentUser = user;
		CurrentMainTarget = mainTarget;

		for (int i = 0; i < user.Skills.Count; i++) {
			user.Skills[i].Effects.Add (Effects [0]);
			user.Skills [i].EffectChances.Add (1.0f);
		}

		StartCoroutine (FinishSkill (0.1f));
	}

	public override void AssignSkillIndexes () {	
		List<BJCreatureObject> ourCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
		int userIndex = ourCreatureObjects.IndexOf (CurrentUser);	
		ValidTargetIndexes = new List<int> { userIndex };
	}

	public override string GetInfo () {		
		return "Deals <color=blue>" + Effects [0].Damage + "</color> bonus damage to the target if attacked it on previous turn.";
	}
}
