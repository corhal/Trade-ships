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

	public Slider ProcessSlider;
	public Text ProcessText;

	Selectable currentSelectable;

	public void Open (Selectable selectable) {		
		ProcessSlider.gameObject.SetActive (false);
		ProcessText.gameObject.SetActive (false);
		currentSelectable = selectable;
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

		if (selectable.Process != null && selectable.InProcess) {
			ProcessSlider.gameObject.SetActive (true);
			ProcessText.gameObject.SetActive (true);

			ProcessSlider.value = selectable.InitialProcessSeconds - selectable.GetProcessSeconds ();
			ProcessSlider.maxValue = selectable.InitialProcessSeconds;

			ProcessText.text = string.Format("{0}: {1:D2}:{2:D2}:{3:D2}", selectable.Process, 0, 0, (int)selectable.GetProcessSeconds ());
		}
	}

	void Update () {
		if (currentSelectable != null && currentSelectable.Process != null && currentSelectable.InProcess) {
			ProcessSlider.gameObject.SetActive (true);
			ProcessText.gameObject.SetActive (true);

			ProcessSlider.value = currentSelectable.InitialProcessSeconds - currentSelectable.GetProcessSeconds ();
			ProcessText.text = string.Format("{0}: {1:D2}:{2:D2}:{3:D2}", currentSelectable.Process, 0, 0, (int)currentSelectable.GetProcessSeconds ());
			if (Mathf.Approximately(ProcessSlider.value, ProcessSlider.maxValue)) {
				ProcessSlider.gameObject.SetActive (false);
				ProcessText.gameObject.SetActive (false);
			}
		}
	}

	public void Close () {
		ProcessSlider.gameObject.SetActive (false);
		ProcessText.gameObject.SetActive (false);
		GameManager.Instance.CloseContextButtons (true);
	}
}
