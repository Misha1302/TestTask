using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CalmState : BaseSheepState
{
    private readonly BaseSheepState[] _allSheepStates;
    private readonly float _distanceWalk;
    private readonly Transform _playerTransform;
    private readonly Transform _sheepTransform;
    private readonly float _speed;
    private readonly float _sleepTime;

    public CalmState(IStationStateSwitcher stationStateSwitcher, float distanceWalk, float speed, float sleepTime,
        Transform sheepTransform,
        Transform playerTransform, NavMeshAgent navMeshAgent, Vector2 minMaxDistanceState,
        BaseSheepState[] allSheepStates)
        : base(stationStateSwitcher, navMeshAgent, minMaxDistanceState, speed, sleepTime, allSheepStates)
    {
        this.navMeshAgent = navMeshAgent;
        this.stationStateSwitcher = stationStateSwitcher;

        this.minMaxDistanceState = minMaxDistanceState;

        _playerTransform = playerTransform;
        _distanceWalk = distanceWalk;
        _sheepTransform = sheepTransform;
        _speed = speed;
        _sleepTime = sleepTime;
        _allSheepStates = allSheepStates;
    }


    public sealed override Vector2 minMaxDistanceState { get; }
    private protected sealed override IStationStateSwitcher stationStateSwitcher { get; set; }
    private protected sealed override NavMeshAgent navMeshAgent { get; set; }


    public override void StartState()
    {
        StartCoroutine(StartStateCoroutine());
    }

    private IEnumerator StartStateCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
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

                var horrorState = _allSheepStates.FirstOrDefault(state => state is HorrorState);
                if (dist >= horrorState.minMaxDistanceState.x && dist <= horrorState.minMaxDistanceState.y)
                    horrorState.StartState();
                yield break;
            }

            navMeshAgent.isStopped = false;
            SetDestination();
            SetSpeed();

            yield return new WaitForSeconds(_sleepTime);
        }
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