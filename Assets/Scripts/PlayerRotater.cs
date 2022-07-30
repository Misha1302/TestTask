using UnityEngine;

public class PlayerRotater : MonoBehaviour
{
    [SerializeField] private float rotateSpeedX;

    private void Update()
    {
        var mouseX = Input.GetAxis("Mouse X");
        var rotateVector = new Vector3(0, mouseX, 0);

        transform.Rotate(rotateVector * (rotateSpeedX * Time.deltaTime));
    }
}