using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipmentNode : MonoBehaviour {
	public Text NameLabel;
	public Text DestinationLabel;
	public Text DistanceLabel;
	public Text RewardLabel;
	public Text WeightLabel;

	public Image MyImage;

	public Shipment MyShipment;
	public PortWindow MyPortWindow;

	public void ClickShipment () {
		MyPortWindow.ShipmentClicked (gameObject);
	}
}
