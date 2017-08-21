using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, ISelectable {

	public GameManager Manager;
	List<Action> actions;
	public List<Action> Actions { get { return actions; } }
	public List<Shipment> Shipments;
	public int ShipmentsCapacity;

	void Start () {
		Shipments = new List<Shipment> ();
		actions = new List<Action> ();
		Action moveAction = new Action ("Move", 0, MoveMode);
		actions.Add (moveAction);
	}

	void OnMouseDown () {
		Manager.OpentContextButtons (this);
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
			if (shipment.Destination == port.MyLocation) {
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
