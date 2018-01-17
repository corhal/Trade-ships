using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

	public bool IsAvailable = true;

	protected List<Action> actions;
	public List<Action> Actions { get { return actions; } }
	public List<string> StatNames;

	public string Name;
	public int Level;
	public int MaxLevel;
	public Allegiance Allegiance;

	public string Process;
	public bool InProcess;
	public float InitialProcessSeconds;

	protected UIOverlay uiManager;
	protected GameManager gameManager;
	protected Player player;

	bool animate;

	SpriteRenderer mySprite;
	Color initialColor;

	protected virtual void Awake () {
		
		actions = new List<Action> ();
		gameManager = GameManager.Instance;
		uiManager = UIOverlay.Instance;
		player = Player.Instance;
		Action infoAction = new Action("Info", 0, player.DataBase.ActionIconsByNames["Info"], ShowInfo);
		Action moveAction = new Action("Move", 0, player.DataBase.ActionIconsByNames["Info"], MoveShipHere);
		actions.Add (infoAction);
		actions.Add (moveAction);
	}

	public void RemoveActionByName (string actionName) {
		foreach (var action in actions) {
			if (action.Name == actionName) {
				actions.Remove (action);
			}
		}
	}

	public virtual void ShowInfo () {
		uiManager.OpenSelectableInfo (this);
	}

	public virtual void MoveShipHere () {
		PlayerShip.Instance.MoveToTile (this as SelectableTile, true);
		uiManager.CloseContextButtons (true);
	}

	public virtual float GetProcessSeconds () {
		return 0.0f;
	}

	protected virtual void Start () {
		mySprite = GetComponentInChildren<SpriteRenderer> ();
		initialColor = mySprite.color;
	}

	protected virtual void Update () {
		if (animate) {
			mySprite.color = Color.Lerp(initialColor, Color.black, Mathf.PingPong(Time.time, 1));
		}
	}

	public virtual int GetStatByString (string statName) {
		return 0;
	}

	public virtual int GetUpgradedStatByString (string statName) {
		return 0;
	}

	public virtual void Deanimate () {
		animate = false;
		mySprite.color = initialColor;
	}

	public virtual void Animate () {
		animate = true;
	}

	void OnMouseUp () {
		Invoke ("RealClick", 0.1f);
	}

	int clickCount = 0;
	void RealClick () {
		if (!GameManager.Instance.CameraDragged && IsAvailable && !Utility.IsPointerOverUIObject () && Allegiance != Allegiance.Enemy) {
			if (!GameManager.Instance.PlayerShip.CurrentTile.Neighbors.Contains((this as SelectableTile))) {
				return;
			}
			//uiManager.OpenContextButtons (this);
			clickCount++;
			Animate ();
			if (clickCount == 2) {
				MoveShipHere ();
				Deanimate ();
				clickCount = 0;
			}
		}
	}
}
