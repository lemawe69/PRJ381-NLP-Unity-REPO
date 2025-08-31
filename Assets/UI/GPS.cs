using UnityEngine;
using System.Collections;

public class GPS : MonoBehaviour
{
    public static GPS Instance { set; get; }

    public float latitude;
    public float longitude;

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        // Check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Location services not enabled by user.");
            yield break;
        }

        // Start location service
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            Debug.Log("Waiting for location service to initialize...");
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Timeout
        if (maxWait <= 0)
        {
            Debug.LogError("Location service initialization timed out.");
            yield break;
        }

        // Failed to start
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Location service failed to start.");
            yield break;
        }

        // Success
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        Debug.Log($"Location acquired: Latitude = {latitude}, Longitude = {longitude}");
    }
}