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

	void Awake () {
		// BJGameController.Instance.OnCardsDealt += BJGameController_Instance_OnCardsDealt; should listen from gameController
		initialColor = CreatureImage.color;
	}

	void Start () {
		Creature.OnDamageTaken += Ship_OnDamageTaken;
		HPSlider.maxValue = Creature.MaxHP;
		HPSlider.value = Creature.HP;
	}

	void Ship_OnDamageTaken () {
		HPSlider.value = Creature.HP;
		if (Creature.HP <= 0) {
			gameObject.SetActive (false);
		}
	}

	public void DealDamage (float multiplier, BJPlayer player) {
		GameObject lineShooterObject = Instantiate (LineShooterPrefab) as GameObject;
		lineShooterObject.transform.position = transform.position;
		lineShooterObject.GetComponent<LineRenderer> ().SetPositions (new Vector3 [] {new Vector3(transform.position.x, transform.position.y, -7.0f),
			new Vector3(BJGameController.Instance.PlayerHPSlider.transform.position.x, BJGameController.Instance.PlayerHPSlider.transform.position.y, -7.0f)});
		LineShooter = lineShooterObject;
		StartCoroutine (TurnOffEffects ());
		Creature.DealDamage (multiplier, player);
	}

	public void DealDamage (float multiplier, BJGameController enemy) {
		GameObject lineShooterObject = Instantiate (LineShooterPrefab) as GameObject;
		lineShooterObject.transform.position = transform.position;
		lineShooterObject.GetComponent<LineRenderer> ().SetPositions (new Vector3 [] {new Vector3(transform.position.x, transform.position.y, -7.0f),
			new Vector3(enemy.EnemyShipHPSlider.transform.position.x, enemy.EnemyShipHPSlider.transform.position.y, -7.0f)});
		LineShooter = lineShooterObject;
		StartCoroutine (TurnOffEffects ());
		Creature.DealDamage (multiplier, enemy);
	}

	public void DealDamage (float multiplier, BJCreatureObject enemy) {
		//DamageLabel.gameObject.SetActive (false);
		//MultiplierLabel.gameObject.SetActive (false);
		GameObject lineShooterObject = Instantiate (LineShooterPrefab) as GameObject;
		lineShooterObject.transform.position = transform.position;
		lineShooterObject.GetComponent<LineRenderer> ().SetPositions (new Vector3 [] {new Vector3(transform.position.x, transform.position.y, -7.0f),
			new Vector3(enemy.transform.position.x, enemy.transform.position.y, -7.0f)});
		LineShooter = lineShooterObject;
		StartCoroutine (TurnOffEffects ());
		Creature.DealDamage (multiplier, enemy.Creature);
	}


	public void BJGameController_Instance_OnCardsDealt (float multiplier) {
		//DamageLabel.gameObject.SetActive (true);
		//DamageLabel.text = Creature.BaseDamage + "";
		//MultiplierLabel.gameObject.SetActive (true);
		//MultiplierLabel.text = "x" + multiplier.ToString("0.0");
	}

	IEnumerator TurnOffEffects () {
		yield return new WaitForSeconds (0.25f);
		Destroy (LineShooter);
	}

	Color initialColor;
	bool animate;
	void Update () {
		if (animate) {
			CreatureImage.color = Color.Lerp(initialColor, Color.black, Mathf.PingPong(Time.time, 1));
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
