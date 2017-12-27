using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Current : PointOfInterest {

	public SelectableTile Target;
	public PlayerShip CaughtPlayerShip;

	void Start () {
		int index = Random.Range (0, Tile.Neighbors.Count);
		Target = Tile.Neighbors [index];
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
