using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipData {	

	public List<RewardChest> RewardChests;
	public bool IsSummoned;
	public List<Skill> Skills;
	// public List<Effect> Effects;

	public Item Soulstone;
	public List<List<string>> PromoteCosts;
	public List<int> LevelRequirements;
	public RankColor RankColor;
	public int Stars;
	public int Level;
	public string Name;
	public Allegiance Allegiance;
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

	public BJCreature Creature;

	public ShipData () {

	}

	public ShipData (BJCreature creature, int level, int stars, List<Skill> skills,
		Item soulstone, List<List<string>> promoteCosts, RankColor rankColor, bool isSummoned, List<int> levelRequirements, List<RewardChest> rewardChests,
		float secPerShot, float attackRange) {
		Creature = creature;
		Name = creature.Name;
		Allegiance = creature.Allegiance;
		Level = level;
		Stars = stars;
		MaxHP = creature.MaxHP;
		HP = creature.MaxHP;
		Power = creature.BaseDamage;

		Soulstone = soulstone;
		Exp = 0;
		EvolveCosts = Player.Instance.DataBase.EvolveCosts;
		StatNames = new List<string> { "MaxHP", "Attack", "Range", "Attack speed", "Speed"};
		SecPerShot = secPerShot;
		AttackRange = attackRange;
		if (promoteCosts != null) {
			PromoteCosts = new List<List<string>> (promoteCosts);
		} else {
			PromoteCosts = new List<List<string>> ();
		}
		RankColor = rankColor;

		Skills = new List<Skill> (skills);

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

	public void UpgradeSkill (Skill skill) {		
		if (Skills.Contains(skill)) {			
			if (Player.Instance.Gold >= skill.UpgradeCosts[skill.Level]) {
				Player.Instance.GiveGold (skill.UpgradeCosts [skill.Level]);
				skill.Upgrade ();
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
			string item = PromoteCosts [(int)RankColor] [i];

			if (!Player.Instance.Inventory.ContainsKey(item) || Player.Instance.Inventory[item] == 0) {
				GameManager.Instance.OpenPopUp ("Not enough items!");
				return;
			}
		}

		foreach (var item in PromoteCosts[(int)RankColor]) {
			Player.Instance.GiveItems (new Dictionary<string, int> { { item, 1 } });
		}

		RankColor += 1;
	}

	public void EvolveStar () {
		if (!Player.Instance.Inventory.ContainsKey(Soulstone.Name) || Player.Instance.Inventory[Soulstone.Name] < EvolveCosts[Stars]) {
			GameManager.Instance.OpenPopUp ("Not enough blueprints!");
			return;
		}
		Player.Instance.GiveItems (new Dictionary<string, int> { { Soulstone.Name, EvolveCosts [Stars] } });
		Stars += 1;
	}
}
