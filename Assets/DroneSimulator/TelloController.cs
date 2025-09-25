using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class TelloController : MonoBehaviour
{
    [Header("Drone Settings")]
    public float moveSpeed = 1f;        // meters per second
    public float liftSpeed = 1f;
    public float rotationSpeed = 60f;
    public float takeOffHeight = 2f;
    public float defaultMoveDistance = 1f; // fallback if no distance

    private Rigidbody rb;
    private bool isFlying = false;
    private bool isEmergencyStopped = false;
    private Coroutine moveRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // start without gravity
    }

    void FixedUpdate()
    {
        if (isFlying && !isEmergencyStopped)
        {
            // Keep stable orientation
            rb.angularVelocity = Vector3.zero;
        }
    }

    // === Command Methods ===
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

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);
    }

    public void Move(string direction, float distance)
    {
        if (isEmergencyStopped || !isFlying) return;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        Vector3 moveDir = DirectionToVector(direction).normalized;
        Vector3 target = rb.position + moveDir * distance;

        moveRoutine = StartCoroutine(MoveToTarget(target));
        Debug.Log($"Drone moving {direction} by {distance} meters");
    }

    public void Rotate(string direction, int degrees)
    {
        if (isEmergencyStopped || !isFlying) return;

        StartCoroutine(RotateSmooth(direction, degrees));
    }

    public void Flip(string direction)
    {
        if (isEmergencyStopped || !isFlying) return;
        Debug.Log($"Drone flipping {direction} (simulated)");
        StartCoroutine(DoFlip(direction));
    }

    public void SetSpeed(int speed)
    {
        moveSpeed = Mathf.Clamp(speed, 0.5f, 20f);
        Debug.Log($"Drone speed set to {speed} (scaled: {moveSpeed})");
    }

    // === Helpers ===
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

    private IEnumerator MoveToTarget(Vector3 target)
    {
        while (Vector3.Distance(rb.position, target) > 0.05f && !isEmergencyStopped)
        {
            Vector3 newPos = Vector3.MoveTowards(rb.position, target, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("Drone reached target, now hovering.");
    }

    private IEnumerator LerpToHeight(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("Drone reached takeoff height, now hovering.");
    }

    private IEnumerator RotateSmooth(string direction, int degrees)
    {
        float dir = direction == "cw" ? 1f : -1f;
        float rotated = 0f;
        while (rotated < degrees)
        {
            float step = rotationSpeed * Time.deltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, dir * step, 0f));
            rotated += step;
            yield return null;
        }
        Debug.Log($"Drone rotation {direction} complete.");
    }

    private IEnumerator DoFlip(string direction)
    {
        float flipAngle = 360f;
        Vector3 axis = Vector3.forward;

        if (direction == "f") axis = transform.right;
        else if (direction == "b") axis = -transform.right;
        else if (direction == "l") axis = transform.forward;
        else if (direction == "r") axis = -transform.forward;

        float rotated = 0f;
        while (rotated < flipAngle)
        {
            float step = 720f * Time.deltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(axis * step));
            rotated += step;
            yield return null;
        }
        Debug.Log("Flip complete!");
    }
}
