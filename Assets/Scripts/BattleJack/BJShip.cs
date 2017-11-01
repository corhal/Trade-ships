using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BJShip {

	public delegate void BJShipTakeDamageEventHandler ();
	public event BJShipTakeDamageEventHandler OnDamageTaken;

	int hp;
	public int HP { get { return hp; } }

	int baseDamage;
	public int BaseDamage { get { return baseDamage; } }

	public BJShip (int hp, int baseDamage) {
		this.hp = hp;
		this.baseDamage = baseDamage;
	}

	public void TakeDamage (int amount) {
		hp = Mathf.Max (0, hp - amount);
		OnDamageTaken ();
	}

	public void DealDamage (float multiplier, BJShip enemy) {
		int damage = (int)(baseDamage * multiplier);
		enemy.TakeDamage (damage);
	}
}
