using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TradeShipData {	

	public List<Shipment> Shipments;

	public string Name;
	public string Allegiance;
	public int ShipmentsCapacity;
	public float[] Coordinates;


	public TradeShipData () {

	}

	public TradeShipData (string name, string allegiance, int shipmentsCapacity, float[] coordinates, List<Shipment> shipments) {
		Name = name;
		Allegiance = allegiance;
		ShipmentsCapacity = shipmentsCapacity;
		Coordinates = new float[coordinates.Length];
		coordinates.CopyTo (Coordinates, 0);
		if (shipments != null) {
			Shipments = new List<Shipment> (shipments);
		} else {
			Shipments = new List<Shipment> ();
		}
	}

	public int TotalWeight { get {
			int totalWeight = 0;
			foreach (var myShipment in Shipments) {
				totalWeight += myShipment.Cargo;
			}
			return totalWeight;
		}}

	public void TakeShipment (Shipment shipment) {		
		if (ShipmentsCapacity - TotalWeight >= shipment.Cargo) {
			Shipments.Add (shipment);
		}
	}

	public bool CanTakeShipment (Shipment shipment) {
		if (ShipmentsCapacity - TotalWeight >= shipment.Cargo) {
			return true;
		} else {
			return false;
		}
	}

	public void GiveShipment (Shipment shipment) {		
		Shipments.Remove (shipment);
	}

}
