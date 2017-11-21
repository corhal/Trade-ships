using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJMover : MonoBehaviour {

	Vector3 secondaryPosition;
	Vector3 targetPosition;
	public bool IsMoving;

	public float MoveSpeed = 10.0F;
	private float startTime;
	private float journeyLength;

	public void MoveToPoint (Vector3 target) {
		if (!IsMoving) {
			IsMoving = true;
			startTime = Time.time;
			secondaryPosition = transform.position;
			targetPosition = target;
			journeyLength = Vector3.Distance(secondaryPosition, targetPosition );
		}
	}

	public Color InitialColor;
	bool animate;
	void Update () {	
		if (IsMoving) {
			float distCovered = (Time.time - startTime) * MoveSpeed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(secondaryPosition, targetPosition, fracJourney);
			if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
				IsMoving = false;
			}
		}
	}
}
