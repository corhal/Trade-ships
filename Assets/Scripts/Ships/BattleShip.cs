using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleShip : MonoBehaviour {

	public GameObject CannonBallPrefab;
	public BattleShip Enemy;

	public string Allegiance;
	public int FirePower;
	public int MaxHP;
	public int HP;
	public float SecPerShot;

	float timer;

	//public delegate void ProducedShipmentEventHandler (Port sender, Shipment shipment);
	//public event ProducedShipmentEventHandler OnProducedShipment;

	public Slider HPSlider;

	public void SetMaxHP (int maxHp) {		
		MaxHP = maxHp;
		HPSlider.maxValue = maxHp;
		HPSlider.value = HP;
	}

	public void Shoot () {
		GameObject cannonBallObject = Instantiate (CannonBallPrefab) as GameObject;
		cannonBallObject.transform.position = transform.position;
		cannonBallObject.GetComponent<CannonBall> ().Shoot (Enemy.transform.position, FirePower, Allegiance);
	}

	void Update () {
		if (Enemy != null) {
			timer += Time.deltaTime;
			if (timer >= SecPerShot) {
				timer = 0.0f;
				Shoot ();
			}
		}
	}

	public void TakeDamage (int damage) {
		HP -= damage;
		HPSlider.value = HP;
		if (HP <= 0) {
			Destroy (gameObject);
		}
	}
}
