using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	public List<Shipment> Shipments;
	public int ShipmentsCapacity;

	void Start () {
		Shipments = new List<Shipment> ();
	}

	public void TakeShipment (Shipment shipment) {
		if (Shipments.Count < ShipmentsCapacity) {
			Shipments.Add (shipment);
		}
	}

	public void GiveShipment (Shipment shipment) {
		Shipments.Remove (shipment);
	}
}
