using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class SheepMover : MonoBehaviour, IStationStateSwitcher
{
    [Header("Speed")] 
    [SerializeField] private float calmSpeed = 0.5f;
    [SerializeField] private float escapeSpeed = 4;
    [SerializeField] private float horrorSpeed = 6;

    [Header("Distance")] 
    [SerializeField] private float horrorDistance = 10;
    [SerializeField] private float escapeDistance = 15;
    [SerializeField] private float calmDistance = 20;

    [Header("Time in seconds")] 
    [SerializeField] private float changeDestinationSecondsOnCalm = 1;
    [SerializeField] private float changeDestinationSecondsOnEscape = 0.5f;
    [SerializeField] private float changeDestinationSecondsOnHorror = 2;


    private BaseSheepState[] _allBaseSheepStates;

    private BaseSheepState _currentSheepState;

    private Transform[] _escapePointsOnHorror;

    private NavMeshAgent _navMeshAgent;
    
    private Transform _player;


    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        var currentTransform = transform;

        _allBaseSheepStates = new BaseSheepState[]
        {
            new CalmState(this, 4, calmSpeed, changeDestinationSecondsOnCalm, currentTransform, _player, _navMeshAgent,
                new Vector2(calmDistance, int.MaxValue), _allBaseSheepStates),

            new EscapeState(this, currentTransform, escapeSpeed, changeDestinationSecondsOnEscape, _player,
                _navMeshAgent, new Vector2(horrorDistance, escapeDistance), _allBaseSheepStates),

            new HorrorState(this, currentTransform, horrorSpeed, changeDestinationSecondsOnHorror, _player,
                _escapePointsOnHorror, _navMeshAgent, new Vector2(-1, horrorDistance), _allBaseSheepStates)
        };

        _currentSheepState = _allBaseSheepStates[0];
        _currentSheepState.StartState();
    }

    public void SwitchState<T>() where T : BaseSheepState
    {
        _currentSheepState.StopState();
        _currentSheepState = _allBaseSheepStates.FirstOrDefault(state => state is T);
        _currentSheepState.StartState();
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