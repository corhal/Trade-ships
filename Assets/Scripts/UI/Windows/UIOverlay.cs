using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlay : MonoBehaviour {

	public Text GoldLabel;
	public Text EnergyLabel;
	Player player;

	void Start () {
		player = Player.Instance;
	}

	void Update () { // OMG
		GoldLabel.text = "Gold: " + player.Gold;
		EnergyLabel.text = "Energy: " + player.Energy + "/" + player.MaxEnergy;
	}
}
