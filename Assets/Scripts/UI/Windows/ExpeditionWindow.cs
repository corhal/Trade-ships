using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionWindow : MonoBehaviour {

	public GameObject Window;
	public GameObject ExpeditionButtonObjectPrefab;

	public List<GameObject> ExpeditionButtonObjects;
	public List<GameObject> ExpeditionParents;

	public void Open (ExpeditionCenter expeditionCenter) {
		Window.SetActive (true);

		foreach (var expeditionButtonObject in ExpeditionButtonObjects) {
			Destroy (expeditionButtonObject);
		}
		ExpeditionButtonObjects.Clear ();

		foreach (var mission in expeditionCenter.Missions) {
			GameObject expeditionButtonObject = Instantiate (ExpeditionButtonObjectPrefab) as GameObject;

			expeditionButtonObject.GetComponent<Button> ().onClick.AddListener (delegate {
				GameManager.Instance.OpenMissionWindow(expeditionCenter, mission);
			});

			foreach (var expeditionParent in ExpeditionParents) {
				if (expeditionParent.GetComponentInChildren<Button>() == null) {
					expeditionButtonObject.transform.SetParent (expeditionParent.transform);
					expeditionButtonObject.transform.localScale = Vector3.one;
					expeditionButtonObject.transform.localPosition = Vector3.zero;
					ExpeditionButtonObjects.Add (expeditionButtonObject);
				}
			}
		}
	}

	public void Close () {
		foreach (var expeditionButtonObject in ExpeditionButtonObjects) {
			Destroy (expeditionButtonObject);
		}
		ExpeditionButtonObjects.Clear ();
		Window.SetActive (false);
	}

}
