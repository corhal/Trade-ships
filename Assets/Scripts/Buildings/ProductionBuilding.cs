using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building {

	public string GoodsName;

	public List<int> MinRewards;
	public List<int> MaxRewards;

	public List<int> MinCargos;
	public List<int> MaxCargos;

	public float SecPerShipment;
	float timer;
	bool shouldProduceShipments;

	new void Awake () {
		base.Awake ();
	}

	new void Start () {
		base.Start ();
	}

	void Update () {
		if (MyIsland.MyPort.Shipments.Count < MyIsland.MyPort.ShipmentsCapacity) {
			if (!shouldProduceShipments) {
				timer = 0.0f;
				shouldProduceShipments = true;
			}
			timer += Time.deltaTime;
			if (timer >= SecPerShipment) {
				timer = 0.0f;
				ProduceShipment ();
			}
			if (MyIsland.MyPort.Shipments.Count == MyIsland.MyPort.ShipmentsCapacity) {
				timer = 0.0f;
				shouldProduceShipments = false;
			}
		}
	}

	void ProduceShipment () {
		Island island = RandomIsland ();
		int reward = Random.Range (MinRewards [Level], MaxRewards [Level] + 1);
		int cargo = Random.Range (MinCargos [Level], MaxCargos [Level] + 1);
		Shipment shipment = new Shipment (GoodsName, MyIsland, island, cargo, reward);
		MyIsland.MyPort.TakeShipment (shipment);
	}

	Island RandomIsland () {
		List<Island> validIslands = new List<Island> ();
		foreach (var island in gameManager.Islands) {
			if (island.MyPort != null && island != MyIsland) {
				validIslands.Add (island);
			}
		}
		int index = Random.Range (0, validIslands.Count);
		return validIslands [index];
	}
}
