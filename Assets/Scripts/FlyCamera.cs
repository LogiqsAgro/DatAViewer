using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(KeyMap))]
public class FlyCamera : MonoBehaviour
{
    public float acceleration = 50; // how fast you accelerate
    public float accSprintMultiplier = 4; // how much faster you go when "sprinting"
    public float lookSensitivity = 1; // mouse look sensitivity
    public float dampingCoefficient = 5; // how quickly you break to a halt after you stop your input
    public bool focusOnEnable = true; // whether or not to focus and lock cursor immediately on enable

    Vector3 velocity; // current velocity

    KeyMap keymap;

    static bool Focused
    {
        get => Cursor.lockState == CursorLockMode.Locked;
        set
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = value == false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        keymap = GetComponent<KeyMap>();
    }

    void OnEnable()
    {
        if (focusOnEnable) Focused = true;
    }

    void OnDisable() => Focused = false;

    void Update()
    {
        // Input
        if (Focused)
        {
            UpdateInput();
        }
        else if (Input.GetMouseButtonDown(keymap.MouseLockCursor))
        {
            Focused = true;
        }

        // Physics
        velocity = Vector3.Lerp(velocity, Vector3.zero, dampingCoefficient * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
    }

    void UpdateInput()
    {
        // Position
        velocity += GetAccelerationVector() * Time.deltaTime;

        // Rotation
        Vector2 mouseDelta = lookSensitivity * new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        Quaternion rotation = transform.rotation;
        Quaternion horiz = Quaternion.AngleAxis(mouseDelta.x, Vector3.up);
        Quaternion vert = Quaternion.AngleAxis(mouseDelta.y, Vector3.right);
        transform.rotation = horiz * rotation * vert;

        // Leave cursor lock
        if (Input.GetKeyDown(keymap.UnlockCursor))
        {
            Focused = false;
        }

        if (Input.GetKeyDown(keymap.SettingsMenu))
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    Vector3 GetAccelerationVector()
    { 
        Vector3 direction;
        if (Input.GetMouseButton(keymap.MouseForward))
        {
            var cam = GetComponent<Camera>();
            direction = MouseUtils.GetMouseRay(cam).direction;
        }
        else
        {
            Vector3 dir = default;
            dir += Input.GetKey(keymap.Forward) ? Vector3.forward : default;
            dir += Input.GetKey(keymap.Back) ? Vector3.back : default;
            dir += Input.GetKey(keymap.Right) ? Vector3.right : default;
            dir += Input.GetKey(keymap.Left) ? Vector3.left : default;
            dir += Input.GetKey(keymap.Up) ? Vector3.up : default;
            dir += Input.GetKey(keymap.Down) ? Vector3.down : default;
            direction = transform.TransformVector(dir.normalized);
        }

        if (Input.GetKey(keymap.Sprint))
            return direction * (acceleration * accSprintMultiplier); // "sprinting"

        return direction * acceleration; // "walking"
    }

}