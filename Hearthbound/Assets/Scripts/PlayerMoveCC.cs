using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveCC : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;
    public float sprintSpeed = 9f;

    [Header("Look")]
    public Transform cameraPivot;         // assign CameraPivot
    public float mouseSensitivity = 2f;
    public float pitchMin = -80f;
    public float pitchMax = 80f;

    [Header("Gravity / Jump")]
    public float gravity = -20f;
    public float jumpHeight = 1.2f;

    private CharacterController cc;
    private float yaw;
    private float pitch;
    private float verticalVel;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (cameraPivot == null)
            Debug.LogError("Assign cameraPivot (e.g., Player/CameraPivot).");
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = cameraPivot ? cameraPivot.localEulerAngles.x : 0f;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        float mx = Input.GetAxisRaw("Mouse X");
        float my = Input.GetAxisRaw("Mouse Y");

        yaw += mx * mouseSensitivity;
        pitch -= my * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraPivot)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        // WASD + Arrow keys via default "Horizontal"/"Vertical" axes
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z);
        if (move.sqrMagnitude > 1f) move.Normalize();

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        Vector3 horizontal = move * speed;

        // Ground check & gravity
        if (cc.isGrounded && verticalVel < 0f)
            verticalVel = -2f; // small stick-to-ground force

        // Jump
        if (cc.isGrounded && Input.GetButtonDown("Jump"))
            verticalVel = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVel += gravity * Time.deltaTime;

        Vector3 velocity = horizontal + Vector3.up * verticalVel;
        cc.Move(velocity * Time.deltaTime);
    }
}
