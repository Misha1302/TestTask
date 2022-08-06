using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class SheepMover : MonoBehaviour, IStationStateSwitcher
{
    [Header("Speed")] [SerializeField] private float calmSpeed = 0.75f;

    [SerializeField] private float escapeSpeed = 4;
    [SerializeField] private float horrorSpeed = 8;

    [Header("Trigger distance")] [SerializeField]
    private float triggerHorrorDistance = 10;

    [SerializeField] private float triggerEscapeDistance = 15;
    [SerializeField] private float triggerCalmDistance = 20;

    [Header("Other")] [SerializeField] private float calmDistanceWalk = 2f;

    private BaseSheepState[] _allBaseSheepStates;
    private BaseSheepState _currentSheepState;

    private Transform[] _escapePointsOnHorror;

    private NavMeshAgent _navMeshAgent;
    private Transform _player;


    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _navMeshAgent.stoppingDistance = 1;
        _navMeshAgent.SetDestination(transform.position);

        var currentTransform = transform;

        _allBaseSheepStates = new BaseSheepState[]
        {
            new CalmState(currentTransform, _player, new Vector2(triggerCalmDistance, float.MaxValue), this, calmSpeed,
                _navMeshAgent, calmDistanceWalk),

            new EscapeState(currentTransform, _player, new Vector2(triggerEscapeDistance, triggerCalmDistance), this,
                escapeSpeed, _navMeshAgent),

            new HorrorState(currentTransform, _player, new Vector2(-1, triggerHorrorDistance), this,
                horrorSpeed, _navMeshAgent, _escapePointsOnHorror)
        };

        _currentSheepState = _allBaseSheepStates[0];
        _currentSheepState.Start();
    }

    private void Update()
    {
        _currentSheepState.Update();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, _navMeshAgent.destination);
    }

    public void SwitchState<T>() where T : BaseSheepState
    {
        _currentSheepState.Stop();
        _currentSheepState = _allBaseSheepStates.First(state => state is T);
        _currentSheepState.Start();
    }


    public void SetPlayer(PlayerMover playerMover)
    {
        _player = playerMover.transform;
    }

    public void SetEscapePointsOnHorror(Transform[] escapePointsOnHorror)
    {
        _escapePointsOnHorror = escapePointsOnHorror;
    }
}