using UnityEngine;
using UnityEngine.AI;

public class EscapeState : BaseSheepState
{
    public EscapeState(Transform sheepTransform, Transform playerTransform, (float minDistance, float maxDistance) minMaxDistanceState,
        IStationStateSwitcher stationStateSwitcher, float speed, NavMeshAgent navMeshAgent)
        : base(sheepTransform, playerTransform, minMaxDistanceState, stationStateSwitcher, speed, navMeshAgent)
    {
    }


    public override void Update()
    {
        var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
        if (dist < _minMaxDistanceState.minDistance)
        {
            _stationStateSwitcher.SwitchState<HorrorState>();
            return;
        }

        if (dist > _minMaxDistanceState.maxDistance)
        {
            _stationStateSwitcher.SwitchState<CalmState>();
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
            var direction = position - _playerTransform.position;
            direction.y += Random.Range(-5, 6);
            Physics.Raycast(position, direction, out var hit);
            randomDestination = hit.point;

            if (++attemptCount != ATTEMPT_LIMIT) continue;
            Debug.LogError($"Sheep has tried {ATTEMPT_LIMIT} times without success to find its way in CalmState mode");
            break;
        } while (!CanWalkTo(randomDestination));

        return randomDestination;
    }
}