using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSheepState
{
    protected const int ATTEMPT_LIMIT = 10;

    protected readonly (float minDistance, float maxDistance) _minMaxDistanceState;
    protected readonly NavMeshAgent _navMeshAgent;

    private readonly NavMeshPath _path = new();
    protected readonly Transform _playerTransform;
    protected readonly Transform _sheepTransform;
    private readonly float _speed;

    protected readonly IStationStateSwitcher _stationStateSwitcher;

    protected BaseSheepState(Transform sheepTransform, Transform playerTransform, (float minDistance, float maxDistance) minMaxDistanceState,
        IStationStateSwitcher stationStateSwitcher, float speed, NavMeshAgent navMeshAgent)
    {
        _sheepTransform = sheepTransform;
        _stationStateSwitcher = stationStateSwitcher;
        _navMeshAgent = navMeshAgent;
        _playerTransform = playerTransform;
        _minMaxDistanceState = minMaxDistanceState;

        _speed = speed;
    }


    public void Start()
    {
        _navMeshAgent.isStopped = false;
        SetDestination();
        SetSpeed();
    }

    public abstract void Update();

    public void Stop()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.SetDestination(_sheepTransform.position);
    }

    protected bool AgentReachedToThePoint()
    {
        return _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;
    }

    protected void SetDestination()
    {
        _navMeshAgent.SetDestination(GetDestination());
    }

    protected abstract Vector3 GetDestination();

    private void SetSpeed()
    {
        _navMeshAgent.speed = _speed;
    }

    protected bool CanWalkTo(Vector3 destination)
    {
        NavMesh.CalculatePath(_sheepTransform.position, destination, NavMesh.AllAreas, _path);
        return _path.status == NavMeshPathStatus.PathComplete;
    }
}