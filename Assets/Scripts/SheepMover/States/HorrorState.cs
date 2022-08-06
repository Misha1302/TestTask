using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HorrorState : BaseSheepState
{
    private readonly Transform[] _escapePointsOnHorror;

    private readonly string _playerTag;


    public HorrorState(Transform sheepTransform, Transform playerTransform, Vector2 minMaxDistanceState,
        IStationStateSwitcher stationStateSwitcher, float speed, NavMeshAgent navMeshAgent,
        Transform[] escapePointsOnHorror)
        : base(sheepTransform, playerTransform, minMaxDistanceState, stationStateSwitcher, speed, navMeshAgent)
    {
        _escapePointsOnHorror = escapePointsOnHorror;
        _playerTag = playerTransform.tag;
    }


    public override void Update()
    {
        if (!AgentReachedToThePoint())
        {
            CheckingThePlayerOnTheWay();
            return;
        }

        var dist = Vector3.Distance(playerTransform.position, sheepTransform.position);
        if (dist > minMaxDistanceState.y)
        {
            stationStateSwitcher.SwitchState<EscapeState>();
            return;
        }


        SetDestination();
    }

    private void CheckingThePlayerOnTheWay()
    {
        if (!Physics.Linecast(sheepTransform.position, navMeshAgent.destination, out var hit)) return;

        if (hit.transform.CompareTag(_playerTag))
            SetDestination();
    }

    protected override Vector3 GetDestination()
    {
        var nearestPoints = GetNearestPoints();

        return nearestPoints[1].escapePointOnHorror.position;
    }

    private (float distance, Transform escapePointOnHorror)[] GetNearestPoints()
    {
        var nearestPointsList = (from escapePointOnHorror in _escapePointsOnHorror
            let distance = Vector3.Distance(escapePointOnHorror.position, sheepTransform.position)
            select (distance, escapePointOnHorror)).ToArray();

        for (var j = 1; j < nearestPointsList.Length; j++)
        for (var i = 1; i < nearestPointsList.Length; i++)
            if (nearestPointsList[i - 1].distance > nearestPointsList[i].distance)
                (nearestPointsList[i - 1], nearestPointsList[i]) = (nearestPointsList[i], nearestPointsList[i - 1]);

        return nearestPointsList;
    }
}