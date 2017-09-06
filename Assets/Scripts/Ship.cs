using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Selectable {	

	public List<Skill> Skills;
	public List<Shipment> Shipments;
	public Dictionary<string, int> Stats;
	public int ShipmentsCapacity;
	public int HP;
	public int Power;

	new void Awake () {
		base.Awake ();
	}

	new void Start () {
		base.Start ();
		Stats = new Dictionary<string, int> {
			{"Cargo", ShipmentsCapacity},
			{"HP", HP},
			{"Firepower", Power}
		};
		Skills = new List<Skill> {			
			new Skill("Trade", 1, 5, new List<int> {0, 10, 20, 30, 50}),
			new Skill("Cannons", 1, 5, new List<int> {0, 10, 20, 30, 50}),
			new Skill("Navigation", 1, 5, new List<int> {0, 10, 20, 30, 50}),
			new Skill("Something else", 1, 5, new List<int> {0, 10, 20, 30, 50})
		};
		Shipments = new List<Shipment> ();
		Action moveAction = new Action ("Move", 0, MoveMode);
		Action infoAction = new Action("Info", 0, ShowInfo);
		actions.Add (moveAction);
		actions.Add (infoAction);
	}

	public void UpgradeSkill (Skill skill) {		
		if (Skills.Contains(skill)) {			
			if (player.Gold >= skill.UpgradeCosts[skill.Level]) {
				player.GiveGold (skill.UpgradeCosts [skill.Level]);
				skill.Upgrade ();
			} else {
				gameManager.OpenPopUp ("Not enough gold!");
			}
		}
	}

	public void TakeShipment (Shipment shipment) {
		int totalWeight = 0;
		foreach (var myShipment in Shipments) {
			totalWeight += myShipment.Cargo;
		}
		if (totalWeight < ShipmentsCapacity) {
			Shipments.Add (shipment);
		}
	}

	public void GiveShipment (Shipment shipment) {
		Shipments.Remove (shipment);
	}

	public void MoveMode () {
		gameManager.InMoveMode = true;
		MoveOnClick mover = gameObject.GetComponent<MoveOnClick> ();
		mover.InMoveMode = true;
	}

	public void ShowInfo () {
		gameManager.OpenShipWindow (this);
	}

	void OnTriggerEnter2D (Collider2D other) { // will work even when passing through another port
		if (other.gameObject.GetComponent<Port> () != null) {
			UnloadCargo (other.gameObject.GetComponent<Port> ());
		}
	}

	public void UnloadCargo (Port port) {
		List<Shipment> shipmentsToDestroy = new List<Shipment> ();
		foreach (var shipment in Shipments) {
			if (shipment.Destination == port.MyIsland) {
				Player.Instance.TakeGold (shipment.Reward);
				shipmentsToDestroy.Add (shipment);
			}
		}

		foreach (var shipment in shipmentsToDestroy) {
			Shipments.Remove (shipment);
		}
		shipmentsToDestroy.Clear ();
	}
}
