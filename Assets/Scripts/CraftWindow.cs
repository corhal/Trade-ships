using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftWindow : MonoBehaviour {

	public GameObject Window;
	public GameObject CraftElementsContainer;
	public GameObject CraftElementPrefab;
	public List<GameObject> CraftElementObjects;

	public Building ResultBuilding;
	public Item ResultItem;

	public void Open (Building building, Item item) {
		ResultBuilding = building;
		ResultItem = item;
		Window.SetActive (true);
		foreach (var craftElementObject in CraftElementObjects) {
			Destroy (craftElementObject);
		}
		CraftElementObjects.Clear ();

		if (ResultBuilding != null) {
			foreach (var amountByItem in building.UpgradeCosts[building.Level]) {
				GameObject craftElementObject = Instantiate(CraftElementPrefab) as GameObject;

				CraftElement craftElement = craftElementObject.GetComponent<CraftElement> ();

				int playersAmount = Player.Instance.Inventory[amountByItem.Key];


				int requiredAmount = amountByItem.Value;
				craftElement.AmountLabel.text = playersAmount + "/" + requiredAmount;

				craftElementObject.transform.SetParent (CraftElementsContainer.transform);
				craftElementObject.transform.localScale = Vector3.one;
				CraftElementObjects.Add (craftElementObject);
			}
		} else if (ResultItem != null) {
			foreach (var amountByItem in item.CraftCost) {
				GameObject craftElementObject = Instantiate(CraftElementPrefab) as GameObject;

				CraftElement craftElement = craftElementObject.GetComponent<CraftElement> ();
				int playersAmount = Player.Instance.Inventory.TryGetValue (amountByItem.Key, 0);
				int requiredAmount = amountByItem.Value;
				craftElement.AmountLabel.text = playersAmount + "/" + requiredAmount;

				craftElementObject.transform.SetParent (CraftElementsContainer.transform);
				craftElementObject.transform.localScale = Vector3.one;
				CraftElementObjects.Add (craftElementObject);
			}
		}
	}

	public void Close () {
		Window.SetActive (false);
	}
}
