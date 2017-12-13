using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableTile : Selectable {

	public GameObject ParticleSystem;
	public GameObject ColliderObject;

	protected override void Awake () {
		base.Awake ();
		if (!Player.Instance.Tiles.ContainsKey(name)) {
			Player.Instance.Tiles.Add (name, true);
		}
		if (!GameManager.Instance.Tiles.Contains(this)) {
			GameManager.Instance.Tiles.Add (this);
		}
	}

	public void StopParticles () {
		ParticleSystem.SetActive (false);
		//ColliderObject.SetActive (false);
		Player.Instance.Tiles [name] = false;
	}

	public override void MoveShipHere () {
		StopParticles ();
		base.MoveShipHere ();
	}
}
