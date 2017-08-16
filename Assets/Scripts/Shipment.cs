using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shipment : MonoBehaviour {

	public string GoodsName;
	public Location StartLocation;
	public Location Destination;
	public string DestinationString;
	public int Reward;

	void Start() {
		int distance = Vector2.Distance (StartLocation.transform.position, Destination.transform.position);
	}
}
