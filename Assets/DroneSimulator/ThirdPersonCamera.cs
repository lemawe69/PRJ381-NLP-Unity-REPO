using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;        // Assign the drone object here
    public Vector3 offset = new Vector3(0f, 3f, -6f); // Camera offset
    public float smoothSpeed = 5f;  // Camera follow speed

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Always look at the drone
        transform.LookAt(target);
    }
}

