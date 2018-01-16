using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour {

	public SelectableTile Tile;
	public List<SelectableTile> AdjacentTiles;
	public float AdjacentRadius;
	public POIData POIData;

	public virtual void Interact () {
		POIData.Interacted = true;
		if (Tile == null) {
			FindTiles ();
		}
		Tile.StopParticles ();
		foreach (var tile in AdjacentTiles) {
			tile.StopParticles ();
		}
	}

	protected virtual void Awake () {		
		Board.OnBoardGenerationFinished += Board_OnBoardGenerationFinished;
	}

	void Board_OnBoardGenerationFinished () {
		FindTiles ();
	}
		
	public void FindTiles () {
		Collider2D[] otherColliders = Physics2D.OverlapCircleAll (transform.position, 0.1f);	
		foreach (var otherCollider in otherColliders) {
			if (otherCollider.gameObject.GetComponent<SelectableTile> () != null) {
				Tile = otherCollider.gameObject.GetComponent<SelectableTile> ();
			}
		}
		AdjacentTiles = new List<SelectableTile> ();
		otherColliders = Physics2D.OverlapCircleAll (transform.position, AdjacentRadius);	
		foreach (var otherCollider in otherColliders) {			
			if (otherCollider.gameObject.GetComponent<SelectableTile> () != null) {
				AdjacentTiles.Add (otherCollider.gameObject.GetComponent<SelectableTile> ());
			}
		}
	}

	void OnDestroy () {
		Board.OnBoardGenerationFinished -= Board_OnBoardGenerationFinished;
	}
}
