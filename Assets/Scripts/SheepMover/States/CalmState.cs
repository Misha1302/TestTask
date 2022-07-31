using UnityEngine;
using UnityEngine.AI;

public class CalmState : BaseSheepState
{
    private readonly float _distanceWalk;
    private readonly Transform _playerTransform;
    private readonly Transform _sheepTransform;


    public CalmState(IStationStateSwitcher stationStateSwitcher, float distanceWalk, Transform sheepTransform,
        Transform playerTransform, NavMeshAgent navMeshAgent, Vector2 minMaxDistanceState)
        : base(stationStateSwitcher, navMeshAgent, minMaxDistanceState)
    {
        this.navMeshAgent = navMeshAgent;
        this.stationStateSwitcher = stationStateSwitcher;
        this.minMaxDistanceState = minMaxDistanceState;
        _playerTransform = playerTransform;
        _distanceWalk = distanceWalk;
        _sheepTransform = sheepTransform;
    }


    private protected sealed override IStationStateSwitcher stationStateSwitcher { get; set; }
    private protected sealed override NavMeshAgent navMeshAgent { get; set; }
    private protected sealed override Vector2 minMaxDistanceState { get; set; }


    public override void SetState()
    {
        var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
        if (!(dist > minMaxDistanceState.x) || !(dist < minMaxDistanceState.y)) return;
        
        stationStateSwitcher.SetState(SheepState.Calm);
        SetDestination();
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

        // _gizmosDestinationPosition = randomDestination;
        navMeshAgent.SetDestination(randomDestination);
    }
}