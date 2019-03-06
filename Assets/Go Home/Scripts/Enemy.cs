using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Seek
    }
    public State currentState;
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float stoppingDistance = 1f;
    public float gravityDistance = 2f;
    public Rigidbody rigid;
    private Transform target;
    private NavMeshAgent agent;
    private Transform[] waypoints;
    private int currentIndex = 1;

    // Use this for initialization
    void Start()
    {

        // Get the children from WaypointParent
        waypoints = waypointParent.GetComponentsInChildren<Transform>();

        agent = GetComponent<NavMeshAgent>();


        // Just in case make state Patrol
        currentState = State.Patrol;

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(y)
        //Run Patrol Every Frame
        Patrol();
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Seek:
                Seek();
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //If waypoints is not empty
        if (waypoints.Length > 0)
        {
            //If waypoints is not null AND waypoints is not empty
            if (waypoints != null && waypoints.Length > 0)
            {

                // Get current waypoint
                Transform point = waypoints[currentIndex];
                Gizmos.color = Color.red;
                // Draw line from position to waypoint
                Gizmos.DrawLine(transform.position, point.position);
                //Draw gravity sphere
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(point.position, stoppingDistance);
            }

        }
    }



    void Patrol()
    {
        //1 - Get the current waypoint
        Transform point = waypoints[currentIndex];

        //2 - Get distance from waypoint
        float distance = Vector3.Distance(transform.position, point.position);

        //2.1 If distance is less than gravity distance
        if (distance < gravityDistance)
        {
            // Turn gravity off
            rigid.useGravity = false;
        }
        else //Otherwise
        {
            rigid.useGravity = true;
        }
        //3 - If distance is less than stopping distance
        if (distance < stoppingDistance)
        {
            //      4 - Move to next waypoint
            currentIndex++;
            //      4.1 If currentIndex >= waypoints.Length
            if (currentIndex >= waypoints.Length)

                //4.2 Set currentIndex to 1
                currentIndex = 1;
        }
        //5 - Translate Enemy towards current waypoint
        transform.position = Vector3.MoveTowards(transform.position, point.position, moveSpeed * Time.deltaTime);
        transform.LookAt(point.position);
    }
    void Seek()
    {
        // Get enemy to follow target
        agent.SetDestination(target.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Set target to the thing in our zone
            target = other.transform;
            // Switch state to seek
            currentState = State.Seek;
        }


    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Switch state back to patrol
            currentState = State.Patrol;
        }

    }
}

