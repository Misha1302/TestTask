using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSheepState
{
    private protected const int ATTEMPT_LIMIT = 10;
    private readonly Transform _sheepTransform;
    private protected NavMeshAgent navMeshAgent;

    private readonly NavMeshPath _path = new();

    private protected float speed;


    private protected IStationStateSwitcher stationStateSwitcher;

    private protected BaseSheepState(Transform sheepTransform, Vector2 minMaxDistanceState)
    {
        _sheepTransform = sheepTransform;
        _minMaxDistanceState = minMaxDistanceState;
    }

    private readonly Vector2 _minMaxDistanceState;


    public void Start()
    {
        navMeshAgent.isStopped = false;
        SetDestination();
        SetSpeed();
    }

    public abstract void Update();

    public abstract void SetAllSheepStates(BaseSheepState[] baseSheepStates);

    public bool IsTheDistanceRight(float playerDistance)
    {
        return playerDistance >= _minMaxDistanceState.x && playerDistance <= _minMaxDistanceState.y;
    }

    public void StopState()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.SetDestination(_sheepTransform.position);
    }

    private protected bool AgentReachedThePoint()
    {
        return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    private protected abstract void SetDestination();

    private void SetSpeed()
    {
        navMeshAgent.speed = speed;
    }

    private protected bool CanWalkTo(Vector3 destination)
    {
        NavMesh.CalculatePath(_sheepTransform.position, destination, NavMesh.AllAreas, _path);
        return _path.status == NavMeshPathStatus.PathComplete;
    }
}