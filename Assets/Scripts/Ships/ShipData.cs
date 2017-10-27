using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipData {	

	public List<RewardChest> RewardChests;
	public bool IsSummoned;
	public List<Skill> Skills;
	public List<Effect> Effects;

	public Item Blueprint;
	public List<List<Item>> PromoteCosts;
	public List<int> LevelRequirements;
	public RankColor RankColor;
	public int Stars;
	public int Level;
	public string Name;
	public string Allegiance;
	public int HP;
	public int MaxHP;
	public int Power;
	public float[] Coordinates;
	public int Exp;
	public List<int> EvolveCosts;
	public List<string> StatNames;

	public float SecPerShot;
	public float AttackRange;
	public float Speed = 1.0f; // change later

	public ShipData () {

	}

	public ShipData (string name, string allegiance, int level, int stars, int maxHP, 
		int hp, int power, float[] coordinates, List<Skill> skills, List<Effect> effects,
		Item blueprint, List<List<Item>> promoteCosts, RankColor rankColor, bool isSummoned, List<int> levelRequirements, List<RewardChest> rewardChests,
		float secPerShot, float attackRange) {
		Name = name;
		Allegiance = allegiance;
		Level = level;
		Stars = stars;
		MaxHP = maxHP;
		HP = hp;
		Power = power;
		Coordinates = new float[coordinates.Length];
		coordinates.CopyTo (Coordinates, 0);
		Blueprint = blueprint;
		Exp = 0;
		EvolveCosts = Player.Instance.DataBase.EvolveCosts;
		StatNames = new List<string> { /*"Cargo",*/ "MaxHP", "Firepower", "Range", "Attack speed", "Speed"};
		SecPerShot = secPerShot;
		AttackRange = attackRange;
		if (promoteCosts != null) {
			PromoteCosts = new List<List<Item>> (promoteCosts);
		} else {
			PromoteCosts = new List<List<Item>> ();
		}
		RankColor = rankColor;
		if (skills != null) {
			Skills = new List<Skill> (skills);
		} else {
			Skills = new List<Skill> ();
		}
		if (effects != null) {
			Effects = new List<Effect> (effects);
		} else {
			Effects = new List<Effect> ();
		}
		if (levelRequirements != null) {
			this.LevelRequirements = new List<int> (levelRequirements);
		} else {
			LevelRequirements = new List<int> ();
		}
		if (rewardChests != null) {
			RewardChests = new List<RewardChest> (rewardChests);
		} else {
			RewardChests = new List<RewardChest> ();
		}
		IsSummoned = isSummoned;
	}

	public int GetStatByString (string statName) {
		switch (statName) {
		case "HP":
			return HP;
		case "MaxHP":
			return MaxHP;
		case "Firepower":
			return Power;
		case "Range":
			return (int)(AttackRange * 1000.0f);
		case "Attack speed":
			return (int)(SecPerShot * 1000.0f);
		case "Speed":
			return (int)(Speed * 1000.0f);
		default:
			return 0;
		}
	}

	public void AddStatByString (string statName, int amount) {
		switch (statName) {
		case "MaxHP":
			MaxHP += amount;
			break;
		case "Firepower":
			Power += amount;
			break;
		case "Range":
			AttackRange += (float)amount / 1000.0f; // munits
			break;
		case "Attack speed":
			SecPerShot += (float)amount / 1000.0f; // msec
			break;
		case "Speed":
			Speed += (float)amount / 1000.0f; // munits
			break;
		default:
			break;
		}
	}

	public void ReduceStatByString (string statName, int amount) {
		AddStatByString (statName, -amount);
	}

	public void ApplyEffect (Effect effect) {
		foreach (var myEffect in Effects) {
			if (effect.Name == myEffect.Name) { // effects don't stack right now
				RemoveEffect (myEffect);
				break;
			}
		}
		Effects.Add (effect.Copy());

		foreach (var statEffect in effect.StatEffects[effect.Level]) {					
			AddStatByString (statEffect.Key, statEffect.Value);					
		}
	}

	public void RemoveEffect (Effect effect) {
		foreach (var statEffect in effect.StatEffects[effect.Level]) {					
			ReduceStatByString (statEffect.Key, statEffect.Value);					
		}
		Effects.Remove (effect);
	}

	public void UpgradeSkill (Skill skill) {		
		if (Skills.Contains(skill)) {			
			if (Player.Instance.Gold >= skill.UpgradeCosts[skill.Level]) {
				Player.Instance.GiveGold (skill.UpgradeCosts [skill.Level]);
				skill.Upgrade ();

				foreach (var effectByTarget in skill.EffectsByTargets) {
					if (effectByTarget.Value != null && effectByTarget.Key == "self" && effectByTarget.Value.Duration == -1.0f) { // kinda sorta determine if skill is passive
						ApplyEffect (effectByTarget.Value);
					}
				}
			} else {
				GameManager.Instance.OpenPopUp ("Not enough gold!");
			}
		}
	}

	public void AddExp (int amount) {
		Exp += amount;
		if (Exp >= LevelRequirements[Level]) {
			LevelUp ();
		}
	}

	public void LevelUp () {
		Level += 1;
	}

	public void PromoteRank () {
		for (int i = 0; i < PromoteCosts[(int)RankColor].Count; i++) {
			Item item = PromoteCosts [(int)RankColor] [i];

			if (!Player.Instance.Inventory.ContainsKey(item) || Player.Instance.Inventory[item] == 0) {
				GameManager.Instance.OpenPopUp ("Not enough items!");
				return;
			}
		}

		foreach (var item in PromoteCosts[(int)RankColor]) {
			Player.Instance.GiveItems (new Dictionary<Item, int> { { item, 1 } });
		}

		RankColor += 1;
	}

	public void EvolveStar () {
		if (!Player.Instance.Inventory.ContainsKey(Blueprint) || Player.Instance.Inventory[Blueprint] < EvolveCosts[Stars]) {
			GameManager.Instance.OpenPopUp ("Not enough blueprints!");
			return;
		}
		Player.Instance.GiveItems (new Dictionary<Item, int> { { Blueprint, EvolveCosts [Stars] } });
		Stars += 1;
	}
}
