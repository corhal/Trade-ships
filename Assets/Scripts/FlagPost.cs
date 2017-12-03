using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagPost : MonoBehaviour {

	public SpriteRenderer Flag;

	public Sprite PlayerFlag;
	public Sprite EnemyFlag;

	int moveCounter = 0;
	bool shouldMove;
	Vector3 destination;

	void Update () {
		if (shouldMove) { 
			float tempSpeed = 1.0f;
			float step = tempSpeed * Time.deltaTime;
			Flag.gameObject.transform.position = Vector3.MoveTowards (Flag.gameObject.transform.position, destination, step);
			if (Vector2.Distance (Flag.gameObject.transform.position, destination) < 0.01f) {
				if (moveCounter == 0) {
					Flag.sprite = PlayerFlag;
					moveCounter++;
					destination = new Vector3 (Flag.gameObject.transform.position.x, Flag.gameObject.transform.position.y + 0.5f, Flag.gameObject.transform.position.z);
				} else {
					shouldMove = false;
				}
			}
		}
	}

	public void ChangeFlag () {
		destination = new Vector3 (Flag.gameObject.transform.position.x, Flag.gameObject.transform.position.y - 0.5f, Flag.gameObject.transform.position.z);
		shouldMove = true;
	}
}
