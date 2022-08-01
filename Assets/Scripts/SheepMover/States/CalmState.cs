using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CalmState : BaseSheepState
{
    private readonly float _distanceWalk;
    private readonly Transform _playerTransform;
    private readonly Transform _sheepTransform;
    private readonly float _speed;
    private BaseSheepState[] _allSheepStates;

    public CalmState(IStationStateSwitcher stationStateSwitcher, float distanceWalk, float speed,
        Transform sheepTransform,
        Transform playerTransform, NavMeshAgent navMeshAgent, Vector2 minMaxDistanceState)
        : base(stationStateSwitcher, navMeshAgent, minMaxDistanceState, speed)
    {
        this.navMeshAgent = navMeshAgent;
        this.stationStateSwitcher = stationStateSwitcher;

        this.minMaxDistanceState = minMaxDistanceState;

        _playerTransform = playerTransform;
        _distanceWalk = distanceWalk;
        _sheepTransform = sheepTransform;
        _speed = speed;
    }


    public sealed override Vector2 minMaxDistanceState { get; }
    private protected sealed override IStationStateSwitcher stationStateSwitcher { get; set; }
    private protected sealed override NavMeshAgent navMeshAgent { get; set; }


    public sealed override void Go()
    {
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) return;
        
        navMeshAgent.isStopped = false;
        var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
        if (dist < minMaxDistanceState.x || dist > minMaxDistanceState.y)
        {
            StopState();
            var escapeState = _allSheepStates.FirstOrDefault(state => state is EscapeState);
            if (dist >= escapeState.minMaxDistanceState.x && dist <= escapeState.minMaxDistanceState.y)
            {
                stationStateSwitcher.SwitchState(escapeState);
                return;
            }

            var horrorState = _allSheepStates.FirstOrDefault(state => state is HorrorState);
            if (dist >= horrorState.minMaxDistanceState.x && dist <= horrorState.minMaxDistanceState.y)
            {
                stationStateSwitcher.SwitchState(horrorState);
                return;
            }
        }

        navMeshAgent.isStopped = false;
        SetDestination();
        SetSpeed();
    }

    public sealed override void SetAllSheepStates(BaseSheepState[] baseSheepStates)
    {
        _allSheepStates = baseSheepStates;
    }

    private protected sealed override void SetSpeed()
    {
        navMeshAgent.speed = _speed;
    }

    public sealed override void StopState()
    {
        navMeshAgent.isStopped = true;
    }

    private protected sealed override void SetDestination()
    {
        var position = _sheepTransform.position;
        var randomDestination = new Vector3
        {
            x = Random.Range(position.x - _distanceWalk, position.x + _distanceWalk),
            y = 0.75f,
            z = Random.Range(position.z - _distanceWalk, position.z + _distanceWalk)
        };

        navMeshAgent.SetDestination(randomDestination);
    }
}