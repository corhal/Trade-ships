using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public BJTooltip Tooltip;

	void Awake () {
		BJSkillButton.OnSkillButtonClicked += BJSkillButton_OnSkillButtonClicked;
		BJSkillButton.OnSkillButtonReleased += BJSkillButton_OnSkillButtonReleased;
	}

	void BJSkillButton_OnSkillButtonClicked (GameObject sender) {
		Tooltip.gameObject.SetActive (true);
		Tooltip.transform.position = new Vector3 (sender.transform.position.x + 1.0f, sender.transform.position.y + 1.0f, Tooltip.transform.position.z);
		foreach (var skillButton in BJGameController.Instance.SkillButtons) {
			if (sender.GetComponent<Button> () == skillButton) {
				BJSkill skill = BJGameController.Instance.CurrentCreatureObject.Skills [BJGameController.Instance.SkillButtons.IndexOf (sender.GetComponent<Button> ()) + 1];
				Tooltip.Header.text = skill.Name;
				Tooltip.MainText.text = skill.GetInfo ();
				break;
			}
		}
	}

	void BJSkillButton_OnSkillButtonReleased (GameObject sender) {
		Tooltip.gameObject.SetActive (false);
	}

	/*public void ShowToolTip (GameObject caller, string header, string mainText) {

	}*/
}
