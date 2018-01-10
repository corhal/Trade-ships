using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RankColor {
	White, Green, GreenP, Blue, BlueP, BluePP, Purple, PurpleP, PurplePP, PurplePPP, PurplePPPP, Orange, OrangeP
}

[System.Serializable]
public class CreatureData {	

	public bool IsSummoned;
	public List<Skill> Skills;

	public Item Soulstone;
	public RankColor RankColor;
	public int Level;
	public string Name;
	public Allegiance Allegiance;



	public float[] Coordinates;
	public List<int> LevelCosts;
	public List<string> StatNames;

	//public float SecPerShot;
	//public float AttackRange;
	//public float Speed = 1.0f; // change later

	public BJCreature Creature;

	public int Attack { get { return Creature.BaseDamage; } set { Creature.BaseDamage = value; } }
	public int MaxHP { get { return Creature.MaxHP; } }
	public int HP { get { return Creature.HP; } set { Creature.HP = value; } }
	public bool IsDead { get { return Creature.IsDead; } set { Creature.IsDead = value; } }

	public CreatureData () {

	}

	public CreatureData (BJCreature creature, int level, List<Skill> skills,
		Item soulstone,  RankColor rankColor, bool isSummoned) {
		Creature = creature;
		Name = creature.Name;
		Allegiance = creature.Allegiance;
		Level = level;
		// MaxHP = creature.MaxHP;
		// HP = creature.MaxHP;
		Attack = creature.BaseDamage;

		Soulstone = soulstone;
		LevelCosts = Player.Instance.DataBase.EvolveCosts;
		StatNames = new List<string> { "MaxHP", "Attack"};

		RankColor = rankColor;

		if (skills != null) {
			Skills = new List<Skill> (skills);
		} else {
			Skills = new List<Skill> ();
		}
		IsSummoned = isSummoned;
	}

	public int GetStatByString (string statName) {
		switch (statName) {
		case "HP":
			return HP;
		case "MaxHP":
			return MaxHP;
		case "Attack":
			return Attack;
		default:
			return 0;
		}
	}

	public void UpgradeSkill (Skill skill) {		
		if (Skills.Contains(skill)) {			
			if (Player.Instance.Gold >= skill.UpgradeCosts[skill.Level]) {
				Player.Instance.GiveGold (skill.UpgradeCosts [skill.Level]);
				skill.Upgrade ();
			} else {
				UIOverlay.Instance.OpenPopUp ("Not enough gold!");
			}
		}
	}

	public void LevelUp () {
		if (!Player.Instance.Inventory.ContainsKey(Soulstone.Name) || Player.Instance.Inventory[Soulstone.Name] < LevelCosts[Level]) {
			UIOverlay.Instance.OpenPopUp ("Not enough soulstones!");
			return;
		}
		Player.Instance.GiveItems (new Dictionary<string, int> { { Soulstone.Name, LevelCosts [Level] } });
		Level += 1;
	}
}
