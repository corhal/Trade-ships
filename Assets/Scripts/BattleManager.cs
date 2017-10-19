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
		Player.Instance.LoadVillage ();
	}

	void Start () {
		
	}
}
