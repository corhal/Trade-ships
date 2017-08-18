using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnClick : MonoBehaviour {

	bool shouldMove = false;

	public float Speed = 1.0F;
	float startTime;
	float journeyLength;
	Vector2 start;
	Vector2 target;

	void Update() {	
		if (!Utility.IsPointerOverUIObject()) {
			if (Input.GetMouseButtonDown(0)) {
				shouldMove = true;
				startTime = Time.time;
				start = transform.position;
				target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				journeyLength = Vector2.Distance(start, target);
			}

			if (shouldMove) {
				float distCovered = (Time.time - startTime) * Speed;
				float fracJourney = distCovered / journeyLength;
				transform.position = Vector2.Lerp(start, target, fracJourney);
				if (fracJourney == 1.0f) {
					shouldMove = false;
				}
			}
		}
	}
}
