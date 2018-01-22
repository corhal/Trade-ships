using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJBleedBuff : BJEffect {

	public BJEffect Bleed;
	public float EffectChance;

	public override void Activate () {
		Debug.Log ("start" + Victim.Name);
		foreach (var skill in Victim.Skills) {
			if (!skill.IsPassive) {
				Debug.Log (Victim.Name);
				skill.Effects.Add(Bleed);
				skill.EffectChances.Add(EffectChance);
			}
		}
	}

	public override void Deactivate () {
		base.Deactivate ();
		foreach (var skill in Victim.Skills) {
			if (!skill.IsPassive) {
				skill.Effects.Remove(Bleed);
				skill.EffectChances.Remove(EffectChance);
			}
		}
	}

	/*public float Chance;

	public override void Tick () {
		base.Tick ();

		if (Random.Range(0.0f, 0.99f) < Chance) {
			foreach (var skill in Victim.Skills) {
				if (!skill.IsPassive) {
					skill.Effect = Bleed;
				}
			}
		} else {
			foreach (var skill in Victim.Skills) {
				if (!skill.IsPassive) {
					skill.Effect = null;
				}
			}
		}
	}*/
}
