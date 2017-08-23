using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, ISelectable {

	public int Level;
	public int MaxLevel;
	public bool UnderConstruction;

	List<Action> actions;
	public List<Action> Actions { get { return actions; } }

	public List<Dictionary<Item, int>> UpgradeCosts;

	GameManager gameManager;
	Player player;

	void Awake () {
		actions = new List<Action> ();
		UpgradeCosts = new List<Dictionary<Item, int>> ();

		gameManager = GameManager.Instance;
		player = Player.Instance;

		Action upgradeAction = new Action ("Upgrade", 0, Upgrade);
		actions.Add (upgradeAction);
	}

	void Start () {
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
			UpgradeCosts.Add (cost);
		}
	}

	void OnMouseDown () {
		gameManager.OpentContextButtons (this);
	}

	public void Upgrade () {
		bool canUpgrade = player.CheckCost (UpgradeCosts[Level]);

		if (canUpgrade) {
			player.GiveItems (UpgradeCosts [Level]);
			Level += 1;
		} else {
			Debug.Log ("Can't upgrade: not enough items");
		}
	}
}
