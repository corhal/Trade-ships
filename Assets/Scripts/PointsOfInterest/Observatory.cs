using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observatory : PointOfInterest {

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.GetComponent<PlayerShip>() != null) {
			Interact ();
		}
	}

	public override void Interact () {
		if (!(POIData.OneTime && POIData.Interacted)) {
			base.Interact ();
			foreach (var tile in AdjacentTiles) {
				tile.StopParticles ();
			}
		}
	}
}
