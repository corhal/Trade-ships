using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill {
	public BJSkill BJSkill;

	public string Name;
	public int Level;
	public int MaxLevel;
	public List<int> UpgradeCosts;

	public Skill(BJSkill bjskill, int level, int maxLevel, List<int> upgradeCosts) {
		BJSkill = bjskill;
		Name = bjskill.Name;
		Level = level;
		MaxLevel = maxLevel;
		UpgradeCosts = upgradeCosts;
	}

	// TODO: Make upgrades meaningful
	public void Upgrade () {
		if (Level < MaxLevel) {
			Level += 1;
		}
	}
}
