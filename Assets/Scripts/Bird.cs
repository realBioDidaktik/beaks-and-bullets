using UnityEngine;
using System.Collections;
using System;

public class Bird : MonoBehaviour
{
	// Necessary for flight direction
	bool spawnedLeft = false;

	[HideInInspector]
	public float targetY = 15;

	// Don't interact with 'dead' directly, use Die() method
	[HideInInspector]
	public bool dead = false;

	// Bird speed and rigidbody push of a wing flap
	[HideInInspector]
	public float flightSpeed = 1;
	public float flightSpeedMin = 1;
	public float flightSpeedMax = 3;

	[HideInInspector]
	public float randomWalk;
	public float randomWalkMax = 10;
	public float flapStrength = 10000;

	public string nameID = "b-proto";

	public float minRelSize = 1;
	public float maxRelSize = 1;
	public bool scaleOnlyY = false;

	[HideInInspector]
	public float removalTime = 10.0f;

	[HideInInspector]
	public bool isGirly = false;

	Vector3 originalPos = Vector3.zero;
	Vector3 originalScale = Vector3.zero;
	float deathTime = 0;
	Transform flashLocation;

	private float moveDuration = 0.5f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public virtual void Start()
	{
		flightSpeed = UnityEngine.Random.Range(flightSpeedMin, flightSpeedMax);
		SetRandomWalk();

		targetY = transform.position.y;
		transform.eulerAngles = Vector3.up * UnityEngine.Random.Range(-15, 15);

		// Check if the bird is spawned left or right
		if (transform.position.x > 50)
			transform.eulerAngles = Vector3.up * UnityEngine.Random.Range(165, 195);
		else if (transform.position.z < 50)
			transform.eulerAngles = Vector3.up * UnityEngine.Random.Range(-105, -75);

		gameObject.tag = "Bird";

		if (PlayerPrefs.HasKey("BirdRemovalTime"))
			removalTime = PlayerPrefs.GetFloat("BirdRemovalTime");

		if (PlayerPrefs.HasKey("GirlyMode"))
			isGirly = PlayerPrefs.GetInt("GirlyMode") != 0;
	}

	public void SetRandomWalk()
	{
		randomWalk = UnityEngine.Random.Range(-randomWalkMax, randomWalkMax);
		Invoke("SetRandomWalk", UnityEngine.Random.Range(10, 30));
	}

	public void DeadAndGirly() {
		float progress = (Time.time - deathTime) / moveDuration;
		progress = Mathf.Clamp01(progress);

		transform.position = Vector3.Lerp(
			originalPos,
			flashLocation.position,
			progress
		);

		transform.localScale = Vector3.Lerp(
					originalScale,
					Vector3.zero,
					progress
			);
	}

	// Update is called once per frame
	public virtual void Update()
	{
		// Move the bird toward the flash location after dying in Girly Mode
		if (dead && isGirly) {
			DeadAndGirly();
			return;
		}

		// Don't control normal flight after the bird has died
		if (dead)
			return;

		transform.Translate(Vector3.right * flightSpeed * Time.deltaTime);
		transform.Rotate(Time.deltaTime * randomWalk * Vector3.up);

		if (transform.position.x < 0 ||
			transform.position.x > 100 ||
			transform.position.z > 150 ||
			transform.position.y < 0)
		{
			Remove();
		}
	}

	// Fixed Update for physics calculation
	void FixedUpdate()
	{
		// Only execute the following code if the bird is alive
		if (dead)
			return;

		if (flapStrength > 0 && transform.position.y < targetY)
		{
			GetComponent<Rigidbody>().AddForce(
				(Vector3.up + (spawnedLeft ? Vector3.right : Vector3.left)) * flapStrength
			);
		}
	}

	// Returns true if the bird was killed, false when it is already dead
	public virtual bool Die()
	{
		bool previouslyDead = dead;
		dead = true;

		gameObject.tag = "DeadBird";

		try
		{
			GetComponent<Animator>().enabled = false;
		}
		catch { }

		try
		{
			GetComponentInChildren<Animator>().enabled = false;
		}
		catch { }

		// In Girly Mode, move the bird to the flash location
		if (isGirly)
		{
			originalScale = transform.localScale;
			originalPos = transform.position;
			deathTime = Time.time;

			int selectedGun = 0;

			if (PlayerPrefs.HasKey("CurrentGun"))
				selectedGun = PlayerPrefs.GetInt("CurrentGun");

			flashLocation = Camera.main.transform
				.Find("Guns")
				.Find((isGirly ? "cam" : "gun") + selectedGun.ToString());

			Invoke("Remove", moveDuration);
		}
		else
		{
			// Apply dead physics
			GetComponent<Rigidbody>().mass = 10;
			GetComponent<Rigidbody>().linearDamping = 1;

			GetComponent<Rigidbody>().AddForce(
				Vector3.forward * UnityEngine.Random.Range(100, 1000)
			);

			GetComponent<Rigidbody>().AddTorque(
				Vector3.right * UnityEngine.Random.Range(13, 67) +
				Vector3.up * UnityEngine.Random.Range(-80, 80)
			);

			foreach (ParticleSystem bloodParticles in
				gameObject.GetComponentsInChildren<ParticleSystem>())
			{
				bloodParticles.transform.rotation = UnityEngine.Random.rotation;
				bloodParticles.Play();
			}

			Invoke("Remove", removalTime);
		}

		// Return the correct kill value
		return !previouslyDead;
	}

	// Removal if bird is outside range or shortly after being killed
	public void Remove()
	{
		Destroy(gameObject);
	}
}
