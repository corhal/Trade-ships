using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, ISelectable {

	public string myname;
	public string Name { get { return myname; } }

	public int level;
	public int Level { get { return level; } }

	public int MaxLevel;
	public bool UnderConstruction;

	List<Action> actions;
	public List<Action> Actions { get { return actions; } }

	public List<Dictionary<Item, int>> BuildCosts;
	public List<int> UpgradeCosts;

	GameManager gameManager;
	Player player;

	Action buildAction;
	Action upgradeAction;

	void Awake () {
		actions = new List<Action> ();
		BuildCosts = new List<Dictionary<Item, int>> ();
		//UpgradeCosts = new List<int> ();

		gameManager = GameManager.Instance;
		player = Player.Instance;

		buildAction = new Action ("Build", 0, ShowCraftWindow);
		upgradeAction = new Action ("Upgrade", 0, Upgrade);
	}

	void RefreshActions () {
		actions.Clear ();
		if (!UnderConstruction) {
			upgradeAction.Cost = UpgradeCosts [Level];
			actions.Add (upgradeAction);
		} else {
			actions.Add (buildAction);
		}
	}

	void Start () {
		RefreshActions ();

		for (int i = 0; i < MaxLevel - Level; i++) {
			int costLength = Random.Range (1, 4);
			Dictionary<Item, int> cost = new Dictionary<Item, int> ();
			for (int j = 0; j < costLength; j++) {
				List<Item> validItems = new List<Item> ();
				foreach (var item in gameManager.TempItemLibrary) {
					if (!cost.ContainsKey(item)) {
						validItems.Add (item);
					}
				}

				int index = Random.Range (0, validItems.Count);

				cost.Add (validItems [index], Random.Range(1, 6));
			}
			BuildCosts.Add (cost);
		}
	}

	void OnMouseDown () {
		gameManager.OpentContextButtons (this);
	}

	void ShowCraftWindow () {
		gameManager.OpenCraftWindow (this, null);
	}

	public void Upgrade () {
		if (player.Gold >= UpgradeCosts[Level]) {
			UnderConstruction = true;
			player.GiveGold (UpgradeCosts [Level]);
			gameManager.CloseContextButtons ();
			RefreshActions ();
		} else {
			Debug.Log ("Not enough gold for upgrade");
		}
	}

	public void Build () {
		bool canBuild = player.CheckCost (BuildCosts[Level]);

		if (canBuild) {
			player.GiveItems (BuildCosts [Level]);
			level += 1;
			UnderConstruction = false;
			RefreshActions ();
		} else {
			Debug.Log ("Can't upgrade: not enough items");
		}
	}
}
