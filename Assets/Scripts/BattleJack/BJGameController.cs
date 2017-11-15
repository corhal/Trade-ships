using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJGameController : MonoBehaviour {
	
	public GameObject PlayerCreaturePrefab;
	public GameObject PlayerCreaturesContainer;
	public List<BJCreatureObject> PlayerCreatureObjects;

	public GameObject CreatureObjectsContainer;
	public GameObject CreatureObjectPrefab;
	public List<BJCreatureObject> EnemyCreatureObjects;

	public static BJGameController Instance;

	public List<Transform> PlayerSpawnPoints;
	public List<Transform> EnemySpawnPoints;

	public Queue<BJCreatureObject> TurnQueue;

	void Awake () {
		Instance = this;
	}

	void Start () {		
		SpawnCreatures (5);

		for (int i = 0; i < EnemyCreatureObjects.Count; i++) {
			EnemyCreatureObjects [i].gameObject.transform.SetParent (EnemySpawnPoints [i]);
			EnemyCreatureObjects [i].gameObject.transform.localScale = Vector3.one;
			EnemyCreatureObjects [i].gameObject.transform.localPosition = Vector3.zero;
		}

		SpawnPlayerCreatures ();
		for (int i = 0; i < PlayerCreatureObjects.Count; i++) {
			PlayerCreatureObjects [i].gameObject.transform.SetParent (PlayerSpawnPoints [i]);
			PlayerCreatureObjects [i].gameObject.transform.localScale = Vector3.one;
			PlayerCreatureObjects [i].gameObject.transform.localPosition = Vector3.zero;
		}

		List<BJCreatureObject> AllCreatureObjects = new List<BJCreatureObject> (EnemyCreatureObjects);
		AllCreatureObjects.AddRange (PlayerCreatureObjects);

		/*List<string> CreatureNames = new List<string> {
			"Ron",
			"Harry",
			"Hermione",
			"Jeanny",
			"Draco",
			"Nevill",
			"Crabb",
			"Goyle",
			"Parvati",
			"Chou"
		};
		for (int i = 0; i < CreatureNames.Count; i++) {
			AllCreatureObjects [i].Name = CreatureNames [i];
		}*/
		ApplyPassiveSkills ();
		FormQueue ();
		Invoke ("StartTurn", 0.25f);
	}

	void ApplyPassiveSkills () { // REALLY should optimize that
		List<BJCreatureObject> AllCreatureObjects = new List<BJCreatureObject> (EnemyCreatureObjects);
		AllCreatureObjects.AddRange (PlayerCreatureObjects);
		foreach (var creatureObject in AllCreatureObjects) {
			foreach (var skill in creatureObject.Skills) {
				if (skill.IsPassive) {
					skill.AssignSkillIndexes ();
					int index = 0;
					int indexOfIndex = 0;
					do {
						indexOfIndex = Random.Range (0, skill.ValidTargetIndexes.Count);
						index = skill.ValidTargetIndexes[indexOfIndex];
					} while (PlayerCreatureObjects[index].Creature.HP <= 0);
					List<BJCreatureObject> ourCreatureObjects = (creatureObject.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.PlayerCreatureObjects : BJGameController.Instance.EnemyCreatureObjects;
					List<BJCreatureObject> enemyCreatureObjects = (creatureObject.Creature.Allegiance == Allegiance.Player) ? BJGameController.Instance.EnemyCreatureObjects : BJGameController.Instance.PlayerCreatureObjects;
					if (skill.TargetTeam == Teams.MyTeam) {
						creatureObject.UseSkill (ourCreatureObjects[index], skill);
					} else {
						creatureObject.UseSkill (enemyCreatureObjects[index], skill);
					}
				}
			}
		}
	}


	public void ReformQueue () {
		List<BJCreatureObject> allCreaturesList = new List<BJCreatureObject> ();
		foreach (var creatureObject in TurnQueue) {
			if (creatureObject.Creature.HP > 0) {
				allCreaturesList.Add (creatureObject);
			}
		}

		allCreaturesList.Sort((x,y) =>
			y.Creature.Speed.CompareTo(x.Creature.Speed));

		TurnQueue = new Queue<BJCreatureObject> (allCreaturesList);
	}

	void FormQueue () {
		List<BJCreatureObject> allCreaturesList = new List<BJCreatureObject> ();
		foreach (var playerCreatureObject in PlayerCreatureObjects) {
			if (playerCreatureObject.Creature.HP > 0) {
				allCreaturesList.Add (playerCreatureObject);
			}
		}
		foreach (var enemyCreatureObject in EnemyCreatureObjects) {
			if (enemyCreatureObject.Creature.HP > 0) {
				allCreaturesList.Add (enemyCreatureObject);
			}
		}

		allCreaturesList.Sort((x,y) =>
			y.Creature.Speed.CompareTo(x.Creature.Speed));

		TurnQueue = new Queue<BJCreatureObject> (allCreaturesList);
	}

	public List<Button> SkillButtons;
	BJCreatureObject currentCreatureObject;
	void StartTurn () {
		CheckDead ();
		if (TurnQueue.Count == 0) {
			FormQueue ();
		}
		currentCreatureObject = TurnQueue.Dequeue ();
		if (currentCreatureObject != null && currentCreatureObject.Creature.HP <= 0) {
			StartTurn ();
			return;
		}
		CurrentCreatureChooseSkill (0);

		currentCreatureObject.GetReadyForTurn ();
	}

	void BjCreatureObject_OnCreatureReadyForTurn (BJCreatureObject creatureObject) {
		currentCreatureObject.StartTurn ();

		for (int i = 0; i < SkillButtons.Count; i++) {
			if (i >= currentCreatureObject.Skills.Count - 1) {
				SkillButtons [i].interactable = false;
				SkillButtons [i].GetComponent<BJSkillButton> ().CooldownSlider.maxValue = 0;
				SkillButtons [i].GetComponent<BJSkillButton> ().CooldownSlider.value = 0;
				SkillButtons [i].GetComponent<BJSkillButton> ().CooldownLabel.gameObject.SetActive (false);
			} else if (currentCreatureObject.Skills [i + 1].IsPassive) {				
				SkillButtons [i].interactable = false;
				SkillButtons [i].GetComponent<BJSkillButton> ().CooldownSlider.maxValue = 0;
				SkillButtons [i].GetComponent<BJSkillButton> ().ButtonImage.sprite = currentCreatureObject.Skills [i + 1].SkillIcon;
				SkillButtons [i].GetComponent<BJSkillButton> ().CooldownSlider.value = 0;
				SkillButtons [i].GetComponent<BJSkillButton> ().CooldownLabel.gameObject.SetActive (false);
			} else {
				SkillButtons [i].interactable = true;
				SkillButtons [i].GetComponent<BJSkillButton> ().ButtonImage.sprite = currentCreatureObject.Skills [i + 1].SkillIcon;
				SkillButtons [i].GetComponent<BJSkillButton> ().CooldownSlider.maxValue = currentCreatureObject.Skills [i + 1].Cooldown;
				SkillButtons [i].GetComponent<BJSkillButton> ().CooldownSlider.value = currentCreatureObject.Skills [i + 1].CurrentCooldown;
				if (currentCreatureObject.Skills [i + 1].CurrentCooldown > 0) {
					SkillButtons [i].GetComponent<BJSkillButton> ().CooldownLabel.gameObject.SetActive (true);
					SkillButtons [i].GetComponent<BJSkillButton> ().CooldownLabel.text = currentCreatureObject.Skills [i + 1].CurrentCooldown + "";
				} else {
					SkillButtons [i].GetComponent<BJSkillButton> ().CooldownLabel.gameObject.SetActive (false);
				}
			}
		}

		if (currentCreatureObject != null && currentCreatureObject.Creature.Allegiance == Allegiance.Enemy && PlayerCreatureObjects.Count > 0) {			
			int index = 0;
			int indexOfIndex = 0;
			do {
				indexOfIndex = Random.Range (0, currentCreatureObject.CurrentSkill.ValidTargetIndexes.Count);
				index = currentCreatureObject.CurrentSkill.ValidTargetIndexes[indexOfIndex];
			} while (PlayerCreatureObjects[index].Creature.HP <= 0);
			foreach (var playerCreatureObject in PlayerCreatureObjects) {
				foreach (var effect in playerCreatureObject.Effects) {
					if (playerCreatureObject.Creature.HP > 0 && effect is BJAggroEffect) {
						index = PlayerCreatureObjects.IndexOf (playerCreatureObject);
					}
				}
			}
			StartCoroutine(EnemyAttack(0.25f, PlayerCreatureObjects[index]));
		}
	}

	IEnumerator EnemyAttack (float delay, BJCreatureObject creatureObject) {
		yield return new WaitForSeconds (delay);
		if (currentCreatureObject.Creature.Allegiance == Allegiance.Player) {
			Debug.Log ("WHAT THE FUCK");
		}
		currentCreatureObject.Attack(creatureObject);
		currentCreatureObject.Deanimate ();
	}

	void CheckDead () {
		int deadCount = 0;
		foreach (var enemyCreatureObject in EnemyCreatureObjects) {
			if (enemyCreatureObject.Creature.HP <= 0) {
				deadCount++;
			}
		}
		if (deadCount == EnemyCreatureObjects.Count) {			
			Player.Instance.LoadVillage ();
		}
	}

	// public GameObject PlayerPortraitPrefab;

	public GameObject SelectionImageObject;

	public void CurrentCreatureChooseSkill (int index) {
		if (currentCreatureObject.Skills [index].CurrentCooldown > 0) {
			return;
		}
		if (currentCreatureObject.CurrentSkill == currentCreatureObject.Skills [index]) {
			currentCreatureObject.CurrentSkill = currentCreatureObject.Skills [0];
			SelectionImageObject.gameObject.SetActive (false);
		} else if (index != 0) { //currentCreatureObject.CurrentSkill != currentCreatureObject.Skills [0]) {
			currentCreatureObject.CurrentSkill = currentCreatureObject.Skills [index];
			SelectionImageObject.gameObject.SetActive (true);
			SelectionImageObject.transform.position = new Vector3 (SkillButtons [index - 1].transform.position.x, SkillButtons [index - 1].transform.position.y, SelectionImageObject.transform.position.z);
		}
		if (index == 0) {
			currentCreatureObject.CurrentSkill = currentCreatureObject.Skills [index];
			SelectionImageObject.gameObject.SetActive (false);
		}
		currentCreatureObject.CurrentSkill.AssignSkillIndexes ();
		if (currentCreatureObject.Creature.Allegiance == Allegiance.Player) {
			foreach (var enemyCreatureObject in EnemyCreatureObjects) {
				enemyCreatureObject.SelectionCircle.gameObject.SetActive (false);
			}
			foreach (var creatureObject in PlayerCreatureObjects) {
				creatureObject.SelectionCircle.gameObject.SetActive (false);
			}
			List<int> targetIndexes = new List<int> ();
			foreach (var enemyCreatureObject in EnemyCreatureObjects) {
				foreach (var effect in enemyCreatureObject.Effects) {
					if (effect is BJAggroEffect) {
						targetIndexes.Add (EnemyCreatureObjects.IndexOf(enemyCreatureObject));
					}
				}
			}

			if (targetIndexes.Count == 0) {
				targetIndexes = new List<int> (currentCreatureObject.CurrentSkill.ValidTargetIndexes);
			}
			foreach (var validTargetIndex in targetIndexes) {
				if (currentCreatureObject.CurrentSkill.TargetTeam == Teams.AnotherTeam) {
					if (validTargetIndex < EnemyCreatureObjects.Count && EnemyCreatureObjects[validTargetIndex].Creature.HP > 0) {
						EnemyCreatureObjects [validTargetIndex].SelectionCircle.gameObject.SetActive (true);
					}
				} else {
					if (validTargetIndex < PlayerCreatureObjects.Count && PlayerCreatureObjects[validTargetIndex].Creature.HP > 0) {
						PlayerCreatureObjects [validTargetIndex].SelectionCircle.gameObject.SetActive (true);
					}
				}
			}
		}
	}

	public void SpawnCreatures (int amount) {
		if (Player.Instance != null) { // kostyll
			foreach (var enemyShipData in Player.Instance.CurrentMission.EnemyShips) {
				SpawnCreatureObject (enemyShipData.Name, enemyShipData.MaxHP, enemyShipData.Power, 3, Random.Range(1, 5), Allegiance.Enemy, AttackType.Melee, new List<string>{ "Melee attack" });
			}
		} else {
			for (int i = 0; i < amount; i++) {
				AttackType attackType = (i < 3) ? AttackType.Melee : AttackType.Ranged;
				SpawnCreatureObject ("Cutthroat Bill", 400, 200, 3, Random.Range(1, 5), Allegiance.Enemy, attackType, new List<string>{ "Melee attack" });
			}
		}
	}

	void BjCreatureObject_OnCreatureTurnFinished (BJCreatureObject creatureObject) {
		foreach (var enemyCreatureObject in EnemyCreatureObjects) {
			enemyCreatureObject.SelectionCircle.gameObject.SetActive (false);
		}
		foreach (var playerCreatureObject in PlayerCreatureObjects) {
			playerCreatureObject.SelectionCircle.gameObject.SetActive (false);
		}
		if (currentCreatureObject == creatureObject) {
			Invoke ("StartTurn", 0.25f);
		}
	}

	void SpawnCreatureObject (string name, int hp, int attack, int armor, int speed, Allegiance allegiance, AttackType attackType, List<string> skillNames) {
		GameObject creatureObject = Instantiate (CreatureObjectPrefab) as GameObject;
		BJCreatureObject bjCreatureObject = creatureObject.GetComponent<BJCreatureObject> ();
		bjCreatureObject.Creature = new BJCreature (name, hp, attack, armor, speed, allegiance, attackType, skillNames);
		if (allegiance == Allegiance.Enemy) {
			int index = Random.Range (0, BJPlayer.Instance.DataBase.CharacterFigurines.Count);
			bjCreatureObject.CreatureImage.sprite = BJPlayer.Instance.DataBase.CharacterFigurines [index];
		} else {
			bjCreatureObject.CreatureImage.sprite = BJPlayer.Instance.DataBase.FigurinesByNames [name];
		}
		bjCreatureObject.HPFill.color = (allegiance == Allegiance.Player) ? Color.green : Color.red; 
		foreach (var skillName in skillNames) {
			bjCreatureObject.AddSkill (BJPlayer.Instance.DataBase.SkillsByNames [skillName]);
		}
		if (allegiance == Allegiance.Enemy) {			
			EnemyCreatureObjects.Add (bjCreatureObject);
		} else {
			PlayerCreatureObjects.Add (bjCreatureObject);
		}
		bjCreatureObject.OnCreatureObjectClicked += BjCreatureObject_OnCreatureObjectClicked;
		bjCreatureObject.OnCreatureReadyForTurn += BjCreatureObject_OnCreatureReadyForTurn;
		bjCreatureObject.OnCreatureTurnFinished += BjCreatureObject_OnCreatureTurnFinished;
	}

	public void SpawnPlayerCreatures () {
		foreach (var creature in BJPlayer.Instance.Creatures) {
			SpawnCreatureObject (creature.Name, creature.HP, creature.BaseDamage, creature.Armor, creature.Speed, creature.Allegiance, creature.AttackType, creature.SkillNames);
		}
	}

	void BjCreatureObject_OnCreatureObjectClicked (BJCreatureObject creatureObject) { // non-player can't click
		if (currentCreatureObject != null && currentCreatureObject.Creature.Allegiance == Allegiance.Player && currentCreatureObject.CurrentSkill.TargetTeam == Teams.AnotherTeam) {		
			if (!currentCreatureObject.CurrentSkill.ValidTargetIndexes.Contains(EnemyCreatureObjects.IndexOf(creatureObject))) {
				return;
			}	
			currentCreatureObject.Attack(creatureObject);
			currentCreatureObject.Deanimate ();

			int deadCount = 0; // not a good place for this script
			foreach (var enemyCreatureObject in EnemyCreatureObjects) {
				if (enemyCreatureObject.Creature.HP <= 0) {
					deadCount++;
				}
			}
			if (deadCount == EnemyCreatureObjects.Count) {			
				Player.Instance.LoadVillage ();
			}			
		}
		if (currentCreatureObject != null && currentCreatureObject.Creature.Allegiance == Allegiance.Player && currentCreatureObject.CurrentSkill.TargetTeam == Teams.MyTeam) {		
			if (!currentCreatureObject.CurrentSkill.ValidTargetIndexes.Contains(PlayerCreatureObjects.IndexOf(creatureObject))) {
				return;
			}	
			currentCreatureObject.Attack(creatureObject);
			currentCreatureObject.Deanimate ();
		}
	}

	IEnumerator Pause (float seconds) {
		yield return new WaitForSeconds (seconds);
	}
}
