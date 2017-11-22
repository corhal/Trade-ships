using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJFlyingText : MonoBehaviour {

	public Text Label;

	public float JourneyTime;
	float startTime;

	Vector3 startPosition;
	Vector3 targetPosition;

	void Start () {
		startTime = Time.time;
		startPosition = transform.position;
		targetPosition = new Vector3 (startPosition.x, startPosition.y + 0.5f, startPosition.y);
	}

	void Update () {		
		float dTime = (Time.time - startTime) / JourneyTime;			
		transform.position = Vector3.Lerp(startPosition, targetPosition, dTime);
		if (Vector3.Distance (transform.position, targetPosition) < 0.01f) {
			Destroy (this.gameObject);				
		}
	}
}
