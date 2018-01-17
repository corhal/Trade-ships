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
			float diceRoll = Random.Range (0.0f, 1.0f);
			//if (diceRoll > 0.5f) {
				Player.Instance.Energy += 50;
				UIOverlay.Instance.OpenPopUp ("This altar gives you 50 energy!");
			/*} else {
				foreach (var creatureData in Player.Instance.CurrentTeam) {
					creatureData.Creature.Heal (20);
				}
				UIOverlay.Instance.OpenPopUp ("This altar heals your current team by 20!");
			}*/
		}
	}
}
