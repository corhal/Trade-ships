using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RankColor {
	White, Green, GreenP, Blue, BlueP, BluePP, Purple, PurpleP, PurplePP, PurplePPP, PurplePPPP, Orange, OrangeP
}

public class Ship : Selectable {	
	public RewardChest RewardChest;

	public ShipData ShipData;

	public GameObject ShipwreckPrefab;
	public GameObject CannonBallPrefab;

	public Slider CargoSlider;

	public Dictionary <string, ParticleSystem> ParticlesByEffectNames;

	// ------------------------------------------------------

	public bool IsSummoned { get { return ShipData.IsSummoned; } set { ShipData.IsSummoned = value; } } 

	public RankColor RankColor { get { return ShipData.RankColor; } set { ShipData.RankColor = value; } } 

	public Item Blueprint { get { return ShipData.Blueprint; } set { ShipData.Blueprint = value; } }
	public int Stars { get { return ShipData.Stars; } set { ShipData.Stars = value; } }

	public int Exp { get { return ShipData.Exp; } set { ShipData.Exp = value; } }
	public List<int> LevelRequirements { get { return ShipData.LevelRequirements; } set { ShipData.LevelRequirements = value; } }

	public List<Effect> Effects { get { return ShipData.Effects; } set { ShipData.Effects = value; } }
	public List<Skill> Skills { get { return ShipData.Skills; } set { ShipData.Skills = value; } }
	//public List<Shipment> Shipments { get { return ShipData.Shipments; } set { ShipData.Shipments = value; } }

	public List<List<Item>> PromoteCosts { get { return ShipData.PromoteCosts; } set { ShipData.PromoteCosts = value; } }
	public List<int> EvolveCosts;

	//public int ShipmentsCapacity { get { return ShipData.ShipmentsCapacity; } set { ShipData.ShipmentsCapacity = value; } }

	public int MaxHP { get { return ShipData.MaxHP; } set { ShipData.MaxHP = value; } }
	public int HP { get { return ShipData.HP; } set { ShipData.HP = value; } } // not a great solution
	public int Power { get { return ShipData.Power; } set { ShipData.Power = value; } }

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
		StatNames = new List<string> {
			"Cargo",
			"HP",
			"MaxHP",
			"Firepower",
			"Range",
			"Attack speed",
			"Speed",
		};
		EvolveCosts = player.DataBase.EvolveCosts;
		Process = "Moving";
		Action moveAction = new Action ("Move", 0, player.DataBase.ActionIconsByNames["Move"], MoveMode);
		actions.Add (moveAction);

		Action useSkillAction = new Action ("Skill", 0, player.DataBase.ActionIconsByNames ["Show missions"], UseSkill);
		actions.Add (useSkillAction);

		Name = ShipData.Name;
		Allegiance = ShipData.Allegiance;

