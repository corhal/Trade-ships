using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeShip : Selectable {	
	public GameObject DepartButtonObject;
	public Island StartIsland;
	public Island CurrentDestination;
	public TradeShipData TradeShipData;
	public TradeShipMover TradeShipMover;

	public Slider CargoSlider;

	public List<Shipment> Shipments { get { return TradeShipData.Shipments; } set { TradeShipData.Shipments = value; } }

	public int ShipmentsCapacity { get { return TradeShipData.ShipmentsCapacity; } set { TradeShipData.ShipmentsCapacity = value; } }

	MoveOnClick mover;
	TradeShipMover tradeShipMover;
	Port currentPort;
	bool docked;
	float timer;

	protected override void Awake () {
		base.Awake ();
		mover = gameObject.GetComponent<MoveOnClick> ();
		tradeShipMover = gameObject.GetComponent<TradeShipMover> ();
		// mover.OnStartedMoving += Mover_OnStartedMoving;
	}

	protected override void Start () {
		base.Start ();
	
		Process = "Moving";
		Action moveAction = new Action ("Move", 0, player.DataBase.ActionIconsByNames["Move"], MoveMode);
		actions.Add (moveAction);

		Name = TradeShipData.Name;
		Allegiance = TradeShipData.Allegiance;

		if (TradeShipData.Coordinates.Length > 0) {
			transform.position = new Vector3 (TradeShipData.Coordinates[0], TradeShipData.Coordinates[1], TradeShipData.Coordinates[2]);
		}

		CargoSlider.maxValue = ShipmentsCapacity;
		CargoSlider.value = TradeShipData.TotalWeight;

		CargoSlider.maxValue = ShipmentsCapacity;
		CargoSlider.value = TradeShipData.TotalWeight;

		tradeShipMover.MoveToPosition (StartIsland.MyPort.transform.position);
	}

	protected override void Update () {
		base.Update ();
		timer += Time.deltaTime;
		if (timer > 1.0f && docked) {
			timer = 0.0f;
			TakeCargo ();
		}
	}

	public void TakeShipment (Shipment shipment) {		
		TradeShipData.TakeShipment (shipment);
		CargoSlider.value = TradeShipData.TotalWeight;
	}

	public void GiveShipment (Shipment shipment) {
		TradeShipData.GiveShipment (shipment);
		CargoSlider.value = TradeShipData.TotalWeight;
	}

	public void MoveMode () {
		gameManager.MoveMode ();
		// mover.InMoveMode = true;
	}

	void Mover_OnStartedMoving (MoveOnClick sender) {
		InitialProcessSeconds = mover.TimeLeft;
		InProcess = true;
	}

	public override float GetProcessSeconds () {		
		if (mover.TimeLeft <= 0.1f) {
			return 0.0f;
		}
		return mover.TimeLeft;
	}

	public override void ShowInfo () {
		gameManager.OpenSelectableInfo (this);
	}

	void OnTriggerEnter2D (Collider2D other) { // will work even when passing through another port
		if (Allegiance != Allegiance.Enemy && other.gameObject.GetComponent<Port> () != null) {
			UnloadCargo (other.gameObject.GetComponent<Port> ());
			if (other.gameObject.GetComponent<Port> ().MyIsland != StartIsland) {
				DepartButtonObject.SetActive (true);
			}
		}
	}

	public void UnloadCargo (Port port) {
		currentPort = port;
		docked = true;
		List<Shipment> shipmentsToDestroy = new List<Shipment> ();
		foreach (var shipment in Shipments) {
			if (port.Name == "Shipwreck") { // bleargh
				continue;
			}
			if (shipment.DestinationIslandName == port.MyIsland.Name) {
				if (shipment.Goods.IsForSale) {
					Player.Instance.TakeGold (shipment.Reward);
					shipmentsToDestroy.Add (shipment);
				} else {
					Player.Instance.TakeItems (new Dictionary<string, int> { { shipment.Goods.Name, 1 } });
					shipmentsToDestroy.Add (shipment);
				}
			}
		}

		foreach (var shipment in shipmentsToDestroy) {
			GiveShipment (shipment);
		}
		shipmentsToDestroy.Clear ();
	}

	public void TakeCargo () {		
		List<Shipment> shipmentsToRemove = new List<Shipment> ();
		foreach (var shipment in currentPort.Shipments) {
			if (ShipmentsCapacity - TradeShipData.TotalWeight >= shipment.Cargo) {	
				shipmentsToRemove.Add (shipment);
				TakeShipment (shipment);
				CurrentDestination = shipment.Destination;
			}
		}
		foreach (var shipment in shipmentsToRemove) {
			currentPort.GiveShipment (shipment);
		}
		if (Shipments.Count > 0) {
			DepartButtonObject.SetActive (true);
		}
	}

	public void Depart () {
		if (currentPort.MyIsland != StartIsland) { // дичайший говнокод
			tradeShipMover.MoveToPosition (StartIsland.MyPort.transform.position);
			docked = false;
		} else {
			tradeShipMover.MoveToPosition (CurrentDestination.MyPort.transform.position);
			docked = false;
		}
		DepartButtonObject.SetActive (false);
	}
}
