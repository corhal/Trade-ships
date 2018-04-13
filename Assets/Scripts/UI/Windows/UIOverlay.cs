using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlay : MonoBehaviour {

	public bool InMoveMode;
	public Selectable Selection;

	public Text GoldLabel;
	public Text EnergyLabel;
	public Text GemsLabel;
	public Text TimeLabel;
	public Text ChestsLabel;
	public Text SecondaryChestTimer;
	public GameObject ChestsActivityMarker;
	public Text LevelLabel;
	public Text ExpLabel;
	public Slider ExpSlider;

	Player player;
	public static UIOverlay Instance;

	public List<GameObject> HideInAdventureObjects;
	public List<GameObject> HideOffAdventureObjects;

	public TeamSelectionWindow MyTeamSelectionWindow;
	public InfoWindow MyInfoWindow;
	public HeroPopup MyShipWindow;
	public MissionWindow MyMissionWindow;
	public ContextButtonsOverlay MyButtonsOverlay;
	public PopUp MyPopUp;
	public ImagesPopUp MyImagesPopUp;
	public HeroCatalog MyShipsCatalogWindow;
	public CurrentTeamShower CurrentTeamShower;
	public AdventureSelectionWindow AdventureSelectionWindow;
	public ChestOpenTooltip ChestOpenTooltip;
	public HealPopUp HealPopUp;
	public ChestsWindow ChestsWindow;

	public GameObject MapNode;

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
	}

	public List<ChestButton> ChestButtons;

	/*public List<GameObject> ShipRewardChestImages;
	public List<GameObject> ShipRewardChestTexts;*/

	public void UpdateShipRewardChests (RewardChest rewardChest) {
		for (int i = 0; i < ChestButtons.Count; i++) {
			if (ChestButtons [i].RewardChest == null) { // TODO
				ChestButtons[i].ReceiveChest(rewardChest);
				break;
			}
		}
		if (rewardChest.ChestState == ChestState.Opening) {
			int index = Player.Instance.RewardChests.IndexOf (rewardChest);
			ChestButtons [index].IsBeingOpenedState ();
			for (int i = 0; i < ChestButtons.Count; i++) {
				if (i != index && ChestButtons[i].RewardChest != null && ChestButtons[i].RewardChest.ChestState != ChestState.Open) {
					ChestButtons [i].AnotherChestIsBeingOpenedState ();
				}
			}
		}
	}

	public void BeginChestOpen (int index) {
		if (Player.Instance.CurrentlyOpeningChest != null && Player.Instance.CurrentlyOpeningChest.ChestState == ChestState.Opening && Player.Instance.CurrentlyOpeningChest == ChestButtons [index].RewardChest) {
			OpenChestOpenTooltip (Player.Instance.CurrentlyOpeningChest);
			return;
		}
		if (Player.Instance.CurrentlyOpeningChest != null && Player.Instance.CurrentlyOpeningChest.ChestState == ChestState.Opening && Player.Instance.CurrentlyOpeningChest != ChestButtons [index].RewardChest) {
			// OpenChestOpenTooltip (ChestButtons [index].RewardChest);
			return;
		}
		if (Player.Instance.CurrentlyOpeningChest != null && Player.Instance.CurrentlyOpeningChest.ChestState == ChestState.Open && Player.Instance.CurrentlyOpeningChest == ChestButtons [index].RewardChest) {
			Player.Instance.ReceiveChestReward (ChestButtons [index].RewardChest);
			ChestButtons [index].EmptyState ();
			ChestButtons [index].RewardChest = null;
			Player.Instance.CurrentlyOpeningChest = null;
			return;
		}
		ChestButtons [index].IsBeingOpenedState ();
		for (int i = 0; i < ChestButtons.Count; i++) {
			if (i != index && ChestButtons[i].RewardChest != null && ChestButtons[i].RewardChest.ChestState != ChestState.Open) {
				ChestButtons [i].AnotherChestIsBeingOpenedState ();
			}
		}
		Player.Instance.BeginChestOpen (ChestButtons [index].RewardChest);
	}

	public void FinishChestOpen (RewardChest rewardChest) {
		if (ChestOpenTooltip.Window.activeSelf) {
			ChestOpenTooltip.CloseLockpicks ();
			ChestOpenTooltip.Close ();
		}
		foreach (var chestButton in ChestButtons) {
			if (chestButton.RewardChest == rewardChest) {
				chestButton.ReadyToOpenState ();
			} else if (chestButton.RewardChest != null && chestButton.RewardChest.ChestState != ChestState.Open) {
				chestButton.TouchToOpenState ();
			}
		}
	}

	public GameObject FlyingRewardPrefab;

	public float JourneyTime;
	float startTime;

	Vector3 startPosition;
	Vector3 targetPosition;
	bool flyReward;
	GameObject flyingReward;

	public void FlyReward (Sprite sprite, Transform initialPosition, GameObject destinationNode) {
		flyingReward = Instantiate (FlyingRewardPrefab) as GameObject;
		flyingReward.transform.position = initialPosition.position;
		flyingReward.GetComponentInChildren<SpriteRenderer> ().sprite = sprite;

		flyingReward.GetComponentInChildren<SpriteRenderer> ().transform.localScale *= 0.15f; // kostyll

		startTime = Time.time;
		startPosition = flyingReward.transform.position;
		targetPosition = destinationNode.transform.position;
		flyReward = true;
	}

	public void OpenChestNow (RewardChest rewardChest) {
		ChestOpenTooltip.Close ();

		rewardChest.Open ();
	}

	void Start () {
		player = Player.Instance;
		if (player.OnAdventure) {
			foreach (var hideObject in HideInAdventureObjects) {
				hideObject.SetActive (false);
			}
			foreach (var hideObject in HideOffAdventureObjects) {
				hideObject.SetActive (true);
			}
			if (Player.Instance.CurrentAdventure.TreasureHunt) {
				MapNode.SetActive (true);
			} else {
				MapNode.SetActive (false);
			}	
		} else {
			MapNode.SetActive (false);
			foreach (var hideObject in HideInAdventureObjects) {
				hideObject.SetActive (true);
			}
			foreach (var hideObject in HideOffAdventureObjects) {
				hideObject.SetActive (false);
			}
		}
		CurrentTeamShower.Open ();
	}

	void Update () { // OMG
		GoldLabel.text = "" + player.Gold;
		GemsLabel.text = "" + player.Inventory ["Gems"];
		EnergyLabel.text = "" + player.Energy + "/" + player.MaxEnergy;
		ChestsLabel.text = "" + player.RewardChests.Count + "/" + 4; // PlayerShip.Instance.RewardChestsCapacity; // player.Inventory ["Key"];
		ExpLabel.text = "" + player.Inventory ["Exp"] + "/" + player.ExpForLevel [player.Level];
		LevelLabel.text = "" + player.Level;
		ExpSlider.maxValue = player.ExpForLevel [player.Level];
		ExpSlider.value = player.Inventory ["Exp"];
		if (player.OnAdventure) {			
			int time = (int)player.AdventureTimer;
			int hours = time / 3600;
			int minutes = (time - hours * 3600) / 60;
			int seconds = time - hours * 3600 - minutes * 60;
			TimeLabel.text = hours + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
			if (player.CurrentAdventure.TreasureHunt) {
				MapNode.GetComponentInChildren<Text> ().text = player.Inventory ["Map"] + "/" + player.CurrentAdventure.MapsForTreasure;
			}
		}

		if (flyReward) {
			float dTime = (Time.time - startTime) / JourneyTime;			
			flyingReward.transform.position = Vector3.Lerp(startPosition, targetPosition, dTime);
			if (Vector3.Distance (flyingReward.transform.position, targetPosition) < 0.01f) {
				flyReward = false;
				Destroy (flyingReward);				
			}
		}

		if (player.CurrentlyOpeningChest != null) {
			SecondaryChestTimer.text = Utility.SecondsToTime (player.CurrentlyOpeningChest.SecondsLeft);
			if (player.CurrentlyOpeningChest.SecondsLeft <= 0.0f) {
				ChestsActivityMarker.SetActive (true);
			} else {
				ChestsActivityMarker.SetActive (false);
			}
		} else if (player.RewardChests.Count > 0) {
			ChestsActivityMarker.SetActive (true);
		}
	}

	public GameObject PreviousAdventureButtonObject;
	public GameObject NextAdventureButtonObject;
	public Image AdventureImage;
	public List<Sprite> AdventureSprites;

	public void ChangeAdventure (bool next) {
		Player.Instance.ChangeAdventure (next);
		if (Player.Instance.Adventures.IndexOf(Player.Instance.CurrentAdventure) == 0) {
			PreviousAdventureButtonObject.SetActive (false);
			if (Player.Instance.Adventures.Count > 1) {
				NextAdventureButtonObject.SetActive (true);
			}
		} else if (Player.Instance.Adventures.IndexOf(Player.Instance.CurrentAdventure) == Player.Instance.Adventures.Count - 1) {
			NextAdventureButtonObject.SetActive (false);
			if (Player.Instance.Adventures.Count > 1) {
				PreviousAdventureButtonObject.SetActive (true);
			}
		} else {
			if (Player.Instance.Adventures.Count > 1) {
				NextAdventureButtonObject.SetActive (true);
				PreviousAdventureButtonObject.SetActive (true);
			}
		}
		AdventureImage.sprite = AdventureSprites [Player.Instance.Adventures.IndexOf (Player.Instance.CurrentAdventure)];
	}

	public void OpenChestOpenTooltip (RewardChest rewardChest) {
		ChestOpenTooltip.Open (rewardChest);
	}

	public void OpenAdventureSelectionWindow () {
		AdventureSelectionWindow.Open ();
		MyInfoWindow.Close ();
		// MyShipWindow.Close ();
		MyMissionWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyPopUp.Close ();
		MyShipsCatalogWindow.Close ();
	}

	public void OpenChestsWindow () {
		ChestsWindow.Open ();
	}

	public void OpenHealPopUp (CreatureData selectedHero) {
		HealPopUp.Open (selectedHero);
	}

	public void CloseAdventureSelectionWindow () {
		if (AdventureSelectionWindow != null) {
			AdventureSelectionWindow.Close ();
		}
	}

	public void OpenSelectableInfo (Selectable selectable) {
		MyInfoWindow.Open (selectable);
		// MyShipWindow.Close ();
		MyMissionWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyPopUp.Close ();
		MyShipsCatalogWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void OpenShipWindow (CreatureData shipData) {
		MyShipWindow.Open (shipData);
		MyMissionWindow.Close ();
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void OpenPopUp (string message) {
		MyPopUp.Open (message);
	}

	public void OpenImagesPopUp (string message, Dictionary<string, int> itemNames) {
		MyImagesPopUp.Open (message, itemNames);
	}

	public void OpenMissionWindow (Mission chosenMission) {
		MyMissionWindow.Open (chosenMission);
		MyButtonsOverlay.Close ();
		// MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void CloseMissionWindow () {
		MyMissionWindow.Close ();
	}

	public void OpenTeamSelectionWindow (Mission mission) {
		MyTeamSelectionWindow.Open (mission);
		MyButtonsOverlay.Close ();
		MyMissionWindow.Close ();
		// MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void OpenContextButtons (Selectable selectable) {
		if (InMoveMode) {
			return;
		}
		CloseContextButtons (true);
		Selection = selectable;
		Selection.Animate ();
		MyButtonsOverlay.Open (selectable);
		MyMissionWindow.Close ();
		// MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void OpenShipsCatalogWindow () {
		MyShipsCatalogWindow.Open ();
		MyButtonsOverlay.Close ();
		// MyMissionWindow.Close ();
		// MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		CloseAdventureSelectionWindow ();
	}

	public void UpdateHeroCatalogWindow () {
		MyShipsCatalogWindow.UpdateLabels ();
	}

	public void CloseContextButtons (bool deselect) {
		MyButtonsOverlay.Overlay.SetActive (false); // kostyll
		if (deselect && Selection != null) {
			Selection.Deanimate ();
		}
	}

	public void CloseAllWindows () {
		MyMissionWindow.Reload ();
		MyButtonsOverlay.Close ();
		// MyShipWindow.Close ();
		MyPopUp.Close ();
		MyInfoWindow.Close ();
		MyShipsCatalogWindow.Close ();
		CloseAdventureSelectionWindow ();
	}
}