		transform.position = new Vector3 (ShipData.Coordinates[0], ShipData.Coordinates[1], ShipData.Coordinates[2]);
		LevelRequirements = new List<int> (ShipData.LevelRequirements);
		/*CargoSlider.maxValue = ShipmentsCapacity;
		CargoSlider.value = ShipData.TotalWeight;

		CargoSlider.maxValue = ShipmentsCapacity;
		CargoSlider.value = ShipData.TotalWeight;*/
		battleship.HP = MaxHP;
		battleship.SetMaxHP (MaxHP);
		battleship.FirePower = Power;
		battleship.Allegiance = Allegiance;
	}

	void UseSkill () {
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
		switch (statName) {
		/*case "Cargo":
			return ShipmentsCapacity;*/
		case "HP":
			return HP;
		case "MaxHP":
			return MaxHP;
		case "Firepower":
			return Power;
		case "Range":
			return (int)(battleship.AttackRange * 1000.0f);
		case "Attack speed":
			return (int)(battleship.SecPerShot * 1000.0f);
		case "Speed":
			return (int)(mover.Speed * 1000.0f);
		default:
			return 0;
		}
	}

	void AddStatByString (string statName, int amount) {
		switch (statName) {
		/*case "Cargo":
			ShipmentsCapacity += amount;
			break;*/
		case "MaxHP":
			MaxHP += amount;
			battleship.SetMaxHP (MaxHP);
			break;
		case "Firepower":
			Power += amount;
			battleship.FirePower = Power;
			break;
		case "Range":
			battleship.AttackRange += (float)amount / 1000.0f; // munits
			break;
		case "Attack speed":
			battleship.SecPerShot += (float)amount / 1000.0f; // msec
			break;
		case "Speed":
			mover.Speed += (float)amount / 1000.0f; // munits
			break;
		default:
			break;
		}
	}

	void ReduceStatByString (string statName, int amount) {
		AddStatByString (statName, -amount);
	}

	public void ApplyEffect (Effect effect) {
		foreach (var myEffect in Effects) {
			if (effect.Name == myEffect.Name) { // effects don't stack right now
				RemoveEffect (myEffect);
				break;
			}
		}
		Effects.Add (effect.Copy());
		if (effect.EffectParticlesPrefab != null) {
			GameObject effectParticlesObject = Instantiate (effect.EffectParticlesPrefab) as GameObject;
			ParticleSystem effectParticles = effectParticlesObject.GetComponent<ParticleSystem> ();
			effectParticlesObject.transform.SetParent (transform);
			effectParticles.transform.position = transform.position;
			effectParticles.Play ();
			ParticlesByEffectNames.Add (effect.Name, effectParticles);
		}
		foreach (var statEffect in effect.StatEffects[effect.Level]) {					
			AddStatByString (statEffect.Key, statEffect.Value);					
		}
	}

	public void RemoveEffect (Effect effect) {
		foreach (var statEffect in effect.StatEffects[effect.Level]) {					
			ReduceStatByString (statEffect.Key, statEffect.Value);					
		}
		Effects.Remove (effect);

		if (ParticlesByEffectNames.ContainsKey(effect.Name)) {
			Destroy (ParticlesByEffectNames[effect.Name].gameObject);
			ParticlesByEffectNames.Remove (effect.Name);
		}
	}

	public void UpgradeSkill (Skill skill) {		
		if (Skills.Contains(skill)) {			
			if (player.Gold >= skill.UpgradeCosts[skill.Level]) {
				player.GiveGold (skill.UpgradeCosts [skill.Level]);
				skill.Upgrade ();

				foreach (var effectByTarget in skill.EffectsByTargets) {
					if (effectByTarget.Value != null && effectByTarget.Key == "self" && effectByTarget.Value.Duration == -1.0f) { // kinda sorta determine if skill is passive
						ApplyEffect (effectByTarget.Value);
					}
				}

				// CargoSlider.maxValue = ShipmentsCapacity; // kek
				CargoSlider.value = ShipData.TotalWeight;
			} else {
				gameManager.OpenPopUp ("Not enough gold!");
			}
		}
	}



	/*public void TakeShipment (Shipment shipment) {		
		ShipData.TakeShipment (shipment);
		CargoSlider.value = ShipData.TotalWeight;
	}

	public void GiveShipment (Shipment shipment) {
		ShipData.GiveShipment (shipment);
		CargoSlider.value = ShipData.TotalWeight;
	}*/

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
		if (Allegiance != "Enemy" && other.gameObject.GetComponent<Port> () != null) {
			//UnloadCargo (other.gameObject.GetComponent<Port> ());
		}
	}

	/*public void UnloadCargo (Port port) {
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
					Player.Instance.TakeItems (new Dictionary<Item, int> { { shipment.Goods, 1 } });
					shipmentsToDestroy.Add (shipment);
				}
			}
		}

		foreach (var shipment in shipmentsToDestroy) {
			GiveShipment (shipment);
		}
		shipmentsToDestroy.Clear ();
	}*/

	public void SpawnShipwreck () {
		GameObject shipwreckObject = Instantiate (ShipwreckPrefab) as GameObject;
		shipwreckObject.transform.position = transform.position;
		/*Port shipwreckPort = shipwreckObject.GetComponent<Port> ();
		shipwreckPort.ShipmentsCapacities [1] = Shipments.Count;
		foreach (var shipment in Shipments) {
			shipwreckPort.TakeShipment (shipment);
		}*/
	}

	public void AddExp (int amount) {
		Exp += amount;
		if (Exp >= LevelRequirements[Level]) {
			LevelUp ();
		}
	}

	public void LevelUp () {
		Level += 1;
	}

	public void PromoteRank () {
		for (int i = 0; i < PromoteCosts[(int)RankColor].Count; i++) {
			Item item = PromoteCosts [(int)RankColor] [i];

			if (!Player.Instance.Inventory.ContainsKey(item) || Player.Instance.Inventory[item] == 0) {
				gameManager.OpenPopUp ("Not enough items!");
				return;
			}
		}

		foreach (var item in PromoteCosts[(int)RankColor]) {
			player.GiveItems (new Dictionary<Item, int> { { item, 1 } });
		}

		RankColor += 1;
	}

	public void EvolveStar () {
		if (!Player.Instance.Inventory.ContainsKey(Blueprint) || Player.Instance.Inventory[Blueprint] < EvolveCosts[Stars]) {
			gameManager.OpenPopUp ("Not enough blueprints!");
			return;
		}
		player.GiveItems (new Dictionary<Item, int> { { Blueprint, EvolveCosts [Stars] } });
		Stars += 1;
	}
}
