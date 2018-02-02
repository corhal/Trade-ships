using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Adventure {
	public int PosWidth;
	public int NegWidth;
	public int PosHeight;
	public int NegHeight;

	public List<POIkind> POIs;
	public List<int> POIamounts;

	public float TimeLimit;
	public bool TreasureHunt;

	public int MapsForTreasure;
	public string Ocean;
}
