using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortWindow : MonoBehaviour {

	public GameObject Window;
	public GameObject Container;
	public GameObject ShipmentNodePrefab;
	public List<GameObject> ShipmentNodeObjects;

	public void Open (Port port) {
		Window.SetActive (true);
		foreach (var shipmentNodeObject in ShipmentNodeObjects) {
			Destroy (shipmentNodeObject);
		}
		ShipmentNodeObjects.Clear ();

		foreach (var shipment in port.Shipments) {
			GameObject shipmentNodeObject = Instantiate (ShipmentNodePrefab) as GameObject;
			ShipmentNode shipmentNode = shipmentNodeObject.GetComponent<ShipmentNode> ();

			shipmentNode.NameLabel.text = shipment.GoodsName;
			shipmentNode.DestinationLabel.text = shipment.Destination.MyPort.Name;
			shipmentNode.DistanceLabel.text = shipment.Distance.ToString ();
			shipmentNode.RewardLabel.text = shipment.Reward.ToString ();
			shipmentNodeObject.transform.SetParent (Container.transform);
			shipmentNodeObject.transform.localScale = Vector3.one;
			ShipmentNodeObjects.Add (shipmentNodeObject);
		}
	}
}
