using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public int Gold;

	public static Player Instance;

	void Awake () {
		Instance = this;
	}

	public void TakeGold (int amount) {
		Gold += amount;
	}

	public void GiveGold (int amount) {
		if (Gold >= amount) {
			Gold -= amount;
		} else {
			Debug.Log ("Not enough gold");
		}
	}
}
