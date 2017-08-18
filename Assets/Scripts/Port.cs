using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port : MonoBehaviour {

	public GameManager Manager;
	public Ship DockedShip;

	public string Name;
	public Location MyLocation;
	public string GoodsName;
	public int ShipmentsCapacity;
	public List<Shipment> Shipments;

	public float SecPerShipment;
	float timer;
	bool shouldProduceShipments;

	public delegate void ProducedShipmentEventHandler (Port sender, Shipment shipment);
	public event ProducedShipmentEventHandler OnProducedShipment;

	void Start () {
		Shipments = new List<Shipment> ();
	}

	void Update () {
		if (Shipments.Count < ShipmentsCapacity) {
			if (!shouldProduceShipments) {
				timer = 0.0f;
				shouldProduceShipments = true;
			}
			timer += Time.deltaTime;
			if (timer >= SecPerShipment) {
				timer = 0.0f;
				ProduceShipment ();
			}
			if (Shipments.Count == ShipmentsCapacity) {
				timer = 0.0f;
				shouldProduceShipments = false;
			}
		}
	}

	void OnMouseDown () {
		Manager.OpenPortWindow (this, DockedShip);
	}

	void ProduceShipment () {
		Location location = RandomLocation ();
		int reward = Random.Range (5, 36);
		Shipment shipment = new Shipment (GoodsName, MyLocation, location, reward);
		Shipments.Add (shipment);
		if (OnProducedShipment != null) {
			OnProducedShipment (this, shipment);
		}
	}

	public void TakeShipment (Shipment shipment) {
		if (Shipments.Count < ShipmentsCapacity) {
			Shipments.Add (shipment);
		}
	}

	public void GiveShipment (Shipment shipment) {
		Shipments.Remove (shipment);
	}

	Location RandomLocation () {
		List<Location> validLocations = new List<Location> ();
		foreach (var location in Manager.Locations) {
			if (location.MyPort != null && location != MyLocation) {
				validLocations.Add (location);
			}
		}
		int index = Random.Range (0, validLocations.Count);
		return validLocations [index];
	}


}
