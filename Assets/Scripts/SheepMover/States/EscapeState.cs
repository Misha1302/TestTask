using UnityEngine;
using UnityEngine.AI;

public class EscapeState : BaseSheepState
{
    private readonly Transform _playerTransform;
    private readonly Transform _sheepTransform;


    public EscapeState(IStationStateSwitcher stationStateSwitcher, Transform sheepTransform,
        Transform playerTransform, NavMeshAgent navMeshAgent, Vector2 minMaxDistanceState)
        : base(stationStateSwitcher, navMeshAgent, minMaxDistanceState)
    {
        this.navMeshAgent = navMeshAgent;
        this.stationStateSwitcher = stationStateSwitcher;
        this.minMaxDistanceState = minMaxDistanceState;
        _playerTransform = playerTransform;
        _sheepTransform = sheepTransform;
    }


    private protected sealed override IStationStateSwitcher stationStateSwitcher { get; set; }
    private protected sealed override NavMeshAgent navMeshAgent { get; set; }
    private protected sealed override Vector2 minMaxDistanceState { get; set; }


    public override void SetState()
    {
        var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
        if (!(dist > minMaxDistanceState.x) || !(dist < minMaxDistanceState.y)) return;
        
        stationStateSwitcher.SetState(SheepState.Escape);
        SetDestination();
    }


    private protected sealed override void SetDestination()
    {
        var position = _sheepTransform.position;
        // there is no point in checking the hit of the raycast because it will hit the wall anyway
        Physics.Raycast(position, position - _playerTransform.position, out var hit);
        var randomDestination = hit.point;

        //_gizmosDestinationPosition = randomDestination;
        navMeshAgent.SetDestination(randomDestination);
    } 
}