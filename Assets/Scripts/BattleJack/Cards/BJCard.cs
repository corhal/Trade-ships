using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BJCard {

	Suits suit;
	public Suits Suit { get { return suit; } }
	Ranks rank;
	public Ranks Rank { get { return rank; } }

	public BJCard (Suits suit, Ranks rank) {
		this.suit = suit;
		this.rank = rank;
	}
}
