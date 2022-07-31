using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSheepState
{
    protected BaseSheepState(IStationStateSwitcher stationStateSwitcher, NavMeshAgent navMeshAgent,
        Vector2 minMaxDistanceState)
    {
        this.stationStateSwitcher = stationStateSwitcher;
        this.navMeshAgent = navMeshAgent;
        this.minMaxDistanceState = minMaxDistanceState;
    }

    private protected abstract IStationStateSwitcher stationStateSwitcher { get; set; }
    private protected abstract NavMeshAgent navMeshAgent { get; set; }
    private protected abstract Vector2 minMaxDistanceState { get; set; }


    private protected abstract void SetDestination();
    public abstract void SetState();
}