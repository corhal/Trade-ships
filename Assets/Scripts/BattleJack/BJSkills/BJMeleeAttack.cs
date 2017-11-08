using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJMeleeAttack : BJSkill {

	int moveCounter;
	void Awake () {
		RangeType = RangeType.Melee;
	}

	void Start () {
		
	}

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {
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
			ValidTargetIndexes = new List<int> { 0, 1 };
			break;
		case 1:
			ValidTargetIndexes = new List<int> { 0, 1, 2 };
			break;
		case 2:
			ValidTargetIndexes = new List<int> { 1, 2 };
			break;
		case 3:
		case 4:
		case 5:
			ValidTargetIndexes = new List<int> { 0, 1, 2};
			break;
		default:
			ValidTargetIndexes = new List<int> { 0, 1, 2, 3, 4};
			break;
		}

		bool allDead = true;
		foreach (var validTargetIndex in ValidTargetIndexes) {
			if (enemyCreatureObjects.Count > validTargetIndex && enemyCreatureObjects[validTargetIndex].Creature.HP > 0) {
				allDead = false;
			}
		}
		if (allDead) {
			switch (userIndex) {
			case 0:
				ValidTargetIndexes = new List<int> { 2 };
				break;
			case 1:
				ValidTargetIndexes = new List<int> { 0, 1, 2 };
				break;
			case 2:
				ValidTargetIndexes = new List<int> { 0 };
				break;
			case 3:
			case 4:
			case 5:
				ValidTargetIndexes = new List<int> { 0, 1, 2};
				break;
			default:
				ValidTargetIndexes = new List<int> { 0, 1, 2, 3, 4};
				break;
			}
		}

		allDead = true;
		foreach (var validTargetIndex in ValidTargetIndexes) {
			if (enemyCreatureObjects.Count > validTargetIndex && enemyCreatureObjects[validTargetIndex].Creature.HP > 0) {
				allDead = false;
			}
		}

		if (allDead) {
			switch (userIndex) {
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				ValidTargetIndexes = new List<int> { 3, 4};
				break;
			default:
				ValidTargetIndexes = new List<int> { 0, 1, 2, 3, 4};
				break;
			}
		}
	}

	public override void User_OnCreatureMovementFinished (BJCreatureObject creatureObject) {
		moveCounter++;
		if (moveCounter == 1) {
			StartCoroutine(CurrentUser.DealDamage (0.0f, CurrentMainTarget));
			CurrentUser.MoveToPoint (CurrentUser.InitialPosition);
		}
		if (moveCounter == 2) {
			FinishSkill ();
		}
	}
}
