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
	public TradeShip CurrentTradeShip;

	public Text LocationLabel;
	public Text LocationCargo;
	public Text ShipCargo;
	public Text ShipLabel;
	public Slider LocationCargoSlider;

	public GameObject LeftButtonObject;
	public GameObject RightButtonObject;

	public void Open (Port port, TradeShip tradeShip) {
		if (CurrentPort != null) {
			 CurrentPort.OnProducedShipment -= CurrentPort_OnProducedShipment;
		}
		CurrentPort = port;
		if (CurrentPort != null) {
			CurrentPort.OnProducedShipment += CurrentPort_OnProducedShipment;
		}
		CurrentTradeShip = tradeShip;
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

		if (CurrentTradeShip != null) {
			foreach (var shipment in tradeShip.Shipments) {
				GameObject shipmentNodeObject = FormShipmentNodeObject (shipment);

				shipmentNodeObject.transform.SetParent (ShipContainer.transform);
				shipmentNodeObject.transform.localScale = Vector3.one;
				ShipShipmentNodeObjects.Add (shipmentNodeObject);
			}
		}

		RefreshData ();
	}

	public void Close () {
		if (CurrentPort != null && CurrentPort.Name == "Shipwreck" && CurrentPort.Shipments.Count == 0) { // as reliable as bullets made of chocolate
			Destroy (CurrentPort.gameObject);
		}
		Window.SetActive (false);
	}

	public void RefreshData () {
		LocationCargoSlider.maxValue = CurrentPort.ShipmentsCapacity;
		LocationCargoSlider.value = CurrentPort.Shipments.Count;
		LocationCargo.text = CurrentPort.Shipments.Count + "/" + CurrentPort.ShipmentsCapacity;

		if (CurrentTradeShip != null) {
			ShipLabel.text = "Docked ship";
			ShipCargo.text = CurrentTradeShip.TradeShipData.TotalWeight + "/" + CurrentTradeShip.ShipmentsCapacity;
		} else {
			ShipLabel.text = "No docked ships";
			ShipCargo.text = "";
		}

		if (CurrentPort.DockedTradeShips.Count > 1) {
			if (CurrentPort.DockedTradeShips.IndexOf (CurrentTradeShip) > 0) {
				LeftButtonObject.SetActive (true);
			}
			if (CurrentPort.DockedTradeShips.IndexOf (CurrentTradeShip) < CurrentPort.DockedTradeShips.Count - 1) {
				RightButtonObject.SetActive (true);
			}
			if (CurrentPort.DockedTradeShips.IndexOf (CurrentTradeShip) == 0) {
				LeftButtonObject.SetActive (false);
			}
			if (CurrentPort.DockedTradeShips.IndexOf (CurrentTradeShip) == CurrentPort.DockedTradeShips.Count - 1) {
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
		shipmentNode.NameLabel.text = shipment.Goods.Name;
		if (Player.Instance.DataBase.ItemIconsByNames.ContainsKey(shipment.Goods.Name)) {
			shipmentNode.MyImage.sprite = Player.Instance.DataBase.ItemIconsByNames [shipment.Goods.Name];
		}
		shipmentNode.DestinationLabel.text = shipment.DestinationIslandName;
		shipmentNode.DistanceLabel.text = shipment.Distance.ToString ();
		shipmentNode.RewardLabel.text = "$" + shipment.Reward;
		if (!shipment.Goods.IsForSale) {
			shipmentNode.RewardLabel.text = "-";
		}
		shipmentNode.WeightLabel.text = shipment.Cargo + "t";
		shipmentNode.MyPortWindow = this;
		return shipmentNodeObject;
	}

	public void ShipmentClicked (GameObject shipmentNodeObject) {
		if (CurrentTradeShip == null) {
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
		ShipCargo.text = CurrentTradeShip.TradeShipData.TotalWeight + "/" + CurrentTradeShip.ShipmentsCapacity;
	}

	void PortToShip (GameObject shipmentNodeObject) {
		if (CurrentTradeShip == null) {
			return;
		}
		if (CurrentTradeShip.ShipmentsCapacity - CurrentTradeShip.TradeShipData.TotalWeight >= shipmentNodeObject.GetComponent<ShipmentNode> ().MyShipment.Cargo) {
			PortShipmentNodeObjects.Remove (shipmentNodeObject);
			ShipShipmentNodeObjects.Add (shipmentNodeObject);
			shipmentNodeObject.transform.SetParent (ShipContainer.transform);
			CurrentPort.GiveShipment (shipmentNodeObject.GetComponent<ShipmentNode> ().MyShipment);
			CurrentTradeShip.TakeShipment (shipmentNodeObject.GetComponent<ShipmentNode> ().MyShipment);
		}
	}

	void ShipToPort (GameObject shipmentNodeObject) {	
		if (CurrentTradeShip == null) {
			return;
		}	
		if (CurrentPort.Shipments.Count < CurrentPort.ShipmentsCapacity) {
			ShipShipmentNodeObjects.Remove (shipmentNodeObject);
			PortShipmentNodeObjects.Add (shipmentNodeObject);
			shipmentNodeObject.transform.SetParent (PortContainer.transform);
			CurrentTradeShip.GiveShipment (shipmentNodeObject.GetComponent<ShipmentNode> ().MyShipment);
			CurrentPort.TakeShipment (shipmentNodeObject.GetComponent<ShipmentNode> ().MyShipment);
		}
	}

	public void ScrollLeft () { // if it bites my ass later: probably should also change dockedShip in Port
		CurrentTradeShip = CurrentPort.DockedTradeShips[CurrentPort.DockedTradeShips.IndexOf(CurrentTradeShip) - 1];
		RefreshData ();
	}

	public void ScrollRight () {
		CurrentTradeShip = CurrentPort.DockedTradeShips[CurrentPort.DockedTradeShips.IndexOf(CurrentTradeShip) + 1];
		RefreshData ();
	}
}
