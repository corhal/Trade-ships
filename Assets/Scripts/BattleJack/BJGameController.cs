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

		List<string> CreatureNames = new List<string> {
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
		}
		FormQueue ();
		Invoke ("StartTurn", 0.25f);
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
			x.Creature.Speed.CompareTo(y.Creature.Speed));

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
			} else {
				SkillButtons [i].interactable = true;
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
			StartCoroutine(EnemyAttack(0.25f, PlayerCreatureObjects[index]));
		}
	}

	IEnumerator EnemyAttack (float delay, BJCreatureObject creatureObject) {
		yield return new WaitForSeconds (delay);
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
		} else {
			currentCreatureObject.CurrentSkill = currentCreatureObject.Skills [index];
			SelectionImageObject.gameObject.SetActive (true);
		}
		if (currentCreatureObject.CurrentSkill == currentCreatureObject.Skills [0]) {
			SelectionImageObject.gameObject.SetActive (false);
		}
		currentCreatureObject.CurrentSkill.AssignSkillIndexes ();
		if (currentCreatureObject.Creature.Allegiance == Allegiance.Player) {
			foreach (var enemyCreatureObject in EnemyCreatureObjects) {
				enemyCreatureObject.SelectionCircle.gameObject.SetActive (false);
			}
			foreach (var validTargetIndex in currentCreatureObject.CurrentSkill.ValidTargetIndexes) {
				if (validTargetIndex < EnemyCreatureObjects.Count && EnemyCreatureObjects[validTargetIndex].Creature.HP > 0) {
					EnemyCreatureObjects [validTargetIndex].SelectionCircle.gameObject.SetActive (true);
				}
			}
		}
	}

	public void SpawnCreatures (int amount) {
		if (Player.Instance != null) { // kostyll
			foreach (var enemyShipData in Player.Instance.CurrentMission.EnemyShips) {
				SpawnCreatureObject (enemyShipData.Name, enemyShipData.MaxHP, enemyShipData.Power, Allegiance.Enemy, AttackType.Melee);
			}
		} else {
			for (int i = 0; i < amount; i++) {
				AttackType attackType = (i < 3) ? AttackType.Melee : AttackType.Ranged;
				SpawnCreatureObject ("Cutthroat Bill", 400, 70, Allegiance.Enemy, attackType);
			}
		}
	}

	void BjCreatureObject_OnCreatureTurnFinished (BJCreatureObject creatureObject) {
		foreach (var enemyCreatureObject in EnemyCreatureObjects) {
			enemyCreatureObject.SelectionCircle.gameObject.SetActive (false);
		}
		/*if (creatureObject.Creature.HP <= 0) {
			StopAllCoroutines ();
			// StopCoroutine ("EnemyAttack");
		}*/
		if (currentCreatureObject == creatureObject) {
			Invoke ("StartTurn", 0.25f);
		}
	}

	void SpawnCreatureObject (string name, int hp, int attack, Allegiance allegiance, AttackType attackType) {
		GameObject creatureObject = Instantiate (CreatureObjectPrefab) as GameObject;
		BJCreatureObject bjCreatureObject = creatureObject.GetComponent<BJCreatureObject> ();
		bjCreatureObject.Creature = new BJCreature (name, hp, attack, Random.Range(1, 7), allegiance, attackType);
		if (allegiance == Allegiance.Enemy) {
			int index = Random.Range (0, BJPlayer.Instance.DataBase.CharacterFigurines.Count);
			bjCreatureObject.CreatureImage.sprite = BJPlayer.Instance.DataBase.CharacterFigurines [index];
		} else {
			bjCreatureObject.CreatureImage.sprite = BJPlayer.Instance.DataBase.FigurinesByNames [name];
		}
		bjCreatureObject.HPFill.color = (allegiance == Allegiance.Player) ? Color.green : Color.red; 
		BJSkill skill = (attackType == AttackType.Melee) ? BJPlayer.Instance.DataBase.Skills [0] : BJPlayer.Instance.DataBase.Skills [1];
		bjCreatureObject.AddSkill(skill);
		if (bjCreatureObject.Creature.AttackType == AttackType.Melee) {
			int skillIndex = Random.Range (2, BJPlayer.Instance.DataBase.Skills.Count);
			bjCreatureObject.AddSkill (BJPlayer.Instance.DataBase.Skills[3]); // for debugging purposes
		}
		if (allegiance == Allegiance.Enemy) {
			bjCreatureObject.OnCreatureObjectClicked += BjCreatureObject_OnCreatureObjectClicked;
			EnemyCreatureObjects.Add (bjCreatureObject);
		} else {
			PlayerCreatureObjects.Add (bjCreatureObject);
		}
		bjCreatureObject.OnCreatureReadyForTurn += BjCreatureObject_OnCreatureReadyForTurn;
		bjCreatureObject.OnCreatureTurnFinished += BjCreatureObject_OnCreatureTurnFinished;
	}

	public void SpawnPlayerCreatures () {
		foreach (var creature in BJPlayer.Instance.Creatures) {
			SpawnCreatureObject (creature.Name, creature.HP, creature.BaseDamage, creature.Allegiance, creature.AttackType);
		}
	}

	void BjCreatureObject_OnCreatureObjectClicked (BJCreatureObject creatureObject) { // non-player can't click
		if (currentCreatureObject != null && currentCreatureObject.Creature.Allegiance == Allegiance.Player) {		
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
	}

	IEnumerator Pause (float seconds) {
		yield return new WaitForSeconds (seconds);
	}
}
