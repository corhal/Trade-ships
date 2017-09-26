using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyShip : MonoBehaviour {

	public GameObject ShipwreckPrefab;

	BattleShip battleship;

	public float SightDistance;
	public float Speed;

	BattleShip enemy;
	List<BattleShip> enemies;

	float initialZ;

	bool shouldMove;

	public List<Shipment> Shipments;

	void Awake () {
		battleship = gameObject.GetComponent<BattleShip> ();
		enemies = GameManager.Instance.GetEnemies (battleship.Allegiance);
		foreach (var enemy in enemies) {
			enemy.OnBattleShipDestroyed += Enemy_OnBattleShipDestroyed;
		}
		initialZ = transform.position.z;
	}

	void Enemy_OnBattleShipDestroyed (BattleShip sender) {
		enemies.Remove (sender);
		sender.OnBattleShipDestroyed -= Enemy_OnBattleShipDestroyed;
	}

	void Start () {
		battleship.SetMaxHP (battleship.MaxHP);

		for (int i = 0; i < Random.Range(4, 8); i++) {
			int reward = Random.Range (2, 6);
			int cargo = Random.Range (1, 4);
			Item goods = GameManager.Instance.GetRandomItem (false);
			Shipment shipment = new Shipment (goods, RandomIsland (), RandomIsland (), cargo, reward);
			Shipments.Add (shipment);
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
			float step = Speed * Time.deltaTime;
			transform.position = Vector2.MoveTowards (transform.position, enemy.transform.position, step);
			transform.position = new Vector3 (transform.position.x, transform.position.y, initialZ);
			if (Vector2.Distance (transform.position, enemy.transform.position) < 2.5f) {
				shouldMove = false;
			}
		}
	}

	public void SpawnShipwreck () {
		GameObject shipwreckObject = Instantiate (ShipwreckPrefab) as GameObject;
		shipwreckObject.transform.position = transform.position;
		Port shipwreckPort = shipwreckObject.GetComponent<Port> ();
		shipwreckPort.ShipmentsCapacities [1] = Shipments.Count;
		foreach (var shipment in Shipments) {
			shipwreckPort.TakeShipment (shipment);
		}
	}

}
