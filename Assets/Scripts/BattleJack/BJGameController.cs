using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BJGameController : MonoBehaviour {

	public GameObject BattleHud;

	public GameObject PlayerCreaturePrefab;
	public GameObject PlayerCreaturesContainer;
	public List<BJCreatureObject> PlayerCreatureObjects;

	public GameObject EnemyCreaturesContainer;
	public GameObject CreatureObjectPrefab;
	public List<BJCreatureObject> EnemyCreatureObjects;

	public static BJGameController Instance;

	public List<Transform> PlayerSpawnPoints;
	public List<Transform> EnemySpawnPoints;

	public Queue<BJCreatureObject> TurnQueue;

	public GameObject PlayerShipObject;
	public GameObject EnemyShipObject;
	public GameObject EnemyCastleObject;

	bool isPreparingForBattle;

	public Transform PlayerShipTargetPosition;
	public Transform EnemyShipTargetPosition;

	public Transform PlayerCreaturesTargetPosition;
	public Transform EnemyCreaturesTargetPosition;

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

		List<BJCreatureObject> AllCreatureObjects = new List<BJCreatureObject> (EnemyCreatureObjects);
		AllCreatureObjects.AddRange (PlayerCreatureObjects);

		ApplyPassiveSkills ();
		FormQueue ();
		isPreparingForBattle = true;
		if (Player.Instance.CurrentMission.IsCastle) {
			EnemyShipObject.SetActive (false);
			EnemyCastleObject.SetActive (true);
		}
	}

	void Update () {
		if (isPreparingForBattle) {
			if (Vector2.Distance(PlayerShipObject.transform.position, PlayerShipTargetPosition.position) < 0.01f &&
				Vector2.Distance(PlayerCreaturesContainer.transform.position, PlayerCreaturesTargetPosition.position) < 0.01f &&
				Vector2.Distance(EnemyCreaturesContainer.transform.position, EnemyCreaturesTargetPosition.position) < 0.01f) {
				if (Player.Instance.CurrentMission.IsCastle || Vector2.Distance(EnemyShipObject.transform.position, EnemyShipTargetPosition.position) < 0.01f) {
					isPreparingForBattle = false;
					foreach (var playerCreatureObject in PlayerCreatureObjects) {
						playerCreatureObject.InitialPosition = playerCreatureObject.transform.position;
					}
					foreach (var enemyCreatureObject in EnemyCreatureObjects) {
						enemyCreatureObject.InitialPosition = enemyCreatureObject.transform.position;
					}
				}
				Invoke ("StartTurn", 0.25f);
			} else {
				if (Player.Instance.CurrentMission.IsCastle) {
					EnemyCreaturesContainer.transform.position = EnemyCreaturesTargetPosition.position;
				}
				PlayerShipObject.GetComponent<BJMover> ().MoveToPoint (PlayerShipTargetPosition.position);
				EnemyShipObject.GetComponent<BJMover> ().MoveToPoint (EnemyShipTargetPosition.position);
				PlayerCreaturesContainer.GetComponent<BJMover> ().MoveToPoint (PlayerCreaturesTargetPosition.position);
				EnemyCreaturesContainer.GetComponent<BJMover> ().MoveToPoint (EnemyCreaturesTargetPosition.position);
			}
		}
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
						Debug.Log (creatureObject.Name + " use " + skill.Name);
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
	public BJCreatureObject CurrentCreatureObject;
	void StartTurn () {
		CheckDead ();
		if (TurnQueue.Count == 0) {
			FormQueue ();
		}
		CurrentCreatureObject = TurnQueue.Dequeue ();
		if (CurrentCreatureObject != null && CurrentCreatureObject.Creature.HP <= 0) {
			StartTurn ();
			return;
		}
		CurrentCreatureChooseSkill (0);

		CurrentCreatureObject.GetReadyForTurn ();
	}

	void BjCreatureObject_OnCreatureReadyForTurn (BJCreatureObject creatureObject) {
		CurrentCreatureObject.StartTurn ();

		for (int i = 0; i < SkillButtons.Count; i++) {
			BJSkillButton skillButton = SkillButtons [i].GetComponent<BJSkillButton> ();
			skillButton.ReadyParticlesObject.SetActive (false);
			if (i >= CurrentCreatureObject.Skills.Count - 1) {				
				SkillButtons [i].interactable = false;
				skillButton.CooldownSlider.maxValue = 0;
				skillButton.CooldownSlider.value = 0;
				skillButton.CooldownLabel.gameObject.SetActive (false);
			} else if (CurrentCreatureObject.Skills [i + 1].IsPassive) {				
				SkillButtons [i].interactable = false;
				skillButton.CooldownSlider.maxValue = 0;
				skillButton.ButtonImage.sprite = CurrentCreatureObject.Skills [i + 1].SkillIcon;
				skillButton.CooldownSlider.value = 0;
				skillButton.CooldownLabel.gameObject.SetActive (false);
			} else {
				SkillButtons [i].interactable = true;
				skillButton.ButtonImage.sprite = CurrentCreatureObject.Skills [i + 1].SkillIcon;
				skillButton.CooldownSlider.maxValue = CurrentCreatureObject.Skills [i + 1].Cooldown;
				skillButton.CooldownSlider.value = CurrentCreatureObject.Skills [i + 1].CurrentCooldown;
				if (CurrentCreatureObject.Skills [i + 1].CurrentCooldown > 0) {					
					skillButton.CooldownLabel.gameObject.SetActive (true);
					skillButton.CooldownLabel.text = CurrentCreatureObject.Skills [i + 1].CurrentCooldown + "";
				} else {
					skillButton.ReadyParticlesObject.SetActive (true);
					skillButton.CooldownLabel.gameObject.SetActive (false);
				}
			}				
		}

		if (CurrentCreatureObject != null && CurrentCreatureObject.Creature.Allegiance == Allegiance.Enemy && PlayerCreatureObjects.Count > 0) {	
			CheckDead ();
			int index = 0;
			int indexOfIndex = 0;
			do {
				indexOfIndex = Random.Range (0, CurrentCreatureObject.CurrentSkill.ValidTargetIndexes.Count);
				index = CurrentCreatureObject.CurrentSkill.ValidTargetIndexes[indexOfIndex];
			} while (PlayerCreatureObjects.Count <= index || PlayerCreatureObjects[index].Creature.HP <= 0);
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
		int playerDeadCount = 0;
		foreach (var enemyCreatureObject in EnemyCreatureObjects) {
			if (enemyCreatureObject.Creature.HP <= 0) {
				deadCount++;
			}
		}
		foreach (var creatureObject in PlayerCreatureObjects) {
			if (creatureObject.Creature.HP <= 0) {
				playerDeadCount++;
			}
		}
		if (deadCount == EnemyCreatureObjects.Count) {		
			if (Player.Instance != null) {
				Player.Instance.CurrentMission.Stars = 3;
				SaveHP ();
				Player.Instance.LoadAdventure ();
			} else {
				SceneManager.LoadScene (1);
			}
		}
		if (playerDeadCount == PlayerCreatureObjects.Count) {
			if (Player.Instance != null) {
				Player.Instance.CurrentMission.Stars = 0;
				SaveHP ();
				Player.Instance.LoadAdventure ();
			} else {
				SceneManager.LoadScene (1);
			}
		}
	}

	void SaveHP () {
		/*foreach (var creatureObject in PlayerCreatureObjects) {
			foreach (var creatureData in Player.Instance.CurrentTeam) {
				if (creatureData.Name == creatureObject.Creature.Name) {
					creatureData.HP = creatureObject.Creature.HP;
					creatureData.IsDead = creatureObject.Creature.IsDead;
				}
			}
		}*/
	}


	public GameObject SelectionImageObject;

	public void CurrentCreatureChooseSkill (int index) {
		if (CurrentCreatureObject.Skills [index].CurrentCooldown > 0) {
			return;
		}
		if (CurrentCreatureObject.CurrentSkill == CurrentCreatureObject.Skills [index]) {
			CurrentCreatureObject.CurrentSkill = CurrentCreatureObject.Skills [0];
			SelectionImageObject.gameObject.SetActive (false);
		} else if (index != 0) {
			CurrentCreatureObject.CurrentSkill = CurrentCreatureObject.Skills [index];
			SelectionImageObject.gameObject.SetActive (true);
			SelectionImageObject.transform.position = new Vector3 (SkillButtons [index - 1].transform.position.x, SkillButtons [index - 1].transform.position.y, SelectionImageObject.transform.position.z);
		}
		if (index == 0) {
			CurrentCreatureObject.CurrentSkill = CurrentCreatureObject.Skills [index];
			SelectionImageObject.gameObject.SetActive (false);
		}
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
			List<CreatureData> enemyShipDatas = Player.Instance.CurrentMission.EnemyShips;
			for (int i = 0; i < enemyShipDatas.Count; i++) {
				BJCreature enemyCreature = enemyShipDatas [i].Creature;
				BJCreatureObject bjCreatureObject = SpawnCreatureObject (enemyCreature, enemyShipDatas [i].Level
					                                    /*enemyCreature.Name, 
					                                    enemyCreature.MaxHP,
					                                    enemyCreature.HP,
					                                    enemyCreature.BaseDamage, 
					                                    enemyCreature.Armor, 
					                                    enemyCreature.Speed, 
					                                    enemyCreature.Allegiance, 
					                                    //enemyCreature.AttackType, 
					                                    enemyCreature.SkillNames*/);
				bjCreatureObject.gameObject.transform.SetParent (EnemySpawnPoints [i]);
				bjCreatureObject.gameObject.transform.localScale = Vector3.one;
				bjCreatureObject.gameObject.transform.localPosition = Vector3.zero;
			}
		} else {
			List<BJCreature> enemyCreatures = new List<BJCreature> (BJPlayer.Instance.DataBase.EnemyCreatures);
			for (int i = 0; i < amount; i++) {
				BJCreatureObject bjCreatureObject = SpawnCreatureObject (enemyCreatures [i], 1/*enemyCreatures [i].Name, enemyCreatures [i].MaxHP, enemyCreatures [i].HP, enemyCreatures [i].BaseDamage, enemyCreatures [i].Armor, enemyCreatures [i].Speed,
					enemyCreatures [i].Allegiance, /*enemyCreatures [i].AttackType,*/ /*new List<string> (enemyCreatures [i].SkillNames)*/);				
				bjCreatureObject.gameObject.transform.SetParent (EnemySpawnPoints [i]);
				bjCreatureObject.gameObject.transform.localScale = Vector3.one;
				bjCreatureObject.gameObject.transform.localPosition = Vector3.zero;
			}
		}
	}

	public void SpawnPlayerCreatures () {
		List<CreatureData> creatureDatas = Player.Instance.CurrentTeam;
		//List<BJCreature> creatures = BJPlayer.Instance.Creatures;
		for (int i = 0; i < creatureDatas.Count; i++) {
			BJCreatureObject bjCreatureObject = SpawnCreatureObject (creatureDatas [i].Creature, creatureDatas [i].Level
				                                    /*creatures [i].Name, 
				                                    creatures [i].MaxHP,
				                                    creatures [i].HP,
				                                    creatures [i].BaseDamage, 
				                                    creatures [i].Armor, 
				                                    creatures [i].Speed, 
				                                    creatures [i].Allegiance, 
				                                    //creatures [i].AttackType, 
				                                    creatures [i].SkillNames*/
			                                        );
			// Debug.Log (bjCreatureObject.Name + " " + bjCreatureObject.Creature.HP + "/" + bjCreatureObject.Creature.MaxHP);
			bjCreatureObject.gameObject.transform.SetParent (PlayerSpawnPoints [i]);
			bjCreatureObject.gameObject.transform.localScale = Vector3.one;
			bjCreatureObject.gameObject.transform.localPosition = Vector3.zero;
		}
	}

	void BjCreatureObject_OnCreatureTurnFinished (BJCreatureObject creatureObject) {
		CheckDead ();
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

					BJCreatureObject SpawnCreatureObject (BJCreature creature, int level/*string name, List<int> maxHpByLevel, int hp, int attack, int armor, int speed, Allegiance allegiance, /*AttackType attackType,*/ /*List<string> skillNames*/) {
		GameObject creatureObject = Instantiate (CreatureObjectPrefab) as GameObject;
		BJCreatureObject bjCreatureObject = creatureObject.GetComponent<BJCreatureObject> ();
		bjCreatureObject.Creature = new BJCreature (creature.Name, creature.MaxHPByLevel, creature.HP, creature.BaseDamageByLevel, creature.Armor, creature.Speed, creature.Allegiance, /*attackType,*/creature.SkillNames);
		bjCreatureObject.Creature.Level = level;
		bjCreatureObject.Creature.HP = bjCreatureObject.Creature.MaxHP;
		bjCreatureObject.Creature.IsDead = false;

		bjCreatureObject.CreatureImage.sprite = BJPlayer.Instance.DataBase.FigurinesByNames [creature.Name];
		bjCreatureObject.CreatureImage.SetNativeSize ();
		bjCreatureObject.CreatureImage.rectTransform.sizeDelta = new Vector2 (bjCreatureObject.CreatureImage.rectTransform.rect.width / 7, 
			bjCreatureObject.CreatureImage.rectTransform.rect.height / 7);
		bjCreatureObject.HPFill.color = (creature.Allegiance == Allegiance.Player) ? Color.green : Color.red; 
		foreach (var skillName in creature.SkillNames) {
			BJPlayer.Instance.DataBase.BJSkillsByNames [skillName].Name = skillName;
			bjCreatureObject.AddSkill (BJPlayer.Instance.DataBase.BJSkillsByNames [skillName]);
		}
		if (creature.Allegiance == Allegiance.Enemy) {			
			EnemyCreatureObjects.Add (bjCreatureObject);
		} else {
			PlayerCreatureObjects.Add (bjCreatureObject);
		}
		bjCreatureObject.OnCreatureObjectClicked += BjCreatureObject_OnCreatureObjectClicked;
		bjCreatureObject.OnCreatureReadyForTurn += BjCreatureObject_OnCreatureReadyForTurn;
		bjCreatureObject.OnCreatureTurnFinished += BjCreatureObject_OnCreatureTurnFinished;
		return bjCreatureObject;
	}

	void BjCreatureObject_OnCreatureObjectClicked (BJCreatureObject creatureObject) { // non-player can't click
		List<BJCreatureObject> targetCreatureObjects = (CurrentCreatureObject.CurrentSkill.TargetTeam == Teams.AnotherTeam) ? BJGameController.Instance.EnemyCreatureObjects : BJGameController.Instance.PlayerCreatureObjects;
		if (CurrentCreatureObject != null && CurrentCreatureObject.Creature.Allegiance == Allegiance.Player) { // && CurrentCreatureObject.CurrentSkill.TargetTeam == Teams.AnotherTeam) {		
			if (!CurrentCreatureObject.CurrentSkill.ValidTargetIndexes.Contains(targetCreatureObjects.IndexOf(creatureObject))) {
				return;
			}	
			CurrentCreatureObject.Attack(creatureObject);
			CurrentCreatureObject.Deanimate ();
		}
	}
}
