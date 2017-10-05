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

	public Text ResultLabel;
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

		ResultButton.onClick.RemoveAllListeners ();

		if (ResultBuilding != null) {
			ResultLabel.text = building.Name;
			FormCraftElements (building.BuildCosts[building.Level]);
			ResultButton.onClick.AddListener (delegate {
				Build(building);
			});
		} else if (ResultItem != null) {
			ResultLabel.text = item.Name;
			FormCraftElements (item.CraftCost);
			ResultButton.onClick.AddListener (delegate {
				Player.Instance.Craft(item);
			});
			ResultButton.onClick.AddListener (delegate {
				Open(building, item);
			});
		}
	}

	void Build (Building building) {
		building.Build ();
		if (Player.Instance.CheckCost (building.BuildCosts[building.Level])) {
			Close ();
		}
	}

	void FormCraftElements (Dictionary<Item, int> amountsByItems) {
		foreach (var amountByItem in amountsByItems) {
			GameObject craftElementObject = Instantiate(CraftElementPrefab) as GameObject;

			CraftElement craftElement = craftElementObject.GetComponent<CraftElement> ();
			int playersAmount = (Player.Instance.Inventory.ContainsKey(amountByItem.Key)) ? Player.Instance.Inventory[amountByItem.Key] : 0;

			int requiredAmount = amountByItem.Value;
			craftElement.AmountLabel.text = playersAmount + "/" + requiredAmount;

			craftElement.NameLabel.text = amountByItem.Key.Name;
			craftElement.Icon.sprite = amountByItem.Key.Icon;

			if (playersAmount >= requiredAmount) {
				craftElement.FindOrCraftButton.gameObject.SetActive (false);
			} else {
				if (amountByItem.Key.CraftCost != null) {
					craftElement.FindOrCraftButton.GetComponentInChildren<Text> ().text = "Craft";
					//craftElement.FindOrCraftButton.onClick.RemoveAllListeners ();
					craftElement.FindOrCraftButton.onClick.AddListener (delegate {
						Open (null, amountByItem.Key);
					});
				} else {
					craftElement.FindOrCraftButton.GetComponentInChildren<Text> ().text = "Find";
					craftElement.FindOrCraftButton.onClick.AddListener (delegate {
						GameManager.Instance.FindMissionForItem(amountByItem.Key);
					});
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
