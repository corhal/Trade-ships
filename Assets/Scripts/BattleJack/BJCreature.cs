using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TakeDamageEventHandler ();
public enum Allegiance {
	Player, Enemy
}

public enum AttackType {
	Melee, Ranged
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
	public int Speed { get { return speed; } set { speed = value; } }

	AttackType attackType;
	public AttackType AttackType { get { return attackType; } }

	Allegiance allegiance;
	public Allegiance Allegiance { get { return allegiance; } }

	public BJCreature (int maxhp, int baseDamage, int speed, Allegiance allegiance, AttackType attackType) {
		this.maxhp = maxhp;
		this.hp = maxhp;
		this.baseDamage = baseDamage;
		this.speed = speed;
		this.allegiance = allegiance;
		this.attackType = attackType;
	}

	public void TakeDamage (int amount) {
		hp = Mathf.Max (0, hp - amount);
		if (OnDamageTaken != null) {
			OnDamageTaken ();
		}
	}

	public void DealDamage (int damage, float multiplier, BJCreature enemy) {
		damage = (int)(damage * multiplier);
		enemy.TakeDamage (damage);
	}

	/*public void DealDamage (float multiplier, BJPlayer enemy) {
		int damage = (int)(baseDamage * multiplier);
		enemy.TakeDamage (damage);
	}

	public void DealDamage (float multiplier, BJGameController enemy) { // cuckoo
		int damage = (int)(baseDamage * multiplier);
		enemy.TakeDamage (damage);
	}*/
}
