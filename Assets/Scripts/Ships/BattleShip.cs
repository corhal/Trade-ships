using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleShip : MonoBehaviour {

	public ParticleSystem smokeParticles;

	public GameObject CannonBallPrefab;
	public List<BattleShip> Enemies;
	public BattleShip Enemy;

	public string Allegiance;
	public int FirePower;
	public int MaxHP;
	public int HP;
	public float SecPerShot;
	public float AttackRange;

	float timer;
	float initialSmokeZ;

	public delegate void BattleshipDestroyedEventHandler (BattleShip sender);
	public event BattleshipDestroyedEventHandler OnBattleShipDestroyed;

	public Slider HPSlider;

	public void SetMaxHP (int maxHp) {		
		MaxHP = maxHp;
		HPSlider.maxValue = maxHp;
		HPSlider.value = HP;
	}

	public void Shoot () {
		GameObject cannonBallObject = Instantiate (CannonBallPrefab) as GameObject;
		smokeParticles.transform.position = Vector2.MoveTowards (transform.position, Enemy.transform.position, 0.5f);
		smokeParticles.transform.position = new Vector3 (smokeParticles.transform.position.x, smokeParticles.transform.position.y, initialSmokeZ);
		smokeParticles.Play ();
		cannonBallObject.transform.position = transform.position;
		cannonBallObject.GetComponent<CannonBall> ().Shoot (Enemy.transform.position, FirePower, Allegiance);
	}

	void Awake () {
		smokeParticles = gameObject.GetComponentInChildren<ParticleSystem> ();
		initialSmokeZ = smokeParticles.transform.position.z;
	}

	void Start () {
		GetEnemies ();
		//Enemies = GameManager.Instance.GetEnemies (Allegiance);
		foreach (var enemy in Enemies) {
			enemy.OnBattleShipDestroyed += Enemy_OnBattleShipDestroyed;
		}
	}

	void Update () {
		if (Enemy != null) {
			if (Vector2.Distance(transform.position, Enemy.transform.position) > AttackRange) {
				Enemy = null;
			}
			timer += Time.deltaTime;
			float secPerShot = (SecPerShot <= 0.0f) ? 0.01f : SecPerShot;
			if (timer >= secPerShot) {
				timer = 0.0f;
				Shoot ();
			}
		} else if (Enemies.Count > 0) {
			for (int i = Enemies.Count - 1; i >= 0; i--) {
				if (Enemies [i] == null) {
					Enemies.Remove (Enemies [i]);
					continue;
				}
				if (Vector2.Distance(transform.position, Enemies [i].transform.position) <= AttackRange) {
					Enemy = Enemies [i];
				}
			}
		}
	}

	public void GetEnemies () {
		List<BattleShip> battleships = new List<BattleShip> (FindObjectsOfType<BattleShip>());

		foreach (var battleShip in battleships) {
			if (battleShip.Allegiance != Allegiance) {
				Enemies.Add (battleShip);
			}
		}
	}

	void Enemy_OnBattleShipDestroyed (BattleShip sender) {
		Enemies.Remove (sender);
		sender.OnBattleShipDestroyed -= Enemy_OnBattleShipDestroyed;
		if (sender == Enemy) {
			Enemy = null;
		}
	}

	public void TakeDamage (int damage) {
		HP -= damage;
		HPSlider.value = HP;
		if (HP <= 0) {
			OnBattleShipDestroyed (this);
			Destroy (gameObject);
		}
	}
}
