using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSkillsWindow : MonoBehaviour {

	/*public GameObject Window;

	public GameObject SkillButtonContainer;

	public GameObject SkillButtonPrefab;

	public List<GameObject> SkillButtons;

	GameManager gameManager;

	void Awake () {
		gameManager = GameManager.Instance;

	}

	public void Open () {		
		Window.SetActive (true);

		foreach (var shipObject in SkillButtons) {
			shipObject.GetComponent<ShipSkillButton> ().OnShipSkillButtonClicked -= SkillButton_OnShipSkillButtonClicked;
			Destroy (shipObject);
		}
		SkillButtons.Clear ();

		foreach (var creature in Player.Instance.CurrentTeam) {
			GameObject skillButtonObject = CreateSkillButtonObject (creature);

			skillButtonObject.transform.SetParent (SkillButtonContainer.transform);
			skillButtonObject.transform.localScale = Vector3.one;
			SkillButtons.Add (skillButtonObject);
		}
	}

	GameObject CreateSkillButtonObject (BJCreature creature) {
		GameObject skillButtonObject = Instantiate (SkillButtonPrefab) as GameObject;
		ShipSkillButton skillButton = skillButtonObject.GetComponent<ShipSkillButton> ();
		skillButton.ShipData = shipData;
		// skillButton.SkillImage = Player.Instance.DataBase.im // add pics

		//skillButtonObject.GetComponent<Button> ().enabled = true;

		skillButton.OnShipSkillButtonClicked += SkillButton_OnShipSkillButtonClicked;

		return skillButtonObject;
	}

	void SkillButton_OnShipSkillButtonClicked (ShipSkillButton sender) {
		// a very bad solution to find specific ship
		Ship myShip = null;
		foreach (var ship in gameManager.Ships) {
			if (ship.ShipData == sender.ShipData) {
				myShip = ship;
			}
		}
		myShip.UseSkill ();
	}
		

	public void Close () {
		Window.SetActive (false);
	}
		
	public void Back () {

	}*/
}
