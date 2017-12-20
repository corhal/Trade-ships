using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	bool initialized;
	public Island MyIsland;
	public string Name;
	public Allegiance Allegiance;

	public SelectableTile Tile;
	public List<SelectableTile> AdjacentTiles;
	public float AdjacentRadius;

	protected virtual void Awake () {		
		MyIsland = GetComponentInParent<Island> ();
		if (MyIsland != null) {
			MyIsland.Buildings.Add (this);
		}
		Board.OnBoardGenerationFinished += Board_OnBoardGenerationFinished;
	}

	void Board_OnBoardGenerationFinished () {
		FindTiles ();
	}

	protected virtual void Start () {		
		if (initialized) {
			return;
		}
	}

	void FindTiles () {
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

	public virtual void InitializeFromData (BuildingData buildingData) {		
		Name = buildingData.Name;
		Allegiance = buildingData.Allegiance;
		initialized = true;
	}

	public virtual void Claim () {
		Allegiance = Allegiance.Player;
		if (Tile == null) {
			FindTiles ();
		}
		Tile.StopParticles ();
		foreach (var tile in AdjacentTiles) {
			tile.StopParticles ();
		}
	}

	void OnDestroy () {
		Board.OnBoardGenerationFinished -= Board_OnBoardGenerationFinished;
	}
}
