using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestButton : MonoBehaviour {

	public Image ChestImage;
	public GameObject TouchToOpenTag;
	public GameObject IsBeingOpenedTag;

	public Text LockedLabel;
	public Text TimeLabel;
	public Text ArenaLabel;
	public Text EmptySlotLabel;
	public Text OpenNowLabel;

	public RewardChest RewardChest;

	public void ReceiveChest (RewardChest rewardChest) {
		RewardChest = rewardChest;
		ChestImage.gameObject.SetActive (true);
	}

	public void AnotherChestIsBeingOpenedState () {
		LockedLabel.gameObject.SetActive (true);
		TouchToOpenTag.SetActive (false);
	}

	public void TouchToOpenState () {
		LockedLabel.gameObject.SetActive (false);
		TouchToOpenTag.SetActive (true);
	}

	public void IsBeingOpenedState () {
		IsBeingOpenedTag.SetActive (true);
		TouchToOpenTag.SetActive (false);
	}

	public void ReadyToOpenState () {
		// wah wah wah
	}

	public void EmptyState () {
		ChestImage.gameObject.SetActive (false);
		IsBeingOpenedTag.SetActive (false);
		OpenNowLabel.gameObject.SetActive (false);

		EmptySlotLabel.gameObject.SetActive (true);
	}
}
