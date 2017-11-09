using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RangeType {
	Melee, Ranged
}

public class BJSkill : MonoBehaviour {

	public int Damage;

	public delegate void FinishSkillEventHandler (BJSkill sender);
	public event FinishSkillEventHandler OnSkillFinished;

	public BJCreatureObject CurrentUser; // :(((
	public BJCreatureObject CurrentMainTarget;

	public RangeType RangeType;
	public List<int> ValidTargetIndexes;

	public virtual void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {

	}

	public virtual void User_OnCreatureMovementFinished (BJCreatureObject creatureObject) {

	}

	protected IEnumerator FinishSkill (float delay) {
		yield return new WaitForSeconds (delay);
		OnSkillFinished (this);
	}

	public virtual void AssignSkillIndexes () {

	}
}
