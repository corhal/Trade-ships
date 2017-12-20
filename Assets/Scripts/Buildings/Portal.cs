using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Building {

	void OnTriggerEnter2D (Collider2D other) {
		UIOverlay.Instance.OpenAdventureSelectionWindow ();
	}
}
