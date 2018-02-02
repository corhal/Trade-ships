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

		ArenaLabel.text = Player.Instance.CurrentAdventure.Ocean;
		RewardChest = rewardChest;
		TimerLabel.text = Utility.SecondsToTime (RewardChest.SecondsLeft);
	}

	public void OpenLockpicks () {
		LockpickParent.SetActive (true);
		UpdateLockpicks ();
	}

	public void CloseLockpicks () {
		LockpickParent.SetActive (false);
	}

	void UpdateLockpicks () {
		Lockpicks [0].LockpickNameLabel.text = "Copper lockpick";
		Lockpicks [0].LockpickCountLabel.text = Player.Instance.Inventory ["Copper lockpick"] + "";
		Lockpicks [0].LockpickTimeLabel.text = "-4m";
		Lockpicks [1].LockpickNameLabel.text = "Silver lockpick";
		Lockpicks [1].LockpickCountLabel.text = Player.Instance.Inventory ["Silver lockpick"] + "";
		Lockpicks [1].LockpickTimeLabel.text = "-10m";
		Lockpicks [2].LockpickNameLabel.text = "Golden lockpick";
		Lockpicks [2].LockpickCountLabel.text = Player.Instance.Inventory ["Golden lockpick"] + "";
		Lockpicks [2].LockpickTimeLabel.text = "-30m";
	}

	public void SpeedUpChest () {
		// LockpickParent.SetActive (false);
		UpdateLockpicks ();
	}

	void Update () {
		if (Window.activeSelf) {
			TimerLabel.text = Utility.SecondsToTime (RewardChest.SecondsLeft);
			int openCost = Mathf.Max (RewardChest.SecondsLeft / 600, 1);
			OpenCostLabel.text = openCost + "";
		}
	}

	public void Close () {
		Window.SetActive (false);
	}

	public void InstantOpenChest () {
		int openCost = Mathf.Max (RewardChest.SecondsLeft / 600, 1);
		if (Player.Instance.Inventory ["Gems"] >= openCost) {
			Player.Instance.Inventory ["Gems"] -= openCost;
			UIOverlay.Instance.OpenChestNow (RewardChest);
		}
	}

	public void UseLockpick (int index) { // govnocode
		if (Player.Instance.Inventory [Lockpicks [index].LockpickNameLabel.text] > 0) {
			Player.Instance.Inventory [Lockpicks [index].LockpickNameLabel.text]--;
			int seconds = 0;
			switch (Lockpicks [index].LockpickNameLabel.text) {
			case "Copper lockpick":
				seconds = 4 * 60;
				break;
			case "Silver lockpick":
				seconds = 10 * 60;
				break;
			case "Gold lockpick":
				seconds = 30 * 60;
				break;
			default:
				seconds = 0;
				break;
			}
			RewardChest.TickOpen (seconds);
			UpdateLockpicks ();
		}
	}
}
