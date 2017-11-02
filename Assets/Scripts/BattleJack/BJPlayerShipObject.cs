using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJPlayerShipObject : MonoBehaviour {

	public Image CaptainImage;
	public Text DamageLabel;
	public Text MultiplierLabel;

	public BJShip Ship;

	public void DealDamage (float multiplier, BJShipObject enemy) {
		DamageLabel.gameObject.SetActive (true);
		DamageLabel.text = Ship.BaseDamage + "";
		MultiplierLabel.gameObject.SetActive (true);
		MultiplierLabel.text = multiplier.ToString("0.0");
		Ship.DealDamage (multiplier, enemy.Ship);
	}
}
