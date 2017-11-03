using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJPlayer : MonoBehaviour {

	int hp;
	public int HP { get { return hp; } }
	int maxhp;
	public int MaxHP { get { return maxhp; } }

	public List<BJShip> Ships;

	public static BJPlayer Instance;

	public event TakeDamageEventHandler OnDamageTaken;

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
		DontDestroyOnLoad(gameObject);
	}

	void Start () {
		Ships = new List<BJShip> ();
		if (Player.Instance != null) {
			foreach (var shipData in Player.Instance.CurrentTeam) {
				Ships.Add (new BJShip (shipData.MaxHP, shipData.Power));
			}
		} else {
			Ships.Add (new BJShip (1000, 100));
			Ships.Add (new BJShip (1000, 100));
		}

		hp = 0;
		foreach (var ship in Ships) {
			hp += ship.HP;
		}
		maxhp = hp;
	}

	public void TakeDamage (int amount) {
		hp = Mathf.Max (0, hp - amount);
		OnDamageTaken ();
	}
}
