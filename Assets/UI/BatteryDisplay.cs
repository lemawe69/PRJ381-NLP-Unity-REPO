using UnityEngine;
using UnityEngine.UI;

public class BatteryDisplay : MonoBehaviour
{
    public Image batteryImage;
    public Sprite BatteryFull;
    public Sprite BatteryMedium;
    public Sprite BatteryLow;
    public Sprite BatteryEmpty;

    [Range(0, 100)]
    public int batteryLevel = 100;
    public float drainRate = 1f; // percent per second

    void Start()
    {
        InvokeRepeating(nameof(DrainBattery), 1f, 1f);
    }

    void DrainBattery()
    {
        batteryLevel = Mathf.Max(batteryLevel - (int)drainRate, 0);
        UpdateBatteryIcon();
    }

    void UpdateBatteryIcon()
    {
        if (batteryLevel > 75)
            batteryImage.sprite = BatteryFull;
        else if (batteryLevel > 50)
            batteryImage.sprite = BatteryMedium;
        else if (batteryLevel > 25)
            batteryImage.sprite = BatteryLow;
        else
            batteryImage.sprite = BatteryEmpty;
    }
}