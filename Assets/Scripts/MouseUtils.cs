using UnityEngine;


public static class MouseUtils
{
     /// <summary> Returns the ray from the user's eye through the mouse pointer </summay>
    public static Ray GetMouseRay(Camera camera)
    {
        var mousePosition = Cursor.lockState == CursorLockMode.Locked
                 ? new Vector3(camera.pixelWidth, camera.pixelHeight, 0) * 0.5f
                 : Input.mousePosition;

        Ray ray = camera.ScreenPointToRay(mousePosition);
        return ray;

    }
}
