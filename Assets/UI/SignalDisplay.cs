using UnityEngine;
using UnityEngine.UI;

public class SignalByDroneMesh : MonoBehaviour
{
    [Header("Drone Mesh Reference")]
    public Transform droneMesh; // Assign the actual mesh object of the drone

    [Header("Signal Thresholds")]
    public float fullSignalRange = 20f;
    public float medSignalRange = 40f;

    [Header("UI Elements")]
    public Image signalImage;
    public Sprite FullSignal;
    public Sprite MedSignal;
    public Sprite NoSignal;

    void Update()
    {
        // Use the drone mesh's world position
        float distance = droneMesh.position.magnitude; // Distance from world origin (0,0,0)
        UpdateSignalIcon(distance);
    }

    void UpdateSignalIcon(float d)
    {
        if (d <= fullSignalRange)
        {
            signalImage.sprite = FullSignal;
        }
        else if (d <= medSignalRange)
        {
            signalImage.sprite = MedSignal;
        }
        else
        {
            signalImage.sprite = NoSignal;
        }
    }
}