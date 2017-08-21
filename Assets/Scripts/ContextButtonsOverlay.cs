using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextButtonsOverlay : MonoBehaviour {

	public GameObject Overlay;
	public GameObject ButtonsContainer;
	public GameObject ContextButtonPrefab;
	public List<GameObject> ContextButtonObjects;

	public void Open (ISelectable selectable) {		
		Overlay.SetActive (true);
		foreach (var contextButtonObject in ContextButtonObjects) {
			Destroy (contextButtonObject);
		}
		ContextButtonObjects.Clear ();

		foreach (var action in selectable.Actions) {			
			GameObject contextButtonObject = Instantiate (ContextButtonPrefab) as GameObject;

			ContextButton contextButton = contextButtonObject.GetComponent<ContextButton> ();
			contextButton.ActionText.text = action.Name;
			contextButton.CostText.text = "" + action.Cost;

			contextButton.MyButton.onClick.AddListener (delegate {				
				action.Execute();
			});

			contextButtonObject.transform.SetParent (ButtonsContainer.transform);
			contextButtonObject.transform.localScale = Vector3.one;
			ContextButtonObjects.Add (contextButtonObject);

		}
			
	}
}
