using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : Selectable {	

	public List<Skill> Skills;
	public List<Shipment> Shipments;

	public List<string> StatNames;
	public Slider CargoSlider;

	[SerializeField]
	int shipmentsCapacity;
	public int ShipmentsCapacity { get { return shipmentsCapacity + CalculateBonus("Cargo"); } }
	[SerializeField]
	int hp;
	public int HP { get { return hp + CalculateBonus("HP"); } }
	[SerializeField]
	int power;
	public int Power { get { return power + CalculateBonus("Firepower"); } }
	MoveOnClick mover;


	public int TotalWeight { get {
			int totalWeight = 0;
			foreach (var myShipment in Shipments) {
				totalWeight += myShipment.Cargo;
			}
			return totalWeight;
		}}

	new void Awake () {
		base.Awake ();
		mover = gameObject.GetComponent<MoveOnClick> ();
		mover.OnStartedMoving += Mover_OnStartedMoving;
	}

	new void Start () {
		base.Start ();

		Process = "Moving";
		StatNames = new List<string> { "Cargo", "HP", "Firepower" };

		Skills = new List<Skill> {			
			new Skill("Trade", 1, 5, new List<int> {0, 10, 20, 30, 50}, new List<string> {"Cargo"}, new List<Dictionary<string, int>> {
				new Dictionary<string, int> {{"Cargo", 1}},
				new Dictionary<string, int> {{"Cargo", 2}},
				new Dictionary<string, int> {{"Cargo", 3}},
				new Dictionary<string, int> {{"Cargo", 4}},
				new Dictionary<string, int> {{"Cargo", 5}},
				new Dictionary<string, int> {{"Cargo", 6}},
			}),
			new Skill("Cannons", 1, 5, new List<int> {0, 10, 20, 30, 50}, new List<string> {"Firepower"}, new List<Dictionary<string, int>> {
				new Dictionary<string, int> {{"Firepower", 10}},
				new Dictionary<string, int> {{"Firepower", 20}},
				new Dictionary<string, int> {{"Firepower", 30}},
				new Dictionary<string, int> {{"Firepower", 40}},
				new Dictionary<string, int> {{"Firepower", 50}},
				new Dictionary<string, int> {{"Firepower", 60}},
			}),
			new Skill("Navigation", 1, 5, new List<int> {0, 10, 20, 30, 50}, new List<string> (), new List<Dictionary<string, int>> ()),
			new Skill("Something else", 1, 5, new List<int> {0, 10, 20, 30, 50}, new List<string> (), new List<Dictionary<string, int>> ())
		};
		Shipments = new List<Shipment> ();
		Action moveAction = new Action ("Move", 0, gameManager.ActionIconsByNames["Move"], MoveMode);
		Action infoAction = new Action("Info", 0, gameManager.ActionIconsByNames["Info"], ShowInfo);
		actions.Add (moveAction);
		actions.Add (infoAction);
		CargoSlider.maxValue = ShipmentsCapacity;
		CargoSlider.value = 0.0f; // kek no
	}

	public int GetStatByString (string statName) {
		switch (statName) {
		case "Cargo":
			return ShipmentsCapacity;
		case "HP":
			return HP;
		case "Firepower":
			return Power;
		default:
			return 0;
		}
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
				CargoSlider.maxValue = ShipmentsCapacity; // kek
				CargoSlider.value = TotalWeight;
			} else {
				gameManager.OpenPopUp ("Not enough gold!");
			}
		}
	}

	public void TakeShipment (Shipment shipment) {		
		if (TotalWeight < ShipmentsCapacity) {
			Shipments.Add (shipment);
			CargoSlider.value = TotalWeight;
		}
	}

	public void GiveShipment (Shipment shipment) {
		Shipments.Remove (shipment);
		CargoSlider.value = TotalWeight;
	}

	public void MoveMode () {
		gameManager.MoveMode ();
		mover.InMoveMode = true;
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
			GiveShipment (shipment);
		}
		shipmentsToDestroy.Clear ();
	}
}
