using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HorrorState : BaseSheepState
{
    private readonly Transform[] _escapePointsOnHorror;

    private readonly string _playerTag;


    public HorrorState(IStationStateSwitcher stationStateSwitcher, Transform sheepTransform, float speed,
        Transform playerTransform, Transform[] escapePointsOnHorror, NavMeshAgent navMeshAgent,
        Vector2 minMaxDistanceState)
        : base(sheepTransform, playerTransform, minMaxDistanceState, stationStateSwitcher, speed, navMeshAgent)
    {
        _escapePointsOnHorror = escapePointsOnHorror;
        _playerTag = playerTransform.tag;
    }


    public override void Update()
    {
        if (!AgentReachedToThePoint())
        {
            CheckingForAPlayer();
            return;
        }

        var dist = Vector3.Distance(playerTransform.position, sheepTransform.position);
        if (!IsTheDistanceRight(dist))
        {
            if(dist > minMaxDistanceState.y)
            {
                stationStateSwitcher.SwitchState<EscapeState>();
                return;
            }
        }

        SetDestination();
    }

    private void CheckingForAPlayer()
    {
        if (!Physics.Linecast(sheepTransform.position, navMeshAgent.destination, out var hit)) return;

        if (hit.transform.CompareTag(_playerTag))
            SetDestination();
    }

    protected override void SetDestination()
    {
        Vector3 randomDestination;
        var attemptCount = 0;
        do
        {
            randomDestination = GetRandomDestination();

            if (++attemptCount != ATTEMPT_LIMIT) continue;
            Debug.LogError($"Sheep has tried {ATTEMPT_LIMIT} times without success to find its way in CalmState mode");
            break;
        } while (!CanWalkTo(randomDestination));

        navMeshAgent.SetDestination(randomDestination);
    }

    private Vector3 GetRandomDestination()
    {
        var nearestPoints = GetNearestPoints();
        for (var i = 1; i < nearestPoints.Count; i++)
            if (Physics.Linecast(sheepTransform.position, nearestPoints[i].escapePointOnHorror.position, out var hit))
            {
                if (!hit.transform.CompareTag(_playerTag))
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
        var nearestPointsList = (from escapePointOnHorror in _escapePointsOnHorror
            let distance = Vector3.Distance(escapePointOnHorror.position, sheepTransform.position)
            select (distance, escapePointOnHorror)).ToList();

        for (var j = 1; j < nearestPointsList.Count; j++)
        for (var i = 1; i < nearestPointsList.Count; i++)
            if (nearestPointsList[i - 1].distance > nearestPointsList[i].distance)
                (nearestPointsList[i - 1], nearestPointsList[i]) = (nearestPointsList[i], nearestPointsList[i - 1]);

        return nearestPointsList;
    }
}