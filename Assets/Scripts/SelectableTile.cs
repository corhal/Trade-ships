using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableTile : Selectable {

	public GameObject ParticleSystem;
	public GameObject ColliderObject;
	public Vector2Int BoardCoords;
	public string BoardCoordsAsString;

	public POIkind PointOfInterest;

	public void StopParticles () {
		ParticleSystem.SetActive (false);
		//ColliderObject.SetActive (false);
		Player.Instance.Tiles [BoardCoordsAsString] = false;
	}

	public override void MoveShipHere () {
		StopParticles ();
		base.MoveShipHere ();
	}
}
