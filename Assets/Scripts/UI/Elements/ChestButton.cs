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
	public Text OpeningTimeLabel;

	public RewardChest RewardChest;

	public void ReceiveChest (RewardChest rewardChest) {
		RewardChest = rewardChest;
		ChestImage.gameObject.SetActive (true);
		if (Player.Instance.CurrentlyOpeningChest != null) {
			AnotherChestIsBeingOpenedState ();
		} else {
			TouchToOpenState ();
		}
	}

	public void AnotherChestIsBeingOpenedState () {
		Debug.Log ("AnotherChestIsBeingOpenedState");
		LockedLabel.gameObject.SetActive (true);
		TouchToOpenTag.SetActive (false);
	}

	public void TouchToOpenState () {
		Debug.Log ("TouchToOpenState");
		EmptySlotLabel.gameObject.SetActive (false);
		LockedLabel.gameObject.SetActive (false);
		TouchToOpenTag.SetActive (true);
	}

	public void IsBeingOpenedState () {
		Debug.Log ("IsBeingOpenedState");
		IsBeingOpenedTag.SetActive (true);
		TouchToOpenTag.SetActive (false);
		OpenNowLabel.gameObject.SetActive (true);
		OpenNowLabel.text = "Speed up";
	}

	public void ReadyToOpenState () {
		Debug.Log ("ReadyToOpenState");
		LockedLabel.gameObject.SetActive (false); //
		IsBeingOpenedTag.SetActive (false);
		OpenNowLabel.gameObject.SetActive (true);
		OpenNowLabel.text = "Open";
	}

	public void EmptyState () {
		Debug.Log ("EmptyState");
		ChestImage.gameObject.SetActive (false);
		IsBeingOpenedTag.SetActive (false);
		OpenNowLabel.gameObject.SetActive (false);

		EmptySlotLabel.gameObject.SetActive (true);
	}

	void Update () {
		if (RewardChest != null && RewardChest.ChestState == ChestState.Opening) {
			int time = RewardChest.SecondsLeft;
			int hours = time / 3600;
			int minutes = (time - hours * 3600) / 60;
			int seconds = time - hours * 3600 - minutes * 60;
			OpeningTimeLabel.text = hours + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
		}
	}
}
