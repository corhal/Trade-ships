using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Port : Building {
	public Slider CargoSlider;
	public Text IslandLabel;
	public List<Ship> DockedShips;
	Ship dockedShip;

	public List<int> ShipmentsCapacities;
	public int ShipmentsCapacity { get { return ShipmentsCapacities [Level]; } }
	public List<Shipment> Shipments;

	public delegate void ProducedShipmentEventHandler (Port sender, Shipment shipment);
	public event ProducedShipmentEventHandler OnProducedShipment;
	Action showShipmentsAction;

	protected override void Awake () {
		base.Awake ();
	}

	protected override void Start () {
		base.Start ();
		//Shipments = new List<Shipment> ();
		//Debug.Log ("here we nullify shipments");
		showShipmentsAction = new Action ("Show shipments", 0, gameManager.ActionIconsByNames["Show shipments"], ShowShipments);
		actions.Add (showShipmentsAction);
		CargoSlider.maxValue = ShipmentsCapacity;
		CargoSlider.value = Shipments.Count;
		if (MyIsland != null) {
			IslandLabel.text = MyIsland.Name;
		}
	}

	public override void InitializeFromData (BuildingData buildingData) {
		base.InitializeFromData (buildingData);
		PortData portData = buildingData as PortData;
		ShipmentsCapacities = new List<int> (portData.ShipmentsCapacities);
		Shipments = new List<Shipment> (portData.Shipments);
	}

	public override int GetStatByString (string statName) {
		switch (statName) {
		case "Cargo":
			return ShipmentsCapacity;
		default:
			return 0;
		}
	}

	void ShowShipments () {
		gameManager.OpenPortWindow (this, dockedShip);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.GetComponent<Ship>() != null) {
			DockedShips.Add(other.gameObject.GetComponent<Ship> ());
			other.gameObject.GetComponent<Ship> ().Actions.Add (showShipmentsAction);
			if (dockedShip == null) {
				dockedShip = DockedShips [0];
			}
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.GetComponent<Ship>() != null) {
			DockedShips.Remove(other.gameObject.GetComponent<Ship> ());
			other.gameObject.GetComponent<Ship> ().Actions.Remove (showShipmentsAction);
			if (DockedShips.Count == 0) {
				dockedShip = null;
			} else {
				dockedShip = DockedShips [0];
			}
		}
	}

	public void TakeShipment (Shipment shipment) {
		if (Shipments.Count < ShipmentsCapacity) {
			Shipments.Add (shipment);
			CargoSlider.maxValue = ShipmentsCapacity;
			CargoSlider.value = Shipments.Count;
		}
		if (OnProducedShipment != null) {
			OnProducedShipment (this, shipment);
			CargoSlider.value = Shipments.Count;
		}
	}

	protected override void RefreshActions () {
		base.RefreshActions ();
		CargoSlider.maxValue = ShipmentsCapacity;
		CargoSlider.value = Shipments.Count;
	}

	public void GiveShipment (Shipment shipment) {
		Shipments.Remove (shipment);
		CargoSlider.value = Shipments.Count;
	}
}
