using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum POIkind {
	None, Portal, Altar, Mission, Chest, Current, Obstacle
}

public class Board : MonoBehaviour {

	public GameObject TileContainer;
	public GameObject TilePrefab;
	public int PosWidth;
	public int NegWidth;
	public int PosHeight;
	public int NegHeight;

	public delegate void BoardGenerationFinished ();
	public static event BoardGenerationFinished OnBoardGenerationFinished; 

	public bool AllClear;

	public List<POIkind> POIS;
	public List<int> POIamounts;

	public Dictionary<POIkind, int> PointsOfInterestAmount;

	public GameObject CurrentPrefab;
	public GameObject PortalIslandPrefab;
	public GameObject AltarIslandPrefab;
	public GameObject MissionPrefab;
	public GameObject MissionIslandPrefab;
	public GameObject ChestPrefab;
	public GameObject ObstaclePrefab;

	public static Board Instance;

	public SelectableTile[,] Tiles;

	void Awake () {
		Instance = this;
	}

	public List<POIkind> POIKinds;

	void Start () {
		PointsOfInterestAmount = new Dictionary<POIkind, int> ();
		Tiles = new SelectableTile[PosWidth - NegWidth + 1, PosHeight - NegHeight + 1];

		for (int i = 0; i < POIS.Count; i++) {
			PointsOfInterestAmount.Add (POIS [i], POIamounts [i]);
		}
		List<POIkind> poiKinds = new List<POIkind> ();
		Dictionary<POIkind, int> tempPointsOfInterestAmount = new Dictionary<POIkind, int> (PointsOfInterestAmount);

		List<POIkind> keys = new List<POIkind> (tempPointsOfInterestAmount.Keys);
		foreach (var key in keys) {
			while (tempPointsOfInterestAmount[key] > 0) {
				poiKinds.Add (key);
				tempPointsOfInterestAmount [key]--;
			}
		}
		Debug.Log (poiKinds.Count + "");
		Utility.Shuffle (poiKinds);
		POIKinds = new List<POIkind> (poiKinds);
		int counter = 0;
		for (int i = NegWidth; i <= PosWidth; i++) {
			for (int j = NegHeight; j <= PosHeight; j++) {				
				GameObject tile = Instantiate (TilePrefab) as GameObject;
				tile.transform.SetParent (TileContainer.transform, false);
				float x = i * 1.223f;
				float y = j * 1.223f;
				float z = -3.0f;
				tile.transform.position = new Vector3 (x, y, z);
				tile.GetComponent<SelectableTile> ().BoardCoords = new Vector2Int (i, j);
				tile.GetComponent<SelectableTile> ().BoardCoordsAsString = i + ":" + j;
				if (!Player.Instance.Tiles.ContainsKey(i + ":" + j)) {
					Player.Instance.Tiles.Add (i + ":" + j, true);
				}
				if (!GameManager.Instance.Tiles.Contains(tile.GetComponent<SelectableTile> ())) {
					GameManager.Instance.Tiles.Add (tile.GetComponent<SelectableTile> ());
				}

				if (AllClear) {
					tile.GetComponent<SelectableTile> ().StopParticles ();
				}
				if (Player.Instance.NewBoard) {
					POIkind poi = poiKinds [counter]; 
					tile.GetComponent<SelectableTile> ().PointOfInterest = poi;

					SpawnPOI (tile.GetComponent<SelectableTile> ());
				} else if (Player.Instance.POIDataByTiles.ContainsKey(i + ":" + j) && Player.Instance.POIDataByTiles[(i + ":" + j)].POIkind != POIkind.None) {
					POIkind poi = Player.Instance.POIDataByTiles [(i + ":" + j)].POIkind;
					tile.GetComponent<SelectableTile> ().PointOfInterest = poi;
					SpawnPOI (tile.GetComponent<SelectableTile> ());
				} 
				Tiles[i - NegWidth, j - NegHeight] = tile.GetComponent<SelectableTile> ();
				tile.GetComponent<SelectableTile> ().AbsBoardCoords = new Vector2Int (i - NegWidth, j - NegHeight);
				counter++;
			}
		}
		foreach (var tile in Tiles) {
			tile.Neighbors = GetTileNeighbors (tile);
		}
		if (Player.Instance.NewBoard) {
			Player.Instance.NewBoard = false;
		}

		if (OnBoardGenerationFinished != null) {
			OnBoardGenerationFinished ();
		}
	}

	public List<SelectableTile> GetTileNeighbors (SelectableTile tile) {
		List<SelectableTile> neighbors = new List<SelectableTile> ();
		if (tile.AbsBoardCoords.x + 1 < Tiles.GetLength (0)) {
			neighbors.Add (Tiles [tile.AbsBoardCoords.x + 1, tile.AbsBoardCoords.y]);
		}
		if (tile.AbsBoardCoords.x - 1 >= 0) {
			neighbors.Add (Tiles [tile.AbsBoardCoords.x - 1, tile.AbsBoardCoords.y]);
		}
		if (tile.AbsBoardCoords.y + 1 < Tiles.GetLength (0)) {
			neighbors.Add (Tiles [tile.AbsBoardCoords.x, tile.AbsBoardCoords.y + 1]);
		}
		if (tile.AbsBoardCoords.y - 1 >= 0) {
			neighbors.Add (Tiles [tile.AbsBoardCoords.x, tile.AbsBoardCoords.y - 1]);
		}
		return neighbors;
	}

	void SpawnPOI (SelectableTile tile) {
		GameObject prefabObject;
		switch (tile.PointOfInterest) {
			case POIkind.Altar:
				prefabObject = AltarIslandPrefab;
				break;
			case POIkind.Portal:
				prefabObject = PortalIslandPrefab;
				break;
			case POIkind.Mission:
				prefabObject = MissionPrefab;
				break;
			case POIkind.Chest:
				prefabObject = ChestPrefab;
				break;
			case POIkind.Current:
				prefabObject = CurrentPrefab;
				break;
			case POIkind.Obstacle:
				prefabObject = ObstaclePrefab;
				break;
			default:
				prefabObject = null;
				break;
		}
		if (prefabObject != null) {
			GameObject poiOBject = Instantiate (prefabObject) as GameObject;
			poiOBject.transform.position = new Vector3 (tile.transform.position.x, tile.transform.position.y, 0);
			if (!Player.Instance.POIDataByTiles.ContainsKey (tile.BoardCoordsAsString)) {
				// Debug.Log (prefabObject);
				// Debug.Log (poiOBject.GetComponentInChildren<PointOfInterest> ());
				POIData poiData = poiOBject.GetComponentInChildren<PointOfInterest> ().POIData;
				Player.Instance.POIDataByTiles.Add (tile.BoardCoordsAsString, poiData);
				Player.Instance.POIDatas.Add (poiData);
			} else {
				poiOBject.GetComponentInChildren<PointOfInterest> ().POIData = Player.Instance.POIDataByTiles [tile.BoardCoordsAsString];
				if (poiOBject.GetComponentInChildren<PointOfInterest> ().POIData.POIkind == POIkind.Chest && poiOBject.GetComponentInChildren<PointOfInterest> ().POIData.Interacted) {
					poiOBject.SetActive (false);
				}
			}
		}
	}
}
