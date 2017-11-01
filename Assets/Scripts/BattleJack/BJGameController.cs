using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJGameController : MonoBehaviour {

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

	void Start () {
		Deck = new BJDeck ();
		Deck.Shuffle ();
		Hand = new BJDeck ();
		Hand.Flush ();

		DealCards (2);

		SpawnShips (3);
	}

	public void PlayerAttack () {
		BJShip currentTarget = null;
		foreach (var enemyShip in EnemyShipObjects) {
			if (enemyShip.Ship.HP > 0) {
				currentTarget = enemyShip.Ship;
				break;
			}
		}

		float multiplier = 1.0f + currentScore / 21.0f;
		foreach (var ship in BJPlayer.Instance.Ships) {
			ship.DealDamage (multiplier, currentTarget);
		}

		FlushCards ();
		DealCards (2);
	}

	public void DealCards (int count) {
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

		if (currentScore > 21) {
			Invoke ("FlushCards", 0.5f); // maybe should use coroutines instead
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

	public void SpawnShips (int amount) {
		for (int i = 0; i < amount; i++) {
			GameObject shipObject = Instantiate (ShipObjectPrefab) as GameObject;
			BJShipObject bjShipObject = shipObject.GetComponent<BJShipObject> ();
			bjShipObject.Ship = new BJShip (400, 100);
			shipObject.transform.SetParent (ShipObjectsContainer.transform);
			shipObject.transform.localScale = Vector3.one;
			EnemyShipObjects.Add (bjShipObject);
		}
	}

	IEnumerator Pause (float seconds) {
		yield return new WaitForSeconds (seconds);
	}
}
