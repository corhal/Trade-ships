using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJMoveBetweenPoints : MonoBehaviour {

	public List<Transform> Points;
	public float Speed;

	bool shouldMove;
	float initialZ;
	Transform currentTarget;

	Vector3 initialPosition;

	bool firstTime = true;

	void OnEnable () {
		if (firstTime) {
			initialPosition = transform.position;
			initialZ = transform.position.z;
			firstTime = false;
		}

		transform.position = initialPosition;
		currentTarget = Points [0];

		shouldMove = true;
	}

	// Update is called once per frame
	void Update () {		
		if (shouldMove) {
			float step = Speed * Time.deltaTime;
			transform.position = Vector2.MoveTowards (transform.position, currentTarget.position, step);
			transform.position = new Vector3 (transform.position.x, transform.position.y, initialZ);

			if (Vector2.Distance (transform.position, currentTarget.position) < 0.0001f) {
				if (Points.IndexOf(currentTarget) == Points.Count - 1) {
					currentTarget = Points [0];
				} else {
					currentTarget = Points [Points.IndexOf(currentTarget) + 1];
				}
				if (Mathf.Abs(currentTarget.position.x - transform.position.x) < 0.01f && currentTarget.position.y < transform.position.y) {
					transform.localEulerAngles = new Vector3 (180.0f, 0.0f, 0.0f);
				} else if (Mathf.Abs(currentTarget.position.x - transform.position.x) < 0.01f && currentTarget.position.y > transform.position.y) {
					transform.localEulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
				} else if (Mathf.Abs(currentTarget.position.y - transform.position.y) < 0.01f && currentTarget.position.x < transform.position.x) {
					transform.localEulerAngles = new Vector3 (0.0f, 0.0f, 90.0f);
				} else if (Mathf.Abs(currentTarget.position.y - transform.position.y) < 0.01f && currentTarget.position.x > transform.position.x) {
					transform.localEulerAngles = new Vector3 (0.0f, 0.0f, 270.0f);
				}
			}
		}
	}

}
