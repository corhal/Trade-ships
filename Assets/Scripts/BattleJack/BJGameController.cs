using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJGameController : MonoBehaviour {

	public Slider PlayerHPSlider;
	public Text PlayerHPLabel;
	public GameObject PlayerShipPrefab;
	public GameObject PlayerShipsContainer;
	public List<BJPlayerShipObject> PlayerShipObjects;

	public GameObject ShipObjectsContainer;
	public GameObject ShipObjectPrefab;
	public List<BJShipObject> EnemyShipObjects;

	public BJDeck Deck;
	public BJDeck Hand;

	public GameObject CardObjectsContainer;
	public GameObject CardObjectPrefab;
	public List<BJCardObject> CardObjects;

	int currentScore;
	public Text ScoreLabel;
	public Slider ScoreSlider;

	void Start () {
		Deck = new BJDeck ();
		Deck.Shuffle ();
		Hand = new BJDeck ();
		Hand.Flush ();

		StartCoroutine (DealCards (2, 0.0f));

		SpawnShips (3);

		BJPlayer.Instance.OnDamageTaken += BJPlayer_Instance_OnDamageTaken;

		PlayerHPSlider.maxValue = BJPlayer.Instance.MaxHP;
		PlayerHPSlider.value = BJPlayer.Instance.HP;
		PlayerHPLabel.text = BJPlayer.Instance.HP + "/" + BJPlayer.Instance.MaxHP;

		SpawnPlayerShips ();
		ScoreSlider.maxValue = 21;
	}

	void BJPlayer_Instance_OnDamageTaken ()	{
		PlayerHPSlider.value = BJPlayer.Instance.HP;
		PlayerHPLabel.text = BJPlayer.Instance.HP + "/" + BJPlayer.Instance.MaxHP;
	}

	public void PlayerAttack () {
		BJShipObject currentTarget = null;
		foreach (var enemyShip in EnemyShipObjects) {
			if (enemyShip.Ship.HP > 0) {
				currentTarget = enemyShip;
				break;
			}
		}

		float multiplier = 1.0f + currentScore / 21.0f; 
		foreach (var shipObject in PlayerShipObjects) {
			 shipObject.DealDamage (multiplier, currentTarget);
		}

		FlushCards ();
		// EnemyAttack ();
		int deadCount = 0;
		foreach (var enemyShipObject in EnemyShipObjects) {
			if (enemyShipObject.Ship.HP <= 0) {
				deadCount++;
			}
		}
		if (deadCount == EnemyShipObjects.Count) {
			//Player.Instance.TakeItems (Player.Instance.CurrentMission.GiveReward ());
			BJPlayer.Instance.OnDamageTaken -= BJPlayer_Instance_OnDamageTaken;
			Player.Instance.LoadVillage ();
		} else {
			Invoke ("EnemyAttack", 0.5f);
		}
	}

	public void EnemyAttack () {
		float multiplier = 1.0f;
		foreach (var shipObject in EnemyShipObjects) {
			if (shipObject.Ship.HP > 0) {
				shipObject.DealDamage (multiplier, BJPlayer.Instance);
			}
		}

		StartCoroutine (DealCards (2, 0.5f));
	}

	public void ClickDealButton () {
		StartCoroutine (DealCards (1, 0.0f));
	}

	public IEnumerator DealCards (int count, float pause) {
		yield return new WaitForSeconds (pause);
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

	public void SpawnPlayerShips () {
		foreach (var ship in BJPlayer.Instance.Ships) {
			GameObject shipObject = Instantiate (PlayerShipPrefab) as GameObject;
			BJPlayerShipObject bjPlayerShipObject = shipObject.GetComponent<BJPlayerShipObject> ();
			bjPlayerShipObject.Ship = ship;
			shipObject.transform.SetParent (PlayerShipsContainer.transform);
			shipObject.transform.localScale = Vector3.one;
			PlayerShipObjects.Add (bjPlayerShipObject);
		}
	}

	public void SpawnShips (int amount) {
		if (Player.Instance != null) { // kostyll
			foreach (var enemyShipData in Player.Instance.CurrentMission.EnemyShips) {
				SpawnShip (enemyShipData.MaxHP, enemyShipData.Power);
			}
		} else {
			for (int i = 0; i < amount; i++) {
				SpawnShip (400, 10);
			}
		}
	}

	void SpawnShip (int hp, int baseDamage) {
		GameObject shipObject = Instantiate (ShipObjectPrefab) as GameObject;
		BJShipObject bjShipObject = shipObject.GetComponent<BJShipObject> ();
		bjShipObject.Ship = new BJShip (hp, baseDamage);
		shipObject.transform.SetParent (ShipObjectsContainer.transform);
		shipObject.transform.localScale = Vector3.one;
		EnemyShipObjects.Add (bjShipObject);
	}

	IEnumerator Pause (float seconds) {
		yield return new WaitForSeconds (seconds);
	}
}
