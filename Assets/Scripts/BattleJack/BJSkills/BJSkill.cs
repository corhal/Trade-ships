using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJSkill : MonoBehaviour {

	public delegate void FinishSkillEventHandler (BJSkill sender);
	public event FinishSkillEventHandler OnSkillFinished;

	public BJCreatureObject CurrentUser; // :(((
	public BJCreatureObject CurrentMainTarget;

	public virtual void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {

	}

	public virtual void User_OnCreatureMovementFinished (BJCreatureObject creatureObject) {

	}

	protected virtual void FinishSkill () {
		OnSkillFinished (this);
	}
}
