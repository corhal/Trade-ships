using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

	protected List<Action> actions;
	public List<Action> Actions { get { return actions; } }

	public string Name;
	public int Level;
	public int MaxLevel;

	protected GameManager gameManager;
	protected Player player;

	protected void Awake () {
		actions = new List<Action> ();
		gameManager = GameManager.Instance;
		player = Player.Instance;
	}

	protected void Start () {
		
	}

	void OnMouseDown () {
		if (!Utility.IsPointerOverUIObject ()) {
			gameManager.OpentContextButtons (this);
		}
	}
}
