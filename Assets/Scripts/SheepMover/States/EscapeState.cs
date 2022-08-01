using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EscapeState : BaseSheepState
{
    private readonly Transform _playerTransform;
    private readonly Transform _sheepTransform;
    private readonly float _speed;
    private BaseSheepState[] _allSheepStates;


    public EscapeState(IStationStateSwitcher stationStateSwitcher, Transform sheepTransform, float speed,
        Transform playerTransform, NavMeshAgent navMeshAgent, Vector2 minMaxDistanceState)
        : base(stationStateSwitcher, navMeshAgent, minMaxDistanceState, speed)
    {
        this.navMeshAgent = navMeshAgent;
        this.stationStateSwitcher = stationStateSwitcher;

        this.minMaxDistanceState = minMaxDistanceState;

        _playerTransform = playerTransform;
        _sheepTransform = sheepTransform;
        _speed = speed;
    }


    private protected sealed override IStationStateSwitcher stationStateSwitcher { get; set; }
    private protected sealed override NavMeshAgent navMeshAgent { get; set; }
    public sealed override Vector2 minMaxDistanceState { get; }


    public sealed override void Go()
    {
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) return;
        
        navMeshAgent.isStopped = false;
        var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
        if (dist < minMaxDistanceState.x || dist > minMaxDistanceState.y)
        {
            StopState();
            var horrorState = _allSheepStates.FirstOrDefault(state => state is HorrorState);
            if (dist >= horrorState.minMaxDistanceState.x && dist <= horrorState.minMaxDistanceState.y)
            {
                stationStateSwitcher.SwitchState(horrorState);
                return;
            }

            var calmState = _allSheepStates.FirstOrDefault(state => state is CalmState);
            if (dist >= calmState.minMaxDistanceState.x && dist <= calmState.minMaxDistanceState.y)
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
        var position = _sheepTransform.position;
        // there is no point in checking the hit of the raycast because it will hit the wall anyway
        var direction = position - _playerTransform.position;
        Physics.Raycast(position, direction, out var hit);

        if (Vector3.Distance(position, hit.point) < 5)
        {
            direction.x += 90;
            Physics.Raycast(position, direction, out hit);
            if (!hit.transform.CompareTag("Player"))
            {
                navMeshAgent.SetDestination(hit.point);
            }
            else
            {
                direction.x -= 180;
                Physics.Raycast(position, direction, out hit);
                navMeshAgent.SetDestination(hit.point);
            }

            return;
        }


        navMeshAgent.SetDestination(hit.point);
    }
}