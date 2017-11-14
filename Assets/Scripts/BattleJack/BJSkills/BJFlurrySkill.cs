using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BJFlurrySkill : BJSkill {

	public int HitsAmount;
	bool shouldHit;
	int moveCounter;

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
			TargetPriorities = new Dictionary<int, int> { {0, 0}, {1, 0}, {2, 1}, {3, 2}, {4, 2} };
			break;
		case 1:
			TargetPriorities = new Dictionary<int, int> { {0, 0}, {1, 0}, {2, 0}, {3, 1}, {4, 1} };
			break;
		case 2:
			TargetPriorities = new Dictionary<int, int> { {0, 1}, {1, 0}, {2, 0}, {3, 2}, {4, 2} };
			break;
		case 3:
		case 4:
		case 5:
			TargetPriorities = new Dictionary<int, int> { {0, 0}, {1, 0}, {2, 0}, {3, 1}, {4, 1} };
			break;
		default:
			break;
		}

		int minPriority = 10;
		List<int> KeysToList = TargetPriorities.Keys.ToList ();
		for (int i = TargetPriorities.Keys.Count - 1; i >= 0; i--) {			
			if (enemyCreatureObjects[KeysToList[i]].Creature.HP <= 0) {
				TargetPriorities.Remove (KeysToList [i]);
			} else if (TargetPriorities[KeysToList [i]] < minPriority) {
				minPriority = TargetPriorities [KeysToList [i]];
			}
		}
		KeysToList = TargetPriorities.Keys.ToList ();
		for (int i = TargetPriorities.Keys.Count - 1; i >= 0; i--) {
			if (TargetPriorities[KeysToList[i]] > minPriority) {
				TargetPriorities.Remove (KeysToList [i]);
			}
		}

		ValidTargetIndexes = new List<int> (TargetPriorities.Keys);

		/*var myList = TargetPriorities.ToList();
		myList.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));
		ValidTargetIndexes = new List<int> ();
		foreach (var keyValPair in myList) {
			ValidTargetIndexes.Add (keyValPair.Key); // AI will just attack every creature by order for now
		}*/
	}

	public override void User_OnCreatureMovementFinished (BJCreatureObject creatureObject) {
		base.User_OnCreatureMovementFinished (creatureObject);
		if (creatureObject.CurrentSkill != this) { // whoops :(
			return;
		}
		if (shouldHit) {
			moveCounter++;
		}
		if (moveCounter < HitsAmount) {
			float xCoord = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? 0.5f : -0.5f;
			if (shouldHit) {
				CurrentUser.DealDamage (CurrentUser.Creature.BaseDamage, 1.0f, CurrentMainTarget);
				CurrentUser.MoveToPoint (CurrentMainTarget.transform.position - new Vector3(xCoord, 0.0f, 0.0f));
				shouldHit = false;
			} else {
				CurrentUser.MoveToPoint (CurrentMainTarget.transform.position - new Vector3(xCoord * 2, 0.0f, 0.0f));
				shouldHit = true;
			}
		}
		if (moveCounter == HitsAmount) {
			CurrentUser.DealDamage (CurrentUser.Creature.BaseDamage, 1.0f, CurrentMainTarget);
			CurrentUser.MoveToPoint (CurrentUser.InitialPosition);
		}
		if (moveCounter > HitsAmount) {
			StartCoroutine(FinishSkill (0.1f));
		}
	}
}
