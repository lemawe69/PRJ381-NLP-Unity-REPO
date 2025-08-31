using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PictureButton : MonoBehaviour
{
    public Button captureButton;

    private string folderName = "Pictures";
    private string folderPath;

    void Start()
    {
        folderPath = Path.Combine(Application.dataPath, folderName);

        // Create folder if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Created folder: " + folderPath);
        }

        // Assign the button click event
        if (captureButton != null)
        {
            captureButton.onClick.AddListener(TakeScreenshot);
        }
    }

    void TakeScreenshot()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filename = $"Picture_{timestamp}.png";
        string fullPath = Path.Combine(folderPath, filename);

        ScreenCapture.CaptureScreenshot(fullPath);
        Debug.Log("Picture saved to: " + fullPath);
    }
}