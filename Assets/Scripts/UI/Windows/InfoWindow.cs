using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour {

	public GameObject Window;

	public GameObject StatsElementContainer;

	public GameObject StatElementPrefab;

	public List<GameObject> StatElementObjects;

	public Text HeaderLabel;

	// GameManager gameManager;

	void Awake () {
		// gameManager = GameManager.Instance;
	}

	public void Open (Selectable selectable) {		
		Window.SetActive (true);

		HeaderLabel.text = selectable.Name;
		foreach (var statElementObject in StatElementObjects) {
			Destroy (statElementObject);
		}
		StatElementObjects.Clear ();

		foreach (var statName in selectable.StatNames) {
			GameObject statElementObject = Instantiate (StatElementPrefab) as GameObject;
			Text statText = statElementObject.GetComponent<Text> ();
			statText.text = statName + ": " + selectable.GetStatByString (statName);

			statElementObject.transform.SetParent (StatsElementContainer.transform);
			statElementObject.transform.localScale = Vector3.one;
			StatElementObjects.Add (statElementObject);
		}
	}

	public void Close () {
		Window.SetActive (false);
	}
}
