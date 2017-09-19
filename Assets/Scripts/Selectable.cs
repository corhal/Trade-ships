using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

	protected List<Action> actions;
	public List<Action> Actions { get { return actions; } }
	public List<string> StatNames;

	public string Name;
	public int Level;
	public int MaxLevel;

	public string Process;
	public bool InProcess;
	public float InitialProcessSeconds;

	protected GameManager gameManager;
	protected Player player;

	bool animate;

	SpriteRenderer MySprite;

	protected void Awake () {
		actions = new List<Action> ();
		gameManager = GameManager.Instance;
		player = Player.Instance;
		Action infoAction = new Action("Info", 0, gameManager.ActionIconsByNames["Info"], ShowInfo);
		actions.Add (infoAction);
	}

	public void ShowInfo () {
		gameManager.OpenSelectableInfo (this);
	}

	public virtual float GetProcessSeconds () {
		return 0.0f;
	}

	protected void Start () {
		MySprite = GetComponentInChildren<SpriteRenderer> ();
	}

	protected virtual void Update () {
		if (animate) {
			MySprite.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 1));
		}
	}

	public virtual int GetStatByString (string statName) {
		return 0;
	}

	public void Deanimate () {
		animate = false;
		MySprite.color = Color.white;
	}

	public void Animate () {
		animate = true;
	}

	void OnMouseDown () {
		if (!Utility.IsPointerOverUIObject ()) {
			gameManager.OpentContextButtons (this);
		}
	}
}
