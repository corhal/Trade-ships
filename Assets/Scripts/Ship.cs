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
		Debug.Log ("caught action");
		MoveOnClick mover = gameObject.GetComponent<MoveOnClick> ();
		mover.InMoveMode = true;
	}
}
