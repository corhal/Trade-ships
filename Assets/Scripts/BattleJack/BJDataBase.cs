﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Suits {
	Hearts, Diamonds, Clubs, Spades
}

public enum Ranks {
	Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 10, Queen = 10, King = 10, Ace = 11
}

public class BJDataBase : MonoBehaviour {

	public List<Sprite> CharacterSprites;
	public List<Sprite> ShipSprites;
	public List<Sprite> CharacterPortraits;
	public List<Sprite> CharacterFigurines;
	public List<BJSkill> Skills;
}
