using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour {

	public Sprite ActiveStarSprite;
	public Sprite InactiveStarSprite;

	public Dictionary<string, Sprite> ItemIconsByNames;
	public List<Sprite> ItemIcons;
	public List<string> ItemNames;

	public Dictionary<string, Sprite> ActionIconsByNames;
	public List<Sprite> ActionIcons;
	public List<string> ActionNames;

	public Dictionary<string, Sprite> CreaturePortraitsByNames;
	public List<Sprite> CreaturePortraits;
	public List<string> CreatureNames = new List<string> {
		"Vulpecula", "Manticora", "Herr Mannelig", "Nexus", "Liguria"
	};

	public List<Item> TempItemLibrary;
	public Dictionary<string, Item> ItemsByNames;

	public List<int> EvolveCosts = new List<int> {
		10,
		30,
		80,
		160,
		300
	};

	public List<GameObject> EffectParticlePrefabs;
	public List<string> EffectParticleNames;

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
		Item food = new Item ("Food", null, ItemIconsByNames["Food"], true, false, new Dictionary<string, int> {{"Firepower", 5}});
		Item steel = new Item ("Steel", null, ItemIconsByNames["Steel"], false, true, new Dictionary<string, int> {{"Firepower", 5}});
		Item nails = new Item ("Nails", new Dictionary<string, int> { { "Steel", 2 } }, ItemIconsByNames["Nails"], false, true, new Dictionary<string, int> {{"Firepower", 5}});
		Item hammers = new Item ("Picks", new Dictionary<string, int> { { "Steel", 1 }, {"Wood", 1} }, ItemIconsByNames["Picks"], false, true, new Dictionary<string, int> {{"Firepower", 5}});
		Item saws = new Item ("Shovels", new Dictionary<string, int> { { "Steel", 2 }, {"Wood", 1} }, ItemIconsByNames["Shovels"], false, true, new Dictionary<string, int> {{"Firepower", 5}});
		Item tools = new Item ("Tools", new Dictionary<string, int> { { "Picks", 1 }, {"Shovels", 1} }, ItemIconsByNames["Tools"], false, true, new Dictionary<string, int> {{"Firepower", 5}});
		Item spices = new Item ("Spices", null, ItemIconsByNames["Spices"], true, false, new Dictionary<string, int> {{"Firepower", 5}});
		Item ale = new Item ("Ale", null, ItemIconsByNames["Ale"], true, false, new Dictionary<string, int> {{"Firepower", 5}});
		Item fish = new Item ("Fish", null, ItemIconsByNames["Fish"], true, false, new Dictionary<string, int> {{"Firepower", 5}});

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

		ItemsByNames = new Dictionary<string, Item> ();
		foreach (var item in TempItemLibrary) {
			ItemsByNames.Add (item.Name, item);
		}

		foreach (var item in TempItemLibrary) {
			Player.Instance.TakeItems (new Dictionary<string, int> { { item.Name, 0 } });
		}
	}
}
