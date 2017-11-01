using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJGameController : MonoBehaviour {

	public BJDeck Deck;

	public GameObject CardObjectsContainer;
	public GameObject CardObjectPrefab;
	public List<GameObject> CardObjects;


	// Use this for initialization
	void Start () {
		Deck = new BJDeck ();
		Deal (2);
	}

	public void Deal (int count) {
		for (int i = 0; i < count; i++) {
			GameObject cardObject = Instantiate (CardObjectPrefab) as GameObject;
			BJCardObject bjCardObject = cardObject.GetComponent<BJCardObject> ();
			bjCardObject.Card = Deck.Deal ();
			cardObject.transform.SetParent (CardObjectsContainer.transform);
			cardObject.transform.localScale = Vector3.one;
		}
	}
}
