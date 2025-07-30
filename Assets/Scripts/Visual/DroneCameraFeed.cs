using UnityEngine;

public class DroneCameraFeed : MonoBehaviour
{
    public RenderTexture droneFeedTexture;

    void Start()
    {
        GetComponent<Camera>().targetTexture = droneFeedTexture;
    }
}