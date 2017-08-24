using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftWindow : MonoBehaviour {

	public GameObject Window;
	public GameObject CraftElementsContainer;
	public GameObject CraftElementPrefab;
	public List<GameObject> CraftElementObjects;

	public Button ResultButton;

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
			FormCraftElements (building.BuildCosts[building.Level]);
			ResultButton.onClick.AddListener (delegate {
				building.Build();
			});
			ResultButton.onClick.AddListener (delegate {
				Close();
			});
		} else if (ResultItem != null) {
			FormCraftElements (item.CraftCost);
		}
	}

	void FormCraftElements (Dictionary<Item, int> amountsByItems) {
		foreach (var amountByItem in amountsByItems) {
			GameObject craftElementObject = Instantiate(CraftElementPrefab) as GameObject;

			CraftElement craftElement = craftElementObject.GetComponent<CraftElement> ();
			int playersAmount = (Player.Instance.Inventory.ContainsKey(amountByItem.Key)) ? Player.Instance.Inventory[amountByItem.Key] : 0;

			int requiredAmount = amountByItem.Value;
			craftElement.AmountLabel.text = playersAmount + "/" + requiredAmount;

			if (playersAmount >= requiredAmount) {
				craftElement.FindOrCraftButton.gameObject.SetActive (false);
			} else {
				if (amountByItem.Key.CraftCost != null) {
					craftElement.FindOrCraftButton.GetComponentInChildren<Text> ().text = "Craft";
					craftElement.FindOrCraftButton.onClick.AddListener (delegate {
						Open (null, amountByItem.Key);
					});
				} else {
					craftElement.FindOrCraftButton.GetComponentInChildren<Text> ().text = "Find";
				}
			}

			craftElementObject.transform.SetParent (CraftElementsContainer.transform);
			craftElementObject.transform.localScale = Vector3.one;
			CraftElementObjects.Add (craftElementObject);
		}
	}

	public void Close () {
		Window.SetActive (false);
	}
}
