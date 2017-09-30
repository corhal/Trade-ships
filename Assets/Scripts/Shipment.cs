using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shipment {

	//public string GoodsName;
	public Item Goods;
	public string StartIslandName;
	public string DestinationIslandName;

	//public Island StartIsland;
	//public Island Destination;
	public string DestinationString;
	public int Reward;
	public int Cargo;

	public float Distance;

	public Shipment (Item goods, string startIslandName, string destinationislandName, int cargo, int reward) {
		this.Goods = goods;
		this.StartIslandName = startIslandName;
		this.DestinationIslandName = destinationislandName;
		this.Reward = reward;
		this.Cargo = cargo;
		Distance = Vector2.Distance (Vector2.zero, Vector2.one); //StartIsland.transform.position, Destination.transform.position);
	}
}
