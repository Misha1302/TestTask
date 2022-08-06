using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSheepState
{
    protected const int ATTEMPT_LIMIT = 10;

    private readonly NavMeshPath _path = new();
    private readonly float _speed;

    protected readonly Vector2 minMaxDistanceState;
    protected readonly NavMeshAgent navMeshAgent;
    protected readonly Transform playerTransform;
    protected readonly Transform sheepTransform;

    protected readonly IStationStateSwitcher stationStateSwitcher;

    protected BaseSheepState(Transform sheepTransform, Transform playerTransform, Vector2 minMaxDistanceState,
        IStationStateSwitcher stationStateSwitcher, float speed, NavMeshAgent navMeshAgent)
    {
        this.sheepTransform = sheepTransform;
        this.stationStateSwitcher = stationStateSwitcher;
        this.navMeshAgent = navMeshAgent;
        this.playerTransform = playerTransform;
        this.minMaxDistanceState = minMaxDistanceState;

        _speed = speed;
    }


    public void Start()
    {
        navMeshAgent.isStopped = false;
        SetDestination();
        SetSpeed();
    }

    public abstract void Update();

    public void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.SetDestination(sheepTransform.position);
    }

    protected bool AgentReachedToThePoint()
    {
        return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    protected void SetDestination()
    {
        navMeshAgent.SetDestination(GetDestination());
    }

    protected abstract Vector3 GetDestination();

    private void SetSpeed()
    {
        navMeshAgent.speed = _speed;
    }

    protected bool CanWalkTo(Vector3 destination)
    {
        NavMesh.CalculatePath(sheepTransform.position, destination, NavMesh.AllAreas, _path);
        return _path.status == NavMeshPathStatus.PathComplete;
    }
}