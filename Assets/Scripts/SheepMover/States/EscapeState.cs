using UnityEngine;
using UnityEngine.AI;

public class EscapeState : BaseSheepState
{
    public EscapeState(IStationStateSwitcher stationStateSwitcher, Transform sheepTransform, float speed,
        Transform playerTransform, NavMeshAgent navMeshAgent, Vector2 minMaxDistanceState)
        : base(sheepTransform, playerTransform, minMaxDistanceState, stationStateSwitcher, speed, navMeshAgent)
    {
    }


    public override void Update()
    {
        var dist = Vector3.Distance(playerTransform.position, sheepTransform.position);
        if (!IsTheDistanceRight(dist))
        {
            if(dist < minMaxDistanceState.x)
            {
                stationStateSwitcher.SwitchState<HorrorState>();
                return;
            }

            if (dist > minMaxDistanceState.y)
            {
                stationStateSwitcher.SwitchState<CalmState>();
                return;
            }
        }

        if (!AgentReachedToThePoint()) return;

        SetDestination();
    }

    protected override void SetDestination()
    {
        var position = sheepTransform.position;
        var direction = position - playerTransform.position;

        Vector3 destinationPoint;
        var attemptCount = 0;
        do
        {
            Physics.Raycast(position, direction, out var hit);
            destinationPoint = hit.point;

            if (++attemptCount != ATTEMPT_LIMIT) continue;
            Debug.LogError($"Sheep has tried {ATTEMPT_LIMIT} times without success to find its way in CalmState mode");
            break;
        } while (!CanWalkTo(destinationPoint));

        navMeshAgent.SetDestination(destinationPoint);
    }
}