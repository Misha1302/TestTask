using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EscapeState : BaseSheepState
{
    private readonly BaseSheepState[] _allSheepStates;
    private readonly Transform _playerTransform;
    private readonly Transform _sheepTransform;
    private readonly float _sleepTime;
    private readonly float _speed;


    public EscapeState(IStationStateSwitcher stationStateSwitcher, Transform sheepTransform, float speed,
        float sleepTime,
        Transform playerTransform, NavMeshAgent navMeshAgent, Vector2 minMaxDistanceState,
        BaseSheepState[] allSheepStates)
        : base(stationStateSwitcher, navMeshAgent, minMaxDistanceState, speed, sleepTime, allSheepStates)
    {
        this.navMeshAgent = navMeshAgent;
        this.stationStateSwitcher = stationStateSwitcher;

        this.minMaxDistanceState = minMaxDistanceState;

        _playerTransform = playerTransform;
        _sheepTransform = sheepTransform;
        _speed = speed;
        _sleepTime = sleepTime;
        _allSheepStates = allSheepStates;
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
                var horrorState = _allSheepStates.FirstOrDefault(state => state is HorrorState);
                if (dist >= horrorState.minMaxDistanceState.x && dist <= horrorState.minMaxDistanceState.y)
                {
                    horrorState.StartState();
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
        var position = _sheepTransform.position;
        // there is no point in checking the hit of the raycast because it will hit the wall anyway
        Physics.Raycast(position, position - _playerTransform.position, out var hit);
        var randomDestination = hit.point;

        navMeshAgent.SetDestination(randomDestination);
    }
}