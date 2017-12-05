using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TakeDamageEventHandler (int amount);
public enum Allegiance {
	Player, Enemy, Neutral
}

/*public enum AttackType {
	Melee, Ranged
}*/

[System.Serializable]
public class BJCreature {
	public event TakeDamageEventHandler OnDamageTaken;
	public event TakeDamageEventHandler OnDodge;
	public event TakeDamageEventHandler OnMiss;

	public delegate void CreatureDiedEventHandler (BJCreature sender);
	public event CreatureDiedEventHandler OnCreatureDied;

	string creatureName;
	public string Name { get { return creatureName; } }

	int hp;
	public int HP { get { return hp; } set { hp = value; } }
	int maxhp;
	public int MaxHP { get { return maxhp; } }
	int baseDamage;
	public int BaseDamage { get { return baseDamage; } set { baseDamage = value; } }

	int armor;
	public int Armor { get { return armor; } set { armor = value; } }

	int armorPierce;
	public int ArmorPierce { get { return armorPierce; } set { armorPierce = value; } }

	int speed;
	public int Speed { get { return speed; } set { speed = value; } }

	float precision;
	public float Precision { get { return precision; } set { precision = value; } }

	float dodge;
	public float Dodge { get { return dodge; } set { dodge = value; } }

	/*AttackType attackType;
	public AttackType AttackType { get { return attackType; } }*/

	Allegiance allegiance;
	public Allegiance Allegiance { get { return allegiance; } }

	public bool IsDead;

	public List<string> SkillNames;

	public BJCreature (string name, int maxhp, int hp, int baseDamage, int armor, int speed, Allegiance allegiance, /*AttackType attackType,*/ List<string> skillNames) {
		this.creatureName = name;
		this.maxhp = maxhp;
		this.hp = hp;
		this.baseDamage = baseDamage;
		this.armor = armor;
		this.armorPierce = 0;
		this.speed = speed;
		this.allegiance = allegiance;
		//this.attackType = attackType;
		this.SkillNames = new List<string> (skillNames);
		this.dodge = 0.0f;
		this.precision = 1.0f;
	}

	// For positive Armor, damage reduction =((armor)*0.06)/(1+0.06*(armor))
	// For negative Armor, it is damage increase = 2-0.94^(-armor) since you take more damage for negative armor scores.
	// A negative armor of 10 increases damage by 46.1%

	public void Heal (int amount) {
		hp = Mathf.Min (maxhp, hp + amount);
		if (OnDamageTaken != null) {
			OnDamageTaken (-amount);
		}
	}

	public void Resurrect () {
		IsDead = false;
		Heal (maxhp / 2);
	}

	public void TakeDamage (int amount, int armorPierce) {
		float diceRoll = Random.Range (0.0f, 0.99f);
		if (dodge < diceRoll) {
			float armorCoef = 1.0f;
			int currentArmor = armor - armorPierce;
			if (currentArmor >= 0) {
				armorCoef = 1.0f - ((float)currentArmor * 0.06f) / (1.0f + 0.06f * (float)currentArmor);
			} else {
				armorCoef = 2.0f - Mathf.Pow(0.94f, -(float)currentArmor);
			}
			float damage = (float)amount * armorCoef;
			int intDamage = (int)damage;
			hp = Mathf.Max (0, hp - intDamage);
			if (OnDamageTaken != null) {
				OnDamageTaken (intDamage);
			}
			if (hp <= 0 && OnCreatureDied != null) {
				OnCreatureDied (this);
			}
		} else {
			OnDodge (0);
			Debug.Log ("Dodge!");
		}
	}

	public void DealDamage (int damage, float multiplier, BJCreature enemy) {
		float diceRoll = Random.Range (0.0f, 0.99f);
		if (precision > diceRoll) {
			damage = (int)(damage * multiplier);
			enemy.TakeDamage (damage, armorPierce);
		} else {
			OnMiss (0);
			Debug.Log ("Miss!");
		}
	}
}
