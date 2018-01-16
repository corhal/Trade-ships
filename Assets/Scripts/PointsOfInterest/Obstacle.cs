using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : PointOfInterest {

	public int AdditionalRequiredEnergy;
	/*public int Tries;
	public RewardChest RewardChest;*/

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
			//UIOverlay.Instance.OpenObstaclePopUp (this);
			Player.Instance.Energy = Mathf.Max(0, Player.Instance.Energy - AdditionalRequiredEnergy);
		}
	}

	/*public void GiveReward () {		
		Player.Instance.TakeItems (RewardChest.RewardItems);
		UIOverlay.Instance.OpenImagesPopUp ("Your reward:", RewardChest.RewardItems);
	}*/
}
