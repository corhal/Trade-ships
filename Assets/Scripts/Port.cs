using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port : MonoBehaviour, ISelectable {

	List<Action> actions;
	public List<Action> Actions { get { return actions; } }

	public GameManager Manager;
	public List<Ship> DockedShips;
	Ship dockedShip;

	public string myname;
	public string Name { get { return myname; } }

	public int level;
	public int Level { get { return level; } }

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
		actions = new List<Action> ();
		Action showShipmentsAction = new Action ("Show shipments", 0, ShowShipments);
		actions.Add (showShipmentsAction);
	}

	void ShowShipments () {
		Manager.OpenPortWindow (this, dockedShip);
	}

	void OnMouseDown () {
		Manager.OpentContextButtons (this);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.GetComponent<Ship>() != null) {
			DockedShips.Add(other.gameObject.GetComponent<Ship> ());
			if (dockedShip == null) {
				dockedShip = DockedShips [0];
			}
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.GetComponent<Ship>() != null) {
			DockedShips.Remove(other.gameObject.GetComponent<Ship> ());
			if (DockedShips.Count == 0) {
				dockedShip = null;
			} else {
				dockedShip = DockedShips [0];
			}
		}
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
