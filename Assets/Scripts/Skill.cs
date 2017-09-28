using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill {

	public string Name;
	public int Level;
	public int MaxLevel;
	public List<int> UpgradeCosts;
	public Effect Effect;

	public Skill(string name, int level, int maxLevel, List<int> upgradeCosts, Effect effect) {
		Name = name;
		Level = level;
		MaxLevel = maxLevel;
		UpgradeCosts = upgradeCosts;
		Effect = effect;
	}

	public void Upgrade () {
		if (Level < MaxLevel) {
			Level += 1;
			Effect.Level += 1;
		}
	}
}
