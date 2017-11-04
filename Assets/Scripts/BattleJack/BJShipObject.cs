using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJShipObject : MonoBehaviour {

	public GameObject LineShooterPrefab;
	public GameObject LineShooter;

	public Slider HPSlider;
	public BJShip Ship;

	void Awake () {
	}

	void Start () {
		Ship.OnDamageTaken += Ship_OnDamageTaken;
		HPSlider.maxValue = Ship.MaxHP;
		HPSlider.value = Ship.HP;
	}

	void Ship_OnDamageTaken () {
		HPSlider.value = Ship.HP;
	}

	public void DealDamage (float multiplier, BJPlayer player) {
		GameObject lineShooterObject = Instantiate (LineShooterPrefab) as GameObject;
		lineShooterObject.transform.position = transform.position;
		lineShooterObject.GetComponent<LineRenderer> ().SetPositions (new Vector3 [] {new Vector3(transform.position.x, transform.position.y, -7.0f),
			new Vector3(BJGameController.Instance.PlayerHPSlider.transform.position.x, BJGameController.Instance.PlayerHPSlider.transform.position.y, -7.0f)});
		LineShooter = lineShooterObject;
		StartCoroutine (TurnOffEffects ());
		Ship.DealDamage (multiplier, player);
	}

	IEnumerator TurnOffEffects () {
		yield return new WaitForSeconds (0.25f);
		Destroy (LineShooter);
	}
}
