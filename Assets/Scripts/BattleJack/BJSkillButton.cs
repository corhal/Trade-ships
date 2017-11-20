using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void UIObjectInteractedEventHandler (GameObject sender);
	
public class BJSkillButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public Slider CooldownSlider;
	public Text CooldownLabel;
	public Image ButtonImage;
	public Text ManaLabel;

	bool shouldShowTooltip;
	float timer = 0.0f;
	float delay = 0.25f;

	public static event UIObjectInteractedEventHandler OnSkillButtonClicked;
	public static event UIObjectInteractedEventHandler OnSkillButtonReleased;

	void Update () {
		if (shouldShowTooltip) {
			timer += Time.deltaTime;
			if (timer > delay) {
				OnSkillButtonClicked (this.gameObject);
				shouldShowTooltip = false;
			}
		}
	}

	public void OnPointerDown (PointerEventData eventData) {
		shouldShowTooltip = true;
		timer = 0.0f;
	}

	public void OnPointerUp (PointerEventData eventData) {
		shouldShowTooltip = false;
		OnSkillButtonReleased (this.gameObject);
	}
}
