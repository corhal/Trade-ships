using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port : Building {
	public List<Ship> DockedShips;
	Ship dockedShip;

	public string GoodsName;
	public int ShipmentsCapacity;
	public List<Shipment> Shipments;

	public float SecPerShipment;
	float timer;
	bool shouldProduceShipments;

	public delegate void ProducedShipmentEventHandler (Port sender, Shipment shipment);
	public event ProducedShipmentEventHandler OnProducedShipment;

	new void Awake () {
		base.Awake ();
	}

	new void Start () {
		base.Start ();
		Shipments = new List<Shipment> ();
		Action showShipmentsAction = new Action ("Show shipments", 0, ShowShipments);
		actions.Add (showShipmentsAction);
	}

	void ShowShipments () {
		gameManager.OpenPortWindow (this, dockedShip);
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
		Island island = RandomIsland ();
		int reward = Random.Range (5, 36);
		Shipment shipment = new Shipment (GoodsName, MyIsland, island, reward);
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

	Island RandomIsland () {
		List<Island> validIslands = new List<Island> ();
		foreach (var island in gameManager.Islands) {
			if (island.MyPort != null && island != MyIsland) {
				validIslands.Add (island);
			}
		}
		int index = Random.Range (0, validIslands.Count);
		return validIslands [index];
	}
}
