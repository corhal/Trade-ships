using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyShip : MonoBehaviour {

	public GameObject CannonBallPrefab;
	public Slider HPSlider;

	int hp;
	int maxHp;

	int firepower;

	public void TakeDamage (int damage) {
		hp -= damage;
		HPSlider.value = hp;
		if (hp <= 0) {
			Destroy (gameObject);
		}
	}



	public void Shoot (Ship ship) {
		GameObject cannonBallObject = Instantiate (CannonBallPrefab) as GameObject;
		cannonBallObject.transform.position = transform.position;
		cannonBallObject.GetComponent<CannonBall> ().Shoot (ship.transform.position, firepower);
	}
}
