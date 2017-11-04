using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TakeDamageEventHandler ();

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

	public BJCreature (int maxhp, int baseDamage) {
		this.maxhp = maxhp;
		this.hp = maxhp;
		this.baseDamage = baseDamage;
	}

	public void TakeDamage (int amount) {
		hp = Mathf.Max (0, hp - amount);
		OnDamageTaken ();
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
