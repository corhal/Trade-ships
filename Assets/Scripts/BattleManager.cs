using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

	public GameObject ShipPrefab;
	public List<Ship> PlayerShips;

	public InfoWindow MyInfoWindow;
	public ShipWindow MyShipWindow;
	public PortWindow MyPortWindow;
	public CraftWindow MyCraftWindow;
	public ContextButtonsOverlay MyButtonsOverlay;
	public PopUp MyPopUp;

	public Selectable Selection;

	public void LoadVillage () {
		Player.Instance.SaveShips (PlayerShips);
		Player.Instance.LoadVillage ();
	}

	void Start () {
		if (!Player.Instance.FirstLoad) {
			/*for (int i = 0; i < Buildings.Count; i++) {
				for (int j = 0; j < Player.Instance.BuildingDatas.Count; j++) { // ОЛОЛО ОЛОЛО Я ВОДИТЕЛЬ НЛО
					Vector3 buildingPosition = new Vector3 (Player.Instance.BuildingDatas [i].Coordinates [0],
						Player.Instance.BuildingDatas [i].Coordinates [1],
						Player.Instance.BuildingDatas [i].Coordinates [2]);
					if (Buildings[i].Name == Player.Instance.BuildingDatas[j].Name && Vector3.Distance(Buildings[i].transform.position, buildingPosition) < 0.001f) {
						Buildings [i].InitializeFromData (Player.Instance.BuildingDatas [i]);
					}
				}
			}*/
			/*foreach (var shipData in Player.Instance.ShipDatas) {
				if (shipData.Allegiance != "Player") {
					continue;
				}
				GameObject shipObject = Instantiate (ShipPrefab) as GameObject;
				Ship ship = shipObject.GetComponent<Ship> ();
				ship.InitializeFromData (shipData);
				if (ship.Allegiance == "Player") {
					PlayerShips.Add (ship);
				}
			}*/
		}
	}
}
