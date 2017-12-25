using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : PointOfInterest {
	
	void Start () {
		POIkind = POIkind.Portal;
	}

	void OnTriggerEnter2D (Collider2D other) {
		UIOverlay.Instance.OpenAdventureSelectionWindow ();
	}
}
