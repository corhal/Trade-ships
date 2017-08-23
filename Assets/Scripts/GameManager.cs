using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public bool InMoveMode = false;
	public List<Location> Locations;
	public List<Item> TempItemLibrary;

	public PortWindow MyPortWindow;
	public ContextButtonsOverlay MyButtonsOverlay;

	public static GameManager Instance;

	void Awake () {
		Instance = this;
	}

	void Start () {
		TempItemLibrary = new List<Item> {
			new Item("Wood"),
			new Item("Food"),
			new Item("Steel"),
			new Item("Nails"),
			new Item("Tools"),};
	}

	public void OpenPortWindow (Port port, Ship ship) {
		if (InMoveMode) {
			return;
		}
		MyPortWindow.Open (port, ship);
		MyButtonsOverlay.Close ();
	}

	public void ClosePortWindow () {
		MyPortWindow.Close ();
	}

	public void OpentContextButtons (ISelectable selectable) {
		if (InMoveMode) {
			return;
		}
		MyButtonsOverlay.Open (selectable);
		MyPortWindow.Close ();
	}

	public void CloseContextButtons () {
		MyButtonsOverlay.Close ();
	}
}
