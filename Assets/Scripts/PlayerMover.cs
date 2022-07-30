using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float maxVelocity = 2;
    [SerializeField] private float minVelocity = -2;
    [SerializeField] private float speed = 2;
    private Rigidbody _playerRigidbody;

    private void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");
        _playerRigidbody.AddForce(transform.forward * (speed * vertical * Time.deltaTime));
        _playerRigidbody.AddForce(transform.right * (speed * horizontal * Time.deltaTime));

        if (_playerRigidbody.velocity.magnitude >= maxVelocity)
            _playerRigidbody.velocity = _playerRigidbody.velocity.normalized * maxVelocity;
        else if (_playerRigidbody.velocity.magnitude <= minVelocity)
            _playerRigidbody.velocity = _playerRigidbody.velocity.normalized * minVelocity;
    }
}