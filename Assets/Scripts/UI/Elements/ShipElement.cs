﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipElement : MonoBehaviour {

	public CreatureData ShipData;
	public Text NameLabel;
	public Text LevelLabel;
	public Image PortraitImage;
	public Image SelectionShade;
	public List<GameObject> Stars;

	public Slider DamageSlider;

	public Button HealButton;

	public delegate void ShipElementClickedEventHandler (ShipElement sender);
	public event ShipElementClickedEventHandler OnShipElementClicked;

	public void Click () {
		OnShipElementClicked (this);
	}
}
