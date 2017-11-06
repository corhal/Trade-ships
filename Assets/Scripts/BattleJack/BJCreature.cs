using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TakeDamageEventHandler ();
public enum Allegiance {
	Player, Enemy
}

[System.Serializable]
public class BJCreature {
	public event TakeDamageEventHandler OnDamageTaken;

	public string CaptainName;

	int hp;
	public int HP { get { return hp; } }
	int maxhp;
	public int MaxHP { get { return maxhp; } }
	int baseDamage;
	public int BaseDamage { get { return baseDamage; } }

	int speed;
	public int Speed { get { return speed; } }

	Allegiance allegiance;
	public Allegiance Allegiance { get { return allegiance; } }

	public BJCreature (int maxhp, int baseDamage, int speed, Allegiance allegiance) {
		this.maxhp = maxhp;
		this.hp = maxhp;
		this.baseDamage = baseDamage;
		this.speed = speed;
		this.allegiance = allegiance;
	}

	public void TakeDamage (int amount) {
		hp = Mathf.Max (0, hp - amount);
		if (OnDamageTaken != null) {
			OnDamageTaken ();
		}
	}

	public void DealDamage (float multiplier, BJCreature enemy) {
		int damage = (int)(baseDamage * multiplier);
		enemy.TakeDamage (damage);
	}

	public void DealDamage (float multiplier, BJPlayer enemy) {
		int damage = (int)(baseDamage * multiplier);
		enemy.TakeDamage (damage);
	}

	public void DealDamage (float multiplier, BJGameController enemy) { // cuckoo
		int damage = (int)(baseDamage * multiplier);
		enemy.TakeDamage (damage);
	}
}
