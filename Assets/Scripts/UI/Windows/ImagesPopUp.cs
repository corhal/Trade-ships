﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagesPopUp : MonoBehaviour {

	public GameObject Window;
	public Text MessageText;
	public GameObject ImagesParent;
	public Button OkButton;

	public GameObject RewardElementPrefab;
	public List<GameObject> RewardElementObjects;

	public void Open (string message, Dictionary<string, int> itemNames) {
		Window.SetActive (true);

		MessageText.text = message;

		foreach (var rewardElementObject in RewardElementObjects) {
			Destroy (rewardElementObject);
		}
		RewardElementObjects.Clear ();

		foreach (var amountByItem in itemNames) {
			GameObject rewardElementObject = Instantiate (RewardElementPrefab) as GameObject;
			Text[] texts = rewardElementObject.GetComponentsInChildren<Text> ();
			texts [0].text = amountByItem.Key;
			/*if (amountByItem.Key.Name == "") {
				Debug.Log (amountByItem.Key);
			}*/
			texts [1].text = amountByItem.Value.ToString ();
			Image rewardImage = rewardElementObject.GetComponentInChildren<Image> ();
			rewardImage.sprite = Player.Instance.DataBase.ItemIconsByNames [amountByItem.Key];

			rewardElementObject.transform.SetParent (ImagesParent.transform);
			rewardElementObject.transform.localScale = Vector3.one;
			RewardElementObjects.Add (rewardElementObject);
		}
	}

	public void Close () {
		Window.SetActive (false);
	}
}