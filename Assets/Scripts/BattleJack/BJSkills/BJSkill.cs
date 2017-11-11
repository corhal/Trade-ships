using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJSkill : MonoBehaviour {

	public Sprite SkillIcon;
	public int Damage;

	public int Cooldown;
	public int CurrentCooldown;

	public delegate void FinishSkillEventHandler (BJSkill sender);
	public event FinishSkillEventHandler OnSkillFinished;

	public BJCreatureObject CurrentUser; // :(((
	public BJCreatureObject CurrentMainTarget;

	public Dictionary<int, int> TargetPriorities;
	public List<int> ValidTargetIndexes;

	public virtual void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {

	}

	public virtual void User_OnCreatureMovementFinished (BJCreatureObject creatureObject) {		
		if (creatureObject.CurrentSkill != this) { // whoops :(
			return;
		}
	}

	protected IEnumerator FinishSkill (float delay) {
		yield return new WaitForSeconds (delay);
		OnSkillFinished (this);
	}

	public virtual void AssignSkillIndexes () {

	}
}
