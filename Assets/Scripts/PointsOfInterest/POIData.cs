using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class POIData {

	public bool OneTime;
	public bool Interacted;
	public POIkind POIkind;

	public string CurrentDirection;
	public bool Revealed;

	/*public void InitializeFromPOI (PointOfInterest poi) {
		OneTime = poi.OneTime;
		Interacted = poi.Interacted;
		POIkind = poi.POIkind;
	}*/
}
