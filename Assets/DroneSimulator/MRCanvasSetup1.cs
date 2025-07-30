using UnityEngine;
using TMPro;

public class MRCanvasSetup : MonoBehaviour
{
    public Canvas canvas;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI alertText;
    public TextMeshProUGUI confidenceText;

    void Start()
    {
        SetupCanvas();
        SetDefaultUI();
    }

    void SetupCanvas()
    {
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 400);
            canvas.transform.position = new Vector3(0, 1.5f, 2);
            canvas.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void SetDefaultUI()
    {
        if (statusText) statusText.text = "Status: Waiting...";
        if (positionText) positionText.text = "Coordinates: N/A";
        if (alertText) alertText.text = "Alerts: None";
        if (confidenceText) confidenceText.text = "Detection Confidence: --%";
    }
}
