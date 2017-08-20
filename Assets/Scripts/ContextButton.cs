using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextButton : MonoBehaviour {

	public Image ActionIcon;
	public Text ActionText;
	public Text CostText;

	void InitButton(Action action) {
		Button button = GetComponentInChildren<Button> ();
		//button.onClick.AddListener(() => ButtonClicked(tempInt));
	}


}
