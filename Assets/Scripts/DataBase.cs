using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour {

	public Dictionary<string, Sprite> ItemIconsByNames;
	public List<Sprite> ItemIcons;
	public List<string> ItemNames;

	public Dictionary<string, Sprite> ActionIconsByNames;
	public List<Sprite> ActionIcons;
	public List<string> ActionNames;

	public Dictionary<string, Sprite> CreatureFigurinesByNames;
	public List<Sprite> CreatureFigurines;

	public Dictionary<string, Sprite> CreaturePortraitsByNames;
	public List<Sprite> CreaturePortraits;
	public List<string> CreatureNames = new List<string> {
		"Vulpecula", "Manticora", "Herr Mannelig", "Nexus", "Liguria"
	};

	public List<Item> TempItemLibrary;

	public List<int> EvolveCosts = new List<int> {
		10,
		30,
		80,
		160,
		300
	};

	public List<GameObject> EffectParticlePrefabs;
	public List<string> EffectParticleNames;

	public Dictionary<string, Skill> SkillsByNames;

	public Dictionary<RankColor, Color> ColorsByRankColors;

	void Awake () {
		ActionIconsByNames = new Dictionary<string, Sprite> ();
		for (int i = 0; i < ActionIcons.Count; i++) {
			ActionIconsByNames.Add (ActionNames [i], ActionIcons [i]);
		}

		ItemIconsByNames = new Dictionary<string, Sprite> ();
		for (int i = 0; i < ItemIcons.Count; i++) {
			ItemIconsByNames.Add (ItemNames [i], ItemIcons [i]);
		}

		CreaturePortraitsByNames = new Dictionary<string, Sprite> ();
		for (int i = 0; i < CreaturePortraits.Count; i++) {
			CreaturePortraitsByNames.Add (CreatureNames [i], CreaturePortraits [i]);
		}

		CreatureFigurinesByNames = new Dictionary<string, Sprite> ();
		for (int i = 0; i < CreatureFigurines.Count; i++) {
			CreatureFigurinesByNames.Add (CreatureNames [i], CreatureFigurines [i]);
		}
	}

	void Start () {
		ColorsByRankColors = new Dictionary<RankColor, Color> {
			{ RankColor.White, Color.white },
			{ RankColor.Green, Color.green },
			{ RankColor.GreenP, Color.green },
			{ RankColor.Blue, Color.blue },
			{ RankColor.BlueP, Color.blue },
			{ RankColor.BluePP, Color.blue },
			{ RankColor.Purple, Color.cyan },
			{ RankColor.PurpleP, Color.cyan },
			{ RankColor.PurplePP, Color.cyan },
			{ RankColor.PurplePPP, Color.cyan },
			{ RankColor.PurplePPPP, Color.cyan },
			{ RankColor.Orange, Color.yellow },
			{ RankColor.OrangeP, Color.yellow },
		};

		Item wood = new Item ("Wood", null, ItemIconsByNames["Wood"], false, true, new Dictionary<string, int> {{"MinReward", 1}, {"MaxReward", 1}});
		Item food = new Item ("Food", null, ItemIconsByNames["Food"], true, false, null);
		Item steel = new Item ("Steel", null, ItemIconsByNames["Steel"], false, true, new Dictionary<string, int> {{"Firepower", 5}});
		Item nails = new Item ("Nails", new Dictionary<Item, int> { { steel, 2 } }, ItemIconsByNames["Nails"], false, true, null);
		Item hammers = new Item ("Picks", new Dictionary<Item, int> { { steel, 1 }, {wood, 1} }, ItemIconsByNames["Picks"], false, true, null);
		Item saws = new Item ("Shovels", new Dictionary<Item, int> { { steel, 2 }, {wood, 1} }, ItemIconsByNames["Shovels"], false, true, null);
		Item tools = new Item ("Tools", new Dictionary<Item, int> { { hammers, 1 }, {saws, 1} }, ItemIconsByNames["Tools"], false, true, null);
		Item spices = new Item ("Spices", null, ItemIconsByNames["Spices"], true, false, null);
		Item ale = new Item ("Ale", null, ItemIconsByNames["Ale"], true, false, null);
		Item fish = new Item ("Fish", null, ItemIconsByNames["Fish"], true, false, null);

		TempItemLibrary = new List<Item> {
			wood,
			food,
			steel,
			nails,
			hammers,
			saws,
			tools,
			spices,
			ale,
			fish,
		};

		foreach (var item in TempItemLibrary) {
			Player.Instance.TakeItems (new Dictionary<Item, int> { { item, 0 } });
		}

		Effect slowDown = new Effect ("Slow down", 0, 10.0f, new List<Dictionary<string, int>> {
			new Dictionary<string, int> { { "Speed", -1000 } },
			new Dictionary<string, int> { { "Speed", -2000 } },
			new Dictionary<string, int> { { "Speed", -3000 } },
			new Dictionary<string, int> { { "Speed", -4000 } },
			new Dictionary<string, int> { { "Speed", -5000 } },
			new Dictionary<string, int> { { "Speed", -6000 } },
		});
		Effect speedUpFire = new Effect ("Speed up", 0, 10.0f, new List<Dictionary<string, int>> {
			new Dictionary<string, int> { { "Attack speed", -1000 } },
			new Dictionary<string, int> { { "Attack speed", -2000 } },
			new Dictionary<string, int> { { "Attack speed", -3000 } },
			new Dictionary<string, int> { { "Attack speed", -4000 } },
			new Dictionary<string, int> { { "Attack speed", -5000 } },
			new Dictionary<string, int> { { "Attack speed", -6000 } },
		});
		Effect trade = new Effect ("Trade", 0, -1.0f, new List<Dictionary<string, int>> {
			new Dictionary<string, int> { { "Cargo", 0 } },
			new Dictionary<string, int> { { "Cargo", 1 } },
			new Dictionary<string, int> { { "Cargo", 2 } },
			new Dictionary<string, int> { { "Cargo", 3 } },
			new Dictionary<string, int> { { "Cargo", 4 } },
			new Dictionary<string, int> { { "Cargo", 5 } },
		});
		Effect cannons = new Effect ("Cannons", 0, -1.0f, new List<Dictionary<string, int>> {
			new Dictionary<string, int> { { "Firepower", 0 } },
			new Dictionary<string, int> { { "Firepower", 10 } },
			new Dictionary<string, int> { { "Firepower", 20 } },
			new Dictionary<string, int> { { "Firepower", 30 } },
			new Dictionary<string, int> { { "Firepower", 40 } },
			new Dictionary<string, int> { { "Firepower", 50 } },
		});

		Skill slowDownSkill = new Skill ("Slow down", 1, 5, RankColor.White, new List<int> { 10, 20, 30, 40, 50 }, new Dictionary<string, Effect> { {
				"enemy",
				slowDown
			}
		});
		Skill speedUpFireSkill = new Skill ("Speed up", 0, 5, RankColor.White, new List<int> { 10, 20, 30, 40, 50 }, new Dictionary<string, Effect> { {
				"self",
				speedUpFire
			}
		});
		Skill tradeSkill =	new Skill ("Trade", 0, 5, RankColor.Green, new List<int> { 10, 20, 30, 40, 50 }, new Dictionary<string, Effect> { {
				"self",
				trade
			}
		});
		Skill cannonsSkill =	new Skill ("Cannons", 0, 5, RankColor.Blue, new List<int> { 10, 20, 30, 40, 50 }, new Dictionary<string, Effect> { {
				"self",
				cannons
			}
		});
		Skill dummySkill = new Skill ("Dummy", 0, 5, RankColor.Purple, new List<int> { 10, 20, 30, 40, 50 }, null);

		SkillsByNames = new Dictionary<string, Skill> {
			{ slowDownSkill.Name, slowDownSkill },
			{ speedUpFireSkill.Name, speedUpFireSkill },
			{ tradeSkill.Name, tradeSkill },
			{ cannonsSkill.Name, cannonsSkill },
			{ dummySkill.Name, dummySkill },
		};
	}
}
