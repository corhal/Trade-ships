using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {
	//public bool UnderConstruction;
	bool initialized;
	//public List<Dictionary<string, int>> BuildCosts;
	//public List<int> UpgradeCosts;
	public Island MyIsland;
	//Action buildAction;
	//Action upgradeAction;
	//public int Level = 1;
	public string Name;
	public Allegiance Allegiance;

	public SelectableTile Tile;
	public List<SelectableTile> AdjacentTiles;
	public float AdjacentRadius;

	protected virtual void Awake () {
		// base.Awake ();
		MyIsland = GetComponentInParent<Island> ();
		if (MyIsland != null) {
			MyIsland.Buildings.Add (this);
		}
		//BuildCosts = new List<Dictionary<string, int>> ();
		Board.OnBoardGenerationFinished += Board_OnBoardGenerationFinished;
	}

	void Board_OnBoardGenerationFinished () {
		FindTiles ();
	}

	protected virtual void Start () {	
		
		RefreshActions ();

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
		//Level = buildingData.Level;
		Name = buildingData.Name;
		Allegiance = buildingData.Allegiance;
		//UnderConstruction = buildingData.UnderConstruction;
		//BuildCosts = new List<Dictionary<string, int>> (buildingData.BuildCosts); // potentially dangerous
		//UpgradeCosts = new List<int> (buildingData.UpgradeCosts);
		initialized = true;
	}

	protected virtual void RefreshActions () {
		
	}

	void ShowCraftWindow () {
		
	}

	public void Upgrade () {
		
	}

	public void Build () {
		
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
