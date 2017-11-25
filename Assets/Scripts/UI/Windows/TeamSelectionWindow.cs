using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectionWindow : MonoBehaviour {

	public GameObject Window;

	public GameObject AllShipsElementContainer;
	public GameObject TeamShipsElementContainer;

	public GameObject ShipElementPrefab;

	public List<GameObject> AllShipObjects;
	public List<GameObject> TeamShipObjects;

	public Button StartButton;
	public Text MightLabel;

	Mission mission;
	GameManager gameManager;

	public List<BJCreature> AllCreatures;

	void Awake () {
		gameManager = GameManager.Instance;

	}

	public void Open (Mission chosenMission) {		
		Window.SetActive (true);

		this.mission = chosenMission;

		foreach (var shipObject in AllShipObjects) {
			shipObject.GetComponent<CreatureElement> ().OnShipElementClicked -= ShipElement_OnShipElementClicked;
			Destroy (shipObject);
		}
		AllShipObjects.Clear ();

		foreach (var shipObject in TeamShipObjects) {
			shipObject.GetComponent<CreatureElement> ().OnShipElementClicked -= ShipElement_OnShipElementClicked;
			Destroy (shipObject);
		}
		TeamShipObjects.Clear ();

		AllCreatures = new List<BJCreature> ();
		foreach (var creature in Player.Instance.Creatures) {
			if (creature.Allegiance == Allegiance.Player && creature.IsSummoned) {
				AllCreatures.Add (creature);
			}
		}

		foreach (var creature in AllCreatures) {
			GameObject shipElementObject = CreateShipElementObject (creature);

			shipElementObject.transform.SetParent (AllShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			AllShipObjects.Add (shipElementObject);
		}

		foreach (var creature in Player.Instance.CurrentTeam) {
			GameObject shipElementObject = CreateShipElementObject (creature);

			shipElementObject.transform.SetParent (TeamShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			TeamShipObjects.Add (shipElementObject);
		}
	}

	GameObject CreateShipElementObject (BJCreature creature) {
		GameObject shipElementObject = Instantiate (ShipElementPrefab) as GameObject;
		CreatureElement shipElement = shipElementObject.GetComponent<CreatureElement> ();
		shipElement.Creature = creature;
		shipElement.PortraitImage.sprite = Player.Instance.DataBase.CreaturePortraitsByNames [creature.Name];
		shipElement.NameLabel.text = creature.Name;
		shipElement.LevelLabel.text = creature.Level.ToString ();

		for (int i = 0; i < 5; i++) {
			shipElement.Stars [i].SetActive (false);
		}

		for (int i = 0; i < creature.Stars; i++) {
			shipElement.Stars [i].SetActive (true);
		}

		shipElementObject.GetComponent<Button> ().enabled = true;
		shipElement.OnShipElementClicked += ShipElement_OnShipElementClicked;

		return shipElementObject;
	}

	void ShipElement_OnShipElementClicked (CreatureElement sender) {
		/*if (AllShipObjects.Contains(sender.gameObject) && Player.Instance.HomeTeam.Contains(sender.ShipData)) {
			gameManager.OpenPopUp ("This ship is on the home map. Later this pop-up will offer to speed it up");
		}*/
		if (AllShipObjects.Contains(sender.gameObject) && Player.Instance.CurrentTeam.Count < 5 && !Player.Instance.CurrentTeam.Contains(sender.Creature)) {
			GameObject shipElementObject = CreateShipElementObject (sender.Creature);

			shipElementObject.transform.SetParent (TeamShipsElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			TeamShipObjects.Add (shipElementObject);

			Player.Instance.CurrentTeam.Add (sender.Creature);
		}
		if (TeamShipObjects.Contains(sender.gameObject)) {
			Player.Instance.CurrentTeam.Remove (sender.Creature);
			TeamShipObjects.Remove (sender.gameObject);
			Destroy (sender.gameObject);
		}
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void StartMission () {	
		if (Player.Instance.CurrentTeam.Count > 0) {
			Close ();
			Player.Instance.CurrentMission = mission;
			gameManager.LoadBattle ();
		} else {
			GameManager.Instance.OpenPopUp ("Choose at least one ship!");
		}
	}

	public void Back () {

	}
}
