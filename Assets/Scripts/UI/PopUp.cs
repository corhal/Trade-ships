using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour {

	public GameObject Window;
	public Text MessageText;
	public Button OkButton;

	public void Open (string message) {
		Window.SetActive (true);

		MessageText.text = message;
	}

	public void Close () {
		Window.SetActive (false);
	}
}
