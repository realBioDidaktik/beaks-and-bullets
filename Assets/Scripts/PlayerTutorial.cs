using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;

public class PlayerTutorial : MonoBehaviour
{
	PlayerControls controls;
	
	Spawner spawner;
	
	public GameObject birdNameDisplay;
	
	public List<GameObject> tutorialStepPanels;
	
	
	public List<GameObject> tutorialBirds;
	public List<GameObject> tutorialSpawnPoints;
	
	List<string> allBirdIDs = new List<string>();
	
	string currentBird = "Weißkopfseeadler";
	
	LocalizeStringEvent assNameDisplay;
	
	int healthPoints = 3;
	int bulletsLeft = 12;
	bool gameOver = false;
	
	int tutorialStep = 0;
	
	//public List<GameObject> hpUI;
	
	Animator gameOverScreen;
	public List<LocalizeStringEvent> gameOverText;
	
	public List<TMP_Text> statisticsUI;
	
	// statistics and scoring
	float shotsFired = 0;
	float kills = 0;
	int score = 0;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		birdNameDisplay.SetActive(false);
		
		for (int i = 1; i < tutorialStepPanels.Count; i++)
			tutorialStepPanels[i].SetActive(false);
		
		//gameOverScreen.SetActive(false);
		gameOverScreen = GameObject.Find("GameOverPanel").GetComponent<Animator>();
		
        spawner = GameObject.FindWithTag("Spawnpoint").GetComponent<Spawner>();
		foreach (GameObject birdPrefab in spawner.birdPrefabs) {
			allBirdIDs.Add(birdPrefab.GetComponent<Bird>().nameID);
			Debug.Log(birdPrefab.GetComponent<Bird>().nameID);
		}
		
		assNameDisplay = GameObject.FindWithTag("NameDisplay").GetComponent<LocalizeStringEvent>();
		Debug.Log(assNameDisplay);
		
		
		//currentBird = SelectBird("");
    }

	void Awake()
	{
	    controls = new PlayerControls();
	    controls.Enable();
	}
	
	public void AdvanceTutorial() {
		tutorialStep++;

		tutorialStepPanels[tutorialStep - 1].SetActive(false);
		tutorialStepPanels[tutorialStep].SetActive(true);
		
		if (tutorialStep == 1)
			SpawnFirstBourne();
		
		if (tutorialStep == 2) {
			birdNameDisplay.SetActive(true);
		}
		
		if (tutorialStep == 3) {
			spawner.isSpawning = true;
		
			spawner.Invoke("SpawnBird", 0.0f);
		}
		
		if (tutorialStep == 4)
			spawner.isSpawning = false;
	}
	
	void SpawnFirstBourne () {
		if (tutorialStep != 1)
			return;
		
		Instantiate(tutorialBirds[0], tutorialSpawnPoints[0].transform.position, Quaternion.identity);
		
		Invoke("SpawnFirstBourne", 6.7f);
	}

    // Update is called once per frame
    void Update()
    {
		if (gameOver)
			return;
		
		if (controls.Player.Attack.WasPressedThisFrame()) {
			shotsFired++;
			
			Ray rayOrigin = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
			RaycastHit hitInfo;
			
			if (Physics.Raycast(rayOrigin, out hitInfo)) {
				Debug.DrawRay(rayOrigin.origin, rayOrigin.direction * 100f, Color.red);
				
				GameObject GO = hitInfo.collider.gameObject;
				Debug.Log(GO.name);
				
				Debug.DrawRay(transform.position, transform.forward, Color.green);
				if (GO.tag == "Bird") {			
					kills++;
					
					Bird bird = GO.GetComponent<Bird>();
					bird.Die();
					
					if (bird.nameID != currentBird && !gameOver) {
						//Debug.Log("-1 leben");
						
						// healthPoints--;
						
						//hpUI[healthPoints].SetActive(false);
						//GameObject.Find("HealthPoint (" + (2-healthPoints).ToString() + ")").SetActive(false);
						
						if (healthPoints == 0)
							GameOver("hp");
					}
					else {
						score++;
						//currentBird = SelectBird(currentBird);
						
						if (tutorialStep == 3)
							AdvanceTutorial();
					}
					
					if (tutorialStep == 1)
						AdvanceTutorial();
				}
				else 
					MissedShot();
			}
			else
				MissedShot();
		}
    }
	
	string SelectBird(string previous) {
		string newBird = "";
		do {
			newBird = allBirdIDs[UnityEngine.Random.Range(0, allBirdIDs.Count)];
		}
		while (newBird == previous);
		
		assNameDisplay.StringReference.TableEntryReference = newBird;
		assNameDisplay.RefreshString();
		
		return newBird;
	}
	
	void MissedShot() {
		//bulletsLeft--;
		
		//GameObject bullet = GameObject.Find("Bulletto (" + (11-bulletsLeft).ToString() + ")");
		//bullet.SetActive(false);
		
		if (bulletsLeft == 0)
			GameOver("ammo");
	}
	
	void GameOver(string reason) {
		gameOver = true;
		GameObject[] remainingBirds = GameObject.FindGameObjectsWithTag("Bird");
		
		float accuracy = kills/shotsFired;
		float knowledge = score/kills;
		
		statisticsUI[0].text = score.ToString();
		statisticsUI[1].text = String.Format("{0:P1}", accuracy);
		statisticsUI[2].text = String.Format("{0:P1}", knowledge);
		
		spawner.isSpawning = false;
		
		foreach (GameObject bird in remainingBirds) {
			bird.GetComponent<Bird>().Die();
		}
		
		gameOverText[0].StringReference.TableEntryReference = "gameOver";
		gameOverText[0].RefreshString();
		
		gameOverText[1].StringReference.TableEntryReference = reason;
		gameOverText[1].RefreshString();
		
		//gameOverScreen.SetActive(true);
		gameOverScreen.SetTrigger("FlyIn");
	}
	
	public void Retry() {
		gameOverScreen.SetTrigger("FlyOut");
		SafeScore();
		Invoke("ReloadCurrentScene",1.0f);
	}
	
	public void QuitToMain() {
		gameOverScreen.SetTrigger("FlyOut");
		SafeScore();
		Invoke("LoadMainMenuScene",1.0f);
	}
	
	void ReloadCurrentScene() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
	
	void LoadMainMenuScene() {
		SceneManager.LoadScene(0);
	}
	
	void SafeScore() {
		int oldScore = 0;
		if (PlayerPrefs.HasKey("Coins"))
			oldScore = PlayerPrefs.GetInt("Coins");
		
		PlayerPrefs.SetInt("Coins", oldScore + score);
	}
}
