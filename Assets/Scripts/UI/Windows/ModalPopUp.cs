using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalPopUp : MonoBehaviour {

	public GameObject Window;
	public Text MessageText;
	public Button YesButton;
	public Button NoButton;

	public void Open (string message) {
		Window.SetActive (true);

		MessageText.text = message;
	}

	public void Close () {
		Window.SetActive (false);
	}
}
