using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Current : PointOfInterest {

	public SelectableTile Target;
	public PlayerShip CaughtPlayerShip;
	public SpriteRenderer CurrentSprite;

	void Start () {
		int index = 0;
		if (POIData.CurrentDirection == "") {
			int loopCount = 5;
			do {
				index = Random.Range (0, Tile.Neighbors.Count);
				loopCount--;
				if (loopCount <= 0) {
					break;
				}
			} while (Tile.Neighbors [index].PointOfInterest == POIkind.Current &&
			         Tile.Neighbors [index].GetComponentInChildren<Current> ().Target == this.Tile);
		} else {			
			for (int i = 0; i < Tile.Neighbors.Count; i++) {
				if ((Tile.Neighbors [i].AbsBoardCoords.x > Tile.AbsBoardCoords.x && POIData.CurrentDirection == "right") ||
					(Tile.Neighbors [i].AbsBoardCoords.x < Tile.AbsBoardCoords.x && POIData.CurrentDirection == "left") ||
					(Tile.Neighbors [i].AbsBoardCoords.y > Tile.AbsBoardCoords.y && POIData.CurrentDirection == "up") ||
					(Tile.Neighbors [i].AbsBoardCoords.y < Tile.AbsBoardCoords.y && POIData.CurrentDirection == "down")) {
					index = i;
				}
			}
		}

		Target = Tile.Neighbors [index];
		RotateToTarget (Target);
	}

	void RotateToTarget (SelectableTile target) {
		if (target.AbsBoardCoords.x > Tile.AbsBoardCoords.x) {
			CurrentSprite.gameObject.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
			POIData.CurrentDirection = "right";
		}
		if (target.AbsBoardCoords.x < Tile.AbsBoardCoords.x) {
			CurrentSprite.gameObject.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 180.0f);
			POIData.CurrentDirection = "left";
		}
		if (target.AbsBoardCoords.y > Tile.AbsBoardCoords.y) {
			CurrentSprite.gameObject.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 90.0f);
			POIData.CurrentDirection = "up";
		}
		if (target.AbsBoardCoords.y < Tile.AbsBoardCoords.y) {
			CurrentSprite.gameObject.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 270.0f);
			POIData.CurrentDirection = "down";
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.GetComponent<PlayerShip>() != null) {
			CaughtPlayerShip = other.gameObject.GetComponent<PlayerShip> ();
			Interact ();
		}
	}

	public override void Interact () {
		if (!(POIData.OneTime && POIData.Interacted)) {
			base.Interact ();
			Invoke ("MovePlayerShip", 0.35f);
		}
	}

	void MovePlayerShip () {
		Target.StopParticles ();
		CaughtPlayerShip.MoveToTile (Target, false);
	}
}
