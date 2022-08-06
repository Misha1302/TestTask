using UnityEngine;
using UnityEngine.AI;

public class CalmState : BaseSheepState
{
    private readonly float _distanceWalk;

    public CalmState(Transform sheepTransform, Transform playerTransform, (float minDistance, float maxDistance) minMaxDistanceState,
        IStationStateSwitcher stationStateSwitcher, float speed, NavMeshAgent navMeshAgent, float distanceWalk)
        : base(sheepTransform, playerTransform, minMaxDistanceState, stationStateSwitcher, speed, navMeshAgent)
    {
        _distanceWalk = distanceWalk;
    }


    public override void Update()
    {
        var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
        if (dist < _minMaxDistanceState.minDistance)
        {
            _stationStateSwitcher.SwitchState<EscapeState>();
            return;
        }

        if (!AgentReachedToThePoint()) return;

        SetDestination();
    }

    protected override Vector3 GetDestination()
    {
        Vector3 randomDestination;
        var attemptCount = 0;
        do
        {
            var position = _sheepTransform.position;

            randomDestination = new Vector3
            {
                x = Random.Range(position.x - _distanceWalk, position.x + _distanceWalk),
                y = 0.75f,
                z = Random.Range(position.z - _distanceWalk, position.z + _distanceWalk)
            };

            if (++attemptCount != ATTEMPT_LIMIT) continue;
            Debug.LogError($"Sheep has tried {ATTEMPT_LIMIT} times without success to find its way in CalmState mode");
            break;
        } while (!CanWalkTo(randomDestination));

        return randomDestination;
    }
}