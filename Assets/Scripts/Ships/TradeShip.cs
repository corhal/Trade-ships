﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeShip : MonoBehaviour {	
	public GameObject DepartButtonObject;
	public Island StartIsland;
	public Island CurrentDestination;
	public TradeShipData TradeShipData;
	public TradeShipMover TradeShipMover;

	public Slider CargoSlider;

	public List<Shipment> Shipments { get { return TradeShipData.Shipments; } set { TradeShipData.Shipments = value; } }

	public int ShipmentsCapacity { get { return TradeShipData.ShipmentsCapacity; } set { TradeShipData.ShipmentsCapacity = value; } }

	TradeShipMover tradeShipMover;
	Port currentPort;
	bool docked;
	float timer;

	void Awake () {
		tradeShipMover = gameObject.GetComponent<TradeShipMover> ();
	}

	void Start () {		
		transform.position = TradeShipData.Coordinates; 

		CargoSlider.maxValue = ShipmentsCapacity;
		CargoSlider.value = TradeShipData.TotalWeight;

		CargoSlider.maxValue = ShipmentsCapacity;
		CargoSlider.value = TradeShipData.TotalWeight;
		if (TradeShipData.StartIslandName == "") {
			TradeShipData.StartIslandName = StartIsland.Name;
		} 
		tradeShipMover.MoveToPosition (StartIsland.MyPort.transform.position);
	}

	void Update () {
		timer += Time.deltaTime;
		if (timer > 1.0f && docked) {
			timer = 0.0f;
			TakeCargo ();
		}
	}

	public void TakeShipment (Shipment shipment) {		
		TradeShipData.TakeShipment (shipment);
		CargoSlider.value = TradeShipData.TotalWeight;
	}

	public void GiveShipment (Shipment shipment) {
		TradeShipData.GiveShipment (shipment);
		CargoSlider.value = TradeShipData.TotalWeight;
	}

	void OnTriggerEnter2D (Collider2D other) { // will work even when passing through another port
		if (other.gameObject.GetComponent<Port> () != null) {
			UnloadCargo (other.gameObject.GetComponent<Port> ());
			if (other.gameObject.GetComponent<Port> ().MyIsland != StartIsland) {
				Invoke ("Depart", 1.5f);
			}
		}
	}

	public void UnloadCargo (Port port) {
		currentPort = port;
		docked = true;
		List<Shipment> shipmentsToDestroy = new List<Shipment> ();
		foreach (var shipment in Shipments) {
			if (port.Name == "Shipwreck") { // bleargh
				continue;
			}
			if (shipment.DestinationIslandName == port.MyIsland.Name) {
				Player.Instance.TakeGold (shipment.Reward);
				shipmentsToDestroy.Add (shipment);
			}
		}

		foreach (var shipment in shipmentsToDestroy) {
			GiveShipment (shipment);
		}
		shipmentsToDestroy.Clear ();
	}

	public void TakeCargo () {		
		List<Shipment> shipmentsToRemove = new List<Shipment> ();
		foreach (var shipment in currentPort.Shipments) {
			if (ShipmentsCapacity - TradeShipData.TotalWeight >= shipment.Cargo) {	
				shipmentsToRemove.Add (shipment);
				TakeShipment (shipment);
			}
		}
		foreach (var shipment in shipmentsToRemove) {
			currentPort.GiveShipment (shipment);
		}
		if (Shipments.Count > 0) {
			DepartButtonObject.SetActive (true);
		}
	}

	public void Depart () {
		if (currentPort.MyIsland != StartIsland) { // дичайший говнокод
			tradeShipMover.MoveToPosition (StartIsland.MyPort.transform.position);
			docked = false;
		} else {
			if (Shipments.Count > 0) {
				CurrentDestination = GameManager.Instance.GetIslandByName (Shipments [0].DestinationIslandName);
			}
			tradeShipMover.MoveToPosition (CurrentDestination.MyPort.transform.position);
			docked = false;
		}
		DepartButtonObject.SetActive (false);
	}
}
