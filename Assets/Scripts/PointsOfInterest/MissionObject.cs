using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionObject : PointOfInterest {

	public Mission Mission;
	public bool IsCastle;
	public Image[] Stars;

	void Start () {
		if (Mission.EnemyShips.Count == 0) {
			CreateMission ();
			Player.Instance.Missions.Add (Mission);
		}
		for (int i = 0; i < Mission.Stars; i++) {
			Stars [i].sprite = Player.Instance.DataBase.ActiveStarSprite;
		}
		/*if (IsCastle && Mission.Stars == 3 && Island.Allegiance != Allegiance.Player) {
			Island.Claim ();
		}*/

		foreach (var mission in Player.Instance.Missions) {
			if (Mission.Name == mission.Name && mission.Stars == 3) {
				Mission.Stars = 3;
			}
		}

		if (Mission.Stars == 3) {
			gameObject.SetActive (false);
		}

		if (Mission.Stars == 3 && !POIData.Interacted) {
			Interact ();
		}
	}


	public void CreateMission () { // TODO: перенести это в более высокий класс, чтобы назначать имя нормальнее
		List<BJCreature> enemyCreatures = new List<BJCreature> (Player.Instance.BJDataBase.EnemyCreatures);

		Utility.Shuffle (enemyCreatures);
		List<CreatureData> enemyShips = new List<CreatureData> ();
		int enemiesCount = Random.Range (1, 6);

		int rankCol = Random.Range (0, System.Enum.GetNames (typeof(RankColor)).Length);
		for (int j = 0; j < enemiesCount; j++) {				
			CreatureData enemy = new CreatureData (enemyCreatures [j], 1, null, null, (RankColor)rankCol, false);
			enemyShips.Add (enemy);
		}

		Dictionary<string, float> rewardChances = new Dictionary<string, float> ();
		Dictionary<string, int> possibleRewards = new Dictionary<string, int> ();

		// possibleRewards.Add ("Gold", Random.Range (10, 100));
		// rewardChances.Add ("Gold", 1.0f);

		float diceRoll = Random.Range (0.0f, 1.0f);

		if (diceRoll < 0.75f) {
			possibleRewards.Add ("Copper lockpick", Random.Range (1, 4));
			rewardChances.Add ("Copper lockpick", 1.0f);
		} else if (diceRoll < 0.95f) {
			possibleRewards.Add ("Silver lockpick", Random.Range (1, 3));
			rewardChances.Add ("Silver lockpick", 1.0f);
		} else {
			possibleRewards.Add ("Golden lockpick", Random.Range (1, 2));
			rewardChances.Add ("Golden lockpick", 1.0f);
		}

		if (Player.Instance.CurrentAdventure.TreasureHunt) {
			possibleRewards.Add ("Map", Random.Range (1, 3));
			rewardChances.Add ("Map", 1.0f);
		}

		Mission = new Mission (Tile.BoardCoordsAsString, IsCastle, rewardChances, possibleRewards, enemyShips);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (Mission.EnemyShips.Count > 0 && other.GetComponent<PlayerShip> () != null) {
			UIOverlay.Instance.OpenMissionWindow (Mission);
		}
	}
}
