using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJCreatureObject : MonoBehaviour {

	public string Name;
	public bool IsStunned = false;
	public bool IsDead = false;

	public Slider HPSlider;
	public BJCreature Creature;

	public Image SelectionCircle;
	public Image CreatureImage;
	public Image HPFill;

	public BJSkill CurrentSkill;
	public List<BJSkill> Skills;

	public delegate void CreatureObjectClickedEventHandler (BJCreatureObject creatureObject);
	public event CreatureObjectClickedEventHandler OnCreatureObjectClicked;

	public delegate void CreatureMovementFinishedEventHandler (BJCreatureObject creatureObject);
	public event CreatureMovementFinishedEventHandler OnCreatureMovementFinished;

	public delegate void CreatureTurnFinishedEventHandler (BJCreatureObject creatureObject);
	public event CreatureTurnFinishedEventHandler OnCreatureTurnFinished;

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
		HPSlider.maxValue = Creature.MaxHP;
		HPSlider.value = Creature.HP;
		InitialPosition = transform.position;
		Skills [0].Damage = Creature.BaseDamage;
		CurrentSkill = Skills [0];
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

	public void RemoveEffect (BJEffect effect) {
		int index = Effects.IndexOf (effect);
		Destroy (EffectIcons [index]);
		EffectIcons.RemoveAt (index);
		Effects.Remove (effect);
	}

	void CurrentSkill_OnSkillFinished (BJSkill sender) {
		foreach (var effect in Effects) {	
			if (effect.Duration <= effect.CurrentLifetime) {
				effect.Deactivate ();
				int index = Effects.IndexOf (effect);
				Destroy (EffectIcons [index]);
				EffectIcons.RemoveAt (index);
				Destroy (effect);
			}
		}
		if (Creature.HP > 0 && OnCreatureTurnFinished != null) {
			OnCreatureTurnFinished (this);
		}
	}

	void Creature_OnDamageTaken () {
		HPSlider.value = Creature.HP;
		if (Creature.HP <= 0 && ! IsDead) {
			IsDead = true;
			gameObject.SetActive (false);
			if (OnCreatureTurnFinished != null) {
				OnCreatureTurnFinished (this);
			}
		}
	}

	public void Attack (BJCreatureObject enemyCreature) {
		CurrentSkill.UseSkill (this, enemyCreature);
	}

	public void DealDamage (int damage, float multiplier, BJCreatureObject enemy) {	
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
	void Update () {
		if (animate) {
			CreatureImage.color = Color.Lerp(InitialColor, Color.black, Mathf.PingPong(Time.time, 1));
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

	public void StartTurn () {
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
			if (OnCreatureTurnFinished != null) {
				OnCreatureTurnFinished (this);
			}
		}
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
