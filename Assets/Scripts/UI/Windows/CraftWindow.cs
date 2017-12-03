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
	public Button FindButton;

	public Text ResultLabel;
	public Image ResultImage;
	public Text StatsLabel;
	public Building ResultBuilding;
	public string ResultItem;

	public GameObject AcquirePanel;
	public GameObject FindPanel;

	public void Open (Building building, string item) {
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
			ResultImage.sprite = building.gameObject.GetComponentInChildren<SpriteRenderer> ().sprite;
			StatsLabel.text = "";
			/*foreach (var statName in ResultBuilding.StatNames) {
				StatsLabel.text += statName + ": +" + ResultBuilding.GetUpgradedStatByString (statName) + "\n";
			}*/
			FindPanel.SetActive (false);
			AcquirePanel.SetActive (true);
			FormCraftElements (building.BuildCosts[building.Level]);
			ResultButton.onClick.AddListener (delegate {
				Build(building);
			});
		} else if (ResultItem != null) {
			ResultLabel.text = item;
			ResultImage.sprite = Player.Instance.DataBase.ItemIconsByNames [item];
			StatsLabel.text = "";
			foreach (var statByName in Player.Instance.DataBase.ItemsByNames [ResultItem].StatsByNames) {
				StatsLabel.text += statByName.Key + ": +" + statByName.Value + "\n";
			}

			if (Player.Instance.DataBase.ItemsByNames [item] != null && Player.Instance.DataBase.ItemsByNames [item].CraftCost != null && Player.Instance.DataBase.ItemsByNames [item].CraftCost.Count > 0) {
				FindPanel.SetActive (false);
				AcquirePanel.SetActive (true);
				FormCraftElements (Player.Instance.DataBase.ItemsByNames [item].CraftCost);

				ResultButton.onClick.AddListener (delegate {
					Player.Instance.Craft (item);
				});
				ResultButton.onClick.AddListener (delegate {
					Open(building, item);
				});
			} else {
				FindPanel.SetActive (true);
				AcquirePanel.SetActive (false);

				FindButton.onClick.AddListener (delegate {
					GameManager.Instance.FindMissionForItem(item);
				});
			}
		}
	}

	void Build (Building building) {
		building.Build ();
		if (Player.Instance.CheckCost (building.BuildCosts[building.Level])) {
			Close ();
		}
	}

	void FormCraftElements (Dictionary<string, int> amountsByItems) {
		foreach (var amountByItem in amountsByItems) {
			GameObject craftElementObject = Instantiate(CraftElementPrefab) as GameObject;

			CraftElement craftElement = craftElementObject.GetComponent<CraftElement> ();
			int playersAmount = (Player.Instance.Inventory.ContainsKey(amountByItem.Key)) ? Player.Instance.Inventory[amountByItem.Key] : 0;

			int requiredAmount = amountByItem.Value;
			craftElement.AmountLabel.text = playersAmount + "/" + requiredAmount;

			craftElement.NameLabel.text = amountByItem.Key;
			craftElement.Icon.sprite = Player.Instance.DataBase.ItemIconsByNames [amountByItem.Key];

			if (playersAmount >= requiredAmount) {
				craftElement.FindOrCraftButton.gameObject.SetActive (false);
			} else {
				if (Player.Instance.DataBase.ItemsByNames[amountByItem.Key].CraftCost != null) {
					craftElement.FindOrCraftButton.GetComponentInChildren<Text> ().text = "Craft";
					//craftElement.FindOrCraftButton.onClick.RemoveAllListeners ();
					craftElement.FindOrCraftButton.onClick.AddListener (delegate {
						Open (null, amountByItem.Key);
					});
				} else {
					craftElement.FindOrCraftButton.GetComponentInChildren<Text> ().text = "Find";
					craftElement.FindOrCraftButton.onClick.AddListener (delegate {
						//GameManager.Instance.FindMissionForItem(amountByItem.Key);
						Open (null, amountByItem.Key);
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
