using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Selectable {	
	public List<Shipment> Shipments;
	public int ShipmentsCapacity;
	public int HP;
	public int Power;

	new void Awake () {
		base.Awake ();
	}

	new void Start () {
		base.Start ();
		Shipments = new List<Shipment> ();
		Action moveAction = new Action ("Move", 0, MoveMode);
		actions.Add (moveAction);
	}

	public void TakeShipment (Shipment shipment) {
		if (Shipments.Count < ShipmentsCapacity) {
			Shipments.Add (shipment);
		}
	}

	public void GiveShipment (Shipment shipment) {
		Shipments.Remove (shipment);
	}

	public void MoveMode () {
		gameManager.InMoveMode = true;
		MoveOnClick mover = gameObject.GetComponent<MoveOnClick> ();
		mover.InMoveMode = true;
	}

	void OnTriggerEnter2D (Collider2D other) { // will work even when passing through another port
		if (other.gameObject.GetComponent<Port> () != null) {
			UnloadCargo (other.gameObject.GetComponent<Port> ());
		}
	}

	public void UnloadCargo (Port port) {
		List<Shipment> shipmentsToDestroy = new List<Shipment> ();
		foreach (var shipment in Shipments) {
			if (shipment.Destination == port.MyIsland) {
				Player.Instance.TakeGold (shipment.Reward);
				shipmentsToDestroy.Add (shipment);
			}
		}

		foreach (var shipment in shipmentsToDestroy) {
			Shipments.Remove (shipment);
		}
		shipmentsToDestroy.Clear ();
	}
}
