using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BJGameController : MonoBehaviour {

	public BJDeck Deck;

	public GameObject CardObjectsContainer;
	public GameObject CardObjectPrefab;
	public List<GameObject> CardObjects;

	int currentScore;
	public Text ScoreLabel;

	void Start () {
		Deck = new BJDeck ();
		Deck.Shuffle ();
		Deal (2);
	}

	public void Deal (int count) {
		for (int i = 0; i < count; i++) {
			GameObject cardObject = Instantiate (CardObjectPrefab) as GameObject;
			BJCardObject bjCardObject = cardObject.GetComponent<BJCardObject> ();
			bjCardObject.Card = Deck.Deal ();
			bjCardObject.RankSuitLabel.text = bjCardObject.Card.Rank + "\n" + "of\n" + bjCardObject.Card.Suit;
			cardObject.transform.SetParent (CardObjectsContainer.transform);
			cardObject.transform.localScale = Vector3.one;
			CardObjects.Add (cardObject);
		}
		CountScore ();
	}

	void CountScore () {
		List<BJCard> Cards = new List<BJCard> ();
		List<BJCard> Aces = new List<BJCard> ();

		currentScore = 0;
		foreach (var cardObject in CardObjects) {
			BJCard card = cardObject.GetComponent<BJCardObject> ().Card; // bad use of GetComponent in foreach
			if (card.Rank == Ranks.Ace) { 
				Aces.Add (card);
			} else {
				Cards.Add (card);
			}
		}

		foreach (var card in Cards) {
			currentScore += (int)card.Rank;
		}

		foreach (var ace in Aces) {
			if (currentScore + (int)ace.Rank > 21) {
				currentScore += 1;
			} else {
				currentScore += (int)ace.Rank;
			}
		}

		ScoreLabel.text = "" + currentScore;
	}
}
