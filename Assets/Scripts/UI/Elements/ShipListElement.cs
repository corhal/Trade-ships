﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipListElement : MonoBehaviour {

	public GameObject ItemsParent;
	public List<Image> ItemImages;
	public Slider SoulstonesSlider;
	public Button SummonButton;
	public Button InfoButton;

	public delegate void ShipListElementClickedEventHandler (ShipListElement sender);
	public event ShipListElementClickedEventHandler OnShipListElementClicked;

	public void Click () {
		OnShipListElementClicked (this);
	}

	public void Toggle () {
		InfoButton.gameObject.SetActive (!InfoButton.gameObject.activeSelf);
	}

	public void SummonShip () {		
		gameObject.GetComponentInChildren<ShipElement>().ShipData.IsSummoned = true;
	}
}
