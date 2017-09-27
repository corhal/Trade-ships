using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipData {	

	public List<Skill> Skills;
	public List<Shipment> Shipments;
	
	public int Level;
	public string Name;
	public int ShipmentsCapacity;
	public int HP;
	public int MaxHP;
	public int Power;
	public float[] Coordinates;
	
	public void InitializeFromShip (Ship ship) {
		if (ship.Skills != null) {
			Skills = new List<Skill> (ship.Skills);
		}
		if (ship.Shipments != null) {
			Shipments = new List<Shipment> (ship.Shipments);
		}
		Level = ship.Level;
		Name = ship.Name;
		ShipmentsCapacity = ship.ShipmentsCapacity;
		HP = ship.HP;
		MaxHP = ship.MaxHP;
		Power = ship.Power;
		Coordinates = new float[3];
		Coordinates [0] = ship.transform.position.x;
		Coordinates [1] = ship.transform.position.y;
		Coordinates [2] = ship.transform.position.z;
	}
	
}
