using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class SheepMover : MonoBehaviour
{
    public Transform player;

    [Header("Speed")] 
    [SerializeField] private float calmSpeed = 0.5f;
    [SerializeField] private float escapeSpeed = 4;
    [SerializeField] private float horrorSpeed = 6;

    [Header("Distance")] 
    [SerializeField] private float horrorDistance = 10;
    [SerializeField] private float alertDistance = 15;
    [SerializeField] private float calmDistance = 20;
    [SerializeField] private float calmDistanceWalk = 4;

    [Header("Time in seconds")] 
    [SerializeField] private float changeDestinationSecondsOnCalm = 1;
    [SerializeField] private float changeDestinationSecondsOnEscape = 0.5f;
    [SerializeField] private float changeDestinationSecondsOnHorror = 2;

    [Header("Other")] 
    [SerializeField] private Transform[] escapePointsOnHorror;
    [SerializeField] private string playerTag = "Player";


    private Vector3 _gizmosDestinationPosition;
    private NavMeshAgent _navMeshAgent;
    private SheepState _sheepState;


    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _sheepState = SheepState.Calm;
        StartCoroutine(SlowFixedUpdate());
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, _gizmosDestinationPosition);
    }

    private IEnumerator SlowFixedUpdate()
    {
        while (true)
        {
            float waitSeconds = -1;
            switch (_sheepState)
            {
                case SheepState.Calm:
                    waitSeconds = changeDestinationSecondsOnCalm;
                    break;
                case SheepState.Escape:
                    waitSeconds = changeDestinationSecondsOnEscape;
                    break;
                case SheepState.Horror:
                    waitSeconds = changeDestinationSecondsOnHorror;
                    break;
                default:
                    Debug.LogException(new Exception($"{_sheepState} not found"));
                    break;
            }

            yield return new WaitForSeconds(waitSeconds);

            var distanceBetweenPlayerAndSheep = GetDistance(player.position, transform.position);

            if (distanceBetweenPlayerAndSheep < horrorDistance) _sheepState = SheepState.Horror;
            else if (distanceBetweenPlayerAndSheep < alertDistance) _sheepState = SheepState.Escape;
            else if (distanceBetweenPlayerAndSheep > calmDistance) _sheepState = SheepState.Calm;

            switch (_sheepState)
            {
                case SheepState.Escape:
                    OnSheepEscape();
                    break;
                case SheepState.Horror:
                    OnSheepHorror();
                    break;
                default:
                    OnSheepCalm();
                    break;
            }
        }
    }


    private void OnSheepHorror()
    {
        _navMeshAgent.speed = horrorSpeed;
        _navMeshAgent.SetDestination(GetRandomDestinationOnHorror());
    }

    private Vector3 GetRandomDestinationOnHorror()
    {
        var randomDestination = SheepFrightedRandomDestination();
        _gizmosDestinationPosition = randomDestination;
        return randomDestination;
    }


    private void OnSheepCalm()
    {
        _navMeshAgent.speed = calmSpeed;
        _navMeshAgent.SetDestination(GetRandomDestinationOnCalm());
    }

    private void OnSheepEscape()
    {
        _navMeshAgent.speed = escapeSpeed;
        _navMeshAgent.SetDestination(GetRandomDestinationOnEscape());
    }

    private Vector3 GetRandomDestinationOnEscape()
    {
        var position = transform.position;
        // there is no point in checking the hit of the raycast because it will hit the wall anyway
        Physics.Raycast(position, position - player.position, out var hit);
        var randomDestination = hit.point;

        _gizmosDestinationPosition = randomDestination;
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

        _gizmosDestinationPosition = randomDestination;
        return randomDestination;
    }

    private static float GetDistance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    private Vector3 SheepFrightedRandomDestination()
    {
        var nearestPoints = GetNearestPoints();
        for (var i = 1; i < nearestPoints.Count; i++)
            if (Physics.Linecast(transform.position, nearestPoints[i].escapePointOnHorror.position, out var hit))
            {
                if (!hit.transform.CompareTag(playerTag))
                    return nearestPoints[i].escapePointOnHorror.position;
            }
            else
            {
                return nearestPoints[i].escapePointOnHorror.position;
            }

        return nearestPoints[1].escapePointOnHorror.position;
    }

    private List<(float distance, Transform escapePointOnHorror)> GetNearestPoints()
    {
        var nearestPointsList = (from escapePointOnHorror in escapePointsOnHorror
            let distance = GetDistance(escapePointOnHorror.position, transform.position)
            select (distance, escapePointOnHorror)).ToList();

        for (var j = 1; j < nearestPointsList.Count; j++)
        for (var i = 1; i < nearestPointsList.Count; i++)
            if (nearestPointsList[i - 1].distance > nearestPointsList[i].distance)
                (nearestPointsList[i - 1], nearestPointsList[i]) = (nearestPointsList[i], nearestPointsList[i - 1]);

        return nearestPointsList;
    }
}