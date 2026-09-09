using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;

public class Player : MonoBehaviour
{
	PlayerControls controls;

	Transform gunContainer;

	Spawner spawner;

	List<string> allBirdIDs = new List<string>();

	string currentBird = "";

	LocalizeStringEvent assNameDisplay;
	TMP_Text nameDisplay;

	int healthPoints = 3;
	int bulletsLeft = 12;
	bool gameOver = false;

	int selectedGun = 0;

	bool isGirly;

	//public List<GameObject> hpUI;

	Animator gameOverScreen;
	List<LocalizeStringEvent> gameOverText = new List<LocalizeStringEvent>();
	List<TMP_Text> gameOverTextNoLoc = new List<TMP_Text>();

	List<TMP_Text> statisticsUI = new List<TMP_Text>();

	// statistics and scoring
	float shotsFired = 0;
	float kills = 0;
	int score = 0;


	float startTime = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
		startTime = Time.time;

		gunContainer = transform.Find("Guns");

		if (PlayerPrefs.HasKey("GirlyMode"))
			isGirly	= PlayerPrefs.GetInt("GirlyMode") != 0;

		if (PlayerPrefs.HasKey("CurrentGun"))
			selectedGun = PlayerPrefs.GetInt("CurrentGun");

		StartCoroutine(InitGunInHand());

		//gameOverScreen.SetActive(false);
		gameOverScreen = GameObject.Find("GameOverPanel").GetComponent<Animator>();

		gameOverText.Add(GameObject.Find("gameOver").GetComponent<LocalizeStringEvent>());
		gameOverText.Add(GameObject.Find("gameOverReason").GetComponent<LocalizeStringEvent>());

		gameOverTextNoLoc.Add(GameObject.Find("gameOver").GetComponent<TMP_Text>());
		gameOverTextNoLoc.Add(GameObject.Find("gameOverReason").GetComponent<TMP_Text>());

		statisticsUI.Add(GameObject.Find("Score").GetComponent<TMP_Text>());
		statisticsUI.Add(GameObject.Find("Accuracy").GetComponent<TMP_Text>());
		statisticsUI.Add(GameObject.Find("Knowledge").GetComponent<TMP_Text>());

    spawner = GameObject.FindWithTag("Spawnpoint").GetComponent<Spawner>();

		foreach (GameObject birdPrefab in spawner.birdPrefabs) {
			allBirdIDs.Add(birdPrefab.GetComponent<Bird>().nameID);
			//Debug.Log(birdPrefab.GetComponent<Bird>().nameID);
		}


		assNameDisplay = GameObject.FindWithTag("NameDisplay").GetComponent<LocalizeStringEvent>();
		nameDisplay = GameObject.FindWithTag("NameDisplay").GetComponent<TMP_Text>();
		//Debug.Log(assNameDisplay);

		currentBird = SelectBird("");
    }

		IEnumerator InitGunInHand() {
			//Debug.Log(gunContainer);
			//Debug.Log((isGirly ? "cam" : "gun") + selectedGun.ToString());
			Transform currentGun = gunContainer.Find((isGirly ? "cam" : "gun") + selectedGun.ToString());
			currentGun.gameObject.SetActive(true);
			yield return null;
		}


	void Awake()
	{
	    controls = new PlayerControls();
	    controls.Enable();
	}

    // Update is called once per frame
    void Update() {
			if (gameOver)
				return;

			if (controls.Player.Attack.WasPressedThisFrame()) {
				StartCoroutine(Shoot());
			}
			//RotateGun();
			//StartCoroutine(RotateGun());
    }

		void FixedUpdate() {
			int layer_mask = LayerMask.GetMask("Backdrop");
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
			Debug.Log(ray);

 			if (Physics.Raycast (ray, out hit, 200, layer_mask)) {
				Debug.Log("hitbackwall");
				gunContainer.LookAt(hit.point);
			}

			//yield return null;
		}

		IEnumerator Shoot() {
			shotsFired++;

			Ray rayOrigin = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
			RaycastHit hitInfo;

			if (Physics.Raycast(rayOrigin, out hitInfo)) {
				//Debug.DrawRay(rayOrigin.origin, rayOrigin.direction * 100f, Color.red);

				GameObject GO = hitInfo.collider.gameObject;
				//Debug.Log(GO.name);

				//Debug.DrawRay(transform.position, transform.forward, Color.green);
				if (GO.tag == "Bird") {
					kills++;

					Bird bird = GO.GetComponent<Bird>();
					if (bird.nameID != currentBird && !gameOver) {
						//Debug.Log("-1 leben");
						healthPoints--;

						//hpUI[healthPoints].SetActive(false);
						GameObject.Find("HealthPoint (" + (2-healthPoints).ToString() + ")").SetActive(false);

						if (healthPoints == 0)
							GameOver("Keine Leben mehr!");
					}
					else {
						score++;
						currentBird = SelectBird(currentBird);
					}
					bird.Die();
				}
				else
					MissedShot();
			}
			else
				MissedShot();

			yield return null;
		}

	string SelectBird(string previous) {
		string newBird = "";
		do {
			newBird = allBirdIDs[UnityEngine.Random.Range(0, allBirdIDs.Count)];
		}
		while (newBird == previous || (newBird == "Kaiserpinguin" && Time.time - startTime < 10));

		//assNameDisplay.StringReference.TableEntryReference = newBird;
		//assNameDisplay.RefreshString();

		nameDisplay.text = newBird;

		return newBird;
	}

	void MissedShot() {
		bulletsLeft--;

		GameObject bullet = GameObject.Find("Bulletto (" + (11-bulletsLeft).ToString() + ")");
		bullet.SetActive(false);

		if (bulletsLeft == 0)
			GameOver("Keine Munition mehr!");
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

		//gameOverText[0].StringReference.TableEntryReference = "gameOver";
		//gameOverText[0].RefreshString();

		//gameOverText[1].StringReference.TableEntryReference = reason;
		//gameOverText[1].RefreshString();

		gameOverTextNoLoc[1].text = reason;

		//gameOverScreen.SetActive(true);
		gameOverScreen.SetTrigger("FlyIn");
	}

	public void SafeScore() {
		int oldScore = 0;
		if (PlayerPrefs.HasKey("Coins"))
			oldScore = PlayerPrefs.GetInt("Coins");

		PlayerPrefs.SetInt("Coins", oldScore + score);
	}
}
