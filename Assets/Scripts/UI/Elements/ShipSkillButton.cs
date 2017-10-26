using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipSkillButton : MonoBehaviour {

	public ShipData ShipData;
	public Image SkillImage;
	public Slider CooldownProgressBar;

	public delegate void ShipSkillButtonClickedEventHandler (ShipSkillButton sender);
	public event ShipSkillButtonClickedEventHandler OnShipSkillButtonClicked;

	public void Click () {
		OnShipSkillButtonClicked (this);
	}
}
