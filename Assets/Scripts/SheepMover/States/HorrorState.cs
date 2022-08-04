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
    private BaseSheepState[] _allSheepStates;
    private BaseSheepState _calmState;
    private BaseSheepState _escapeState;


    public HorrorState(IStationStateSwitcher stationStateSwitcher, Transform sheepTransform, float speed,
        Transform playerTransform, Transform[] escapePointsOnHorror, NavMeshAgent navMeshAgent,
        Vector2 minMaxDistanceState)
        : base(sheepTransform, minMaxDistanceState)
    {
        this.speed = speed;

        _playerTransform = playerTransform;
        _sheepTransform = sheepTransform;
        _escapePointsOnHorror = escapePointsOnHorror;
        this.navMeshAgent = navMeshAgent;
        this.stationStateSwitcher = stationStateSwitcher;

        _playerTag = _playerTransform.tag;
    }


    public override void Update()
    {
        if (!AgentReachedThePoint())
        {
            CheckingForAPlayer();
            return;
        }

        var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
        if (!IsTheDistanceRight(dist))
        {
            if (_escapeState.IsTheDistanceRight(dist))
            {
                stationStateSwitcher.SwitchState(_escapeState);
                return;
            }

            if (_calmState.IsTheDistanceRight(dist))
            {
                stationStateSwitcher.SwitchState(_calmState);
                return;
            }
        }

        SetDestination();
    }

    private void CheckingForAPlayer()
    {
        if (!Physics.Linecast(_sheepTransform.position, navMeshAgent.destination, out var hit)) return;
        
        if (hit.transform.CompareTag(_playerTag))
            SetDestination();
    }

    public override void SetAllSheepStates(BaseSheepState[] baseSheepStates)
    {
        _allSheepStates = baseSheepStates;
        _calmState = _allSheepStates.First(state => state is CalmState);
        _escapeState = _allSheepStates.First(state => state is EscapeState);
    }

    private protected override void SetDestination()
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