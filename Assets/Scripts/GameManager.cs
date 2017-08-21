using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public List<Location> Locations;

	public PortWindow MyPortWindow;
	public ContextButtonsOverlay MyButtonsOverlay;

	public void OpenPortWindow (Port port, Ship ship) {
		MyPortWindow.Open (port, ship);
	}

	public void OpentContextButtons (ISelectable selectable) {
		MyButtonsOverlay.Open (selectable);
	}
}
