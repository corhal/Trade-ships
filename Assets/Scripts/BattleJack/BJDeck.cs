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
}
