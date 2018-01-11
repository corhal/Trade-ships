using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Suits {
	Hearts, Diamonds, Clubs, Spades
}

public enum Ranks {
	Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 10, Queen = 10, King = 10, Ace = 11
}

public class BJDataBase : MonoBehaviour {

	public List<Sprite> CharacterSprites;
	public List<Sprite> ShipSprites;
	public List<Sprite> CharacterPortraits;
	public List<Sprite> CharacterFigurines;

	public List<string> SkillNames;
	public List<BJSkill> Skills;
	public Dictionary<string, BJSkill> BJSkillsByNames;
	public Dictionary<string, Skill> SkillsByNames;
	public List<Sprite> EffectIcons;

	public List<BJCreature> Creatures;
	public List<BJCreature> EnemyCreatures;

	public List<string> CreatureNames;
	public Dictionary<string, Sprite> FigurinesByNames;
	public Dictionary<string, Sprite> CreaturePortraitsByNames;

	void Start () {		
		List<int> maxHpByLevel = new List<int> { 0, 180, 200, 220, 240, 260, 280, 300, 320, 340, 360 };
		Creatures.Add (new BJCreature ("Lucky Ellie", maxHpByLevel, 200, 250, 3, 2, Allegiance.Player, /*AttackType.Ranged,*/ new List<string>{ "Ranged attack" }));

		maxHpByLevel = new List<int> { 0, 60, 80, 100, 120, 140, 160, 180, 200, 220, 240 };
		Creatures.Add (new BJCreature ("Johnny Two Knives", maxHpByLevel, 80, 20, 4, 4, Allegiance.Player, /*AttackType.Melee,*/ new List<string>{ "Melee attack", "Combo", "Ricochet", "Execution", "Lifesteal" }));

		maxHpByLevel = new List<int> { 0, 100, 120, 140, 160, 180, 200, 220, 240, 260, 280 };
		Creatures.Add (new BJCreature ("Bill the Bull", maxHpByLevel, 121, 10, 1, 1, Allegiance.Player, /*AttackType.Melee,*/ new List<string>{ "Melee attack", "Aggro", "Delay damage", "Cheer team", "Dodge buff" }));

		maxHpByLevel = new List<int> { 0, 70, 90, 110, 130, 150, 170, 190, 210, 230, 250 };
		Creatures.Add (new BJCreature ("Cutthroat Jack", maxHpByLevel, 90, 20, 2, 4, Allegiance.Player, /*AttackType.Melee,*/ new List<string>{ "Melee attack", "Heart strike", "Flurry", "Blinding strike", "Bleed buff" }));

		maxHpByLevel = new List<int> { 0, 40, 60, 80, 100, 120, 140, 160, 180, 200, 220 };
		Creatures.Add (new BJCreature ("One-Shot Ed", maxHpByLevel, 57, 20, 1, 2, Allegiance.Player, /*AttackType.Ranged,*/ new List<string>{ "Ranged attack", "Headshot", "Shoot leg", "Shoot hand", "Adjustment fire" }));

		maxHpByLevel = new List<int> { 0, 40, 60, 80, 100, 120, 140, 160, 180, 200, 220 };
		Creatures.Add (new BJCreature ("Evil Eye Jeanny", maxHpByLevel, 57, 10, 1, 3, Allegiance.Player, /*AttackType.Ranged,*/ new List<string>{ "Ranged attack", "Mass regen", "Heal", "Berserk", "Sacrifice" }));

		maxHpByLevel = new List<int> { 0, 70, 90, 110, 130, 150, 170, 190, 210, 230, 250 };
		EnemyCreatures.Add (new BJCreature ("Skeleton militia", maxHpByLevel, 90, 20, 2, 4, Allegiance.Enemy, /*AttackType.Melee,*/ new List<string>{ "Melee attack" }));

		maxHpByLevel = new List<int> { 0, 110, 130, 150, 170, 190, 210, 230, 250, 270, 290 };
		EnemyCreatures.Add (new BJCreature ("Skeleton captain", maxHpByLevel, 128, 10, 4, 1, Allegiance.Enemy, /*AttackType.Melee,*/ new List<string>{ "Melee attack" }));

		maxHpByLevel = new List<int> { 0, 65, 85, 105, 125, 145, 165, 185, 205, 225, 245 };
		EnemyCreatures.Add (new BJCreature ("Skeleton militia", maxHpByLevel, 85, 20, 3, 4, Allegiance.Enemy, /*AttackType.Melee,*/ new List<string>{ "Melee attack" }));

		maxHpByLevel = new List<int> { 0, 35, 55, 75, 95, 115, 135, 155, 175, 195, 215 };
		EnemyCreatures.Add (new BJCreature ("Skeleton arbalest", maxHpByLevel, 54, 30, 2, 2, Allegiance.Enemy, /*AttackType.Ranged,*/ new List<string>{ "Ranged attack" }));

		maxHpByLevel = new List<int> { 0, 40, 60, 80, 100, 120, 140, 160, 180, 200, 220 };
		EnemyCreatures.Add (new BJCreature ("Skeleton arbalest", maxHpByLevel, 57, 30, 1, 3, Allegiance.Enemy, /*AttackType.Ranged,*/ new List<string>{ "Ranged attack" }));

		FigurinesByNames = new Dictionary<string, Sprite> ();
		CreaturePortraitsByNames = new Dictionary<string, Sprite> ();

		for (int i = 0; i < CreatureNames.Count; i++) {
			FigurinesByNames.Add (CreatureNames [i], CharacterFigurines [i]);
			if (i < CharacterPortraits.Count) {
				CreaturePortraitsByNames.Add (CreatureNames [i], CharacterPortraits [i]);
			}
		}

		BJSkillsByNames = new Dictionary<string, BJSkill> ();
		for (int i = 0; i < SkillNames.Count; i++) {
			BJSkillsByNames.Add (SkillNames [i], Skills [i]);
			Skills [i].Name = SkillNames [i];
		}

		SkillsByNames = new Dictionary<string, Skill> ();
		foreach (var bjskillByName in BJSkillsByNames) {
			SkillsByNames.Add (bjskillByName.Key, new Skill (bjskillByName.Value, 1, 5, new List<int> {
				10,
				20,
				30,
				40,
				50
			}));
		}
	}
}
