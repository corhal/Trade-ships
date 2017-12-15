using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionWindow : MonoBehaviour {

	public GameObject Window;

	public GameObject CurrentTeamContainer;
	public GameObject EnemiesElementContainer;

	public GameObject EnemyElementPrefab;

	public List<GameObject> CurrentTeamObjects;
	public List<GameObject> EnemyElementObjects;

	public Button StartButton;
	public Text HeaderLabel;

	Mission mission;


	void Awake () {
	}

	public void Open (Mission chosenMission) {		
		Window.SetActive (true);

		this.mission = chosenMission;

		foreach (var currentTeamObject in CurrentTeamObjects) {
			Destroy (currentTeamObject);
		}
		CurrentTeamObjects.Clear ();

		foreach (var enemyElementObject in EnemyElementObjects) {
			Destroy (enemyElementObject);
		}
		EnemyElementObjects.Clear ();

		foreach (var currentTeamMember in Player.Instance.CurrentTeam) {
			if (currentTeamMember == null || currentTeamMember.Name == "") {
				continue;
			}
			GameObject shipElementObject = Instantiate (EnemyElementPrefab) as GameObject;
			ShipElement shipElement = shipElementObject.GetComponent<ShipElement> ();
			if (Player.Instance.BJDataBase.CreaturePortraitsByNames.ContainsKey(currentTeamMember.Name)) {
				shipElement.PortraitImage.sprite = Player.Instance.BJDataBase.CreaturePortraitsByNames [currentTeamMember.Name];
			}
			shipElement.NameLabel.text = currentTeamMember.Name;
			shipElement.LevelLabel.text = currentTeamMember.Level.ToString ();

			shipElement.ShipData = currentTeamMember;

			shipElement.DamageSlider.maxValue = currentTeamMember.MaxHP;
			shipElement.DamageSlider.value = currentTeamMember.MaxHP - currentTeamMember.HP;

			if (currentTeamMember.HP < currentTeamMember.MaxHP) {
				shipElement.HealButton.gameObject.SetActive (true);
			} else {
				shipElement.HealButton.gameObject.SetActive (false);
			}

			shipElementObject.transform.SetParent (CurrentTeamContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			shipElement.OnShipElementClicked += ShipElement_OnShipElementClicked;
			CurrentTeamObjects.Add (shipElementObject);
		}

		List<CreatureData> enemies = new List<CreatureData> (chosenMission.EnemyShips);

		foreach (var enemy in enemies) {
			GameObject shipElementObject = Instantiate (EnemyElementPrefab) as GameObject;
			ShipElement shipElement = shipElementObject.GetComponent<ShipElement> ();
			if (Player.Instance.DataBase.CreaturePortraitsByNames.ContainsKey(enemy.Name)) {
				shipElement.PortraitImage.sprite = Player.Instance.DataBase.CreaturePortraitsByNames [enemy.Name];
			}
			shipElement.NameLabel.text = enemy.Name;
			shipElement.LevelLabel.text = enemy.Level.ToString ();

			shipElementObject.transform.SetParent (EnemiesElementContainer.transform);
			shipElementObject.transform.localScale = Vector3.one;
			EnemyElementObjects.Add (shipElementObject);
		}
	}

	void ShipElement_OnShipElementClicked (ShipElement sender) {
		sender.ShipData.Creature.Heal (sender.ShipData.MaxHP - sender.ShipData.HP);
		RefreshDamageSliders ();

		if (sender.ShipData.IsDead) {
			sender.ShipData.Creature.Resurrect ();
			RefreshDamageSliders ();
			return;
		}
	}

	void RefreshDamageSliders () {
		foreach (var shipObject in CurrentTeamObjects) {
			ShipElement shipElement = shipObject.GetComponent<ShipElement> ();

			shipElement.DamageSlider.maxValue = shipElement.ShipData.MaxHP;
			shipElement.DamageSlider.value = shipElement.ShipData.MaxHP - shipElement.ShipData.HP;

			if (shipElement.ShipData.HP < shipElement.ShipData.MaxHP) {
				shipElement.HealButton.gameObject.SetActive (true);
			} else {
				shipElement.HealButton.gameObject.SetActive (false);
			}
		}
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void StartMission () {		
		int counter = 0;
		foreach (var currentTeamMember in Player.Instance.CurrentTeam) {
			if (currentTeamMember == null || currentTeamMember.Name == "") {
				counter++;
			}
		}
		if (counter == 0) {
			Close ();
			Player.Instance.CurrentMission = mission;
			GameManager.Instance.LoadBattle ();
		} else {
			UIOverlay.Instance.OpenPopUp ("Your team is not full!");
		}
	}

	public void Back () {
		//gameManager.OpenExpeditionWindow(expeditionCenter);
	}
}
