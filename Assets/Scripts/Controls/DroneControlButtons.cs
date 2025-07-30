using UnityEngine;

public class DroneControlButtons : MonoBehaviour
{
    public void Takeoff()
    {
        Debug.Log("Drone taking off...");
    }

    public void Land()
    {
        Debug.Log("Drone landing...");
    }

    public void Scan()
    {
        Debug.Log("Scanning...");
    }
}