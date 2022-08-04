using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EscapeState : BaseSheepState
{
    private readonly string _playerTag;
    private readonly Transform _playerTransform;
    private readonly Transform _sheepTransform;
    private BaseSheepState[] _allSheepStates;
    private BaseSheepState _calmState;
    private BaseSheepState _horrorState;


    public EscapeState(IStationStateSwitcher stationStateSwitcher, Transform sheepTransform, float speed,
        Transform playerTransform, NavMeshAgent navMeshAgent, Vector2 minMaxDistanceState)
        : base(sheepTransform, minMaxDistanceState)
    {
        this.speed = speed;

        _playerTransform = playerTransform;
        _sheepTransform = sheepTransform;
        this.navMeshAgent = navMeshAgent;
        this.stationStateSwitcher = stationStateSwitcher;

        _playerTag = playerTransform.tag;
    }



    public override void Update()
    {
        var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
        if (!IsTheDistanceRight(dist))
        {
            if (_horrorState.IsTheDistanceRight(dist))
            {
                stationStateSwitcher.SwitchState(_horrorState);
                return;
            }

            if (_calmState.IsTheDistanceRight(dist))
            {
                stationStateSwitcher.SwitchState(_calmState);
                return;
            }
        }
        
        if (!AgentReachedThePoint()) return;

        SetDestination();
    }

    public override void SetAllSheepStates(BaseSheepState[] baseSheepStates)
    {
        _allSheepStates = baseSheepStates;
        _calmState = _allSheepStates.First(state => state is CalmState);
        _horrorState = _allSheepStates.First(state => state is HorrorState);
    }

    private protected override void SetDestination()
    {
        var position = _sheepTransform.position;
        var direction = position - _playerTransform.position;

        Vector3 destinationPoint;
        var attemptCount = 0;
        do
        {
            Physics.Raycast(position, direction, out var hit);
            destinationPoint = hit.point;
            
            if (++attemptCount != ATTEMPT_LIMIT) continue;
            Debug.LogError($"Sheep has tried {ATTEMPT_LIMIT} times without success to find its way in CalmState mode");
            break;
        } while (!CanWalkTo(destinationPoint));

        navMeshAgent.SetDestination(destinationPoint);
    }
}