using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : Selectable {	

	bool initialized;
	public GameObject CannonBallPrefab;

	public List<Effect> Effects;
	public List<Skill> Skills;
	public List<Shipment> Shipments;
	public Slider CargoSlider;

	[SerializeField]
	int shipmentsCapacity;
	public int ShipmentsCapacity { get { return shipmentsCapacity; } }
	int hp;
	[SerializeField]
	int maxHp;
	public int MaxHP { get { return maxHp; } }
	public int HP { get { return battleship.HP; } }
	[SerializeField]
	int power;
	public int Power { get { return power; } }
	MoveOnClick mover;

	BattleShip battleship;

	public int TotalWeight { get {
			int totalWeight = 0;
			foreach (var myShipment in Shipments) {
				totalWeight += myShipment.Cargo;
			}
			return totalWeight;
		}}

	protected override void Awake () {
		base.Awake ();
		mover = gameObject.GetComponent<MoveOnClick> ();
		battleship = gameObject.GetComponent<BattleShip> ();
		mover.OnStartedMoving += Mover_OnStartedMoving;
	}

	protected override void Start () {
		base.Start ();

		Process = "Moving";
		Action moveAction = new Action ("Move", 0, gameManager.ActionIconsByNames["Move"], MoveMode);
		actions.Add (moveAction);

		if (initialized) {
			return;
		}
		Skills = new List<Skill> {			
			new Skill("Trade", 0, 5, new List<int> {0, 10, 20, 30, 50}, new Effect("Trade", 0, -1.0f, new List<Dictionary<string, int>> {
				new Dictionary<string, int> {{"Cargo", 0}},
				new Dictionary<string, int> {{"Cargo", 1}},
				new Dictionary<string, int> {{"Cargo", 2}},
				new Dictionary<string, int> {{"Cargo", 3}},
				new Dictionary<string, int> {{"Cargo", 4}},
				new Dictionary<string, int> {{"Cargo", 5}},
			})),
			new Skill("Cannons", 0, 5, new List<int> {0, 10, 20, 30, 50}, new Effect("Cannons", 0, -1.0f, new List<Dictionary<string, int>> {
				new Dictionary<string, int> {{"Firepower", 0}},
				new Dictionary<string, int> {{"Firepower", 10}},
				new Dictionary<string, int> {{"Firepower", 20}},
				new Dictionary<string, int> {{"Firepower", 30}},
				new Dictionary<string, int> {{"Firepower", 40}},
				new Dictionary<string, int> {{"Firepower", 50}},
			})),
			new Skill("Navigation", 0, 5, new List<int> {0, 10, 20, 30, 50}, null),
			new Skill("Something else", 0, 5, new List<int> {0, 10, 20, 30, 50}, null)
		};
		Shipments = new List<Shipment> ();
		CargoSlider.maxValue = ShipmentsCapacity;
		CargoSlider.value = 0.0f; // kek no
		battleship.HP = maxHp;
		battleship.SetMaxHP (maxHp);
		battleship.FirePower = Power;
	}

	public void InitializeFromData (ShipData shipData) {		
		Skills = new List<Skill> (shipData.Skills);	
		Shipments = new List<Shipment> (shipData.Shipments);
		
		Level = shipData.Level;
		Name = shipData.Name;
		shipmentsCapacity = shipData.ShipmentsCapacity;
		Debug.Log (hp.ToString ());
		maxHp = shipData.MaxHP;
		power = shipData.Power;
		transform.position = new Vector3 (shipData.Coordinates[0], shipData.Coordinates[1], shipData.Coordinates[2]);

		CargoSlider.maxValue = ShipmentsCapacity;
		CargoSlider.value = TotalWeight; // kek no
		battleship.HP = shipData.HP;
		battleship.SetMaxHP (maxHp);
		battleship.FirePower = Power;
		initialized = true;
	}

	public override int GetStatByString (string statName) {
		switch (statName) {
		case "Cargo":
			return ShipmentsCapacity;
		case "HP":
			return HP;
		case "MaxHP":
			return MaxHP;
		case "Firepower":
			return Power;
		default:
			return 0;
		}
	}

	void AddStatByString (string statName, int amount) {
		switch (statName) {
		case "Cargo":
			Debug.Log ("shipment capacity: " + shipmentsCapacity);
			shipmentsCapacity += amount;
			Debug.Log ("shipment capacity: " + shipmentsCapacity);
			break;
		case "MaxHP":
			maxHp += amount;
			battleship.SetMaxHP (maxHp);
			break;
		case "Firepower":
			power += amount;
			battleship.FirePower = power;
			break;
		default:
			break;
		}
	}

	void ReduceStatByString (string statName, int amount) {
		switch (statName) {
		case "Cargo":
			shipmentsCapacity -= amount;
			break;
		case "MaxHP":
			maxHp -= amount;
			battleship.SetMaxHP (maxHp);
			break;
		case "Firepower":
			power -= amount;
			battleship.FirePower = power;
			break;
		default:
			break;
		}
	}

	public void ApplyEffect (Effect effect) {
		foreach (var myEffect in Effects) {
			if (effect.Name == myEffect.Name) { // effects don't stack right now
				RemoveEffect (myEffect);
				break;
			}
		}
		Effects.Add (effect.Copy());
		foreach (var statEffect in effect.StatEffects[effect.Level]) {					
			AddStatByString (statEffect.Key, statEffect.Value);					
		}
	}

	public void RemoveEffect (Effect effect) {
		foreach (var statEffect in effect.StatEffects[effect.Level]) {					
			ReduceStatByString (statEffect.Key, statEffect.Value);					
		}
		Effects.Remove (effect);
	}

	public void UpgradeSkill (Skill skill) {		
		if (Skills.Contains(skill)) {			
			if (player.Gold >= skill.UpgradeCosts[skill.Level]) {
				player.GiveGold (skill.UpgradeCosts [skill.Level]);
				skill.Upgrade ();

				if (skill.Effect != null) {
					ApplyEffect (skill.Effect);
				}

				CargoSlider.maxValue = ShipmentsCapacity; // kek
				CargoSlider.value = TotalWeight;
			} else {
				gameManager.OpenPopUp ("Not enough gold!");
			}
		}
	}

	public void TakeShipment (Shipment shipment) {		
		if (ShipmentsCapacity - TotalWeight >= shipment.Cargo) {
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

	public override void ShowInfo () {
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
				if (shipment.Goods.IsForSale) {
					Player.Instance.TakeGold (shipment.Reward);
					shipmentsToDestroy.Add (shipment);
				} else {
					Player.Instance.TakeItems (new Dictionary<Item, int> { { shipment.Goods, 1 } });
					shipmentsToDestroy.Add (shipment);
				}
			}
		}

		foreach (var shipment in shipmentsToDestroy) {
			GiveShipment (shipment);
		}
		shipmentsToDestroy.Clear ();
	}
}
