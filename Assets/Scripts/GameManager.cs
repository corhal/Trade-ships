﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public bool InMoveMode = false;
	public List<Island> Islands;
	public List<Item> TempItemLibrary;

	public List<Ship> Ships;
	public List<Building> Buildings;

	public MissionWindow MyMissionWindow;
	public PortWindow MyPortWindow;
	public CraftWindow MyCraftWindow;
	public ContextButtonsOverlay MyButtonsOverlay;

	public static GameManager Instance;

	void Awake () {
		Instance = this;
	}

	void Start () {
		Item wood = new Item ("Wood", null);
		Item food = new Item ("Food", null);
		Item steel = new Item ("Steel", null);
		Item nails = new Item ("Nails", new Dictionary<Item, int> { { steel, 2 } });
		Item hammers = new Item ("Hammers", new Dictionary<Item, int> { { steel, 1 }, {wood, 1} });
		Item saws = new Item ("Saws", new Dictionary<Item, int> { { steel, 2 }, {wood, 1} });
		Item tools = new Item ("Tools", new Dictionary<Item, int> { { hammers, 1 }, {saws, 1} });
		TempItemLibrary = new List<Item> {
			wood,
			food,
			steel,
			nails,
			hammers,
			saws,
			tools,};

		Ships = new List<Ship> (GameObject.FindObjectsOfType<Ship>());
		Buildings = new List<Building> (GameObject.FindObjectsOfType<Building>());
	}

	public void OpenMissionWindow (ExpeditionCenter expeditionCenter) {
		MyMissionWindow.Open (expeditionCenter);
		MyPortWindow.Close ();
		MyCraftWindow.Close ();
		MyButtonsOverlay.Close ();
	}

	public void CloseMissionWindow () {

	}

	public void OpenPortWindow (Port port, Ship ship) {
		if (InMoveMode) {
			return;
		}
		MyPortWindow.Open (port, ship);
		MyCraftWindow.Close ();
		MyButtonsOverlay.Close ();
	}

	public void ClosePortWindow () {
		MyPortWindow.Close ();
	}

	public void OpenCraftWindow (Building building, Item item) {
		MyCraftWindow.Open (building, item);
		MyButtonsOverlay.Close ();
		MyPortWindow.Close ();
	}

	public void CloseCraftWindow () {
		MyCraftWindow.Close ();
	}

	public void OpentContextButtons (Selectable selectable) {
		if (InMoveMode) {
			return;
		}
		MyButtonsOverlay.Open (selectable);
		MyPortWindow.Close ();
		MyCraftWindow.Close ();
	}

	public void CloseContextButtons () {
		MyButtonsOverlay.Close ();
	}
}
