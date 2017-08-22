using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public bool InMoveMode = false;
	public List<Location> Locations;

	public PortWindow MyPortWindow;
	public ContextButtonsOverlay MyButtonsOverlay;

	public static GameManager Instance;

	void Awake () {
		Instance = this;
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
