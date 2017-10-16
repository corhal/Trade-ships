﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect {

	public string Name;
	public int Level;
	public float Duration;
	//public float TickPeriod;
	public float ElapsedTime;

	public List<Dictionary<string, int>> StatEffects;

	public GameObject EffectParticlesPrefab;

	public Effect (string name, int level, float duration, List<Dictionary<string, int>> statEffects) {
		Name = name;
		Level = level;
		Duration = duration;
		StatEffects = statEffects;
		ElapsedTime = 0.0f;

		int index = Player.Instance.DataBase.EffectParticleNames.IndexOf (Name);
		if (index > -1) {
			EffectParticlesPrefab = Player.Instance.DataBase.EffectParticlePrefabs [index];
		}
	}

	public Effect Copy () {
		return new Effect (Name, Level, Duration, new List<Dictionary<string, int>> (StatEffects));
	}
}
