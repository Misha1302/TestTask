using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSheepState : MonoBehaviour
{
    protected BaseSheepState(
        IStationStateSwitcher stationStateSwitcher,
        NavMeshAgent navMeshAgent,
        Vector2 minMaxDistanceState,
        float speed,
        float sleepTime,
        BaseSheepState[] allSheepStates)
    {
    }
    
    public abstract Vector2 minMaxDistanceState { get; }

    private protected abstract IStationStateSwitcher stationStateSwitcher { get; set; }
    private protected abstract NavMeshAgent navMeshAgent { get; set; }


    public abstract void StartState();
    public abstract void StopState();
    
    private protected abstract void SetSpeed();
    private protected abstract void SetDestination();
}