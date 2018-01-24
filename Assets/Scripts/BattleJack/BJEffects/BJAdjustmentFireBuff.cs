using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJAdjustmentFireBuff : BJEffect {

	public BJEffect AdjustmentFireEffect;
	public float EffectChance;

	public override void Activate () {
		foreach (var skill in Victim.Skills) {
			if (!skill.IsPassive) {
				skill.Effects.Add(AdjustmentFireEffect);
				skill.EffectChances.Add(EffectChance);
			}
		}
	}

	public override void Deactivate () {
		base.Deactivate ();
		foreach (var skill in Victim.Skills) {
			if (!skill.IsPassive) {
				skill.Effects.Remove(AdjustmentFireEffect);
				skill.EffectChances.Remove(EffectChance);
			}
		}
	}
}
