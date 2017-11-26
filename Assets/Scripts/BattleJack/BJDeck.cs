using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJDeck {

	public List<BJCard> Cards; // should probably use Queue?

	public BJDeck () {
		Cards = new List<BJCard> ();
		foreach (Suits suit in System.Enum.GetValues(typeof(Suits))) {
			foreach (Ranks rank in System.Enum.GetValues(typeof(Ranks))) {
				Cards.Add (new BJCard (suit, rank));
			}
		}
	}

	public void Shuffle () {
		Utility.Shuffle (Cards);
	}

	public BJCard Deal () {
		BJCard card = Cards [Cards.Count - 1];
		Cards.RemoveAt (Cards.Count - 1);
		return card;
	}

	public BJCard Peek () {
		BJCard card = Cards [Cards.Count - 1];
		return card;
	}

	public void Add (BJCard card) {
		Cards.Add (card);

	}

	public void Flush () {
		Cards.Clear ();
	}

	public int CountScore () {		
		List<BJCard> cards = new List<BJCard> ();
		List<BJCard> aces = new List<BJCard> ();

		int score = 0;
		foreach (var card in Cards) {			
			if (card.Rank == Ranks.Ace) { 
				aces.Add (card);
			} else {
				cards.Add (card);
			}
		}

		foreach (var card in cards) {
			score += (int)card.Rank;
		}

		foreach (var ace in aces) {
			if (score + (int)ace.Rank > 21) {
				score += 1;
			} else {
				score += (int)ace.Rank;
			}
		}

		return score;
	}
}
