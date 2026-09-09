using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public class Pingu : Bird
{
  NavMeshAgent agent;
  Action whenGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start() {
      // Pingu might be spawn in the air, wait for ground contact in coroutine to enable agent
      whenGrounded = ActivateAgent;

			//flightSpeed = UnityEngine.Random.Range(flightSpeedMin, flightSpeedMax);
			//SetRandomWalk();

			gameObject.tag = "Bird";



			//rigidbody = gameObject.GetComponent<Rigidbody>();

			if (PlayerPrefs.HasKey("BirdRemovalTime"))
				removalTime = PlayerPrefs.GetFloat("BirdRemovalTime");

			isGirly	= PlayerPrefs.GetInt("GirlyMode") != 0;
    }

    void ActivateAgent() {
      //Debug.Log("AgentActivated");
      agent = GetComponent<NavMeshAgent>();
      agent.enabled = true;
      Invoke("SetTarget", 1.0f);
    }

    void SetTarget() {
      agent.SetDestination(new Vector3(UnityEngine.Random.Range(30, 70), 7.5f, UnityEngine.Random.Range(5, 35)));
    }

    // Update is called once per frame
    public override void Update() {
      if (dead && isGirly) {
  			DeadAndGirly();
  			return;
  		}
      
		  // only execute the following code, if the bird is alive
  		if (dead)
  			return;

  		//transform.Translate(Vector3.right * flightSpeed * Time.deltaTime);
  		//transform.Rotate(Time.deltaTime * randomWalk * Vector3.up);

  		if (transform.position.x < 0 || transform.position.x > 100 || transform.position.y < 0)
  			Remove();
      }

      public void OnCollisionEnter(Collision coll) {
        Action callback = whenGrounded;
        whenGrounded = null;
        callback?.Invoke();
      }

      public override bool Die() {
        agent.enabled = false;
        return base.Die();
      }
}
