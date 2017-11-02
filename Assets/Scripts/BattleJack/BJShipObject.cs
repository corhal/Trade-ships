using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJShipObject : MonoBehaviour {

	public Slider HPSlider;
	public BJShip Ship;

	void Start () {
		Ship.OnDamageTaken += Ship_OnDamageTaken;
		HPSlider.maxValue = Ship.MaxHP;
		HPSlider.value = Ship.HP;
	}

	void Ship_OnDamageTaken () {
		HPSlider.value = Ship.HP;
	}

	public void DealDamage (float multiplier, BJPlayer player) {
		Ship.DealDamage (multiplier, player);
	}
}
