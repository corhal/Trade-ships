using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableTile : Selectable {

	public static int LastId;
	public int Id;

	public GameObject ParticleSystem;
	public GameObject ColliderObject;

	protected override void Awake () {
		base.Awake ();
		Id = LastId++;
		if (!Player.Instance.Tiles.ContainsKey(name)) {
			Player.Instance.Tiles.Add (name, true);
		}
		if (!GameManager.Instance.Tiles.Contains(this)) {
			GameManager.Instance.Tiles.Add (this);
		}
	}

	public void StopParticles () {
		ParticleSystem.SetActive (false);
		ColliderObject.SetActive (false);
		Player.Instance.Tiles [name] = false;
	}
}
