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

	public TextAsset ItemsTable;

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

		LoadItems (ItemsTable);

		ItemsByNames = new Dictionary<string, Item> ();
		foreach (var item in TempItemLibrary) {
			ItemsByNames.Add (item.Name, item);
		}

		foreach (var item in TempItemLibrary) {
			Player.Instance.TakeItems (new Dictionary<string, int> { { item.Name, 0 } });
		}
	}

	public void LoadItems (TextAsset csvTable) {
		string[,] strings = CSVReader.SplitCsvGrid (csvTable.text);
		for (int i = 1; i < strings.GetLength(1) - 1; i++) { // Х - хардкодий			
			if (strings [0, i] == null) {
				break;
			}
			// TODO: use id
			// int index = System.Int32.Parse (strings [0, i]);

			string name = strings [1, i];

			string[] craftStrings = strings [2, i].Split (';');
			Dictionary<string, int> craftNames = new Dictionary<string, int> ();
			foreach (var craftString in craftStrings) {
				if (craftString == "") {
					continue;
				}
				string[] lines = craftString.Split (':');
				craftNames.Add (lines [0], System.Int32.Parse (lines [1]));
			}

			// TODO: take asset from here
			// string assetName = strings [3, i];

			TempItemLibrary.Add (new Item (name, ItemIconsByNames [name]));
		}
	}
}
