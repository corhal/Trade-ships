using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {

	public string Allegiance;
	public float Speed;
	public Vector2 Target;
	float initialZ;

	int damage;

	void Start () {
		initialZ = transform.position.z;
	}

	public void Shoot (Vector2 target, int damage) {
		this.Target = target;
		this.damage = damage;
	}

	void Update () {
		float step = Speed * Time.deltaTime;
		transform.position = Vector2.MoveTowards (transform.position, Target, step);
		transform.position = new Vector3 (transform.position.x, transform.position.y, initialZ);
		if (Vector2.Distance (transform.position, Target) < 0.0001f) {
			Destroy (gameObject);
		}
	}

	void OnCollider2DEnter (Collider2D other) {
		if (Allegiance == "player" && other.gameObject.GetComponent<EnemyShip>() != null) {
			other.gameObject.GetComponent<EnemyShip> ().TakeDamage (damage);
			Destroy (gameObject);
		}
		if (Allegiance == "enemy" && other.gameObject.GetComponent<Ship>() != null) {
			other.gameObject.GetComponent<Ship> ().TakeDamage (damage);
			Destroy (gameObject);
		}
	}
}
