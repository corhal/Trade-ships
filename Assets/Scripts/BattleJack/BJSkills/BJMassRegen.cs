using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BJMassRegen : BJSkill {

	// public BJEffect ArmorBuffEffect;

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {	
		base.UseSkill (user, mainTarget);	
		CurrentUser = user;
		CurrentMainTarget = mainTarget;

		List<BJCreatureObject> ourCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
		int userIndex = ourCreatureObjects.IndexOf (CurrentUser);	
		List<BJCreatureObject> secondaryTargets = new List<BJCreatureObject> ();

		foreach (var creatureObject in ourCreatureObjects) {
			if (creatureObject != user && TargetPriorities.ContainsKey(ourCreatureObjects.IndexOf(creatureObject)) && creatureObject.Creature.HP > 0) {
				secondaryTargets.Add (creatureObject);
			}
		}
		Effects [0].Applier = user;
		mainTarget.ApplyEffect (Effects [0]);
		foreach (var secondaryTarget in secondaryTargets) {
			Effects [0].Applier = user;
			secondaryTarget.ApplyEffect (Effects [0]);
		}
		StartCoroutine (FinishSkill (0.1f));
	}

	public override void AssignSkillIndexes () {	
		List<BJCreatureObject> ourCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
		int userIndex = ourCreatureObjects.IndexOf (CurrentUser);	
		TargetPriorities = new Dictionary<int, int> { {0, 0}, {1, 0}, {2, 0}, {3, 0}, {4, 0} };
		ValidTargetIndexes = new List<int> { userIndex };
	}

	public override string GetInfo () {		
		string turnsString = (Effects [0].Duration > 1) ? " turns" : " turn";
		return "For <color=blue>" + Effects [0].Duration + "</color>" + turnsString + " heals <color=blue>" + Effects [0].Damage + "</color> HP for each team member every turn.";
	}
}
