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

	public SelectableTile CurrentTile;

	public List<GameObject> Arrows;

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
	}

	void Start () {
		mover.EnergyPerDistance = EnergyPerDistance;

		Collider2D[] otherColliders = Physics2D.OverlapCircleAll (transform.position, 0.1f);
		foreach (var otherCollider in otherColliders) {
			if (otherCollider.gameObject.GetComponent<SelectableTile> () != null) {
				CurrentTile = otherCollider.gameObject.GetComponent<SelectableTile> ();
			}
		}
		HideArrows ();
		ShowArrows ();
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

	public void MoveToTile (SelectableTile tile, bool spendEnergy) {
		if (!spendEnergy) {
			mover.MoveToPoint (tile.transform.position);
			CurrentTile = tile;
			HideArrows ();
			Invoke ("ShowArrows", 1.5f);
			return;
		}
		if (Player.Instance.Energy >= EnergyPerDistance * 1) {
			Player.Instance.Energy -= EnergyPerDistance * 1;
			mover.MoveToPoint (tile.transform.position);
			CurrentTile = tile;
			HideArrows ();
			Invoke ("ShowArrows", 1.5f);
		} else {
			UIOverlay.Instance.OpenPopUp ("Not enough energy!");
		}
	}

	public void HideArrows () {
		foreach (var arrow in Arrows) {
			arrow.SetActive (false);
		}
	}

	public void ShowArrows () {		
		for (int i = 0; i < CurrentTile.Neighbors.Count; i++) {
			if (CurrentTile.Neighbors [i].AbsBoardCoords.x > CurrentTile.AbsBoardCoords.x) {
				Arrows [0].SetActive (true);
			}
			if (CurrentTile.Neighbors [i].AbsBoardCoords.y < CurrentTile.AbsBoardCoords.y) {
				Arrows [1].SetActive (true);
			}		
			if (CurrentTile.Neighbors [i].AbsBoardCoords.x < CurrentTile.AbsBoardCoords.x) {
				Arrows [2].SetActive (true);
			}
			if (CurrentTile.Neighbors [i].AbsBoardCoords.y > CurrentTile.AbsBoardCoords.y) {
				Arrows [3].SetActive (true);
			}
		}	
	}

	void OnDestroy () {
		mover.OnFinishedMoving -= Mover_OnFinishedMoving;
	}
}
