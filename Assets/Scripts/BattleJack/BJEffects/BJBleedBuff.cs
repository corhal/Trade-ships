using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJBleedBuff : BJEffect {

	public BJEffect Bleed;
	public float EffectChance;

	public override void Activate () {
		foreach (var skill in Victim.Skills) {
			if (!skill.IsPassive) {
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
}
