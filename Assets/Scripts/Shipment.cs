using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shipment {

	public string GoodsName;
	public Location StartLocation;
	public Location Destination;
	public string DestinationString;
	public int Reward;

	public float Distance;

	public Shipment (string goodsName, Location startLocation, Location destination, int reward) {
		this.GoodsName = goodsName;
		this.StartLocation = startLocation;
		this.Destination = destination;
		this.Reward = reward;
		Distance = Vector2.Distance (StartLocation.transform.position, Destination.transform.position);
	}
}
