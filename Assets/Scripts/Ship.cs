using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stat {
	public string Name;
	public int Value;

	public Stat (string name, int value) {
		Name = name;
		Value = value;
	}
}

public class Ship : Selectable {	

	public List<Skill> Skills;
	public List<Shipment> Shipments;

	public List<Stat> Stats;

	[SerializeField]
	int shipmentsCapacity;
	public int ShipmentsCapacity { get { return shipmentsCapacity + CalculateBonus("Cargo"); } }
	[SerializeField]
	int hp;
	public int HP { get { return hp + CalculateBonus("HP"); } }
	[SerializeField]
	int power;
	public int Power { get { return power + CalculateBonus("Firepower"); } }

	new void Awake () {
		base.Awake ();
	}

	new void Start () {
		base.Start ();

		Stats = new List<Stat> { new Stat ("Cargo", ShipmentsCapacity), new Stat ("HP", HP), new Stat ("Firepower", Power) };

		Skills = new List<Skill> {			
			new Skill("Trade", 1, 5, new List<int> {0, 10, 20, 30, 50}, new List<string> {"Cargo"}, new List<Dictionary<string, int>> {
				new Dictionary<string, int> {{"Cargo", 1}},
				new Dictionary<string, int> {{"Cargo", 2}},
				new Dictionary<string, int> {{"Cargo", 3}},
				new Dictionary<string, int> {{"Cargo", 4}},
				new Dictionary<string, int> {{"Cargo", 5}},
			}),
			new Skill("Cannons", 1, 5, new List<int> {0, 10, 20, 30, 50}, new List<string> {"Firepower"}, new List<Dictionary<string, int>> {
				new Dictionary<string, int> {{"Firepower", 10}},
				new Dictionary<string, int> {{"Firepower", 20}},
				new Dictionary<string, int> {{"Firepower", 30}},
				new Dictionary<string, int> {{"Firepower", 40}},
				new Dictionary<string, int> {{"Firepower", 50}},
			}),
			new Skill("Navigation", 1, 5, new List<int> {0, 10, 20, 30, 50}, new List<string> (), new List<Dictionary<string, int>> ()),
			new Skill("Something else", 1, 5, new List<int> {0, 10, 20, 30, 50}, new List<string> (), new List<Dictionary<string, int>> ())
		};
		Shipments = new List<Shipment> ();
		Action moveAction = new Action ("Move", 0, MoveMode);
		Action infoAction = new Action("Info", 0, ShowInfo);
		actions.Add (moveAction);
		actions.Add (infoAction);
	}

	int CalculateBonus (string statName) {
		foreach (var skill in Skills) {
			if (skill.AffectedStats.Contains(statName) && skill.StatEffects[skill.Level].ContainsKey(statName)) {
				return skill.StatEffects [skill.Level] [statName];
			}
		}
		return 0;
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
