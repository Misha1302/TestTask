using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class SheepMover : MonoBehaviour, IStationStateSwitcher
{
    [Header("Speed")] 
    [SerializeField] private float calmSpeed = 0.75f;
    [SerializeField] private float escapeSpeed = 4;
    [SerializeField] private float horrorSpeed = 8;

    [Header("Trigger distance")] 
    [SerializeField] private float triggerHorrorDistance = 10;
    [SerializeField] private float triggerEscapeDistance = 15;
    [SerializeField] private float triggerCalmDistance = 20;

    [Header("Other")] 
    [SerializeField] private float calmDistanceWalk = 2f;


    private BaseSheepState[] _allBaseSheepStates;
    private BaseSheepState _currentSheepState;

    private Transform[] _escapePointsOnHorror;
    private Transform _player;

    private NavMeshAgent _navMeshAgent;



    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _navMeshAgent.stoppingDistance = 1;
        _navMeshAgent.SetDestination(transform.position);

        var currentTransform = transform;

        print(3.40282347E+38f);
        _allBaseSheepStates = new BaseSheepState[]
        {
            new CalmState(this, calmDistanceWalk, calmSpeed, currentTransform, _player, _navMeshAgent,
                new Vector2(triggerCalmDistance, float.MaxValue)),

            new EscapeState(this, currentTransform, escapeSpeed, _player,
                _navMeshAgent, new Vector2(triggerHorrorDistance, triggerEscapeDistance)),

            new HorrorState(this, currentTransform, horrorSpeed, _player,
                _escapePointsOnHorror, _navMeshAgent, new Vector2(-1, triggerHorrorDistance))
        };

        foreach (var baseSheepState in _allBaseSheepStates) baseSheepState.SetAllSheepStates(_allBaseSheepStates);

        _currentSheepState = _allBaseSheepStates[0];
    }

    private void Update()
    {
        _currentSheepState.Go();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, _navMeshAgent.destination);
    }

    public void SwitchState(BaseSheepState sheepState)
    {
        _currentSheepState.StopState();
        _currentSheepState = sheepState;
        _currentSheepState.Go();
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