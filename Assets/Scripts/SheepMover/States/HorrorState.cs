using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HorrorState : BaseSheepState
{
    private readonly Transform[] _escapePointsOnHorror;

    public HorrorState(Transform sheepTransform, Transform playerTransform, (float minDistance, float maxDistance) minMaxDistanceState,
        IStationStateSwitcher stationStateSwitcher, float speed, NavMeshAgent navMeshAgent,
        Transform[] escapePointsOnHorror)
        : base(sheepTransform, playerTransform, minMaxDistanceState, stationStateSwitcher, speed, navMeshAgent)
    {
        _escapePointsOnHorror = escapePointsOnHorror;
    }


    public override void Update()
    {
        if (!AgentReachedToThePoint())
        {
            CheckingThePlayerOnTheWay();
            return;
        }

        var dist = Vector3.Distance(_playerTransform.position, _sheepTransform.position);
        if (dist > _minMaxDistanceState.maxDistance)
        {
            _stationStateSwitcher.SwitchState<EscapeState>();
            return;
        }


        SetDestination();
    }

    private void CheckingThePlayerOnTheWay()
    {
        if (!Physics.Linecast(_sheepTransform.position, _navMeshAgent.destination, out var hit)) return;

        if (hit.transform.TryGetComponent<PlayerMover>(out _))
            SetDestination();
    }

    protected override Vector3 GetDestination()
    {
        var nearestPoints = _escapePointsOnHorror
            .OrderBy(x => Vector3.Distance(_sheepTransform.position, x.position)).ToArray();
        return nearestPoints[Random.Range(1, nearestPoints.Length - 1)].position;
    }
}