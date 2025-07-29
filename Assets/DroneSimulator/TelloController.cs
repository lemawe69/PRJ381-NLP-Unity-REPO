using UnityEngine;

public class TelloController : MonoBehaviour
{
    [Header("Drone Settings")]
    public float moveSpeed = 3f;
    public float liftSpeed = 2f;
    public float rotationSpeed = 60f;
    public float takeOffHeight = 2f;

    private bool isTakingOff = false;
    private bool isLanding = false;
    private bool isHovering = false;
    private Vector3 targetPosition;

    void Update()
    {
        HandleTakeOff();
        HandleLanding();
        HandleMoveTo();
    }

    // --- Command Methods ---
    public void TakeOff()
    {
        Debug.Log("Drone taking off...");
        isTakingOff = true;
        isLanding = false;
        isHovering = false;
        targetPosition = Vector3.zero;
    }

    public void Land()
    {
        Debug.Log("Drone landing...");
        isLanding = true;
        isTakingOff = false;
        isHovering = false;
    }

    public void EmergencyStop()
    {
        Debug.Log("Emergency stop!");
        isTakingOff = false;
        isLanding = false;
        isHovering = true;
        targetPosition = transform.position;
    }

    public void Move(string direction, int distance)
    {
        Debug.Log($"Drone moving {direction} by {distance}cm");
        Vector3 offset = DirectionToVector(direction) * (distance / 100f); // convert cm to meters
        targetPosition = transform.position + offset;
        isHovering = false;
    }

    public void Rotate(string direction, int degrees)
    {
        Debug.Log($"Drone rotating {direction} {degrees}°");
        float dir = direction == "cw" ? 1 : -1;
        transform.Rotate(Vector3.up, dir * degrees);
    }

    public void Flip(string direction)
    {
        Debug.Log($"Drone flipping {direction} (simulated)");
        // Optional: Add animation or rotation effect
    }

    public void SetSpeed(int speed)
    {
        Debug.Log($"Drone speed set to {speed}");
        moveSpeed = speed / 50f; // scale speed
    }

    // --- Internal Behavior ---
    private void HandleTakeOff()
    {
        if (isTakingOff)
        {
            Vector3 target = new Vector3(transform.position.x, takeOffHeight, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, target, liftSpeed * Time.deltaTime);
            if (Mathf.Abs(transform.position.y - takeOffHeight) < 0.05f)
            {
                isTakingOff = false;
                isHovering = true;
                Debug.Log("Drone reached takeoff height, now hovering.");
            }
        }
    }

    private void HandleLanding()
    {
        if (isLanding)
        {
            Vector3 target = new Vector3(transform.position.x, 0.2f, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, target, liftSpeed * Time.deltaTime);
            if (transform.position.y <= 0.25f)
            {
                isLanding = false;
                isHovering = false;
                Debug.Log("Drone landed.");
            }
        }
    }

    private void HandleMoveTo()
    {
        if (targetPosition != Vector3.zero && !isTakingOff && !isLanding)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                targetPosition = Vector3.zero;
                isHovering = true;
                Debug.Log("Drone reached target, hovering.");
            }
        }
    }

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
        }
        return Vector3.zero;
    }
}
