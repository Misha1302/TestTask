using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSheepState
{
    protected BaseSheepState(
        IStationStateSwitcher stationStateSwitcher,
        NavMeshAgent navMeshAgent,
        Vector2 minMaxDistanceState,
        float speed)
    {
    }

    public abstract Vector2 minMaxDistanceState { get; }

    private protected abstract IStationStateSwitcher stationStateSwitcher { get; set; }
    private protected abstract NavMeshAgent navMeshAgent { get; set; }


    public abstract void Go();
    public abstract void StopState();
    public abstract void SetAllSheepStates(BaseSheepState[] baseSheepStates);

    public bool IsTheDistanceRight(float playerDistance)
    {
        return playerDistance >= minMaxDistanceState.x && playerDistance <= minMaxDistanceState.y;
    }

    private protected bool AgentReachedThePoint()
    {
        return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    private protected abstract void SetSpeed();
    private protected abstract void SetDestination();
}