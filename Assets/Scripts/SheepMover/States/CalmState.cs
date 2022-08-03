using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CalmState : BaseSheepState
{
    private readonly float _distanceWalk;
    private readonly Transform _playerTransform;
    private readonly Transform _sheepTransform;
    private BaseSheepState[] _allSheepStates;
    private BaseSheepState _escapeState;
    private BaseSheepState _horrorState;

    public CalmState(IStationStateSwitcher stationStateSwitcher, float distanceWalk, float speed,
        Transform sheepTransform,
        Transform playerTransform, NavMeshAgent navMeshAgent, Vector2 minMaxDistanceState)
        : base(sheepTransform, minMaxDistanceState)
    {
        this.speed = speed;

        _playerTransform = playerTransform;
        _distanceWalk = distanceWalk;
        _sheepTransform = sheepTransform;
        this.navMeshAgent = navMeshAgent;
        this.stationStateSwitcher = stationStateSwitcher;
    }


    public sealed override void Update()
    {
        if (!AgentReachedThePoint()) return;

        var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
        if (!IsTheDistanceRight(dist))
        {
            if (_escapeState.IsTheDistanceRight(dist))
            {
                stationStateSwitcher.SwitchState(_escapeState);
                return;
            }

            if (_horrorState.IsTheDistanceRight(dist))
            {
                stationStateSwitcher.SwitchState(_horrorState);
                return;
            }
        }

        SetDestination();
    }

    public sealed override void SetAllSheepStates(BaseSheepState[] baseSheepStates)
    {
        _allSheepStates = baseSheepStates;
        _escapeState = _allSheepStates.First(state => state is EscapeState);
        _horrorState = _allSheepStates.First(state => state is HorrorState);
    }

    private protected sealed override void SetDestination()
    {
        var position = _sheepTransform.position;
        Vector3 randomDestination;
        var attemptCount = 0;
        do
        {
            randomDestination = new Vector3
            {
                x = Random.Range(position.x - _distanceWalk, position.x + _distanceWalk),
                y = 0.75f,
                z = Random.Range(position.z - _distanceWalk, position.z + _distanceWalk)
            };
            
            if (++attemptCount != ATTEMPT_LIMIT) continue;
            Debug.LogError($"Sheep has tried {ATTEMPT_LIMIT} times without success to find its way in CalmState mode");
            break;
        } while (!CanWalkTo(randomDestination));


        navMeshAgent.SetDestination(randomDestination);
    }
}