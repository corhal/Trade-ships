using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingData {

	public string Name;

	public Allegiance Allegiance;
	public float[] Coordinates;

	public virtual void InitializeFromBuilding (Building building) {
		Name = building.Name;
		Allegiance = building.Allegiance;
		Coordinates = new float[3];
		Coordinates [0] = building.transform.position.x;
		Coordinates [1] = building.transform.position.y;
		Coordinates [2] = building.transform.position.z;
	}
}
