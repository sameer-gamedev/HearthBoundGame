using UnityEngine;

public class RepairIconAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform iconVisual;       // the hammer mesh transform
    [SerializeField] private Camera targetCamera;        // main camera

    [Header("Billboard")]
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private bool lockX = true;          // keep upright
    [SerializeField] private bool lockZ = true;

    [Header("Motion")]
    [SerializeField] private float bobHeight = 0.15f;    // world units
    [SerializeField] private float bobSpeed = 2.0f;      // cycles per second-ish
    [SerializeField] private float pulseScale = 0.12f;   // 0.12 = +/-12%
    [SerializeField] private float pulseSpeed = 2.0f;
    [SerializeField] private float rotateSpeed = 0f;     // degrees/sec (0 = off)

    [Header("Visibility (Optional)")]
    [SerializeField] private bool hideWhenNotInRange = false;
    [SerializeField] private float showRange = 2.5f;
    [SerializeField] private Transform player;           // assign player transform

    private Vector3 _startLocalPos;
    private Vector3 _startLocalScale;

    private void Awake()
    {
        if (iconVisual == null) iconVisual = transform;
        if (targetCamera == null) targetCamera = Camera.main;

        _startLocalPos = iconVisual.localPosition;
        _startLocalScale = iconVisual.localScale;
    }

    private void Update()
    {
        // Optional: hide unless player is close
        if (hideWhenNotInRange && player != null)
        {
            float d = Vector3.Distance(player.position, transform.position);
            bool shouldShow = d <= showRange;
            if (iconVisual.gameObject.activeSelf != shouldShow)
                iconVisual.gameObject.SetActive(shouldShow);

            if (!shouldShow) return;
        }

        float t = Time.time;

        // Bob (up/down)
        float bob = Mathf.Sin(t * bobSpeed) * bobHeight;
        iconVisual.localPosition = _startLocalPos + Vector3.up * bob;

        // Pulse (scale)
        float pulse = 1f + Mathf.Sin(t * pulseSpeed) * pulseScale;
        iconVisual.localScale = _startLocalScale * pulse;

        // Rotate (optional)
        if (rotateSpeed != 0f)
            iconVisual.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self);

        // Billboard (face camera)
        if (faceCamera && targetCamera != null)
        {
            Vector3 camPos = targetCamera.transform.position;
            Vector3 lookDir = iconVisual.position - camPos; // face camera
            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Quaternion rot = Quaternion.LookRotation(lookDir, Vector3.up);

                Vector3 e = rot.eulerAngles;
                if (lockX) e.x = 0f;
                if (lockZ) e.z = 0f;

                iconVisual.rotation = Quaternion.Euler(e);
            }
        }
    }
}
