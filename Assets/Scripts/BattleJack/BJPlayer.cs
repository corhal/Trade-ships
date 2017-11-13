using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJPlayer : MonoBehaviour {

	int hp;
	public int HP { get { return hp; } }
	int maxhp;
	public int MaxHP { get { return maxhp; } }

	public List<BJCreature> Creatures;
	public BJDataBase DataBase;

	public static BJPlayer Instance;

	public event TakeDamageEventHandler OnDamageTaken;

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
		//DontDestroyOnLoad(gameObject);
	}

	void Start () {
		Creatures = new List<BJCreature> ();
		if (Player.Instance != null) {
			foreach (var shipData in Player.Instance.CurrentTeam) {
				Creatures.Add (new BJCreature (shipData.Name, shipData.MaxHP, shipData.Power, 2, Random.Range(1, 7), Allegiance.Player, AttackType.Melee, new List<string>{"Melee attack"}));
			}
		} else {
			Creatures = new List<BJCreature> (DataBase.Creatures);
			Creatures.RemoveAt (0);
		}

		hp = 0;
		maxhp = hp;
	}

	public void TakeDamage (int amount) {
		hp = Mathf.Max (0, hp - amount);
		OnDamageTaken ();
	}
}
