using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJGameController : MonoBehaviour {

	public Slider PlayerHPSlider;
	public Text PlayerHPLabel;
	public GameObject PlayerCreaturePrefab;
	public GameObject PlayerCreaturesContainer;
	public List<BJCreatureObject> PlayerCreatureObjects;

	public Slider EnemyShipHPSlider;
	public Text EnemyShipHPLabel;
	public GameObject CreatureObjectsContainer;
	public GameObject CreatureObjectPrefab;
	public List<BJCreatureObject> EnemyCreatureObjects;

	public BJDeck Deck;
	public BJDeck Hand;

	public GameObject CardObjectsContainer;
	public GameObject CardObjectPrefab;
	public List<BJCardObject> CardObjects;

	int currentScore;
	public Text ScoreLabel;
	public Slider ScoreSlider;

	public static BJGameController Instance;

	public delegate void DealCardsEventHandler (float multiplier);

	public event DealCardsEventHandler OnCardsDealt;

	public int EnemyShipMaxHP = 0;
	public int EnemyShipHP = 0;

	public List<Transform> PlayerSpawnPoints;
	public List<Transform> EnemySpawnPoints;

	public Queue<BJCreatureObject> TurnQueue;

	void Awake () {
		Instance = this;
	}

	void Start () {
		Deck = new BJDeck ();
		Deck.Shuffle ();
		Hand = new BJDeck ();
		Hand.Flush ();

		StartCoroutine (DealCards (2, 0.0f));

		SpawnCreatures (3);

		for (int i = 0; i < EnemyCreatureObjects.Count; i++) {
			EnemyCreatureObjects [i].gameObject.transform.SetParent (EnemySpawnPoints [i]);
			EnemyCreatureObjects [i].gameObject.transform.localScale = Vector3.one;
			EnemyCreatureObjects [i].gameObject.transform.localPosition = Vector3.zero;
		}

		BJPlayer.Instance.OnDamageTaken += BJPlayer_Instance_OnDamageTaken;

		PlayerHPSlider.maxValue = BJPlayer.Instance.MaxHP;
		PlayerHPSlider.value = BJPlayer.Instance.HP;
		PlayerHPLabel.text = BJPlayer.Instance.HP + "/" + BJPlayer.Instance.MaxHP;

		EnemyShipHPSlider.maxValue = EnemyShipMaxHP;
		EnemyShipHPSlider.value = EnemyShipHP;
		EnemyShipHPLabel.text = EnemyShipHP + "/" + EnemyShipMaxHP;

		SpawnPlayerCreatures ();
		for (int i = 0; i < PlayerCreatureObjects.Count; i++) {
			PlayerCreatureObjects [i].gameObject.transform.SetParent (PlayerSpawnPoints [i]);
			PlayerCreatureObjects [i].gameObject.transform.localScale = Vector3.one;
			PlayerCreatureObjects [i].gameObject.transform.localPosition = Vector3.zero;
		}
		ScoreSlider.maxValue = 21;

		FormQueue ();
		StartTurn ();
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

	BJCreatureObject currentCreatureObject;
	void StartTurn () {
		if (TurnQueue.Count == 0) {
			FormQueue ();
		}
		currentCreatureObject = TurnQueue.Dequeue ();
		if (currentCreatureObject != null && currentCreatureObject.Creature.HP <= 0) {
			StartTurn ();
		}
		currentCreatureObject.Animate ();

		if (currentCreatureObject != null && currentCreatureObject.Creature.Allegiance == Allegiance.Enemy && PlayerCreatureObjects.Count > 0) {
			int index = 0;
			do {
				index = Random.Range (0, PlayerCreatureObjects.Count);
			} while (PlayerCreatureObjects[index].Creature.HP <= 0);
			currentCreatureObject.DealDamage (2.0f, PlayerCreatureObjects [index]);
			currentCreatureObject.Deanimate ();
			Invoke ("StartTurn", 0.5f);
		}
	}

	void BJPlayer_Instance_OnDamageTaken ()	{
		PlayerHPSlider.value = BJPlayer.Instance.HP;
		PlayerHPLabel.text = BJPlayer.Instance.HP + "/" + BJPlayer.Instance.MaxHP;
		if (BJPlayer.Instance.HP <= 0 && PlayerHPSlider.gameObject.activeSelf && PlayerHPLabel.gameObject.activeSelf) {
			PlayerHPSlider.gameObject.SetActive (false);
			PlayerHPLabel.gameObject.SetActive (false);
		}
	}

	public void TakeDamage (int amount) {
		EnemyShipHP = Mathf.Max (0, EnemyShipHP - amount);
		EnemyShipHPSlider.value = EnemyShipHP;
		EnemyShipHPLabel.text = EnemyShipHP + "/" + EnemyShipMaxHP;
		if (EnemyShipHP <= 0 && EnemyShipHPSlider.gameObject.activeSelf && EnemyShipHPLabel.gameObject.activeSelf) {
			EnemyShipHPSlider.gameObject.SetActive (false);
			EnemyShipHPLabel.gameObject.SetActive (false);
		}
	}

	public void PlayerAttack () {
		if (EnemyShipHP > 0) {
			float multiplier = 1.0f + currentScore / 21.0f; 
			foreach (var creatureObject in PlayerCreatureObjects) {
				if (creatureObject.Creature.HP > 0) {
					creatureObject.DealDamage (multiplier, this);
				}
			}
		} else {
			BJCreatureObject currentTarget = null;
			foreach (var enemyCreature in EnemyCreatureObjects) {
				if (enemyCreature.Creature.HP > 0) {
					currentTarget = enemyCreature;
					break;
				}
			}

			float multiplier = 1.0f + currentScore / 21.0f; 
			foreach (var creatureObject in PlayerCreatureObjects) {
				if (creatureObject.Creature.HP > 0) {
					creatureObject.DealDamage (multiplier, currentTarget);
				}
			}
		}

		FlushCards ();

		int deadCount = 0;
		foreach (var enemyCreatureObject in EnemyCreatureObjects) {
			if (enemyCreatureObject.Creature.HP <= 0) {
				deadCount++;
			}
		}
		if (deadCount == EnemyCreatureObjects.Count) {			
			BJPlayer.Instance.OnDamageTaken -= BJPlayer_Instance_OnDamageTaken;
			Player.Instance.LoadVillage ();
		} else {
			Invoke ("EnemyAttack", 0.5f);
		}
	}

	public void EnemyAttack () {
		if (BJPlayer.Instance.HP > 0) {
			float multiplier = 1.0f;
			foreach (var enemyCreatureObject in EnemyCreatureObjects) {
				if (enemyCreatureObject.Creature.HP > 0) {
					enemyCreatureObject.DealDamage (multiplier, BJPlayer.Instance);
				}
			}
		} else {
			BJCreatureObject currentTarget = null;
			foreach (var playerCreature in PlayerCreatureObjects) {
				if (playerCreature.Creature.HP > 0) {
					currentTarget = playerCreature;
					break;
				}
			}

			float multiplier = 1.0f + currentScore / 21.0f; 
			foreach (var enemyCreatureObject in EnemyCreatureObjects) {
				if (enemyCreatureObject.Creature.HP > 0) {
					enemyCreatureObject.DealDamage (multiplier, currentTarget);
				}
			}
		}

		StartCoroutine (DealCards (2, 0.5f));
	}

	public void ClickDealButton () {
		StartCoroutine (DealCards (1, 0.0f));
	}

	public IEnumerator DealCards (int count, float pause) {
		yield return new WaitForSeconds (pause);
		if (Deck.Cards.Count == 0) {
			Deck = new BJDeck ();
			Deck.Shuffle ();
		}
		for (int i = 0; i < count; i++) {
			GameObject cardObject = Instantiate (CardObjectPrefab) as GameObject;
			BJCardObject bjCardObject = cardObject.GetComponent<BJCardObject> ();
			bjCardObject.Card = Deck.Deal ();
			bjCardObject.RankSuitLabel.text = bjCardObject.Card.Rank + "\n" + "of\n" + bjCardObject.Card.Suit;
			cardObject.transform.SetParent (CardObjectsContainer.transform);
			cardObject.transform.localScale = Vector3.one;
			CardObjects.Add (bjCardObject);
			Hand.Add (bjCardObject.Card);
		}
		currentScore = Hand.CountScore ();
		ScoreLabel.text = "" + currentScore;
		ScoreSlider.value = currentScore;

		if (currentScore > 21) {
			Invoke ("FlushCards", 0.5f); // maybe should use coroutines instead
			Invoke ("EnemyAttack", 0.5f);
		} else {
			float multiplier = 1.0f + currentScore / 21.0f; 
			OnCardsDealt (multiplier);
		}
	}

	public void FlushCards () {
		Hand.Flush ();
		foreach (var cardObject in CardObjects) {
			Destroy (cardObject.gameObject);
		}
		CardObjects.Clear ();
		currentScore = 0;
		ScoreLabel.text = "" + currentScore;
	}

	public GameObject PlayerPortraitPrefab;

	public void SpawnPlayerCreatures () {
		foreach (var creature in BJPlayer.Instance.Creatures) {
			GameObject creatureObject = Instantiate (PlayerCreaturePrefab) as GameObject;
			BJCreatureObject bjCreatureObject = creatureObject.GetComponent<BJCreatureObject> (); 
			int index = Random.Range (0, BJPlayer.Instance.DataBase.CharacterFigurines.Count);
			creatureObject.GetComponent<Image> ().sprite = BJPlayer.Instance.DataBase.CharacterFigurines [index];
			bjCreatureObject.Creature = creature;
			bjCreatureObject.HPFill.color = Color.green;
			OnCardsDealt += bjCreatureObject.BJGameController_Instance_OnCardsDealt;

			GameObject portraitObject = Instantiate (PlayerPortraitPrefab) as GameObject;
			portraitObject.GetComponent<BJPlayerShipObject> ().PortraitImage.sprite = BJPlayer.Instance.DataBase.CharacterPortraits [index];
			portraitObject.transform.SetParent (PlayerCreaturesContainer.transform);
			portraitObject.transform.localScale = Vector3.one;

			/*creatureObject.transform.SetParent (PlayerCreaturesContainer.transform);
			creatureObject.transform.localScale = Vector3.one;*/
			PlayerCreatureObjects.Add (bjCreatureObject);
		}
	}

	public void SpawnCreatures (int amount) {
		if (Player.Instance != null) { // kostyll
			foreach (var enemyShipData in Player.Instance.CurrentMission.EnemyShips) {
				SpawnCreature (enemyShipData.MaxHP, enemyShipData.Power);
			}
		} else {
			for (int i = 0; i < amount; i++) {
				SpawnCreature (400, 10);
			}
		}
	}

	void SpawnCreature (int hp, int baseDamage) {
		GameObject creatureObject = Instantiate (CreatureObjectPrefab) as GameObject;
		BJCreatureObject bjCreatureObject = creatureObject.GetComponent<BJCreatureObject> ();
		int index = Random.Range (0, BJPlayer.Instance.DataBase.CharacterFigurines.Count);
		bjCreatureObject.GetComponent<Image> ().sprite = BJPlayer.Instance.DataBase.CharacterFigurines [index];
		bjCreatureObject.Creature = new BJCreature (hp, baseDamage, Random.Range(1, 7), Allegiance.Enemy);
		/*creatureObject.transform.SetParent (CreatureObjectsContainer.transform);
		creatureObject.transform.localScale = Vector3.one;*/
		EnemyCreatureObjects.Add (bjCreatureObject);
		bjCreatureObject.OnCreatureObjectClicked += BjCreatureObject_OnCreatureObjectClicked;
	}

	void BjCreatureObject_OnCreatureObjectClicked (BJCreatureObject creatureObject) { // non-player can't be clicked
		if (currentCreatureObject != null && currentCreatureObject.Creature.Allegiance == Allegiance.Player) {
			currentCreatureObject.DealDamage (2.0f, creatureObject);
			currentCreatureObject.Deanimate ();

			int deadCount = 0; // not a good place for this script
			foreach (var enemyCreatureObject in EnemyCreatureObjects) {
				if (enemyCreatureObject.Creature.HP <= 0) {
					deadCount++;
				}
			}
			if (deadCount == EnemyCreatureObjects.Count) {			
				BJPlayer.Instance.OnDamageTaken -= BJPlayer_Instance_OnDamageTaken;
				Player.Instance.LoadVillage ();
			}

			Invoke ("StartTurn", 0.5f);
		}
	}

	IEnumerator Pause (float seconds) {
		yield return new WaitForSeconds (seconds);
	}
}
