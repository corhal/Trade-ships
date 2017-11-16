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
	public Dictionary<string, BJSkill> SkillsByNames;
	public List<Sprite> EffectIcons;

	public List<BJCreature> Creatures;
	public List<BJCreature> EnemyCreatures;

	public List<string> CreatureNames;
	public Dictionary<string, Sprite> FigurinesByNames;
	//public Dictionary<string, Sprite>

	void Start () {		
		Creatures.Add (new BJCreature ("Lucky Ellie", 200, 250, 3, 2, Allegiance.Player, AttackType.Ranged, new List<string>{ "Ranged attack" }));
		Creatures.Add (new BJCreature ("Johnny Two Knives", 80, 20, 4, 4, Allegiance.Player, AttackType.Melee, new List<string>{ "Melee attack", "Combo", "Ricochet", "Execution", "Lifesteal" }));
		Creatures.Add (new BJCreature ("Bill the Bull", 121, 10, 1, 1, Allegiance.Player, AttackType.Melee, new List<string>{ "Melee attack", "Aggro", "Delay damage", "Cheer team", "Dodge buff" }));
		Creatures.Add (new BJCreature ("Cutthroat Jack", 90, 20, 2, 4, Allegiance.Player, AttackType.Melee, new List<string>{ "Melee attack", "Heart strike", "Flurry", "Blinding strike", "Bleed buff" }));
		Creatures.Add (new BJCreature ("One-Shot Ed", 57, 20, 1, 2, Allegiance.Player, AttackType.Ranged, new List<string>{ "Ranged attack", "Headshot", "Shoot leg", "Shoot hand", "Adjustment fire" }));
		Creatures.Add (new BJCreature ("Evil Eye Jeanny", 57, 10, 1, 3, Allegiance.Player, AttackType.Ranged, new List<string>{ "Ranged attack", "Mass regen", "Heal", "Berserk", "Sacrifice" }));

		EnemyCreatures.Add (new BJCreature ("Skeleton militia", 90, 20, 2, 4, Allegiance.Enemy, AttackType.Melee, new List<string>{ "Melee attack" }));
		EnemyCreatures.Add (new BJCreature ("Skeleton captain", 128, 10, 4, 1, Allegiance.Enemy, AttackType.Melee, new List<string>{ "Melee attack" }));
		EnemyCreatures.Add (new BJCreature ("Skeleton militia", 85, 20, 3, 4, Allegiance.Enemy, AttackType.Melee, new List<string>{ "Melee attack" }));
		EnemyCreatures.Add (new BJCreature ("Skeleton arbalest", 54, 30, 2, 2, Allegiance.Enemy, AttackType.Ranged, new List<string>{ "Ranged attack" }));
		EnemyCreatures.Add (new BJCreature ("Skeleton arbalest", 57, 30, 1, 3, Allegiance.Enemy, AttackType.Ranged, new List<string>{ "Ranged attack" }));

		FigurinesByNames = new Dictionary<string, Sprite> ();

		for (int i = 0; i < CreatureNames.Count; i++) {
			FigurinesByNames.Add (CreatureNames [i], CharacterFigurines [i]);
		}

		SkillsByNames = new Dictionary<string, BJSkill> ();
		for (int i = 0; i < SkillNames.Count; i++) {
			SkillsByNames.Add (SkillNames [i], Skills [i]);
		}
	}
}
