using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class SheepMove : MonoBehaviour
{
    public Transform player;

    [Header("Speed")] [SerializeField] private float calmSpeed = 2;

    [SerializeField] private float escapeSpeed = 12;
    [SerializeField] private float horrorSpeed = 20;

    [Header("Distance")] [SerializeField] private float alertDistance = 2;

    [SerializeField] private float calmDistance = 4;
    [SerializeField] private float calmDistanceWalk = 4;

    [Header("Time in seconds")] [SerializeField]
    private float changeDestinationSecondsOnCalm = 1;

    [SerializeField] private float changeDestinationSecondsOnEscape = 0.5f;
    [SerializeField] private float changeDestinationSecondsOnFright = 2;


    private Rigidbody currentSheepRigidbody;

    private Vector3 gizmosDestinationPosition;

    private NavMeshAgent navMeshAgent;

    private SheepState sheepState;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        sheepState = SheepState.Calm;
        StartCoroutine(SlowFixedUpdate());
        StartCoroutine(CheckPathPerPlayer());
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, gizmosDestinationPosition);
    }

    private IEnumerator SlowFixedUpdate()
    {
        while (true)
        {
            float waitSeconds = -1;
            switch (sheepState)
            {
                case SheepState.Calm:
                    waitSeconds = changeDestinationSecondsOnCalm;
                    break;
                case SheepState.Escape:
                    waitSeconds = changeDestinationSecondsOnEscape;
                    break;
                case SheepState.Fright:
                    waitSeconds = changeDestinationSecondsOnFright;
                    break;
                default:
                    Debug.LogException(new Exception($"{sheepState} not found"));
                    break;
            }

            yield return new WaitForSeconds(waitSeconds);

            var distanceBetweenPlayerAndSheep = GetDistance(player.position, transform.position);

            if (distanceBetweenPlayerAndSheep > calmDistance) sheepState = SheepState.Calm;
            else if (distanceBetweenPlayerAndSheep < alertDistance) sheepState = SheepState.Escape;

            if (sheepState == SheepState.Escape) OnSheepEscape();
            else OnSheepCalm();
        }
    }

    private IEnumerator CheckPathPerPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            if (Physics.Raycast(transform.position, transform.forward, out var hit, 10))
                if (hit.transform.tag.Equals("Player"))
                {
                    var randomDestination = SheepFrightedRandomDestination();
                    gizmosDestinationPosition = randomDestination;
                    navMeshAgent.SetDestination(randomDestination);
                }
        }
    }

    private void OnSheepCalm()
    {
        navMeshAgent.speed = calmSpeed;
        navMeshAgent.SetDestination(GetRandomDestinationOnCalm());
    }

    private void OnSheepEscape()
    {
        navMeshAgent.SetDestination(GetRandomDestinationOnEscape());
        navMeshAgent.speed = sheepState == SheepState.Escape ? escapeSpeed : horrorSpeed;
    }

    private Vector3 GetRandomDestinationOnEscape()
    {
        var position = transform.position;
        // there is no point in checking the hit of the raycast because it will hit the wall anyway
        Physics.Raycast(position, position - player.position, out var hit);
        var randomDestination = hit.point;
        if (GetDistance(randomDestination, transform.position) < 7)
        {
            sheepState = SheepState.Fright;
            gizmosDestinationPosition = randomDestination;
            return SheepFrightedRandomDestination();
        }

        gizmosDestinationPosition = randomDestination;
        return randomDestination;
    }

    private Vector3 GetRandomDestinationOnCalm()
    {
        var position = transform.position;
        var randomDestination = new Vector3
        {
            x = Random.Range(position.x - calmDistanceWalk, position.x + calmDistanceWalk),
            y = 0.75f,
            z = Random.Range(position.z - calmDistanceWalk, position.z + calmDistanceWalk)
        };

        gizmosDestinationPosition = randomDestination;
        return randomDestination;
    }

    private static float GetDistance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    private Vector3 SheepFrightedRandomDestination()
    {
        // try to go right
        var currentTransform = transform;
        Physics.Raycast(currentTransform.position, currentTransform.right, out var hit);
        if (GetDistance(hit.point, currentTransform.position) > 5) return hit.point;

        // try to go left
        Physics.Raycast(currentTransform.position, -currentTransform.right, out hit);
        if (GetDistance(hit.point, currentTransform.position) > 5) return hit.point;
        return hit.point;

        // go to centre
        print("LKJHGFDS");
        var randomDestination = new Vector3
        {
            x = Random.Range(-10f, 10f),
            y = 0.75f,
            z = Random.Range(-10f, 10f)
        };

        gizmosDestinationPosition = randomDestination;
        return randomDestination;
    }
}