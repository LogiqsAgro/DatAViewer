using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMap : MonoBehaviour
{
    private const int MousePrimaryButton = 0;
    private const int MouseSecondaryButton = 1;
    private const int MouseMiddleButton = 2;

    public KeyCode Forward;
    public KeyCode Back;
    public KeyCode Left;
    public KeyCode Right;
    public KeyCode Up;
    public KeyCode Down;
    public KeyCode Sprint;

    public KeyCode UnlockCursor;

    public KeyCode SettingsMenu;

    public int MouseForward;
    public int MouseLockCursor;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    void Reset()
    {
        Forward = KeyCode.W;
        Back = KeyCode.S;
        Right = KeyCode.D;
        Left = KeyCode.A;
        Up = KeyCode.Space;
        Down = KeyCode.LeftControl;
        Sprint = KeyCode.LeftShift;
        UnlockCursor = KeyCode.Escape;
        SettingsMenu = KeyCode.F7;
        MouseLockCursor = MousePrimaryButton;
        MouseForward = MouseSecondaryButton;
    }
}
