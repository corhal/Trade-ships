using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : Selectable {
	

	protected override void Awake () {
		base.Awake ();
		Collider2D myCollider = gameObject.GetComponent<Collider2D> ();
		Collider2D[] colliders = new Collider2D[20];
		ContactFilter2D filter = new ContactFilter2D ();
		filter.useTriggers = true;
		myCollider.OverlapCollider(filter, colliders);
		foreach (var collider in colliders) {
			Debug.Log (collider);
		}
	}
}
