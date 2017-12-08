using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionObject : Selectable {

	public Mission Mission;
	public bool IsCastle;
	public Image[] Stars;

	public Island Island;

	protected override void Awake () {
		base.Awake ();
		Island = GetComponentInParent<Island> ();
	}

	protected override void Start () {
		base.Start ();
		if (Mission.EnemyShips.Count == 0) {
			CreateMission ();
			Player.Instance.Missions.Add (Mission);
		}
		for (int i = 0; i < Mission.Stars; i++) {
			Stars [i].sprite = Player.Instance.DataBase.ActiveStarSprite;
		}
		if (IsCastle && Mission.Stars == 3 && Island.Allegiance != Allegiance.Player) {
			Island.Claim ();
		}
		if (Mission.Stars == 3) {
			// enabled = false;
			GetComponent<Collider2D>().enabled = false;
		}
	}


	public void CreateMission () { // TODO: перенести это в более высокий класс, чтобы назначать имя нормальнее
		List<BJCreature> enemyCreatures = new List<BJCreature> (Player.Instance.BJDataBase.EnemyCreatures);

		Utility.Shuffle (enemyCreatures);
		List<CreatureData> enemyShips = new List<CreatureData> ();
		int enemiesCount = Random.Range (1, 6);

		int rankCol = Random.Range (0, System.Enum.GetNames (typeof(RankColor)).Length);
		for (int j = 0; j < enemiesCount; j++) {				
			CreatureData enemy = new CreatureData (enemyCreatures [j], 1, 1, null, null, null, (RankColor)rankCol, false, null);
			enemyShips.Add (enemy);
		}

		int costLength = Random.Range (1, 6);
		Dictionary<string, float> rewardChances = new Dictionary<string, float> ();
		Dictionary<string, int> possibleRewards = new Dictionary<string, int> ();

		for (int i = 0; i < Player.Instance.ShipDatas.Count; i++) {
			// !!! Replace with something more sensible
			float coinToss = Random.Range (0.0f, 1.0f);
			if (coinToss < 0.25f) {
				possibleRewards.Add (Player.Instance.ShipDatas [i].Soulstone.Name, Random.Range (1, 6));
				rewardChances.Add (Player.Instance.ShipDatas [i].Soulstone.Name, Random.Range (0.3f, 0.7f));
			}
		}

		for (int j = 1; j < costLength; j++) {
			List<Item> validItems = new List<Item> ();
			foreach (var item in player.DataBase.TempItemLibrary) {
				if (!possibleRewards.ContainsKey (item.Name) && !item.IsForCraft && !item.IsForSale) {
					validItems.Add (item);
				}
			}

			int index = Random.Range (0, validItems.Count - 1);
			/*Debug.Log ("-----");
			Debug.Log (index);
			Debug.Log (validItems.Count);*/
			possibleRewards.Add (validItems [index].Name, Random.Range (1, 6));
			rewardChances.Add (validItems [index].Name, Random.Range (0.3f, 0.7f));
		}

		Mission = new Mission (Name, IsCastle, rewardChances, possibleRewards, enemyShips);
	}
}
