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

	public Dictionary<string, Sprite> FigurinesByNames;
	//public Dictionary<string, Sprite>

	void Start () {		
		Creatures.Add (new BJCreature ("Lucky Ellie", 200, 250, 3, 2, Allegiance.Player, AttackType.Ranged, new List<string>{ "Ranged attack" }));
		Creatures.Add (new BJCreature ("Johnny Two Knives", 500, 200, 5, 4, Allegiance.Player, AttackType.Melee, new List<string>{ "Melee attack", "Combo", "Ricochet", "Execution" }));
		Creatures.Add (new BJCreature ("Bill the Bull", 1000, 50, 10, 1, Allegiance.Player, AttackType.Melee, new List<string>{ "Melee attack", "Aggro", "Delay damage", "Cheer team", "Dodge buff" }));
		Creatures.Add (new BJCreature ("Cutthroat Jack", 600, 150, 4, 4, Allegiance.Player, AttackType.Melee, new List<string>{ "Melee attack", "Heart strike", "Flurry", "Blinding strike", "Bleed buff" }));
		Creatures.Add (new BJCreature ("One-Shot Ed", 250, 200, 2, 2, Allegiance.Player, AttackType.Ranged, new List<string>{ "Ranged attack", "Headshot", "Shoot leg", "Shoot hand", "Adjustment fire" }));
		Creatures.Add (new BJCreature ("Evil Eye Jeanny", 200, 150, 1, 3, Allegiance.Player, AttackType.Ranged, new List<string>{ "Ranged attack", "Mass regen", "Heal", "Berserk", "Sacrifice" }));
		FigurinesByNames = new Dictionary<string, Sprite> ();
		for (int i = 0; i < Creatures.Count; i++) {
			FigurinesByNames.Add (Creatures [i].Name, CharacterFigurines [i]);
		}
		SkillsByNames = new Dictionary<string, BJSkill> ();
		for (int i = 0; i < SkillNames.Count; i++) {
			SkillsByNames.Add (SkillNames [i], Skills [i]);
		}
	}
}
