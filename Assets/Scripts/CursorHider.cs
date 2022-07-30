using UnityEngine;

public class CursorHider : MonoBehaviour
{
    private bool _hideCursor = true;

    private void Start()
    {
        HideOrShowCursor(_hideCursor);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        _hideCursor = !_hideCursor;
        HideOrShowCursor(_hideCursor);
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