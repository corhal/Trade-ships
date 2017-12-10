using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeShipMover : MonoBehaviour {
	
	public Vector2 Destination;

	float initialZ;

	bool shouldMove;

	void Awake () {
		initialZ = transform.position.z;
	}
		

	void Start () {
		
	}

	public void MoveToPosition (Vector2 position) {
		Destination = position;
		shouldMove = true;
	}

	void Update () {
		if (Vector2.Distance (transform.position, Destination) >= 0.01f) {
			shouldMove = true;
		}
		if (shouldMove) { 
			float tempSpeed = 1.0f;
			float step = tempSpeed * Time.deltaTime;
			transform.position = Vector2.MoveTowards (transform.position, Destination, step);
			transform.position = new Vector3 (transform.position.x, transform.position.y, initialZ);
			if (Vector2.Distance (transform.position, Destination) < 0.01f) {
				shouldMove = false;
			}
		}
	}
}
