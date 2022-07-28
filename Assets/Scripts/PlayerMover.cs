using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float maxVelocity = 2;
    [SerializeField] private float minVelocity = -2;
    [SerializeField] private float speed = 2;
    private Rigidbody playerRigidbody;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");
        playerRigidbody.AddForce(transform.forward * (speed * vertical * Time.deltaTime));
        playerRigidbody.AddForce(transform.right * (speed * horizontal * Time.deltaTime));

        if (playerRigidbody.velocity.magnitude >= maxVelocity)
            playerRigidbody.velocity = playerRigidbody.velocity.normalized * maxVelocity;
        else if (playerRigidbody.velocity.magnitude <= minVelocity)
            playerRigidbody.velocity = playerRigidbody.velocity.normalized * minVelocity;
    }
}