using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipListElement : MonoBehaviour {

	public GameObject ItemsParent;
	public List<Image> ItemImages;
	public Slider SoulstonesSlider;
	public Button SummonButton;
	public Button InfoButton;
	public Button UseButton;
	public CreatureData CreatureData;

	public delegate void ShipListElementClickedEventHandler (ShipListElement sender);
	public event ShipListElementClickedEventHandler OnShipListElementClicked;
	public event ShipListElementClickedEventHandler OnShipListElementReadyToSwap;
	public event ShipListElementClickedEventHandler OnInfoButtonClicked;

	public void InfoButtonClick () {
		if (OnInfoButtonClicked != null) {
			OnInfoButtonClicked (this);
		}
	}

	public void Click () {
		if (OnShipListElementClicked != null) {
			OnShipListElementClicked (this);
		}
	}

	public void Swap () {
		if (OnShipListElementReadyToSwap != null) {
			OnShipListElementReadyToSwap (this);
		}
	}

	public void SummonShip () {		
		gameObject.GetComponentInChildren<ShipElement>().ShipData.IsSummoned = true;
	}
}
