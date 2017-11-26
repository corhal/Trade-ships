using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {

	public Allegiance Allegiance;
	public float Speed;
	public Vector2 Target;
	float initialZ;

	int damage;

	void Start () {
		initialZ = transform.position.z;
	}

	public void Shoot (Vector2 target, int damage, Allegiance allegiance) {
		this.Target = target;
		this.damage = damage;
		this.Allegiance = allegiance;
	}

	void Update () {
		float step = Speed * Time.deltaTime;
		transform.position = Vector2.MoveTowards (transform.position, Target, step);
		transform.position = new Vector3 (transform.position.x, transform.position.y, initialZ);
		if (Vector2.Distance (transform.position, Target) < 0.0001f) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		BattleShip otherShip = other.gameObject.GetComponent<BattleShip> ();
		if (otherShip != null && otherShip.Allegiance != Allegiance) {
			otherShip.TakeDamage (damage);
			Destroy (gameObject);
		}
	}
}
