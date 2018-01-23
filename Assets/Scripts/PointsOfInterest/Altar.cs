using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : PointOfInterest {
	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.GetComponent<PlayerShip>() != null) {
			Interact ();
		}
	}

	public override void Interact () {
		if (!(POIData.OneTime && POIData.Interacted)) {
			base.Interact ();

			Player.Instance.Energy += 20;
			UIOverlay.Instance.OpenPopUp ("This altar gives you 20 energy!");
		}
	}
}
