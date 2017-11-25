using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureElement : MonoBehaviour {

	public BJCreature Creature;
	public Text NameLabel;
	public Text LevelLabel;
	public Image PortraitImage;
	public Image SelectionShade;
	public List<GameObject> Stars;

	public delegate void ShipElementClickedEventHandler (CreatureElement sender);
	public event ShipElementClickedEventHandler OnShipElementClicked;

	public void Click () {
		OnShipElementClicked (this);
	}
}
