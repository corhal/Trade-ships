using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlay : MonoBehaviour {

	public Text GoldLabel;
	Player player;

	void Start () {
		player = Player.Instance;
	}

	void Update () {
		GoldLabel.text = "Gold: " + player.Gold;
	}
}
