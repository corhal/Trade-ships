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
