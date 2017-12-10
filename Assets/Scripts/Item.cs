using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item {

	public string Name;
	public Sprite Icon;

	public Item (string name, Sprite icon) {
		this.Name = name;
		this.Icon = icon;
	}
}
