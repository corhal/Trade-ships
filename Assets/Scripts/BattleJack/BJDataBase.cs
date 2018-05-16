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

	public List<List<BJCreature>> EnemySets;

	void Start () {		
		
		List<int> maxHpByLevel = new List<int> { 0, 100, 120, 140, 160, 180, 200, 220, 240, 260, 280 };
		List<int> baseDamageByLevel = new List<int> { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
		Creatures.Add (new BJCreature ("Brawler", maxHpByLevel, maxHpByLevel[1], baseDamageByLevel, 1, 1, Allegiance.Player, /*AttackType.Melee,*/ new List<string>{ "Melee attack", "Aggro", /*"Delay damage", "Cheer team",*/ "Dodge buff" }));

		maxHpByLevel = new List<int> { 0, 70, 90, 110, 130, 150, 170, 190, 210, 230, 250 };
		baseDamageByLevel = new List<int> { 0, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55 };
		Creatures.Add (new BJCreature ("Cutthroat", maxHpByLevel, maxHpByLevel[1], baseDamageByLevel, 2, 4, Allegiance.Player, /*AttackType.Melee,*/ new List<string>{ "Melee attack", /*"Heart strike",*/ "Flurry", /*"Blinding strike",*/ "Bleed buff" }));

		maxHpByLevel = new List<int> { 0, 40, 60, 80, 100, 120, 140, 160, 180, 200, 220 };
		baseDamageByLevel = new List<int> { 0, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60 };
		Creatures.Add (new BJCreature ("Hunter", maxHpByLevel, maxHpByLevel[1], baseDamageByLevel, 1, 2, Allegiance.Player, /*AttackType.Ranged,*/ new List<string>{ "Ranged attack", "Headshot", /*"Shoot leg", "Shoot hand",*/ "Adjustment fire" }));

		maxHpByLevel = new List<int> { 0, 40, 60, 80, 100, 120, 140, 160, 180, 200, 220 };
		baseDamageByLevel = new List<int> { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
		Creatures.Add (new BJCreature ("Witch", maxHpByLevel, maxHpByLevel[1], baseDamageByLevel, 1, 3, Allegiance.Player, /*AttackType.Ranged,*/ new List<string>{ "Ranged attack", /*"Mass regen",*/ "Heal", /*"Berserk",*/ "Sacrifice" }));

		maxHpByLevel = new List<int> { 0, 60, 80, 100, 120, 140, 160, 180, 200, 220, 240 };
		baseDamageByLevel = new List<int> { 0, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60 };
		Creatures.Add (new BJCreature ("Bandit", maxHpByLevel, maxHpByLevel[1], baseDamageByLevel, 4, 4, Allegiance.Player, /*AttackType.Melee,*/ new List<string>{ "Melee attack", "Combo", /*"Ricochet", "Execution",*/ "Lifesteal" }));

		maxHpByLevel = new List<int> { 0, 40, 60, 80, 100, 120, 140, 160, 180, 200, 220 };
		baseDamageByLevel = new List<int> { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
		Creatures.Add (new BJCreature ("Alchemist", maxHpByLevel, maxHpByLevel[1], baseDamageByLevel, 1, 4, Allegiance.Player, /*AttackType.Ranged,*/ new List<string>{ "Ranged attack", "Blinding strike", "Corrosion" }));

		maxHpByLevel = new List<int> { 0, 110, 130, 150, 170, 190, 210, 230, 250, 270, 290 };
		baseDamageByLevel = new List<int> { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
		BJCreature skeletonCaptain = new BJCreature ("Skeleton captain", maxHpByLevel, maxHpByLevel [1], baseDamageByLevel, 4, 1, Allegiance.Enemy, /*AttackType.Melee,*/new List<string>{ "Melee attack" });
		EnemyCreatures.Add (skeletonCaptain);

		maxHpByLevel = new List<int> { 0, 65, 85, 105, 125, 145, 165, 185, 205, 225, 245 };
		baseDamageByLevel = new List<int> { 0, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60 };
		BJCreature skeletonMilitia = new BJCreature("Skeleton militia", maxHpByLevel, maxHpByLevel[1], baseDamageByLevel, 3, 4, Allegiance.Enemy, /*AttackType.Melee,*/ new List<string>{ "Melee attack" });
		EnemyCreatures.Add (skeletonMilitia);

		maxHpByLevel = new List<int> { 0, 35, 55, 75, 95, 115, 135, 155, 175, 195, 215 };
		baseDamageByLevel = new List<int> { 0, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70 };
		BJCreature skeletonArbalest = new BJCreature ("Skeleton arbalest", maxHpByLevel, maxHpByLevel [1], baseDamageByLevel, 2, 2, Allegiance.Enemy, /*AttackType.Ranged,*/new List<string>{ "Ranged attack" });
		EnemyCreatures.Add (skeletonArbalest);

		BJCreature skeletonOgre = new BJCreature ("Skeleton ogre", maxHpByLevel, maxHpByLevel [1], baseDamageByLevel, 4, 1, Allegiance.Enemy, /*AttackType.Melee,*/new List<string> {
			"Melee attack",
			"Armor buff"
		});
		maxHpByLevel = new List<int> { 0, 90, 110, 130, 150, 170, 190, 210, 230, 250, 270 };
		baseDamageByLevel = new List<int> { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
		EnemyCreatures.Add (skeletonOgre);

		maxHpByLevel = new List<int> { 0, 70, 90, 110, 130, 150, 170, 190, 210, 230, 250 };
		baseDamageByLevel = new List<int> { 0, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55 };
		Creatures.Add (new BJCreature ("Executioner", maxHpByLevel, maxHpByLevel[1], baseDamageByLevel, 2, 4, Allegiance.Player, /*AttackType.Melee,*/ new List<string>{ "Melee attack", "Execution", "Bloodlust" }));

		maxHpByLevel = new List<int> { 0, 35, 55, 75, 95, 115, 135, 155, 175, 195, 215 };
		baseDamageByLevel = new List<int> { 0, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60 };
		BJCreature skeletonWitch = new BJCreature ("Skeleton witch", maxHpByLevel, maxHpByLevel [1], baseDamageByLevel, 2, 2, Allegiance.Enemy, /*AttackType.Ranged,*/new List<string>{ "Ranged attack" });
		EnemyCreatures.Add (skeletonWitch);

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

		EnemySets = new List<List<BJCreature>> {
			new List<BJCreature> { skeletonMilitia, skeletonCaptain, skeletonArbalest },
			new List<BJCreature> { skeletonMilitia, skeletonCaptain, skeletonWitch },
			new List<BJCreature> { skeletonMilitia, skeletonMilitia, skeletonArbalest },
			new List<BJCreature> { skeletonMilitia, skeletonCaptain, skeletonArbalest, skeletonArbalest },
			new List<BJCreature> { skeletonMilitia, skeletonCaptain, skeletonArbalest, skeletonWitch },
			new List<BJCreature> { skeletonMilitia, skeletonMilitia, skeletonArbalest },
			new List<BJCreature> { skeletonMilitia, skeletonMilitia, skeletonWitch }
		};
	}
}
