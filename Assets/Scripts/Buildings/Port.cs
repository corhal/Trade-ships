﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Port : Building {
	public GameObject TradeShipPrefab;
	public Slider CargoSlider;
	public Text IslandLabel;
	public List<TradeShip> DockedTradeShips;
	TradeShip dockedTradeShip;

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
		showShipmentsAction = new Action ("Show shipments", 0, player.DataBase.ActionIconsByNames["Show shipments"], ShowShipments);
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
		gameManager.OpenPortWindow (this, dockedTradeShip);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.GetComponent<TradeShip>() != null) {
			DockedTradeShips.Add(other.gameObject.GetComponent<TradeShip> ());
			other.gameObject.GetComponent<TradeShip> ().Actions.Add (showShipmentsAction);
			if (dockedTradeShip == null) {
				dockedTradeShip = DockedTradeShips [0];
			}
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.GetComponent<TradeShip>() != null) {
			DockedTradeShips.Remove(other.gameObject.GetComponent<TradeShip> ());
			other.gameObject.GetComponent<TradeShip> ().Actions.Remove (showShipmentsAction);
			if (DockedTradeShips.Count == 0) {
				dockedTradeShip = null;
			} else {
				dockedTradeShip = DockedTradeShips [0];
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

	public override void Claim () {
		base.Claim ();
		GameObject tradeShipObject = Instantiate (TradeShipPrefab) as GameObject;
		tradeShipObject.transform.position = transform.position;
		TradeShip tradeShip = tradeShipObject.GetComponent<TradeShip> ();
		tradeShip.StartIsland = MyIsland;
		Player.Instance.TradeShips.Add (tradeShip);
		Player.Instance.TradeShipDatas.Add (tradeShip.TradeShipData);
	}
}
