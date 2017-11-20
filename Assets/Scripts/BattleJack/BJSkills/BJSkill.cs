using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Teams {
	MyTeam, AnotherTeam
}

public class BJSkill : MonoBehaviour {

	public List<BJEffect> Effects;
	public List<float> EffectChances;
	public bool IsPassive = false;
	public Teams TargetTeam;
	// public Allegiance TargetAllegiance;
	public Sprite SkillIcon;
	public int Damage;

	public string Name;
	public int ManaCost;
	public int Cooldown;
	public int CurrentCooldown;

	public delegate void FinishSkillEventHandler (BJSkill sender);
	public event FinishSkillEventHandler OnSkillFinished;

	public BJCreatureObject CurrentUser; // :(((
	public BJCreatureObject CurrentMainTarget;
	public List<BJCreatureObject> CurrentSecondaryTargets;

	public Dictionary<int, int> TargetPriorities;
	public List<int> ValidTargetIndexes;

	public virtual void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {
		CurrentCooldown = Cooldown;
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
	public virtual string GetInfo () {
		return "Skill info";
	}
}
