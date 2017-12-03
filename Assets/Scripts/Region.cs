using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : Selectable {

	public int UnlockCost;
	List<Selectable> selectables;

	protected override void Awake () {
		base.Awake ();
		Collider2D myCollider = gameObject.GetComponent<Collider2D> ();
		Collider2D[] colliders = new Collider2D[20];
		ContactFilter2D filter = new ContactFilter2D ();
		filter.useTriggers = true;
		myCollider.OverlapCollider(filter, colliders);
		selectables = new List<Selectable> ();
		foreach (var collider in colliders) {
			if (collider != null && collider.gameObject.GetComponent<Selectable>() != null) {
				collider.gameObject.GetComponent<Selectable> ().IsAvailable = false;
				selectables.Add (collider.gameObject.GetComponent<Selectable> ());
			}
		}
	}

	protected override void Start () {
		base.Start ();
		Action unlockAction = new Action ("Unlock", UnlockCost, player.DataBase.ActionIconsByNames["Unlock"], Unlock);
		actions.Add (unlockAction);
	}

	void Unlock () {
		if (player.Gold >= UnlockCost) {			
			player.GiveGold (UnlockCost);
			UIOverlay.Instance.CloseContextButtons (true);
			foreach (var selectable in selectables) {
				selectable.IsAvailable = true;
			}
			Destroy (gameObject);
		} else {
			UIOverlay.Instance.OpenPopUp ("Not enough gold for unlock");
		}
	}
}
