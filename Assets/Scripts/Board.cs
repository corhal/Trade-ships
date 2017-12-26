using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum POIkind {
	None, Portal, Altar, Mission, Chest, Current
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

	public static Board Instance;

	public SelectableTile[,] Tiles;

	void Awake () {
		Instance = this;
	}

	void Start () {
		PointsOfInterestAmount = new Dictionary<POIkind, int> ();
		Tiles = new SelectableTile[PosWidth - NegWidth, PosHeight - NegHeight];
		for (int i = 0; i < POIS.Count; i++) {
			PointsOfInterestAmount.Add (POIS [i], POIamounts [i]);
		}
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
				if (Player.Instance.NewBoard && Random.Range(0.0f, 1.0f) > 0.7f) {
					POIkind poi = POIkind.Portal;
					while (PointsOfInterestAmount [poi] == 0) {
						poi = Utility.RandomEnumValue <POIkind> ();
					}
					PointsOfInterestAmount [poi] -= 1;
					tile.GetComponent<SelectableTile> ().PointOfInterest = poi;

					SpawnPOI (tile.GetComponent<SelectableTile> ());
				} else if (Player.Instance.POIDataByTiles.ContainsKey(i + ":" + j) && Player.Instance.POIDataByTiles[(i + ":" + j)].POIkind != POIkind.None) {
					POIkind poi = Player.Instance.POIDataByTiles [(i + ":" + j)].POIkind;
					tile.GetComponent<SelectableTile> ().PointOfInterest = poi;
					SpawnPOI (tile.GetComponent<SelectableTile> ());
				}
				Tiles[i, j] = tile.GetComponent<SelectableTile> ();
			}
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
		if (Tiles[tile.BoardCoords.x + 1, tile.BoardCoords.y] != null) { // not null, doesn't exits, rewrite
			
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
			default:
				prefabObject = null;
				break;
		}
		if (prefabObject != null) {
			GameObject poiOBject = Instantiate (prefabObject) as GameObject;
			poiOBject.transform.position = new Vector3 (tile.transform.position.x, tile.transform.position.y, 0);
			if (!Player.Instance.POIDataByTiles.ContainsKey (tile.BoardCoordsAsString)) {
				POIData poiData = poiOBject.GetComponentInChildren<PointOfInterest> ().POIData;
				Player.Instance.POIDataByTiles.Add (tile.BoardCoordsAsString, poiData);
				Player.Instance.POIDatas.Add (poiData);
			} else {
				poiOBject.GetComponentInChildren<PointOfInterest> ().POIData = Player.Instance.POIDataByTiles [tile.BoardCoordsAsString];
			}

			if (poiOBject.GetComponentInChildren<PointOfInterest> ().POIData.POIkind == POIkind.Current) {
				
			}
		}
	}
}
