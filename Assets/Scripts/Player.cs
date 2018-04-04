using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public List<Adventure> Adventures;

	public Adventure CurrentAdventure;

	public int MaxEnergy;
	public int Energy;

	public List<Mission> Missions;
	public Mission CurrentMission;
	public List<CreatureData> ShipDatas;
	public bool FirstLoad = true;
	public static Player Instance;

	public DataBase DataBase;
	public BJDataBase BJDataBase;
	public Dictionary<string, int> Inventory;

	public int Gold { get { return Inventory ["Gold"]; } }

	public List<CreatureData> CurrentTeam;

	public Vector3 PlayerShipCoordinates;

	public Dictionary<string, bool> Tiles;
	public Dictionary<string, POIData> POIDataByTiles;
	public List<POIData> POIDatas;

	public bool NewBoard;
	public bool OnAdventure;
	public float AdventureTimer;

	public List<RewardChest> PlayerShipRewardChests;
	public List<RewardChest> RewardChests;
	public RewardChest CurrentlyOpeningChest;

	public bool ReceivedReward;

	void Awake () {
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);  
		}
		DontDestroyOnLoad(gameObject);
		// CurrentTeam = new List<CreatureData> ();
		Inventory = new Dictionary<string, int> ();
		Tiles = new Dictionary<string, bool> ();
		POIDataByTiles = new Dictionary<string, POIData> ();
		POIDatas = new List<POIData> ();
		RewardChests = new List<RewardChest> ();
		PlayerShipRewardChests = new List<RewardChest> ();
	}

	void Start () {
		CurrentAdventure = Adventures [0];
		//Inventory.Add(
	}

	float timer = 0.0f;
	int previousTimer = 0;

	void Update () {
		if (OnAdventure) {
			AdventureTimer -= Time.deltaTime;
			if (AdventureTimer <= 0) {
				LoadVillage ();
				UIOverlay.Instance.OpenPopUp ("Adventure time is over!");
			}
		}
		timer += Time.deltaTime;
		if (CurrentlyOpeningChest != null && CurrentlyOpeningChest.ChestState != ChestState.Open) {
			if (timer > previousTimer + 1) {
				previousTimer++;
				CurrentlyOpeningChest.TickOpen (1);
			}
		}
	}

	public void SavePlayerShip (PlayerShip playerShip) {
		PlayerShipCoordinates = playerShip.gameObject.transform.position;
	}

	public void CreateShipDatas () {
		// CurrentTeam = new List<CreatureData> (5);
		ShipDatas.Clear ();

		List<BJCreature> creatures = BJDataBase.Creatures;
		for (int j = 0; j < creatures.Count; j++) {
			bool summoned = (j > 0 && j < 6) ? true : false;

			List<Skill> skills = new List<Skill> ();
			foreach (var skillName in creatures[j].SkillNames) {
				
				skills.Add (BJDataBase.SkillsByNames [skillName]);
			}
			Sprite soulstoneSprite = null;
			if (Player.Instance.BJDataBase.CreaturePortraitsByNames.ContainsKey (creatures [j].Name)) {
				soulstoneSprite = Player.Instance.BJDataBase.CreaturePortraitsByNames [creatures [j].Name];
			}

			Item soulstone = new Item ((creatures [j].Name + " soulstone"), soulstoneSprite);
			if (!DataBase.ItemIconsByNames.ContainsKey (soulstone.Name)) {
				DataBase.ItemIconsByNames.Add (soulstone.Name, soulstoneSprite);
			}
			if (!DataBase.TempItemLibrary.Contains (soulstone)) {
				DataBase.TempItemLibrary.Add (soulstone);
				DataBase.ItemsByNames.Add (soulstone.Name, soulstone);
				Inventory.Add (soulstone.Name, 0);
			}

			CreatureData newShipData = new CreatureData (creatures [j], 1,
				skills, soulstone, RankColor.White, summoned);

			ShipDatas.Add (newShipData);
		}
	}

	public void LoadBattle (int sceneIndex) {
		// PlayerShipRewardChests = new List<RewardChest> (GameManager.Instance.PlayerShip.RewardChests);
		SceneManager.LoadScene (sceneIndex);
	}

	public void LoadVillage () {
		Invoke ("ReloadChests", 0.1f);
		for (int i = CurrentTeam.Count - 1; i >= 0; i--) {
			if (CurrentTeam [i].IsDead) {
				CurrentTeam.Remove (CurrentTeam [i]);
			}
		}
		Player.Instance.Inventory ["Map"] = 0;
		OnAdventure = false;
		SceneManager.LoadScene (0);
		// Invoke ("ReceiveAdventureReward", 0.5f);
		CurrentAdventure = Adventures [0];
	}

	public void ReceiveAdventureReward () {
		Dictionary<string, int> totalReward = new Dictionary<string, int> ();
		foreach (var rewardChest in RewardChests) {
			foreach (var rewardItem in rewardChest.RewardItems) {
				if (!totalReward.ContainsKey(rewardItem.Key)) {
					totalReward.Add (rewardItem.Key, rewardItem.Value);
				} else {
					totalReward [rewardItem.Key] += rewardItem.Value;
				}
			}
		}

		Player.Instance.TakeItems (totalReward);
		UIOverlay.Instance.OpenImagesPopUp ("Your reward:", totalReward);

		RewardChests.Clear ();
		PlayerShipRewardChests.Clear ();
	}

	public void BeginChestOpen (RewardChest rewardChest) {
		timer = 0.0f;
		previousTimer = 0;
		rewardChest.StartOpen ();
		CurrentlyOpeningChest = rewardChest;
	}

	public void OpenChest (RewardChest rewardChest) {
		UIOverlay.Instance.FinishChestOpen (rewardChest);
	}

	public void ReceiveChestReward (RewardChest rewardChest) {
		Player.Instance.TakeItems (rewardChest.RewardItems);
		UIOverlay.Instance.OpenImagesPopUp ("Your reward:", rewardChest.RewardItems);
		Player.Instance.RewardChests.Remove (rewardChest);
	}

	public void LoadAdventure () {
		if (!OnAdventure) {
			OnAdventure = true;
			AdventureTimer = CurrentAdventure.TimeLimit;
			Missions.Clear ();
			Tiles.Clear ();
			POIDataByTiles.Clear ();
			POIDatas.Clear ();
		} else {
			ReceivedReward = false;
		}
		SceneManager.LoadScene (2);
		Invoke ("ReloadChests", 0.1f);
	}

	public void ReloadChests () {
		foreach (var chest in RewardChests) {
			UIOverlay.Instance.UpdateShipRewardChests (chest);
		}
	}

	public void ChangeAdventure (bool next) {
		if (next) {
			if (Adventures.IndexOf (CurrentAdventure) + 1 < Adventures.Count) {
				CurrentAdventure = Adventures [Adventures.IndexOf (CurrentAdventure) + 1];
			} else {
				CurrentAdventure = Adventures [0];
			}
		} else {
			if (Adventures.IndexOf (CurrentAdventure) - 1 >= 0) {
				CurrentAdventure = Adventures [Adventures.IndexOf (CurrentAdventure) - 1];
			} else {
				CurrentAdventure = Adventures [Adventures.Count - 1];
			}
		}
	}

	public void TakeGold (int amount) {
		Inventory ["Gold"] += amount;
		// Gold += amount;
	}

	public void GiveGold (int amount) {
		if (Gold >= amount) {
			Inventory ["Gold"] -= amount;
		} else {
			UIOverlay.Instance.OpenPopUp ("Not enough gold");
		}
	}

	public void GiveItems (Dictionary<string, int> amountsByItemNames) {
		// probably should check here
		foreach (var amountByItemName in amountsByItemNames) {
			if (!Inventory.ContainsKey(amountByItemName.Key)) {
				Inventory.Add (amountByItemName.Key, 0);
			}
			Inventory [amountByItemName.Key] -= amountByItemName.Value;
		}
	}

	public void TakeItems (Dictionary<string, int> amountsByItemNames) {
		foreach (var amountByItemName in amountsByItemNames) {
			if (!Inventory.ContainsKey(amountByItemName.Key)) {
				Inventory.Add (amountByItemName.Key, 0);
			}
			Inventory [amountByItemName.Key] += amountByItemName.Value;
		}
	}

	public bool CheckCost (Dictionary<string, int> amountsByItemNames) {
		foreach (var amountByItemName in amountsByItemNames) {
			if (!Inventory.ContainsKey(amountByItemName.Key)) {
				Inventory.Add (amountByItemName.Key, 0);
			}
			if (Inventory[amountByItemName.Key] < amountByItemName.Value) {
				return false;
			}
		}
		return true;
	}
}
