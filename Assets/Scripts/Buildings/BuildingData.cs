using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingData {

	public int Level;
	public string Name;

	public Allegiance Allegiance;
	public bool UnderConstruction;

	public List<Dictionary<string, int>> BuildCosts;
	public List<int> UpgradeCosts;	
	public float[] Coordinates;

	public virtual void InitializeFromBuilding (Building building) {
		Level = building.Level;
		Name = building.Name;
		Allegiance = building.Allegiance;
		UnderConstruction = building.UnderConstruction;
		BuildCosts = new List<Dictionary<string, int>> (building.BuildCosts); // potentially dangerous
		UpgradeCosts = new List<int> (building.UpgradeCosts);
		Coordinates = new float[3];
		Coordinates [0] = building.transform.position.x;
		Coordinates [1] = building.transform.position.y;
		Coordinates [2] = building.transform.position.z;
	}
}
