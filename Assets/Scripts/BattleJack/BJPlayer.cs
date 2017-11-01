using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJPlayer : MonoBehaviour {

	public List<BJShip> Ships;

	public static BJPlayer Instance;

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
		Ships.Add (new BJShip (1000, 100));
	}
}
