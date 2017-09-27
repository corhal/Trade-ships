using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PortData {

	public int Level;
	public string Name;
	public List<int> ShipmentsCapacities;
	public List<Shipment> Shipments;
	
	public bool UnderConstruction;

	public List<Dictionary<Item, int>> BuildCosts;
	public List<int> UpgradeCosts;	
	
	public void InitializeFromPort (Port port) {
		Level = port.Level;
		Name = port.Name;
		ShipmentsCapacities = new List<int> (port.ShipmentsCapacities);
		Shipments = new List<Shipment> (port.Shipments);
		UnderConstruction = port.UnderConstruction;
		BuildCosts = new List<Dictionary<Item, int>> (port.BuildCosts); // potentially dangerous
		UpgradeCosts = new List<int> (port.UpgradeCosts);
	}
	
}
