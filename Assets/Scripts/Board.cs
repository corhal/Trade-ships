using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public List<PointOfInterest> POIS;
	public List<int> POIamounts;

	public Dictionary<PointOfInterest, int> PointsOfInterestAmount;

	void Start () {
		PointsOfInterestAmount = new Dictionary<PointOfInterest, int> ();
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
					PointOfInterest poi = PointOfInterest.Portal;
					while (PointsOfInterestAmount [poi] == 0) {
						poi = Utility.RandomEnumValue <PointOfInterest> ();
					}
					PointsOfInterestAmount [poi] -= 1;
					tile.GetComponent<SelectableTile> ().PointOfInterest = poi;
				}
			}
		}
		if (Player.Instance.NewBoard) {
			Player.Instance.NewBoard = false;
		}

		if (OnBoardGenerationFinished != null) {
			OnBoardGenerationFinished ();
		}
	}
}
