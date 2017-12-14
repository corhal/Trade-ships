using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableTile : Selectable {

	public GameObject ParticleSystem;
	public GameObject ColliderObject;
	public Vector2Int BoardCoords;
	public string BoardCoordsAsString;

	protected override void Awake () {
		base.Awake ();
		if (!Player.Instance.Tiles.ContainsKey(BoardCoordsAsString)) {
			Player.Instance.Tiles.Add (BoardCoordsAsString, true);
		}
		if (!GameManager.Instance.Tiles.Contains(this)) {
			GameManager.Instance.Tiles.Add (this);
		}
	}

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
