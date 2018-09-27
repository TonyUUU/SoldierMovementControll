using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    public Transform[] patrolPoints;

    int patrolPointIndex;
    float proximityBeforeChangeDestination = 0.5f;
    NavMeshAgent navMeshAgent;
	private bool displayText = false;
    void Start()
	{
		navMeshAgent = GetComponent<NavMeshAgent> ();
		navMeshAgent.autoBraking = false;
		navMeshAgent.updatePosition = true;
		navMeshAgent.updateRotation = true;
		SetNextDestination ();
	}

    void Update()
    {

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < proximityBeforeChangeDestination)
        {
            SetNextDestination();
        }

		DetectPlayer (transform.position, 200);
    }

    void SetNextDestination()
    {
        navMeshAgent.SetDestination(patrolPoints[patrolPointIndex].position);
        patrolPointIndex = (patrolPointIndex + 1) % patrolPoints.Length;
    }

    void OnCollisionEnter(Collision c)
    {
        var other = c.gameObject;
        if (other.CompareTag("Player"))
        {
			displayText = true;
			navMeshAgent.isStopped = true;
			other.SendMessage ("gotHit", 100);
        }
    }

	void OnGUI() {
		if (displayText)
		GUI.Label(new Rect(200, 200, 100, 100), "You are DESTROYED");
	}

	void DetectPlayer(Vector3 center, float distance)
	{
		
		Vector3 fwd = transform.TransformDirection(Vector3.forward);
		Debug.DrawRay(transform.position, fwd * distance, Color.red, 0.001f);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, fwd, out hit, distance)) {
			if (hit.collider.CompareTag ("Player")) {
				navMeshAgent.isStopped = true;
				transform.LookAt (hit.collider.transform);
				if (Random.value >= 0.96f) {
					hit.collider.SendMessage ("gotHit", 10);
				}
			} else {
				navMeshAgent.isStopped = false;
			}
		} else {
			navMeshAgent.isStopped = false;
		}
	}

}
