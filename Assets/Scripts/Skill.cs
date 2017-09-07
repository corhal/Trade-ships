using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill {

	public string Name;
	public int Level;
	public int MaxLevel;
	public List<int> UpgradeCosts;
	public List<string> AffectedStats;
	public List<Dictionary<string, int>> StatEffects;

	public Skill(string name, int level, int maxLevel, List<int> upgradeCosts, List<string> affectedStats, List<Dictionary<string, int>> statEffects) {
		Name = name;
		Level = level;
		MaxLevel = maxLevel;
		UpgradeCosts = upgradeCosts;
		AffectedStats = affectedStats;
		StatEffects = statEffects;
	}

	public void Upgrade () {
		if (Level < MaxLevel) {
			Level += 1;
		}
	}
}
