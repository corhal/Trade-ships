using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port : MonoBehaviour {

	public GameManager Manager;

	public string Name;
	public Location MyLocation;
	public string GoodsName;
	public int ShipmentsCapacity;
	public List<Shipment> Shipments;

	public float SecPerShipment;
	float timer;
	bool shouldProduceShipments;

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
		Manager.OpenPortWindow (this);
	}

	void ProduceShipment () {
		Location location = RandomLocation ();
		int reward = Random.Range (5, 36);
		Shipment shipment = new Shipment (GoodsName, MyLocation, location, reward);
		Shipments.Add (shipment);
		Debug.Log (Name + " now has " + Shipments.Count + " shipments.");
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
