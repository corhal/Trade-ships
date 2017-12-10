using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BJComboSkill : BJSkill {

	public GameObject Projectile;
	public GameObject ProjectilePrefab;
	bool shouldMoveProjectile;
	float speed = 10.0f;
	float initialZ = -7.0f;

	bool shouldMoveSelf;
	int moveCounter;

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {
		base.UseSkill (user, mainTarget);
		moveCounter = 0;
		CurrentUser = user;
		CurrentMainTarget = mainTarget;

		Projectile = Instantiate (ProjectilePrefab) as GameObject;
		shouldMoveProjectile = true;
		Projectile.transform.position = new Vector3 (CurrentUser.transform.position.x, CurrentUser.transform.position.y, initialZ);
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
			if (enemyCreatureObjects.Count > KeysToList[i] && enemyCreatureObjects[KeysToList[i]].Creature.HP <= 0) {
				TargetPriorities.Remove (KeysToList [i]);
			} else if (enemyCreatureObjects.Count > KeysToList[i] && TargetPriorities[KeysToList [i]] < minPriority) {
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
	}

	void Update () {
		if (shouldMoveProjectile) {
			float step = speed * Time.deltaTime;
			Projectile.transform.position = Vector2.MoveTowards (Projectile.transform.position, CurrentMainTarget.transform.position, step);
			Projectile.transform.position = new Vector3 (Projectile.transform.position.x, Projectile.transform.position.y, initialZ);
			if (Vector2.Distance (Projectile.transform.position, CurrentMainTarget.transform.position) < 0.001f) {				
				shouldMoveProjectile = false;
				Destroy (Projectile);
				CurrentUser.DealDamage ((int) (CurrentUser.Creature.BaseDamage / 2), 1.0f, CurrentMainTarget);
				for (int i = 0; i < Effects.Count; i++) {
					if (Random.Range(0.0f, 0.99f) < EffectChances [i]) {
						Effects [i].Applier = CurrentUser;
						CurrentMainTarget.ApplyEffect (Effects [i]);
					}
				}
				float xCoord = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? 0.5f : -0.5f;
				CurrentUser.MoveToPoint (CurrentMainTarget.transform.position - new Vector3(xCoord, 0.0f, 0.0f));
			}
		}
	}

	public override void User_OnCreatureMovementFinished (BJCreatureObject creatureObject) {
		base.User_OnCreatureMovementFinished (creatureObject);
		if (creatureObject.CurrentSkill != this) { // whoops :(
			return;
		}

		moveCounter++;
		if (moveCounter == 1) {
			CurrentUser.DealDamage ((int)CurrentUser.Creature.BaseDamage, 1.0f, CurrentMainTarget);
			CurrentUser.MoveToPoint (CurrentUser.InitialPosition);
		}
		if (moveCounter == 2) {
			StartCoroutine(FinishSkill (0.1f));
		}
	}

	public override string GetInfo () {		
		return "Shoots target for <color=blue>" + CurrentUser.Creature.BaseDamage / 2 + "</color> damage, lowering target's armor by <color=blue>" + Effects [0].Damage +
			"</color>; then deals <color=blue>" + CurrentUser.Creature.BaseDamage + "</color> damage in melee.";
	}
}
