using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnClick : MonoBehaviour {

	public bool InMoveMode = false;
	bool shouldMove = false;

	public float Speed = 1.0F;
	float startTime;
	float journeyLength;
	Vector2 start;
	Vector2 target;
	float initialZ;

	void Start () {
		initialZ = transform.position.z;
	}

	void Update () {	
		if (!Utility.IsPointerOverUIObject()) {
			if (Input.GetMouseButtonDown(0) && InMoveMode) {		
				start = transform.position;
				target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (hit.collider != null) {					
					if (hit.collider.gameObject.GetComponent<Port> () != null) {
						target = hit.collider.gameObject.transform.position;
						target = start + (target - start) * 0.8f;
					}
				}

				GameManager.Instance.InMoveMode = false;
				//GameManager.Instance.CloseContextButtons ();
				shouldMove = true;
				InMoveMode = false;
				startTime = Time.time;

				journeyLength = Vector2.Distance(start, target);
			}

			if (shouldMove) {
				float distCovered = (Time.time - startTime) * Speed;
				float fracJourney = distCovered / journeyLength;
				transform.position = Vector2.Lerp(start, target, fracJourney);
				transform.position = new Vector3 (transform.position.x, transform.position.y, initialZ);
				if (fracJourney == 1.0f) {
					shouldMove = false;
				}
			}
		}
	}
}
