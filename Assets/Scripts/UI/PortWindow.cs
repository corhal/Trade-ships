using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortWindow : MonoBehaviour {

	public GameObject Window;
	public GameObject PortContainer;
	public GameObject ShipContainer;
	public GameObject ShipmentNodePrefab;
	public List<GameObject> PortShipmentNodeObjects;
	public List<GameObject> ShipShipmentNodeObjects;

	public Port CurrentPort;
	public Ship CurrentShip;

	public Text LocationLabel;
	public Text LocationCargo;
	public Text ShipCargo;
	public Text ShipLabel;
	public Slider LocationCargoSlider;

	public GameObject LeftButtonObject;
	public GameObject RightButtonObject;

	public void Open (Port port, Ship ship) {
		if (CurrentPort != null) {
			 CurrentPort.OnProducedShipment -= CurrentPort_OnProducedShipment;
		}
		CurrentPort = port;
		if (CurrentPort != null) {
			CurrentPort.OnProducedShipment += CurrentPort_OnProducedShipment;
		}
		CurrentShip = ship;
		Window.SetActive (true);
		foreach (var shipmentNodeObject in PortShipmentNodeObjects) {
			Destroy (shipmentNodeObject);
		}
		foreach (var shipmentNodeObject in ShipShipmentNodeObjects) {
			Destroy (shipmentNodeObject);
		}
		PortShipmentNodeObjects.Clear ();
		ShipShipmentNodeObjects.Clear ();
		LocationLabel.text = CurrentPort.Name;
		foreach (var shipment in port.Shipments) {
			GameObject shipmentNodeObject = FormShipmentNodeObject (shipment);

			shipmentNodeObject.transform.SetParent (PortContainer.transform);
			shipmentNodeObject.transform.localScale = Vector3.one;
			PortShipmentNodeObjects.Add (shipmentNodeObject);
		}

		if (CurrentShip != null) {
			foreach (var shipment in ship.Shipments) {
				GameObject shipmentNodeObject = FormShipmentNodeObject (shipment);

				shipmentNodeObject.transform.SetParent (ShipContainer.transform);
				shipmentNodeObject.transform.localScale = Vector3.one;
				ShipShipmentNodeObjects.Add (shipmentNodeObject);
			}
		}

		RefreshData ();
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void RefreshData () {
		LocationCargoSlider.maxValue = CurrentPort.ShipmentsCapacity;
		LocationCargoSlider.value = CurrentPort.Shipments.Count;
		LocationCargo.text = CurrentPort.Shipments.Count + "/" + CurrentPort.ShipmentsCapacity;

		if (CurrentShip != null) {
			ShipLabel.text = "Docked ship";
			ShipCargo.text = CurrentShip.Shipments.Count + "/" + CurrentShip.ShipmentsCapacity;
		} else {
			ShipLabel.text = "No docked ships";
			ShipCargo.text = "";
		}

		if (CurrentPort.DockedShips.Count > 1) {
			if (CurrentPort.DockedShips.IndexOf (CurrentShip) > 0) {
				LeftButtonObject.SetActive (true);
			}
			if (CurrentPort.DockedShips.IndexOf (CurrentShip) < CurrentPort.DockedShips.Count - 1) {
				RightButtonObject.SetActive (true);
			}
			if (CurrentPort.DockedShips.IndexOf (CurrentShip) == 0) {
				LeftButtonObject.SetActive (false);
			}
			if (CurrentPort.DockedShips.IndexOf (CurrentShip) == CurrentPort.DockedShips.Count - 1) {
				RightButtonObject.SetActive (false);
			}
		} else {
			LeftButtonObject.SetActive (false);
			RightButtonObject.SetActive (false);
		}
	}

	void CurrentPort_OnProducedShipment (Port sender, Shipment shipment) {
		GameObject shipmentNodeObject = FormShipmentNodeObject (shipment);

		shipmentNodeObject.transform.SetParent (PortContainer.transform);
		shipmentNodeObject.transform.localScale = Vector3.one;
		PortShipmentNodeObjects.Add (shipmentNodeObject);

		RefreshData ();
	}

	GameObject FormShipmentNodeObject (Shipment shipment) {
		GameObject shipmentNodeObject = Instantiate (ShipmentNodePrefab) as GameObject;
		ShipmentNode shipmentNode = shipmentNodeObject.GetComponent<ShipmentNode> ();
		shipmentNode.MyShipment = shipment;
		shipmentNode.NameLabel.text = shipment.GoodsName;
		shipmentNode.DestinationLabel.text = shipment.Destination.MyPort.MyIsland.Name;
		shipmentNode.DistanceLabel.text = shipment.Distance.ToString ();
		shipmentNode.RewardLabel.text = shipment.Reward.ToString ();
		shipmentNode.WeightLabel.text = shipment.Cargo + "t";
		shipmentNode.MyPortWindow = this;
		return shipmentNodeObject;
	}

	public void ShipmentClicked (GameObject shipmentNodeObject) {
		if (CurrentShip == null) {
			return;
		}
		if (PortShipmentNodeObjects.Contains(shipmentNodeObject)) {
			PortToShip (shipmentNodeObject);
		} else if (ShipShipmentNodeObjects.Contains(shipmentNodeObject)) {
			ShipToPort (shipmentNodeObject);
		}
		LocationCargoSlider.maxValue = CurrentPort.ShipmentsCapacity;
		LocationCargoSlider.value = CurrentPort.Shipments.Count;
		LocationCargo.text = CurrentPort.Shipments.Count + "/" + CurrentPort.ShipmentsCapacity;
		ShipCargo.text = CurrentShip.Shipments.Count + "/" + CurrentShip.ShipmentsCapacity;
	}

	void PortToShip (GameObject shipmentNodeObject) {
		if (CurrentShip == null) {
			return;
		}
		if (CurrentShip.Shipments.Count < CurrentShip.ShipmentsCapacity) {
			PortShipmentNodeObjects.Remove (shipmentNodeObject);
			ShipShipmentNodeObjects.Add (shipmentNodeObject);
			shipmentNodeObject.transform.SetParent (ShipContainer.transform);
			CurrentPort.GiveShipment (shipmentNodeObject.GetComponent<ShipmentNode> ().MyShipment);
			CurrentShip.TakeShipment (shipmentNodeObject.GetComponent<ShipmentNode> ().MyShipment);
		}
	}

	void ShipToPort (GameObject shipmentNodeObject) {	
		if (CurrentShip == null) {
			return;
		}	
		if (CurrentPort.Shipments.Count < CurrentPort.ShipmentsCapacity) {
			ShipShipmentNodeObjects.Remove (shipmentNodeObject);
			PortShipmentNodeObjects.Add (shipmentNodeObject);
			shipmentNodeObject.transform.SetParent (PortContainer.transform);
			CurrentShip.GiveShipment (shipmentNodeObject.GetComponent<ShipmentNode> ().MyShipment);
			CurrentPort.TakeShipment (shipmentNodeObject.GetComponent<ShipmentNode> ().MyShipment);
		}
	}

	public void ScrollLeft () { // if it bites my ass later: probably should also change dockedShip in Port
		CurrentShip = CurrentPort.DockedShips[CurrentPort.DockedShips.IndexOf(CurrentShip) - 1];
		RefreshData ();
	}

	public void ScrollRight () {
		CurrentShip = CurrentPort.DockedShips[CurrentPort.DockedShips.IndexOf(CurrentShip) + 1];
		RefreshData ();
	}
}
