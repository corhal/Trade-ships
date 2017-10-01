using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill {

	public GameObject ProjectilePrefab;

	//public string Target; // "self", "allies", "enemy", "enemies", "coordinatesEnemy", "coordinatesAlly"
	public string Name;
	public int Level;
	public int MaxLevel;
	public List<int> UpgradeCosts;
	public Dictionary<string, Effect> EffectsByTargets;
	public RankColor RankColorReuirement;

	public Skill(string name, int level, int maxLevel, RankColor rankColorRequirement, List<int> upgradeCosts, Dictionary<string, Effect> effectsByTargets) {
		Name = name;
		Level = level;
		MaxLevel = maxLevel;
		UpgradeCosts = upgradeCosts;
		EffectsByTargets = effectsByTargets;
		RankColorReuirement = rankColorRequirement;
	}

	public void Upgrade () {
		if (Level < MaxLevel) {
			Level += 1;
			if (EffectsByTargets == null) {
				return;
			}
			foreach (var effectByTarget in EffectsByTargets) {
				effectByTarget.Value.Level += 1;
			}
		}
	}

	// Apply effect directly
	// Apply damage directly
	// Shoot projectile (damage, effect)

	public void Use (Ship user) { // write shit now, refactor later
		foreach (var effectByTarget in EffectsByTargets) {
			if (effectByTarget.Key == "self") {
				user.ApplyEffect (effectByTarget.Value);
			}
		}
		if (ProjectilePrefab != null) {
			
		}
		else {
			foreach (var effectByTarget in EffectsByTargets) {
				if (effectByTarget.Key == "enemy" && user.GetComponent<BattleShip>().Enemy != null) {
					user.GetComponent<BattleShip>().Enemy.gameObject.GetComponent<Ship>().ApplyEffect (effectByTarget.Value);
				}
			}
		}
	}
}
