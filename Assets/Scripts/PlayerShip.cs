using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {

	Player player;
	public static PlayerShip Instance;
	MoveOnClick mover;

	public int EnergyPerDistance;

	public Collider2D lastSeenCollider;

	public int RewardChestsCapacity;
	public List<RewardChest> RewardChests;

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
		player = Player.Instance;
		mover = GetComponent<MoveOnClick> ();
		mover.OnFinishedMoving += Mover_OnFinishedMoving;
		RewardChests = new List<RewardChest> ();
	}

	void Mover_OnFinishedMoving (MoveOnClick sender) {
		if (lastSeenCollider == null) {
			return;
		}
		if (lastSeenCollider.gameObject.GetComponent<Shipwreck> () != null) {
			//Player.Instance.TakeItems (lastSeenCollider.gameObject.GetComponent<Shipwreck> ().RewardChest.RewardItems);
			//UIOverlay.Instance.OpenImagesPopUp ("Your reward:", lastSeenCollider.gameObject.GetComponent<Shipwreck> ().RewardChest.RewardItems);
			/*if (RewardChests.Count < RewardChestsCapacity) {
				RewardChests.Add (lastSeenCollider.gameObject.GetComponent<Shipwreck> ().RewardChest);
				lastSeenCollider.gameObject.GetComponent<Shipwreck> ().Interact ();
				lastSeenCollider.gameObject.SetActive (false);
				UIOverlay.Instance.UpdateShipRewardChests (this);
			} */
		} else if (lastSeenCollider.gameObject.GetComponent<MissionObject> () != null) {
			// UIOverlay.Instance.OpenMissionWindow (lastSeenCollider.gameObject.GetComponent<MissionObject> ().Mission);
		}
	}

	void Start () {
		mover.EnergyPerDistance = EnergyPerDistance;
	}

	void OnTriggerEnter2D (Collider2D other) { // will work even when passing through
		if (other.GetComponent<SelectableTile> () == null && other.GetComponentInParent<SelectableTile> () == null) {
			lastSeenCollider = other;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (lastSeenCollider == other) {
			lastSeenCollider = null;
		}
	}

	public void MoveToPoint (Vector2 target, bool spendEnergy) {
		if (!spendEnergy) {
			mover.MoveToPoint (target);
			return;
		}
		if (Player.Instance.Energy >= EnergyPerDistance * 1) {
			Player.Instance.Energy -= EnergyPerDistance * 1;
			mover.MoveToPoint (target);
		} else {
			UIOverlay.Instance.OpenPopUp ("Not enough energy!");
		}

	}

	void OnDestroy () {
		mover.OnFinishedMoving -= Mover_OnFinishedMoving;
	}
}
