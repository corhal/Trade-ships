using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public List<Location> Locations;

	public PortWindow MyPortWindow;

	public void OpenPortWindow (Port port) {
		MyPortWindow.Open (port);
	}
}
