using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableTile : Selectable {

	public GameObject ParticleSystem;
	public GameObject ColliderObject;

	public void StopParticles () {
		ParticleSystem.SetActive (false);
		ColliderObject.SetActive (false);
	}
}
