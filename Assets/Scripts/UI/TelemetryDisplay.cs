using UnityEngine;
using TMPro;

public class TelemetryDisplay : MonoBehaviour
{
    public TMP_Text batteryText;
    public TMP_Text gpsText;

    private float battery = 89f;
    private Vector3 gps = new Vector3(12.345f, 0, 67.890f);

    void Update()
    {
        battery -= Time.deltaTime * 0.05f;
        batteryText.text = $"Battery: {battery:F1}%";
        gpsText.text = $"GPS: {gps.x:F3}, {gps.z:F3}";
    }
}