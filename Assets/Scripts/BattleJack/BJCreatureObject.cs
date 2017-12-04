using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJCreatureObject : MonoBehaviour {

	public GameObject FlyingTextPrefab;

	public bool IsStunned = false;
	public bool IsDead = false;

	public Slider HPSlider;
	public BJCreature Creature;
	public string Name { get { return Creature.Name; } }

	public Image SelectionCircle;
	public Image CreatureImage;
	public Image HPFill;

	public BJSkill CurrentSkill;
	public List<BJSkill> Skills;

	public delegate void CreatureEventHandler (BJCreatureObject creatureObject);
	public event CreatureEventHandler OnCreatureObjectClicked;
	public event CreatureEventHandler OnCreatureMovementFinished;
	public event CreatureEventHandler OnCreatureReadyForTurn;
	public event CreatureEventHandler OnCreatureTurnFinished;

	public bool IsAttacking;
	bool shouldMove;
	public Vector3 InitialPosition;
	Vector3 secondaryPosition;
	Vector3 targetPosition;

	public List<BJEffect> Effects;
	public List<GameObject> EffectIcons;
	public GameObject EffectIconPrefab;
	public GameObject EffectIconsContainer;

	void Awake () {
		InitialColor = CreatureImage.color;
	}

	void Start () {
		Creature.OnDamageTaken += Creature_OnDamageTaken;
		Creature.OnDodge += Creature_OnDodge;
		Creature.OnMiss += Creature_OnMiss;
		HPSlider.maxValue = Creature.MaxHP;
		HPSlider.value = Creature.HP;
		InitialPosition = transform.position;
		Skills [0].Damage = Creature.BaseDamage;
		CurrentSkill = Skills [0];
	}

	void ShowFlyingText (string message, Color color) {
		GameObject flyingTextObject = Instantiate (FlyingTextPrefab) as GameObject;
		flyingTextObject.transform.SetParent (BJGameController.Instance.BattleHud.transform);
		flyingTextObject.transform.localScale = Vector3.one;
		flyingTextObject.transform.position = new Vector3 (transform.position.x, transform.position.y + 0.5f, transform.position.z);
		BJFlyingText flyingText = flyingTextObject.GetComponent<BJFlyingText> ();
		flyingText.Label.color = color;
		flyingText.Label.text = message;
	}

	void Creature_OnDodge (int amount) {
		ShowFlyingText ("Dodge!", Color.blue);
	}

	void Creature_OnMiss (int amount) {
		ShowFlyingText ("Miss!", Color.blue);
	}

	public void AddSkill (BJSkill skill) {
		BJSkill skillCopy = Instantiate (skill);
		Skills.Add (skillCopy);
		skillCopy.CurrentUser = this;
		OnCreatureMovementFinished += skillCopy.User_OnCreatureMovementFinished;
		skillCopy.OnSkillFinished += CurrentSkill_OnSkillFinished;
	}

	public void ApplyEffect (BJEffect effect) { // effects will strangely stack for now
		BJEffect effectCopy = Instantiate (effect);
		Effects.Add (effectCopy);
		effectCopy.Victim = this;
		GameObject effectIconObject = Instantiate (EffectIconPrefab) as GameObject;
		effectIconObject.GetComponent<Image> ().sprite = effectCopy.EffectIcon;
		effectIconObject.transform.SetParent (EffectIconsContainer.transform);
		effectIconObject.transform.localScale = Vector3.one;
		EffectIcons.Add (effectIconObject);
		effectCopy.Activate ();
	}

	void CurrentSkill_OnSkillFinished (BJSkill sender) {
		FinishTurn (sender);
	}

	bool isFinishingTurn;
	void FinishTurn (BJSkill sender) {
		isFinishingTurn = true;
		if (Creature.HP > 0) {
			List<BJEffect> effectsToRemove = new List<BJEffect> ();
			foreach (var effect in Effects) {	
				if (effect.Duration <= effect.CurrentLifetime) {
					effect.Deactivate ();
					int index = Effects.IndexOf (effect);
					if (index >= EffectIcons.Count) {
						Debug.Log ("Something went wrong with " + Name + "'s effects");
					} else {
						Destroy (EffectIcons [index]);
						EffectIcons.RemoveAt (index);
					}
					effectsToRemove.Add (effect);
					// Destroy (effect);
				}
			}
			for (int i = Effects.Count - 1; i >= 0; i--) {
				if (effectsToRemove.Contains(Effects [i])) {
					Destroy (Effects [i]);
					Effects.Remove (Effects [i]);
				}
			}
			effectsToRemove.Clear ();
		}
		if (/*Creature.HP > 0 &&*/ OnCreatureTurnFinished != null) {
			if (sender == null || !sender.IsPassive) {
				OnCreatureTurnFinished (this);
			}
		}
	}

	void Creature_OnDamageTaken (int amount) {
		HPSlider.value = Creature.HP;
		string signString = (amount <= 0) ? "+" : "-";
		Color color = (signString == "+") ? Color.green : Color.red;
		string message = signString + Mathf.Abs(amount);
		ShowFlyingText (message, color);

		showHit = true;
		startTintTime = Time.time;

		if (Creature.HP <= 0 && ! IsDead) {
			IsDead = true;
			gameObject.SetActive (false);
			if (!isFinishingTurn) {
				FinishTurn (null);
			}
		}
	}

	public void Attack (BJCreatureObject enemyCreature) {
		if (!IsAttacking) {
			IsAttacking = true;
			UseSkill (enemyCreature, CurrentSkill);
		}
	}

	public void UseSkill (BJCreatureObject target, BJSkill skill) {		
		skill.UseSkill (this, target);
	}

	public void DealDamage (int damage, float multiplier, BJCreatureObject enemy) {	
		foreach (var effect in enemy.Effects) {
			if (effect is BJDelayDamageEffect) {
				(effect as BJDelayDamageEffect).DelayedDamage += damage;
				return;
			}

			if (effect is BJAdjustmentFireEffect && effect.Applier == this) {
				damage += effect.Damage;
			}
		}
		Creature.DealDamage (damage, multiplier, enemy.Creature);
	}

	public void MoveToPoint (Vector3 target) {
		shouldMove = true;
		startTime = Time.time;
		secondaryPosition = transform.position;
		targetPosition = target;
		journeyLength = Vector3.Distance(secondaryPosition, targetPosition );
	}

	public float MoveSpeed = 10.0F;
	private float startTime;
	private float journeyLength;

	public Color InitialColor;
	bool animate;

	bool showHit;
	public float TintTime;
	float startTintTime;

	float t;

	void Update () {
		if (animate) {
			CreatureImage.color = Color.Lerp(InitialColor, Color.black, Mathf.PingPong(Time.time, 1));
		}

		if (showHit) {						
			CreatureImage.color = Color.Lerp(InitialColor, Color.red, t);
			if (t < 1){
				t += Time.deltaTime/TintTime;
			} else {
				showHit = false;
				CreatureImage.color = InitialColor;
			}
		}

		if (shouldMove) {
			float distCovered = (Time.time - startTime) * MoveSpeed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(secondaryPosition, targetPosition, fracJourney);
			if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
				shouldMove = false;
				if (OnCreatureMovementFinished != null) {
					OnCreatureMovementFinished (this);
				}
			}
		}
	}

	public void GetReadyForTurn () {
		isFinishingTurn = false;
		IsAttacking = false;
		for (int i = Effects.Count - 1; i >= 0; i--) {
			if (Effects [i] == null) {
				Effects.Remove (Effects [i]);
			}
		}
		foreach (var effect in Effects) {			
			if (effect.Duration > effect.CurrentLifetime && (effect.TickPeriod == 0 || effect.CurrentLifetime % effect.TickPeriod == 0)) {
				effect.Tick ();
			}
		}
		foreach (var skill in Skills) {
			skill.CurrentCooldown = Mathf.Max (0, skill.CurrentCooldown - 1);
		}
		if (IsStunned) {
			FinishTurn (null);
			return;
		} else if (Creature.HP > 0) {
			OnCreatureReadyForTurn (this);
		}
	}

	public void StartTurn () {		
		Animate ();
	}

	public void Deanimate () {
		animate = false;
		CreatureImage.color = InitialColor;
	}

	public void Animate () {
		animate = true;
	}

	void OnMouseDown () {
		if (OnCreatureObjectClicked != null) {
			OnCreatureObjectClicked (this);
		}
	}
}
