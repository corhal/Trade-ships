using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyShip : MonoBehaviour {

	BattleShip battleship;

	void Awake () {
		battleship = gameObject.GetComponent<BattleShip> ();
	}

	void Start () {
		battleship.SetMaxHP (battleship.MaxHP);
	}
}
