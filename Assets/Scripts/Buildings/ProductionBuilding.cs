using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionBuilding : Building {

	public string GoodsName;
	public Slider ProcessSlider;
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
		// ProcessSlider.maxValue = SecPerShipment;
	}

	public override int GetStatByString (string statName) {
		switch (statName) {
		case "MinReward":
			return MinRewards [Level];
		case "MaxReward":
			return MaxRewards [Level];
		case "MinCargo":
			return MinCargos [Level];
		case "MaxCargo":
			return MaxCargos [Level];
		case "SecPerShipment":
			return (int)(SecPerShipment * 100);
		default:
			return 0;
		}
	}

	protected override void Update () {
		base.Update ();
		if (MyIsland.MyPort.Shipments.Count < MyIsland.MyPort.ShipmentsCapacity) {
			if (!shouldProduceShipments) {
				timer = 0.0f;
				// ProcessSlider.value = timer;
				shouldProduceShipments = true;
			}
			timer += Time.deltaTime;
			// ProcessSlider.value = timer;
			if (timer >= SecPerShipment) {
				timer = 0.0f;
				// ProcessSlider.value = timer;
				ProduceShipment ();
			}
			if (MyIsland.MyPort.Shipments.Count == MyIsland.MyPort.ShipmentsCapacity) {
				timer = 0.0f;
				ProcessSlider.value = timer;
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
