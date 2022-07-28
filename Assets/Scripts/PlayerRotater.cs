using System;
using UnityEngine;

public class PlayerRotater : MonoBehaviour
{
    [SerializeField] private float rotateSpeedX;

    private bool hideCursor = true;

    private void Start()
    {
        HideOrShowCursor(hideCursor);
    }

    private void Update()
    {
        var mouseX = Input.GetAxis("Mouse X");
        var rotateVector = new Vector3(0, mouseX, 0);

        transform.Rotate(rotateVector * (rotateSpeedX * Time.deltaTime));

        
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        
        hideCursor = !hideCursor;
        HideOrShowCursor(hideCursor);
    }

    private static void HideOrShowCursor(bool hideCursor)
    {
        if (hideCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}