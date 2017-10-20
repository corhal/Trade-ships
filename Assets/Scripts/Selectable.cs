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
	public string Allegiance;

	public string Process;
	public bool InProcess;
	public float InitialProcessSeconds;

	protected GameManager gameManager;
	protected Player player;

	bool animate;

	SpriteRenderer mySprite;
	Color initialColor;

	protected virtual void Awake () {
		
		actions = new List<Action> ();
		gameManager = GameManager.Instance;
		player = Player.Instance;
		Action infoAction = new Action("Info", 0, player.DataBase.ActionIconsByNames["Info"], ShowInfo);
		actions.Add (infoAction);
	}

	public void RemoveActionByName (string actionName) {
		foreach (var action in actions) {
			if (action.Name == actionName) {
				actions.Remove (action);
			}
		}
	}

	public virtual void ShowInfo () {
		gameManager.OpenSelectableInfo (this);
	}

	public virtual float GetProcessSeconds () {
		return 0.0f;
	}

	protected virtual void Start () {
		mySprite = GetComponentInChildren<SpriteRenderer> ();
		initialColor = mySprite.color;
	}

	bool tempTest = false;
	protected virtual void Update () {
		if (animate) {
			//Debug.Log (tempTest);
			if (!tempTest) {
				Debug.Log (mySprite);
				tempTest = true;
			}
			mySprite.color = Color.Lerp(initialColor, Color.black, Mathf.PingPong(Time.time, 1));
		}
	}

	public virtual int GetStatByString (string statName) {
		return 0;
	}

	public virtual void Deanimate () {
		Debug.Log ("deanimating " + mySprite);
		animate = false;
		mySprite.color = initialColor;
	}

	public virtual void Animate () {
		animate = true;
		Debug.Log ("animating " + mySprite);
	}

	void OnMouseDown () {
		if (IsAvailable && !Utility.IsPointerOverUIObject () && Allegiance != "Enemy") {
			gameManager.OpentContextButtons (this);
		}
	}
}
