using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour {

	Ship ship;

	public float SightDistance;

	BattleShip enemy;
	List<BattleShip> enemies;

	float initialZ;

	bool shouldMove;

	void Awake () {
		ship = gameObject.GetComponent<Ship> ();

		initialZ = transform.position.z;
	}

	void Enemy_OnBattleShipDestroyed (BattleShip sender) {
		enemies.Remove (sender);
		sender.OnBattleShipDestroyed -= Enemy_OnBattleShipDestroyed;
	}

	void Start () {
		for (int i = 0; i < Random.Range(4, ship.ShipmentsCapacity); i++) {
			int reward = Random.Range (2, 6);
			int cargo = Random.Range (1, 4);
			Item goods = GameManager.Instance.GetRandomItem (false);
			Shipment shipment = new Shipment (goods, "Tortuga", "Santiago", cargo, reward);
			ship.TakeShipment (shipment);
		}
		enemies = gameObject.GetComponent<BattleShip> ().Enemies;
		foreach (var enemy in enemies) {
			enemy.OnBattleShipDestroyed += Enemy_OnBattleShipDestroyed;
		}
	}

	Island RandomIsland () {
		List<Island> validIslands = new List<Island> ();
		foreach (var island in GameManager.Instance.Islands) {
			if (island.MyPort != null && island.MyPort.IsAvailable) {
				validIslands.Add (island);
			}
		}
		int index = Random.Range (0, validIslands.Count);
		return validIslands [index];
	}

	void Update () {
		if (enemies.Count > 0 && enemy == null) {
			foreach (var enemyShip in enemies) {
				if (Vector2.Distance(transform.position, enemyShip.transform.position) <= SightDistance) {
					enemy = enemyShip;
					shouldMove = true;
				}
			}
		}
		if (enemy != null && Vector2.Distance (transform.position, enemy.transform.position) >= 2.5f) {
			shouldMove = true;
		}
		if (enemy != null && shouldMove) { 
			float tempSpeed = (gameObject.GetComponent<MoveOnClick>().Speed <= 0.0f) ? 0.25f : gameObject.GetComponent<MoveOnClick>().Speed;
			float step = tempSpeed * Time.deltaTime;
			transform.position = Vector2.MoveTowards (transform.position, enemy.transform.position, step);
			transform.position = new Vector3 (transform.position.x, transform.position.y, initialZ);
			if (Vector2.Distance (transform.position, enemy.transform.position) < 2.5f) {
				shouldMove = false;
			}
		}
	}
}
