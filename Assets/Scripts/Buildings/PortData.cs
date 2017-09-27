using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PortData: BuildingData {

	public List<int> ShipmentsCapacities;
	public List<Shipment> Shipments;
	
	public override void InitializeFromBuilding (Building building) {
		base.InitializeFromBuilding (building);
		Port port = building as Port;
		ShipmentsCapacities = new List<int> (port.ShipmentsCapacities);
		Shipments = new List<Shipment> (port.Shipments);
	}
	
}
