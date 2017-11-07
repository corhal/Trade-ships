using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJMeleeAttack : BJSkill {

	int moveCounter;

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {
		moveCounter = 0;
		CurrentUser = user;
		CurrentMainTarget = mainTarget;
		float xCoord = (user.Creature.Allegiance == Allegiance.Player) ? 0.5f : -0.5f;
		user.MoveToPoint (mainTarget.transform.position - new Vector3(xCoord, 0.0f, 0.0f));
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
