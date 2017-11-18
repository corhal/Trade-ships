using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BJGameController : MonoBehaviour {

	public Allegiance CurrentTurn;

	public GameObject PlayerCreaturePrefab;
	public GameObject PlayerCreaturesContainer;
	public List<BJCreatureObject> PlayerCreatureObjects;

	public GameObject CreatureObjectsContainer;
	public GameObject CreatureObjectPrefab;
	public List<BJCreatureObject> EnemyCreatureObjects;

	public static BJGameController Instance;

	public List<Transform> PlayerSpawnPoints;
	public List<Transform> EnemySpawnPoints;

	// public Queue<BJCreatureObject> TurnQueue;

	public Slider ManaSlider;
	public Text ManaLabel;

	public List <BJSkill> PlayerSkills;
	public List <BJSkill> CurrentPlayerSkills;
	public List <BJSkill> EnemySkills;

	void Awake () {
		Instance = this;
		PlayerSkills = new List<BJSkill> ();
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

		ManaSlider.maxValue = BJPlayer.Instance.MaxMana;
		ManaSlider.value = BJPlayer.Instance.Mana;
		ManaLabel.text = BJPlayer.Instance.Mana + "";
		
		ApplyPassiveSkills ();
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

	public List<Button> SkillButtons;
	public BJCreatureObject CurrentCreatureObject;
	void StartTurn () {		
		CheckDead ();
		if (CurrentTurn == Allegiance.Player) {
			CurrentTurn = Allegiance.Enemy;
			BJSkill randomEnemySkill = null;
			EnemySkills.Shuffle ();
			foreach (var enemySkill in EnemySkills) {
				if (!(enemySkill.CurrentUser.IsDead || enemySkill.CurrentUser.IsStunned)) {
					randomEnemySkill = enemySkill;
				}
			}
			ChooseSkillAndCreature (randomEnemySkill);
		} else {
			CurrentTurn = Allegiance.Player;
			BJPlayer.Instance.Mana = Mathf.Min (BJPlayer.Instance.Mana + 1, BJPlayer.Instance.MaxMana);
			ManaSlider.value = BJPlayer.Instance.Mana;
			ManaLabel.text = BJPlayer.Instance.Mana + "";

			List<BJSkill> playerSkillsCopy = new List<BJSkill> (PlayerSkills);
			foreach (var skill in CurrentPlayerSkills) {
				playerSkillsCopy.Remove (skill);
			}
			for (int i = 0; i < CurrentPlayerSkills.Count; i++) {
				if (CurrentPlayerSkills [i] == null) {
					int randomIndex = Random.Range (0, playerSkillsCopy.Count);
					CurrentPlayerSkills [i] = playerSkillsCopy [randomIndex];
					playerSkillsCopy.RemoveAt (randomIndex);
				}
			}

			PrepareSkillButtons ();
		}	
	}

	void ChooseSkillAndCreature (BJSkill skill) {
	    CurrentCreatureObject = skill.CurrentUser;
		CurrentCreatureObject.CurrentSkill = skill;
		CurrentCreatureObject.CurrentSkill.AssignSkillIndexes ();
		CurrentCreatureObject.GetReadyForTurn (); // should either not be done here, or change GetReadyForTurn ()
	}

	public void SkipTurn () {
		// CurrentCreatureObject.FinishTurn (null);
		StartTurn ();
	}

	void PrepareSkillButtons () {
		for (int i = 0; i < SkillButtons.Count; i++) {			
			if (i >= CurrentPlayerSkills.Count) {
				SkillButtons [i].interactable = false;
			} else if (CurrentPlayerSkills [i].IsPassive) {		
				SkillButtons [i].GetComponent<BJSkillButton> ().ManacostLabel.text = CurrentPlayerSkills [i].ManaCost + "";
				SkillButtons [i].interactable = false;
				SkillButtons [i].GetComponent<BJSkillButton> ().ButtonImage.sprite = CurrentPlayerSkills [i].SkillIcon;
			} else {
				SkillButtons [i].GetComponent<BJSkillButton> ().ManacostLabel.text = CurrentPlayerSkills [i].ManaCost + "";
				SkillButtons [i].interactable = true;
				SkillButtons [i].GetComponent<BJSkillButton> ().ButtonImage.sprite = CurrentPlayerSkills [i].SkillIcon;
				if (CurrentPlayerSkills [i].ManaCost > BJPlayer.Instance.Mana) {
					SkillButtons [i].GetComponent<BJSkillButton> ().CooldownSlider.value = SkillButtons [i].GetComponent<BJSkillButton> ().CooldownSlider.maxValue;
				} else {
					SkillButtons [i].GetComponent<BJSkillButton> ().CooldownSlider.value = 0;
				}				
			}
		}
	}

	void BjCreatureObject_OnCreatureReadyForTurn (BJCreatureObject creatureObject) {
		CurrentCreatureObject.StartTurn ();		

		if (CurrentCreatureObject != null && CurrentCreatureObject.Creature.Allegiance == Allegiance.Enemy && PlayerCreatureObjects.Count > 0) {			
			int index = 0;
			int indexOfIndex = 0;
			do {
				indexOfIndex = Random.Range (0, CurrentCreatureObject.CurrentSkill.ValidTargetIndexes.Count);
				// Debug.Log(CurrentCreatureObject.CurrentSkill.ValidTargetIndexes.Count + "/" + indexOfIndex);
				index = CurrentCreatureObject.CurrentSkill.ValidTargetIndexes[indexOfIndex];
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
		if (CurrentCreatureObject.Creature.Allegiance == Allegiance.Player) {
			Debug.Log ("WHAT THE FUCK");
		}
		CurrentCreatureObject.Attack(creatureObject);
		CurrentCreatureObject.Deanimate ();
	}

	void CheckDead () {
		int deadCount = 0;
		foreach (var enemyCreatureObject in EnemyCreatureObjects) {
			if (enemyCreatureObject.Creature.HP <= 0) {
				deadCount++;
			}
		}
		if (deadCount == EnemyCreatureObjects.Count) {		
			if (Player.Instance != null) {
				Player.Instance.LoadVillage ();
			} else {
				SceneManager.LoadScene (0);
			}
		}
	}

	public GameObject SelectionImageObject;

	public void CurrentCreatureChooseSkill (int index) {
		
		if (CurrentPlayerSkills [index].ManaCost > BJPlayer.Instance.Mana) {
			return;
		} 

		if (CurrentCreatureObject != null) {
			CurrentCreatureObject.Deanimate ();
		}
		
		ChooseSkillAndCreature (CurrentPlayerSkills [index]);
			
		SelectionImageObject.gameObject.SetActive (true);
		SelectionImageObject.transform.position = new Vector3 (SkillButtons [index].transform.position.x, SkillButtons [index].transform.position.y, SelectionImageObject.transform.position.z);
		
		CurrentCreatureObject.CurrentSkill.AssignSkillIndexes ();
		if (CurrentCreatureObject.Creature.Allegiance == Allegiance.Player) {
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
				targetIndexes = new List<int> (CurrentCreatureObject.CurrentSkill.ValidTargetIndexes);
			}
			foreach (var validTargetIndex in targetIndexes) {
				if (CurrentCreatureObject.CurrentSkill.TargetTeam == Teams.AnotherTeam) {
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
			List<BJCreature> enemyCreatures = new List<BJCreature> (BJPlayer.Instance.DataBase.EnemyCreatures);
			for (int i = 0; i < amount; i++) {
				SpawnCreatureObject (enemyCreatures [i].Name, enemyCreatures [i].HP, enemyCreatures [i].BaseDamage, enemyCreatures [i].Armor, enemyCreatures [i].Speed,
					enemyCreatures [i].Allegiance, enemyCreatures [i].AttackType, new List<string> (enemyCreatures [i].SkillNames));
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
		if (CurrentCreatureObject == creatureObject) {
			Invoke ("StartTurn", 0.25f);
		}
	}

	void SpawnCreatureObject (string name, int hp, int attack, int armor, int speed, Allegiance allegiance, AttackType attackType, List<string> skillNames) {
		GameObject creatureObject = Instantiate (CreatureObjectPrefab) as GameObject;
		BJCreatureObject bjCreatureObject = creatureObject.GetComponent<BJCreatureObject> ();
		bjCreatureObject.Creature = new BJCreature (name, hp, attack, armor, speed, allegiance, attackType, skillNames);
		bjCreatureObject.CreatureImage.sprite = BJPlayer.Instance.DataBase.FigurinesByNames [name];
		bjCreatureObject.CreatureImage.SetNativeSize ();
		bjCreatureObject.CreatureImage.rectTransform.sizeDelta = new Vector2 (bjCreatureObject.CreatureImage.rectTransform.rect.width / 7, 
			bjCreatureObject.CreatureImage.rectTransform.rect.height / 7);
		bjCreatureObject.HPFill.color = (allegiance == Allegiance.Player) ? Color.green : Color.red; 
		foreach (var skillName in skillNames) {
			bjCreatureObject.AddSkill (BJPlayer.Instance.DataBase.SkillsByNames [skillName]);
		}
		if (allegiance == Allegiance.Enemy) {			
			foreach (var skill in bjCreatureObject.Skills) {
				EnemySkills.Add (skill);
			}
			EnemyCreatureObjects.Add (bjCreatureObject);
		} else {
			foreach (var skill in bjCreatureObject.Skills) {
				if (!skill.IsPassive) {
					PlayerSkills.Add (skill);
				}				
			}
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
		List<BJSkill> playerSkillsCopy = new List<BJSkill> (PlayerSkills);
		for (int j = 0; j < 4; j++) {
			int randomIndex = Random.Range (0, playerSkillsCopy.Count);
			CurrentPlayerSkills.Add (playerSkillsCopy [randomIndex]);
			playerSkillsCopy.RemoveAt (randomIndex);
		}			
	}

	void BjCreatureObject_OnCreatureObjectClicked (BJCreatureObject creatureObject) { // non-player can't click
		if (CurrentCreatureObject != null && CurrentCreatureObject.Creature.Allegiance == Allegiance.Player && CurrentCreatureObject.CurrentSkill.TargetTeam == Teams.AnotherTeam) {		
			if (!CurrentCreatureObject.CurrentSkill.ValidTargetIndexes.Contains(EnemyCreatureObjects.IndexOf(creatureObject))) {
				return;
			}	

			CurrentCreatureObject.Attack(creatureObject);
			CurrentCreatureObject.Deanimate ();

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
		if (CurrentCreatureObject != null && CurrentCreatureObject.Creature.Allegiance == Allegiance.Player && CurrentCreatureObject.CurrentSkill.TargetTeam == Teams.MyTeam) {		
			if (!CurrentCreatureObject.CurrentSkill.ValidTargetIndexes.Contains(PlayerCreatureObjects.IndexOf(creatureObject))) {
				return;
			}	
			CurrentCreatureObject.Attack(creatureObject);
			CurrentCreatureObject.Deanimate ();
		}
		if (CurrentCreatureObject.Creature.Allegiance == Allegiance.Player) {
			BJPlayer.Instance.Mana -= CurrentCreatureObject.CurrentSkill.ManaCost;
			ManaSlider.value = BJPlayer.Instance.Mana;
			ManaLabel.text = BJPlayer.Instance.Mana + "";
			int index = CurrentPlayerSkills.IndexOf (CurrentCreatureObject.CurrentSkill);
			CurrentPlayerSkills [index] = null;
		}
	}

	IEnumerator Pause (float seconds) {
		yield return new WaitForSeconds (seconds);
	}
}
