using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill {

	public string Name;
	public int Level;
	public int MaxLevel;
	public List<int> UpgradeCosts;

	public Skill(string name, int level, int maxLevel, List<int> upgradeCosts) {
		Name = name;
		Level = level;
		MaxLevel = maxLevel;
		UpgradeCosts = upgradeCosts;
	}

	public void Upgrade () {
		if (Level < MaxLevel) {
			Level += 1;
		}
	}
}
