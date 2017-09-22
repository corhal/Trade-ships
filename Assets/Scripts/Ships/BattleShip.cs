﻿using System.Collections;
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
		smokeParticles.transform.position = new Vector3 (smokeParticles.transform.position.x, smokeParticles.transform.position.y, -2.0f);
		smokeParticles.Play ();
		cannonBallObject.transform.position = transform.position;
		cannonBallObject.GetComponent<CannonBall> ().Shoot (Enemy.transform.position, FirePower, Allegiance);
	}

	void Awake () {
		smokeParticles = gameObject.GetComponentInChildren<ParticleSystem> ();
	}

	void Start () {
		Enemies = GameManager.Instance.GetEnemies (Allegiance);
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
			if (timer >= SecPerShot) {
				timer = 0.0f;
				Shoot ();
			}
		} else if (Enemies.Count > 0) {
			foreach (var enemy in Enemies) {
				if (Vector2.Distance(transform.position, enemy.transform.position) <= AttackRange) {
					Enemy = enemy;
				}
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
