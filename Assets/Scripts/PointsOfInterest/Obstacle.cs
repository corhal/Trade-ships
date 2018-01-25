using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : PointOfInterest {

	public int AdditionalRequiredEnergy;

	void Start () {
		
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.GetComponent<PlayerShip>() != null) {			
			Interact ();
		}
	}

	public override void Interact () {
		if (!(POIData.OneTime && POIData.Interacted)) {
			base.Interact ();
			PlayerShip.Instance.ShowFlyingText (("-" + AdditionalRequiredEnergy), Color.red);
			Player.Instance.Energy = Player.Instance.Energy - AdditionalRequiredEnergy;
		}
	}
}
