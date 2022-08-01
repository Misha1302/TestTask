using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HorrorState : BaseSheepState
{
    private readonly BaseSheepState[] _allSheepStates;
    private readonly Transform[] _escapePointsOnHorror;

    private readonly string _playerTag;
    private readonly Transform _playerTransform;
    private readonly Transform _sheepTransform;
    private readonly float _sleepTime;
    private readonly float _speed;


    public HorrorState(IStationStateSwitcher stationStateSwitcher, Transform sheepTransform, float speed,
        float sleepTime,
        Transform playerTransform, Transform[] escapePointsOnHorror, NavMeshAgent navMeshAgent,
        Vector2 minMaxDistanceState, BaseSheepState[] allSheepStates)
        : base(stationStateSwitcher, navMeshAgent, minMaxDistanceState, speed, sleepTime, allSheepStates)
    {
        this.navMeshAgent = navMeshAgent;
        this.stationStateSwitcher = stationStateSwitcher;

        this.minMaxDistanceState = minMaxDistanceState;

        _playerTransform = playerTransform;
        _sheepTransform = sheepTransform;
        _escapePointsOnHorror = escapePointsOnHorror;
        _speed = speed;
        _sleepTime = sleepTime;
        _allSheepStates = allSheepStates;

        _playerTag = _playerTransform.tag;
    }


    private protected sealed override IStationStateSwitcher stationStateSwitcher { get; set; }
    private protected sealed override NavMeshAgent navMeshAgent { get; set; }
    public sealed override Vector2 minMaxDistanceState { get; }


    public sealed override void StartState()
    {
        StartCoroutine(StartStateCoroutine());
    }

    private IEnumerator StartStateCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            navMeshAgent.isStopped = false;
            var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
            if (dist < minMaxDistanceState.x || dist > minMaxDistanceState.y)
            {
                StopState();
                var escapeState = _allSheepStates.FirstOrDefault(state => state is EscapeState);
                if (dist >= escapeState.minMaxDistanceState.x && dist <= escapeState.minMaxDistanceState.y)
                {
                    escapeState.StartState();
                    yield break;
                }

                var calmState = _allSheepStates.FirstOrDefault(state => state is CalmState);
                if (dist >= calmState.minMaxDistanceState.x && dist <= calmState.minMaxDistanceState.y)
                    calmState.StartState();
                yield break;
            }

            navMeshAgent.isStopped = false;
            SetDestination();
            SetSpeed();
        }
    }

    public sealed override void StopState()
    {
        navMeshAgent.isStopped = true;
    }

    private protected sealed override void SetSpeed()
    {
        navMeshAgent.speed = _speed;
    }

    private protected sealed override void SetDestination()
    {
        navMeshAgent.SetDestination(GetRandomDestination());
    }

    private Vector3 GetRandomDestination()
    {
        var nearestPoints = GetNearestPoints();
        for (var i = 1; i < nearestPoints.Count; i++)
            if (Physics.Linecast(_sheepTransform.position, nearestPoints[i].escapePointOnHorror.position, out var hit))
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
            let distance = Vector3.Distance(escapePointOnHorror.position, _sheepTransform.position)
            select (distance, escapePointOnHorror)).ToList();

        for (var j = 1; j < nearestPointsList.Count; j++)
        for (var i = 1; i < nearestPointsList.Count; i++)
            if (nearestPointsList[i - 1].distance > nearestPointsList[i].distance)
                (nearestPointsList[i - 1], nearestPointsList[i]) = (nearestPointsList[i], nearestPointsList[i - 1]);

        return nearestPointsList;
    }
}