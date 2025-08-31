using UnityEngine;
using UnityEngine.UI;

public class SignalByManualDistance : MonoBehaviour
{
    [Header("Manual Distance Input")]
    public float distance = 0f;

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