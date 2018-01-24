using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJPassiveSkill : BJSkill {

	public override void UseSkill (BJCreatureObject user, BJCreatureObject mainTarget) {	
		base.UseSkill (user, mainTarget);	
		CurrentUser = user;
		CurrentMainTarget = mainTarget;

		Effects [0].Applier = user;
		mainTarget.ApplyEffect (Effects [0]);

		StartCoroutine (FinishSkill (0.1f));
	}

	public override void AssignSkillIndexes () {	
		List<BJCreatureObject> ourCreatureObjects = (CurrentUser.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
		int userIndex = ourCreatureObjects.IndexOf (CurrentUser);	
		ValidTargetIndexes = new List<int> { userIndex };
	}

	// dodge
	public override string GetInfo () {		
		return "Raises dodge by <color=blue>" + Effects [0].Damage + "%</color> passively.";
	}

	// bleedbuff
	/*public override string GetInfo () {		
		string turnsString = (Effects [0].Duration > 1) ? " turns" : " turn";
		BJBleed bleedEffect = (Effects [0] as BJBleedBuff).Bleed as BJBleed;
		return "Passive. With <color=blue>" + (int)((Effects [0] as BJBleedBuff).EffectChance * 100) + "% </color> chance applies Bleed; For <color=blue>" + bleedEffect.Duration + "</color>" +
			turnsString + " it deals <color=blue>" + bleedEffect.Damage + "</color> damage per turn. Ignores armor.";
	}*/

	// sacrifice
	/*public override string GetInfo () {				
		return "Whenever an ally dies, raises speed by <color=blue>" + Effects [0].Damage + "</color>.";
	}*/

	// adjustment fire
    /*public override string GetInfo () {		
	    return "Deals <color=blue>" + Effects [0].Damage + "</color> bonus damage to the target if attacked it on previous turn.";
    }*/

	// lifesteal buff
	/* public override string GetInfo () {		
		return "Passive. Heals <color=blue>" + (int)((CurrentUser.Effects [0] as BJLifestealEffect).LifeStealRate * 100) + "%</color> of damage dealt.";
	}*/

	// armor buff
	/*
	  :(
	*/

	// corrosion buff
	/*
	  :(
	*/

	// bloodlust buff
	/*
	  :(
	*/
}

