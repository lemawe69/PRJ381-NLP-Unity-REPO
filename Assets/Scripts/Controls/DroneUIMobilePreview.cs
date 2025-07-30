using UnityEngine;
using UnityEngine.UI;

public class DroneUIMobilePreview : MonoBehaviour
{
    public Canvas canvas;
    public RenderTexture droneFeed;

    private Text alertText;
    private GameObject alertTextObj;
    private Button toggleAlertButton;
    private bool isAlertVisible = false;

    void Start()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("MobileUICanvas");
        canvasObj.layer = LayerMask.NameToLayer("UI");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Background panel
        GameObject bgPanel = CreateUIObject("BGPanel", canvas.transform);
        Image bgImage = bgPanel.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f);
        StretchRectTransform(bgPanel.GetComponent<RectTransform>());

        // Drone camera feed
        GameObject droneFeedObj = CreateUIObject("DroneFeed", canvas.transform);
        RawImage rawImage = droneFeedObj.AddComponent<RawImage>();
        rawImage.texture = droneFeed;
        RectTransform feedRect = droneFeedObj.GetComponent<RectTransform>();
        feedRect.anchorMin = new Vector2(0.05f, 0.3f);
        feedRect.anchorMax = new Vector2(0.95f, 0.85f);
        feedRect.offsetMin = Vector2.zero;
        feedRect.offsetMax = Vector2.zero;

        // Top bar telemetry text
        GameObject telemetryTextObj = CreateUIObject("TelemetryText", canvas.transform);
        Text telemetryText = telemetryTextObj.AddComponent<Text>();
        telemetryText.text = "Battery: 89%  |  Signal: Strong  |  GPS: Locked";
        telemetryText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        telemetryText.fontSize = 32;
        telemetryText.color = Color.green;
        telemetryText.alignment = TextAnchor.MiddleCenter;
        RectTransform telemetryRect = telemetryTextObj.GetComponent<RectTransform>();
        telemetryRect.anchorMin = new Vector2(0.05f, 0.9f);
        telemetryRect.anchorMax = new Vector2(0.95f, 0.97f);
        telemetryRect.offsetMin = Vector2.zero;
        telemetryRect.offsetMax = Vector2.zero;

        // Bottom alert box (toggleable)
        alertTextObj = CreateUIObject("AlertText", canvas.transform);
        alertText = alertTextObj.AddComponent<Text>();
        alertText.text = "Warning: Pothole detected 2.5m ahead";
        alertText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        alertText.fontSize = 28;
        alertText.color = Color.red;
        alertText.alignment = TextAnchor.MiddleCenter;
        RectTransform alertRect = alertTextObj.GetComponent<RectTransform>();
        alertRect.anchorMin = new Vector2(0.05f, 0.05f);
        alertRect.anchorMax = new Vector2(0.95f, 0.15f);
        alertRect.offsetMin = Vector2.zero;
        alertRect.offsetMax = Vector2.zero;

        alertTextObj.SetActive(false); // Start hidden

        // Toggle Alert Button
        GameObject buttonObj = CreateUIObject("ToggleAlertButton", canvas.transform);
        toggleAlertButton = buttonObj.AddComponent<Button>();
        Image btnImage = buttonObj.AddComponent<Image>();
        btnImage.color = Color.gray;
        RectTransform btnRect = buttonObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.35f, 0.17f);
        btnRect.anchorMax = new Vector2(0.65f, 0.22f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        GameObject btnTextObj = CreateUIObject("ButtonText", buttonObj.transform);
        Text btnText = btnTextObj.AddComponent<Text>();
        btnText.text = "Toggle Alert";
        btnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        btnText.fontSize = 24;
        btnText.color = Color.white;
        btnText.alignment = TextAnchor.MiddleCenter;
        StretchRectTransform(btnTextObj.GetComponent<RectTransform>());

        toggleAlertButton.onClick.AddListener(ToggleAlert);
    }

    GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.AddComponent<RectTransform>();
        return obj;
    }

    void StretchRectTransform(RectTransform rect)
    {
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    public void ShowAlert(string message)
    {
        if (alertText != null && alertTextObj != null)
        {
            alertText.text = message;
            alertTextObj.SetActive(true);
        }
    }

    public void HideAlert()
    {
        if (alertTextObj != null)
        {
            alertTextObj.SetActive(false);
        }
    }

    private void ToggleAlert()
    {
        isAlertVisible = !isAlertVisible;
        if (isAlertVisible)
        {
            ShowAlert("Warning: Pothole detected 2.5m ahead");
        }
        else
        {
            HideAlert();
        }
    }
}

