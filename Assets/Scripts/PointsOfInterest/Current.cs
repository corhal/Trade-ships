using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Current : PointOfInterest {

	public SelectableTile Target;
	public PlayerShip CaughtPlayerShip;
	public SpriteRenderer CurrentSprite;

	void Start () {
		int index = Random.Range (0, Tile.Neighbors.Count);
		Target = Tile.Neighbors [index];
		if (Target.AbsBoardCoords.x > Tile.AbsBoardCoords.x) {
			CurrentSprite.gameObject.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
		}
		if (Target.AbsBoardCoords.x < Tile.AbsBoardCoords.x) {
			CurrentSprite.gameObject.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 180.0f);
		}
		if (Target.AbsBoardCoords.y > Tile.AbsBoardCoords.y) {
			CurrentSprite.gameObject.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 90.0f);
		}
		if (Target.AbsBoardCoords.y < Tile.AbsBoardCoords.y) {
			CurrentSprite.gameObject.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 270.0f);
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
			Target.StopParticles ();
			CaughtPlayerShip.MoveToPoint (Target.gameObject.transform.position, false);
		}
	}
}
