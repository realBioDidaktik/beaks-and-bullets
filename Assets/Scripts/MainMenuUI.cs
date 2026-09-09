using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;
using System;
using System.Collections.Generic;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
	Animator anim;

	int selectedLevel = 0;
	int selectedGun = 0;

	public GameObject btLvlUp, btLvlDown, btGunUp, btGunDown;
	//public LocalizeStringEvent levelNameDisplay, gunNameDisplay;
	public TMP_Text levelNameDisplay, gunNameDisplay;
	public RawImage levelPanel, gunPanel;

	[Header("For PlayerPrefs")]
	public Toggle girlyMode;
	public Toggle hardMode;
	public Slider spawnSpeed, despawnTime;

	[Header("Market")]
	// public int maxLevelID, maxGunID;
	public List<String> levelNameIDs;
	public List<Texture> levelPictures;
	public List<int> levelCost;

	public List<String> gunNameIDs;
	public List<Texture> gunPictures;
	public List<int> gunCost;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
			StartCoroutine(LoadPlayerPrefs());
	    anim = GetComponent<Animator>();
			LoadCurrentGun();
			UpdateButtons();
    }

		// Loads the PlayerPrefs and updates the Settings UI menu
		IEnumerator LoadPlayerPrefs() {
			if (PlayerPrefs.HasKey("GirlyMode")) {
				girlyMode.isOn = PlayerPrefs.GetInt("GirlyMode") != 0;
			}
			if (PlayerPrefs.HasKey("HardMode")) {
				hardMode.isOn = PlayerPrefs.GetInt("HardMode") != 0;
			}
			if (PlayerPrefs.HasKey("SpawnSpeed")) {
				spawnSpeed.value = PlayerPrefs.GetFloat("SpawnSpeed");
			}
			if (PlayerPrefs.HasKey("BirdRemovalTime")) {
				despawnTime.value = PlayerPrefs.GetFloat("BirdRemovalTime");
			}
			yield return null;
		}

    // Update is called once per frame
    void Update()
    {

    }

	// Switch animated to a different view eg Main -> Settings
	public void SwitchTo(string where) {
		anim.SetTrigger(where);
	}

	// Select Gun or Level
	public void SwitchLvl(int upDown) {
		selectedLevel += upDown;
		UpdateButtons();
	}

	public void SwitchGun(int upDown) {
		selectedGun += upDown;
		UpdateButtons();
	}

	// Make up/down buttons invisible when the player has reached either end of the list and visible if not
	public void UpdateButtons() {
		if (selectedLevel == 0)
			btLvlDown.SetActive(false);
		else
			btLvlDown.SetActive(true);
		if (selectedLevel == levelNameIDs.Count - 1)
			btLvlUp.SetActive(false);
		else
			btLvlUp.SetActive(true);

		if (selectedGun == 0)
			btGunDown.SetActive(false);
		else
			btGunDown.SetActive(true);
		if (selectedGun == gunNameIDs.Count - 1)
			btGunUp.SetActive(false);
		else
			btGunUp.SetActive(true);

		//levelNameDisplay.StringReference.TableEntryReference = levelNameIDs[selectedLevel];
		//levelNameDisplay.RefreshString();
		levelNameDisplay.text = levelNameIDs[selectedLevel];

		levelPanel.texture = levelPictures[selectedLevel];


		//gunNameDisplay.StringReference.TableEntryReference = gunNameIDs[selectedGun];
		//gunNameDisplay.RefreshString();

		//gunPanel.texture = gunPictures[selectedGun];
	}

	// restore what the player has previously selected
	void LoadCurrentGun() {
		if (PlayerPrefs.HasKey("CurrentLevel"))
			selectedLevel = PlayerPrefs.GetInt("CurrentLevel");
		else
			PlayerPrefs.SetInt("CurrentLevel", 0);

		if (PlayerPrefs.HasKey("CurrentGun"))
			selectedGun = PlayerPrefs.GetInt("CurrentGun");
		else
			PlayerPrefs.SetInt("CurrentGun", 0);
	}

	// Load Tutorail Level if selected
		public void LoadTutorial() {
			SceneManager.LoadScene(1);
		}

	// Select Level or Gun from List. Guns and Buying not implemented yet
	public void PlayOrBuyLevel() {
		anim.SetTrigger("Main");
		SafeCurrentSelection();
		SceneManager.LoadScene(selectedLevel+3);
	}

	public void SelectOrBuyGun() {
		SafeCurrentSelection();
		SwitchTo("Main");
	}

	// Safe the Selection to the PlayerPrefs
	void SafeCurrentSelection() {
		PlayerPrefs.SetInt("CurrentGun", selectedGun);
		PlayerPrefs.SetInt("CurrentLevel", selectedLevel);
	}

	// Following 4 functions update PlayerPrefs when toggles and sliders in Settings UI change
	public void ToggleGirlMode(bool isOn) {
		PlayerPrefs.SetInt("GirlyMode", isOn ? 1 : 0);
	}

	public void ToggleHardMode(bool isOn) {
		PlayerPrefs.SetInt("HardMode", isOn ? 1 : 0);
	}

	public void ChangeSpawnSpeed(float spawnSpeedModifier) {
		PlayerPrefs.SetFloat("SpawnSpeed", spawnSpeedModifier);
	}

	public void ChangeDespawnTime(float despawnTime) {
		PlayerPrefs.SetFloat("BirdRemovalTime", despawnTime);
	}
}
