using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSheepState
{
    protected const int ATTEMPT_LIMIT = 10;

    protected readonly Vector2 minMaxDistanceState;

    private readonly NavMeshPath _path = new();
    protected readonly Transform playerTransform;
    protected readonly Transform sheepTransform;

    protected readonly IStationStateSwitcher stationStateSwitcher;
    protected readonly NavMeshAgent navMeshAgent;

    protected float speed;

    protected BaseSheepState(Transform sheepTransform, Transform playerTransform, Vector2 minMaxDistanceState,
        IStationStateSwitcher stationStateSwitcher, float speed, NavMeshAgent navMeshAgent)
    {
        this.sheepTransform = sheepTransform;
        this.stationStateSwitcher = stationStateSwitcher;
        this.speed = speed;
        this.navMeshAgent = navMeshAgent;
        this.playerTransform = playerTransform;
        this.minMaxDistanceState = minMaxDistanceState;
    }


    public void Start()
    {
        navMeshAgent.isStopped = false;
        SetDestination();
        SetSpeed();
    }

    public abstract void Update();

    protected bool IsTheDistanceRight(float playerDistance)
    {
        return playerDistance >= minMaxDistanceState.x && playerDistance <= minMaxDistanceState.y;
    }

    public void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.SetDestination(sheepTransform.position);
    }

    protected bool AgentReachedToThePoint()
    {
        return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    protected abstract void SetDestination();

    private void SetSpeed()
    {
        navMeshAgent.speed = speed;
    }

    protected bool CanWalkTo(Vector3 destination)
    {
        NavMesh.CalculatePath(sheepTransform.position, destination, NavMesh.AllAreas, _path);
        return _path.status == NavMeshPathStatus.PathComplete;
    }
}