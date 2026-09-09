using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
	// stores the birds for this level
	public List<GameObject> birdPrefabs;

	Transform[] spawnPoints;

	public float minSpawnInterval = 1;
	public float maxSpawnInterval = 2;

	private float spawnSpeedModifier = 1;

	// Stop spawning eg when game over
	//[HideInInspector]
	public bool isSpawning = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		spawnPoints = transform.GetComponentsInChildren<Transform>();

        SpawnBird();

				if (PlayerPrefs.HasKey("SpawnSpeed"))
					spawnSpeedModifier = PlayerPrefs.GetFloat("SpawnSpeed");
    }

    // Update is called once per frame
    void Update()
    {

    }

	void SpawnBird() {
		if (!isSpawning)
			return;

		GameObject currentBird = birdPrefabs[Random.Range(0, birdPrefabs.Count)];
		Vector3 currentPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

		GameObject.Instantiate(currentBird, currentPosition, Quaternion.identity);

		Invoke("SpawnBird", Random.Range(minSpawnInterval/spawnSpeedModifier, maxSpawnInterval/spawnSpeedModifier));
	}
}
