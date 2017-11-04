using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJPlayerShipObject : MonoBehaviour {

	public GameObject LineShooterPrefab;
	public GameObject LineShooter;

	public Image CaptainImage;
	public Text DamageLabel;
	public Text MultiplierLabel;

	public BJShip Ship;

	void Awake () {
		BJGameController.Instance.OnCardsDealt += BJGameController_Instance_OnCardsDealt;
	}

	void BJGameController_Instance_OnCardsDealt (float multiplier) {
		DamageLabel.gameObject.SetActive (true);
		DamageLabel.text = Ship.BaseDamage + "";
		MultiplierLabel.gameObject.SetActive (true);
		MultiplierLabel.text = "x" + multiplier.ToString("0.0");
	}

	public void DealDamage (float multiplier, BJShipObject enemy) {
		DamageLabel.gameObject.SetActive (false);
		MultiplierLabel.gameObject.SetActive (false);
		GameObject lineShooterObject = Instantiate (LineShooterPrefab) as GameObject;
		lineShooterObject.transform.position = transform.position;
		lineShooterObject.GetComponent<LineRenderer> ().SetPositions (new Vector3 [] {new Vector3(transform.position.x, transform.position.y, -7.0f),
			new Vector3(enemy.transform.position.x, enemy.transform.position.y, -7.0f)});
		LineShooter = lineShooterObject;
		StartCoroutine (TurnOffEffects ());
		Ship.DealDamage (multiplier, enemy.Ship);
	}

	IEnumerator TurnOffEffects () {
		yield return new WaitForSeconds (0.25f);
		Destroy (LineShooter);
	}
}
