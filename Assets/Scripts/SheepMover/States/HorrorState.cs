using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HorrorState : BaseSheepState
{
    private readonly Transform[] _escapePointsOnHorror;

    private readonly string _playerTag;
    private readonly Transform _playerTransform;
    private readonly Transform _sheepTransform;
    private readonly float _speed;
    private BaseSheepState[] _allSheepStates;


    public HorrorState(IStationStateSwitcher stationStateSwitcher, Transform sheepTransform, float speed,
        Transform playerTransform, Transform[] escapePointsOnHorror, NavMeshAgent navMeshAgent,
        Vector2 minMaxDistanceState)
        : base(stationStateSwitcher, navMeshAgent, minMaxDistanceState, speed)
    {
        this.navMeshAgent = navMeshAgent;
        this.stationStateSwitcher = stationStateSwitcher;

        this.minMaxDistanceState = minMaxDistanceState;

        _playerTransform = playerTransform;
        _sheepTransform = sheepTransform;
        _escapePointsOnHorror = escapePointsOnHorror;
        _speed = speed;

        _playerTag = _playerTransform.tag;
    }


    private protected sealed override IStationStateSwitcher stationStateSwitcher { get; set; }
    private protected sealed override NavMeshAgent navMeshAgent { get; set; }
    public sealed override Vector2 minMaxDistanceState { get; }


    public sealed override void Go()
    {
        if (!AgentReachedThePoint()) return;
        
        navMeshAgent.isStopped = false;
        var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
        if (!IsTheDistanceRight(dist))
        {
            var escapeState = _allSheepStates.First(state => state is EscapeState);
            if (escapeState.IsTheDistanceRight(dist))
            {
                stationStateSwitcher.SwitchState(escapeState);
                return;
            }

            var calmState = _allSheepStates.First(state => state is CalmState);
            if (calmState.IsTheDistanceRight(dist))
            {
                stationStateSwitcher.SwitchState(calmState);
                return;
            }
        }

        navMeshAgent.isStopped = false;
        SetDestination();
        SetSpeed();
    }

    public sealed override void StopState()
    {
        navMeshAgent.isStopped = true;
    }

    private protected sealed override void SetSpeed()
    {
        navMeshAgent.speed = _speed;
    }

    public sealed override void SetAllSheepStates(BaseSheepState[] baseSheepStates)
    {
        _allSheepStates = baseSheepStates;
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