using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shipment {

	public string GoodsName;
	public Island StartIsland;
	public Island Destination;
	public string DestinationString;
	public int Reward;

	public float Distance;

	public Shipment (string goodsName, Island startIsland, Island destination, int reward) {
		this.GoodsName = goodsName;
		this.StartIsland = startIsland;
		this.Destination = destination;
		this.Reward = reward;
		Distance = Vector2.Distance (StartIsland.transform.position, Destination.transform.position);
	}
}
