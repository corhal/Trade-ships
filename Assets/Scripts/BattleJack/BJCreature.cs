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

	string creatureName;
	public string Name { get { return creatureName; } }

	int hp;
	public int HP { get { return hp; } }
	int maxhp;
	public int MaxHP { get { return maxhp; } }
	int baseDamage;
	public int BaseDamage { get { return baseDamage; } }

	int armor;
	public int Armor { get { return armor; } set { armor = value; } }

	int speed;
	public int Speed { get { return speed; } set { speed = value; } }

	float precision;
	public float Precision { get { return precision; } set { precision = value; } }

	float dodge;
	public float Dodge { get { return dodge; } set { dodge = value; } }

	AttackType attackType;
	public AttackType AttackType { get { return attackType; } }

	Allegiance allegiance;
	public Allegiance Allegiance { get { return allegiance; } }

	public List<string> SkillNames;

	public BJCreature (string name, int maxhp, int baseDamage, int armor, int speed, Allegiance allegiance, AttackType attackType, List<string> skillNames) {
		this.creatureName = name;
		this.maxhp = maxhp;
		this.hp = maxhp;
		this.baseDamage = baseDamage;
		this.armor = armor;
		this.speed = speed;
		this.allegiance = allegiance;
		this.attackType = attackType;
		this.SkillNames = new List<string> (skillNames);
		this.dodge = 0.0f;
		this.precision = 1.0f;
	}

	// For positive Armor, damage reduction =((armor)*0.06)/(1+0.06*(armor))
	// For negative Armor, it is damage increase = 2-0.94^(-armor) since you take more damage for negative armor scores.
	// A negative armor of 10 increases damage by 46.1%

	public void TakeDamage (int amount) {
		float diceRoll = Random.Range (0.0f, 0.99f);
		if (dodge < diceRoll) {
			float armorCoef = 1.0f;
			if (armor >= 0) {
				armorCoef = 1.0f - ((float)armor * 0.06f) / (1.0f + 0.06f * (float)armor);
			} else {
				armorCoef = 2.0f - Mathf.Pow(0.94f, (float)armor);
			}
			float damage = (float)amount * armorCoef;
			int intDamage = (int)damage;
			hp = Mathf.Max (0, hp - intDamage);
			if (OnDamageTaken != null) {
				OnDamageTaken ();
			}
		} else {
			Debug.Log ("Dodge!");
		}
	}

	public void DealDamage (int damage, float multiplier, BJCreature enemy) {
		float diceRoll = Random.Range (0.0f, 0.99f);
		if (precision > diceRoll) {
			damage = (int)(damage * multiplier);
			enemy.TakeDamage (damage);
		} else {
			Debug.Log ("Miss!");
		}
	}
}
