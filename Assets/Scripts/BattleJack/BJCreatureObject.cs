using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJCreatureObject : MonoBehaviour {
	
	public GameObject LineShooterPrefab;
	public GameObject LineShooter;

	public Text DamageLabel;
	public Text MultiplierLabel;

	public Slider HPSlider;
	public BJCreature Creature;

	public Image CreatureImage;
	public Image HPFill;

	public delegate void CreatureObjectClickedEventHandler (BJCreatureObject creatureObject);
	public event CreatureObjectClickedEventHandler OnCreatureObjectClicked;

	public delegate void CreatureTurnFinishedEventHandler (BJCreatureObject creatureObject);
	public event CreatureTurnFinishedEventHandler OnCreatureTurnFinished;

	void Awake () {
		initialColor = CreatureImage.color;
	}

	void Start () {
		Creature.OnDamageTaken += Ship_OnDamageTaken;
		HPSlider.maxValue = Creature.MaxHP;
		HPSlider.value = Creature.HP;
		initialPosition = transform.position;
	}

	void Ship_OnDamageTaken () {
		HPSlider.value = Creature.HP;
		if (Creature.HP <= 0) {
			gameObject.SetActive (false);
		}
	}

	bool moveToEnemy;
	bool moveBack;
	Vector3 initialPosition;
	Vector3 secondaryPosition;
	Vector3 targetPosition;

	public void Attack (BJCreatureObject enemyCreature) {
		if (Creature.AttackType == AttackType.Melee) {
			moveToEnemy = true;
			float xCoord = (Creature.Allegiance == Allegiance.Player) ? 1.0f : -1.0f;
			targetPosition = enemyCreature.transform.position - new Vector3(xCoord, 0.0f, 0.0f);

			startTime = Time.time;
			journeyLength = Vector3.Distance(initialPosition, targetPosition );

			float delay = journeyLength / moveSpeed;
			StartCoroutine(DealDamage(delay, enemyCreature));
		}
	}

	public IEnumerator DealDamage (float delay, BJCreatureObject enemy) {		
		yield return new WaitForSeconds(delay);
		/*GameObject lineShooterObject = Instantiate (LineShooterPrefab) as GameObject;
		lineShooterObject.transform.position = transform.position;
		lineShooterObject.GetComponent<LineRenderer> ().SetPositions (new Vector3 [] {new Vector3(transform.position.x, transform.position.y, -7.0f),
			new Vector3(enemy.transform.position.x, enemy.transform.position.y, -7.0f)});
		LineShooter = lineShooterObject;
		StartCoroutine (TurnOffEffects ());*/
		Creature.DealDamage (2.0f, enemy.Creature);
	}


	public void BJGameController_Instance_OnCardsDealt (float multiplier) {
		
	}

	IEnumerator TurnOffEffects () {
		yield return new WaitForSeconds (0.25f);
		Destroy (LineShooter);
	}

	float moveSpeed = 10.0F;
	private float startTime;
	private float journeyLength;

	Color initialColor;
	bool animate;
	void Update () {
		if (animate) {
			CreatureImage.color = Color.Lerp(initialColor, Color.black, Mathf.PingPong(Time.time, 1));
		}

		if (moveToEnemy) {
			float distCovered = (Time.time - startTime) * moveSpeed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(initialPosition, targetPosition, fracJourney);
			if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
				moveToEnemy = false;
				moveBack = true;
				targetPosition = initialPosition;
				secondaryPosition = transform.position;

				startTime = Time.time;
				journeyLength = Vector3.Distance(secondaryPosition, targetPosition );
			}
		}
		if (moveBack) {
			float distCovered = (Time.time - startTime) * moveSpeed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(secondaryPosition, targetPosition, fracJourney);
			if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
				moveBack = false;
				if (OnCreatureTurnFinished != null) {
					OnCreatureTurnFinished (this);
				}
			}
		}
	}

	public void Deanimate () {
		animate = false;
		CreatureImage.color = initialColor;
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
