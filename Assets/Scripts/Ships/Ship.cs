using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RankColor {
	White, Green, GreenP, Blue, BlueP, BluePP, Purple, PurpleP, PurplePP, PurplePPP, PurplePPPP, Orange, OrangeP
}

public class Ship : Selectable {	
	public List<RewardChest> RewardChests { get { return ShipData.RewardChests; } set { ShipData.RewardChests = value; } }

	public ShipData ShipData;

	public GameObject ShipwreckPrefab;
	public GameObject CannonBallPrefab;

	public Slider CargoSlider;

	public Dictionary <string, ParticleSystem> ParticlesByEffectNames;

	// ------------------------------------------------------

	public bool IsSummoned { get { return ShipData.IsSummoned; } set { ShipData.IsSummoned = value; } } 

	public int MaxHP { get { return ShipData.MaxHP; } set { ShipData.MaxHP = value; } }
	public int HP { get { return ShipData.HP; } set { ShipData.HP = value; } } // not a great solution
	public int Power { get { return ShipData.Power; } set { ShipData.Power = value; } }

	public float SecPerShot { get { return ShipData.SecPerShot; } set { ShipData.SecPerShot = value; } }
	public float AttackRange { get { return ShipData.AttackRange; } set { ShipData.AttackRange = value; } }

	MoveOnClick mover;

	BattleShip battleship;

	protected override void Awake () {
		base.Awake ();
		mover = gameObject.GetComponent<MoveOnClick> ();
		battleship = gameObject.GetComponent<BattleShip> ();
		mover.OnStartedMoving += Mover_OnStartedMoving;
		battleship.OnBattleShipDestroyed += Battleship_OnBattleShipDestroyed;
		ParticlesByEffectNames = new Dictionary<string, ParticleSystem> ();
	}

	void Battleship_OnBattleShipDestroyed (BattleShip sender) {
		SpawnShipwreck ();
	}

	public void Summon () {
		IsSummoned = true;
	}

	protected override void Start () {
		base.Start ();
		mover.Speed = ShipData.Speed;
		StatNames = new List<string> {
			"Cargo",
			"HP",
			"MaxHP",
			"Firepower",
			"Range",
			"Attack speed",
			"Speed",
		};
		Process = "Moving";
		Action moveAction = new Action ("Move", 0, player.DataBase.ActionIconsByNames["Move"], MoveMode);
		actions.Add (moveAction);

		Name = ShipData.Name;
		Allegiance = ShipData.Allegiance;

		transform.position = new Vector3 (ShipData.Coordinates[0], ShipData.Coordinates[1], ShipData.Coordinates[2]);
	}


	protected override void Update () {
		base.Update ();
	}

	public override int GetStatByString (string statName) {
		return ShipData.GetStatByString (statName);
	}

	void AddStatByString (string statName, int amount) {
		ShipData.AddStatByString (statName, amount);
		mover.Speed = ShipData.Speed;
	}

	void ReduceStatByString (string statName, int amount) {
		AddStatByString (statName, -amount);
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
		gameManager.OpenShipWindow (this.ShipData);
	}

	void OnTriggerEnter2D (Collider2D other) { // will work even when passing through another port
		if (Allegiance != Allegiance.Enemy && other.gameObject.GetComponent<Shipwreck> () != null) {
			Dictionary<string, int> rewards = new Dictionary<string, int> ();
			foreach (var rewardChest in other.gameObject.GetComponent<Shipwreck> ().RewardChests) {
				player.TakeItems (rewardChest.RewardItems);
				foreach (var amountByItem in rewardChest.RewardItems) {
					if (!rewards.ContainsKey(amountByItem.Key)) {
						rewards.Add (amountByItem.Key, amountByItem.Value);
					} else {
						rewards [amountByItem.Key] += amountByItem.Value;
					}
				}
			}
			gameManager.OpenImagesPopUp ("Reward: ", rewards);
			Destroy (other.gameObject);
		}
	}


	public void SpawnShipwreck () {
		GameObject shipwreckObject = Instantiate (ShipwreckPrefab) as GameObject;
		shipwreckObject.transform.position = transform.position;
		Shipwreck shipwreck = shipwreckObject.GetComponent<Shipwreck> ();
		foreach (var rewardChest in RewardChests) {
			shipwreck.RewardChests.Add (rewardChest);
		}
	}
}
