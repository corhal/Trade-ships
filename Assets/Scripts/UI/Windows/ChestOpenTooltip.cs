using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestOpenTooltip : MonoBehaviour {

	public GameObject Window;

	public Image ChestImage;
	public Text ArenaLabel;
	public Text ChestLabel;

	public Text GoldRewardLabel;
	public Text CardsRewardLabel;

	public Text TimerLabel;

	public Text OpenCostLabel;

	public RewardChest RewardChest;

	public GameObject LockpickParent;

	public List<LockpickElement> Lockpicks;

	public void Open (RewardChest rewardChest) {
		Window.SetActive (true);

		RewardChest = rewardChest;

		TimerLabel.text = Utility.SecondsToTime (RewardChest.SecondsLeft);
	}

	public void OpenLockpicks () {
		LockpickParent.SetActive (true);
		UpdateLockpicks ();
	}

	void UpdateLockpicks () {
		Lockpicks [0].LockpickNameLabel.text = "Copper lockpick";
		Lockpicks [0].LockpickCountLabel.text = Player.Instance.Inventory ["Copper lockpick"] + "";
		Lockpicks [0].LockpickTimeLabel.text = "-10m";
		Lockpicks [1].LockpickNameLabel.text = "Silver lockpick";
		Lockpicks [1].LockpickCountLabel.text = Player.Instance.Inventory ["Silver lockpick"] + "";
		Lockpicks [1].LockpickTimeLabel.text = "-1h";
	}

	public void SpeedUpChest () {
		// LockpickParent.SetActive (false);
		UpdateLockpicks ();
	}

	void Update () {
		if (Window.activeSelf) {
			TimerLabel.text = Utility.SecondsToTime (RewardChest.SecondsLeft);
		}
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void InstantOpenChest () {
		UIOverlay.Instance.OpenChestNow (RewardChest);
	}
}
