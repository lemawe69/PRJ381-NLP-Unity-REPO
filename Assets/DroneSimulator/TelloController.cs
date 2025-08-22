using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TelloController : MonoBehaviour
{
    [Header("Drone Settings")]
    public float moveSpeed = 3f;
    public float liftSpeed = 2f;
    public float rotationSpeed = 60f;
    public float takeOffHeight = 2f;
    public float defaultMoveDistance = 1f; // fallback if no distance is specified

    private Rigidbody rb;
    private bool isFlying = false;
    private bool isEmergencyStopped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // start without gravity
    }

    void FixedUpdate()
    {
        if (isFlying && !isEmergencyStopped)
        {
            // Basic hover simulation
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // --- Command Methods ---
    public void TakeOff()
    {
        Debug.Log("Drone taking off...");
        isFlying = true;
        isEmergencyStopped = false;
        rb.useGravity = false;

        Vector3 target = new Vector3(transform.position.x, takeOffHeight, transform.position.z);
        StartCoroutine(LerpToHeight(target, liftSpeed));
    }

    public void Land()
    {
        Debug.Log("Drone landing...");
        isFlying = false;
        rb.useGravity = true;
    }

    public void EmergencyStop()
    {
        Debug.Log("Emergency stop triggered!");
        isFlying = false;
        isEmergencyStopped = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void Move(string direction, float distance)
    {
        if (isEmergencyStopped) return;

        Vector3 moveDir = DirectionToVector(direction);
        Vector3 move = moveDir * distance;
        rb.MovePosition(rb.position + move);
        Debug.Log($"Drone moving {direction} by {distance} meters");
    }

    public void Rotate(string direction, int degrees)
    {
        if (isEmergencyStopped) return;

        float dir = direction == "cw" ? 1f : -1f;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, degrees * dir, 0f));
        Debug.Log($"Drone rotating {direction} {degrees}�");
    }

    public void Flip(string direction)
    {
        if (isEmergencyStopped) return;

        Debug.Log($"Drone flipping {direction} (simulated)");
        // Simple flip animation using rotation
        StartCoroutine(DoFlip(direction));
    }

    public void SetSpeed(int speed)
    {
        moveSpeed = Mathf.Clamp(speed / 50f, 0.5f, 10f);
        Debug.Log($"Drone speed set to {speed} (scaled: {moveSpeed})");
    }

    // --- Helpers ---
    private Vector3 DirectionToVector(string dir)
    {
        switch (dir)
        {
            case "forward": return transform.forward;
            case "back": return -transform.forward;
            case "left": return -transform.right;
            case "right": return transform.right;
            case "up": return Vector3.up;
            case "down": return Vector3.down;
            default: return Vector3.zero;
        }
    }

    private System.Collections.IEnumerator LerpToHeight(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("Drone reached takeoff height, now hovering.");
    }

    private System.Collections.IEnumerator DoFlip(string direction)
    {
        float flipAngle = 360f;
        Vector3 axis = Vector3.forward;

        if (direction == "f") axis = transform.right;      // front flip
        else if (direction == "b") axis = -transform.right; // back flip
        else if (direction == "l") axis = transform.forward; // left roll
        else if (direction == "r") axis = -transform.forward; // right roll

        float rotated = 0f;
        while (rotated < flipAngle)
        {
            float step = 720f * Time.deltaTime; // flip speed
            rb.MoveRotation(rb.rotation * Quaternion.Euler(axis * step));
            rotated += step;
            yield return null;
        }

        Debug.Log("Flip complete!");
    }
}
