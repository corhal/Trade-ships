using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : Selectable {	

	bool initialized;
	public GameObject CannonBallPrefab;

	public List<Effect> Effects;
	public List<Skill> Skills;
	public List<string> SkillNames;
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
		ParticlesByEffectNames = new Dictionary<string, ParticleSystem> ();
	}

	protected override void Start () {
		base.Start ();

		Process = "Moving";
		Action moveAction = new Action ("Move", 0, gameManager.ActionIconsByNames["Move"], MoveMode);
		actions.Add (moveAction);

		Action useSkillAction = new Action ("Skill", 0, gameManager.ActionIconsByNames ["Show missions"], UseSkill);
		actions.Add (useSkillAction);

		if (initialized) {
			return;
		}

		Skills = new List<Skill> ();
		foreach (var skillName in SkillNames) { // will not save between scenes!
			Skills.Add (gameManager.SkillsByNames [skillName]);
		}

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
		case "Cargo":
			return ShipmentsCapacity;
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
		case "Cargo":
			shipmentsCapacity += amount;
			break;
		case "MaxHP":
			maxHp += amount;
			battleship.SetMaxHP (maxHp);
			break;
		case "Firepower":
			power += amount;
			battleship.FirePower = power;
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
		case "Range":
			battleship.AttackRange -= (float)amount / 1000.0f; // munits
			break;
		case "Attack speed":
			battleship.SecPerShot -= (float)amount / 1000.0f; // msec
			break;
		case "Speed":
			mover.Speed -= (float)amount / 1000.0f; // munits
			break;
		default:
			break;
		}
	}

	public Dictionary <string, ParticleSystem> ParticlesByEffectNames;

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
