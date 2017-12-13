using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionBuilding : Building {

	public Island Destination;
	public string GoodsName;
	public Slider ProcessSlider;
	public List<int> MinRewards;
	public List<int> MaxRewards;

	public List<int> MinCargos;
	public List<int> MaxCargos;

	public float SecPerShipment;
	float timer;
	bool shouldProduceShipments;

	protected override void Awake () {
		base.Awake ();
	}

	protected override void Start () {
		base.Start ();
		AdjacentRadius = 3.0f;
	}

	/*protected override*/ void Update () {
		//base.Update ();
		if (MyIsland.MyPort.Shipments.Count < MyIsland.MyPort.ShipmentsCapacity && Allegiance == Allegiance.Player) {
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
		Island island = Destination;
		int reward = Random.Range (MinRewards [0], MaxRewards [0] + 1);
		int cargo = Random.Range (MinCargos [0], MaxCargos [0] + 1);
		Item goods = GameManager.Instance.GetItemByName (GoodsName);
		Shipment shipment = new Shipment (goods, MyIsland.Name, island.Name, cargo, reward);
		MyIsland.MyPort.TakeShipment (shipment);
	}

	Island RandomIsland () {
		List<Island> validIslands = new List<Island> ();
	foreach (var island in GameManager.Instance.Islands) {
			if (island.MyPort != null && island != MyIsland /*&& island.MyPort.IsAvailable*/) {
				validIslands.Add (island);
			}
		}
		int index = Random.Range (0, validIslands.Count);
		return validIslands [index];
	}
}
