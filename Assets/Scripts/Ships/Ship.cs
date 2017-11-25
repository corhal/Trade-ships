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

	public List<Effect> Effects { get { return ShipData.Effects; } set { ShipData.Effects = value; } }
	public List<Skill> Skills { get { return ShipData.Skills; } set { ShipData.Skills = value; } }

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

		Action useSkillAction = new Action ("Skill", 0, player.DataBase.ActionIconsByNames ["Show missions"], UseSkill);
		actions.Add (useSkillAction);

		Name = ShipData.Name;
		Allegiance = ShipData.Allegiance;

		transform.position = new Vector3 (ShipData.Coordinates[0], ShipData.Coordinates[1], ShipData.Coordinates[2]);
	}

	public void UseSkill () {
		Skills [0].Use (this);
	}

	protected override void Update () {
		base.Update ();

		for (int i = Effects.Count - 1; i >= 0; i--) {
			if (Effects [i].Duration > -1.0f) {
				Effects [i].ElapsedTime += Time.deltaTime;
				if (Effects [i].ElapsedTime >= Effects [i].Duration) {
					RemoveEffect (Effects [i]);
				}
			}
		}
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

	public void ApplyEffect (Effect effect) {
		ShipData.ApplyEffect (effect);
		/*foreach (var myEffect in Effects) {
			if (effect.Name == myEffect.Name) { // effects don't stack right now
				RemoveEffect (myEffect);
				break;
			}
		}
		Effects.Add (effect.Copy());*/
		if (effect.EffectParticlesPrefab != null) {
			GameObject effectParticlesObject = Instantiate (effect.EffectParticlesPrefab) as GameObject;
			ParticleSystem effectParticles = effectParticlesObject.GetComponent<ParticleSystem> ();
			effectParticlesObject.transform.SetParent (transform);
			effectParticles.transform.position = transform.position;
			effectParticles.Play ();
			if (!ParticlesByEffectNames.ContainsKey(effect.Name)) {
				ParticlesByEffectNames.Add (effect.Name, effectParticles);
			}
		}
		/*foreach (var statEffect in effect.StatEffects[effect.Level]) {					
			AddStatByString (statEffect.Key, statEffect.Value);					
		}*/
	}

	public void RemoveEffect (Effect effect) {
		ShipData.RemoveEffect (effect);
		/*foreach (var statEffect in effect.StatEffects[effect.Level]) {					
			ReduceStatByString (statEffect.Key, statEffect.Value);					
		}
		Effects.Remove (effect);*/

		if (ParticlesByEffectNames.ContainsKey(effect.Name)) {
			Destroy (ParticlesByEffectNames[effect.Name].gameObject);
			ParticlesByEffectNames.Remove (effect.Name);
		}
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
		// gameManager.OpenShipWindow (this.ShipData);
	}

	void OnTriggerEnter2D (Collider2D other) { // will work even when passing through another port
		if (Allegiance != "Enemy" && other.gameObject.GetComponent<Shipwreck> () != null) {
			Dictionary<Item, int> rewards = new Dictionary<Item, int> ();
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
