using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextButtonsOverlay : MonoBehaviour {

	public Text HeaderText;
	public GameObject Overlay;
	public GameObject ButtonsContainer;
	public GameObject ContextButtonPrefab;
	public List<GameObject> ContextButtonObjects;

	public void Open (Selectable selectable) {		
		Overlay.SetActive (true);
		foreach (var contextButtonObject in ContextButtonObjects) {
			Destroy (contextButtonObject);
		}
		ContextButtonObjects.Clear ();
		HeaderText.text = selectable.Name + " lvl " + selectable.Level;

		foreach (var action in selectable.Actions) {			
			GameObject contextButtonObject = Instantiate (ContextButtonPrefab) as GameObject;

			ContextButton contextButton = contextButtonObject.GetComponent<ContextButton> ();
			contextButton.ActionText.text = action.Name;
			contextButton.CostText.text = "" + action.Cost;
			if (action.Cost == 0) {
				contextButton.CostText.gameObject.SetActive (false);
			}

			contextButton.MyButton.onClick.AddListener (delegate {				
				action.Execute();
			});
			contextButton.ActionIcon.sprite = action.Icon;
			contextButtonObject.transform.SetParent (ButtonsContainer.transform);
			contextButtonObject.transform.localScale = Vector3.one;
			ContextButtonObjects.Add (contextButtonObject);

		}			
	}

	public void Close () {
		GameManager.Instance.CloseContextButtons (true);
	}
}
