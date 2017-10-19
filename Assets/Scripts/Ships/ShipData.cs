using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipData {	

	public bool IsSummoned;
	public List<Skill> Skills;
	public List<Effect> Effects;
	public List<Shipment> Shipments;

	public Item Blueprint;
	public List<List<Item>> PromoteCosts;
	public List<int> LevelRequirements;
	public RankColor RankColor;
	public int Stars;
	public int Level;
	public string Name;
	public string Allegiance;
	public int ShipmentsCapacity;
	public int HP;
	public int MaxHP;
	public int Power;
	public float[] Coordinates;
	public int Exp;

	public ShipData () {

	}

	public ShipData (string name, string allegiance, int level, int stars, int shipmentsCapacity, int maxHP, 
		int hp, int power, float[] coordinates, List<Skill> skills, List<Effect> effects, List<Shipment> shipments,
		Item blueprint, List<List<Item>> promoteCosts, RankColor rankColor, bool isSummoned, List<int> levelRequirements) {
		Name = name;
		Allegiance = allegiance;
		Level = level;
		Stars = stars;
		ShipmentsCapacity = shipmentsCapacity;
		MaxHP = maxHP;
		HP = hp;
		Power = power;
		Coordinates = new float[coordinates.Length];
		coordinates.CopyTo (Coordinates, 0);
		Blueprint = blueprint;
		Exp = 0;
		if (promoteCosts != null) {
			PromoteCosts = new List<List<Item>> (promoteCosts);
		} else {
			PromoteCosts = new List<List<Item>> ();
		}
		RankColor = rankColor;
		if (skills != null) {
			Skills = new List<Skill> (skills);
		} else {
			Skills = new List<Skill> ();
		}
		if (effects != null) {
			Effects = new List<Effect> (effects);
		} else {
			Effects = new List<Effect> ();
		}
		if (shipments != null) {
			Shipments = new List<Shipment> (shipments);
		} else {
			Shipments = new List<Shipment> ();
		}
		if (levelRequirements != null) {
			this.LevelRequirements = new List<int> (levelRequirements);
		} else {
			LevelRequirements = new List<int> ();
		}
		IsSummoned = isSummoned;
	}
	
	public void InitializeFromShip (Ship ship) {
		/*if (ship.Skills != null) {
			Skills = new List<Skill> (ship.Skills);
		}
		if (ship.Shipments != null) {
			Shipments = new List<Shipment> (ship.Shipments);
		}
		if (ship.Effects != null) {
			Effects = new List<Effect> (ship.Effects);
		}
		if (ship.PromoteCosts != null) {
			PromoteCosts = new List<List<Item>> (ship.PromoteCosts);
		}
		Blueprint = ship.Blueprint;
		Stars = ship.Stars;
		RankColor = ship.RankColor;
		Level = ship.Level;
		Exp = ship.Exp;
		Name = ship.Name;
		Allegiance = ship.Allegiance;
		ShipmentsCapacity = ship.ShipmentsCapacity;
		HP = ship.HP;
		MaxHP = ship.MaxHP;
		Power = ship.Power;
		Coordinates = new float[3];
		Coordinates [0] = ship.transform.position.x;
		Coordinates [1] = ship.transform.position.y;
		Coordinates [2] = ship.transform.position.z;
		IsSummoned = ship.IsSummoned;
		if (ship.LevelRequirements != null) {
			LevelRequirements = new List<int> (ship.LevelRequirements);
		}*/
	}

	public int TotalWeight { get {
			int totalWeight = 0;
			foreach (var myShipment in Shipments) {
				totalWeight += myShipment.Cargo;
			}
			return totalWeight;
		}}

	public void TakeShipment (Shipment shipment) {		
		if (ShipmentsCapacity - TotalWeight >= shipment.Cargo) {
			Shipments.Add (shipment);
		} else {
			//Debug.Log ("Couldn't take shipment: need " + shipment.Cargo + ", have " + (ShipmentsCapacity - TotalWeight));
		}
	}

	public bool CanTakeShipment (Shipment shipment) {
		if (ShipmentsCapacity - TotalWeight >= shipment.Cargo) {
			return true;
		} else {
			//Debug.Log ("Couldn't take shipment: need " + shipment.Cargo + ", have " + (ShipmentsCapacity - TotalWeight));
			return false;
		}
	}

	public void GiveShipment (Shipment shipment) {		
		Shipments.Remove (shipment);
	}
	
}
