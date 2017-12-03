using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour {

	public string Name;
	public Port MyPort = null;
	public FlagPost FlagPost;
	public Allegiance Allegiance;
	public List<Building> Buildings;

	void Awake () {
		MyPort = GetComponentInChildren<Port> ();
		FlagPost = GetComponentInChildren<FlagPost> ();
		// Buildings = new List <Building> (GetComponentsInChildren<Building> ());
	}

	public void Claim () {
		// MyPort.Claim ();
		Allegiance = Allegiance.Player;
		FlagPost.Invoke ("ChangeFlag", 2.0f);
		//FlagPost.ChangeFlag ();
		foreach (var building in Buildings) {
			building.Claim ();
			// Debug.Log ("Claiming " + building.Name);
		}
	}
}
