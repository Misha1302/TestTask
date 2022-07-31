using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class SheepMover : MonoBehaviour, IStationStateSwitcher
{
    [Header("Speed")] [SerializeField] private float calmSpeed = 0.5f;

    [SerializeField] private float escapeSpeed = 4;
    [SerializeField] private float horrorSpeed = 6;

    [Header("Distance")] [SerializeField] private float horrorDistance = 10;

    [SerializeField] private float alertDistance = 15;
    [SerializeField] private float calmDistance = 20;

    [Header("Time in seconds")] [SerializeField]
    private float changeDestinationSecondsOnCalm = 1;

    [SerializeField] private float changeDestinationSecondsOnEscape = 0.5f;
    [SerializeField] private float changeDestinationSecondsOnHorror = 2;


    private BaseSheepState[] _baseSheepStates;

    private Transform[] _escapePointsOnHorror;

    private NavMeshAgent _navMeshAgent;
    private Transform _player;

    private SheepState _sheepState;


    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _sheepState = SheepState.Calm;
        
        var currentTransform = transform;
        
        print($"c: {calmDistance} - {int.MaxValue}");
        print($"a: {horrorDistance} - {alertDistance}");
        print($"h: {-1} - {horrorDistance}");
        
        _baseSheepStates = new BaseSheepState[]
        {
            new CalmState(this, 4, currentTransform, _player, _navMeshAgent,
                new Vector2(calmDistance, int.MaxValue)),
            new EscapeState(this, currentTransform, _player, _navMeshAgent,
                new Vector2(horrorDistance, alertDistance)),
            new HorrorState(this, currentTransform, _player, _escapePointsOnHorror, _navMeshAgent,
                new Vector2(-1, horrorDistance))
        };
        
        StartCoroutine(SlowFixedUpdate());
    }

    public void SetState(SheepState sheepState)
    {
        _sheepState = sheepState;
    }

    public void SetPlayer(PlayerMover playerMover)
    {
        _player = playerMover.transform;
    }

    public void SetEscapePointsOnHorror(Transform[] escapePointsOnHorror)
    {
        _escapePointsOnHorror = escapePointsOnHorror;
    }

    private IEnumerator SlowFixedUpdate()
    {
        while (true)
        {
            float waitSeconds = -1;

            switch (_sheepState)
            {
                case SheepState.Calm:
                    waitSeconds = changeDestinationSecondsOnCalm;
                    break;
                case SheepState.Escape:
                    waitSeconds = changeDestinationSecondsOnEscape;
                    break;
                case SheepState.Horror:
                    waitSeconds = changeDestinationSecondsOnHorror;
                    break;
                default:
                    Debug.LogException(new Exception($"{_sheepState} not found"));
                    break;
            }

            yield return new WaitForSeconds(waitSeconds);

            foreach (var baseSheepState in _baseSheepStates) baseSheepState.SetState();

            SetCurrentSheepSpeed();
        }
    }

    private void SetCurrentSheepSpeed()
    {
        switch (_sheepState)
        {
            case SheepState.Calm:
                _navMeshAgent.speed = calmSpeed;
                break;
            case SheepState.Escape:
                _navMeshAgent.speed = escapeSpeed;
                break;
            case SheepState.Horror:
                _navMeshAgent.speed = horrorSpeed;
                break;
        }
    }
}