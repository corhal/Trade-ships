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

	public ShipData () {

	}

	public ShipData (string name, string allegiance, int level, int stars, int maxHP, 
		int hp, int power, float[] coordinates, List<Skill> skills, List<Effect> effects,
		Item blueprint, List<List<Item>> promoteCosts, RankColor rankColor, bool isSummoned, List<int> levelRequirements, List<RewardChest> rewardChests) {
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
	
}
