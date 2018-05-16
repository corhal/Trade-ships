﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJEffect : MonoBehaviour {

	public BJCreatureObject Applier;
	public Sprite EffectIcon;
	public int Damage;
	public BJCreatureObject Victim;
	public int TickPeriod;
	public int Duration;
	public int CurrentLifetime = 0;

	public ParticleSystem EffectParticles;
	public float KostyllYModifier;

	public virtual void Activate () {
		
	}

	public virtual void Tick () {
		CurrentLifetime++;
	}

	public virtual void Deactivate () {

	}
}
