using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {

	Player player;
	public static PlayerShip Instance;
	MoveOnClick mover;

	public int EnergyPerDistance;

	public Collider2D lastSeenCollider;

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
		player = Player.Instance;
		mover = GetComponent<MoveOnClick> ();
		mover.OnFinishedMoving += Mover_OnFinishedMoving;
	}

	void Mover_OnFinishedMoving (MoveOnClick sender) {
		if (lastSeenCollider == null) {
			return;
		}
		if (lastSeenCollider.gameObject.GetComponent<Shipwreck> () != null) {
			Dictionary<string, int> rewards = new Dictionary<string, int> ();
			foreach (var rewardChest in lastSeenCollider.gameObject.GetComponent<Shipwreck> ().RewardChests) {
				player.TakeItems (rewardChest.RewardItems);
				foreach (var amountByItem in rewardChest.RewardItems) {
					if (!rewards.ContainsKey(amountByItem.Key)) {
						rewards.Add (amountByItem.Key, amountByItem.Value);
					} else {
						rewards [amountByItem.Key] += amountByItem.Value;
					}
				}
			}
			UIOverlay.Instance.OpenImagesPopUp ("Reward: ", rewards);
			Destroy (lastSeenCollider.gameObject);
		} else if (lastSeenCollider.gameObject.GetComponent<MissionObject> () != null) {
			UIOverlay.Instance.OpenMissionWindow (lastSeenCollider.gameObject.GetComponent<MissionObject> ().Mission);
		}
	}

	void Start () {
		mover.EnergyPerDistance = EnergyPerDistance;
	}

	void OnTriggerEnter2D (Collider2D other) { // will work even when passing through
		if (other.GetComponent<SelectableTile> () == null && other.GetComponentInParent<SelectableTile> () == null && other.GetComponent<Port> () == null) {
			lastSeenCollider = other;
		}
		if (other.GetComponentInParent<SelectableTile> () != null) {
			other.GetComponentInParent<SelectableTile> ().StopParticles ();
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (lastSeenCollider == other) {
			lastSeenCollider = null;
		}
	}

	public void MoveToPoint (Vector2 target) {
		mover.MoveToPoint (target);
	}

	void OnDestroy () {
		mover.OnFinishedMoving -= Mover_OnFinishedMoving;
	}
}
